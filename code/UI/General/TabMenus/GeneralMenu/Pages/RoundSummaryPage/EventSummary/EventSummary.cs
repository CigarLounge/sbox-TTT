using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using System;

namespace TTT.UI;

public partial class EventSummary : Panel
{
	private static List<EventInfo> _events = new();

	[ClientRpc]
	public static void SendData( byte[] eventBytes )
	{
		_events = eventBytes.Deserialize<List<EventInfo>>();
	}

	protected override int BuildHash() => HashCode.Combine( _events.HashCombine( e => e.GetHashCode() ) );
}
