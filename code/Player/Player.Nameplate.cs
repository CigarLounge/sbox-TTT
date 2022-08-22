namespace TTT;

public partial class Player : IEntityHint
{
	public float HintDistance => MaxHintDistance;

	public bool CanHint( Player player )
	{
		var disguiser = Perks.Find<Disguiser>();
		return !disguiser?.IsEnabled ?? true;
	}

	UI.EntityHintPanel IEntityHint.DisplayHint( Player player )
	{
		return new UI.Nameplate( this, player.TaggedPlayers );
	}
}
