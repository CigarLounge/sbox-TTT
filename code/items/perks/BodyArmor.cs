using System;
using Sandbox;
using TTT.Roles;

namespace TTT.Items
{
	[Library( "Body Armor" )]
	[Buyable( Price = 100 )]
	[Shops( new Type[] { typeof( TraitorRole ) } )]
	[Hammer.Skip]
	public partial class BodyArmor : TTTPerk
	{
		public BodyArmor() : base()
		{

		}
	}
}
