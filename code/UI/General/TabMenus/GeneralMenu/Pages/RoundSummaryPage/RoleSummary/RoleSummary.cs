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

	// TODO: Figure out if we can trigger this a different way. RoundEnd event doesn't get data in time.
	[ClientRpc]
	public static void SendData()
	{
		_innocents = Role.GetPlayers<Innocent>().ToList();
		_detectives = Role.GetPlayers<Detective>().ToList();
		_traitors = Role.GetPlayers<Traitor>().ToList();

		Instance?.StateHasChanged();
	}
}
