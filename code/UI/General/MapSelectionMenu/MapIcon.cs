using System;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class MapIcon : Panel
{
	public string Ident { get; set; }
	public int Votes { get; set; }

	private Package _data;

	protected void VoteMap() => MapSelectionState.SetVote( Ident );

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

	protected override int BuildHash() => HashCode.Combine( Votes );
}
