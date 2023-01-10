using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class DNAMenu : Panel
{
	private DNAScanner _dnaScanner;

	private bool AutoScan { get; set; } = false;

	public override void Tick()
	{
		if ( !IsVisible || Game.LocalPawn is not Player player )
			return;

		_dnaScanner ??= player.Inventory.Find<DNAScanner>();
		if ( !_dnaScanner.IsValid() )
			return;

		if ( _dnaScanner.AutoScan != AutoScan )
			SetAutoScan( AutoScan );
	}

	[ConCmd.Server]
	public static void SetAutoScan( bool enabled )
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		var scanner = player.Inventory.Find<DNAScanner>();
		if ( !scanner.IsValid() )
			return;

		scanner.AutoScan = enabled;
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( _dnaScanner?.IsCharging, _dnaScanner?.SlotText, _dnaScanner?.DNACollected?.HashCombine( d => d.Id ), _dnaScanner.SelectedId );
	}
}
