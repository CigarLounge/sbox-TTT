using System.Collections.Generic;

namespace TTT;

public static class InputAction
{
	public static readonly List<string> All = new()
	{
		PrimaryAttack,
		SecondaryAttack,
		Use,
		Run,
		View
	};

	public static string PrimaryAttack = "attack1";
	public static string SecondaryAttack = "attack2";
	public static string Reload = "reload";
	public static string Drop = "drop";
	public static string Flashlight = "flashlight";
	public static string Grenade = "grenade";
	public static string Use = "use";
	public static string Run = "run";
	public static string View = "view";
	public static string Jump = "jump";
	public static string Duck = "duck";
	public static string Chat = "chat";
	public static string Zoom = "zoom";
	public static string Menu = "menu";
	public static string Score = "score";
	public static string Slot0 = "slot0";
	public static string Slot1 = "slot1";
	public static string Slot2 = "slot2";
	public static string Slot3 = "slot3";
	public static string Slot4 = "slot4";
	public static string Slot5 = "slot5";
	public static string Slot6 = "slot6";
	public static string Slot7 = "slot7";
	public static string Slot8 = "slot8";
	public static string Slot9 = "slot9";
}
