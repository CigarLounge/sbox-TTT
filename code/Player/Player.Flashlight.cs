using Sandbox;

namespace TTT;

public partial class Player
{
	[Net, Predicted]
	public bool FlashlightEnabled { get; private set; } = false;

	[Net, Local, Predicted]
	private TimeSince TimeSinceLightToggled { get; set; }

	/// <summary>
	/// The third person flashlight.
	/// </summary>
	private SpotLightEntity _worldLight;

	/// <summary>
	/// The first person flashlight.
	/// </summary>
	private SpotLightEntity _viewLight;

	public void SimulateFlashlight()
	{
		var toggle = Input.Pressed( InputButton.Flashlight );

		if ( _worldLight.IsValid() )
		{
			var transform = GetAttachment( "eyes" ) ?? default;
			_worldLight.Transform = transform;

			if ( ActiveCarriable.IsValid() )
				_worldLight.Transform = ActiveCarriable.GetAttachment( "muzzle" ) ?? transform;
		}

		if ( TimeSinceLightToggled > 0.25f && toggle )
		{
			FlashlightEnabled = !FlashlightEnabled;

			PlaySound( "flashlight-toggle" );

			if ( _worldLight.IsValid() )
				_worldLight.Enabled = FlashlightEnabled;

			TimeSinceLightToggled = 0;
		}
	}

	protected void CreateFlashlight()
	{
		if ( Game.IsServer )
		{
			_worldLight = CreateLight();
			_worldLight.EnableHideInFirstPerson = true;
			FlashlightEnabled = false;
		}
		else
		{
			_viewLight = CreateLight();
			_viewLight.EnableViewmodelRendering = true;
			_viewLight.Enabled = FlashlightEnabled;
		}
	}

	protected void DeleteFlashlight()
	{
		_worldLight?.Delete();
		_worldLight = null;
		_viewLight?.Delete();
		_viewLight = null;
	}

	[Event.Client.Frame]
	private void FrameUpdate()
	{
		if ( !_viewLight.IsValid() )
			return;

		_viewLight.Enabled = FlashlightEnabled && IsLocalPawn;

		if ( !_viewLight.Enabled )
			return;

		var eyeTransform = new Transform( EyePosition, EyeRotation );
		_viewLight.Transform = eyeTransform;

		if ( !ActiveCarriable.IsValid() )
			return;

		var muzzleTransform = ActiveCarriable.ViewModelEntity?.GetAttachment( "muzzle" );

		if ( !muzzleTransform.HasValue )
			return;

		var mz = muzzleTransform.Value;

		// First check if there is something like a door/wall between the muzzle and our eyes.
		var muzzleTrace = Trace.Ray( mz.Position, EyePosition )
			.Size( 2 )
			.Ignore( this )
			.Ignore( ActiveCarriable )
			.Run();

		var downOffset = Vector3.Down * 2f;
		var origin = mz.Position + downOffset;

		// If there IS something between our eyes and the muzzle, add the distance.
		if ( muzzleTrace.Hit )
			origin = muzzleTrace.EndPosition + (mz.Rotation.Backward * muzzleTrace.Distance) + downOffset;

		// Continue with the forward trace.
		var destination = origin + mz.Rotation.Forward * _viewLight.Range;
		var direction = destination - origin;

		var fwdTrace = Trace.Box( Vector3.One * 2, origin, destination )
			.Ignore( this )
			.Ignore( ActiveCarriable )
			.Run();

		var pullbackAmount = 0.0f;
		const float pullbackThreshold = 48f;
		if ( fwdTrace.Distance < pullbackThreshold )
			pullbackAmount = (pullbackThreshold - fwdTrace.Distance).Remap( 0, pullbackThreshold, 0.0f, 0.045f );

		origin -= direction * pullbackAmount;

		_viewLight.Position = origin;
		_viewLight.Rotation = mz.Rotation;
	}

	private SpotLightEntity CreateLight()
	{
		return new SpotLightEntity
		{
			Enabled = false,
			DynamicShadows = true,
			Range = 1024,
			Falloff = 1.0f,
			LinearAttenuation = 0.0f,
			QuadraticAttenuation = 1.0f,
			Brightness = 2,
			Color = Color.White,
			InnerConeAngle = 20,
			OuterConeAngle = 40,
			FogStrength = 1.0f,
			Owner = this,
			Parent = this,
			LightCookie = Texture.Load( "materials/effects/lightcookie.vtex" )
		};
	}
}
