using Sandbox;
using System.Threading.Tasks;

namespace TTT.Items;

[Hammer.Skip]
[Library( "ttt_perk_disguiser", Title = "Disguiser" )]
public partial class Disguiser : Perk
{
	public override string SlotText => IsEnabled ? "ON" : "OFF";

	[Net, Local]
	public bool IsEnabled { get; set; } = false;
	private readonly float _lockOutSeconds = 1f;
	private bool _isLocked = false;

	public override void Simulate( Client client )
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

	private async Task DisguiserLockout()
	{
		await GameTask.DelaySeconds( _lockOutSeconds );
		_isLocked = false;
	}
}
