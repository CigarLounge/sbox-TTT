using System.Threading.Tasks;

using Sandbox;

namespace TTT;

public interface IGrabbable
{
	bool IsHolding { get; }
	void Drop();
	void Update( Player player );
	void SecondaryAction();
}

[Hammer.Skip]
[Library( "ttt_equipment_hands", Title = "Hands" )]
public partial class Hands : Carriable
{
	public const float MAX_INTERACT_DISTANCE = Player.USE_DISTANCE;
	public const string MIDDLE_HANDS_ATTACHMENT = "middle_of_both_hands";

	private const float MAX_PICKUP_MASS = 205;
	private readonly Vector3 MAX_PICKUP_SIZE = new( 75, 75, 75 );
	private const float PUSHING_FORCE = 350f;

	private IGrabbable GrabbedEntity;
	private bool IsHoldingEntity => GrabbedEntity is not null && (GrabbedEntity?.IsHolding ?? false);
	private bool IsPushingEntity = false;

	public override void Spawn()
	{
		base.Spawn();

		RenderColor = Color.Transparent;
	}

	public override void Simulate( Client client )
	{
		if ( !IsServer )
			return;

		using ( Prediction.Off() )
		{
			if ( Input.Pressed( InputButton.Attack1 ) )
			{
				if ( IsHoldingEntity )
				{
					GrabbedEntity?.SecondaryAction();
				}
				else
				{
					TryGrabEntity();
				}
			}
			else if ( Input.Pressed( InputButton.Attack2 ) )
			{
				if ( IsHoldingEntity )
				{
					GrabbedEntity?.Drop();
				}
				else
				{
					PushEntity();
				}
			}

			GrabbedEntity?.Update( Owner );
		}
	}

	private void PushEntity()
	{
		if ( IsPushingEntity )
			return;

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * MAX_INTERACT_DISTANCE )
				.EntitiesOnly()
				.Ignore( Owner )
				.Run();

		if ( !trace.Hit || !trace.Entity.IsValid() )
			return;

		IsPushingEntity = true;

		Owner.SetAnimParameter( "b_attack", true );
		Owner.SetAnimParameter( "holdtype", 4 );
		Owner.SetAnimParameter( "holdtype_handedness", 0 );

		trace.Entity.Velocity += Owner.EyeRotation.Forward * PUSHING_FORCE;

		_ = WaitForAnimationFinish();
	}

	private async Task WaitForAnimationFinish()
	{
		await GameTask.DelaySeconds( 0.6f );
		IsPushingEntity = false;
	}

	private void TryGrabEntity()
	{
		if ( IsHoldingEntity )
			return;

		var eyePos = Owner.EyePosition;
		var eyeDir = Owner.EyeRotation.Forward;

		var trace = Trace.Ray( eyePos, eyePos + eyeDir * MAX_INTERACT_DISTANCE )
			.UseHitboxes()
			.Ignore( Owner )
			.HitLayer( CollisionLayer.Debris )
			.EntitiesOnly()
			.Run();

		// Make sure trace is hit and not null.
		if ( !trace.Hit || !trace.Entity.IsValid() )
			return;

		// Only allow dynamic entities to be picked up.
		if ( trace.Body is null || trace.Body.BodyType == PhysicsBodyType.Keyframed || trace.Body.BodyType == PhysicsBodyType.Static )
			return;

		// Cannot pickup items held by other players.
		if ( trace.Entity.Parent is not null )
			return;

		switch ( trace.Entity )
		{
			case Corpse corpse:
				GrabbedEntity = new GrabbableCorpse( Owner, corpse, trace.Body, trace.Bone );
				break;
			case Carriable: // Ignore any size requirements, any weapon can be picked up.
				GrabbedEntity = new GrabbableProp( Owner, trace.Entity );
				break;
			case ModelEntity model:
				if ( !model.CollisionBounds.Size.HasGreatorOrEqualAxis( MAX_PICKUP_SIZE ) && model.PhysicsGroup.Mass < MAX_PICKUP_MASS )
					GrabbedEntity = new GrabbableProp( Owner, trace.Entity );
				break;
		}
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		GrabbedEntity?.Drop();

		base.ActiveEnd( ent, dropped );
	}

	protected override void OnDestroy()
	{
		GrabbedEntity?.Drop();

		base.OnDestroy();
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		if ( !IsServer )
			return;

		if ( IsPushingEntity )
			return;

		if ( IsHoldingEntity )
		{
			anim.SetAnimParameter( "holdtype", 4 );
			anim.SetAnimParameter( "holdtype_handedness", 0 );
		}
		else
		{
			anim.SetAnimParameter( "holdtype", 0 );
		}
	}

	[Event.Entity.PreCleanup]
	private void OnPreCleanup()
	{
		GrabbedEntity?.Drop();
	}
}
