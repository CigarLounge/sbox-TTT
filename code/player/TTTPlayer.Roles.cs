using System.Collections.Generic;

using Sandbox;

using TTT.Roles;
using TTT.Rounds;

namespace TTT.Player;

public partial class TTTPlayer
{
	[Net, Local]
	public BaseRole Role { get; set; } = new TTT.Roles.NoneRole();

	public Team Team => Role.Team;

	public void SetRole( BaseRole role )
	{
		Role?.OnDeselect( this );
		Role = role;
		Role.OnSelect( this );
	}

	/// <summary>
	/// Sends the role + team and all connected additional data like logic buttons of the current TTTPlayer to the given target or - if no target was provided - the player itself
	/// </summary>
	/// <param name="to">optional - The target</param>
	public void SendClientRole( To? to = null )
	{
		RPCs.ClientSetRole( to ?? To.Single( this ), this, Role );

		if ( to == null || to.Value.ToString().Equals( Client.Name ) )
			SendLogicButtonsToClient();
	}

	public void SyncMIA( TTTPlayer player = null )
	{
		if ( Gamemode.Game.Current.Round is not InProgressRound )
			return;

		if ( player == null )
		{
			List<Client> traitors = new();

			foreach ( Client client in Client.All )
			{
				if ( (client.Pawn as TTTPlayer).Team == Team.Traitors )
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
