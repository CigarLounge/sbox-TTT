using Sandbox;
using Sandbox.UI;
using System.Linq;

namespace TTT.UI;

[UseTemplate]
public class VoiceChatDisplay : Panel
{
	public static VoiceChatDisplay Instance { get; private set; }

	public VoiceChatDisplay() => Instance = this;

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
