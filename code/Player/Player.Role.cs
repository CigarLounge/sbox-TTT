using Sandbox;
using System.Collections.Generic;

namespace TTT;

public partial class Player
{
	private readonly HashSet<int> _playersWhoKnowTheRole = new();

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
			_isRoleKnown = false;
			_playersWhoKnowTheRole.Clear();

			// Always send the role to this player's client
			if ( IsServer )
				SendRole( To.Single( this ) );

			_role.OnSelect( this );

			Event.Run( TTTEvent.Player.RoleChanged, this, oldRole );
		}
	}

	public Team Team => Role.Team;

	private bool _isRoleKnown;
	/// <summary>
	/// Serverside, this means the role is publicly announced to everyone.
	/// Clientside, this means we know this player's actual role.
	/// </summary>
	public bool IsRoleKnown
	{
		get => _isRoleKnown;
		set
		{
			if ( _isRoleKnown == value )
				return;

			if ( IsServer && value )
				SendRole( To.Everyone );

			_isRoleKnown = value;
		}
	}

	/// <summary>
	/// Sends the role to the given target.
	/// </summary>
	/// <param name="to">The target. </param>
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
		SetRole( roleInfo.ClassName );
		IsRoleKnown = true;
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
			Role = new Innocent();
	}
}
