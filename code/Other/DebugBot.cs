using Sandbox;
using Sandbox.Internal;
using System.Linq;

namespace TTT;

#if DEBUG
public partial class DebugBot : Bot
{
	private Player _pawn;
	private Player _spawner;

	private static bool _mimic;
	private static bool _wander;

	[ConVar.Replicated( "bot_debug" )]
	private static bool DrawDebug { get; set; }

	[ConCmd.Admin( "bot_mimic", Help = "Make bots mimic your inputs." )]
	private static void ToggleMimicHost()
	{
		_mimic = !_mimic;
		_wander = false;
	}

	[ConCmd.Admin( "bot_wander", Help = "Make bots randomly press move buttons." )]
	private static void ToggleWander()
	{
		_wander = !_wander;
		_mimic = false;
	}

	[ConCmd.Admin( "bot_reset", Help = "Resets bot to default settings." )]
	private static void BotReset()
	{
		_wander = false;
		_mimic = false;
	}

	[ConCmd.Admin( "bot_kill", Help = "Kills a bot by name if provided, else kills all bots." )]
	private static void BotKill( string name = "" )
	{
		if ( string.IsNullOrEmpty( name ) )
		{
			foreach ( var bot in Bot.All.ToArray() )
			{
				if ( bot.Client.Pawn is Entity ent )
					ent.OnKilled();
			}

			return;
		}

		var nameLower = name.ToLower();
		foreach ( var bot in Bot.All.ToArray() )
		{
			if ( bot.Client.Name.ToLower() == nameLower && bot.Client.Pawn is Entity ent )
				ent.OnKilled();
		}
	}

	[ConCmd.Admin( "bot_kick", Help = "Kicks all bots." )]
	private static void BotKick()
	{
		foreach ( var bot in Bot.All.ToArray() )
			bot.Client.Kick();
	}

	[ConCmd.Admin( "bot_add" )]
	private static void AddBot()
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player )
			return;

		var bot = new DebugBot();
		bot._spawner = player;
		bot._pawn = bot.Client.Pawn as Player;
	}

	public override void BuildInput()
	{
		_pawn.InputDirection = Vector3.Zero;

		if ( DrawDebug )
		{
			DebugOverlay.Axis( _pawn.EyePosition, _pawn.EyeRotation, 24 );
			DebugOverlay.Axis( _pawn.Position, _pawn.Rotation, 32 );
		}

		if ( _wander )
			_pawn.InputDirection = Vector3.Random * 2f;

		if ( _mimic )
		{
			Input.CopyLastInput( _spawner.Client );
			foreach ( var item in from p in GlobalGameNamespace.TypeLibrary.GetPropertyDescriptions( Client.Pawn )
								  where p.HasAttribute<ClientInputAttribute>()
								  select p )
				item.SetValue( Client.Pawn, item.GetValue( _spawner ) );
		}
	}
}
#endif
