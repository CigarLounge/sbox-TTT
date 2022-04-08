namespace TTT;

public partial class Player : IEntityHint
{
	public float HintDistance { get; set; } = MaxHintDistance;

	public bool CanHint( Player player )
	{
		var disguiser = Perks.Find<Disguiser>();
		return disguiser is null || (disguiser is not null && !disguiser.IsEnabled);
	}

	UI.EntityHintPanel IEntityHint.DisplayHint( Player player )
	{
		return new UI.Nameplate( this );
	}
}
