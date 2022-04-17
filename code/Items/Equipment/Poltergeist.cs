using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_poltergeist", Title = "Poltergeist" )]
public class Poltergeist : Weapon
{
	public override string SlotText => AmmoClip.ToString();

	protected override void OnHit( TraceResult trace )
	{

	}
}
