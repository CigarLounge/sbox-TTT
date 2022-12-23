using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class DNASample : Panel
{
	public DNA DNA { get; set; }

	[ConCmd.Server]
	public static void DeleteSample( int id )
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		var scanner = player.Inventory.Find<DNAScanner>();
		if ( !scanner.IsValid() )
			return;

		foreach ( var dna in scanner.DNACollected )
		{
			if ( dna.Id == id )
			{
				scanner.RemoveDNA( dna );
				return;
			}
		}
	}

	[ConCmd.Server]
	public static void SetActiveSample( int id )
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		var scanner = player.Inventory.Find<DNAScanner>();
		if ( !scanner.IsValid() )
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
}
