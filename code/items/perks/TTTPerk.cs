using Sandbox;

using TTT.Player;
using TTT.UI;

namespace TTT.Items
{
	public abstract class TTTPerk
	{
		public Entity Owner { get; private set; }

		public void Equip( TTTPlayer player )
		{
			Owner = player;

			OnEquip();
		}

		public virtual void OnEquip()
		{
			if ( Host.IsClient )
			{
				InventoryWrapper.Instance.Effects.AddEffect( this );
			}
		}

		public void Remove()
		{
			OnRemove();
		}

		public virtual void OnRemove()
		{
			Owner = null;
		}

		public void Delete()
		{
			if ( Host.IsClient )
			{
				InventoryWrapper.Instance.Effects.RemoveEffect( this );
			}
		}

		public virtual void Simulate( Client owner )
		{

		}
	}
}
