using BinarySerializer;
using BinarySerializer.Klonoa.DTP;
using BinarySerializer.PS1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace R1Engine
{
    public class PSKlonoa_DTP_ModifiersLoader
    {
        public PSKlonoa_DTP_ModifiersLoader(PSKlonoa_DTP_BaseManager manager, Loader loader, float scale, GameObject parentObject, ModifierObject[] modifiers, BackgroundModifierObject[] backgroundModifiers)
        {
            Manager = manager;
            Loader = loader;
            Scale = scale;
            ParentObject = parentObject;
            Modifiers = modifiers;
            BackgroundModifiers = backgroundModifiers;

            GameObj_Objects = new List<GameObject>();
            GameObj_CameraAnimations = new List<CameraAnimations_File>();
            BG_Layers = new List<BackgroundLayer>();
            BG_Clears = new List<BackgroundModifierData_Clear>();
            Anim_Manager = new PS1VRAMAnimationManager();
            Anim_TextureAnimations = new List<PS1VRAMAnimation>();
            Anim_PaletteAnimations = new List<PS1VRAMAnimation>();
            Anim_BGPaletteAnimations = new List<PS1VRAMAnimation>();
            Anim_ScrollAnimations = new List<UVScrollAnimation_File>();
        }

        public PSKlonoa_DTP_BaseManager Manager { get; }
        public Loader Loader { get; }
        public float Scale { get; }
        public GameObject ParentObject { get; }
        public ModifierObject[] Modifiers { get; }
        public BackgroundModifierObject[] BackgroundModifiers { get; }

        // Game objects
        public bool GameObj_IsAnimated { get; set; }
        public List<GameObject> GameObj_Objects { get; }
        public List<CameraAnimations_File> GameObj_CameraAnimations { get; }

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

        private HashSet<ObjTransform_ArchiveFile> _correctedTransforms;
        public void GameObj_LoadModifier(ModifierObject modifier, LoadLoop loop)
        {
            // Ignore invalid modifiers
            if (modifier.IsInvalid)
                return;

            // Log unknown modifiers
            if (modifier.GlobalModifierType == GlobalModifierType.Unknown)
            {
                if (loop != LoadLoop.Animations)
                    return;

                Debug.LogWarning($"Unknown modifier type at {modifier.Offset} of type " +
                                 $"{(int)modifier.PrimaryType}-{modifier.SecondaryType} with data:{Environment.NewLine}" +
                                 $"{String.Join($"{Environment.NewLine}{Environment.NewLine}", modifier.Data_Unknown?.Select(dataFile => $"Length: {dataFile.Data.Length} bytes{Environment.NewLine}{dataFile.Data.ToHexString(align: 16, maxLines: 16)}") ?? new string[0])}");
                return;
            }

            // Handle animations
            if (loop == LoadLoop.Animations)
            {
                switch (modifier.GlobalModifierType)
                {
                    case GlobalModifierType.ScrollAnimation:
                        Anim_ScrollAnimations.Add(modifier.Data_UVScrollAnimation);
                        break;

                    case GlobalModifierType.VRAMScrollAnimation:
                    case GlobalModifierType.VRAMScrollAnimationWithTexture:
                        KlonoaSettings_DTP.VRAMScrollInfo[] scroll = modifier.VRAMScrollInfos;
                        PS1_VRAM vram = Loader.VRAM;

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
                                    prevFrame[y * region.Width * 2 + x] = vram.GetPixel8(0, 0, region.XPos * 2 + x, region.YPos + y);
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

                            Anim_TextureAnimations.Add(new PS1VRAMAnimation(region, frames, scroll_0.AnimSpeed, false));
                        }
                        break;

                    case GlobalModifierType.RGBAnimation:
                        // TODO: Implement
                        break;

                    case GlobalModifierType.TextureAnimation:
                        Anim_TextureAnimations.Add(new PS1VRAMAnimation(modifier.Data_TextureAnimation.Files.Select(x => x.Obj).ToArray(), modifier.TextureAnimationInfo.AnimSpeed, modifier.TextureAnimationInfo.PingPong));
                        break;

                    case GlobalModifierType.PaletteAnimation:
                    case GlobalModifierType.PaletteAnimations:
                    case GlobalModifierType.ObjectWithPaletteAnimation:
                    case GlobalModifierType.NahatombPaletteAnimation:
                        var anim = modifier.GlobalModifierType == GlobalModifierType.PaletteAnimation || 
                                   modifier.GlobalModifierType == GlobalModifierType.ObjectWithPaletteAnimation ||
                                   modifier.GlobalModifierType == GlobalModifierType.NahatombPaletteAnimation
                            ? modifier.Data_PaletteAnimation.YieldToArray() 
                            : modifier.Data_PaletteAnimations.Files;

                        // Correct the alpha in the colors. Sometimes it's incorrect. Seems the game for model graphics only uses
                        // transparency if the color index is 0?
                        foreach (var p in anim.SelectMany(x => x.Files).Select(x => x.Colors))
                        {
                            for (int c = 2 + 1; c < p.Length; c += 2)
                                p[c] |= 0x80;
                        }

                        for (int i = 0; i < modifier.PaletteAnimationVRAMRegions.Length; i++)
                        {
                            var region = modifier.PaletteAnimationVRAMRegions[i];
                            var palettes = anim[i % anim.Length].Files;
                            var colors = palettes.Select(x => x.Colors).ToArray();
                            Anim_PaletteAnimations.Add(new PS1VRAMAnimation(region, colors, modifier.PaletteAnimationInfo.AnimSpeed, true));
                        }
                        break;
                }
            }
            // Handle objects
            else
            {
                if (modifier.Data_TMD == null)
                    return;

                if (modifier.GlobalModifierType == GlobalModifierType.GeyserPlatform)
                {
                    var positions = modifier.GeyserPlatformPositions;

                    for (int i = 0; i < positions.Length; i++)
                    {
                        var geyserObj = GameObj_LoadTMD(modifier, modifier.Data_TMD, index: i);
                        GameObj_ApplyPosition(geyserObj, positions[i].Position);
                    }

                    return;
                }

                bool isMultiple = false;
                Vector3Int objPosOffset = Vector3Int.zero;

                // TODO: Hard-code this in the modifier object
                switch (modifier.GlobalModifierType)
                {
                    case GlobalModifierType.WindSwirl:
                        objPosOffset = new Vector3Int(0, 182, 0);
                        break;

                    case GlobalModifierType.MultiWheel:
                        isMultiple = true;
                        break;

                    case GlobalModifierType.FallingTargetPlatform:
                        _correctedTransforms ??= new HashSet<ObjTransform_ArchiveFile>();

                        foreach (var rot in modifier.Data_LocalTransforms.Files.Where(x => !_correctedTransforms.Contains(x)).SelectMany(x => x.Rotations.Rotations).SelectMany(x => x))
                            rot.RotationX += 0x400;
                        
                        foreach (var f in modifier.Data_LocalTransforms.Files)
                            _correctedTransforms.Add(f);
                        break;
                }

                // Load the object model from the TMD data
                var obj = GameObj_LoadTMD(
                    modifier: modifier, 
                    tmd: modifier.Data_TMD, 
                    localTransforms: modifier.Data_LocalTransform?.YieldToArray() ?? modifier.Data_LocalTransforms?.Files, 
                    absoluteTransforms: modifier.Data_AbsoluteTransform?.YieldToArray() ?? modifier.Data_AbsoluteTransforms?.Files, 
                    multiple: isMultiple);

                // Apply a position if available
                if (modifier.Data_Position != null)
                    GameObj_ApplyPosition(obj, modifier.Data_Position, objPosOffset);

                // Apply a constant rotation if available
                addConstantRot(KlonoaDTPConstantRotationComponent.RotationAxis.X, modifier.ConstantRotationX);
                addConstantRot(KlonoaDTPConstantRotationComponent.RotationAxis.Y, modifier.ConstantRotationY);
                addConstantRot(KlonoaDTPConstantRotationComponent.RotationAxis.Z, modifier.ConstantRotationZ);

                void addConstantRot(KlonoaDTPConstantRotationComponent.RotationAxis axis, float? speed)
                {
                    if (speed == null)
                        return;

                    var rotComponent = obj.AddComponent<KlonoaDTPConstantRotationComponent>();
                    rotComponent.animatedTransform = obj.transform;
                    rotComponent.initialRotation = obj.transform.localRotation;
                    rotComponent.axis = axis;
                    rotComponent.rotationSpeed = speed.Value;
                    rotComponent.minValue = modifier.ConstantRotationMin;
                    rotComponent.length = modifier.ConstantRotationLength;
                }

                // Load secondary object if available
                if (modifier.Data_TMD_Secondary != null)
                {
                    var secondaryObj = GameObj_LoadTMD(
                        modifier: modifier,
                        tmd: modifier.Data_TMD_Secondary,
                        index: 1);

                    // Apply a position if available (without the offset)
                    if (modifier.Data_Position != null)
                        GameObj_ApplyPosition(secondaryObj, modifier.Data_Position);
                }

                if (modifier.Data_CameraAnimations != null)
                    GameObj_CameraAnimations.Add(modifier.Data_CameraAnimations);
            }
        }

        public GameObject GameObj_LoadTMD(
            ModifierObject modifier, 
            PS1_TMD tmd, 
            ObjTransform_ArchiveFile[] localTransforms = null, 
            ObjTransform_ArchiveFile[] absoluteTransforms = null, 
            int index = 0, 
            bool multiple = false)
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
                transforms: localTransforms,
                animSpeed: new AnimSpeed_FrameIncrease(modifier.AnimatedLocalTransformSpeed),
                animLoopMode: modifier.DoesAnimatedLocalTransformPingPong ? AnimLoopMode.PingPong : AnimLoopMode.Repeat);

            if (isAnimated)
                GameObj_IsAnimated = true;

            int count = multiple ? absoluteTransforms[0].Positions.ObjectsCount : 1;

            for (int i = 0; i < count; i++)
            {
                var obj = i == 0 ? gameObj : Object.Instantiate(gameObj);

                // Apply the absolute transform
                isAnimated = Manager.ApplyTransform(
                    gameObj: gameObj, 
                    transforms: absoluteTransforms, 
                    scale: Scale, 
                    objIndex: i, 
                    animSpeed: new AnimSpeed_FrameIncrease(modifier.AnimatedAbsoluteTransformSpeed), 
                    animLoopMode: modifier.DoesAnimatedAbsoluteTransformPingPong ? AnimLoopMode.PingPong : AnimLoopMode.Repeat);

                if (isAnimated)
                    GameObj_IsAnimated = true;

                // Set the parent object
                obj.transform.SetParent(ParentObject.transform);

                GameObj_Objects.Add(obj);
            }

            return gameObj;
        }

        public void GameObj_ApplyPosition(GameObject obj, KlonoaVector16 pos, Vector3? posOffset = null)
        {
            obj.transform.position = PSKlonoaHelpers.GetPositionVector(pos, posOffset, Scale);
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

                case BackgroundModifierObject.BackgroundModifierType.PaletteSwap:
                    // TODO: Implement
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