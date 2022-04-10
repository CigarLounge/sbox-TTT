using Sandbox;

namespace TTT.Items;

[Hammer.Skip]
[Library( "ttt_equipment_teleporter", Title = "Teleporter" )]
public partial class Teleporter : Carriable
{
	[Net, Predicted]
	public int Charges { get; private set; } = 16;

	[Net, Local, Predicted]
	public TimeSince TimeSinceAction { get; private set; }

	[Net, Local, Predicted]
	public TimeSince TimeSinceStartedTeleporting { get; private set; }

	[Net, Predicted]
	public bool IsTeleporting { get; private set; }

	[Net, Local, Predicted]
	public bool LocationIsSet { get; private set; }

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
					Owner.Position = _teleportLocation;

				if ( TimeSinceStartedTeleporting >= 4f )
				{
					IsTeleporting = false;
					_particle?.Destroy();
					_particle = null;
				}
			}

			return;
		}

		// We can't do anything if we aren't standing on the ground
		if ( Charges <= 0 || TimeSinceAction < 1f || Owner.GroundEntity is not WorldEntity )
			return;

		if ( Input.Pressed( InputButton.Attack2 ) )
		{
			using ( LagCompensation() )
			{
				SetLocation();
			}
		}
		else if ( Input.Pressed( InputButton.Attack1 ) )
		{
			StartTeleport();
		}
	}

	public override void BuildInput( InputBuilder input )
	{
		base.BuildInput( input );

		if ( !IsTeleporting )
			return;

		input.ClearButton( InputButton.Jump );
		input.ClearButton( InputButton.Drop );
		input.ActiveChild = this;
		input.InputDirection = 0;
	}

	private void SetLocation()
	{
		TimeSinceAction = 0;

		var trace = Trace.Ray( Owner.Position, Owner.Position )
				.WorldOnly()
				.Ignore( Owner )
				.Ignore( this )
				.Run();

		LocationIsSet = true;
		_teleportLocation = trace.EndPosition;

		if ( Prediction.FirstTime )
			UI.InfoFeed.Instance?.AddEntry( "Teleport location set." );
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		_particle?.Destroy( true );
	}

	private void StartTeleport()
	{
		if ( !LocationIsSet )
			return;

		IsTeleporting = true;
		TimeSinceAction = 0;
		TimeSinceStartedTeleporting = 0;
		Charges -= 1;
	}
}
