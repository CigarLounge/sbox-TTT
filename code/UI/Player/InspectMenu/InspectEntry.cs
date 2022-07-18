using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class InspectEntry : Panel
{
	public string ActiveText { get; set; }
	private Image InspectIcon { get; set; }
	private Label IconText { get; set; }

	public InspectEntry( Panel parent, string iconText, string activeText, string imagePath )
	{
		Parent = parent;
		IconText.Text = iconText;
		ActiveText = activeText;
		InspectIcon.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, imagePath, false ) ?? Texture.Load( FileSystem.Mounted, $"/ui/none.png" );
	}

	public void SetImageText( string text )
	{
		IconText.Text = text;
	}
}
