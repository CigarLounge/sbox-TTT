using Sandbox;
using System;

namespace TTT;

[Category( "Equipment" )]
[ClassName( "ttt_equipment_binoculars" )]
[Title( "Binoculars" )]
public partial class Binoculars : Carriable
{
	[Net, Local, Predicted]
	private int ZoomLevel { get; set; }

	public bool IsZoomed => ZoomLevel > 0;
	private Corpse _corpse;
	private float _defaultFOV;

	public override void ActiveStart( Player player )
	{
		base.ActiveStart( player );

		_defaultFOV = Camera.FieldOfView;
	}

	public override void ActiveEnd( Player player, bool dropped )
	{
		base.ActiveEnd( player, dropped );

		_corpse = null;
		ZoomLevel = 0;
	}

	public override void Simulate( IClient client )
	{
		if ( Input.Pressed( InputAction.SecondaryAttack ) )
			ChangeZoomLevel();

		if ( Input.Pressed( InputAction.Reload ) )
		{
			// Reset zoom.
			ZoomLevel = 4;
			ChangeZoomLevel();
		}

		if ( !IsZoomed )
			return;

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.MaxHintDistance )
			.Ignore( this )
			.Ignore( Owner )
			.WithTag( "interactable" )
			.Run();

		_corpse = trace.Entity as Corpse;

		if ( !Game.IsServer || !_corpse.IsValid() )
			return;

		if ( Input.Pressed( InputAction.PrimaryAttack ) )
			_corpse.Search( Owner, Input.Down( InputAction.Run ), false );
	}

	public override void BuildInput()
	{
		base.BuildInput();

		if ( IsZoomed )
			Owner.ViewAngles = Angles.Lerp( Owner.OriginalViewAngles, Owner.ViewAngles, 0.5f / MathF.Pow( 2.5f, ZoomLevel ) );
	}

	protected override void DestroyHudElements()
	{
		base.DestroyHudElements();

		Camera.FieldOfView = _defaultFOV;
	}

	private void ChangeZoomLevel()
	{
		if ( ZoomLevel >= 4 )
		{
			_corpse = null;
			ZoomLevel = 0;
			Camera.FieldOfView = _defaultFOV;

			return;
		}

		if ( Game.IsClient )
			PlaySound( "scope_in" );

		ZoomLevel++;
		Camera.FieldOfView = 40f / MathF.Pow( 2.5f, ZoomLevel );
	}
}
