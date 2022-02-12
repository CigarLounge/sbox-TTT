// using System;
// using System.Threading.Tasks;

// using Sandbox;

// using TTT.Player;
// using TTT.Roles;

// namespace TTT.Items
// {
// 	[Library( "ttt_perk_disguiser", Title = "Disguiser" )]
// 	[Shop( SlotType.Perk, 100, new Type[] { typeof( TraitorRole ) } )]
// 	[Hammer.Skip]
// 	public partial class Disguiser : TTTBoolPerk, IItem
// 	{
// 		public ItemData GetItemData() { return _data; }
// 		private readonly ItemData _data = new( typeof( Disguiser ) );

// 		public override bool IsEnabled { get; set; } = false;

// 		private readonly float _lockOutSeconds = 1f;
// 		private bool _isLocked = false;

// 		public Disguiser() : base()
// 		{
// 			if ( Owner is TTTPlayer player )
// 			{
// 				IsEnabled = player.IsDisguised;
// 			}
// 		}

// 		public override void OnRemove()
// 		{
// 			if ( Owner is TTTPlayer player )
// 			{
// 				player.IsDisguised = false;
// 			}
// 		}

// 		public override void Simulate( Client owner )
// 		{
// 			if ( owner.Pawn is not TTTPlayer player )
// 			{
// 				return;
// 			}

// 			IsEnabled = player.IsDisguised;

// 			if ( Input.Down( InputButton.Grenade ) && !_isLocked )
// 			{
// 				if ( Host.IsServer )
// 				{
// 					player.IsDisguised = !player.IsDisguised;
// 					_isLocked = true;
// 				}

// 				_ = DisguiserLockout();
// 			}
// 		}

// 		private async Task DisguiserLockout()
// 		{
// 			await GameTask.DelaySeconds( _lockOutSeconds );
// 			_isLocked = false;
// 		}
// 	}
// }

