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
		bool toggle = Input.Pressed( InputButton.Flashlight );

		if ( _worldLight.IsValid() )
		{
			var transform = GetAttachment( "eyes" ) ?? default;
			transform.Rotation *= Rotation.From( new Angles( 0, 90, 0 ) );
			_worldLight.Transform = transform;

			if ( ActiveChild.IsValid() && ActiveChild is Carriable carriable )
				_worldLight.Transform = carriable.GetAttachment( "muzzle" ) ?? transform;
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

	[Event.Frame]
	private void Frame()
	{
		if ( !_viewLight.IsValid() )
			return;

		_viewLight.Enabled = FlashlightEnabled & IsFirstPersonMode;

		if ( !_viewLight.Enabled )
			return;

		var transform = new Transform( EyePosition, EyeRotation );
		_viewLight.Transform = transform;

		if ( ActiveChild.IsValid() && ActiveChild is Carriable carriable )
			_viewLight.Transform = carriable.ViewModelEntity?.GetAttachment( "muzzle" ) ?? transform;
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
