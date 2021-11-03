using BinarySerializer;
using BinarySerializer.Klonoa.DTP;
using BinarySerializer.PS1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Klonoa;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ray1Map.PSKlonoa
{
    public class PSKlonoa_DTP_GameObjectsLoader
    {
        public PSKlonoa_DTP_GameObjectsLoader(PSKlonoa_DTP_BaseManager manager, Loader loader, float scale, GameObject parentObject, GameObject3D[] gameObjects3D, BackgroundGameObject[] backgroundObjects)
        {
            Manager = manager;
            Loader = loader;
            Scale = scale;
            ParentObject = parentObject;
            GameObjects3D = gameObjects3D;
            BackgroundObjects = backgroundObjects;

            GameObj_Objects = new List<GameObject>();
            GameObj_CameraAnimations = new List<CameraAnimations_File>();
            BG_Layers = new List<BackgroundLayer>();
            BG_Clears = new List<BackgroundGameObjectData_Clear>();
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
        public GameObject3D[] GameObjects3D { get; }
        public BackgroundGameObject[] BackgroundObjects { get; }

        // Game objects
        public bool GameObj_IsAnimated { get; set; }
        public List<GameObject> GameObj_Objects { get; }
        public List<CameraAnimations_File> GameObj_CameraAnimations { get; }

        // Backgrounds
        public bool HasHUD { get; set; }
        public List<BackgroundLayer> BG_Layers { get; }
        public List<BackgroundGameObjectData_Clear> BG_Clears { get; }
        public BackgroundGameObjectData_SetLightState BG_LightState { get; set; }

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
                for (var objIndex = 0; objIndex < GameObjects3D.Length; objIndex++)
                {
                    var obj3D = GameObjects3D[objIndex];

                    Controller.DetailedState = $"Loading game object {objIndex + 1}/{GameObjects3D.Length} (loop {i + 1}/2)";
                    await Controller.WaitIfNecessary();

                    LoadGameObject3D(obj3D, (LoadLoop)i);
                }

                for (var objIndex = 0; objIndex < BackgroundObjects.Length; objIndex++)
                {
                    var bgObj = BackgroundObjects[objIndex];

                    Controller.DetailedState = $"Loading background object {objIndex + 1}/{BackgroundObjects.Length} (loop {i + 1}/2)";
                    await Controller.WaitIfNecessary();

                    LoadBackgroundObject(bgObj, (LoadLoop)i);
                }
            }
        }

        private HashSet<ModelAnimation_ArchiveFile> _correctedTransforms;
        public void LoadGameObject3D(GameObject3D obj3D, LoadLoop loop)
        {
            // Ignore invalid objects
            if (obj3D.IsInvalid)
                return;

            // Log unknown objects
            if (obj3D.GlobalGameObjectType == GlobalGameObjectType.Unknown)
            {
                if (loop != LoadLoop.Animations)
                    return;

                Debug.LogWarning($"Unknown object type at {obj3D.Offset} of type " +
                                 $"{(int)obj3D.PrimaryType}-{obj3D.SecondaryType} with data:{Environment.NewLine}" +
                                 $"{String.Join($"{Environment.NewLine}{Environment.NewLine}", obj3D.Data_Unknown?.Select(dataFile => $"Length: {dataFile.Data.Length} bytes{Environment.NewLine}{dataFile.Data.ToHexString(align: 16, maxLines: 16)}") ?? new string[0])}");
                return;
            }

            // Handle animations
            if (loop == LoadLoop.Animations)
            {
                switch (obj3D.GlobalGameObjectType)
                {
                    case GlobalGameObjectType.ScrollAnimation:
                        Anim_ScrollAnimations.Add(obj3D.Data_UVScrollAnimation);
                        break;

                    case GlobalGameObjectType.VRAMScrollAnimation:
                    case GlobalGameObjectType.VRAMScrollAnimationWithTexture:
                        KlonoaSettings_DTP.VRAMScrollInfo[] scroll = obj3D.VRAMScrollInfos;
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

                    case GlobalGameObjectType.RGBAnimation:
                        // TODO: Implement
                        break;

                    case GlobalGameObjectType.TextureAnimation:
                        Anim_TextureAnimations.Add(new PS1VRAMAnimation(obj3D.Data_TextureAnimation.Files.Select(x => x.Obj).ToArray(), obj3D.TextureAnimationInfo.AnimSpeed, obj3D.TextureAnimationInfo.PingPong));
                        break;

                    case GlobalGameObjectType.PaletteAnimation:
                    case GlobalGameObjectType.PaletteAnimations:
                    case GlobalGameObjectType.ObjectWithPaletteAnimation:
                    case GlobalGameObjectType.NahatombPaletteAnimation:
                        var anim = obj3D.GlobalGameObjectType == GlobalGameObjectType.PaletteAnimation || 
                                   obj3D.GlobalGameObjectType == GlobalGameObjectType.ObjectWithPaletteAnimation ||
                                   obj3D.GlobalGameObjectType == GlobalGameObjectType.NahatombPaletteAnimation
                            ? obj3D.Data_PaletteAnimation.YieldToArray() 
                            : obj3D.Data_PaletteAnimations.Files;

                        // Correct the alpha in the colors. Sometimes it's incorrect. Seems the game for model graphics only uses
                        // transparency if the color index is 0?
                        foreach (var p in anim.SelectMany(x => x.Files).Select(x => x.Colors))
                        {
                            for (int c = 2 + 1; c < p.Length; c += 2)
                                p[c] |= 0x80;
                        }

                        for (int i = 0; i < obj3D.PaletteAnimationVRAMRegions.Length; i++)
                        {
                            var region = obj3D.PaletteAnimationVRAMRegions[i];
                            var palettes = anim[i % anim.Length].Files;
                            var colors = palettes.Select(x => x.Colors).ToArray();
                            Anim_PaletteAnimations.Add(new PS1VRAMAnimation(region, colors, obj3D.PaletteAnimationInfo.AnimSpeed, true));
                        }
                        break;
                }
            }
            // Handle objects
            else
            {
                if (obj3D.Data_TMD == null)
                    return;

                if (obj3D.GlobalGameObjectType == GlobalGameObjectType.GeyserPlatform)
                {
                    var positions = obj3D.GeyserPlatformPositions;

                    for (int i = 0; i < positions.Length; i++)
                    {
                        var geyserObj = GameObj_LoadTMD(obj3D, obj3D.Data_TMD, index: i);
                        GameObj_ApplyPosition(geyserObj, positions[i].Position);
                    }

                    return;
                }

                bool isMultiple = false;
                Vector3Int objPosOffset = Vector3Int.zero;

                // TODO: Hard-code this in the object
                switch (obj3D.GlobalGameObjectType)
                {
                    case GlobalGameObjectType.WindSwirl:
                        objPosOffset = new Vector3Int(0, 182, 0);
                        break;

                    case GlobalGameObjectType.MultiWheel:
                        isMultiple = true;
                        break;

                    case GlobalGameObjectType.FallingTargetPlatform:
                        _correctedTransforms ??= new HashSet<ModelAnimation_ArchiveFile>();

                        foreach (var rot in obj3D.Data_LocalTransforms.Files.Where(x => !_correctedTransforms.Contains(x)).SelectMany(x => x.Rotations.Vectors).SelectMany(x => x))
                            rot.X += 0x400;
                        
                        foreach (var f in obj3D.Data_LocalTransforms.Files)
                            _correctedTransforms.Add(f);
                        break;
                }

                // Load the object model from the TMD data
                var obj = GameObj_LoadTMD(
                    obj3D: obj3D, 
                    tmd: obj3D.Data_TMD, 
                    localTransforms: obj3D.Data_LocalTransform?.YieldToArray() ?? obj3D.Data_LocalTransforms?.Files, 
                    absoluteTransforms: obj3D.Data_AbsoluteTransform?.YieldToArray() ?? obj3D.Data_AbsoluteTransforms?.Files, 
                    multiple: isMultiple,
                    modelAnims: obj3D.ModelAnimations);

                // Apply a position if available
                if (obj3D.Data_Position != null)
                    GameObj_ApplyPosition(obj, obj3D.Data_Position, objPosOffset);

                // Apply a constant rotation if available
                addConstantRot(KlonoaDTPConstantRotationComponent.RotationAxis.X, obj3D.ConstantRotationX);
                addConstantRot(KlonoaDTPConstantRotationComponent.RotationAxis.Y, obj3D.ConstantRotationY);
                addConstantRot(KlonoaDTPConstantRotationComponent.RotationAxis.Z, obj3D.ConstantRotationZ);

                void addConstantRot(KlonoaDTPConstantRotationComponent.RotationAxis axis, float? speed)
                {
                    if (speed == null)
                        return;

                    var rotComponent = obj.AddComponent<KlonoaDTPConstantRotationComponent>();
                    rotComponent.animatedTransform = obj.transform;
                    rotComponent.initialRotation = obj.transform.localRotation;
                    rotComponent.axis = axis;
                    rotComponent.rotationSpeed = speed.Value;
                    rotComponent.minValue = obj3D.ConstantRotationMin;
                    rotComponent.length = obj3D.ConstantRotationLength;
                }

                // Load secondary object if available
                if (obj3D.Data_TMD_Secondary != null)
                {
                    var secondaryObj = GameObj_LoadTMD(
                        obj3D: obj3D,
                        tmd: obj3D.Data_TMD_Secondary,
                        index: 1);

                    // Apply a position if available (without the offset)
                    if (obj3D.Data_Position != null)
                        GameObj_ApplyPosition(secondaryObj, obj3D.Data_Position);
                }

                if (obj3D.Data_CameraAnimations != null)
                    GameObj_CameraAnimations.Add(obj3D.Data_CameraAnimations);
            }
        }

        public GameObject GameObj_LoadTMD(
            GameObject3D obj3D, 
            PS1_TMD tmd, 
            ModelAnimation_ArchiveFile[] localTransforms = null, 
            ModelAnimation_ArchiveFile[] absoluteTransforms = null, 
            int index = 0, 
            bool multiple = false,
            ArchiveFile<ModelBoneAnimation_ArchiveFile> modelAnims = null)
        {
            if (tmd == null) 
                throw new ArgumentNullException(nameof(tmd));
            
            GameObject gameObj;
            bool isAnimated;

            (gameObj, isAnimated) = Manager.CreateGameObject(
                tmd: tmd,
                loader: Loader,
                scale: Scale,
                name: $"Object3D Offset:{obj3D.Offset} Index:{index} Type:{obj3D.PrimaryType}-{obj3D.SecondaryType} ({obj3D.GlobalGameObjectType})",
                objectsLoader: this,
                isPrimaryObj: false,
                animations: localTransforms,
                animSpeed: new AnimSpeed_FrameIncrease(obj3D.AnimatedLocalTransformSpeed),
                animLoopMode: obj3D.DoesAnimatedLocalTransformPingPong ? AnimLoopMode.PingPong : AnimLoopMode.Repeat,
                boneAnimations: modelAnims);

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
                    animSpeed: new AnimSpeed_FrameIncrease(obj3D.AnimatedAbsoluteTransformSpeed), 
                    animLoopMode: obj3D.DoesAnimatedAbsoluteTransformPingPong ? AnimLoopMode.PingPong : AnimLoopMode.Repeat);

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

        public void LoadBackgroundObject(BackgroundGameObject bgObj, LoadLoop loop)
        {
            switch (bgObj.Type)
            {
                case BackgroundGameObject.BackgroundGameObjectType.HUD:
                    {
                        if (loop != LoadLoop.Objects)
                            return;

                        HasHUD = true;
                    }
                    break;

                case BackgroundGameObject.BackgroundGameObjectType.BackgroundLayer_19:
                case BackgroundGameObject.BackgroundGameObjectType.BackgroundLayer_22:
                    {
                        if (loop != LoadLoop.Objects)
                            return;

                        var (frames, speed) = Manager.GetBackgroundFrames(Loader, this, Loader.BackgroundPack, bgObj);

                        BG_Layers.Add(new BackgroundLayer(bgObj, frames, speed));
                    }
                    break;

                case BackgroundGameObject.BackgroundGameObjectType.Clear_Gradient:
                case BackgroundGameObject.BackgroundGameObjectType.Clear:
                    {
                        if (loop != LoadLoop.Objects)
                            return;

                        BG_Clears.Add(bgObj.Data_Clear);
                    }
                    break;

                case BackgroundGameObject.BackgroundGameObjectType.PaletteScroll:
                    {
                        if (loop != LoadLoop.Animations)
                            return;

                        BackgroundGameObjectData_PaletteScroll scroll = bgObj.Data_PaletteScroll;
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

                case BackgroundGameObject.BackgroundGameObjectType.SetLightState:
                    {
                        if (loop != LoadLoop.Animations)
                            return;

                        BG_LightState = bgObj.Data_SetLightState;
                    }
                    break;

                case BackgroundGameObject.BackgroundGameObjectType.PaletteSwap:
                    // TODO: Implement
                    break;

                case BackgroundGameObject.BackgroundGameObjectType.Unknown_1:
                case BackgroundGameObject.BackgroundGameObjectType.Reset:
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
            public BackgroundLayer(BackgroundGameObject obj, Texture2D[] frames, int speed)
            {
                Object = obj;
                Frames = frames;
                Speed = speed;
            }

            public BackgroundGameObject Object { get; }
            public Texture2D[] Frames { get; }
            public int Speed { get; }
        }
    }
}