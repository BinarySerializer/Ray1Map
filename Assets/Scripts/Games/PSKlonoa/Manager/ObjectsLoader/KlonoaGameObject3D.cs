using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Klonoa;
using BinarySerializer.Klonoa.DTP;
using BinarySerializer.PS1;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ray1Map.PSKlonoa
{
    public class KlonoaGameObject3D : KlonoaObject
    {
        public KlonoaGameObject3D(KlonoaObjectsLoader objLoader, GameObject3D obj) : base(objLoader)
        {
            Obj = obj;

            // Log unknown objects
            if (Obj.GlobalGameObjectType == GlobalGameObjectType.Unknown)
                Debug.LogWarning($"Unknown object type at {Obj.Offset} of type " +
                                 $"{(int)Obj.PrimaryType}-{Obj.SecondaryType} with data:{Environment.NewLine}" +
                                 $"{String.Join($"{Environment.NewLine}{Environment.NewLine}", Obj.Data_Unknown?.Select(dataFile => $"Length: {dataFile.Data.Length} bytes{Environment.NewLine}{dataFile.Data.ToHexString(align: 16, maxLines: 16)}") ?? new string[0])}");
        }

        private static HashSet<ModelAnimation_ArchiveFile> _correctedTransforms; // TODO: Find better solution

        public GameObject3D Obj { get; }

        public override void LoadAnimations()
        {
            if (Obj.Data_RawTextureAnimation != null)
                ObjLoader.TextureAnimations.Add(new PS1VRAMAnimation(
                    region: Obj.Data_RawTextureAnimation.Region, 
                    frames: Obj.Data_RawTextureAnimation.Frames, 
                    speed: Obj.TextureAnimationInfo.AnimSpeed, 
                    pingPong: Obj.TextureAnimationInfo.PingPong));

            if (Obj.Data_CameraAnimations != null)
                ObjLoader.CameraAnimations.Add(Obj.Data_CameraAnimations);

            switch (Obj.GlobalGameObjectType)
            {
                case GlobalGameObjectType.ScrollAnimation:
                    ObjLoader.ScrollAnimations.Add(Obj.Data_UVScrollAnimation);
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
                    ObjLoader.TextureAnimations.Add(new PS1VRAMAnimation(Obj.Data_TextureAnimation.Files.Select(x => x.Obj).ToArray(), Obj.TextureAnimationInfo.AnimSpeed, Obj.TextureAnimationInfo.PingPong));
                    break;

                case GlobalGameObjectType.PaletteAnimation:
                case GlobalGameObjectType.PaletteAnimations:
                case GlobalGameObjectType.ObjectWithPaletteAnimation:
                case GlobalGameObjectType.NahatombPaletteAnimation:
                    var anim = Obj.GlobalGameObjectType == GlobalGameObjectType.PaletteAnimation ||
                               Obj.GlobalGameObjectType == GlobalGameObjectType.ObjectWithPaletteAnimation ||
                               Obj.GlobalGameObjectType == GlobalGameObjectType.NahatombPaletteAnimation
                        ? Obj.Data_PaletteAnimation.YieldToArray()
                        : Obj.Data_PaletteAnimations.Files;

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
            // Return if the object has no model
            if (Obj.Data_TMD == null)
                return;

            if (Obj.GlobalGameObjectType == GlobalGameObjectType.GeyserPlatform)
            {
                var positions = Obj.GeyserPlatformPositions;

                for (int i = 0; i < positions.Length; i++)
                {
                    var geyserObj = GameObj_LoadTMD(Obj, Obj.Data_TMD, ObjLoader, VRAM, Scale, index: i);
                    GameObj_ApplyPosition(geyserObj, positions[i].Position, Scale);
                }

                return;
            }
            else if (Obj.GlobalGameObjectType == GlobalGameObjectType.Cutscene_7_1_CageFence)
            {
                VectorAnimation_File positions = Obj.Data_Positions;
                VectorAnimation_File rotations = Obj.Data_Rotations;

                for (int i = 0; i < positions.ObjectsCount; i++)
                {
                    var fenceObj = GameObj_LoadTMD(Obj, Obj.Data_TMD, ObjLoader, VRAM, Scale, index: i);
                    GameObj_ApplyPosition(fenceObj, positions.Vectors[0][i], Scale);
                    GameObj_ApplyRotation(fenceObj, rotations.Vectors[0][i]);
                }

                return;
            }

            bool isMultiple = false;
            Vector3Int objPosOffset = Vector3Int.zero;

            // TODO: Hard-code this in the object
            switch (Obj.GlobalGameObjectType)
            {
                case GlobalGameObjectType.WindSwirl:
                    objPosOffset = new Vector3Int(0, 182, 0);
                    break;

                case GlobalGameObjectType.MultiWheel:
                    isMultiple = true;
                    break;

                case GlobalGameObjectType.FallingTargetPlatform:
                    _correctedTransforms ??= new HashSet<ModelAnimation_ArchiveFile>();

                    foreach (var rot in Obj.Data_LocalTransforms.Files.Where(x => !_correctedTransforms.Contains(x)).SelectMany(x => x.Rotations.Vectors).SelectMany(x => x))
                        rot.X += 0x400;

                    foreach (var f in Obj.Data_LocalTransforms.Files)
                        _correctedTransforms.Add(f);
                    break;
            }

            // Load the object model from the TMD data
            var obj = GameObj_LoadTMD(
                obj3D: Obj,
                tmd: Obj.Data_TMD,
                objLoader: ObjLoader,
                vram: VRAM,
                scale: Scale,
                localTransforms: Obj.Data_LocalTransform?.YieldToArray() ?? Obj.Data_LocalTransforms?.Files,
                absoluteTransforms: Obj.Data_AbsoluteTransform?.YieldToArray() ?? Obj.Data_AbsoluteTransforms?.Files,
                multiple: isMultiple,
                modelAnims: Obj.Data_ModelAnimations,
                vertexAnimation: Obj.Data_VertexAnimation);

            // Apply a position if available
            if (Obj.Data_Position != null)
                GameObj_ApplyPosition(obj, Obj.Data_Position, Scale, objPosOffset);

            // Apply a constant rotation if available
            addConstantRot(KlonoaDTPConstantRotationComponent.RotationAxis.X, Obj.ConstantRotationX);
            addConstantRot(KlonoaDTPConstantRotationComponent.RotationAxis.Y, Obj.ConstantRotationY);
            addConstantRot(KlonoaDTPConstantRotationComponent.RotationAxis.Z, Obj.ConstantRotationZ);

            void addConstantRot(KlonoaDTPConstantRotationComponent.RotationAxis axis, float? speed)
            {
                if (speed == null)
                    return;

                var rotComponent = obj.AddComponent<KlonoaDTPConstantRotationComponent>();
                rotComponent.animatedTransform = obj.transform;
                rotComponent.initialRotation = obj.transform.localRotation;
                rotComponent.axis = axis;
                rotComponent.rotationSpeed = speed.Value;
                rotComponent.minValue = Obj.ConstantRotationMin;
                rotComponent.length = Obj.ConstantRotationLength;
            }

            // Load secondary object if available
            if (Obj.Data_TMD_Secondary != null)
            {
                var secondaryObj = GameObj_LoadTMD(
                    obj3D: Obj,
                    tmd: Obj.Data_TMD_Secondary,
                    objLoader: ObjLoader,
                    vram: VRAM,
                    scale: Scale,
                    index: 1);

                // Apply a position if available (without the offset)
                if (Obj.Data_Position != null)
                    GameObj_ApplyPosition(secondaryObj, Obj.Data_Position, Scale);
            }
        }

        public GameObject GameObj_LoadTMD(
            GameObject3D obj3D,
            PS1_TMD tmd,
            KlonoaObjectsLoader objLoader,
            PS1_VRAM vram,
            float scale,
            ModelAnimation_ArchiveFile[] localTransforms = null,
            ModelAnimation_ArchiveFile[] absoluteTransforms = null,
            int index = 0,
            bool multiple = false,
            ArchiveFile<ModelBoneAnimation_ArchiveFile> modelAnims = null,
            GameObject3D.ModelVertexAnimation vertexAnimation = null)
        {
            if (tmd == null)
                throw new ArgumentNullException(nameof(tmd));

            var tmdGameObj = new KlonoaTMDGameObject(
                tmd: tmd,
                vram: vram,
                scale: scale,
                objectsLoader: objLoader,
                isPrimaryObj: false,
                animations: localTransforms,
                animSpeed: new AnimSpeed_FrameIncrease(obj3D.AnimatedLocalTransformSpeed),
                animLoopMode: obj3D.DoesAnimatedLocalTransformPingPong ? AnimLoopMode.PingPong : AnimLoopMode.Repeat,
                boneAnimations: modelAnims,
                vertexAnimation: vertexAnimation);

            GameObject gameObj = tmdGameObj.CreateGameObject($"Object3D Offset:{obj3D.Offset} Index:{index} Type:{obj3D.PrimaryType}-{obj3D.SecondaryType} ({obj3D.GlobalGameObjectType})", PSKlonoa_DTP_BaseManager.IncludeDebugInfo);
            bool isAnimated = tmdGameObj.HasAnimations;

            if (isAnimated)
                IsAnimated = true;

            int count = multiple ? absoluteTransforms[0].Positions.ObjectsCount : 1;

            for (int i = 0; i < count; i++)
            {
                var obj = i == 0 ? gameObj : Object.Instantiate(gameObj);

                // Apply the absolute transform
                isAnimated = KlonoaHelpers.ApplyTransform(
                    gameObj: gameObj,
                    transforms: absoluteTransforms,
                    scale: scale,
                    objIndex: i,
                    animSpeed: new AnimSpeed_FrameIncrease(obj3D.AnimatedAbsoluteTransformSpeed),
                    animLoopMode: obj3D.DoesAnimatedAbsoluteTransformPingPong ? AnimLoopMode.PingPong : AnimLoopMode.Repeat);

                if (isAnimated)
                    IsAnimated = true;

                GameObjects.Add(obj);
            }

            return gameObj;
        }

        public void GameObj_ApplyPosition(GameObject obj, KlonoaVector16 pos, float scale, Vector3? posOffset = null)
        {
            obj.transform.position = pos.GetPositionVector(posOffset, scale);
        }
        public void GameObj_ApplyRotation(GameObject obj, KlonoaVector16 rot)
        {
            obj.transform.rotation = rot.GetQuaternion();
        }
    }
}