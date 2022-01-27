using Sandbox;

using TTT.Items;
using TTT.UI;

namespace TTT.Player
{
	public partial class TTTPlayer : IEntityHint
	{
		public float HintDistance => 20480f;

		public bool ShowGlow => false;

		public bool CanHint( TTTPlayer player )
		{
			return !IsDisguised;
		}

		public EntityHintPanel DisplayHint( TTTPlayer player )
		{
			return new Nameplate( this );
		}

		public void Tick( TTTPlayer player )
		{

		}
	}
}
