using Sandbox;
using Sandbox.Diagnostics;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace TTT.UI;

public partial class InspectMenu : Panel
{
	private Panel IconsContainer { get; set; }
	private readonly Corpse _corpse;
	private InspectEntry _selectedInspectEntry;
	private readonly List<InspectEntry> _inspectionEntries = new();

	public InspectMenu( Corpse corpse )
	{
		Assert.NotNull( corpse );
		_corpse = corpse;
	}

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
			return;

		foreach ( var entry in IconsContainer.ChildrenOfType<InspectEntry>() )
		{
			entry.AddEventListener( "onmouseover", () => { _selectedInspectEntry = entry; } );
			entry.AddEventListener( "onmouseout", () => { _selectedInspectEntry = null; } );
		}
	}

	private (string iconText, string activeText) GetCauseOfDeathStrings()
	{
		var causeOfDeath = ("Unknown", "The cause of death is unknown.");
		foreach ( var tag in _corpse.Player.LastDamage.Tags )
		{
			return tag switch
			{
				DamageTags.Bullet => ("Bullet", "This corpse was shot to death."),
				DamageTags.Slash => ("Slash", "This corpse was cut to death."),
				DamageTags.Burn => ("Burn", "This corpse has burn marks all over."),
				DamageTags.Vehicle => ("Vehicle", "This corpse was hit by a vehicle."),
				DamageTags.Fall => ("Fall", "This corpse fell from a high height."),
				DamageTags.Explode => ("Explode", "An explosion eviscerated this corpse."),
				DamageTags.Drown => ("Drown", "This player drowned to death."),
				_ => ("Unknown", "The cause of death is unknown.")
			};
		}
		return causeOfDeath;
	}

	protected override int BuildHash()
	{
		return HashCode.Combine(
			_corpse.HasCalledDetective,
			_corpse.TimeUntilDNADecay,
			_corpse.Player.TimeSinceDeath.Relative.TimerString(),
			_corpse.TimeUntilDNADecay.Relative.TimerString(),
			(Game.LocalPawn as Player)?.IsAlive,
			_selectedInspectEntry?.ActiveText.ToString()
		);
	}

	// Called from UI panel
	public void CallDetective()
	{
		if ( _corpse.HasCalledDetective )
			return;

		CallDetectives( _corpse.NetworkIdent );
		_corpse.HasCalledDetective = true;
	}

	[ConCmd.Server]
	private static void CallDetectives( int ident )
	{
		var enemy = Entity.FindByIndex( ident );
		if ( !enemy.IsValid() || enemy is not Corpse corpse )
			return;

		TextChat.AddInfo( To.Everyone, $"{ConsoleSystem.Caller.Name} called a Detective to the body of {corpse.Player.SteamName}." );
		SendDetectiveMarker( To.Multiple( Utils.GetClientsWhere( p => p.IsAlive && p.Role is Detective ) ), corpse.Position );
	}

	[ClientRpc]
	public static void SendDetectiveMarker( Vector3 corpseLocation )
	{
		TimeSince timeSinceCreated = 0;
		WorldPoints.Instance.AddChild(
			new WorldMarker
			(
				"/ui/d-call-icon.png",
				() => $"{(Game.LocalPawn as Player).Position.Distance( corpseLocation ).SourceUnitsToMeters():n0}m",
				() => corpseLocation,
				() => timeSinceCreated > 30
			)
		);
	}
}
