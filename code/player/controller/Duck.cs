using Sandbox;

namespace TTT;

public partial class Duck : Sandbox.Duck
{
	public Duck( BasePlayerController controller ) : base( controller )
	{
		Controller = controller;
	}

	public override float GetWishSpeed()
	{
		if ( !IsActive )
		{
			return -1;
		}

		return 96.0f;
	}
}
