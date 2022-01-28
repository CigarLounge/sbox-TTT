﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sandbox;
using Sandbox.UI;

namespace SWB_Base
{
    [UseTemplate]
    public class AttachmentEditorMenu : ModelEditorMenu
    {
        public float Scale { get; set; } = 1f;
        public string BoneName { get; set; }

        public TextEntry ModelInput { get; set; }
        public DropDown BoneDropDown { get; set; }
        public Button ViewModelButton { get; set; }
        public Button WorldModelButton { get; set; }
        public ModelDisplay ModelDisplay { get; set; }

        private AttachmentModel activeModel;
        private WeaponBase activeWeapon;
        private List<SceneObject> activeWorldAttachments = new List<SceneObject>();

        private Angles defaultModelAngles;

        private string modelPlaceholder = "path/to/attachment.vmdl";

        private bool isValidAttachment;
        private bool isEditingViewModel = true;

        private string[] blacklistedBones = new string[] {
            "l_",
            "r_",
            "v_",
            "c_"
        };

        public AttachmentEditorMenu() : base()
        {
            // Weapon Model
            var player = Local.Pawn as PlayerBase;
            if (player == null) return;

            activeWeapon = player.ActiveChild as WeaponBase;

            if (activeWeapon != null)
            {
                var viewModel = activeWeapon.ViewModelEntity;

                FillBoneList(viewModel.Model);
            }

            // ModelInput
            ModelInput.Text = "attachments/swb/muzzle/silencer_pistol/silencer_pistol.vmdl"; //modelPlaceholder;
            ModelInput.AddEventListener("onmousedown", () =>
            {
                if (ModelInput.Text == modelPlaceholder)
                {
                    ModelInput.Text = "";
                }
            });

            ModelInput.AddEventListener("value.changed", () => OnModelChange(ModelInput.Text));

            // BoneDropDown
            BoneDropDown.AddEventListener("value.changed", () => CreateAttachmentModels());
        }

        public void FillBoneList(Model model)
        {
            BoneDropDown.Selected = new Option("", -1);
            BoneDropDown.Options.Clear();

            for (var i = 0; i < model.BoneCount; i++)
            {
                var name = model.GetBoneName(i);
                var canUseBone = true;

                foreach (string word in blacklistedBones)
                {
                    if (name.StartsWith(word))
                    {
                        canUseBone = false;
                        break;
                    }
                }

                if (canUseBone)
                    BoneDropDown.Options.Add(new Option(name, name));
            }
        }

        // No dragging if not directly on base panel
        public override bool CanDragOnPanel(Panel p)
        {
            return p.ElementName == "attachmenteditormenu" || p.ElementName == "scenepanel";
        }

        public void OnClose()
        {
            // Cleanup
            if (activeModel != null)
            {
                activeModel.Delete();
                activeModel = null;
            }

            if (activeWeapon != null && activeWeapon.ViewModelEntity != null && !isEditingViewModel)
            {
                activeWeapon.ViewModelEntity.EnableDrawing = true;
            }
        }

        private void CreateAttachmentModels()
        {
            if (!isEditingViewModel && ModelDisplay != null)
            {
                // World Model
                foreach (var sceneObj in activeWorldAttachments)
                {
                    sceneObj.Delete();
                }

                // Create active attachment models on worlmodel
                foreach (var activeAttach in activeWeapon.ActiveAttachments)
                {
                    var attach = (OffsetAttachment)activeWeapon.GetAttachment(activeAttach.Name);
                    CreateAttachmentModel(attach.ModelPath, attach.WorldTransform);
                }
            }

            // Create attachment model
            var pos = new Vector3(X, Y, Z);
            var ang = new Angles(Pitch, Yaw, Roll);
            var transform = new Transform(pos, Rotation.From(ang), Scale);

            CreateAttachmentModel(ModelInput.Text, transform);
        }

        private void CreateAttachmentModel(string model, Transform transform)
        {
            if (!isValidAttachment || activeWeapon == null || BoneName == null || BoneName == "-1") return;

            if (isEditingViewModel)
            {
                // View Model
                if (activeModel != null)
                {
                    activeModel.Delete();
                    activeModel = null;
                }

                activeModel = new AttachmentModel(true);
                activeModel.Owner = activeWeapon.Owner;
                activeModel.SetModel(model);
                activeModel.SetParent(activeWeapon.ViewModelEntity, BoneName, transform);
            }
            else if (ModelDisplay != null)
            {
                // Bone base offsets
                var weaponTrans = activeWeapon.Transform;
                var attachTrans = activeWeapon.GetBoneTransform(BoneName);
                var editorOffsetPos = attachTrans.PointToWorld(transform.Position);
                var boneOffsetPos = weaponTrans.PointToLocal(editorOffsetPos);
                var boneOffetRot = weaponTrans.RotationToLocal(attachTrans.Rotation);

                var attachRot = boneOffetRot * transform.Rotation;
                var attachPos = boneOffsetPos + ModelDisplay.SceneObject.Position;
                var modelTransform = new Transform(attachPos, attachRot, transform.Scale);

                using (SceneWorld.SetCurrent(ModelDisplay.SceneWorld))
                {
                    var sceneObject = SceneObject.CreateModel(model, modelTransform);
                    activeWorldAttachments.Add(sceneObject);
                    ModelDisplay.SceneObject.AddChild("attach" + activeWorldAttachments.Count, sceneObject);
                }
            }
        }

        public void OnModelChange(string modelPath)
        {
            var rx = new Regex(@"\.vmdl$", RegexOptions.IgnoreCase);

            if (!rx.IsMatch(modelPath) || !FileSystem.Mounted.FileExists(modelPath))
            {
                ModelInput.AddClass("invalid");
                isValidAttachment = false;
                return;
            }

            isValidAttachment = true;
            ModelInput.AddClass("valid");
            CreateAttachmentModels();
        }

        public void OnSliderChange()
        {
            CreateAttachmentModels();
        }

        public override void OnReset()
        {
            base.OnReset();
            Scale = 1f;
        }

        public override void OnCopy()
        {
            var dataStr = String.Format("new Transform {{ Position = new Vector3({3:0.###}f, {4:0.###}f, {5:0.###}f), Rotation = Rotation.From(new Angles({0:0.###}f, {1:0.###}f, {2:0.###}f)), Scale = {6:0.###}f }},", Pitch, Yaw, Roll, X, Y, Z, Scale);
            Clipboard.SetText(dataStr);
        }

        public override void Tick()
        {
        }

        // Model switching
        public void OnEditViewModel()
        {
            if (isEditingViewModel) return;
            isEditingViewModel = true;

            ViewModelButton.SetClass("selected", true);
            WorldModelButton.SetClass("selected", false);

            activeWeapon.ViewModelEntity.EnableDrawing = true;

            if (ModelDisplay != null)
            {
                ModelDisplay.Delete(true);
                ModelDisplay = null;
            }

            FillBoneList(activeWeapon.ViewModelEntity.Model);
            CreateAttachmentModels();
        }

        public void OnEditWorldModel()
        {
            if (!isEditingViewModel) return;
            isEditingViewModel = false;

            ViewModelButton.SetClass("selected", false);
            WorldModelButton.SetClass("selected", true);

            activeWeapon.ViewModelEntity.EnableDrawing = false;

            ModelDisplay = new ModelDisplay(activeWeapon.WorldModelPath);
            ModelDisplay.Parent = this;
            defaultModelAngles = ModelDisplay.CamAngles;

            FillBoneList(activeWeapon.Model);
            CreateAttachmentModels();
        }


        // Weapon Rotation
        private int leftRotation = 0;
        private int rightRotation = 0;
        private int upRotation = 0;
        private int downRotation = 0;

        private int worldModelRotationAmount = 15;

        public void OnRotateReset()
        {
            if (activeWeapon == null) return;
            if (isEditingViewModel && activeWeapon.ViewModelEntity is ViewModelBase viewModel)
            {
                viewModel.editorOffset = new AngPos();
            }
            else if (ModelDisplay != null)
            {
                ModelDisplay.CamAngles = defaultModelAngles;
            }

            leftRotation = 0;
            rightRotation = 0;
            upRotation = 0;
            downRotation = 0;
        }

        private void RotateWeapon(AngPos offset)
        {
            if (activeWeapon == null) return;
            if (isEditingViewModel && activeWeapon.ViewModelEntity is ViewModelBase viewModel)
            {
                viewModel.editorOffset += offset;
            }
            else if (ModelDisplay != null)
            {
                ModelDisplay.CamAngles += offset.Angle;
            }
        }

        public void OnRotateLeft()
        {
            if (rightRotation != 0 && isEditingViewModel)
                OnRotateReset();

            leftRotation += 2;

            RotateWeapon(new AngPos
            {
                Angle = new Angles(0, isEditingViewModel ? 45 : -worldModelRotationAmount, 0),
                Pos = new Vector3(17, -5 * leftRotation, 0),
            });
        }

        public void OnRotateRight()
        {
            if (leftRotation != 0 && isEditingViewModel)
                OnRotateReset();

            rightRotation++;

            RotateWeapon(new AngPos
            {
                Angle = new Angles(0, isEditingViewModel ? -45 : worldModelRotationAmount, 0),
                Pos = new Vector3(-17 * rightRotation, -5 * rightRotation, 0),
            });
        }

        public void OnRotateUp()
        {
            if (downRotation != 0 && isEditingViewModel)
                OnRotateReset();

            upRotation++;

            RotateWeapon(new AngPos
            {
                Angle = new Angles(isEditingViewModel ? -45 : worldModelRotationAmount, 0, 0),
                Pos = new Vector3(0, -5 * upRotation, -15),
            });
        }

        public void OnRotateDown()
        {
            if (upRotation != 0 && isEditingViewModel)
                OnRotateReset();

            downRotation++;

            RotateWeapon(new AngPos
            {
                Angle = new Angles(isEditingViewModel ? 45 : -worldModelRotationAmount, 0, 0),
                Pos = new Vector3(0, -5 * downRotation, 20),
            });
        }
    }
}
