using Sandbox;

namespace TTT;

public partial class Player : IQuickChatTarget
{
	string IQuickChatTarget.Message => CanHint( Game.LocalPawn as Player ) ? SteamName : "someone in disguise";
	Color IQuickChatTarget.MessageColor => IsRoleKnown && Role.Team != Team.Traitors ? Role.Color : Color.White;
}
