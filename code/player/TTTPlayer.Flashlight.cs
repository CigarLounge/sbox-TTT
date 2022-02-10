using Sandbox;

namespace TTT.Player
{
	public partial class TTTPlayer
	{
		private Flashlight _worldFlashlight;
		private Flashlight _viewFlashlight;

		private const float FLASHLIGHT_DISTANCE = 35f;
		private const float SMOOTH_SPEED = 25f;

		public bool HasFlashlightEntity
		{
			get
			{
				if ( IsLocalPawn )
				{
					return _viewFlashlight != null && _viewFlashlight.IsValid();
				}

				return _worldFlashlight != null && _worldFlashlight.IsValid();
			}
		}

		public bool IsFlashlightOn
		{
			get
			{
				if ( IsLocalPawn )
				{
					return HasFlashlightEntity && _viewFlashlight.Enabled;
				}

				return HasFlashlightEntity && _worldFlashlight.Enabled;
			}
		}

		public void ToggleFlashlight()
		{
			ShowFlashlight( !IsFlashlightOn );
		}

		public void ShowFlashlight( bool shouldShow, bool playSounds = true )
		{
			if ( IsFlashlightOn == shouldShow )
			{
				return;
			}

			if ( IsFlashlightOn )
			{
				if ( IsServer )
				{
					_worldFlashlight.TurnOff();
				}
				else
				{
					_viewFlashlight.TurnOff();
				}
			}

			if ( IsServer )
			{
				using ( Prediction.Off() )
				{
					ClientShowFlashlightLocal( To.Single( this ), shouldShow );
				}
			}

			if ( shouldShow )
			{
				if ( !HasFlashlightEntity )
				{
					if ( IsServer )
					{
						_worldFlashlight = new();
						_worldFlashlight.EnableHideInFirstPerson = true;
						_worldFlashlight.Rotation = EyeRotation;
						_worldFlashlight.Position = EyePosition + EyeRotation.Forward * FLASHLIGHT_DISTANCE;
						_worldFlashlight.SetParent( this );
					}
					else
					{
						_viewFlashlight = new();
						_viewFlashlight.EnableViewmodelRendering = false;
						_viewFlashlight.Position = EyePosition + EyeRotation.Forward * FLASHLIGHT_DISTANCE;
						_viewFlashlight.Rotation = EyeRotation;
						_viewFlashlight.SetParent( this );
					}
				}
				else
				{
					if ( IsServer )
					{
						_worldFlashlight.SetParent( null );
						_worldFlashlight.Rotation = EyeRotation;
						_worldFlashlight.Position = EyePosition + EyeRotation.Forward * FLASHLIGHT_DISTANCE;
						_worldFlashlight.SetParent( this );
						_worldFlashlight.TurnOn();
					}
					else
					{
						_viewFlashlight.TurnOn();
					}
				}

				if ( IsServer && playSounds )
				{
					PlaySound( "flashlight-on" );
				}
			}
			else if ( IsServer && playSounds )
			{
				PlaySound( "flashlight-off" );
			}
		}

		private void TickPlayerFlashlight()
		{
			if ( IsServer )
			{
				using ( Prediction.Off() )
				{
					if ( Input.Released( InputButton.Flashlight ) )
					{
						ToggleFlashlight();
					}
				}

				if ( IsFlashlightOn )
				{
					_worldFlashlight.Rotation = Rotation.Slerp( _worldFlashlight.Rotation, Input.Rotation, SMOOTH_SPEED );
					_worldFlashlight.Position = Vector3.Lerp( _worldFlashlight.Position, EyePosition + Input.Rotation.Forward * FLASHLIGHT_DISTANCE, SMOOTH_SPEED );
				}
			}
		}

		public override void PostCameraSetup( ref CameraSetup camSetup )
		{
			base.PostCameraSetup( ref camSetup );

			if ( IsFlashlightOn )
			{
				_viewFlashlight.Rotation = Input.Rotation;
				_viewFlashlight.Position = EyePosition + Input.Rotation.Forward * FLASHLIGHT_DISTANCE;
			}
		}
	}

	[Hammer.Skip]
	[Library( "ttt_flashlight", Title = "Flashlight" )]
	public partial class Flashlight : SpotLightEntity
	{
		public Flashlight() : base()
		{
			Transmit = TransmitType.Always;
			Enabled = true;
			DynamicShadows = true;
			Range = 512f;
			Falloff = 4f;
			LinearAttenuation = 0f;
			QuadraticAttenuation = 1f;
			Brightness = 1f;
			Color = Color.White;
			InnerConeAngle = 10f;
			OuterConeAngle = 30f;
			FogStength = 1f;
		}
	}
}
