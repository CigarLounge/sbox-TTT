using System;

using Sandbox;

using TTT.Globals;
using TTT.Player;

namespace TTT.Items
{
	public abstract class TTTEquipment : BaseCarriable, ICarriableItem
	{
		public string LibraryTitle { get; }
		public SlotType SlotType { get; } = SlotType.UtilityEquipment;
		public Type DroppedType { get; set; } = null;

		protected TTTEquipment()
		{
			LibraryTitle = Utils.GetLibraryTitle( GetType() );
			EnableShadowInFirstPerson = false;
		}

		public void Equip( TTTPlayer player )
		{
			OnEquip();
		}

		public virtual void OnEquip()
		{

		}

		public void Remove()
		{
			OnRemove();
		}

		public virtual void OnRemove()
		{

		}

		public virtual bool CanDrop() => true;
	}
}
