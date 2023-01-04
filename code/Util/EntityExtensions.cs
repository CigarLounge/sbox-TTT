using Sandbox;

namespace TTT;

public static class EntityExtensions
{
	public static void Kill( this Entity entity ) => entity.TakeDamage( DamageInfo.Generic( float.MaxValue ) );
	public static Entity AsEntity( this IEntity iEntity ) => (Entity)iEntity;
}
