using Sandbox;
using TTT.Player;

namespace TTT.Items
{
	public abstract class Perk : BaseNetworkable, IItem
	{
		public ItemData Data { get; set; }
		public virtual void Simulate( TTTPlayer player ) { }
		public virtual string ActiveText() { return string.Empty; }

		public Perk()
		{
			if ( string.IsNullOrEmpty( ClassInfo?.Name ) )
				Data = ItemData.All[ClassInfo.Name];
		}
	}
}
