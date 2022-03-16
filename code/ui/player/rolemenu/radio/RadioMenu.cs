using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace TTT.UI;

public partial class RadioMenu : Panel
{
	private readonly Dictionary<string, List<string>> _sounds = new()
	{
		{ "Pistol", new List<string>() { "idk", "wtf" } },
		{ "Rifle", new List<string>() { "idk", "wtf" } },
		{ "Sniper", new List<string>() { "idk", "wtf" } },
		{ "Magnum", new List<string>() { "idk", "wtf" } },
		{ "Testing", new List<string>() { "idk", "wtf" } },
		{ "Not Sure", new List<string>() { "idk", "wtf" } },
		{ "Not Sure 2", new List<string>() { "idk", "wtf" } },
	};

	public RadioMenu()
	{
		StyleSheet.Load( "/ui/player/rolemenu/radio/RadioMenu.scss" );

		foreach ( var sound in _sounds )
		{
			Add.Button( sound.Key, () =>
			{

			} );
		}
	}
}
