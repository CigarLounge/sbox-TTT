using Sandbox;
using Sandbox.UI;
using System;
using System.Threading.Tasks;

// No namespace since `SWB_Base.PlayerBase.cs` hooks into this class.
public partial class DamageIndicator : Panel
{
	public static DamageIndicator Current;

	public DamageIndicator()
	{
		Current = this;
		StyleSheet.Load( "/ui/alivehud/damageindicator/DamageIndicator.scss" );
	}

	public void OnHit( Vector3 pos )
	{
		var p = new HitPoint( pos );
		p.Parent = this;
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

			var wpos = CurrentView.Rotation.Inverse * (Position.WithZ( 0 ) - CurrentView.Position.WithZ( 0 )).Normal;
			wpos = wpos.WithZ( 0 ).Normal;

			var angle = MathF.Atan2( wpos.y, -1.0f * wpos.x );

			var pt = new PanelTransform();

			pt.AddTranslateX( Length.Percent( -50.0f ) );
			pt.AddTranslateY( Length.Percent( -50.0f ) );
			pt.AddRotation( 0, 0, angle.RadianToDegree() );

			Style.Transform = pt;
			Style.Dirty();

		}

		async Task Lifetime()
		{
			await Task.Delay( 200 );
			AddClass( "dying" );
			await Task.Delay( 500 );
			Delete();
		}
	}
}
