using Sandbox;

namespace TTT;

public partial class Player
{
	public BaseRole Role { get; private set; }
	public Team Team => Role.Team;

	public void SetRole( BaseRole role )
	{
		if ( role == Role )
			return;

		Role?.OnDeselect( this );
		var oldRole = Role;
		Role = role;

		// Always send the role to this player's client
		if ( IsServer )
			SendRoleToClient();

		Role.OnSelect( this );

		Event.Run( TTTEvent.Player.RoleChanged, this, oldRole );
	}

	[ClientRpc]
	private void ClientSetRole( int id )
	{
		IsRoleKnown = true;
		SetRole( id );
	}

	public void SetRole( string libraryName )
	{
		SetRole( Library.Create<BaseRole>( libraryName ) );
	}

	public void SetRole( int id )
	{
		SetRole( Asset.CreateFromId<BaseRole>( id ) );
	}

	/// <summary>
	/// Sends the role and all connected additional data like logic buttons of the current Player to the given target or - if no target was provided - the player itself
	/// </summary>
	/// <param name="to">optional - The target.</param>
	public void SendRoleToClient( To? to = null )
	{
		Host.AssertServer();

		ClientSetRole( to ?? To.Single( this ), Role.Info.Id );
	}
}
