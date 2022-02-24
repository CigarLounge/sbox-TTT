
using Sandbox;
using TTT.UI;

namespace TTT;

public partial class Player
{
	private void DisplayPlayerVoiceChat()
	{
		if ( Input.Down( InputButton.Voice ) )
		{
			VoiceChatDisplay.Instance?.OnVoicePlayed( Client, 1f );
		}
	}
}