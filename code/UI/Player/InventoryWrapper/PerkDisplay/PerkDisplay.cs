using Sandbox.UI;
using System.Collections.Generic;

namespace TTT.UI;

public partial class PerkDisplay : Panel
{
	private readonly Dictionary<Perk, PerkSlot> _entries = new();

	public override void Tick()
	{
		foreach ( var perk in PlayerCamera.Target.Perks )
		{
			if ( !_entries.ContainsKey( perk ) && PlayerCamera.Target.IsRoleKnown )
			{
				_entries[perk] = AddPerkSlot( perk );
			}
		}

		foreach ( var keyValue in _entries )
		{
			var perk = keyValue.Key;
			var slot = keyValue.Value;

			if ( !PlayerCamera.Target.Perks.Contains( perk ) )
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
