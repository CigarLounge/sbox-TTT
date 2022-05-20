using Sandbox;

namespace TTT;

public partial class Player
{
	private BaseRole _role;
	public BaseRole Role
	{
		get => _role;
		set
		{
			if ( _role == value )
				return;

			_role?.OnDeselect( this );
			var oldRole = _role;
			_role = value;

			// Always send the role to this player's client
			if ( IsServer )
				SendRole( To.Single( this ) );

			_role.OnSelect( this );

			Event.Run( TTTEvent.Player.RoleChanged, this, oldRole );
		}
	}

	public Team Team => Role.Team;

	/// <summary>
	/// Sends the role to the given target.
	/// </summary>
	/// <param name="to">The target.</param>
	public void SendRole( To to )
	{
		Host.AssertServer();

		ClientSetRole( to, Role.Info );
	}

	public void SetRole( string libraryName )
	{
		Role = TypeLibrary.Create<BaseRole>( libraryName );
	}

	[ClientRpc]
	private void ClientSetRole( RoleInfo roleInfo )
	{
		IsRoleKnown = true;
		SetRole( roleInfo.LibraryName );
	}
}
