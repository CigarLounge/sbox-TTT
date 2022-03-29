using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Linq;

namespace TTT.UI;

[UseTemplate]
public partial class DNAMenu : Panel
{
	private readonly Dictionary<DNA, Sample> _entries = new();

	private Panel SampleContainer { get; set; }
	private Label Charge { get; set; }
	private Label ChargeStatus { get; set; }
	private Button ScanButton { get; set; }
	private Checkbox AutoRepeat { get; set; }

	public override void Tick()
	{
		ScanButton.SetClass( "inactive", AutoRepeat.Checked );

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
		SampleContainer.AddChild( panel );
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
