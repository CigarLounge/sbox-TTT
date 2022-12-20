using System;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class MapIcon : Panel
{
	public string Ident { get; private set; }
	public string VoteCount { get; set; } = "0";

	private Package _data;

	public MapIcon( string fullIdent ) => Ident = fullIdent;

	protected override async Task OnParametersSetAsync()
	{
		var package = await Package.Fetch( Ident, true );
		if ( package is null || package.PackageType != Package.Type.Map )
		{
			Delete();
			return;
		}

		_data = package;
		StateHasChanged();
	}

	protected override int BuildHash() => HashCode.Combine( VoteCount );
}
