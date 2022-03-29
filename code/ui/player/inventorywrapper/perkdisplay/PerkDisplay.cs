using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class PerkDisplay : Panel
{
	private readonly Dictionary<Perk, PerkSlot> _entries = new();

	public override void Tick()
	{
		base.Tick();

		if ( Local.Pawn is not Player player )
			return;

		for ( int i = 0; i < player.Perks.Count; ++i )
		{
			var perk = player.Perks.Get( i );
			if ( !_entries.ContainsKey( perk ) )
			{
				_entries[perk] = AddPerkSlot( perk );
			}
		}

		foreach ( var keyValue in _entries )
		{
			var perk = keyValue.Key;
			var slot = keyValue.Value;

			if ( !player.Perks.Contains( perk ) )
			{
				_entries.Remove( perk );
				slot?.Delete();
			}
		}
	}

	public PerkSlot AddPerkSlot( Perk perk )
	{
		var perkSlot = new PerkSlot( perk );
		AddChild( perkSlot );
		return perkSlot;
	}
}
