using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_teleporter", Title = "Teleporter" )]
public partial class Teleporter : Carriable
{
	[Net, Predicted]
	public int Charges { get; private set; } = 16;

	[Net, Predicted]
	public TimeSince TimeSinceAction { get; private set; }

	[Net, Predicted]
	public TimeSince TimeSinceStartedTeleporting { get; private set; }

	[Net, Predicted]
	public bool IsTeleporting { get; private set; }

	[Net, Predicted]
	public bool LocationIsSet { get; private set; }

	private Vector3 _teleportLocation;

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		IsTeleporting = false;
	}

	public override void Simulate( Client cl )
	{
		if ( TimeSinceDeployed < Info.DeployTime || TimeSinceAction < 1f )
			return;

		if ( IsTeleporting )
		{
			if ( TimeSinceStartedTeleporting >= 2f )
			{
				if ( IsServer )
					Owner.Position = _teleportLocation;

				IsTeleporting = false;
			}

			return;
		}

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
		if ( IsTeleporting )
			input.InputDirection = 0;
	}

	private void SetLocation()
	{
		if ( Owner.GroundEntity is not WorldEntity )
			return;

		TimeSinceAction = 0;

		var trace = Trace.Ray( Owner.Position, Owner.Position )
				.WorldOnly()
				.Ignore( Owner )
				.Ignore( this )
				.Run();

		LocationIsSet = true;
		_teleportLocation = trace.EndPosition;
		UI.InfoFeed.Instance?.AddEntry( "Teleport location set." );

		// TODO: maybe also check if the player doesn't get stuck
		// when teleported to the target location
	}

	private void StartTeleport()
	{
		if ( !LocationIsSet )
			return;

		IsTeleporting = true;
		TimeSinceAction = 0;
		TimeSinceStartedTeleporting = 0;
		Charges--;
	}
}
