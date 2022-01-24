using System.Threading.Tasks;

using Sandbox;

namespace TTT.Items
{
    [Library("perk_disguiser")]
    [Buyable(Price = 100)]
    [Hammer.Skip]
    public partial class Disguiser : TTTPerk
    {
        [Net]
        public bool IsEnabled { get; set; }

        private readonly float _lockOutSeconds = 1f;
        private bool _isLocked = false;

        public Disguiser() : base()
        {
            IsEnabled = false;
        }

        public override void Simulate(Client owner)
        {
            if (Input.Down(InputButton.Grenade) && !_isLocked)
            {
                if (Host.IsServer)
                {
                    IsEnabled = !IsEnabled;
                    _isLocked = true;
                }
                _ = DisguiserLockout();
            }
        }

        private async Task DisguiserLockout()
        {
            await GameTask.DelaySeconds(_lockOutSeconds);
            _isLocked = false;
        }
    }
}
