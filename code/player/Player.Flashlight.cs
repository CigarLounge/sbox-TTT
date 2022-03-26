using Sandbox;

namespace TTT;

public partial class Player
{
	[Net, Predicted]
	public bool FlashlightEnabled { get; private set; } = false;

	[Net, Local, Predicted]
	public TimeSince TimeSinceLightToggled { get; private set; }

	private SpotLightEntity worldLight;
	private SpotLightEntity viewLight;

	public void SimulateFlashlight()
	{
		bool toggle = Input.Pressed( InputButton.Flashlight );

		if ( worldLight.IsValid() )
		{
			var transform = new Transform( EyePosition + EyeRotation.Forward * 20, EyeRotation );
			worldLight.Transform = transform;

			if ( ActiveChild.IsValid() && ActiveChild is Carriable carriable )
				worldLight.Transform = carriable.GetAttachment( "muzzle" ) ?? transform;
		}

		if ( TimeSinceLightToggled > 0.1f && toggle )
		{
			FlashlightEnabled = !FlashlightEnabled;

			PlaySound( FlashlightEnabled ? "flashlight_on" : "flashlight_off" );

			if ( worldLight.IsValid() )
				worldLight.Enabled = FlashlightEnabled;

			TimeSinceLightToggled = 0;
		}
	}

	protected void ActivateFlashlight()
	{
		if ( Host.IsServer )
		{
			worldLight = CreateLight();
			worldLight.Parent = this;
			worldLight.EnableHideInFirstPerson = true;
			worldLight.Enabled = false;
		}
		else
		{
			viewLight = CreateLight();
			viewLight.Parent = this;
			viewLight.EnableViewmodelRendering = true;
			viewLight.Enabled = FlashlightEnabled;
		}
	}

	protected void DeactivateFlashlight()
	{
		worldLight?.Delete();
		viewLight?.Delete();
	}

	public void FrameSimulateFlashlight()
	{
		if ( !viewLight.IsValid() )
			return;

		viewLight.Enabled = FlashlightEnabled;

		if ( !viewLight.Enabled )
			return;

		var transform = new Transform( EyePosition + EyeRotation.Forward * 10, EyeRotation );
		viewLight.Transform = transform;

		if ( ActiveChild.IsValid() && ActiveChild is Carriable carriable )
			viewLight.Transform = carriable.ViewModelEntity?.GetAttachment( "muzzle" ) ?? transform;
	}

	private SpotLightEntity CreateLight()
	{
		var light = new SpotLightEntity
		{
			Enabled = true,
			DynamicShadows = true,
			Range = 512,
			Falloff = 1.0f,
			LinearAttenuation = 0.0f,
			QuadraticAttenuation = 1.0f,
			Brightness = 2,
			Color = Color.White,
			InnerConeAngle = 20,
			OuterConeAngle = 40,
			FogStength = 1.0f,
			Owner = this,
			LightCookie = Texture.Load( "materials/effects/lightcookie.vtex" )
		};

		return light;
	}
}
