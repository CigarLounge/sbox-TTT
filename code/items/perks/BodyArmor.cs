using Sandbox;

namespace TTT.Items
{
    [Library("perk_bodyarmor")]
    [Buyable(Price = 100)]
    [Hammer.Skip]
    public partial class BodyArmor : TTTPerk
    {
        public BodyArmor() : base()
        {

        }
    }
}
