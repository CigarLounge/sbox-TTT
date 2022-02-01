using System;

using Sandbox;

using TTT.Player;
using TTT.UI;

namespace TTT.Items
{
	public partial class TTTWeaponBaseMelee : SWB_Base.WeaponBaseMelee, ICarriableItem, IEntityHint
	{
		public string LibraryTitle { get; }
		public SlotType SlotType { get; } = SlotType.Secondary;
		public Type DroppedType { get; set; } = null;

		public TTTWeaponBaseMelee() : base()
		{
			LibraryTitle = Utils.GetLibraryTitle( GetType() );

			foreach ( object obj in GetType().GetCustomAttributes( false ) )
			{
				if ( obj is WeaponAttribute weaponAttribute )
				{
					SlotType = weaponAttribute.SlotType;
				}
			}

			EnableShadowInFirstPerson = false;

			Tags.Add( IItem.ITEM_TAG );
		}

		public override void Simulate( Client owner )
		{
			TTTWeaponBaseGeneric.Simulate( owner, DroppedType, Primary );

			base.Simulate( owner );
		}

		public new bool CanDrop() => true;

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

		public float HintDistance => 80f;

		public string TextOnTick => TTTWeaponBaseGeneric.PickupText( LibraryTitle );

		public bool CanHint( TTTPlayer client )
		{
			return true;
		}

		public EntityHintPanel DisplayHint( TTTPlayer client )
		{
			return new Hint( TextOnTick );
		}

		public void Tick( TTTPlayer player )
		{
			TTTWeaponBaseGeneric.Tick( player, this );
		}
	}
}
