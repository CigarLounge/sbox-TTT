using Sandbox;
using System.Collections.Generic;

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

			_playersWhoKnowTheRole.Clear();
			_role.OnSelect( this );

			Event.Run( TTTEvent.Player.RoleChanged, this, oldRole );
		}
	}

	public Team Team => Role.Team;
	private readonly HashSet<int> _playersWhoKnowTheRole = new();

	/// <summary>
	/// Sends the role to the given target.
	/// </summary>
	/// <param name="to">The target.</param>
	public void SendRole( To to )
	{
		Host.AssertServer();

		foreach ( var client in to )
		{
			var id = client.Pawn.NetworkIdent;
			if ( _playersWhoKnowTheRole.Contains( id ) )
				continue;

			_playersWhoKnowTheRole.Add( id );
			ClientSetRole( To.Single( client ), Role.Info );
		}
	}

	public void SetRole( string className )
	{
		Role = TypeLibrary.Create<Role>( className );
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

		// After the roles have been assigned, set everyone
		// to being innocent clientside.
		if ( !IsRoleKnown )
			Role = new Innocent();
	}
}
