using Sandbox;

namespace TTT;

public partial class Player
{
	private SpotLightEntity worldLight;
	private SpotLightEntity viewLight;

	[Net, Predicted]
	private bool FlashlightEnabled { get; set; } = false;

	[Net, Local, Predicted]
	public TimeSince TimeSinceLightToggled { get; set; }

	public void SimulateFlashlight()
	{
		bool toggle = Input.Pressed( InputButton.Flashlight );

		if ( worldLight.IsValid() )
		{
			var transform = new Transform( EyePosition + EyeRotation.Forward * 20, EyeRotation );
			worldLight.Transform = transform;

			if ( ActiveChild.IsValid() )
				worldLight.Transform = (ActiveChild as Carriable).GetAttachment( "muzzle" ) ?? transform;
		}

		if ( TimeSinceLightToggled > 0.1f && toggle )
		{
			FlashlightEnabled = !FlashlightEnabled;

			PlaySound( FlashlightEnabled ? "flashlight-on" : "flashlight-off" );

			if ( worldLight.IsValid() )
				worldLight.Enabled = FlashlightEnabled;

			if ( viewLight.IsValid() )
				viewLight.Enabled = FlashlightEnabled;

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
		if ( !viewLight.IsValid() || !viewLight.Enabled )
			return;

		var transform = new Transform( EyePosition + EyeRotation.Forward * 10, EyeRotation );
		viewLight.Transform = transform;

		if ( ActiveChild.IsValid() )
			viewLight.Transform = (ActiveChild as Carriable)?.ViewModelEntity?.GetAttachment( "muzzle" ) ?? transform;
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
