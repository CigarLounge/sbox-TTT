using Sandbox;

namespace TTT.UI;

public class BindingTip
{
	public InputButton Button { get; private set; }
	public string Text { get; private set; }

	public BindingTip( InputButton button, string text )
	{
		Button = button;
		Text = text;
	}
}
