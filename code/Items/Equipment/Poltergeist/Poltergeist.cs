using Sandbox;

namespace TTT;

// WIP, not currently added to any shop.
[Category( "Equipment" )]
[ClassName( "ttt_equipment_poltergeist" )]
[Title( "Poltergeist" )]
public class Poltergeist : Weapon
{
	private TraceResult _trace;

	public override void Simulate( Client client )
	{
		base.Simulate( client );

		_trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.MaxHintDistance )
			.Ignore( this )
			.Ignore( Owner )
			.Run();
	}

	protected override void AttackPrimary()
	{
		if ( !HasValidPlacement() || AmmoClip == 0 )
			return;

		AmmoClip--;
		Owner.SetAnimParameter( "b_attack", true );
		PlaySound( Info.FireSound );
		AttachEnt();
	}

	private void AttachEnt()
	{
		var ent = new PoltergeistEntity { Position = _trace.EndPosition };
		ent.SetParent( _trace.Entity );
	}

	private bool HasValidPlacement()
	{
		return _trace.Hit && _trace.Entity.PhysicsGroup is not null;
	}
}
