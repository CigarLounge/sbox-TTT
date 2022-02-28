using Sandbox;

namespace TTT;

public partial class Player
{
	public BaseRole Role
	{
		get
		{
			if ( _role == null )
			{
				_role = new NoneRole();
			}

			return _role;
		}
		private set
		{
			_role = value;
		}
	}
	private BaseRole _role;

	public Team Team => Role.Info.Team;

	public void SetRole( BaseRole role )
	{
		if ( role == null )
			return;

		Role?.OnDeselect( this );
		Role = role;
		Role.OnSelect( this );

		if ( IsServer )
			SendClientRole();
	}

	public void SetRole( string libraryName )
	{
		SetRole( Library.Create<BaseRole>( libraryName ) );
	}

	public void SetRole( int id )
	{
		SetRole( Asset.CreateFromAssetId<BaseRole>( id ) );
	}

	/// <summary>
	/// Sends the role + team and all connected additional data like logic buttons of the current Player to the given target or - if no target was provided - the player itself
	/// </summary>
	/// <param name="to">optional - The target</param>
	public void SendClientRole( To? to = null )
	{
		RPCs.ClientSetRole( to ?? To.Single( this ), this, Role.Info.Id );

		if ( to == null || to.Value.ToString().Equals( Client.Name ) )
			SendLogicButtonsToClient();
	}
}
