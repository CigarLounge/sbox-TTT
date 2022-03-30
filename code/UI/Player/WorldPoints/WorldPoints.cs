using Sandbox.UI;

namespace TTT.UI;

public class WorldPoints : Panel
{
	public static WorldPoints Instance { get; set; }

	public WorldPoints()
	{
		Instance = this;
		AddClass( "fullscreen" );
		Style.ZIndex = -1;
	}

	public void DeletePoints<T>()
	{
		foreach ( var child in Children )
		{
			if ( child is T )
				child.Delete();
		}
	}
}
