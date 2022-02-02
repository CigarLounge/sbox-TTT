using System;
using System.Threading.Tasks;

using Sandbox;

using TTT.Player;
using TTT.Roles;

namespace TTT.Items
{
	[Library( "ttt_equipment_disguiser", Title = "Disguiser" )]
	[Buyable( Price = 100 )]
	[Shops( new Type[] { typeof( TraitorRole ) } )]
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

			IsEnabled = player.IsDisguised;

			if ( Input.Down( InputButton.Grenade ) && !_isLocked )
			{
				if ( Host.IsServer )
				{
					player.IsDisguised = !player.IsDisguised;
					_isLocked = true;
				}

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
