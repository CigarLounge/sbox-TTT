using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class Hud : HudEntity<RootPanel>
{
	public Hud()
	{
		if ( !Host.IsClient )
			return;

		RootPanel.StyleSheet.Load( "/ui/Hud.scss" );
		RootPanel.AddClass( "panel" );

		RootPanel.AddChild<GeneralHud>();
	}

	[Event.Hotload]
	private void OnHotReload()
	{
		if ( !IsClient ) return;

		RootPanel.DeleteChildren( true );
		RootPanel.AddChild<GeneralHud>();
	}

	public class GeneralHud : Panel
	{
		public static GeneralHud Instance;
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
			Instance = this;

			AddClass( "fullscreen" );
			AddChild<WIPDisclaimer>();

			AddChild<HintDisplay>();
			AddChild<PlayerInfo>();
			AddChild<InventoryWrapper>();
			AddChild<ChatBox>();

			AddChild<VoiceChatDisplay>();
			AddChild<RoundTimer>();

			AddChild<VoiceList>();

			AddChild<InfoFeed>();
			AddChild<FullScreenHintMenu>();
			AddChild<PostRoundMenu>();
			AddChild<TabMenu>();
		}

		public void AddChildToAliveHud( Panel panel )
		{
			AddChild( panel );
			_aliveHud.Add( panel );
		}

		private void CreateAliveHud()
		{
			if ( _aliveHud.Count != 0 ) return;
			_aliveHud = new()
			{
				AddChild<Crosshair>(),
				AddChild<RoleMenu>(),
				AddChild<DamageIndicator>()
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
			if ( Local.Pawn is not Player player )
			{
				return;
			}

			if ( Instance != null )
			{
				Instance.AliveHudEnabled = player.LifeState == LifeState.Alive && !player.IsForcedSpectator;
			}
		}

		// Use "GeneralHud" as the Panel that displays any s&box popups.
		public override Panel FindPopupPanel()
		{
			return this;
		}
	}
}
