namespace TTT.UI;

public class BindingPrompt
{
	public string Action { get; private set; }
	public string Text { get; private set; }

	public BindingPrompt( string action, string text )
	{
		Action = action;
		Text = text;
	}
}
