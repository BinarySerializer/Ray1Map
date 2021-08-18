using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Klonoa;
using BinarySerializer.Klonoa.DTP;
using BinarySerializer.PS1;
using UnityEngine;

namespace R1Engine
{
    public class PSKlonoa_DTP_ModifiersLoader
    {
        public PSKlonoa_DTP_ModifiersLoader(PSKlonoa_DTP_BaseManager manager, Loader_DTP loader, float scale, GameObject parentObject, ModifierObject[] modifiers)
        {
            Manager = manager;
            Loader = loader;
            Scale = scale;
            ParentObject = parentObject;
            Modifiers = modifiers;

            GameObjects = new List<GameObject>();
            TextureAnimations = new List<PS1_TIM[]>();
            ScrollAnimations = new List<UVScrollAnimation_File>();
        }

        public PSKlonoa_DTP_BaseManager Manager { get; }
        public Loader_DTP Loader { get; }
        public float Scale { get; }
        public GameObject ParentObject { get; }
        public ModifierObject[] Modifiers { get; }

        public bool IsAnimated { get; set; }
        public List<GameObject> GameObjects { get; }
        public List<PS1_TIM[]> TextureAnimations { get; }
        public List<UVScrollAnimation_File> ScrollAnimations { get; }

        public void Load()
        {
            // Loop twice, ensuring all texture animations are loaded before the objects
            for (int i = 0; i < 2; i++)
            {
                foreach (var modifier in Modifiers)
                    LoadModifier(modifier, i);
            }
        }

        public void LoadModifier(ModifierObject modifier, int loop)
        {
            if (modifier.PrimaryType == PrimaryObjectType.Invalid ||
                modifier.PrimaryType == PrimaryObjectType.None ||
                modifier.SecondaryType == -1 ||
                modifier.SecondaryType == 0)
                return;

            switch (modifier.GlobalModifierType)
            {
                case GlobalModifierType.Unknown:
                {
                    if (loop != 0)
                        return;

                    Debug.LogWarning($"Unknown modifier type at {modifier.Offset} of type " +
                                     $"{(int)modifier.PrimaryType}-{modifier.SecondaryType} with data:{Environment.NewLine}" +
                                     $"{String.Join(Environment.NewLine, modifier.DataFiles.Select(x => ByteArrayExtensions.ToHexString(x.Unknown, align: 16, maxLines: 16)))}");
                    return;
                }

                case GlobalModifierType.WindSwirl:
                {
                    if (loop != 1)
                        return;

                    var obj_0 = Load_TMD(modifier, modifier.DataFiles[0].TMD);
                    var obj_1 = Load_TMD(modifier, modifier.DataFiles[1].TMD, index: 1);

                    var pos = modifier.DataFiles[2].Position;

                    ApplyPosition(obj_0, pos, new Vector3Int(0, 182, 0));
                    ApplyPosition(obj_1, pos);
                    ApplyConstantRotation(obj_0, modifier.RotationAttribute);
                } 
                    break;

                case GlobalModifierType.BigWindmill:
                case GlobalModifierType.SmallWindmill:
                {
                    if (loop != 1)
                        return;

                    var obj = Load_TMD(modifier, modifier.DataFiles[0].TMD, absoluteTransform: modifier.DataFiles[1].Transform);
                    ApplyConstantRotation(obj, modifier.RotationAttribute);
                }
                    break;

                case GlobalModifierType.MovingPlatform:
                {
                    if (loop != 1)
                        return;

                    Load_TMD(modifier, modifier.DataFiles[0].TMD, absoluteTransform: modifier.DataFiles[3].Transform);
                }
                    break;

                case GlobalModifierType.RoadSign:
                {
                    if (loop != 1)
                        return;

                    Load_TMD(modifier, modifier.DataFiles[0].TMD, absoluteTransform: modifier.DataFiles[1].Transform);
                } 
                    break;

                case GlobalModifierType.ScrollAnimation:
                {
                    if (loop != 0)
                        return;

                    ScrollAnimations.Add(modifier.DataFiles[0].UVScrollAnimation);
                } 
                    break;

                case GlobalModifierType.Object:
                {
                    if (loop != 1)
                        return;

                    Load_TMD(modifier, modifier.DataFiles[0].TMD);
                }
                    break;

                case GlobalModifierType.LevelModelSection:
                {
                    if (loop != 1)
                        return;

                    Load_TMD(modifier, modifier.DataFiles[0].TMD);
                } 
                    break;

                case GlobalModifierType.ScenerySprites:
                {
                    if (loop != 1)
                        return;

                    // TODO: Get correct sprites to show
                }
                    break;

                case GlobalModifierType.TextureAnimation:
                {
                    if (loop != 0)
                        return;

                    TextureAnimations.Add(modifier.DataFiles[0].TextureAnimation.Files);
                } 
                    break;

                case GlobalModifierType.Special:
                default:
                    // Do nothing
                    break;
            }
        }

        // TODO: Apply TMD modifications after creating
        public GameObject Load_TMD(ModifierObject modifier, PS1_TMD tmd, ObjTransform_ArchiveFile localTransform = null, ObjTransform_ArchiveFile absoluteTransform = null, int index = 0)
        {
            if (tmd == null) 
                throw new ArgumentNullException(nameof(tmd));
            
            GameObject gameObj;
            bool isAnimated;

            (gameObj, isAnimated) = Manager.CreateGameObject(
                tmd: tmd,
                loader: Loader,
                scale: Scale,
                name: $"Object3D Offset:{modifier.Offset} Index:{index} Type:{modifier.PrimaryType}-{modifier.SecondaryType} ({modifier.GlobalModifierType})",
                texAnimations: TextureAnimations,
                scrollAnimations: new UVScrollAnimation_File[0],
                positions: localTransform?.Positions.Positions[0].Select(x => Manager.GetPositionVector(x, Vector3.zero, Scale)).ToArray(),
                rotations: localTransform?.Rotations.Rotations[0].Select(x => Manager.GetQuaternion(x)).ToArray());

            if (isAnimated)
                IsAnimated = true;

            if (absoluteTransform != null)
            {
                if (tmd.ObjectsCount > 1)
                    Debug.LogWarning($"An absolute transform was used on a TMD with multiple objects!");

                gameObj.transform.localPosition = Manager.GetPositionVector(absoluteTransform.Positions.Positions[0][0], null, Scale);
                gameObj.transform.localRotation = Manager.GetQuaternion(absoluteTransform.Rotations.Rotations[0][0]);
            }
            else
            {
                gameObj.transform.localPosition = Vector3.zero;
                gameObj.transform.localRotation = Quaternion.identity;
            }

            gameObj.transform.SetParent(ParentObject.transform);

            if (absoluteTransform?.Positions.Positions.Length > 1)
            {
                var mtComponent = gameObj.AddComponent<AnimatedTransformComponent>();
                mtComponent.animatedTransform = gameObj.transform;

                var positions = absoluteTransform.Positions.Positions.Select(x => Manager.GetPositionVector(x[0], null, Scale)).ToArray();
                var rotations = absoluteTransform.Rotations.Rotations.Select(x => Manager.GetQuaternion(x[0])).ToArray();

                var frameCount = Math.Max(positions.Length, rotations.Length);
                mtComponent.frames = new AnimatedTransformComponent.Frame[frameCount];

                for (int i = 0; i < frameCount; i++)
                {
                    mtComponent.frames[i] = new AnimatedTransformComponent.Frame()
                    {
                        Position = positions[i],
                        Rotation = rotations[i],
                        Scale = Vector3.one
                    };
                }
            }

            GameObjects.Add(gameObj);
            return gameObj;
        }

        public void ApplyPosition(GameObject obj, ObjPosition pos, Vector3? posOffset = null)
        {
            obj.transform.position = Manager.GetPositionVector(pos, posOffset, Scale);
        }

        public void ApplyConstantRotation(GameObject obj, ModifierRotationAttribute rot)
        {
            if (obj == null) 
                throw new ArgumentNullException(nameof(obj));
            if (rot == null) 
                throw new ArgumentNullException(nameof(rot));

            var mtComponent = obj.AddComponent<AnimatedTransformComponent>();
            mtComponent.animatedTransform = obj.transform;

            int rotationPerFrame = Math.Abs(rot.Rotation);
            int count = 0x1000 / rotationPerFrame;

            mtComponent.frames = new AnimatedTransformComponent.Frame[count];

            for (int i = 0; i < count; i++)
            {
                var degrees = Manager.GetRotationInDegrees(i * rotationPerFrame);

                if (rot.Rotation > 0)
                    degrees = -degrees;

                mtComponent.frames[i] = new AnimatedTransformComponent.Frame()
                {
                    Position = obj.transform.position,
                    Rotation = obj.transform.localRotation * Quaternion.Euler(
                        x: rot.Axis == ModifierRotationAttribute.RotAxis.X ? degrees * 1 : 0,
                        y: rot.Axis == ModifierRotationAttribute.RotAxis.Y ? degrees * 1 : 0,
                        z: rot.Axis == ModifierRotationAttribute.RotAxis.Z ? degrees * 1 : 0),
                    Scale = Vector3.one
                };
            }
        }
    }
}