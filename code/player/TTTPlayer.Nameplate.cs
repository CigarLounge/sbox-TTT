using TTT.Items;
using TTT.UI;

namespace TTT.Player
{
    public partial class TTTPlayer : IEntityHint
    {
        public float HintDistance => 20480f;

        public bool ShowGlow => false;

        public bool CanHint(TTTPlayer player)
        {
            if (Inventory.Perks.Has(Utils.GetLibraryName(typeof(Disguiser))))
            {
                var disguiser = Inventory.Perks.Find<Disguiser>("perk_disguiser");
                if (disguiser != null && disguiser.IsEnabled)
                {
                    return false;
                }
            }
            return true;
        }

        public EntityHintPanel DisplayHint(TTTPlayer player)
        {
            return new Nameplate(this);
        }

        public void Tick(TTTPlayer player)
        {

        }
    }
}
