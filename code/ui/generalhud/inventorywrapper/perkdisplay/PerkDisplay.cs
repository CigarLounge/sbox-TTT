using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using TTT.Items;
using TTT.Player;

namespace TTT.UI
{
	public class PerkDisplay : Panel
	{
		private readonly Dictionary<Perk, PerkSlot> _entries = new();

		public PerkDisplay()
		{
			StyleSheet.Load( "/ui/generalhud/inventorywrapper/perkdisplay/PerkDisplay.scss" );
		}

		public override void Tick()
		{
			base.Tick();

			if ( Local.Pawn is not TTTPlayer player )
			{
				return;
			}

			for ( int i = 0; i < player.Perks.Count; ++i )
			{
				var perk = player.Perks.Get( i );
				if ( !_entries.ContainsKey( perk ) )
				{
					_entries[perk] = AddPerkSlot( perk );
				}
			}

			foreach ( var keyValue in _entries )
			{
				var perk = keyValue.Key;
				var slot = keyValue.Value;

				if ( !player.Perks.Contains( perk ) )
				{
					_entries.Remove( perk );
					slot?.Delete();
				}
			}
		}

		public PerkSlot AddPerkSlot( Perk perk )
		{
			var perkSlot = new PerkSlot( perk );
			AddChild( perkSlot );
			return perkSlot;
		}

		public class PerkSlot : Panel
		{
			public PerkSlot( Perk perk )
			{
				var item = perk as IItem;
				switch ( perk.GetCategory() )
				{
					case PerkCategory.Passive:
						Add.Label( item.GetItemData().Title );
						break;
				}
			}

			public override void Tick()
			{
				base.Tick();
			}
		}
	}
}
