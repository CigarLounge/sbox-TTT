using Sandbox;
using System.Threading.Tasks;

namespace TTT;

[Category( "Perks" )]
[ClassName( "ttt_perk_disguiser" )]
[Title( "Disguiser" )]
public partial class Disguiser : Perk
{
	[Net, Local]
	public bool IsActive { get; set; } = false;

	public override string SlotText => IsActive ? "ON" : "OFF";
	private readonly float _lockOutSeconds = 1f;
	private bool _isLocked = false;

	public override void Simulate( Client client )
	{
		if ( Input.Down( InputButton.Grenade ) && !_isLocked )
		{
			if ( Host.IsServer )
			{
				IsActive = !IsActive;
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
