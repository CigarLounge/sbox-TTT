using System.Collections.Generic;
using Sandbox;
using TTT.Roles;

namespace TTT.Items
{
	[Library( "ttt_perk_bodyarmor", Title = "Body Armor" )]
	[Hammer.Skip]
	public partial class BodyArmor : TTTPerk, IItem
	{
		public LibraryData GetLibraryData() { return _data; }
		public List<TTTRole> ShopAvailability => new() { new TraitorRole() };
		private readonly LibraryData _data = new( typeof( BodyArmor ) );

		public BodyArmor() : base()
		{

		}
	}
}
