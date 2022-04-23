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

			dnaPanel.SetClass( "selected", scanner?.SelectedSample?.NetworkIdent == dnaPanel.DNA.NetworkIdent );
		}
	}

	private DNASample AddDNASample( DNA dna )
	{
		var panel = new DNASample( dna );
		SampleContainer.AddChild( panel );
		return panel;
	}

	public class DNASample : Panel
	{
		public DNA DNA;

		public DNASample( DNA dna )
		{
			DNA = dna;

			AddClass( "rounded" );
			AddClass( "background-color-primary" );

			var deleteButton = Add.Icon( "cancel", "delete-button" );
			deleteButton.AddEventListener( "onclick", () =>
			{
				DeleteSample( dna.NetworkIdent );
			} );

			Add.Button( $"{dna.DNAType}", () =>
			{
				SetActiveSample( dna.NetworkIdent );
			} );
		}
	}

	[ServerCmd]
	public static void SetActiveSample( int ident )
	{
		Player player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		if ( player.ActiveChild is not DNAScanner scanner )
			return;

		foreach ( var dna in scanner.DNACollected )
		{
			if ( dna.NetworkIdent == ident )
			{
				Log.Info( "found one" );
				scanner.SelectedSample = dna;
				return;
			}
		}
	}

	[ServerCmd]
	public static void DeleteSample( int ident )
	{
		Player player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		if ( player.ActiveChild is not DNAScanner scanner )
			return;

		foreach ( var dna in scanner.DNACollected )
		{
			if ( dna.NetworkIdent == ident )
			{
				scanner.DNACollected.Remove( dna );
				return;
			}
		}
	}
}
