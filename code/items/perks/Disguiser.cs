using System.Threading.Tasks;

using Sandbox;

using TTT.Player;

namespace TTT.Items
{
	[Library( "Disguiser" )]
	[Buyable( Price = 100 )]
	[Hammer.Skip]
	public partial class Disguiser : TTTBoolPerk
	{
		public override bool IsEnabled { get; set; } = false;

		private readonly float _lockOutSeconds = 1f;
		private bool _isLocked = false;

		public Disguiser() : base()
		{
			if ( Owner is TTTPlayer player )
			{
				IsEnabled = player.IsDisguised;
			}
		}

		public override void OnRemove()
		{
			if ( Owner is TTTPlayer player )
			{
				player.IsDisguised = false;
			}
		}

		public override void Simulate( Client owner )
		{
			if ( owner.Pawn is not TTTPlayer player )
			{
				return;
			}

			if ( Input.Down( InputButton.Grenade ) && !_isLocked )
			{
				if ( Host.IsServer )
				{
					player.IsDisguised = !player.IsDisguised;
					_isLocked = true;
				}

				IsEnabled = player.IsDisguised;

				_ = DisguiserLockout();
			}
		}

		private async Task DisguiserLockout()
		{
			await GameTask.DelaySeconds( _lockOutSeconds );
			_isLocked = false;
		}
	}
}
