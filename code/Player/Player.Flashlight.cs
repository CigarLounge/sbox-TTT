using Sandbox;

namespace TTT;

public partial class Player
{
	[Net, Predicted]
	public bool FlashlightEnabled { get; private set; } = false;

	[Net, Local, Predicted]
	public TimeSince TimeSinceLightToggled { get; private set; }

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
			transform.Rotation *= Rotation.From( new Angles( 0, 90, 0 ) );
			_worldLight.Transform = transform;

			if ( ActiveChild.IsValid() )
				_worldLight.Transform = ActiveChild.GetAttachment( "muzzle" ) ?? transform;
		}

		if ( TimeSinceLightToggled > 0.25f && toggle )
		{
			FlashlightEnabled = !FlashlightEnabled;

			PlaySound( FlashlightEnabled ? "flashlight-on" : "flashlight-off" );

			if ( _worldLight.IsValid() )
				_worldLight.Enabled = FlashlightEnabled;

			TimeSinceLightToggled = 0;
		}
	}

	protected void CreateFlashlight()
	{
		if ( Host.IsServer )
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

	[ConVar.Client( "ttt_visualizeflashlight" )]
	public static bool VisualizeFlashlight { get; set; } = false;

	[Event.Frame]
	private void Frame()
	{
		if ( !_viewLight.IsValid() )
			return;

		_viewLight.Enabled = FlashlightEnabled & IsFirstPersonMode;

		if ( !_viewLight.Enabled )
			return;

		var eyeTransform = new Transform( EyePosition, EyeRotation );
		_viewLight.Transform = eyeTransform;

		if ( !ActiveChild.IsValid() )
			return;

		var muzzleTransform = ActiveChild.ViewModelEntity?.GetAttachment( "muzzle" );

		if ( !muzzleTransform.HasValue )
			return;

		var mz = muzzleTransform.Value;

		// First check if there is something like a door/wall between the muzzle and our eyes.
		var muzzleTrace = Trace.Ray( mz.Position, EyePosition )
			.Size( 2 )
			.Ignore( this )
			.Ignore( ActiveChild )
			.Run();

		Vector3 downOffset = Vector3.Down * 2f;
		var origin = mz.Position + downOffset;

		// If there IS something between our eyes and the muzzle, add the distance.
		if ( muzzleTrace.Hit)
		{
			var dist = (muzzleTrace.EndPosition - mz.Position).Length;
			origin = muzzleTrace.EndPosition + (mz.Rotation.Backward * dist) + downOffset;
		}

		// Continue with the forward trace.
		var destination = origin + mz.Rotation.Forward * _viewLight.Range;
		var direction = destination - origin;

		var fwdTrace = Trace.Box( Vector3.One * 2, origin, destination )
			.Ignore( this )
			.Ignore( ActiveChild )
			.Run();

		var distance = (fwdTrace.EndPosition - origin).Length;

		float pullbackMult = 0;
		const float pullbackThreshold = 48f;
		if ( distance < pullbackThreshold )
		{
			var pullbackAmount = pullbackThreshold - distance;
			pullbackMult = MathX.Remap( pullbackAmount, 0, pullbackThreshold, 0.0f, 0.045f );
		}

		origin -= direction * pullbackMult;
		_viewLight.Position = origin;
		_viewLight.Rotation = mz.Rotation;

		if ( !VisualizeFlashlight )
			return;

		DebugOverlay.Sphere( origin, 2, Color.Green, depthTest: false );
		DebugOverlay.Line( origin, fwdTrace.EndPosition, Color.Red );
		DebugOverlay.Box( fwdTrace.EndPosition, Vector3.One * -4, Vector3.One * 4, Color.Blue, depthTest: false );
		DebugOverlay.Sphere( mz.Position, 2f, Color.Red );
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
