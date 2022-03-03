using Sandbox;

namespace TTT;

public partial class Flashlight : EntityComponent
{
	public Player Player => Entity as Player;
	private SpotLightEntity worldLight;
	private SpotLightEntity viewLight;

	[Net, Local, Predicted]
	private bool LightEnabled { get; set; } = false;

	TimeSince timeSinceLightToggled;

	public void Simulate()
	{
		bool toggle = Input.Pressed( InputButton.Flashlight );

		if ( worldLight.IsValid() )
		{
			var transform = new Transform( Player.EyePosition + Player.EyeRotation.Forward * 20, Player.EyeRotation );
			worldLight.Transform = (Player.ActiveChild as Carriable)?.GetAttachment( "muzzle" ) ?? transform;
		}

		if ( timeSinceLightToggled > 0.1f && toggle )
		{
			LightEnabled = !LightEnabled;

			Entity.PlaySound( LightEnabled ? "flashlight-on" : "flashlight-off" );

			if ( worldLight.IsValid() )
				worldLight.Enabled = LightEnabled;

			timeSinceLightToggled = 0;
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();

		if ( Host.IsServer )
		{
			worldLight = CreateLight();
			worldLight.SetParent( Entity );
			worldLight.EnableHideInFirstPerson = true;
			worldLight.Enabled = false;
		}
		else
		{
			viewLight = CreateLight();
			viewLight.SetParent( Entity );
			viewLight.EnableViewmodelRendering = true;
			viewLight.Enabled = LightEnabled;
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		worldLight?.Delete();
		viewLight?.Delete();
	}

	[Event.Frame]
	public void Frame()
	{
		if ( !viewLight.IsValid() )
			return;

		viewLight.Enabled = Player.IsFirstPersonMode && LightEnabled;
		if ( !viewLight.Enabled )
			return;

		var transform = new Transform( Player.EyePosition + Player.EyeRotation.Forward * 10, Player.EyeRotation );
		viewLight.Transform = (Player.ActiveChild as Carriable)?.ViewModelEntity?.GetAttachment( "muzzle" ) ?? transform;
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
			Brightness = 1,
			Color = Color.White,
			InnerConeAngle = 20,
			OuterConeAngle = 40,
			FogStength = 1.0f,
			Owner = Entity,
			LightCookie = Texture.Load( "materials/effects/lightcookie.vtex" )
		};

		return light;
	}
}
