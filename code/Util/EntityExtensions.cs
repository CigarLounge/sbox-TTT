using Sandbox;

namespace TTT;

public static class EntityExtensions
{
	public static bool IsAlive( this Entity entity ) => entity.LifeState == LifeState.Alive;
	public static void Kill( this Entity entity ) => entity.TakeDamage( ExtendedDamageInfo.Generic( float.MaxValue ) );
	public static Entity AsEntity( this IEntity iEntity ) => (Entity)iEntity;
}
