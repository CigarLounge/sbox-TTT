using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

using TTT.Events;
using TTT.Player;

namespace TTT.UI
{
	public partial class Hud : HudEntity<RootPanel>
	{
		public static Hud Current { set; get; }

		public GeneralHud GeneralHudPanel;
		public AliveHud AliveHudInstance;

		public Hud()
		{
			if ( Host.IsServer )
			{
				return;
			}

			RootPanel.StyleSheet.Load( "/ui/Hud.scss" );
			RootPanel.AddClass( "panel" );

			GeneralHudPanel = RootPanel.AddChild<GeneralHud>();
			AliveHudInstance = new( RootPanel );
			Current = this;
		}

		[Event.Hotload]
		public static void OnHotReloaded()
		{
			if ( Host.IsServer )
			{
				return;
			}

			Hud.Current?.Delete();

			Hud hud = new();

			if ( Local.Client.Pawn is TTTPlayer player )
			{
				Current.AliveHudInstance.Enabled = player.LifeState == LifeState.Alive && !player.IsForcedSpectator;
			}

			Event.Run( TTTEvent.UI.Reloaded );
		}

		[Event( TTTEvent.Player.Spawned )]
		private void OnPlayerSpawned( TTTPlayer player )
		{
			if ( IsServer || player != Local.Client.Pawn )
			{
				return;
			}

			AliveHudInstance.Enabled = !player.IsSpectator && !player.IsForcedSpectator;
		}

		[Event( TTTEvent.Player.Died )]
		private void OnPlayerDied( TTTPlayer deadPlayer )
		{
			if ( deadPlayer != Local.Client.Pawn )
			{
				return;
			}

			AliveHudInstance.Enabled = false;
		}

		[Event( TTTEvent.Player.InitialSpawn )]
		public static void Initialize( Client client )
		{
			if ( Host.IsServer || client != Local.Client )
			{
				return;
			}

			Hud hud = new();
			hud.OnPlayerSpawned( client.Pawn as TTTPlayer ); // InitialSpawn event is called after Spawned event, so we have to initialize manually
		}

		public class GeneralHud : Panel
		{
			public GeneralHud()
			{
				AddClass( "fullscreen" );
				AddChild<WIPDisclaimer>();

				AddChild<HintDisplay>();
				AddChild<RadarDisplay>();
				AddChild<PlayerRoleDisplay>();
				AddChild<PlayerInfoDisplay>();
				AddChild<InventoryWrapper>();
				AddChild<ChatBox>();

				AddChild<VoiceChatDisplay>();
				AddChild<GameTimerDisplay>();

				AddChild<VoiceList>();

				AddChild<InfoFeed>();
				AddChild<FullScreenMenu>();
				AddChild<PostRoundMenu>();
				AddChild<Scoreboard>();
				AddChild<MapSelectionMenu>();
				AddChild<TTTMenu>();
			}

			// Use "GeneralHud" as the Panel that displays any s&box popups.
			public override Panel FindPopupPanel()
			{
				return this;
			}
		}

		public class AliveHud
		{
			public bool Enabled
			{
				get => _enabled;
				internal set
				{
					_enabled = value;

					if ( value )
					{
						Create();
					}
					else
					{
						Destroy();
					}
				}
			}
			private bool _enabled = false;

			private RootPanel _rootPanel;

			private List<Panel> _panelList = new();

			public AliveHud( RootPanel rootPanel )
			{
				_rootPanel = rootPanel;
			}

			private void Create()
			{
				_panelList = new()
				{
					_rootPanel.AddChild<Crosshair>(),
					_rootPanel.AddChild<BreathIndicator>(),
					_rootPanel.AddChild<StaminaIndicator>(),
					_rootPanel.AddChild<QuickShop>(),
					_rootPanel.AddChild<DamageIndicator>()
				};
			}

			private void Destroy()
			{
				_panelList.ForEach( ( panel ) => panel.Delete( true ) );
				_panelList.Clear();
			}
		}
	}
}
