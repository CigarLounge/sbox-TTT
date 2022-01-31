using System;

using Sandbox;

using TTT.Globals;
using TTT.Player;
using TTT.UI;

namespace TTT.Items
{
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
	public class PerkAttribute : ItemAttribute
	{
		public PerkAttribute() : base()
		{

		}
	}

	public abstract class TTTPerk : IItem
	{
		public string LibraryName { get; }
		public Entity Owner { get; private set; }

		protected TTTPerk()
		{
			LibraryName = Utils.GetLibraryName( GetType() );
		}

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
