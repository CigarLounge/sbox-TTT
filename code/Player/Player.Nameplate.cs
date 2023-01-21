using System.Collections.Generic;
using Sandbox.UI;

namespace TTT;

public partial class Player : IEntityHint
{
	private static readonly List<ColorGroup> _healthGroupList = new()
	{
		new ColorGroup( "Healthy", Color.FromBytes( 44, 233, 44 ) ),
		new ColorGroup( "Hurt", Color.FromBytes( 171, 231, 3 ) ),
		new ColorGroup( "Wounded", Color.FromBytes( 213, 202, 4 ) ),
		new ColorGroup( "Badly Wounded", Color.FromBytes( 234, 129, 4 ) ),
		new ColorGroup( "Near Death", Color.FromBytes( 246, 6, 6 ) )
	};

	public static ColorGroup GetHealthGroup( Player player )
	{
		if ( player.Health > MaxHealth * 0.9 )
			return _healthGroupList[0];
		else if ( player.Health > MaxHealth * 0.7 )
			return _healthGroupList[1];
		else if ( player.Health > MaxHealth * 0.45 )
			return _healthGroupList[2];
		else if ( player.Health > MaxHealth * 0.2 )
			return _healthGroupList[3];
		else
			return _healthGroupList[4];
	}

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
