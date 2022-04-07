using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class InspectEntry : Panel
{
	public string ActiveText { get; private set; }
	private readonly Image _inspectIcon;
	private readonly Label _inspectLabel;

	public InspectEntry( Panel parent ) : base( parent )
	{
		Parent = parent;

		AddClass( "rounded" );
		AddClass( "text-shadow" );
		AddClass( "background-color-secondary" );

		_inspectIcon = Add.Image();
		_inspectIcon.AddClass( "inspect-icon" );

		_inspectLabel = Add.Label();
		_inspectLabel.AddClass( "quick-label" );
	}

	public void SetTexture( Texture texture )
	{
		_inspectIcon.Style.BackgroundImage = texture ?? Texture.Load( FileSystem.Mounted, $"/ui/none.png" );
	}

	public void SetImage( string imagePath )
	{
		_inspectIcon.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, imagePath, false ) ?? Texture.Load( FileSystem.Mounted, $"/ui/none.png" );
	}

	public void SetActiveText( string text )
	{
		ActiveText = text;
	}

	public void SetImageText( string text )
	{
		_inspectLabel.Text = text;
	}
}
