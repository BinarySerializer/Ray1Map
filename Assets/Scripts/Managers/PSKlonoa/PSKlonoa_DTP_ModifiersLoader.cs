using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Klonoa;
using BinarySerializer.Klonoa.DTP;
using BinarySerializer.PS1;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    public class PSKlonoa_DTP_ModifiersLoader
    {
        public PSKlonoa_DTP_ModifiersLoader(PSKlonoa_DTP_BaseManager manager, Loader_DTP loader, float scale, GameObject parentObject, ModifierObject[] modifiers, BackgroundModifierObject[] backgroundModifiers)
        {
            Manager = manager;
            Loader = loader;
            Scale = scale;
            ParentObject = parentObject;
            Modifiers = modifiers;
            BackgroundModifiers = backgroundModifiers;

            GameObj_Objects = new List<GameObject>();
            BG_Layers = new List<BackgroundLayer>();
            BG_Clears = new List<BackgroundModifierData_Clear>();
            Anim_Manager = new PS1VRAMAnimationManager();
            Anim_TextureAnimations = new List<PS1VRAMAnimation>();
            Anim_PaletteAnimations = new List<PS1VRAMAnimation>();
            Anim_BGPaletteAnimations = new List<PS1VRAMAnimation>();
            Anim_ScrollAnimations = new List<UVScrollAnimation_File>();
        }

        public PSKlonoa_DTP_BaseManager Manager { get; }
        public Loader_DTP Loader { get; }
        public float Scale { get; }
        public GameObject ParentObject { get; }
        public ModifierObject[] Modifiers { get; }
        public BackgroundModifierObject[] BackgroundModifiers { get; }

        // Game objects
        public bool GameObj_IsAnimated { get; set; }
        public List<GameObject> GameObj_Objects { get; }
        
        // Backgrounds
        public bool HasHUD { get; set; }
        public List<BackgroundLayer> BG_Layers { get; }
        public List<BackgroundModifierData_Clear> BG_Clears { get; }
        public BackgroundModifierData_SetLightState BG_LightState { get; set; }

        // Animations
        public PS1VRAMAnimationManager Anim_Manager { get; }
        public List<PS1VRAMAnimation> Anim_TextureAnimations { get; }
        public List<PS1VRAMAnimation> Anim_PaletteAnimations { get; }
        public List<PS1VRAMAnimation> Anim_BGPaletteAnimations { get; }
        public List<UVScrollAnimation_File> Anim_ScrollAnimations { get; }

        public async UniTask LoadAsync()
        {
            // Loop twice, ensuring all texture animations are loaded before the objects
            for (int i = 0; i < 2; i++)
            {
                for (var modifierIndex = 0; modifierIndex < Modifiers.Length; modifierIndex++)
                {
                    var modifier = Modifiers[modifierIndex];

                    Controller.DetailedState = $"Loading modifier {modifierIndex + 1}/{Modifiers.Length} (loop {i + 1}/2)";
                    await Controller.WaitIfNecessary();

                    GameObj_LoadModifier(modifier, (LoadLoop)i);
                }

                for (var modifierIndex = 0; modifierIndex < BackgroundModifiers.Length; modifierIndex++)
                {
                    var modifier = BackgroundModifiers[modifierIndex];

                    Controller.DetailedState = $"Loading background modifier {modifierIndex + 1}/{BackgroundModifiers.Length} (loop {i + 1}/2)";
                    await Controller.WaitIfNecessary();

                    BG_LoadModifier(modifier, (LoadLoop)i);
                }
            }
        }

        public void GameObj_LoadModifier(ModifierObject modifier, LoadLoop loop)
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
                        if (loop != LoadLoop.Animations)
                            return;

                        Debug.LogWarning($"Unknown modifier type at {modifier.Offset} of type " +
                                         $"{(int)modifier.PrimaryType}-{modifier.SecondaryType} with data:{Environment.NewLine}" +
                                         $"{String.Join(Environment.NewLine, modifier.DataFiles?.Select(x => x.Unknown?.ToHexString(align: 16, maxLines: 16)) ?? new string[0])}");
                        return;
                    }

                case GlobalModifierType.WindSwirl:
                    {
                        if (loop != LoadLoop.Objects)
                            return;

                        var obj_0 = GameObj_LoadTMD(modifier, modifier.DataFiles[0].TMD);
                        var obj_1 = GameObj_LoadTMD(modifier, modifier.DataFiles[1].TMD, index: 1);

                        var pos = modifier.DataFiles[2].Position;

                        GameObj_ApplyPosition(obj_0, pos, new Vector3Int(0, 182, 0));
                        GameObj_ApplyPosition(obj_1, pos);
                        GameObj_ApplyConstantRotation(obj_0, modifier.RotationAttribute);
                    } 
                    break;

                case GlobalModifierType.BigWindmill:
                case GlobalModifierType.SmallWindmill:
                    {
                        if (loop != LoadLoop.Objects)
                            return;

                        var obj = GameObj_LoadTMD(modifier, modifier.DataFiles[0].TMD, absoluteTransform: modifier.DataFiles[1].Transform);
                        GameObj_ApplyConstantRotation(obj, modifier.RotationAttribute);
                    }
                    break;

                case GlobalModifierType.MovingPlatform:
                    {
                        if (loop != LoadLoop.Objects)
                            return;

                        GameObj_LoadTMD(modifier, modifier.DataFiles[0].TMD, absoluteTransform: modifier.DataFiles[3].Transform);
                    }
                    break;

                case GlobalModifierType.RoadSign:
                    {
                        if (loop != LoadLoop.Objects)
                            return;

                        GameObj_LoadTMD(modifier, modifier.DataFiles[0].TMD, absoluteTransform: modifier.DataFiles[1].Transform);
                    } 
                    break;

                case GlobalModifierType.TiltRock:
                    {
                        if (loop != LoadLoop.Objects)
                            return;

                        GameObj_LoadTMD(modifier, modifier.DataFiles[0].TMD, absoluteTransform: modifier.DataFiles[3].Transform);
                    } 
                    break;

                case GlobalModifierType.Minecart:
                    {
                        if (loop != LoadLoop.Objects)
                            return;

                        GameObj_LoadTMD(modifier, modifier.DataFiles[0].TMD, absoluteTransforms: modifier.DataFiles[3].Transforms.Files, localTransform: modifier.DataFiles[5].Transform);
                    } 
                    break;

                case GlobalModifierType.ScrollAnimation:
                    {
                        if (loop != LoadLoop.Animations)
                            return;

                        Anim_ScrollAnimations.Add(modifier.DataFiles[0].UVScrollAnimation);
                    } 
                    break;

                case GlobalModifierType.Object:
                    {
                        if (loop != LoadLoop.Objects)
                            return;

                        GameObj_LoadTMD(modifier, modifier.DataFiles[0].TMD);
                    }
                    break;

                case GlobalModifierType.LevelModelSection:
                    {
                        if (loop != LoadLoop.Objects)
                            return;

                        GameObj_LoadTMD(modifier, modifier.DataFiles[0].TMD);
                    } 
                    break;

                case GlobalModifierType.ScenerySprites:
                    {
                        if (loop != LoadLoop.Objects)
                            return;

                        // TODO: Get correct sprites to show
                    }
                    break;

                case GlobalModifierType.TextureAnimation:
                    {
                        if (loop != LoadLoop.Animations)
                            return;

                        Anim_TextureAnimations.Add(new PS1VRAMAnimation(modifier.DataFiles[0].TextureAnimation.Files, Loader.Config.TextureAnimationSpeeds[Loader.BINBlock], true));
                    } 
                    break;

                case GlobalModifierType.PaletteAnimation:
                    {
                        if (loop != LoadLoop.Animations)
                            return;

                        var anim = modifier.DataFiles[0].PaletteAnimation;

                        for (int i = 0; i < anim.Files.Length; i++)
                        {
                            var region = anim.VRAMRegions[i];
                            var palettes = anim.Files[i].Files;
                            var colors = palettes.Select(x => x.Colors).ToArray();
                            Anim_PaletteAnimations.Add(new PS1VRAMAnimation(region, colors, anim.AnimationInfo.AnimSpeed, true));
                        }
                    }
                    break;

                case GlobalModifierType.Special:
                default:
                    // Do nothing
                    break;
            }
        }

        public GameObject GameObj_LoadTMD(ModifierObject modifier, PS1_TMD tmd, ObjTransform_ArchiveFile localTransform = null, ObjTransform_ArchiveFile absoluteTransform = null, int index = 0)
        {
            return GameObj_LoadTMD(modifier, tmd, localTransform, absoluteTransform == null ? null : new ObjTransform_ArchiveFile[]
            {
                absoluteTransform
            }, index);
        }

        public GameObject GameObj_LoadTMD(ModifierObject modifier, PS1_TMD tmd, ObjTransform_ArchiveFile localTransform, ObjTransform_ArchiveFile[] absoluteTransforms, int index = 0)
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
                modifiersLoader: this,
                isPrimaryObj: false,
                positions: localTransform?.Positions.Positions[0].Select(x => Manager.GetPositionVector(x, Vector3.zero, Scale)).ToArray(),
                rotations: localTransform?.Rotations.Rotations[0].Select(x => Manager.GetQuaternion(x)).ToArray());

            if (isAnimated)
                GameObj_IsAnimated = true;

            if (absoluteTransforms != null && absoluteTransforms.Any())
            {
                gameObj.transform.localPosition = Manager.GetPositionVector(absoluteTransforms[0].Positions.Positions[0][0], null, Scale);
                gameObj.transform.localRotation = Manager.GetQuaternion(absoluteTransforms[0].Rotations.Rotations[0][0]);
            }
            else
            {
                gameObj.transform.localPosition = Vector3.zero;
                gameObj.transform.localRotation = Quaternion.identity;
            }

            gameObj.transform.SetParent(ParentObject.transform);

            if (absoluteTransforms?.FirstOrDefault()?.Positions.Positions.Length > 1)
            {
                var mtComponent = gameObj.AddComponent<AnimatedTransformComponent>();
                mtComponent.animatedTransform = gameObj.transform;

                var positions = absoluteTransforms.SelectMany(x => x.Positions.Positions).Select(x => Manager.GetPositionVector(x[0], null, Scale)).ToArray();
                var rotations = absoluteTransforms.SelectMany(x => x.Rotations.Rotations).Select(x => Manager.GetQuaternion(x[0])).ToArray();

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

            GameObj_Objects.Add(gameObj);
            return gameObj;
        }

        public void GameObj_ApplyPosition(GameObject obj, ObjPosition pos, Vector3? posOffset = null)
        {
            obj.transform.position = Manager.GetPositionVector(pos, posOffset, Scale);
        }

        public void GameObj_ApplyConstantRotation(GameObject obj, ModifierRotationAttribute rot)
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

        public void BG_LoadModifier(BackgroundModifierObject modifier, LoadLoop loop)
        {
            switch (modifier.Type)
            {
                case BackgroundModifierObject.BackgroundModifierType.HUD:
                    {
                        if (loop != LoadLoop.Objects)
                            return;

                        HasHUD = true;
                    }
                    break;

                case BackgroundModifierObject.BackgroundModifierType.BackgroundLayer_19:
                case BackgroundModifierObject.BackgroundModifierType.BackgroundLayer_22:
                    {
                        if (loop != LoadLoop.Objects)
                            return;

                        var (frames, speed) = Manager.GetBackgroundFrames(Loader, this, Loader.BackgroundPack, modifier);

                        BG_Layers.Add(new BackgroundLayer(modifier, frames, speed));
                    }
                    break;

                case BackgroundModifierObject.BackgroundModifierType.Clear_Gradient:
                case BackgroundModifierObject.BackgroundModifierType.Clear:
                    {
                        if (loop != LoadLoop.Objects)
                            return;

                        BG_Clears.Add(modifier.Data_Clear);
                    }
                    break;

                case BackgroundModifierObject.BackgroundModifierType.PaletteScroll:
                    {
                        if (loop != LoadLoop.Animations)
                            return;

                        BackgroundModifierData_PaletteScroll scroll = modifier.Data_PaletteScroll;
                        PS1_VRAM vram = Loader.VRAM;

                        var frames = new byte[scroll.Length][];
                        var pal = vram.GetPixels8(0, 0, scroll.XPosition * 2, scroll.YPosition, 32);

                        frames[0] = pal;

                        for (int i = 1; i < frames.Length; i++)
                        {
                            // Clone the array to avoid modifying the previous frames
                            pal = (byte[])pal.Clone();

                            var firstColor_0 = pal[0];
                            var firstColor_1 = pal[1];

                            var index = scroll.StartIndex;
                            var endIndex = index + scroll.Length;

                            do
                            {
                                pal[index * 2] = pal[(index + 1) * 2];
                                pal[index * 2 + 1] = pal[(index + 1) * 2 + 1];

                                index += 1;
                            } while (index < endIndex);

                            pal[(endIndex - 1) * 2] = firstColor_0;
                            pal[(endIndex - 1) * 2 + 1] = firstColor_1;

                            frames[i] = pal;
                        }

                        var region = new RectInt(scroll.XPosition * 2, scroll.YPosition, 32, 1);
                        Anim_BGPaletteAnimations.Add(new PS1VRAMAnimation(region, frames, scroll.Speed, false));
                    }
                    break;

                case BackgroundModifierObject.BackgroundModifierType.SetLightState:
                    {
                        if (loop != LoadLoop.Animations)
                            return;

                        BG_LightState = modifier.Data_SetLightState;
                    }
                    break;

                case BackgroundModifierObject.BackgroundModifierType.Unknown_1:
                case BackgroundModifierObject.BackgroundModifierType.Reset:
                default:
                    // Do nothing
                    break;
            }
        }

        public IEnumerable<PS1VRAMAnimation> Anim_GetAnimationsFromRegion(RectInt textureRegion, RectInt palRegion)
        {
            return Anim_PaletteAnimations.Where(x => x.Overlaps(palRegion)).Concat(Anim_TextureAnimations.Where(x => x.Overlaps(textureRegion)));
        }

        public IEnumerable<PS1VRAMAnimation> Anim_GetBGAnimationsFromRegion(RectInt palRegion)
        {
            return Anim_BGPaletteAnimations.Where(x => x.Overlaps(palRegion));
        }

        public enum LoadLoop
        {
            Animations = 0,
            Objects = 1,
        }

        public class BackgroundLayer
        {
            public BackgroundLayer(BackgroundModifierObject modifierObject, Texture2D[] frames, int speed)
            {
                ModifierObject = modifierObject;
                Frames = frames;
                Speed = speed;
            }

            public BackgroundModifierObject ModifierObject { get; }
            public Texture2D[] Frames { get; }
            public int Speed { get; }
        }
    }
}