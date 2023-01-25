using Sandbox;

namespace TTT.UI;

public class BindingPrompt
{
	public InputButton Button { get; private set; }
	public string Text { get; private set; }

	public BindingPrompt( InputButton button, string text )
	{
		Button = button;
		Text = text;
	}
}
