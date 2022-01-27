using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTT.Player;

namespace TTT.UI
{
	public partial class BreathIndicator : Panel
	{
		public static BreathIndicator Instance;

		private Panel _breathBar;
		private Label _breathLabel;

		public BreathIndicator() : base()
		{
			Instance = this;

			StyleSheet.Load( "/ui/alivehud/breathindicator/BreathIndicator.scss" );

			AddClass( "text-shadow" );

			_breathBar = new( this );
			_breathBar.AddClass( "breath-bar" );
			_breathBar.AddClass( "center-horizontal" );
			_breathBar.AddClass( "rounded" );

			_breathLabel = Add.Label();
			_breathLabel.AddClass( "breath-label" );

			this.Enabled( true );

			Style.ZIndex = -1;
		}

		public override void Tick()
		{
			base.Tick();

			if ( Local.Pawn is not TTTPlayer player || player.Controller is not DefaultWalkController playerController )
			{
				return;
			}

			if ( playerController.Breath < DefaultWalkController.MAX_BREATH )
			{
				_breathBar.Style.Width = Length.Percent( playerController.Breath );
			}

			_breathLabel.Text = playerController.Breath.ToString( "F0" );

			SetClass( "fade-in", playerController.Breath < DefaultWalkController.MAX_BREATH );
		}
	}
}
