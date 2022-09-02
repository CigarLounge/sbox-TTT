using System.Threading.Tasks;
using Sandbox;

namespace TTT;

public interface IGrabbable
{
	string PrimaryAttackHint { get; }
	string SecondaryAttackHint { get; }
	bool IsHolding { get; }
	Entity Drop();
	void Update( Player player );
	void SecondaryAction();
}

[ClassName( "ttt_equipment_hands" )]
[HideInEditor]
[Title( "Hands" )]
public partial class Hands : Carriable
{
	// Since everything happens on server, network the current hints
	// for the client to display.
	[Net, Local] private string CurrentPrimaryHint { get; set; }
	[Net, Local] private string CurrentSecondaryHint { get; set; }

	public override string PrimaryAttackHint => !CurrentPrimaryHint.IsNullOrEmpty() ? CurrentPrimaryHint : "Pickup";
	public override string SecondaryAttackHint => !CurrentSecondaryHint.IsNullOrEmpty() ? CurrentSecondaryHint : "Push";

	public Entity GrabPoint { get; private set; }
	public const string MiddleHandsAttachment = "middle_of_both_hands";

	private bool IsHoldingEntity => _grabbedEntity is not null && _grabbedEntity.IsHolding;
	private bool _isPushingEntity = false;
	private IGrabbable _grabbedEntity;

	private const float MaxPickupMass = 205;
	private const float PushForce = 350f;
	private readonly Vector3 _maxPickupSize = new( 75, 75, 75 );

	public override void Simulate( Client client )
	{
		if ( Host.IsClient )
			return;

		if ( Input.Pressed( InputButton.PrimaryAttack ) )
		{
			if ( IsHoldingEntity )
				_grabbedEntity.SecondaryAction();
			else
				TryGrabEntity();
		}
		else if ( Input.Pressed( InputButton.SecondaryAttack ) )
		{
			if ( IsHoldingEntity )
				_grabbedEntity.Drop();
			else
				PushEntity();
		}

		if ( _grabbedEntity is null )
			return;

		_grabbedEntity.Update( Owner );
		CurrentPrimaryHint = _grabbedEntity.PrimaryAttackHint;
		CurrentSecondaryHint = _grabbedEntity.SecondaryAttackHint;
	}

	private void PushEntity()
	{
		if ( _isPushingEntity )
			return;

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance )
				.Ignore( Owner )
				.Run();

		if ( !trace.Hit || !trace.Entity.IsValid() || trace.Entity.IsWorld )
			return;

		trace.Entity.Velocity += Owner.EyeRotation.Forward * PushForce;

		_isPushingEntity = true;
		_ = WaitForAnimationFinish();
	}

	private async Task WaitForAnimationFinish()
	{
		await GameTask.DelaySeconds( 0.6f );
		_isPushingEntity = false;
	}

	private void TryGrabEntity()
	{
		var eyePos = Owner.EyePosition;
		var eyeDir = Owner.EyeRotation.Forward;

		var trace = Trace.Ray( eyePos, eyePos + eyeDir * Player.UseDistance )
			.UseHitboxes()
			.Ignore( Owner )
			.WithAnyTags( "solid", "interactable" )
			.EntitiesOnly()
			.Run();

		if ( !trace.Hit || !trace.Entity.IsValid() || !trace.Body.IsValid() || trace.Body.BodyType != PhysicsBodyType.Dynamic )
			return;

		if ( trace.Entity is Player )
			return;

		// Cannot pickup items held by other players.
		if ( trace.Entity.Parent is not null )
			return;

		switch ( trace.Entity )
		{
			case Corpse corpse:
				_grabbedEntity = new GrabbableCorpse( Owner, corpse );
				break;
			case Carriable: // Ignore any size requirements, any weapon can be picked up.
				_grabbedEntity = new GrabbableProp( Owner, GrabPoint, trace.Entity as ModelEntity );
				break;
			case ModelEntity model:
				if ( !HasGreatorOrEqualAxis( model.CollisionBounds.Size, _maxPickupSize ) && model.PhysicsGroup.Mass < MaxPickupMass )
					_grabbedEntity = new GrabbableProp( Owner, GrabPoint, model );
				break;
		}
	}

	public override void OnCarryStart( Player player )
	{
		base.OnCarryStart( player );

		if ( !Host.IsServer )
			return;

		GrabPoint = new ModelEntity( "models/hands/grabpoint.vmdl" );
		GrabPoint.EnableHideInFirstPerson = false;
		GrabPoint.SetParent( player, MiddleHandsAttachment, new Transform( Vector3.Zero ) );
	}

	public override void OnCarryDrop( Player player )
	{
		base.OnCarryStart( player );

		if ( !Host.IsServer )
			return;

		_grabbedEntity?.Drop();
		GrabPoint?.Delete();
	}

	public override void ActiveEnd( Player player, bool dropped )
	{
		_grabbedEntity?.Drop();

		base.ActiveEnd( player, dropped );
	}

	public override void SimulateAnimator( PawnAnimator animator )
	{
		if ( Host.IsClient )
			return;

		animator.SetAnimParameter( "b_attack", animator.HasTag( "push" ) );

		if ( IsHoldingEntity )
		{
			animator.SetAnimParameter( "holdtype", 4 );
			animator.SetAnimParameter( "holdtype_handedness", 0 );
		}
		else
		{
			animator.SetAnimParameter( "holdtype", 0 );
		}
	}

	private static bool HasGreatorOrEqualAxis( Vector3 local, Vector3 other )
	{
		return local.x >= other.x || local.y >= other.y || local.z >= other.z;
	}

	[Event.Entity.PreCleanup]
	private void OnPreCleanup()
	{
		_grabbedEntity?.Drop();
	}
}
