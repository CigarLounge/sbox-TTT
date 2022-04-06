using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class C4ArmMenu : EntityHintPanel
{
	public int Timer { get; set; } = 0;

	private Label TimerDisplay { get; set; }

	private readonly C4Entity _c4;

	public C4ArmMenu() { }

	public C4ArmMenu( C4Entity c4 )
	{
		_c4 = c4;
	}

	public void Arm()
	{
		ArmC4( _c4.NetworkIdent, Timer );
	}

	public void Destroy()
	{
		DeleteC4( _c4.NetworkIdent );
	}

	public override void Tick()
	{
		TimerDisplay.Text = TimeSpan.FromSeconds( Timer ).ToString( "mm':'ss" );
	}

	[ServerCmd]
	public static void ArmC4( int networkIdent, int time )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player )
			return;

		var entity = Entity.FindByIndex( networkIdent );

		if ( entity is null || entity is not C4Entity c4 )
			return;

		c4.Arm( time );
	}

	[ServerCmd]
	public static void DeleteC4( int networkIdent )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player )
			return;

		var entity = Entity.FindByIndex( networkIdent );

		if ( entity is null || entity is not C4Entity c4 )
			return;

		c4.Delete();
	}
}
