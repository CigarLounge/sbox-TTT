using System;

using Sandbox;

using TTT.Globals;
using TTT.Player;

namespace TTT.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EquipmentAttribute : CarriableAttribute
    {
        public EquipmentAttribute() : base()
        {

        }
    }

    [Hammer.Skip]
    public abstract class TTTEquipment : BaseCarriable, ICarriableItem
    {
        public string LibraryName { get; }
        public SlotType SlotType { get; } = SlotType.UtilityEquipment;
        public Type DroppedType { get; set; } = null;

        protected TTTEquipment()
        {
            LibraryName = Utils.GetLibraryName(GetType());

            foreach (object obj in GetType().GetCustomAttributes(false))
            {
                if (obj is EquipmentAttribute equipmentAttribute)
                {
                    SlotType = equipmentAttribute.SlotType;
                }
            }

            EnableShadowInFirstPerson = false;
        }

        public void Equip(TTTPlayer player)
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
