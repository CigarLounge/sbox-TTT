using Sandbox;

namespace TTT;

[HideInEditor]
[Library( "ttt_equipment_teleporter", Title = "Teleporter" )]
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
	private Vector3 _teleportLocation;
	private Particles _particle;

	public override void ActiveStart( Entity entity )
	{
		base.ActiveStart( entity );

		IsTeleporting = false;
	}

	public override void ActiveEnd( Entity entity, bool dropped )
	{
		base.ActiveEnd( entity, dropped );

		_particle?.Destroy( true );
	}

	public override void Simulate( Client client )
	{
		if ( TimeSinceDeployed < Info.DeployTime )
			return;

		if ( IsTeleporting )
		{
			_particle ??= Particles.Create( "particles/teleport.vpcf", Owner, true );

			if ( TimeSinceStartedTeleporting >= 2f )
			{
				if ( IsServer )
				{
					Owner.Position = _teleportLocation;
					foreach ( var ent in Entity.FindInBox( Owner.CollisionBounds ) )
						if ( ent is Player player && player != Owner )
							TeleFrag( player );
				}

				if ( TimeSinceStartedTeleporting >= 4f )
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

	public override void BuildInput( InputBuilder input )
	{
		base.BuildInput( input );

		if ( !IsTeleporting )
			return;

		input.ActiveChild = this;
		input.ClearButton( InputButton.Jump );
		input.ClearButton( InputButton.Drop );
		input.InputDirection = 0;
	}

	private void SetLocation()
	{
		var trace = Trace.Ray( Owner.Position, Owner.Position )
			.WorldOnly()
			.Ignore( Owner )
			.Ignore( this )
			.Run();

		LocationIsSet = true;
		TimeSinceAction = 0;
		_teleportLocation = trace.EndPosition;

		if ( Prediction.FirstTime )
			UI.InfoFeed.Instance?.AddEntry( "Teleport location set." );
	}

	private void StartTeleport()
	{
		if ( !LocationIsSet )
			return;

		Charges -= 1;
		IsTeleporting = true;
		TimeSinceAction = 0;
		TimeSinceStartedTeleporting = 0;
	}

	private void TeleFrag( Player player )
	{
		var damageInfo = DamageInfo.Generic( float.MaxValue )
			.WithPosition( player.Position )
			.WithFlag( DamageFlags.Beam )
			.WithAttacker( Owner )
			.WithWeapon( this );

		player.TakeDamage( damageInfo );
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		_particle?.Destroy( true );
	}
}
