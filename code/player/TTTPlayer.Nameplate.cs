using TTT.Items;
using TTT.UI;

namespace TTT.Player
{
	public partial class TTTPlayer : IEntityHint
	{
		public float HintDistance => 20480f;
		public bool ShowGlow => false;
		public string TextOnTick => "";

		public bool CanHint( TTTPlayer player )
		{
			var disguiser = Perks.Find<Disguiser>();
			return disguiser == null || (disguiser != null && !disguiser.IsEnabled);
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
