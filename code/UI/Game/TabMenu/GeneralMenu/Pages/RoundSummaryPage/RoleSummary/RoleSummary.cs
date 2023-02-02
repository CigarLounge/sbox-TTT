using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class RoleSummary : Panel
{
	public static Panel Instance { get; set; }

	private static List<Player> _innocents = new();
	private static List<Player> _detectives = new();
	private static List<Player> _traitors = new();

	public RoleSummary() => Instance = this;

	[GameEvent.Round.End]
	private static void OnRoundEnd( Team winningTeam, WinType winType )
	{
		if ( !Game.IsServer )
			return;

		RoleSummary.SendData();
	}

	// TODO: Figure out if we can trigger this a different way. RoundEnd event doesn't get data in time.
	[ClientRpc]
	public static void SendData()
	{
		_innocents = Role.GetPlayers<Innocent>().OrderByDescending( p => p.Score ).ToList();
		_detectives = Role.GetPlayers<Detective>().OrderByDescending( p => p.Score ).ToList();
		_traitors = Role.GetPlayers<Traitor>().OrderByDescending( p => p.Score ).ToList();

		Instance?.StateHasChanged();
	}
}
