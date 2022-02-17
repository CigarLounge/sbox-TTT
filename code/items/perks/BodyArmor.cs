using Sandbox;
using TTT.Roles;
using System;
using TTT.Player;

namespace TTT.Items
{
	[Library( "ttt_perk_bodyarmor", Title = "Body Armor" )]
	[Shop( SlotType.Perk, 100, new Type[] { typeof( TraitorRole ) } )]
	[Hammer.Skip]
	public partial class BodyArmor : Perk
	{
	}
}
