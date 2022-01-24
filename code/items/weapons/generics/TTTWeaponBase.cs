using System;

using Sandbox;

using TTT.Globalization;
using TTT.Player;
using TTT.UI;

namespace TTT.Items
{
    [Hammer.Skip]
    public partial class TTTWeaponBase : SWB_Base.WeaponBase, ICarriableItem, IEntityHint
    {
        public string LibraryName { get; }
        public SlotType SlotType { get; } = SlotType.Secondary;
        public Type DroppedType { get; set; } = null;

        public TTTWeaponBase() : base()
        {
            LibraryName = Utils.GetLibraryName(GetType());

            foreach (object obj in GetType().GetCustomAttributes(false))
            {
                if (obj is WeaponAttribute weaponAttribute)
                {
                    SlotType = weaponAttribute.SlotType;
                }
            }

            EnableShadowInFirstPerson = false;

            Tags.Add(IItem.ITEM_TAG);
        }

        public override void Simulate(Client owner)
        {
            TTTWeaponBaseGeneric.Simulate(owner, DroppedType, Primary);

            base.Simulate(owner);
        }

        public new bool CanDrop() => true;

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

        public float HintDistance => 80f;

        public TranslationData TextOnTick => new("GENERIC_PICKUP", Input.GetKeyWithBinding("+iv_use").ToUpper(), new TranslationData(LibraryName.ToUpper()));

        public bool CanHint(TTTPlayer client)
        {
            return true;
        }

        public EntityHintPanel DisplayHint(TTTPlayer client)
        {
            return new Hint(TextOnTick);
        }

        public void Tick(TTTPlayer player)
        {
            TTTWeaponBaseGeneric.Tick(player, this);
        }
    }
}
