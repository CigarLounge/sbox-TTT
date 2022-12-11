using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TTT;

public class TTTCamera : BaseCamera
{
	private static Player _target;
	public static Player Target
	{
		get => _target;
		set
		{
			if ( _target == value )
				return;

			var oldTarget = _target;
			_target = value;

			Event.Run( GameEvent.Player.SpectatorChanged, oldTarget, _target );
		}
	}

	public static bool IsSpectator => Target.IsValid() && !Target.IsLocalPawn;
	public static bool IsLocal => !IsSpectator;

	public virtual IEnumerable<Player> GetPlayers()
	{
		return Entity.All.OfType<Player>();
	}

	public override void BuildInput()
	{
	}

	public override void Update()
	{
		if ( Game.LocalPawn is Player player )
			Target = player;

		if ( !Target.IsValid() )
			Target = GetPlayers().FirstOrDefault();

		var target = Target;
		if ( !target.IsValid() )
			return;

		Camera.Position = target.EyePosition;

		if ( IsLocal )
			Camera.Rotation = target.EyeRotation;
		else
			Camera.Rotation = Rotation.Slerp( Camera.Rotation, target.EyeRotation, Time.Delta * 20f );

		Camera.FirstPersonViewer = target;
	}
}
