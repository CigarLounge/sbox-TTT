using Sandbox;

namespace TTT;

[Library( "ttt_map_settings" )]
public partial class MapSettings : Entity
{
	/// <summary>
	/// Fired after PostLevelLoaded runs and MapSettings entity is found.
	/// </summary>
	protected Output SettingsSpawned { get; set; }

	/// <summary>
	/// Fired once round starts and roles are assigned.
	/// </summary>
	protected Output RolesAssigned { get; set; }

	/// <summary>
	/// Fired once round starts and roles are assigned.
	/// </summary>
	protected Output RoundStart { get; set; }

	/// <summary>
	/// Fired once Preround begins.
	/// </summary>
	protected Output<Team> RoundEnd { get; set; }

	/// <summary>
	/// Does not run on entity awake/spawn, is called explicitly by the TTT gamemode to trigger.
	/// </summary>
	public void FireSettingsSpawn() => SettingsSpawned.Fire( this );

	[TTTEvent.Round.Started]
	private void OnRoundStarted()
	{
		RoundStart.Fire( this );
	}

	[TTTEvent.Round.RolesAssigned]
	private void OnRolesAssigned()
	{
		RolesAssigned.Fire( this );
	}

	[TTTEvent.Round.Ended]
	private void OnRoundEnded( Team winningTeam, WinType winType )
	{
		RoundEnd.Fire( this, winningTeam );
	}
}
