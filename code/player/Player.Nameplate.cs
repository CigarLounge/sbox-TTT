namespace TTT;

public partial class Player : IEntityHint
{
	public float HintDistance { get; set; } = MAX_HINT_DISTANCE;

	public bool CanHint( Player player )
	{
		var disguiser = Perks.Find<Disguiser>();
		return disguiser is null || (disguiser is not null && !disguiser.IsEnabled);
	}

	public UI.EntityHintPanel DisplayHint( Player player )
	{
		return new UI.Nameplate( this );
	}
}
