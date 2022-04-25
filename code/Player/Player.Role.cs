using Sandbox;

namespace TTT;

public partial class Player
{
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
				SendRole();

			_role.OnSelect( this );

			Event.Run( TTTEvent.Player.RoleChanged, this, oldRole );
		}
	}
	private BaseRole _role;

	public Team Team => Role.Team;

	[ClientRpc]
	private void ClientSetRole( int id )
	{
		IsRoleKnown = true;
		SetRole( id );
	}

	public void SetRole( string libraryName )
	{
		Role = Library.Create<BaseRole>( libraryName );
	}

	public void SetRole( int id )
	{
		Role = Asset.CreateFromId<BaseRole>( id );
	}

	/// <summary>
	/// Sends the role to the given target or - if no target was provided - the player itself
	/// </summary>
	/// <param name="to">optional - The target.</param>
	public void SendRole( To? to = null )
	{
		Host.AssertServer();

		ClientSetRole( to ?? To.Single( this ), Role.Info.Id );
	}
}
