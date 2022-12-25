using Editor;
using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT;

[ClassName( "ttt_grenade_random" )]
[EditorModel( "models/weapons/w_frag.vmdl" )]
[HammerEntity]
[Title( "Random Grenade" )]
public class RandomGrenade : Entity
{
	private static readonly List<Type> _cachedGrenadeTypes = new();
	private const int GrenadeOffset = 4;

	public override void Spawn()
	{
		Transmit = TransmitType.Never;
		CacheGrenadeTypes();

		if ( _cachedGrenadeTypes.IsNullOrEmpty() )
			return;

		var grenade = TypeLibrary.Create<Grenade>( Game.Random.FromList( _cachedGrenadeTypes ) );
		if ( grenade is null )
			return;

		grenade.Position = Position + (Vector3.Up * GrenadeOffset);
		grenade.Rotation = Rotation;
	}

	private static void CacheGrenadeTypes()
	{
		if ( !_cachedGrenadeTypes.IsNullOrEmpty() )
			return;

		var grenades = TypeLibrary.GetTypes<Grenade>();
		foreach ( var grenadeDesc in grenades )
		{
			var grenadeInfo = GameResource.GetInfo<CarriableInfo>( grenadeDesc.TargetType );
			if ( grenadeInfo is not null && grenadeInfo.Spawnable )
				_cachedGrenadeTypes.Add( grenadeDesc.TargetType );
		}
	}
}
