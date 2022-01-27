using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTT.Player;

namespace TTT.UI
{
	public class InspectMenu : Panel
	{
		public static InspectMenu Instance;

		private PlayerCorpse _playerCorpse;
		private ConfirmationData _confirmationData;
		private InspectEntry _selectedInspectEntry;

		private readonly InspectEntry _timeSinceDeathEntry;
		private readonly InspectEntry _suicideEntry;
		private readonly InspectEntry _weaponEntry;
		private readonly InspectEntry _headshotEntry;
		private readonly InspectEntry _distanceEntry;
		private readonly List<InspectEntry> _perkEntries;

		private readonly Panel _backgroundPanel;
		private readonly Panel _inspectContainer;
		private readonly Image _avatarImage;
		private readonly Label _playerLabel;
		private readonly Label _roleLabel;
		private readonly Panel _inspectIconsPanel;
		private readonly Label _inspectDetailsLabel;

		public bool Enabled
		{
			get => this.IsEnabled();
			set
			{
				this.Enabled( value );

				SetClass( "fade-in", this.IsEnabled() );
				_inspectContainer.SetClass( "pop-in", this.IsEnabled() );
			}
		}

		public InspectMenu()
		{
			Instance = this;

			StyleSheet.Load( "/ui/generalhud/inspectmenu/InspectMenu.scss" );

			AddClass( "text-shadow" );

			_backgroundPanel = new Panel( this );
			_backgroundPanel.AddClass( "background-color-secondary" );
			_backgroundPanel.AddClass( "opacity-medium" );
			_backgroundPanel.AddClass( "fullscreen" );

			_inspectContainer = new Panel( this );
			_inspectContainer.AddClass( "inspect-container" );

			_avatarImage = _inspectContainer.Add.Image();
			_avatarImage.AddClass( "avatar-image" );
			_avatarImage.AddClass( "box-shadow" );
			_avatarImage.AddClass( "circular" );

			_playerLabel = _inspectContainer.Add.Label( String.Empty );
			_playerLabel.AddClass( "player-label" );

			_roleLabel = _inspectContainer.Add.Label();
			_roleLabel.AddClass( "role-label" );

			_inspectIconsPanel = new Panel( _inspectContainer );
			_inspectIconsPanel.AddClass( "info-panel" );

			List<InspectEntry> inspectionEntries = new();

			_timeSinceDeathEntry = new InspectEntry( _inspectIconsPanel );
			_timeSinceDeathEntry.Enabled( true ); // Time since death is ALWAYS visible
			_timeSinceDeathEntry.SetData( "/ui/inspectmenu/time.png", "" );
			inspectionEntries.Add( _timeSinceDeathEntry );

			_suicideEntry = new InspectEntry( _inspectIconsPanel );
			_suicideEntry.Enabled( false );
			inspectionEntries.Add( _suicideEntry );

			_weaponEntry = new InspectEntry( _inspectIconsPanel );
			_weaponEntry.Enabled( false );
			inspectionEntries.Add( _weaponEntry );

			_headshotEntry = new InspectEntry( _inspectIconsPanel );
			_headshotEntry.Enabled( false );
			inspectionEntries.Add( _headshotEntry );

			_distanceEntry = new InspectEntry( _inspectIconsPanel );
			_distanceEntry.Enabled( false );
			inspectionEntries.Add( _distanceEntry );

			_perkEntries = new List<InspectEntry>();

			foreach ( InspectEntry entry in inspectionEntries )
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

			_inspectDetailsLabel = _inspectContainer.Add.Label();
			_inspectDetailsLabel.AddClass( "inspect-details-label" );

			Enabled = false;
		}

		public void InspectCorpse( PlayerCorpse playerCorpse )
		{
			if ( playerCorpse?.DeadPlayer == null )
			{
				return;
			}

			_playerCorpse = playerCorpse;

			_avatarImage.SetTexture( $"avatar:{_playerCorpse.DeadPlayer?.Client.PlayerId}" );

			_playerLabel.Text = _playerCorpse.DeadPlayer?.Client.Name;

			_roleLabel.Text = _playerCorpse.DeadPlayer?.Role.Name;
			_roleLabel.Style.FontColor = _playerCorpse.DeadPlayer?.Role.Color;

			SetConfirmationData( _playerCorpse.GetConfirmationData(), _playerCorpse.KillerWeapon, _playerCorpse.Perks );

			Enabled = true;
		}

		public void SetPlayerData( PlayerCorpse playerCorpse )
		{
			_playerCorpse = playerCorpse;

			SetConfirmationData( playerCorpse.GetConfirmationData(), _playerCorpse.KillerWeapon, _playerCorpse.Perks );
		}

		public void SetConfirmationData( ConfirmationData confirmationData, string killerWeapon, string[] perks )
		{
			_confirmationData = confirmationData;

			_headshotEntry.Enabled( confirmationData.Headshot );
			_headshotEntry.SetData( "/ui/inspectmenu/headshot.png", new string( "CORPSE_INSPECT_IDENTIFIER_HEADSHOT" ) );
			_headshotEntry.SetQuickInfo( new string( "CORPSE_INSPECT_QUICKINFO_HEADSHOT" ) );

			_suicideEntry.Enabled( confirmationData.Suicide );
			_suicideEntry.SetData( String.Empty, new string( "CORPSE_INSPECT_IDENTIFIER_SUICIDE" ) );
			_suicideEntry.SetQuickInfo( new string( "CORPSE_INSPECT_QUICKINFO_SUICIDE" ) );

			_distanceEntry.Enabled( !confirmationData.Suicide );
			_distanceEntry.SetData( "/ui/inspectmenu/distance.png", $"They were killed from approximately {confirmationData.Distance}m away." );
			_distanceEntry.SetQuickInfo( $"{confirmationData.Distance}m" );

			_weaponEntry.Enabled( !string.IsNullOrEmpty( killerWeapon ) );

			if ( _weaponEntry.IsEnabled() )
			{
				_weaponEntry.SetData( $"/ui/weapons/{killerWeapon}.png", $"It appears a {killerWeapon.ToUpper()} was used to kill them." );
				_weaponEntry.SetQuickInfo( $"{killerWeapon.ToUpper()}" );
			}

			// Clear and delete all perks
			foreach ( InspectEntry perkEntry in _perkEntries )
			{
				perkEntry.Delete();
			}

			_perkEntries.Clear();

			// Populate perk entries
			if ( perks != null )
			{
				foreach ( string perkName in perks )
				{
					InspectEntry perkEntry = new( this );
					perkEntry.SetData( $"/ui/weapons/{perkName}.png", $"They were carrying a {perkName}" );

					_perkEntries.Add( perkEntry );
				}
			}
		}

		private void UpdateCurrentInspectDescription()
		{
			_inspectDetailsLabel.SetClass( "fade-in", _selectedInspectEntry != null );

			if ( _selectedInspectEntry == null )
			{
				return;
			}

			_inspectDetailsLabel.Text = _selectedInspectEntry.Text;
		}

		public override void Tick()
		{
			if ( !Enabled || !_playerCorpse.IsValid() || _playerCorpse.Transform.Position.Distance( Local.Pawn.Owner.Position ) > 100f )
			{
				Enabled = false;

				return;
			}

			string timeSinceDeath = Utils.TimerString( Time.Now - _confirmationData.Time );
			_timeSinceDeathEntry.SetQuickInfo( $"{timeSinceDeath}" );

			if ( _selectedInspectEntry != null && _selectedInspectEntry == _timeSinceDeathEntry )
			{
				UpdateCurrentInspectDescription();
			}
		}
	}
}
