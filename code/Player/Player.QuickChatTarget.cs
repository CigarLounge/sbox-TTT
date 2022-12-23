namespace TTT;

public partial class Player : IQuickChatTarget
{
	public string QuickChatMessage => SteamName;
	public Color QuickChatColor => IsRoleKnown && Role.Team != Team.Traitors ? Role.Color : Color.White;
}
