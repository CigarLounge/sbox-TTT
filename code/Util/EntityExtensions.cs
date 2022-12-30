using Sandbox;

namespace TTT;

public static class EntityExtensions
{
	public static bool IsAlive( this Entity entity ) => entity.LifeState == LifeState.Alive;
	public static void Kill( this Entity entity ) => entity.TakeDamage( ExtendedDamageInfo.Generic( float.MaxValue ) );
	public static void TakeDamage( this Entity entity, ExtendedDamageInfo info )
	{
		if ( entity is Player player )
			player.TakeDamage( info );
		else
			entity.TakeDamage( info.ToDamageInfo() );
	}
	public static Entity AsEntity( this IEntity iEntity ) => (Entity)iEntity;
}
