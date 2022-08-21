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
	/// Fired once a round has ended.
	/// </summary>
	protected Output<Team> OnRoundEnd { get; set; }

	/// <summary>
	/// Does not run on entity awake/spawn, is called explicitly by the TTT gamemode to trigger.
	/// </summary>
	public void FireSettingsSpawn() => _ = SettingsSpawned.Fire( this );

	[GameEvent.Round.Start]
	private void RoundStart() => _ = OnRoundStart.Fire( this );


	[GameEvent.Round.End]
	private void RoundEnd( Team winningTeam, WinType winType ) => _ = OnRoundEnd.Fire( this, winningTeam );
}
