using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class PerkDisplay : Panel
{
	private readonly Dictionary<Perk, PerkSlot> _entries = new();

	public PerkDisplay()
	{
		StyleSheet.Load( "/ui/player/inventorywrapper/perkdisplay/PerkDisplay.scss" );
	}

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

	public class PerkSlot : Panel
	{
		private readonly Perk _perk;
		private readonly Label _name;
		private readonly Image _image;
		private readonly Label _activeLabel;

		public PerkSlot( Perk perk )
		{
			_perk = perk;

			var panel = Add.Panel( "icon-panel" );
			_image = panel.Add.Image();
			_image.SetImage( perk.Info.Icon );

			_activeLabel = panel.Add.Label( "", "active" );
			_activeLabel.AddClass( "text-shadow" );
			_activeLabel.AddClass( "centered" );

			_name = Add.Label( perk.Info.Title, "name" );
			_name.AddClass( "text-shadow" );
		}

		public override void Tick()
		{
			base.Tick();
			_activeLabel.Text = _perk.ActiveText();
		}
	}
}
