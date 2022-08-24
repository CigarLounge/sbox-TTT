using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public class Hud : RootPanel
{
	public Hud()
	{
		Local.Hud = this;

		StyleSheet.Load( "/UI/Hud.scss" );
		AddClass( "panel" );
		AddClass( "fullscreen" );

		Init();
	}

	private void Init()
	{
		AddChild<HintDisplay>();
		AddChild<PlayerInfo>();
		AddChild<InventoryWrapper>();
		AddChild<ChatBox>();
		AddChild<QuickChat>();
		AddChild<VoiceChatDisplay>();
		AddChild<RoundTimer>();
		AddChild<VoiceList>();
		AddChild<InfoFeed>();
		AddChild<FullScreenHintMenu>();
		AddChild<TabMenus>();
		AddChild<Crosshair>();
		AddChild<CarriableHint>();
		AddChild<RoleMenu>();
		AddChild<DamageIndicator>();
		AddChild<WorldPoints>();
		AddChild<SpectatingHint>();
	}

	[Event.Hotload]
	private void OnHotReload()
	{
		DeleteChildren( true );
		Init();
	}
}
