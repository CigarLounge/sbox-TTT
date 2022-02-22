using Sandbox;

namespace TTT;

public partial class FirstPersonCamera : Sandbox.FirstPersonCamera
{
	public override void BuildInput( InputBuilder input )
	{
		var pawn = Local.Pawn as Player;
		if ( !pawn.IsValid() ) return;

		var weapon = pawn.ActiveChild as Weapon;
		if ( weapon.IsValid() )
		{
			var oldPitch = input.ViewAngles.pitch;
			var oldYaw = input.ViewAngles.yaw;
			input.ViewAngles.pitch -= weapon.CurrentRecoilAmount.y * Time.Delta;
			input.ViewAngles.yaw -= weapon.CurrentRecoilAmount.x * Time.Delta;
			weapon.CurrentRecoilAmount -= weapon.CurrentRecoilAmount.WithY( (oldPitch - input.ViewAngles.pitch) * weapon.Info.RecoilRecoveryScale * 1f ).WithX( (oldYaw - input.ViewAngles.yaw) * weapon.Info.RecoilRecoveryScale * 1f );
		}

		base.BuildInput( input );
	}
}
