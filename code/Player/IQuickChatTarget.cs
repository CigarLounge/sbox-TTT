using Sandbox;

namespace TTT;

/// <summary>
/// An entity can implement this interface to be detected by quick chat.
/// </summary>
public interface IQuickChatTarget
{
	string QuickChatMessage => DisplayInfo.For( (Entity)this ).Name;
	Color QuickChatColor => Color.FromBytes( 253, 196, 24 );
}
