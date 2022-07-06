using Sandbox;

namespace TTT;

public static class EntityExtensions
{
	public static bool IsAlive( this Entity entity ) => entity.LifeState == LifeState.Alive;

	public static void Kill( this Entity entity ) => entity.TakeDamage( DamageInfo.Generic( float.MaxValue ) );
}
