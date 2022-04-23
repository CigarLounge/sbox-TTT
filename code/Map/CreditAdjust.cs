using Sandbox;

namespace TTT;

[Library( "ttt_credit_adjust", Title = "Credit Adjust", Description = "Changes the amount of credits upon the activator." )]
public partial class CreditAdjust : Entity
{
	[Property( "Credits", "Amount of credits to remove from activator. Negative numbers add credits. Removes 1 credit by default." )]
	public int Credits { get; set; } = 100;

	[Input]
	public void ExchangeCredits( Entity activator )
	{
		if ( Game.Current.Round is not InProgressRound )
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
