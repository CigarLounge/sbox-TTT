﻿using Sandbox;
using Sandbox.UI;
using SWB_Base.UI;

/* 
 * Weapon base for sniper based zooming
*/

namespace SWB_Base
{
    public partial class WeaponBaseSniper : WeaponBase
    {
        /// <summary>Path to the lens texture</summary>
        public virtual string LensTexture => "/materials/swb/scopes/swb_lens_hunter.png";

        /// <summary>Path to the scope texture</summary>
        public virtual string ScopeTexture => "/materials/swb/scopes/swb_scope_hunter.png";

        /// <summary>Sound to play when zooming in</summary>
        public virtual string ZoomInSound => "swb_sniper.zoom_in";

        /// <summary>Sound to play when zooming out</summary>
        public virtual string ZoomOutSound => "";

        /// <summary>The amount to zoom in (lower is more)</summary>
        public virtual float ZoomAmount => 20f;

        /// <summary>EXPERIMENTAL - Use a render target instead of a full screen texture zoom</summary>
        public virtual bool UseRenderTarget => false;

        private Panel SniperScopePanel;
        private bool switchBackToThirdP = false;
        private float lerpZoomAmount = 0;
        private float oldSpread = -1;

        public override void ActiveStart(Entity ent)
        {
            base.ActiveStart(ent);
        }

        public override void ActiveEnd(Entity ent, bool dropped)
        {
            base.ActiveEnd(ent, dropped);

            SniperScopePanel?.Delete();
        }

        public virtual void OnScopedStart()
        {
            IsScoped = true;

            if (oldSpread == -1)
                oldSpread = Primary.Spread;

            Primary.Spread = 0;

            if (IsServer)
            {
                if (Owner.Camera is ThirdPersonCamera)
                {
                    switchBackToThirdP = true;
                    Owner.Camera = new FirstPersonCamera();
                }
            }

            if (IsLocalPawn)
            {
                ViewModelEntity.RenderColor = Color.Transparent;

                foreach (var child in ViewModelEntity.Children)
                {
                    (child as ModelEntity).RenderColor = Color.Transparent;
                }

                if (HandsModel != null)
                {
                    HandsModel.RenderColor = Color.Transparent;
                }

                if (!string.IsNullOrEmpty(ZoomInSound))
                    PlaySound(ZoomInSound);
            }
        }

        public virtual void OnScopedEnd()
        {
            IsScoped = false;
            Primary.Spread = oldSpread;
            lerpZoomAmount = 0;

            if (IsServer && switchBackToThirdP)
            {
                switchBackToThirdP = false;
                Owner.Camera = new ThirdPersonCamera();
            }

            if (IsLocalPawn)
            {
                ViewModelEntity.RenderColor = Color.White;

                foreach (var child in ViewModelEntity.Children)
                {
                    (child as ModelEntity).RenderColor = Color.White;
                }

                if (HandsModel != null)
                {
                    HandsModel.RenderColor = Color.White;
                }

                if (!string.IsNullOrEmpty(ZoomOutSound))
                    PlaySound(ZoomOutSound);
            }
        }

        public override void Simulate(Client owner)
        {
            base.Simulate(owner);
            var shouldTuck = ShouldTuck();

            if (((Input.Pressed(InputButton.Attack2) && !IsReloading && !IsRunning) || (IsZooming && !IsScoped)) && !shouldTuck)
            {
                OnScopedStart();
            }

            if (Input.Released(InputButton.Attack2) || (IsScoped && (IsRunning || shouldTuck)))
            {
                OnScopedEnd();
            }
        }
        public override void CreateHudElements()
        {
            base.CreateHudElements();

            if (Local.Hud == null) return;

            if (UseRenderTarget)
            {
                SniperScopePanel = new SniperScopeRT(LensTexture, ScopeTexture);
                SniperScopePanel.Parent = Local.Hud;
            }
            else
            {
                SniperScopePanel = new SniperScope(LensTexture, ScopeTexture);
                SniperScopePanel.Parent = Local.Hud;
            }
        }

        public override void PostCameraSetup(ref CameraSetup camSetup)
        {
            base.PostCameraSetup(ref camSetup);

            if (IsScoped)
            {
                if (lerpZoomAmount == 0)
                    lerpZoomAmount = camSetup.FieldOfView;

                lerpZoomAmount = MathUtil.FILerp(lerpZoomAmount, ZoomAmount, 10f);
                camSetup.FieldOfView = lerpZoomAmount;
            }
        }
    }
}
