using System.Collections.Generic;

namespace TTT;

public static class InputAction
{
	public static readonly List<string> All = new()
	{
		PrimaryAttack,
		SecondaryAttack,
		Reload,
		Drop,
		Flashlight,
		Grenade,
		Use,
		Run,
		View,
		Jump,
		Duck,
		Chat,
		Voice,
		Zoom,
		Menu,
		Score,
		Slot0,
		Slot1,
		Slot2,
		Slot3,
		Slot4,
		Slot5,
		Slot6,
		Slot7,
		Slot8,
		Slot9
	};

	public const string PrimaryAttack = "attack1";
	public const string SecondaryAttack = "attack2";
	public const string Reload = "reload";
	public const string Drop = "drop";
	public const string Flashlight = "flashlight";
	public const string Grenade = "grenade";
	public const string Use = "use";
	public const string Run = "run";
	public const string View = "view";
	public const string Jump = "jump";
	public const string Duck = "duck";
	public const string Chat = "chat";
	public const string Voice = "voice";
	public const string Zoom = "zoom";
	public const string Menu = "menu";
	public const string Score = "score";
	public const string Slot0 = "slot0";
	public const string Slot1 = "slot1";
	public const string Slot2 = "slot2";
	public const string Slot3 = "slot3";
	public const string Slot4 = "slot4";
	public const string Slot5 = "slot5";
	public const string Slot6 = "slot6";
	public const string Slot7 = "slot7";
	public const string Slot8 = "slot8";
	public const string Slot9 = "slot9";
}
