using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer : IEntityHint
    {
        public float HintDistance => 400f;

        public bool ShowGlow => false;

        public bool CanHint(TTTPlayer client)
        {
            return true;
        }

        public EntityHintPanel DisplayHint(TTTPlayer client)
        {
            return new Nameplate(this);
        }

        public void Tick(TTTPlayer player)
        {

        }
    }
}
