using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class MapVotePanel : Panel
{
	private Label TimeText { get; set; }
	private Panel Body { get; set; }
	private readonly List<MapIcon> MapIcons = new();

	public MapVotePanel()
	{
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
		var icon = MapIcons.FirstOrDefault( x => x.Ident == fullIdent );

		if ( icon is not null )
			return icon;

		icon = new MapIcon( fullIdent );
		icon.AddEventListener( "onclick", () => MapSelectionState.SetVote( fullIdent ) );
		Body.AddChild( icon );

		MapIcons.Add( icon );
		return icon;
	}

	public override void Tick()
	{
		var mapSelectionState = Game.Current.State as MapSelectionState;

		TimeText.Text = mapSelectionState.TimeLeftFormatted;

		foreach ( var icon in MapIcons )
			icon.VoteCount = "0";

		foreach ( var group in mapSelectionState.Votes.GroupBy( x => x.Value ).OrderByDescending( x => x.Count() ) )
		{
			var icon = AddMap( group.Key );
			icon.VoteCount = group.Count().ToString( "n0" );
		}
	}
}
