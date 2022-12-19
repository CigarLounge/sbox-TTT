using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class InspectEntry : Panel
{
	public string ActiveText { get; set; }
	public string IconText { get; set; }
	public string IconPath { get; set; }
	protected override int BuildHash() => HashCode.Combine( IconText );
	private string Icon => Texture.Load( FileSystem.Mounted, IconPath, false ) is not null ? IconPath : $"/ui/none.png";
}
