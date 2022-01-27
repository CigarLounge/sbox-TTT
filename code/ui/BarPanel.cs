using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI
{
	public class BarPanel : Panel
	{
		public readonly Label TextLabel;

		public BarPanel( Panel parent, string text, string name )
		{
			Parent = parent;

			TextLabel = Add.Label( text, name );
		}
	}
}
