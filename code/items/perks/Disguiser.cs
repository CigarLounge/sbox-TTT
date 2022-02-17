using Sandbox;
using TTT.Roles;
using System;
using TTT.Player;
using System.Threading.Tasks;

namespace TTT.Items
{
	[Library( "ttt_perk_disguiser", Title = "Disguiser" )]
	[Shop( SlotType.Perk, 100, new Type[] { typeof( TraitorRole ) } )]
	[Hammer.Skip]
	public partial class Disguiser : Perk
	{
		[Net, Local]
		public bool IsEnabled { get; set; } = false;
		private readonly float _lockOutSeconds = 1f;
		private bool _isLocked = false;

		public override void Simulate( TTTPlayer player )
		{
			if ( Input.Down( InputButton.Grenade ) && !_isLocked )
			{
				if ( Host.IsServer )
				{
					IsEnabled = !IsEnabled;
					_isLocked = true;
				}

				_ = DisguiserLockout();
			}
		}

		public override string ActiveText()
		{
			return IsEnabled ? "ON" : "OFF";
		}

		private async Task DisguiserLockout()
		{
			await GameTask.DelaySeconds( _lockOutSeconds );
			_isLocked = false;
		}
	}
}
