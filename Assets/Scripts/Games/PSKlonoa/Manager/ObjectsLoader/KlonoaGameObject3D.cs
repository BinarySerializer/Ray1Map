﻿using BinarySerializer;
using BinarySerializer.Klonoa.DTP;
using BinarySerializer.PS1;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ray1Map.PSKlonoa
{
    public class KlonoaGameObject3D : KlonoaObject
    {
        public KlonoaGameObject3D(KlonoaObjectsLoader objLoader, GameObjectData obj) : base(objLoader)
        {
            Obj = obj;

            // Log unknown objects
            if (Obj.GlobalGameObjectType == GlobalGameObjectType.Unknown)
                Debug.LogWarning($"Unknown object type of type " +
                                 $"{(int)Obj.PrimaryType}-{Obj.SecondaryType} with data:{Environment.NewLine}" +
                                 $"{String.Join($"{Environment.NewLine}{Environment.NewLine}", Obj.UnknownData?.Select(dataFile => $"Length: {dataFile.Data.Length} bytes{Environment.NewLine}{dataFile.Data.ToHexString(align: 16, maxLines: 16)}") ?? new string[0])}");
        }

        private static HashSet<ModelAnimation_ArchiveFile> _correctedTransforms; // TODO: Find better solution

        public GameObjectData Obj { get; }

        public override void LoadAnimations()
        {
            if (Obj.RawTextureAnimation != null)
                ObjLoader.TextureAnimations.Add(new PS1VRAMAnimation(
                    region: Obj.RawTextureAnimation.Region, 
                    frames: Obj.RawTextureAnimation.Frames, 
                    speed: Obj.TextureAnimationInfo.AnimSpeed, 
                    pingPong: Obj.TextureAnimationInfo.PingPong));

            if (Obj.CameraAnimations != null)
                ObjLoader.CameraAnimations.Add(Obj.CameraAnimations);

            switch (Obj.GlobalGameObjectType)
            {
                case GlobalGameObjectType.ScrollAnimation:
                    ObjLoader.ScrollAnimations.Add(Obj.UVScrollAnimation);
                    break;

                case GlobalGameObjectType.VRAMScrollAnimation:
                case GlobalGameObjectType.VRAMScrollAnimationWithTexture:
                    KlonoaSettings_DTP.VRAMScrollInfo[] scroll = Obj.VRAMScrollInfos;

                    // Kind of hacky solution, but works... The game essentially just defines what data gets copied where in VRAM
                    for (int scrollIndex = 0; scrollIndex < scroll.Length; scrollIndex += 2)
                    {
                        // Due to the way the animations are set up we need to group them in two. Each region on the VRAM it updates will
                        // have two entries defined so it can handle the overflow. Since they both need to run together we group them.
                        var scroll_0 = scroll[scrollIndex + 0];
                        var scroll_1 = scroll[scrollIndex + 1];

                        var region = new PS1_VRAMRegion()
                        {
                            XPos = (short)Math.Min(scroll_0.DestinationX, scroll_1.DestinationX),
                            YPos = (short)Math.Min(scroll_0.DestinationY, scroll_1.DestinationY),
                            Width = scroll_0.Region.Width,
                            Height = (short)(Math.Max(scroll_0.DestinationY + scroll_0.Region.Height, scroll_1.DestinationY +
                                scroll_1.Region.Height) - Math.Min(scroll_0.DestinationY, scroll_1.DestinationY)),
                        };

                        int framesCount = region.Height / Math.Min(scroll_0.Region.Height, scroll_1.Region.Height);

                        var prevFrame = new byte[region.Width * 2 * region.Height];

                        // Fill out the first state to use
                        for (int y = 0; y < region.Height; y++)
                        {
                            for (int x = 0; x < region.Width * 2; x++)
                            {
                                prevFrame[y * region.Width * 2 + x] = VRAM.GetPixel8(0, 0, region.XPos * 2 + x, region.YPos + y);
                            }
                        }

                        var frames = new byte[framesCount][];

                        var buffer_0 = new byte[scroll_0.Region.Width * 2 * scroll_0.Region.Height];
                        var buffer_1 = new byte[scroll_1.Region.Width * 2 * scroll_1.Region.Height];

                        // Enumerate every frame
                        for (int frameIndex = 0; frameIndex < framesCount; frameIndex++)
                        {
                            Array.Copy(prevFrame, (scroll_0.Region.YPos - region.YPos) * region.Width * 2, buffer_0, 0, buffer_0.Length);
                            Array.Copy(prevFrame, (scroll_1.Region.YPos - region.YPos) * region.Width * 2, buffer_1, 0, buffer_1.Length);

                            Array.Copy(buffer_0, 0, prevFrame, (scroll_0.DestinationY - region.YPos) * region.Width * 2, buffer_0.Length);
                            Array.Copy(buffer_1, 0, prevFrame, (scroll_1.DestinationY - region.YPos) * region.Width * 2, buffer_1.Length);

                            frames[frameIndex] = prevFrame.ToArray();
                        }

                        ObjLoader.TextureAnimations.Add(new PS1VRAMAnimation(region, frames, scroll_0.AnimSpeed, false));
                    }
                    break;

                case GlobalGameObjectType.RGBAnimation:
                    // TODO: Implement
                    Debug.LogWarning($"Not implemented RGB animation");
                    break;

                case GlobalGameObjectType.TextureAnimation:
                    ObjLoader.TextureAnimations.Add(new PS1VRAMAnimation(Obj.TextureAnimation.Files.Select(x => x.Obj).ToArray(), Obj.TextureAnimationInfo.AnimSpeed, Obj.TextureAnimationInfo.PingPong));
                    break;

                case GlobalGameObjectType.PaletteAnimation:
                case GlobalGameObjectType.PaletteAnimations:
                case GlobalGameObjectType.ObjectWithPaletteAnimation:
                case GlobalGameObjectType.NahatombPaletteAnimation:
                    var anim = Obj.GlobalGameObjectType == GlobalGameObjectType.PaletteAnimation ||
                               Obj.GlobalGameObjectType == GlobalGameObjectType.ObjectWithPaletteAnimation ||
                               Obj.GlobalGameObjectType == GlobalGameObjectType.NahatombPaletteAnimation
                        ? Obj.PaletteAnimation.YieldToArray()
                        : Obj.PaletteAnimations.Files;

                    // Correct the alpha in the colors. Sometimes it's incorrect. Seems the game for model graphics only uses
                    // transparency if the color index is 0?
                    foreach (var p in anim.SelectMany(x => x.Files).Select(x => x.Colors))
                    {
                        for (int c = 2 + 1; c < p.Length; c += 2)
                            p[c] |= 0x80;
                    }

                    for (int i = 0; i < Obj.PaletteAnimationVRAMRegions.Length; i++)
                    {
                        var region = Obj.PaletteAnimationVRAMRegions[i];
                        var palettes = anim[i % anim.Length].Files;
                        var colors = palettes.Select(x => x.Colors).ToArray();

                        ObjLoader.PaletteAnimations.Add(new PS1VRAMAnimation(region, colors, Obj.PaletteAnimationInfo.AnimSpeed, true));
                    }
                    break;
            }
        }

        public override void LoadObject()
        {
            // Return if the object has no models
            if (Obj.Models?.Any(x => x.TMD != null) != true)
                return;

            if (Obj.GlobalGameObjectType == GlobalGameObjectType.MultiWheel)
            {
                PS1_TMD tmd = Obj.Models[0].TMD;
                ModelAnimation_ArchiveFile transform = Obj.AbsoluteTransform;

                Obj.Models = new GameObjectData_Model[transform.Positions.ObjectsCount];
                Obj.AbsoluteTransform = null;

                for (int i = 0; i < Obj.Models.Length; i++)
                {
                    Obj.Models[i] = new GameObjectData_Model()
                    {
                        TMD = tmd,
                        LocalTransform = new ModelAnimation_ArchiveFile
                        {
                            Rotations = new VectorAnimation_File
                            {
                                ObjectsCount = 1,
                                FramesCount = transform.Rotations.FramesCount,
                                Vectors = transform.Rotations.Vectors.Select(x => new KlonoaVector16[]
                                {
                                    x[i]
                                }).ToArray()
                            },
                            Positions = new VectorAnimation_File
                            {
                                ObjectsCount = 1,
                                FramesCount = transform.Positions.FramesCount,
                                Vectors = transform.Positions.Vectors.Select(x => new KlonoaVector16[]
                                {
                                    x[i]
                                }).ToArray()
                            }
                        }
                    };
                }
            }
            else if (Obj.GlobalGameObjectType == GlobalGameObjectType.FallingTargetPlatform)
            {
                _correctedTransforms ??= new HashSet<ModelAnimation_ArchiveFile>();

                foreach (var rot in Obj.Models[0].LocalTransforms.Files.Where(x => !_correctedTransforms.Contains(x))
                    .SelectMany(x => x.Rotations.Vectors).SelectMany(x => x))
                    rot.X += 0x400;

                foreach (var f in Obj.Models[0].LocalTransforms.Files)
                    _correctedTransforms.Add(f);
            }
            else if (Obj.GlobalGameObjectType == GlobalGameObjectType.Boss_GelgBolm || 
                     Obj.GlobalGameObjectType == GlobalGameObjectType.Boss_Joka)
            {
                // Combine the models into one so we can more easily animate it
                var combinedTmd = new PS1_TMD
                {
                    ObjectsCount = (uint)Obj.Models.Length,
                    Objects = Obj.Models.Select(x => x.TMD.Objects[0]).ToArray(),
                };

                Obj.Models = Obj.Models.Take(1).ToArray();
                Obj.Models[0].TMD = combinedTmd;
            }

            string name = Obj.DefinitionOffset != null
                ? $"Object3D Offset:{Obj.DefinitionOffset} Type:{Obj.PrimaryType}-{Obj.SecondaryType} ({Obj.GlobalGameObjectType})"
                : $"Object3D Type:{Obj.PrimaryType}-{Obj.SecondaryType} ({Obj.GlobalGameObjectType})";

            // Create the game object
            GameObject = new GameObject(name);

            // Create a child object for each model the object contains
            for (var modelIndex = 0; modelIndex < Obj.Models.Length; modelIndex++)
            {
                // Get the model
                GameObjectData_Model model = Obj.Models[modelIndex];
                
                var tmdGameObj = new KlonoaTMDGameObject(
                    tmd: model.TMD,
                    vram: VRAM,
                    scale: Scale,
                    objectsLoader: ObjLoader,
                    isPrimaryObj: false,
                    animations: model.LocalTransform?.YieldToArray() ?? model.LocalTransforms?.Files,
                    animSpeed: new AnimSpeed_FrameIncrease(model.AnimatedLocalTransformSpeed),
                    animLoopMode: model.DoesAnimatedLocalTransformPingPong ? AnimLoopMode.PingPong : AnimLoopMode.Repeat,
                    boneAnimations: model.ModelBoneAnimations,
                    vertexAnimation: model.VertexAnimation,
                    isJoka: Obj.GlobalGameObjectType == GlobalGameObjectType.Boss_Joka);

                // Get the game object
                GameObject modelGameObj = tmdGameObj.CreateGameObject($"Model {modelIndex}", PSKlonoa_DTP_BaseManager.IncludeDebugInfo);

                // Apply a position if available
                if (model.Position != null)
                    modelGameObj.transform.localPosition = model.Position.GetPositionVector(Scale);

                // Apply a rotation if available
                if (model.Rotation != null)
                    modelGameObj.transform.localRotation = model.Rotation.GetQuaternion();

                GameObjectData_ConstantRotation rot = model.ConstantRotation;

                // Apply a constant rotation if available
                if (rot != null)
                {
                    AddConstantRot(modelGameObj, KlonoaDTPConstantRotationComponent.RotationAxis.X, rot.RotX, rot.Min, rot.Length);
                    AddConstantRot(modelGameObj, KlonoaDTPConstantRotationComponent.RotationAxis.Y, rot.RotY, rot.Min, rot.Length);
                    AddConstantRot(modelGameObj, KlonoaDTPConstantRotationComponent.RotationAxis.Z, rot.RotZ, rot.Min, rot.Length);
                }

                bool isAnimated = tmdGameObj.HasAnimations;

                if (isAnimated)
                    IsAnimated = true;

                modelGameObj.transform.SetParent(GameObject.transform);
            }

            // Get the absolute transforms
            ModelAnimation_ArchiveFile[] absoluteTransforms = Obj.AbsoluteTransform?.YieldToArray() ?? Obj.AbsoluteTransforms?.Files;

            // Apply the absolute transform
            bool isObjAnimated = KlonoaHelpers.ApplyTransform(
                gameObj: GameObject,
                transforms: absoluteTransforms,
                scale: Scale,
                objIndex: 0,
                animSpeed: new AnimSpeed_FrameIncrease(Obj.AnimatedAbsoluteTransformSpeed),
                animLoopMode: Obj.DoesAnimatedAbsoluteTransformPingPong ? AnimLoopMode.PingPong : AnimLoopMode.Repeat);

            if (isObjAnimated)
                IsAnimated = true;

            // Apply an absolute position if available
            if (Obj.Position != null)
                GameObject.transform.position = Obj.Position.GetPositionVector(Scale);
        }

        public void AddConstantRot(GameObject obj, KlonoaDTPConstantRotationComponent.RotationAxis axis, float? speed, float min, float length)
        {
            if (speed == null)
                return;

            var rotComponent = obj.AddComponent<KlonoaDTPConstantRotationComponent>();
            rotComponent.animatedTransform = obj.transform;
            rotComponent.initialRotation = obj.transform.localRotation;
            rotComponent.axis = axis;
            rotComponent.rotationSpeed = speed.Value;
            rotComponent.minValue = min;
            rotComponent.length = length;
        }
    }
}