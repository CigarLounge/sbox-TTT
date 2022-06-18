using Sandbox;
using SandboxEditor;

namespace TTT;

[ClassName( "ttt_credit_adjust" )]
[Description( "Changes the amount of credits upon the activator." )]
[HammerEntity]
[Title( "Credit Adjust" )]
public partial class CreditAdjust : Entity
{
	[Description( "Amount of credits to remove from activator. Negative numbers add credits" )]
	[Property]
	public int Credits { get; private set; } = 100;

	[Input]
	public void ExchangeCredits( Entity activator )
	{
		if ( Game.Current.State is not InProgress )
			return;

		if ( activator is not Player player )
			return;

		if ( player.Credits >= Credits )
		{
			player.Credits -= Credits;
			_ = OnSuccess.Fire( activator );
		}
		else
		{
			_ = OnFailure.Fire( activator );
		}
	}

	/// <summary>
	/// Fires when credits are successfully added or removed from activator.
	/// </summary>
	protected Output OnSuccess { get; set; }

	/// <summary>
	/// Fires if credits cannot be removed or added to activator. Such as not having enough credits for removal as a player cannot have 'negative' credits.
	/// </summary>
	protected Output OnFailure { get; set; }
}
