using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TTT.UI;

public partial class InfoFeed : Panel
{
	public static InfoFeed Instance { get; private set; }

	public InfoFeed() => Instance = this;

	private const int MaxMessagesDisplayed = 5;
	private const float DisplayDuration = 5f;
	private readonly Queue<InfoFeedEntry> _entryQueue = new();
	private RealTimeUntil _timeUntilNextDisplay = 0f;
	private RealTimeSince _timeSinceLastDisplayed = 0f;
	private RealTimeUntil _timeUntilNextDelete = 0f;

	public void AddToFeed( InfoFeedEntry entry )
	{
		_entryQueue.Enqueue( entry );
	}

	public override void Tick()
	{
		if ( _entryQueue.Any() && _timeUntilNextDisplay && ChildrenCount < MaxMessagesDisplayed )
		{
			var newEntry = _entryQueue.Dequeue();
			AddChild( newEntry );

			_timeUntilNextDisplay = 1f;
			_timeSinceLastDisplayed = 0f;
		}

		if ( Children.Any() && _timeUntilNextDelete && _timeSinceLastDisplayed > DisplayDuration )
		{
			var oldEntry = Children.ElementAt( 0 );
			oldEntry.Delete();
			_timeUntilNextDelete = DisplayDuration;
		}
	}

	[ClientRpc]
	public static void AddEntry( string message )
	{
		Instance.AddToFeed( new InfoFeedEntry( message ) );
	}

	[ClientRpc]
	public static void AddEntry( string message, Color color )
	{
		Instance.AddToFeed( new InfoFeedEntry( message, color ) );
	}

	[ClientRpc]
	public static void AddEntry( Player player, string message )
	{
		Instance.AddToFeed( new InfoFeedEntry( player, message ) );
	}

	[ClientRpc]
	public static void AddRoleEntry( RoleInfo roleInfo, string message )
	{
		Instance.AddToFeed( new InfoFeedEntry( roleInfo, message ) );
	}

	[ClientRpc]
	public static void AddPlayerToPlayerEntry( Player left, Player right, string message, string suffix = "" )
	{
		Instance.AddToFeed( new InfoFeedEntry( left, right, message, suffix ) );
	}

	[GameEvent.Player.CorpseFound]
	private void OnCorpseFound( Player player )
	{
		AddPlayerToPlayerEntry
		(
			player.Corpse.Finder,
			player,
			"found the body of",
			$"({player.Role.Title})"
		);
	}

	[GameEvent.State.Start]
	private void OnStateStart( BaseState state )
	{
		if ( state is not PreRound round )
			return;

		AddEntry( $"A new round begins in {round.Duration} seconds. Prepare yourself." );

		if ( !Karma.Enabled || Game.LocalPawn is not Player player )
			return;

		var karma = MathF.Round( player.BaseKarma );
		var df = MathF.Round( 100f - player.DamageFactor * 100f );
		var damageFactor = df == 0 ? $"Your karma is {karma}, you'll deal full damage this round." : $"Your karma is {karma}, you'll deal {df}% reduced damage this round.";
		AddEntry( damageFactor );
	}

	[GameEvent.Round.Start]
	private void OnRoundStart()
	{
		if ( Game.IsServer )
			return;

		AddEntry( "Roles have been assigned and the round has begun!" );
		AddEntry( $"Traitors will receive an additional {GameManager.InProgressSecondsPerDeath} seconds per death." );
	}

	[GameEvent.Round.End]
	private void OnRoundEnd( Team _, WinType _1 )
	{
		this.Enabled( false );

		DeleteChildren();
	}
}
