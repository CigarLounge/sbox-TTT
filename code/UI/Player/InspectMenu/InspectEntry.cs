using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class InspectEntry : Panel
{
	public string ActiveText { get; private set; }
	private readonly Image _inspectIcon;
	private readonly Label _iconText;

	public InspectEntry( Panel parent, string iconText, string activeText, string imagePath )
	{
		Parent = parent;

		AddClass( "rounded" );
		AddClass( "text-shadow" );
		AddClass( "background-color-gradient" );

		_inspectIcon = Add.Image();
		_inspectIcon.AddClass( "inspect-icon" );

		_iconText = Add.Label();
		_iconText.AddClass( "icon-text" );

		_iconText.Text = iconText;
		ActiveText = activeText;
		_inspectIcon.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, imagePath, false ) ?? Texture.Load( FileSystem.Mounted, $"/ui/none.png" );
	}

	public void SetActiveText( string text )
	{
		ActiveText = text;
	}

	public void SetImageText( string text )
	{
		_iconText.Text = text;
	}
}
