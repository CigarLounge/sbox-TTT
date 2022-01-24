using Sandbox;

using TTTReborn.Events;
using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        [ClientRpc]
        private void ClientShowFlashlightLocal(bool shouldShow)
        {
            ShowFlashlight(shouldShow);
        }

        [ClientRpc]
        public void ClientSetAmmo(SWB_Base.AmmoType ammoType, int amount)
        {
            SetAmmo(ammoType, amount);
        }

        [ClientRpc]
        public void ClientClearAmmo()
        {
            ClearAmmo();
        }

        [ClientRpc]
        public void ClientAddPerk(string perkName)
        {
            TTTPerk perk = Utils.GetObjectByType<TTTPerk>(Utils.GetTypeByLibraryName<TTTPerk>(perkName));

            if (perk == null)
            {
                return;
            }

            Inventory.TryAdd(perk, deleteIfFails: true, makeActive: false);
        }

        [ClientRpc]
        public void ClientRemovePerk(string perkName)
        {
            TTTPerk perk = Utils.GetObjectByType<TTTPerk>(Utils.GetTypeByLibraryName<TTTPerk>(perkName));

            if (perk == null)
            {
                return;
            }

            Inventory.Perks.Take(perk);
        }

        [ClientRpc]
        public void ClientClearPerks()
        {
            Inventory.Perks.Clear();
        }

        [ClientRpc]
        public void ClientAnotherPlayerDidDamage(Vector3 position, float inverseHealth)
        {
            Sound.FromScreen("dm.ui_attacker")
                .SetPitch(1 + inverseHealth * 1)
                .SetPosition(position);
        }

        [ClientRpc]
        public void ClientTookDamage(Vector3 position, float damage)
        {
            Event.Run(TTTEvent.Player.TakeDamage, this, damage);
        }


        [ClientRpc]
        public void ClientInitialSpawn()
        {
            Event.Run(TTTEvent.Player.InitialSpawn, Client);
        }
    }
}
