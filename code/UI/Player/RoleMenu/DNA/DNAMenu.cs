using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Linq;

namespace TTT.UI;

public partial class DNAMenu : Panel
{
	private readonly Dictionary<DNA, Sample> _entries = new();

	private readonly Panel _sampleContainer;
	private Panel _emptyPanel;
	private readonly Panel _infoContainer;
	private readonly Button _button;
	private readonly Checkbox _checkbox;

	public DNAMenu()
	{
		StyleSheet.Load( "/ui/player/rolemenu/dna/DNAMenu.scss" );

		var wrapper = Add.Panel();

		_sampleContainer = wrapper.Add.Panel( "sample-container" );
		_infoContainer = wrapper.Add.Panel( "info-container" );

		_infoContainer.AddClass( "rounded" );
		_infoContainer.AddClass( "background-color-primary" );
		var currentCharge = _infoContainer.Add.Label( "95.45%", "charge" );
		var status = _infoContainer.Add.Label( "CHARGING", "charge-status" );
		_button = _infoContainer.Add.ButtonWithIcon( "Scan", "radar", "scan-button" );

		_checkbox = new Checkbox();
		_checkbox.LabelText = "Auto-repeat";
		_infoContainer.AddChild( _checkbox );
	}

	public override void Tick()
	{
		_button.SetClass( "inactive", _checkbox.Checked );

		if ( Local.Pawn is not Player player )
			return;

		if ( player.ActiveChild is not DNAScanner scanner )
			return;

		foreach ( var sample in scanner.DNACollected.Except( _entries.Keys ) )
		{
			var panel = AddSample( sample );
			_entries[sample] = panel;
		}

		foreach ( var dna in _entries.Keys.Except( scanner.DNACollected ) )
		{
			if ( _entries.TryGetValue( dna, out var panel ) )
			{
				panel?.Delete();
				_entries.Remove( dna );
			}
		}
	}

	private Sample AddSample( DNA dna )
	{
		var panel = new Sample( dna );
		_sampleContainer.AddChild( panel );
		return panel;
	}

	public class Sample : Panel
	{
		public Sample( DNA dna )
		{
			AddClass( "rounded" );
			AddClass( "background-color-primary" );

			var deleteButton = Add.Icon( "cancel", "delete-button" );
			deleteButton.AddEventListener( "onclick", () =>
			{
				DeleteSample( dna.NetworkIdent );
			} );

			Add.Button( $"{true}" );
		}
	}

	[ServerCmd()]
	public static void DeleteSample( int indent )
	{
		Player player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		if ( player.ActiveChild is not DNAScanner scanner )
			return;

		foreach ( var dna in scanner.DNACollected )
		{
			if ( dna.NetworkIdent == indent )
			{
				scanner.DNACollected.Remove( dna );
				return;
			}
		}
	}
}
