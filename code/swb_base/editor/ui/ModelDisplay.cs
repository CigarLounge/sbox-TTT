﻿using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SWB_Base
{
    public class ModelDisplay : Panel
    {
        public SceneWorld SceneWorld { get; private set; }
        public SceneObject SceneObject { get; private set; }
        public Model ActiveModel { get { return SceneObject.Model; } }

        public Angles CamAngles = new(50f, 450f, 0.0f);
        public float CamDistance = 150;
        public Vector3 CamPos => Vector3.Up * 10 + CamAngles.Direction * -CamDistance;

        private readonly ScenePanel scene;
        private bool canMouseDrag;

        public ModelDisplay(string modelPath, bool canMouseDrag = false)
        {
            StyleSheet.Load("/swb_base/editor/ui/ModelDisplay.scss");

            this.canMouseDrag = canMouseDrag;
            SetMouseCapture(canMouseDrag);
            Style.PointerEvents = canMouseDrag ? "all" : "none";

            Style.FlexWrap = Wrap.Wrap;
            Style.JustifyContent = Justify.Center;
            Style.AlignItems = Align.Center;
            Style.AlignContent = Align.Center;

            SceneWorld = new SceneWorld();

            using (SceneWorld.SetCurrent(SceneWorld))
            {
                SceneObject = SceneObject.CreateModel(modelPath, Transform.Zero);

                var maxX = SceneObject.Model.Bounds.Maxs.x;
                var maxZ = SceneObject.Model.Bounds.Maxs.z;
                SceneObject.Position += new Vector3(-maxX / 2, 0, maxZ / 2);

                SceneObject.CreateModel("models/room.vmdl", new Transform(SceneObject.Model.Bounds.Mins - 10));

                Light.Point(Vector3.Up * 150.0f, 9999, Color.White);
                Light.Point(Vector3.Up * 10.0f + Vector3.Forward * 100.0f, 9999, Color.White);
                Light.Point(Vector3.Up * 10.0f + Vector3.Backward * 100.0f, 9999, Color.White);
                Light.Point(Vector3.Up * 10.0f + Vector3.Left * 100.0f, 9999, Color.White);
                Light.Point(Vector3.Up * 10.0f + Vector3.Right * 100.0f, 9999, Color.White);

                scene = Add.ScenePanel(SceneWorld.Current, CamPos, Rotation.From(CamAngles), 45);
            }
        }

        public override void OnMouseWheel(float value)
        {
            CamDistance += value;
            CamDistance = CamDistance.Clamp(10, 200);

            base.OnMouseWheel(value);
        }

        public override void OnButtonEvent(ButtonEvent e)
        {
            if (e.Button == "mouseleft")
            {
                SetMouseCapture(e.Pressed);
            }

            base.OnButtonEvent(e);
        }

        public override void Tick()
        {
            base.Tick();

            if (canMouseDrag && HasMouseCapture)
            {
                CamAngles.pitch += Mouse.Delta.y;
                CamAngles.yaw -= Mouse.Delta.x;
                CamAngles.pitch = CamAngles.pitch.Clamp(0, 90);
            }

            scene.CameraPosition = MathUtil.FILerp(scene.CameraPosition, CamPos, 10);

            var newAngles = MathUtil.FILerp(scene.CameraRotation.Angles(), CamAngles, 10);
            scene.CameraRotation = Rotation.From(newAngles);
        }
    }
}
