using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

using TTT.Items;
using TTT.Player;

namespace TTT.UI
{
	public class Effects : Panel
	{
		private readonly List<Effect> _effectList = new();

		public Effects()
		{
			StyleSheet.Load( "/ui/generalhud/inventorywrapper/effects/Effects.scss" );

			if ( Local.Pawn is not TTTPlayer player )
			{
				return;
			}

			PerksInventory perks = player.Inventory.Perks;

			for ( int i = 0; i < perks.Count(); i++ )
			{
				AddEffect( perks.Get( i ) );
			}
		}

		public void AddEffect( TTTPerk perk )
		{
			_effectList.Add( new Effect( this, perk ) );
		}

		public void RemoveEffect( TTTPerk perk )
		{
			foreach ( Effect effect in _effectList )
			{
				if ( effect.Item.LibraryName == perk.LibraryName )
				{
					_effectList.Remove( effect );
					effect.Delete();

					return;
				}
			}
		}
	}
}
