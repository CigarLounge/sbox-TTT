﻿/* 
 * Weapon base attachment model
*/

using Sandbox;

namespace SWB_Base
{
    public class AttachmentModel : AnimEntity
    {
        public AttachmentModel() { }

        public AttachmentModel(bool isViewModel)
        {
            EnableViewmodelRendering = isViewModel;
            EnableHideInFirstPerson = !isViewModel;
        }

        public override void PostCameraSetup(ref CameraSetup camSetup) { }
    }
}
