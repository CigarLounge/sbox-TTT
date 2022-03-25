using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class LineBreak : Panel
{
	public LineBreak()
	{
		AddClass( "linebreak" );
	}
}

public class HorizontalLineBreak : Panel
{
	public HorizontalLineBreak()
	{
		AddClass( "horizontallinebreak" );
	}
}


public static class LineBreakConstructor
{
	public static LineBreak LineBreak( this PanelCreator self )
	{
		LineBreak lineBreak = new();

		self.panel.AddChild( lineBreak );

		return lineBreak;
	}

	public static HorizontalLineBreak HorizontalLineBreak( this PanelCreator self )
	{
		HorizontalLineBreak horizontalLineBreak = new();

		self.panel.AddChild( horizontalLineBreak );

		return horizontalLineBreak;
	}
}
