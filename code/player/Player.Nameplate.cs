namespace TTT;

public partial class Player : IEntityHint
{
	public float HintDistance => 20480f;
	public bool ShowGlow => false;
	public string TextOnTick => "";

	public bool CanHint( Player player )
	{
		var disguiser = Perks.Find<Disguiser>();
		return disguiser == null || (disguiser != null && !disguiser.IsEnabled);
	}

	public UI.EntityHintPanel DisplayHint( Player player )
	{
		return new UI.Nameplate( this );
	}

	public void Tick( Player player )
	{

	}
}
