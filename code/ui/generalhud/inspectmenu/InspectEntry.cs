using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI
{
	public class InspectEntry : Panel
	{
		public string Text;
		private readonly Image _inspectIcon;
		private readonly Label _inspectQuickLabel;

		public InspectEntry( Panel parent ) : base( parent )
		{
			Parent = parent;

			AddClass( "rounded" );
			AddClass( "text-shadow" );
			AddClass( "background-color-secondary" );

			_inspectIcon = Add.Image();
			_inspectIcon.AddClass( "inspect-icon" );

			_inspectQuickLabel = Add.Label();
			_inspectQuickLabel.AddClass( "quick-label" );
		}

		public void SetData( string imagePath, string text )
		{
			_inspectQuickLabel.Text = text;
			_inspectIcon.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, imagePath, false ) ?? Texture.Load( FileSystem.Mounted, $"/ui/none.png" );
			Text = text;
		}

		public void SetQuickInfo( string text )
		{
			_inspectQuickLabel.Text = text;
			Text = text;
		}
	}
}
