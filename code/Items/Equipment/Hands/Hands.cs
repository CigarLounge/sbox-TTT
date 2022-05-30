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

[ClassName( "ttt_equipment_hands" )]
[HideInEditor]
[Title( "Hands" )]
public partial class Hands : Carriable
{
	public Entity GrabPoint { get; private set; }
	public const string MiddleHandsAttachment = "middle_of_both_hands";

	private IGrabbable GrabbedEntity;
	private bool IsHoldingEntity => GrabbedEntity is not null && (GrabbedEntity?.IsHolding ?? false);
	private bool IsPushingEntity = false;

	private const float MaxPickupMass = 205;
	private const float PushForce = 350f;
	private readonly Vector3 MaxPickupSize = new( 75, 75, 75 );

	public override void Simulate( Client client )
	{
		if ( !IsServer )
			return;

		using ( Prediction.Off() )
		{
			if ( Input.Pressed( InputButton.PrimaryAttack ) )
			{
				if ( IsHoldingEntity )
					GrabbedEntity?.SecondaryAction();
				else
					TryGrabEntity();
			}
			else if ( Input.Pressed( InputButton.SecondaryAttack ) )
			{
				if ( IsHoldingEntity )
					GrabbedEntity?.Drop();
				else
					PushEntity();
			}

			GrabbedEntity?.Update( Owner );
		}
	}

	private void PushEntity()
	{
		if ( IsPushingEntity )
			return;

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance )
				.EntitiesOnly()
				.Ignore( Owner )
				.Run();

		if ( !trace.Hit || !trace.Entity.IsValid() )
			return;

		IsPushingEntity = true;

		Owner.SetAnimParameter( "b_attack", true );
		Owner.SetAnimParameter( "holdtype", 4 );
		Owner.SetAnimParameter( "holdtype_handedness", 0 );

		trace.Entity.Velocity += Owner.EyeRotation.Forward * PushForce;

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

		var trace = Trace.Ray( eyePos, eyePos + eyeDir * Player.UseDistance )
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
				GrabbedEntity = new GrabbableCorpse( Owner, corpse, trace.Bone );
				break;
			case Carriable: // Ignore any size requirements, any weapon can be picked up.
				GrabbedEntity = new GrabbableProp( Owner, GrabPoint, trace.Entity as ModelEntity );
				break;
			case ModelEntity model:
				if ( !model.CollisionBounds.Size.HasGreatorOrEqualAxis( MaxPickupSize ) && model.PhysicsGroup.Mass < MaxPickupMass )
					GrabbedEntity = new GrabbableProp( Owner, GrabPoint, model );
				break;
		}
	}

	public override void OnCarryStart( Entity carrier )
	{
		base.OnCarryStart( carrier );

		if ( !Host.IsServer )
			return;

		GrabPoint = new ModelEntity( "models/hands/grabpoint.vmdl" );
		GrabPoint.EnableHideInFirstPerson = false;
		GrabPoint.SetParent( carrier, MiddleHandsAttachment, new Transform( Vector3.Zero, Rotation.FromRoll( -90 ) ) );
	}

	public override void OnCarryDrop( Entity carrier )
	{
		base.OnCarryStart( carrier );

		if ( !Host.IsServer )
			return;

		GrabbedEntity?.Drop();
		GrabPoint?.Delete();
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		GrabbedEntity?.Drop();
		base.ActiveEnd( ent, dropped );
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
