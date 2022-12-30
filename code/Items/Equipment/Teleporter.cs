using Sandbox;

namespace TTT;

[Category( "Equipment" )]
[ClassName( "ttt_equipment_teleporter" )]
[Title( "Teleporter" )]
public partial class Teleporter : Carriable
{
	[Net, Predicted]
	public int Charges { get; private set; } = 16;

	[Net, Predicted]
	public bool IsTeleporting { get; private set; }

	[Net, Local, Predicted]
	public bool LocationIsSet { get; private set; }

	[Net, Local, Predicted]
	public TimeSince TimeSinceAction { get; private set; }

	[Net, Local, Predicted]
	public TimeSince TimeSinceStartedTeleporting { get; private set; }

	public override string SlotText => Charges.ToString();
	public override string PrimaryAttackHint => LocationIsSet ? "Teleport" : string.Empty;
	public override string SecondaryAttackHint => "Set teleport location";

	private const float TeleportTime = 4f;
	private bool _hasReachedLocation;
	private Vector3 _teleportLocation;
	private Particles _particle;

	public override void ActiveStart( Player player )
	{
		base.ActiveStart( player );

		IsTeleporting = false;
	}

	public override void ActiveEnd( Player player, bool dropped )
	{
		base.ActiveEnd( player, dropped );

		_particle?.Destroy( true );
	}

	public override void Simulate( IClient client )
	{
		if ( IsTeleporting )
		{
			_particle ??= Particles.Create( "particles/teleporter/teleport.vpcf", Owner, true );

			if ( TimeSinceStartedTeleporting >= TeleportTime / 2 )
			{
				if ( Game.IsServer && !_hasReachedLocation )
					Teleport();

				if ( TimeSinceStartedTeleporting >= TeleportTime )
				{
					IsTeleporting = false;
					_particle?.Destroy();
					_particle = null;
				}
			}

			return;
		}

		if ( Charges <= 0 || TimeSinceAction < 1f )
			return;

		// We can't do anything if we aren't standing on the ground
		if ( Owner.GroundEntity is not WorldEntity )
			return;

		if ( Input.Pressed( InputButton.PrimaryAttack ) )
		{
			StartTeleport();
		}
		else if ( Input.Pressed( InputButton.SecondaryAttack ) )
		{
			using ( LagCompensation() )
			{
				SetLocation();
			}
		}
	}

	public override void BuildInput()
	{
		base.BuildInput();

		if ( !IsTeleporting )
			return;

		Owner.ActiveCarriable = this;
		Owner.InputDirection = 0;
		Input.ClearButton( InputButton.Jump );
		Input.ClearButton( InputButton.Drop );
	}

	private void SetLocation()
	{
		var trace = Trace.Ray( Owner.Position, Owner.Position )
			.WorldOnly()
			.Run();

		LocationIsSet = true;
		TimeSinceAction = 0;
		_teleportLocation = trace.EndPosition;

		if ( Game.IsClient && Prediction.FirstTime )
			UI.InfoFeed.AddEntry( "Teleport location set." );
	}

	private void StartTeleport()
	{
		if ( !LocationIsSet )
			return;

		Charges -= 1;
		IsTeleporting = true;
		TimeSinceAction = 0;
		TimeSinceStartedTeleporting = 0;
		_hasReachedLocation = false;
	}

	private void Teleport()
	{
		_hasReachedLocation = true;
		Owner.Position = _teleportLocation;

		// TeleFrag players.
		var bbox = Owner.CollisionBounds + Owner.Position;

		var damageInfo = ExtendedDamageInfo.Generic( Player.MaxHealth )
			.WithAttacker( Owner )
			.WithTag( Strings.Tags.Explode )
			.WithWeapon( this );

		foreach ( var entity in FindInBox( bbox ) )
		{
			if ( entity != Owner )
				entity.TakeDamage( damageInfo );
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		_particle?.Destroy( true );
	}
}
