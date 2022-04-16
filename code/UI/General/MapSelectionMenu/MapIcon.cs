using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class MapIcon : Panel
{
	public string Ident { get; internal set; }
	public string VoteCount { get; set; } = "0";

	private Label Title { get; set; }
	private Label Org { get; set; }
	private Panel Container { get; set; }
	private Panel OrgAvatar { get; set; }

	public MapIcon( string fullIdent )
	{
		Ident = fullIdent;

		_ = FetchMapInformation();
	}

	private async Task FetchMapInformation()
	{
		var package = await Package.Fetch( Ident, true );
		if ( package == null )
			return;

		if ( package.PackageType != Package.Type.Map )
			return;

		Title.Text = package.Title;
		Org.Text = package.Org.Title;

		await Container.Style.SetBackgroundImageAsync( package.Thumb );
		await OrgAvatar.Style.SetBackgroundImageAsync( package.Org.Thumb );
	}
}
