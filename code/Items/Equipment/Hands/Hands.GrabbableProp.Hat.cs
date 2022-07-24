using Sandbox;

namespace TTT;

public class GrabbableHat : GrabbableProp
{
	private readonly Player _owner;
	public DetectiveHat _hat;

	public GrabbableHat( Player owner, Entity grabPoint, DetectiveHat hat ) : base( owner, grabPoint, hat )
	{
		_owner = owner;
		_hat = hat;
	}

	public override void SecondaryAction()
	{
		_hat?.PutOn( _owner );
		_hat?.Delete();
	}
}
