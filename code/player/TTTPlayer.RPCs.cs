using Sandbox;

using TTT.Events;
using TTT.Globals;
using TTT.Items;
using TTT.UI;

namespace TTT.Player
{
	public partial class TTTPlayer
	{
		[ClientRpc]
		private void ClientShowFlashlightLocal( bool shouldShow )
		{
			ShowFlashlight( shouldShow );
		}

		[ClientRpc]
		public void ClientSetAmmo( SWB_Base.AmmoType ammoType, int amount )
		{
			SetAmmo( ammoType, amount );
		}

		[ClientRpc]
		public void ClientClearAmmo()
		{
			ClearAmmo();
		}

		[ClientRpc]
		public void ClientAddPerk( string perkName )
		{
			TTTPerk perk = Utils.GetObjectByType<TTTPerk>( Utils.GetTypeByLibraryTitle<TTTPerk>( perkName ) );

			if ( perk == null )
			{
				return;
			}

			Inventory.TryAdd( perk as IItem, deleteIfFails: true, makeActive: false );
		}

		[ClientRpc]
		public void ClientRemovePerk( string perkName )
		{
			TTTPerk perk = Utils.GetObjectByType<TTTPerk>( Utils.GetTypeByLibraryTitle<TTTPerk>( perkName ) );

			if ( perk == null )
			{
				return;
			}

			Inventory.Perks.Take( perk as IItem );
		}

		[ClientRpc]
		public void ClientClearPerks()
		{
			Inventory.Perks.Clear();
		}

		[ClientRpc]
		public void ClientAnotherPlayerDidDamage( Vector3 position, float inverseHealth )
		{
			Sound.FromScreen( "dm.ui_attacker" )
				.SetPitch( 1 + inverseHealth * 1 )
				.SetPosition( position );
		}

		[ClientRpc]
		public void ClientTookDamage( Vector3 position, float damage )
		{
			Event.Run( TTTEvent.Player.TakeDamage, this, damage );
		}


		[ClientRpc]
		public void ClientInitialSpawn()
		{
			Event.Run( TTTEvent.Player.InitialSpawn, Client );
		}
	}
}
