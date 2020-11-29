using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public abstract class GBC_BaseManager : IGameManager
    {
        public const int CellSize = 8;
        public const string GlobalOffsetTableKey = "GlobalOffsetTable";

        public abstract int LevelCount { get; }
        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, LevelCount).ToArray()),
        });

        public virtual GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Log Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output, false)),
            new GameAction("Export Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output, true)),
            new GameAction("Export Animation Frames", false, true, (input, output) => ExportAnimFramesAsync(settings, output)),
        };

        public async UniTask ExportBlocksAsync(GameSettings settings, string outputDir, bool export)
        {
            using (var context = new Context(settings))
            {
                await LoadFilesAsync(context);

                // Get the deserializer
                var s = context.Deserializer;

                var references = new Dictionary<Pointer, HashSet<Pointer>>();

                using (var logFile = File.Create(Path.Combine(outputDir, "GBC_Blocks_Log-Map.txt")))
                {
                    using (var writer = new StreamWriter(logFile))
                    {
                        var indentLevel = 0;
                        GBC_DummyBlock rootBlock = s.DoAt(GetSceneList(context).Offset, () => s.SerializeObject<GBC_DummyBlock>(default, name: $"RootBlock"));

                        void ExportBlocks(GBC_DummyBlock block, int index, string path)
                        {
                            indentLevel++;

                            if (export && block.Data != null)
                                Util.ByteArrayToFile(Path.Combine(outputDir, path, $"{block.Offset.file.filePath}_0x{block.Offset.StringFileOffset}.bin"), block.Data);

                            writer.WriteLine($"{$"{block.Offset}:",-30}{new string(' ', indentLevel * 2)}[{index}] Offsets: {block.OffsetTable.OffsetsCount} - BlockSize: {block.Data?.Length}");

                            // Handle every block offset in the table
                            for (int i = 0; i < block.SubBlocks.Length; i++)
                            {
                                if (block.SubBlocks[i] == null)
                                    continue;

                                if (!references.ContainsKey(block.SubBlocks[i].Offset))
                                    references[block.SubBlocks[i].Offset] = new HashSet<Pointer>();

                                references[block.SubBlocks[i].Offset].Add(block.Offset);

                                // Export
                                ExportBlocks(block.SubBlocks[i], i, Path.Combine(path, $"{i} - {block.SubBlocks[i].Offset.file.filePath}_0x{block.SubBlocks[i].Offset.StringFileOffset}"));
                            }

                            indentLevel--;
                        }

                        ExportBlocks(rootBlock, 0, $"{rootBlock.Offset.file.filePath}_0x{rootBlock.Offset.StringFileOffset}");
                    }
                }

                // Log references
                using (var logFile = File.Create(Path.Combine(outputDir, "GBC_Blocks_Log-References.txt")))
                {
                    using (var writer = new StreamWriter(logFile))
                    {
                        foreach (var r in references.OrderBy(x => x.Key))
                        {
                            writer.WriteLine($"{$"{r.Key}:",-30} {String.Join(", ", r.Value.Select(x => $"{x.AbsoluteOffset:X8}"))}");
                        }
                    }
                }

                // If LUDI, export unreferenced blocks
                if (export) {
                    var got = context.GetStoredObject<LUDI_GlobalOffsetTable>(GlobalOffsetTableKey);
                    if (got != null) {
                        foreach (var file in got.Files) {
                            string filename = Path.GetFileNameWithoutExtension(file.Offset.file.AbsolutePath);
                            if (file.OffsetTable != null) {
                                for (int i = 0; i < file.OffsetTable.NumEntries; i++) {
                                    var id = file.OffsetTable.Entries[i].BlockID;
                                    Pointer blockPtr = file.Resolve(id);
                                    if (!references.ContainsKey(blockPtr)) {
                                        uint? blockLength = file.GetLength(id);
                                        if (blockLength.HasValue) {
                                            s.DoAt(blockPtr, () => {
                                                byte[] data = s.SerializeArray<byte>(default, blockLength.Value, name: $"{filename}_{id}");
                                                Util.ByteArrayToFile(Path.Combine(outputDir, $"Unreferenced/{filename} - ID_{file.FileID.FileID}", $"{id}_{blockPtr.StringFileOffset}.bin"), data);
                                            });
                                        }
                                    }
                                }
                            } else if (file.DataInfo != null) {
                                for (int i = 0; i < file.DataInfo.NumDataBlocks; i++) {
                                    var id = (ushort)(i + 1);
                                    Pointer blockPtr = file.Resolve(id);
                                    if (!references.ContainsKey(blockPtr)) {
                                        uint? blockLength = file.GetLength(id);
                                        if (blockLength.HasValue) {
                                            s.DoAt(blockPtr, () => {
                                                LUDI_DummyBlock dummyBlock = s.SerializeObject<LUDI_DummyBlock>(default, name: $"{filename}_{id}");
                                                Util.ByteArrayToFile(Path.Combine(outputDir, $"Unreferenced/{filename} - ID_{file.FileID.FileID}", $"{id}_{blockPtr.StringFileOffset}.bin"), dummyBlock.Data);
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Debug.Log("Finished logging blocks");
        }

        public async UniTask ExportAnimFramesAsync(GameSettings settings, string outputDir)
        {
            var exported = new HashSet<Pointer>();

            // Enumerate every level
            for (int lev = 0; lev < LevelCount; lev++)
            {
                Debug.Log($"Exporting level {lev + 1}/{LevelCount}");

                settings.Level = lev;

                using (var context = new Context(settings))
                {
                    // Load the files
                    await LoadFilesAsync(context);

                    // Read the level
                    var data = GetSceneList(context).Level;

                    await UniTask.WaitForEndOfFrame();

                    // Enumerate every graphic group
                    foreach (var model in data.Scene.Actors.Select(x => x.ActorModel).Where(x => x != null).Distinct())
                    {
                        if (exported.Contains(model.Offset))
                            return;

                        exported.Add(model.Offset);

                        try
                        {
                            var commonDesign = GetCommonDesign(model);

                            for (var animIndex = 0; animIndex < commonDesign.Animations.Count; animIndex++)
                            {
                                await UniTask.WaitForEndOfFrame();

                                var anim = commonDesign.Animations[animIndex];

                                var frameIndex = 0;

                                foreach (var tex in GetAnimationFrames(model, anim, commonDesign.Sprites))
                                {
                                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{model.Offset.file.filePath}_0x{model.Offset.FileOffset}", $"{animIndex}", $"{frameIndex}.png"), tex.EncodeToPNG());
                                    frameIndex++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning(ex);
                        }
                    }
                }
            }

            Debug.Log("Finished export");
        }

        public IEnumerable<Texture2D> GetAnimationFrames(GBC_ActorModel model, Unity_ObjAnimation objAnim, IList<Sprite> sprites)
        {
            var w = model.RenderBoxWidth + Math.Abs(model.RenderBoxX);
            var h = model.RenderBoxHeight + Math.Abs(model.RenderBoxY);

            var minX = objAnim.Frames.SelectMany(x => x.SpriteLayers).Min(x => x.XPosition);
            var minY = objAnim.Frames.SelectMany(x => x.SpriteLayers).Min(x => x.YPosition);

            // Enumerate every frame
            foreach (var frame in objAnim.Frames)
            {
                var tex = TextureHelpers.CreateTexture2D(w - minX, h - minY, clear: true);

                foreach (var layer in frame.SpriteLayers)
                {
                    var sprite = sprites[layer.ImageIndex];

                    // TODO: Copy sprite pixels to tex, ignoring transparent pixels
                }

                tex.Apply();
                yield return tex;
            }
        }

        public abstract GBC_LevelList GetSceneList(Context context);

        public abstract Unity_Map[] GetMaps(Context context, GBC_PlayField playField, GBC_Level level);

        public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            var sceneList = GetSceneList(context);

            // Log unused data blocks in offset tables
            var notParsedBlocks = GBC_OffsetTable.OffsetTables.Where(x => x.UsedOffsets.Any(y => !y) && x != sceneList.OffsetTable).ToArray();
            if (notParsedBlocks.Any())
                Debug.Log($"The following blocks were never parsed:{Environment.NewLine}" + String.Join(Environment.NewLine, notParsedBlocks.Select(y => $"[{y.Offset}]:" + String.Join(", ", y.UsedOffsets.Select((o, i) => new
                {
                    Obj = o,
                    Index = i
                }).Where(o => !o.Obj).Select(o => o.Index.ToString())))));

            var level = sceneList.Level;
            var scene = level.Scene;
            var playField = scene.PlayField;

            var maps = GetMaps(context, playField, level);

            var actorModels = new List<Unity_ObjectManager_GBC.ActorModel>();

            foreach (var actor in scene.Actors)
            {
                if (actorModels.Any(x => x.Index == actor.Index_ActorModel) || actor.ActorModel == null)
                    continue;

                try
                {
                    actorModels.Add(new Unity_ObjectManager_GBC.ActorModel(actor.Index_ActorModel, actor.ActorModel.ActionTable.Actions, GetCommonDesign(actor.ActorModel)));
                }
                catch (Exception ex)
                {
                    actorModels.Add(new Unity_ObjectManager_GBC.ActorModel(actor.Index_ActorModel, actor.ActorModel.ActionTable.Actions, null));
                    Debug.LogWarning(ex);
                }
            }

            var objManager = new Unity_ObjectManager_GBC(context, actorModels.ToArray());
            var objects = new List<Unity_Object>(scene.Actors.Select(x => new Unity_Object_GBC(x, objManager)));

            return UniTask.FromResult(new Unity_Level(
                maps: maps,
                objManager: objManager,
                eventData: objects,
                cellSize: CellSize,
                sectors: scene.Knots.Select(x => new Unity_Sector(x.Actors.Select(i => i - 1).ToList())).ToArray(),
                getCollisionTypeGraphicFunc: x => ((GBC_TileCollisionType)x).GetCollisionTypeGraphic(),
                getCollisionTypeNameFunc: x => ((GBC_TileCollisionType)x).ToString()));
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public virtual UniTask LoadFilesAsync(Context context) => UniTask.CompletedTask;

        public Unity_ObjGraphics GetCommonDesign(GBC_ActorModel model)
        {
            // Create the design
            var des = new Unity_ObjGraphics
            {
                Sprites = new List<Sprite>(),
                Animations = new List<Unity_ObjAnimation>(),
            };

            if (model == null)
                return des;

            // Get properties
            var puppet = model.ActionTable.Puppet;
            var curPuppet = puppet;
            var tileKit = curPuppet.TileKit;
            while (tileKit == null) {
                curPuppet = puppet.BasePuppet;
                tileKit = curPuppet.TileKit;
            }

            var tileSets = tileKit.GetTileSetTex();

            // Add sprites for each palette
            foreach (var tileSetTex in tileSets)
            {
                var tileIndex = 0;

                // Split texture into tile sprites
                for (int y = 0; y < tileSetTex.height; y += CellSize)
                {
                    for (int x = 0; x < tileSetTex.width; x += CellSize)
                    {
                        if (tileIndex >= tileKit.TilesCount)
                            break;

                        des.Sprites.Add(tileSetTex.CreateSprite(rect: new Rect(x, tileSetTex.height - y - CellSize, CellSize, CellSize)));
                        tileIndex++;
                    }
                }
            }

            // Add animations
            foreach (var anim in puppet.Animations)
            {
                // Create the animation
                var unityAnim = new Unity_ObjAnimation()
                {
                    Frames = new Unity_ObjAnimationFrame[anim.Keyframes.Length-1],
                    AnimSpeeds = new int[anim.Keyframes.Length-1]
                };

                // Keep track of the layers
                var channels = new List<AnimChannel>();
                //[model.PuppetLayersCount];

                // TODO: Collision

                // Enumerate every frame
                for (var frameIndex = 0; frameIndex < anim.Keyframes.Length-1; frameIndex++)
                {
                    // Get the frame
                    var frame = anim.Keyframes[frameIndex];

                    // Process every command
                    foreach (var cmd in frame.Commands)
                        ProcessAnimCommands(cmd, channels);

                    var animationParts = new List<Unity_ObjAnimationPart>();

                    // Add every visible layer
                    for (var i = 0; i < channels.Count; i++)
                    {
                        var channel = channels[i];

                        // Make sure the layer is not null and visible
                        if (!channel.IsVisible) continue;

                        // Get the layer info
                        foreach (var l in channel.SpriteInfo) {
                            addAnimationPart(l.Tile, l.XPos, l.YPos);
                        }

                        // Helper for adding a layer to the frame
                        void addAnimationPart(GBC_Keyframe_Command.TileGraphicsInfo tile, int xPos, int yPos)
                        {
                            var imgIndex = tileSets.Length == 1 ? tile.TileIndex : (int)(tile.TileIndex + (tile.Attr_PalIndex * tileKit.TilesCount));

                            animationParts.Add(new Unity_ObjAnimationPart
                            {
                                ImageIndex = imgIndex,
                                XPosition = xPos,
                                YPosition = yPos,
                                IsFlippedHorizontally = tile.Attr_HorizontalFlip,
                                IsFlippedVertically = tile.Attr_VerticalFlip
                            });
                        }
                    }

                    unityAnim.Frames[frameIndex] = new Unity_ObjAnimationFrame(animationParts.ToArray());
                    unityAnim.AnimSpeeds[frameIndex] = frame.Time;
                }

                // Add the animation
                des.Animations.Add(unityAnim);
            }

            return des;
        }

        protected void ProcessAnimCommands(GBC_Keyframe_Command cmd, List<AnimChannel> channels)
        {
            switch (cmd.Command)
            {
                case GBC_Keyframe_Command.InstructionCommand.SpriteNew:
                    var layerInfos = new AnimLayerInfo[cmd.LayerInfos.Length];
                    for (int i = 0; i < cmd.LayerInfos.Length; i++) {
                        layerInfos[i] = new AnimLayerInfo() {
                            SpriteID = cmd.LayerInfos[i].SpriteID,
                            Tile = cmd.LayerInfos[i].Tile,
                            XPos = cmd.LayerInfos[i].XPos + (i > 0 ? layerInfos[0].XPos : 0),
                            YPos = cmd.LayerInfos[i].YPos  + (i > 0 ? layerInfos[0].YPos : 0)
                        };
                    }
                    channels.Add(new AnimChannel(layerInfos));
                    break;

                case GBC_Keyframe_Command.InstructionCommand.SpriteMove:
                    foreach (var sprite in channels[cmd.ChannelIndex].SpriteInfo) {
                        sprite.XPos += cmd.XPos;
                        sprite.YPos += cmd.YPos;
                    }

                    break;

                case GBC_Keyframe_Command.InstructionCommand.SetTileGraphics:

                    for (int i = 0; i < cmd.TileGraphicsInfos.Length; i++)
                        channels[cmd.ChannelIndex].SpriteInfo[i].Tile = cmd.TileGraphicsInfos[i];
                    break;

                case GBC_Keyframe_Command.InstructionCommand.SetInvisible:
                    channels[cmd.ChannelIndex].IsVisible = false;
                    break;

                case GBC_Keyframe_Command.InstructionCommand.SetVisible:
                    channels[cmd.ChannelIndex].IsVisible = true;
                    break;

                case GBC_Keyframe_Command.InstructionCommand.Terminator:
                    channels.Clear();
                    break;
            }
        }

        protected class AnimChannel
        {
            public AnimChannel(AnimLayerInfo[] spriteInfo) {
                SpriteInfo = spriteInfo;
            }

            public AnimLayerInfo[] SpriteInfo { get; }
            public bool IsVisible { get; set; } = true;
        }
        protected class AnimLayerInfo {
            public byte SpriteID { get; set; } // The index this sprite is given in the puppet's sprite array
            public GBC_Keyframe_Command.TileGraphicsInfo Tile { get; set; }
            public int XPos { get; set; }
            public int YPos { get; set; }
        }
    }
}