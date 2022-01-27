using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI
{
	public class WIPDisclaimer : Panel
	{
		public static WIPDisclaimer Instance { get; set; }

		public WIPDisclaimer() : base()
		{
			Instance = this;

			Panel wrapper = new( this );
			wrapper.Style.FlexDirection = FlexDirection.Column;
			wrapper.Style.TextAlign = TextAlign.Center;
			wrapper.AddClass( "centered-vertical-10" );
			wrapper.AddClass( "opacity-medium" );
			wrapper.AddClass( "text-color-info" );
			wrapper.AddClass( "text-shadow" );

			wrapper.Add.Label( "TTT is work-in-progress!" );
			wrapper.Add.Label( "Everything you see is subject to change or is actively being worked on!" );
			wrapper.Add.Label( "Our project is open source, consider contributing at github.com/TTT" );

			AddClass( "fullscreen" );
		}
	}
}
