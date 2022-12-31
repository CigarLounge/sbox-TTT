using Sandbox;
using Sandbox.Diagnostics;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace TTT.UI;

public partial class InspectMenu : Panel
{
	private Panel IconsContainer { get; set; }
	private readonly Corpse _corpse;
	private InspectEntry _selectedInspectEntry;
	private readonly List<InspectEntry> _inspectionEntries = new();
	private InspectEntry _timeSinceDeath;
	private InspectEntry _dna;

	public InspectMenu( Corpse corpse )
	{
		Assert.NotNull( corpse );
		_corpse = corpse;
	}

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
			return;

		SetupInspectIcons();
	}

	private void SetupInspectIcons()
	{
		var player = _corpse.Player;

		_timeSinceDeath = AddInspectEntry( string.Empty, string.Empty, "/ui/inspectmenu/time.png" );

		var (name, deathImageText, deathActiveText) = GetCauseOfDeathStrings();
		AddInspectEntry( deathImageText, deathActiveText, $"/ui/inspectmenu/{name}.png" );

		if ( player.LastAttackerWeaponInfo is not null )
			AddInspectEntry( $"{player.LastAttackerWeaponInfo.Title}",
			$"It appears a {player.LastAttackerWeaponInfo.Title} was used to kill them.",
			player.LastAttackerWeaponInfo.IconPath );

		if ( player.KilledWithHeadShot )
			AddInspectEntry( "Headshot", "The fatal wound was a headshot. No time to scream.", "/ui/inspectmenu/headshot.png" );

		_dna = AddInspectEntry( string.Empty, string.Empty, "/ui/inspectmenu/dna.png" );
		_dna.Enabled( !_corpse.TimeUntilDNADecay );

		if ( player.LastSeenPlayer.IsValid() )
			AddInspectEntry( player.LastSeenPlayer.SteamName,
			$"The last person they saw was {player.LastSeenPlayer.SteamName}... killer or coincidence?",
			"/ui/inspectmenu/lastseen.png" );

		if ( player.PlayersKilled.Count > 0 )
		{
			var activeText = "You found a list of kills that confirms the death(s) of... ";
			for ( var i = 0; i < player.PlayersKilled.Count; ++i )
				activeText += i == player.PlayersKilled.Count - 1 ? $"{player.PlayersKilled[i].SteamName}." : $"{player.PlayersKilled[i].SteamName}, ";
			AddInspectEntry( "Kill List", activeText, "/ui/inspectmenu/killlist.png" );
		}

		if ( !_corpse.C4Note.IsNullOrEmpty() )
			AddInspectEntry( "C4 Defuse Note",
			$"You find a note stating that cutting wire {_corpse.C4Note} will safely disarm the C4.",
			"/ui/inspectmenu/c4note.png" );

		if ( !_corpse.LastWords.IsNullOrEmpty() )
			AddInspectEntry( "Last Words",
			$"Their last words were... \"{_corpse.LastWords}\"",
			"/ui/inspectmenu/lastwords.png" );

		if ( !_corpse.Perks.IsNullOrEmpty() )
		{
			foreach ( var perk in _corpse.Perks )
				AddInspectEntry( perk.Title, $"They were carrying {perk.Title}.", perk.IconPath );
		}

		foreach ( var entry in _inspectionEntries )
		{
			entry.AddEventListener( "onmouseover", () => { _selectedInspectEntry = entry; } );
			entry.AddEventListener( "onmouseout", () => { _selectedInspectEntry = null; } );
		}
	}

	private InspectEntry AddInspectEntry( string iconText, string activeText, string iconPath )
	{
		var entry = new InspectEntry() { Parent = IconsContainer, IconText = iconText, ActiveText = activeText, IconPath = iconPath };
		_inspectionEntries.Add( entry );
		return entry;
	}

	private (string name, string imageText, string activeText) GetCauseOfDeathStrings()
	{
		var causeOfDeath = ("Unknown", "Unknown", "The cause of death is unknown.");
		foreach ( var tag in _corpse.Player.LastDamage.Tags )
		{
			return tag switch
			{
				Strings.Tags.Bullet => ("Bullet", "Bullet", "This corpse was shot to death."),
				Strings.Tags.Slash => ("Slash", "Slashed", "This corpse was cut to death."),
				Strings.Tags.Burn => ("Burn", "Burned", "This corpse has burn marks all over."),
				Strings.Tags.Vehicle => ("Vehicle", "Vehicle", "This corpse was hit by a vehicle."),
				Strings.Tags.Fall => ("Fall", "Fell", "This corpse fell from a high height."),
				Strings.Tags.Explode => ("Explode", "Explosion", "An explosion eviscerated this corpse."),
				Strings.Tags.Drown => ("Drown", "Drown", "This player drowned to death."),
				_ => ("Unknown", "Unknown", "The cause of death is unknown.")
			};
		}
		return causeOfDeath;
	}

	public override void Tick()
	{
		var timeSinceDeath = _corpse.Player.TimeSinceDeath.Relative.TimerString();
		_timeSinceDeath.IconText = $"{timeSinceDeath}";
		_timeSinceDeath.ActiveText = $"They died roughly {timeSinceDeath} ago.";

		_dna.Enabled( !_corpse.TimeUntilDNADecay );
		if ( _dna.IsEnabled() )
		{
			_dna.IconText = $"DNA {_corpse.TimeUntilDNADecay.Relative.TimerString()}";
			_dna.ActiveText = $"The DNA sample will decay in {_corpse.TimeUntilDNADecay.Relative.TimerString()}.";
		}
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( _corpse.HasCalledDetective, Game.LocalPawn.IsAlive(), _selectedInspectEntry );
	}

	// Called from UI panel
	public void CallDetective()
	{
		if ( _corpse.HasCalledDetective )
			return;

		CallDetectives( _corpse.NetworkIdent );
		_corpse.HasCalledDetective = true;
	}

	[ConCmd.Server]
	private static void CallDetectives( int ident )
	{
		var enemy = Entity.FindByIndex( ident );
		if ( !enemy.IsValid() || enemy is not Corpse corpse )
			return;

		TextChat.AddInfo( To.Everyone, $"{ConsoleSystem.Caller.Name} called a Detective to the body of {corpse.Player.SteamName}." );
		SendDetectiveMarker( To.Multiple( Utils.GetClientsWhere( p => p.IsAlive() && p.Role is Detective ) ), corpse.Position );
	}

	[ClientRpc]
	public static void SendDetectiveMarker( Vector3 corpseLocation )
	{
		TimeSince timeSinceCreated = 0;
		WorldPoints.Instance.AddChild(
			new WorldMarker
			(
				"/ui/d-call-icon.png",
				() => $"{(Game.LocalPawn as Player).Position.Distance( corpseLocation ).SourceUnitsToMeters():n0}m",
				() => corpseLocation,
				() => timeSinceCreated > 30
			)
		);
	}
}
