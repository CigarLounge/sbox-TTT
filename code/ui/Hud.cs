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
		RootPanel.AddClass( "fullscreen" );

		Init();
	}

	private void Init()
	{
		RootPanel.AddChild<WIPDisclaimer>();
		RootPanel.AddChild<HintDisplay>();
		RootPanel.AddChild<PlayerInfo>();
		RootPanel.AddChild<InventoryWrapper>();
		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<VoiceChatDisplay>();
		RootPanel.AddChild<RoundTimer>();
		RootPanel.AddChild<VoiceList>();
		RootPanel.AddChild<InfoFeed>();
		RootPanel.AddChild<FullScreenHintMenu>();
		RootPanel.AddChild<PostRoundMenu>();
		RootPanel.AddChild<TabMenu>();
		RootPanel.AddChild<Crosshair>();
		RootPanel.AddChild<RoleMenu>();
		RootPanel.AddChild<DamageIndicator>();
	}

	[Event.Hotload]
	private void OnHotReload()
	{
		if ( !IsClient ) return;

		Local.Hud.DeleteChildren( true );
		Init();
	}
}
