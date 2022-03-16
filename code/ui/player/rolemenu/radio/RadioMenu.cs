using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TTT.UI;

public partial class RadioMenu : Panel
{
	// TODO: Let's hook up some of the other sounds like death, fire, explore, etc.
	private readonly Dictionary<string, List<string>> _sounds = new()
	{
		{ "Pistol", new List<string>() { "vertec_fire-1", "p250_fire-1" } },
		{ "SMG", new List<string>() { "mp5_fire-1", "mp7_fire-1" } },
		{ "Rifle", new List<string>() { "ak47_fire-1", "m4_fire-1" } },
		{ "Sniper", new List<string>() { "spr_fire-1" } },
		{ "Shotgun", new List<string>() { "bekas_fire-1" } },
		{ "Silenced", new List<string>() { "vertec_fire_silenced-1", "mp7_fire_silenced-1" } },
	};

	private RadioEntity _cachedRadio;
	private bool _isPlayingSound = false;

	public RadioMenu()
	{
		StyleSheet.Load( "/ui/player/rolemenu/radio/RadioMenu.scss" );

		FetchRadio();

		foreach ( var sound in _sounds )
		{
			Add.Button( sound.Key, () =>
			{
				if ( !_isPlayingSound )
					_ = PlayRadioSound( sound.Value );
			} );
		}
	}

	private async Task PlayRadioSound( List<string> sounds )
	{
		if ( _cachedRadio == null )
		{
			FetchRadio();
			return;
		}

		_isPlayingSound = true;

		var timesToPlay = Rand.Int( 1, 5 );
		var soundToPlay = sounds[Rand.Int( 0, sounds.Count - 1 )];

		for ( int i = 0; i < timesToPlay; ++i )
		{
			_cachedRadio?.PlaySound( soundToPlay );
			await Task.Delay( 750 * Rand.Int( 1, 3 ) );
		}

		_isPlayingSound = false;
	}

	private void FetchRadio()
	{
		if ( Local.Pawn is Player player )
		{
			var radioComponent = player.Components.Get<RadioComponent>();
			if ( radioComponent != null )
				_cachedRadio = radioComponent.Radio;
		}
	}
}
