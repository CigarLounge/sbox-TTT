using Sandbox.UI;

namespace TTT;

public partial class Player : IEntityHint
{
	public float HintDistance => MaxHintDistance;
	public bool ShowGlow => false;

	public bool CanHint( Player player )
	{
		var disguiser = Perks.Find<Disguiser>();

		return !disguiser?.IsActive ?? true;
	}

	Panel IEntityHint.DisplayHint( Player player )
	{
		return new UI.Nameplate( this );
	}
}
