using Sandbox;
using System.Linq;

namespace TTT;

public partial class Player
{
	private Role _role;
	public Role Role
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

	public void SetRole( string className )
	{
		if ( className == Role.Innocent.Info.ClassName )
			Role = Role.Innocent;
		else if ( className == Role.Traitor.Info.ClassName )
			Role = Role.Traitor;
		else if ( className == Role.Detective.Info.ClassName )
			Role = Role.Detective;
	}

	[ClientRpc]
	private void ClientSetRole( RoleInfo roleInfo )
	{
		IsRoleKnown = true;
		SetRole( roleInfo.ClassName );
	}

	[TTTEvent.Round.RolesAssigned]
	private void OnRolesAssigned()
	{
		if ( !IsClient || IsLocalPawn )
			return;

		if ( IsSpectator )
			return;

		// After the roles have been assigned, set everyone
		// to being innocent clientside.
		if ( !IsRoleKnown )
			Role = Role.Innocent;
	}
}
