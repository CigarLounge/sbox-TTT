using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class InspectMenu : Panel
{
	private readonly Corpse _playerCorpse;
	private InspectEntry _selectedInspectEntry;

	private readonly List<InspectEntry> _inspectionEntries = new();
	private readonly InspectEntry _timeSinceDeathEntry;
	private readonly InspectEntry _deathCauseEntry;
	private readonly InspectEntry _weaponEntry;
	private readonly InspectEntry _headshotEntry;
	private readonly InspectEntry _distanceEntry;

	private readonly Panel _inspectContainer;
	private readonly Image _avatarImage;
	private readonly Label _playerLabel;
	private readonly Label _roleLabel;
	private readonly Panel _inspectIconsPanel;
	private readonly Label _inspectDetailsLabel;

	public InspectMenu( Corpse playerCorpse )
	{
		if ( playerCorpse.DeadPlayer == null )
			return;

		StyleSheet.Load( "/ui/player/inspectmenu/InspectMenu.scss" );

		AddClass( "text-shadow" );

		_inspectContainer = new Panel( this );
		_inspectContainer.AddClass( "inspect-container" );

		_avatarImage = _inspectContainer.Add.Image();
		_avatarImage.AddClass( "avatar-image" );
		_avatarImage.AddClass( "box-shadow" );
		_avatarImage.AddClass( "circular" );

		_playerLabel = _inspectContainer.Add.Label();
		_playerLabel.AddClass( "player-label" );

		_roleLabel = _inspectContainer.Add.Label();
		_roleLabel.AddClass( "role-label" );

		_inspectIconsPanel = new Panel( _inspectContainer );
		_inspectIconsPanel.AddClass( "info-panel" );

		_timeSinceDeathEntry = new InspectEntry( _inspectIconsPanel );
		_timeSinceDeathEntry.Enabled( true ); // Time since death is ALWAYS visible
		_timeSinceDeathEntry.SetImage( "/ui/inspectmenu/time.png" );
		_inspectionEntries.Add( _timeSinceDeathEntry );

		_deathCauseEntry = new InspectEntry( _inspectIconsPanel );
		_deathCauseEntry.Enabled( false );
		_inspectionEntries.Add( _deathCauseEntry );

		_weaponEntry = new InspectEntry( _inspectIconsPanel );
		_weaponEntry.Enabled( false );
		_inspectionEntries.Add( _weaponEntry );

		_headshotEntry = new InspectEntry( _inspectIconsPanel );
		_headshotEntry.Enabled( false );
		_inspectionEntries.Add( _headshotEntry );

		_distanceEntry = new InspectEntry( _inspectIconsPanel );
		_distanceEntry.Enabled( false );
		_inspectionEntries.Add( _distanceEntry );

		_inspectDetailsLabel = _inspectContainer.Add.Label();
		_inspectDetailsLabel.AddClass( "inspect-details-label" );

		_playerCorpse = playerCorpse;
		SetConfirmationData( _playerCorpse.KillerWeapon, _playerCorpse.Perks );
	}

	private void SetConfirmationData( CarriableInfo carriableInfo, string[] perks )
	{
		_avatarImage.SetTexture( $"avatar:{_playerCorpse.PlayerId}" );
		_playerLabel.Text = _playerCorpse.PlayerName;
		_roleLabel.Text = _playerCorpse.DeadPlayer.Role.Title;
		_roleLabel.Style.FontColor = _playerCorpse.DeadPlayer.Role.Color;

		_headshotEntry.Enabled( _playerCorpse.WasHeadshot );
		_headshotEntry.SetImage( "/ui/inspectmenu/headshot.png" );
		_headshotEntry.SetImageText( "Headshot" );
		_headshotEntry.SetActiveText( "The fatal wound was a headshot. No time to scream." );

		var (name, imageText, activeText) = GetCauseOfDeathStrings();
		_deathCauseEntry.Enabled( true );
		_deathCauseEntry.SetImage( $"/ui/inspectmenu/{name}.png" );
		_deathCauseEntry.SetImageText( imageText );
		_deathCauseEntry.SetActiveText( activeText );

		_distanceEntry.Enabled( _playerCorpse.KillInfo.Flags != DamageFlags.Generic );
		_distanceEntry.SetImage( "/ui/inspectmenu/distance.png" );
		_distanceEntry.SetImageText( $"{_playerCorpse.Distance:n0}m" );
		_distanceEntry.SetActiveText( $"They were killed from approximately {_playerCorpse.Distance:n0}m away." );

		_weaponEntry.Enabled( carriableInfo != null );
		if ( _weaponEntry.IsEnabled() )
		{
			_weaponEntry.SetImage( carriableInfo.Icon );
			_weaponEntry.SetImageText( $"{carriableInfo.Title}" );
			_weaponEntry.SetActiveText( $"It appears a {carriableInfo.Title} was used to kill them." );
		}

		// Populate perk entries
		if ( perks != null )
		{
			foreach ( string perkName in perks )
			{
				InspectEntry perkEntry = new( _inspectIconsPanel );
				perkEntry.SetImage( $"/ui/icons/{perkName}.png" );
				perkEntry.SetImageText( $"{perkName}" );
				perkEntry.SetActiveText( $"They were carrying {perkName}." );

				_inspectionEntries.Add( perkEntry );
			}
		}

		foreach ( InspectEntry entry in _inspectionEntries )
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
		_inspectDetailsLabel.SetClass( "fade-in", _selectedInspectEntry != null );

		if ( _selectedInspectEntry == null )
		{
			return;
		}

		_inspectDetailsLabel.Text = _selectedInspectEntry.ActiveText;
	}

	private (string name, string imageText, string activeText) GetCauseOfDeathStrings()
	{
		return _playerCorpse.KillInfo.Flags switch
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
			DamageFlags.Drown => ("Drown", "Drown", "This corpse drowned to death."),
			_ => ("Unknown", "Unknown", "The cause of death is unknown.")
		};
	}

	public override void Tick()
	{
		string timeSinceDeath = (Time.Now - _playerCorpse.KilledTime).TimerString();
		_timeSinceDeathEntry.SetImageText( $"{timeSinceDeath}" );
		_timeSinceDeathEntry.SetActiveText( $"They died roughly {timeSinceDeath} ago." );

		if ( _selectedInspectEntry != null && _selectedInspectEntry == _timeSinceDeathEntry )
		{
			UpdateCurrentInspectDescription();
		}
	}
}
