using Sandbox;

namespace TTT;

public partial class Player
{
	[Net]
	public Prop Prop { get; private set; }

	public void SimulatePossession()
	{
		if ( Input.Pressed( InputButton.Duck ) )
		{
			CancelPossession();
			return;
		}

		if ( Input.Pressed( InputButton.Jump ) || InputDirection.x != 0f || InputDirection.y != 0f )
			Prop.Components.Get<PropPossession>().Punch();
	}

	[Event.Entity.PreCleanup]
	public void CancelPossession()
	{
		if ( !this.IsAlive() && IsLocalPawn )
			CurrentCamera = new FreeCamera();

		if ( !Game.IsServer )
			return;

		if ( Prop.IsValid() )
		{
			Prop.Components.RemoveAny<PropPossession>();
			Prop.Owner = null;
		}

		Prop = null;
	}

	[ConCmd.Server]
	public static void Possess( int propNetworkIdent )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player )
			return;

		if ( player.IsAlive() || player.Prop.IsValid() )
			return;

		var target = FindByIndex( propNetworkIdent );
		if ( target is not Prop prop || !prop.IsValid() || prop.PhysicsBody is null || target.Owner is not null )
			return;

		prop.Owner = player;
		prop.Components.GetOrCreate<PropPossession>();
		player.Prop = prop;
		player.OnPropPossess();
	}

	[ClientRpc]
	private void OnPropPossess()
	{
		CurrentCamera = new FollowEntityCamera( Prop );
	}
}
