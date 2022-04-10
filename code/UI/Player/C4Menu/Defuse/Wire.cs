using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class Wire : Panel
{
	private Label Number { get; set; }
	private Panel Line { get; set; }
	private readonly int _height = 80;

	public Wire( int num, Color color )
	{
		Number.Text = $"{num}";
		Line.Style.BackgroundColor = color;
		Line.Style.Height = _height;
	}

	public void Cut()
	{
		Line.Style.Height = _height / 2;
	}
}
