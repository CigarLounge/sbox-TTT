using Sandbox;

namespace TTT;

[Category( "Equipment" )]
[ClassName( "ttt_equipment_poltergeist" )]
[Title( "Poltergeist" )]
public class Poltergeist : Weapon
{
	private const float MaxPlacementDistance = 1250f;
	private TraceResult _trace;

	public override void Simulate( Client client )
	{
		base.Simulate( client );

		_trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * MaxPlacementDistance )
			.Ignore( this )
			.Ignore( Owner )
			.Run();
	}

	protected override void AttackPrimary()
	{
		// TODO: Consider sound effects, weapon effects, etc.
		if ( AmmoClip == 0 )
			return;

		AmmoClip--;
		Owner.SetAnimParameter( "b_attack", true );
		PlaySound( Info.FireSound );
		AttachEnt();
	}

	protected override bool CanPrimaryAttack()
	{
		return Input.Pressed( InputButton.PrimaryAttack ) && HasValidPlacement();
	}

	private void AttachEnt()
	{
		var ent = new PoltergeistEntity { Position = _trace.EndPosition };
		ent.SetParent( _trace.Entity );
	}

	private bool HasValidPlacement()
	{
		return _trace.Hit && _trace.Entity is ModelEntity;
	}
}