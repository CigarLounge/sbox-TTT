using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Linq;

namespace TTT.UI;

[UseTemplate]
public partial class DNAMenu : Panel
{
	private readonly Dictionary<DNA, DNASample> _entries = new();

	private Panel SampleContainer { get; init; }
	private Panel Empty { get; init; }
	private Label Charge { get; init; }
	private Button ScanButton { get; init; }
	private Checkbox AutoRepeat { get; init; }

	public override void Tick()
	{
		ScanButton.SetClass( "inactive", AutoRepeat.Checked );

		if ( Local.Pawn is not Player player )
			return;

		if ( player.ActiveChild is not DNAScanner scanner )
			return;

		Charge.Text = scanner.SlotText;

		foreach ( var dna in scanner.DNACollected )
		{
			if ( !_entries.ContainsKey( dna ) )
				_entries[dna] = AddDNASample( dna );
		}

		foreach ( var dnaPanel in _entries.Values )
		{
			if ( !scanner.DNACollected.Contains( dnaPanel.DNA ) )
			{
				_entries.Remove( dnaPanel.DNA );
				dnaPanel?.Delete();
			}

			dnaPanel.SetClass( "selected", scanner?.SelectedId == dnaPanel.DNA.Id );
		}

		Empty.Enabled( !_entries.Any() );
	}

	private DNASample AddDNASample( DNA dna )
	{
		var panel = new DNASample( dna );
		SampleContainer.AddChild( panel );
		return panel;
	}

	public class DNASample : Panel
	{
		public DNA DNA { get; private set; }

		public DNASample( DNA dna )
		{
			DNA = dna;

			var deleteButton = Add.Icon( "cancel", "delete-button" );
			deleteButton.AddEventListener( "onclick", () => { DeleteSample( dna.Id ); } );

			Add.Button( $"#{dna.Id} - {dna.Source} - {dna.TimeCollected.TimerString()}", () => { SetActiveSample( dna.Id ); } );
		}
	}

	[ServerCmd]
	public static void SetActiveSample( int id )
	{
		Player player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		if ( player.ActiveChild is not DNAScanner scanner )
			return;

		foreach ( var dna in scanner.DNACollected )
		{
			if ( dna.Id == id )
			{
				scanner.SelectedId = id;
				return;
			}
		}
	}

	[ServerCmd]
	public static void DeleteSample( int id )
	{
		Player player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		if ( player.ActiveChild is not DNAScanner scanner )
			return;

		foreach ( var dna in scanner.DNACollected )
		{
			if ( dna.Id == id )
			{
				scanner.DNACollected.Remove( dna );
				return;
			}
		}
	}
}
