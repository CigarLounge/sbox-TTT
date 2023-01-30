#if DEBUG
using Sandbox;
using Sandbox.Internal;
using System.Linq;

namespace TTT;

public partial class DebugBot : Bot
{
	/// <summary>
	/// The Player pawn this bot controls.
	/// </summary>
	public Player Pawn;

	/// <summary>
	/// The host Player who spawned this bot.
	/// </summary>
	public Player Spawner;

	public static bool Mimic;
	public static bool Wander;

	[ConVar.Replicated( "bot_debug" )]
	public static bool DrawDebug { get; set; }

	[ConCmd.Admin( "bot_mimic", Help = "Make bots mimic your inputs." )]
	public static void ToggleMimicHost()
	{
		Mimic = !Mimic;
		Wander = false;
	}

	[ConCmd.Admin( "bot_wander", Help = "Make bots randomly press move buttons." )]
	public static void ToggleWander()
	{
		Wander = !Wander;
		Mimic = false;
	}

	[ConCmd.Admin( "bot_reset", Help = "Resets bot to default settings." )]
	public static void BotReset()
	{
		Wander = false;
		Mimic = false;
	}

	[ConCmd.Admin( "bot_kill", Help = "Kills a bot by name if provided, else kills all bots." )]
	public static void BotKill( string name = "" )
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
	public static void BotKick()
	{
		foreach ( var bot in Bot.All.ToArray() )
			bot.Client.Kick();
	}

	[ConCmd.Admin( "bot_add" )]
	public static void AddBot()
	{
		var pawn = ConsoleSystem.Caller.Pawn;
		if ( pawn is not Player player )
			return;

		var bot = new DebugBot();
		bot.Spawner = player;
		bot.Pawn = bot.Client.Pawn as Player;
	}

	public override void BuildInput()
	{
		Pawn.InputDirection = Vector3.Zero;

		if ( DrawDebug )
		{
			DebugOverlay.Axis( Pawn.EyePosition, Pawn.EyeRotation, 24 );
			DebugOverlay.Axis( Pawn.Position, Pawn.Rotation, 32 );
		}

		if ( Wander )
			Pawn.InputDirection = Vector3.Random * 2f;

		if ( Mimic )
		{
			Input.CopyLastInput( Spawner.Client );
			foreach ( var item in from p in GlobalGameNamespace.TypeLibrary.GetPropertyDescriptions( Client.Pawn )
								  where p.HasAttribute<ClientInputAttribute>()
								  select p )
				item.SetValue( Client.Pawn, item.GetValue( Spawner ) );
		}
	}
}
#endif
