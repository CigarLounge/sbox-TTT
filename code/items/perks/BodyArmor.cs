using Sandbox;
using TTT.Roles;
using System;

namespace TTT.Items
{
	[Library( "ttt_perk_bodyarmor", Title = "Body Armor" )]
	[Shop( SlotType.Perk, 100, new Type[] { typeof( TraitorRole ) } )]
	[Hammer.Skip]
	public partial class BodyArmor : Perk, IItem
	{
		public ItemData GetItemData() { return _data; }
		private readonly ItemData _data = new( typeof( BodyArmor ) );
		public override PerkCategory GetCategory() { return PerkCategory.Passive; }
	}
}
