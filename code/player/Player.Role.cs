using System.Collections.Generic;

using Sandbox;

namespace TTT;

public partial class Player
{
	public BaseRole Role { get; set; }

	public Team Team => Role.Info.Team;

	public void SetRole( BaseRole role )
	{
		Role?.OnDeselect( this );
		Role = role;
		Role.OnSelect( this );
	}

	public void SetRole( string libraryName )
	{
		SetRole( Library.Create<BaseRole>( libraryName ) );
	}

	/// <summary>
	/// Sends the role + team and all connected additional data like logic buttons of the current TTTPlayer to the given target or - if no target was provided - the player itself
	/// </summary>
	/// <param name="to">optional - The target</param>
	public void SendClientRole( To? to = null )
	{
		RPCs.ClientSetRole( to ?? To.Single( this ), this, Role.ClassInfo.Name );

		if ( to == null || to.Value.ToString().Equals( Client.Name ) )
			SendLogicButtonsToClient();
	}

	public void SyncMIA( Player player = null )
	{
		if ( Game.Current.Round is not InProgressRound )
			return;

		if ( player == null )
		{
			List<Client> traitors = new();

			foreach ( Client client in Client.All )
			{
				if ( (client.Pawn as Player).Team == Team.Traitors )
				{
					traitors.Add( client );
				}
			}

			RPCs.ClientAddMissingInAction( To.Multiple( traitors ), this );
		}
		else
		{
			RPCs.ClientAddMissingInAction( To.Single( player ), this );
		}
	}
}
