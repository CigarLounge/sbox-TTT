using Sandbox;

namespace TTT.Items
{
	[Library( "Body Armor" )]
	[Buyable( Price = 100 )]
	[Hammer.Skip]
	public partial class BodyArmor : TTTPerk
	{
		public BodyArmor() : base()
		{

		}
	}
}
