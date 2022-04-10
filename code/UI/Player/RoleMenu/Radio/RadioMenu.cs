using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using TTT.Entities;

namespace TTT.UI;

public partial class RadioMenu : Panel
{
	// TODO: Let's hook up some of the other sounds like death, fire, explore, etc.
	private static readonly Dictionary<string, List<string>> _sounds = new()
	{
		{ "Pistol", new List<string>() { "vertec_fire-1", "p250_fire-1" } },
		{ "SMG", new List<string>() { "mp5_fire-1", "mp7_fire-1" } },
		{ "Rifle", new List<string>() { "ak47_fire-1", "m4_fire-1" } },
		{ "Sniper", new List<string>() { "spr_fire-1" } },
		{ "Shotgun", new List<string>() { "bekas_fire-1" } },
		{ "Silenced", new List<string>() { "vertec_fire_silenced-1", "mp7_fire_silenced-1" } },
	};

	public RadioMenu()
	{
		StyleSheet.Load( "/UI/Player/RoleMenu/Radio/RadioMenu.scss" );

		foreach ( var sound in _sounds )
		{
			Add.Button( sound.Key, () =>
			{
				Radio.PlayRadio( FetchRadio().NetworkIdent, sound.Value[Rand.Int( 0, sound.Value.Count - 1 )] );
			} );
		}
	}

	private Radio FetchRadio()
	{
		var radioComponent = Local.Pawn.Components.Get<Items.RadioComponent>();
		return radioComponent.Radio;
	}
}
