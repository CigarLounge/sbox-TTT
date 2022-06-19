using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class MapVotePanel : Panel
{
	private Label TimeText { get; init; }
	private Panel Body { get; init; }

	private readonly List<MapIcon> _mapIcons = new();

	public MapVotePanel()
	{
		// Delete unneeded UI elements.
		foreach ( var panel in Local.Hud.Children.ToList() )
		{
			if ( panel is FullScreenHintMenu )
				continue;

			if ( panel is not ChatBox and not VoiceChatDisplay )
				panel.Delete( true );
		}

		_ = PopulateMaps();
	}

	public async Task PopulateMaps()
	{
		var query = new Package.Query
		{
			Type = Package.Type.Map,
			Order = Package.Order.User,
			Take = 99,
		};

		query.Tags.Add( "game:" + Global.GameIdent );

		var packages = await query.RunAsync( default );

		foreach ( var package in packages )
		{
			AddMap( package.FullIdent );
		}
	}

	private MapIcon AddMap( string fullIdent )
	{
		var icon = _mapIcons.FirstOrDefault( x => x.Ident == fullIdent );

		if ( icon is not null )
			return icon;

		icon = new MapIcon( fullIdent );
		icon.AddEventListener( "onclick", () => MapSelectionState.SetVote( fullIdent ) );
		Body.AddChild( icon );

		_mapIcons.Add( icon );
		return icon;
	}

	public override void Tick()
	{
		var mapSelectionState = Game.Current.State as MapSelectionState;

		TimeText.Text = mapSelectionState.TimeLeftFormatted;

		foreach ( var icon in _mapIcons )
			icon.VoteCount = "0";

		foreach ( var group in mapSelectionState.Votes.GroupBy( x => x.Value ).OrderByDescending( x => x.Count() ) )
		{
			var icon = AddMap( group.Key );
			icon.VoteCount = group.Count().ToString( "n0" );
		}
	}
}
