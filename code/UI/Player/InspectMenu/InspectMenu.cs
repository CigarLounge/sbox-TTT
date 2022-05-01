using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace TTT.UI;

[UseTemplate]
public partial class InspectMenu : Panel
{
	private readonly Corpse _corpse;
	private InspectEntry _selectedInspectEntry;

	private readonly List<InspectEntry> _inspectionEntries = new();
	private readonly InspectEntry _timeSinceDeath;
	private readonly InspectEntry _deathCause;
	private readonly InspectEntry _weapon;
	private readonly InspectEntry _headshot;
	private readonly InspectEntry _lastSeen;
	private readonly InspectEntry _c4Note;

	private Panel InspectContainer { get; set; }
	private Image PlayerAvatar { get; set; }
	private Label RoleName { get; set; }
	private Label PlayerName { get; set; }
	private Panel IconsContainer { get; set; }
	private Button CallDetectiveButton { get; set; }
	private readonly Label _inspectDetailsLabel;

	public InspectMenu( Corpse corpse )
	{
		if ( corpse.Player is null )
			return;

		_timeSinceDeath = new InspectEntry( IconsContainer );
		_timeSinceDeath.Enabled( true );
		_timeSinceDeath.SetImage( "/ui/inspectmenu/time.png" );
		_inspectionEntries.Add( _timeSinceDeath );

		_deathCause = new InspectEntry( IconsContainer );
		_deathCause.Enabled( false );
		_inspectionEntries.Add( _deathCause );

		_weapon = new InspectEntry( IconsContainer );
		_weapon.Enabled( false );
		_inspectionEntries.Add( _weapon );

		_headshot = new InspectEntry( IconsContainer );
		_headshot.Enabled( false );
		_inspectionEntries.Add( _headshot );

		_lastSeen = new InspectEntry( IconsContainer );
		_lastSeen.Enabled( false );
		_inspectionEntries.Add( _lastSeen );

		_c4Note = new InspectEntry( IconsContainer );
		_c4Note.Enabled( false );
		_inspectionEntries.Add( _c4Note );

		_inspectDetailsLabel = InspectContainer.Add.Label();
		_inspectDetailsLabel.AddClass( "inspect-details-label" );

		_corpse = corpse;
		SetConfirmationData( _corpse.KillerWeapon, _corpse.Perks );
	}

	private void SetConfirmationData( CarriableInfo carriableInfo, PerkInfo[] perks )
	{
		PlayerAvatar.SetTexture( $"avatar:{_corpse.PlayerId}" );
		PlayerName.Text = _corpse.PlayerName;
		RoleName.Text = _corpse.Player.Role.Title;
		RoleName.Style.FontColor = _corpse.Player.Role.Color;

		_headshot.Enabled( _corpse.WasHeadshot );
		_headshot.SetImage( "/ui/inspectmenu/headshot.png" );
		_headshot.SetImageText( "Headshot" );
		_headshot.SetActiveText( "The fatal wound was a headshot. No time to scream." );

		var (name, imageText, activeText) = GetCauseOfDeathStrings();
		_deathCause.Enabled( true );
		_deathCause.SetImage( $"/ui/inspectmenu/{name}.png" );
		_deathCause.SetImageText( imageText );
		_deathCause.SetActiveText( activeText );

		_lastSeen.Enabled( !string.IsNullOrEmpty( _corpse.LastSeenPlayerName ) );
		if ( _lastSeen.IsEnabled() )
		{
			_lastSeen.SetImage( "/ui/inspectmenu/lastseen.png" );
			_lastSeen.SetImageText( _corpse.LastSeenPlayerName );
			_lastSeen.SetActiveText( $"The last person they saw was {_corpse.LastSeenPlayerName}... killer or coincidence?" );
		}

		_weapon.Enabled( carriableInfo is not null );
		if ( _weapon.IsEnabled() )
		{
			_weapon.SetTexture( carriableInfo.Icon );
			_weapon.SetImageText( $"{carriableInfo.Title}" );
			_weapon.SetActiveText( $"It appears a {carriableInfo.Title} was used to kill them." );
		}

		_c4Note.Enabled( !string.IsNullOrEmpty( _corpse.C4Note ) );
		if ( _c4Note.IsEnabled() )
		{
			_c4Note.SetImage( "/ui/inspectmenu/c4note.png" );
			_c4Note.SetImageText( "C4 Defuse Note" );
			_c4Note.SetActiveText( $"You find a note stating that cutting wire {_corpse.C4Note} will safely disarm the C4." );
		}

		if ( !perks.IsNullOrEmpty() )
		{
			foreach ( var perk in perks )
			{
				var perkEntry = new InspectEntry( IconsContainer );
				perkEntry.SetTexture( perk.Icon );
				perkEntry.SetImageText( perk.Title );
				perkEntry.SetActiveText( $"They were carrying {perk.Title}." );

				_inspectionEntries.Add( perkEntry );
			}
		}

		foreach ( var entry in _inspectionEntries )
		{
			entry.AddEventListener( "onmouseover", () =>
			 {
				 _selectedInspectEntry = entry;
				 UpdateCurrentInspectDescription();
			 } );

			entry.AddEventListener( "onmouseout", () =>
			 {
				 _selectedInspectEntry = null;
				 UpdateCurrentInspectDescription();
			 } );
		}
	}

	private void UpdateCurrentInspectDescription()
	{
		_inspectDetailsLabel.SetClass( "fade-in", _selectedInspectEntry is not null );

		if ( _selectedInspectEntry is null )
			return;

		_inspectDetailsLabel.Text = _selectedInspectEntry.ActiveText;
	}

	private (string name, string imageText, string activeText) GetCauseOfDeathStrings()
	{
		return _corpse.KillInfo.Flags switch
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
		CallDetectiveButton.Enabled( _corpse.Player.IsConfirmedDead );
		CallDetectiveButton.SetClass( "inactive", _corpse.HasCalledDetective || !Local.Pawn.IsAlive() );

		string timeSinceDeath = (Time.Now - _corpse.KilledTime).TimerString();
		_timeSinceDeath.SetImageText( $"{timeSinceDeath}" );
		_timeSinceDeath.SetActiveText( $"They died roughly {timeSinceDeath} ago." );

		if ( _selectedInspectEntry is not null && _selectedInspectEntry == _timeSinceDeath )
			UpdateCurrentInspectDescription();
	}

	// Called from UI panel
	public void CallDetective()
	{
		if ( _corpse.HasCalledDetective )
			return;

		CallDetectives( _corpse.NetworkIdent );
		_corpse.HasCalledDetective = true;
	}

	[ServerCmd]
	private static void CallDetectives( int ident )
	{
		var ent = Entity.FindByIndex( ident );
		if ( !ent.IsValid() || ent is not Corpse corpse )
			return;

		ChatBox.AddInfo( To.Everyone, $"{ConsoleSystem.Caller.Name} called a Detective to the body of {corpse.PlayerName}" );
		SendDetectiveMarker( To.Multiple( Utils.GetAliveClientsWithRole( new Detective() ) ), corpse.Position );
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
