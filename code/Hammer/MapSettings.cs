using Sandbox;
using SandboxEditor;

namespace TTT;

[ClassName( "ttt_settings" )]
[HammerEntity]
public partial class MapSettings : Entity
{
	/// <summary>
	/// Fired after PostLevelLoaded runs and MapSettings entity is found.
	/// </summary>
	protected Output SettingsSpawned { get; set; }

	/// <summary>
	/// Fired once a new round starts.
	/// </summary>
	protected Output OnRoundStart { get; set; }

	/// <summary>
	/// Fired once the roles have been assigned to each player.
	/// </summary>
	protected Output OnRolesAssigned { get; set; }

	/// <summary>
	/// Fired once a round has ended.
	/// </summary>
	protected Output<Team> OnRoundEnd { get; set; }

	/// <summary>
	/// Does not run on entity awake/spawn, is called explicitly by the TTT gamemode to trigger.
	/// </summary>
	public void FireSettingsSpawn() => _ = SettingsSpawned.Fire( this );

	[GameEvent.Round.Started]
	private void RoundStarted() => _ = OnRoundStart.Fire( this );

	[GameEvent.Round.RolesAssigned]
	private void RolesAssigned() => _ = OnRolesAssigned.Fire( this );

	[GameEvent.Round.Ended]
	private void RoundEnded( Team winningTeam, WinType winType ) => _ = OnRoundEnd.Fire( this, winningTeam );
}
