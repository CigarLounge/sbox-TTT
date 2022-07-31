namespace TTT;

public static class PlayerExtensions
{
	public static bool KilledWithHeadShot( this Player player )
	{
		return (HitboxGroup)player.GetHitboxGroup( player.LastDamage.HitboxIndex ) == HitboxGroup.Head;
	}
}
