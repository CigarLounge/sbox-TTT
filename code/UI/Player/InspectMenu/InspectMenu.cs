using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace TTT.UI;

[UseTemplate]
public partial class InspectMenu : Panel
{
	private readonly Corpse _corpse;
	private InspectEntry _selectedInspectEntry;

	private readonly List<InspectEntry> _inspectionEntries = new();
	private InspectEntry _timeSinceDeath;
	private InspectEntry _dna;

	private Image PlayerAvatar { get; init; }
	private Label RoleName { get; init; }
	private Label PlayerName { get; init; }
	private Panel IconsContainer { get; init; }
	private Button CallDetectiveButton { get; init; }
	private Label ActiveText { get; init; }

	public InspectMenu( Corpse corpse )
	{
		Assert.NotNull( corpse );

		_corpse = corpse;
		var player = corpse.Player;

		PlayerAvatar.SetTexture( $"avatar:{player.SteamId}" );
		PlayerName.Text = player.SteamName;
		RoleName.Text = player.Role.Title;
		RoleName.Style.FontColor = player.Role.Color;

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
			player.LastAttackerWeaponInfo.Icon.ResourcePath );

		if ( player.KilledWithHeadShot() )
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

		if ( !string.IsNullOrEmpty( _corpse.C4Note ) )
			AddInspectEntry( "C4 Defuse Note",
			$"You find a note stating that cutting wire {_corpse.C4Note} will safely disarm the C4.",
			"/ui/inspectmenu/c4note.png" );

		if ( !_corpse.Perks.IsNullOrEmpty() )
		{
			foreach ( var perk in _corpse.Perks )
				AddInspectEntry( perk.Title, $"They were carrying {perk.Title}.", perk.Icon.ResourcePath );
		}

		foreach ( var entry in _inspectionEntries )
		{
			entry.AddEventListener( "onmouseover", () => { _selectedInspectEntry = entry; } );
			entry.AddEventListener( "onmouseout", () => { _selectedInspectEntry = null; } );
		}
	}

	private InspectEntry AddInspectEntry( string iconText, string activeText, string imagePath )
	{
		var entry = new InspectEntry( IconsContainer, iconText, activeText, imagePath );
		_inspectionEntries.Add( entry );
		return entry;
	}

	private (string name, string imageText, string activeText) GetCauseOfDeathStrings()
	{
		return _corpse.Player.LastDamage.Flags switch
		{
			DamageFlags.Generic => ("Unknown", "Unknown", "The cause of death is unknown."),
			DamageFlags.Crush => ("Crushed", "Crushed", "This corpse was crushed to death."),
			DamageFlags.Bullet => ("Bullet", "Bullet", "This corpse was shot to death."),
			DamageFlags.Buckshot => ("Bullet", "Bullet", "This corpse was shot to death."),
			DamageFlags.Slash => ("Slash", "Slashed", "This corpse was cut to death."),
			DamageFlags.Burn => ("Burn", "Burned", "This corpse has burn marks all over."),
			DamageFlags.Vehicle => ("Vehicle", "Vehicle", "This corpse was hit by a vehicle."),
			DamageFlags.Fall => ("Fall", "Fell", "This corpse fell from a high height."),
			DamageFlags.Blast => ("Explode", "Explosion", "An explosion eviscerated this corpse."),
			DamageFlags.PhysicsImpact => ("Prop", "Prop", "A wild flying prop caused this death."),
			DamageFlags.Drown => ("Drown", "Drown", "This player drowned to death."),
			_ => ("Unknown", "Unknown", "The cause of death is unknown.")
		};
	}

	public override void Tick()
	{
		var player = _corpse.Player;

		CallDetectiveButton.Enabled( player.IsConfirmedDead );
		CallDetectiveButton.SetClass( "inactive", _corpse.HasCalledDetective || !Local.Pawn.IsAlive() );

		var timeSinceDeath = player.TimeSinceDeath.Relative.TimerString();
		_timeSinceDeath.SetImageText( $"{timeSinceDeath}" );
		_timeSinceDeath.ActiveText = $"They died roughly {timeSinceDeath} ago.";

		_dna.Enabled( !_corpse.TimeUntilDNADecay );
		if ( _dna.IsEnabled() )
		{
			_dna.SetImageText( $"DNA {_corpse.TimeUntilDNADecay.Relative.TimerString()}" );
			_dna.ActiveText = $"The DNA sample will decay in {_corpse.TimeUntilDNADecay.Relative.TimerString()}.";
		}

		var isShowing = _selectedInspectEntry is not null;
		ActiveText.SetClass( "fade-in", isShowing );

		if ( isShowing )
			ActiveText.Text = _selectedInspectEntry.ActiveText;
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

		ChatBox.AddInfo( To.Everyone, $"{ConsoleSystem.Caller.Name} called a Detective to the body of {corpse.Player.SteamName}." );
		SendDetectiveMarker( To.Multiple( Utils.GetAliveClientsWithRole<Detective>() ), corpse.Position );
	}

	[ClientRpc]
	public static void SendDetectiveMarker( Vector3 corpseLocation )
	{
		var activeDetectiveMarkers = WorldPoints.Instance.FindPoints<DetectiveMarker>();
		foreach ( var marker in activeDetectiveMarkers )
		{
			if ( marker.CorpseLocation == corpseLocation )
				return;
		}

		WorldPoints.Instance.AddChild( new DetectiveMarker( corpseLocation ) );
	}
}
