using System;
using System.Linq;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class MapSelectionMenu : Panel
{
	private Panel Maps { get; set; }

	public MapSelectionMenu()
	{
		// Delete unneeded UI elements.
		foreach ( var panel in Game.RootPanel.Children.ToList() )
		{
			if ( panel is FullScreenHintMenu )
				continue;

			if ( panel is not TextChat and not VoiceChatDisplay )
				panel.Delete( true );
		}
	}

	public override void Tick()
	{
		if ( GameManager.Current.State is not MapSelectionState mapSelectionState )
			return;

		// We are looping quite a lot in this code. Maybe we can use razor to make this less painful?
		var maps = Maps.ChildrenOfType<MapIcon>();
		foreach ( var icon in maps )
			icon.Votes = 0;

		foreach ( var group in mapSelectionState.Votes.GroupBy( x => x.Value ).OrderByDescending( x => x.Count() ) )
		{
			foreach ( var map in maps )
			{
				if ( group.Key == map.Ident )
					map.Votes = group.Count();
			}
		}
	}

	protected override int BuildHash() => HashCode.Combine( GameManager.Current.State.TimeLeftFormatted );
}
