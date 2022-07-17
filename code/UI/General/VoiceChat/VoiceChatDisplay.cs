using Sandbox;
using Sandbox.UI;
using System.Linq;

namespace TTT.UI;

public class VoiceChatDisplay : Panel
{
	public static VoiceChatDisplay Instance { get; private set; }

	public VoiceChatDisplay() : base()
	{
		Instance = this;

		StyleSheet.Load( "/UI/General/VoiceChat/VoiceChatDisplay.scss" );
	}

	public void OnVoicePlayed( Client client, float level )
	{
		var entry = ChildrenOfType<VoiceChatEntry>().FirstOrDefault( x => x.Friend.Id == client.PlayerId ) ?? new VoiceChatEntry( this, client );

		entry.Update( level );
	}

	[Event.BuildInput]
	private void BuildInput( InputBuilder input )
	{
		if ( input.Down( InputButton.Voice ) )
			OnVoicePlayed( Local.Client, 1f );
	}
}
