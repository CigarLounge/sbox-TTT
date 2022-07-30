using Sandbox;

namespace TTT;

public class GrabbableHat : GrabbableProp
{
	public override string PrimaryAttackHint => _hat.IsValid() ? "Put on" : string.Empty;

	private readonly Player _owner;
	private readonly DetectiveHat _hat;

	public GrabbableHat( Player owner, Entity grabPoint, DetectiveHat hat ) : base( owner, grabPoint, hat )
	{
		_owner = owner;
		_hat = hat;
	}

	public override void SecondaryAction()
	{
		if ( Host.IsServer )
		{
			_hat?.PutOn( _owner );
			_hat?.Delete();
		}
	}
}
