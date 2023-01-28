using Sandbox;
using Sandbox.UI;
using System;
using System.Threading.Tasks;

namespace TTT.UI;

public class DamageIndicator : Panel
{
	public static DamageIndicator Instance;

	public DamageIndicator()
	{
		Instance = this;
		StyleSheet.Load( "/UI/Player/DamageIndicator/DamageIndicator.scss" );
	}

	[GameEvent.Player.TookDamage]
	private void OnHit( Player player )
	{
		var info = player.LastDamage;
		var damageLocation = info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.IsValid() ? info.Attacker.Position : player.Position;

		_ = new HitPoint( damageLocation )
		{
			Parent = this
		};
	}

	public class HitPoint : Panel
	{
		public Vector3 Position;

		public HitPoint( Vector3 pos )
		{
			Position = pos;

			_ = Lifetime();
		}

		public override void Tick()
		{
			base.Tick();

			var wpos = Camera.Rotation.Inverse * (Position.WithZ( 0 ) - Camera.Position.WithZ( 0 )).Normal;
			wpos = wpos.WithZ( 0 ).Normal;

			var angle = MathF.Atan2( wpos.y, -1.0f * wpos.x );

			var pt = new PanelTransform();

			pt.AddTranslateX( Length.Percent( -50.0f ) );
			pt.AddTranslateY( Length.Percent( -50.0f ) );
			pt.AddRotation( 0, 0, angle.RadianToDegree() );

			Style.Transform = pt;
		}

		async Task Lifetime()
		{
			await GameTask.Delay( 200 );
			AddClass( "dying" );
			await GameTask.Delay( 500 );
			Delete();
		}
	}
}
