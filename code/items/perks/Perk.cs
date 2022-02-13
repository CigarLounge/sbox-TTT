using Sandbox;
using TTT.Player;

namespace TTT.Items
{
	public abstract class Perk : BaseNetworkable
	{
		public virtual void Simulate( TTTPlayer player ) { }
		public virtual string ActiveText() { return string.Empty; }
	}
}
