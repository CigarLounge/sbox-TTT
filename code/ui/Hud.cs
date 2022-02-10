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

		public Hud()
		{
			if ( Host.IsServer )
			{
				return;
			}

			Current = this;

			RootPanel.StyleSheet.Load( "/ui/Hud.scss" );
			RootPanel.AddClass( "panel" );

			GeneralHudPanel = RootPanel.AddChild<GeneralHud>();
		}

		[Event.Hotload]
		public static void OnHotReloaded()
		{
			if ( Host.IsServer )
			{
				return;
			}

			Hud.Current?.Delete();
			_ = new Hud();

			Event.Run( TTTEvent.UI.Reloaded );
		}

		public class GeneralHud : Panel
		{
			private List<Panel> _aliveHud = new();
			public bool AliveHudEnabled
			{
				get => _enabled;
				internal set
				{
					_enabled = value;

					if ( value )
					{
						CreateAliveHud();
					}
					else
					{
						DeleteAliveHud();
					}
				}
			}
			private bool _enabled = false;

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
				AddChild<FullScreenHintMenu>();
				AddChild<PostRoundMenu>();
				AddChild<Scoreboard>();
				AddChild<TTTMenu>();
			}

			private void CreateAliveHud()
			{
				if ( _aliveHud.Count != 0 ) return;
				_aliveHud = new()
				{
					AddChild<Crosshair>(),
					AddChild<BreathIndicator>(),
					AddChild<StaminaIndicator>(),
					AddChild<QuickShop>(),
					AddChild<SWB_Base.DamageIndicator>()
				};
			}

			private void DeleteAliveHud()
			{
				if ( _aliveHud.Count == 0 ) return;
				_aliveHud.ForEach( ( panel ) => panel.Delete( true ) );
				_aliveHud.Clear();
			}

			public override void Tick()
			{
				if ( Local.Pawn is not TTTPlayer player )
				{
					return;
				}

				if ( Current.GeneralHudPanel != null )
				{
					Current.GeneralHudPanel.AliveHudEnabled = player.LifeState == LifeState.Alive && !player.IsForcedSpectator;
				}
			}

			// Use "GeneralHud" as the Panel that displays any s&box popups.
			public override Panel FindPopupPanel()
			{
				return this;
			}
		}
	}
}
