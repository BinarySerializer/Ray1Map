using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageMagick;
using UnityEngine;

namespace R1Engine
{
    public class GBACrash_Crash2_Manager : IGameManager
    {
        public const string LocTableID = "LocTable";

        public const int CellSize = 8;
        public string GetROMFilePath => "ROM.gba";

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, Levels.Length).ToArray()),
        });

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Animation Frames", false, true, (input, output) => ExportAnimFramesAsync(settings, output, false)),
            new GameAction("Export Animations as GIF", false, true, (input, output) => ExportAnimFramesAsync(settings, output, true)),
        };

        public async UniTask ExportAnimFramesAsync(GameSettings settings, string outputDir, bool saveAsGif)
        {
            using (var context = new Context(settings))
            {
                // Load the files
                await LoadFilesAsync(context);

                // Read the rom
                var rom = FileFactory.Read<GBACrash_ROM>(GetROMFilePath, context);

                await UniTask.WaitForEndOfFrame();

                // Enumerate every anim set
                for (int animSetIndex = 0; animSetIndex < rom.AnimSets.Length; animSetIndex++)
                {
                    var animSet = rom.AnimSets[animSetIndex];

                    // Enumerate every animation
                    for (var animIndex = 0; animIndex < animSet.Animations.Length; animIndex++)
                    {
                        await UniTask.WaitForEndOfFrame();

                        var anim = animSet.Animations[animIndex];
                        var frames = GetAnimFrames(animSet, animIndex, rom.ObjTileSet, Util.ConvertGBAPalette(rom.ObjPalettes[anim.PaletteIndex].Palette));

                        if (saveAsGif)
                        {
                            using (MagickImageCollection collection = new MagickImageCollection())
                            {
                                int index = 0;

                                foreach (var frameTex in frames)
                                {
                                    var img = frameTex.ToMagickImage();
                                    collection.Add(img);
                                    collection[index].AnimationDelay = anim.AnimSpeed + 1;
                                    collection[index].AnimationTicksPerSecond = 60;
                                    collection[index].Trim();

                                    collection[index].GifDisposeMethod = GifDisposeMethod.Background;
                                    index++;
                                }

                                // Save gif
                                collection.Write(Path.Combine(outputDir, $"{animSetIndex} - {animIndex}.gif"));
                            }
                        }
                        else
                        {
                            var frameIndex = 0;

                            foreach (var tex in frames)
                            {
                                Util.ByteArrayToFile(Path.Combine(outputDir, $"{animSetIndex}", $"{animIndex}", $"{frameIndex}.png"), tex.EncodeToPNG());
                                frameIndex++;
                            }
                        }
                    }
                }
            }
        }

        public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBACrash_ROM>(GetROMFilePath, context);
            var levInfo = Levels[context.Settings.Level];
            var levelInfo = rom.LevelInfos[levInfo.LevelIndex];

            GBACrash_MapInfo map;

            if (levInfo.MapType == LevInfo.Type.Normal)
                map = levelInfo.LevelData.Maps[levInfo.MapIndex];
            else if (levInfo.MapType == LevInfo.Type.Bonus)
                map = levelInfo.LevelData.BonusMap;
            else
                map = levelInfo.LevelData.ChallengeMap;

            Controller.DetailedState = "Loading tilesets";
            await Controller.WaitIfNecessary();

            var tileSets = new Dictionary<GBACrash_TileSet, Unity_TileSet>();

            for (int i = 0; i < map.MapData2D.MapLayers.Length; i++)
            {
                var l = map.MapData2D.MapLayers[i];

                if (l == null)
                    continue;

                if (!tileSets.ContainsKey(l.TileSet))
                    tileSets.Add(l.TileSet, LoadTileSet(l.TileSet.TileSet, map.TilePalette2D, i == 3));
            }

            Controller.DetailedState = "Loading maps";
            await Controller.WaitIfNecessary();

            var maps = map.MapData2D.MapLayers.Select((x, i) =>
            {
                if (x == null)
                    return null;

                return new
                {
                    Map = new Unity_Map
                    {
                        Width = x.MapWidth,
                        Height = x.MapHeight,
                        TileSet = new Unity_TileSet[]
                        {
                            tileSets[x.TileSet]
                        },
                        MapTiles = GetTileMap(x, map.MapData2D.DataBlock.TileLayerDatas[i], i == 3, x.TileSet.TileSet.Length / 32),
                        Type = Unity_Map.MapType.Graphics,
                        Layer = x.LayerPrio == 1 ? Unity_Map.MapLayer.Front : Unity_Map.MapLayer.Middle
                    },
                    Prio = x.LayerPrio
                };
            }).Where(x => x != null).OrderByDescending(x => x.Prio).Select(x => x.Map).Append(new Unity_Map()
            {
                Width = map.MapData2D.CollisionLayer.MapWidth,
                Height = map.MapData2D.CollisionLayer.MapHeight,
                TileSet = new Unity_TileSet[0],
                MapTiles = GetTileMap(map.MapData2D.CollisionLayer, map.MapData2D.DataBlock.CollisionLayerData, isCollision: true),
                Type = Unity_Map.MapType.Collision,
            }).ToArray();

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            var objmanager = new Unity_ObjectManager_GBACrash(context, LoadAnimSets(rom));
            var objects = map.MapData2D.ObjData.ObjGroups.SelectMany(x => x.Objects).Select(x => new Unity_Object_GBACrash(objmanager, x));

            return new Unity_Level(
                maps: maps,
                objManager: objmanager,
                eventData: new List<Unity_Object>(objects),
                cellSize: CellSize,
                getCollisionTypeGraphicFunc: x => ((GBACrash_Crash2_CollisionType)x).GetCollisionTypeGraphic(),
                getCollisionTypeNameFunc: x => ((GBACrash_Crash2_CollisionType)x).ToString());
        }

        public Unity_TileSet LoadTileSet(byte[] tileSet, RGBA5551Color[] pal, bool is8bit)
        {
            Texture2D tex;

            if (is8bit)
            { 
                tex = Util.ToTileSetTexture(tileSet, Util.ConvertGBAPalette(pal), Util.TileEncoding.Linear_8bpp, CellSize, false);
            }
            else
            {
                var palettes = Util.ConvertAndSplitGBAPalette(pal);

                const int wrap = 32;
                const int tileSize = 32;
                int tilesetLength = tileSet.Length / tileSize;

                int tilesX = Math.Min((tilesetLength * palettes.Length), wrap);
                int tilesY = Mathf.CeilToInt((tilesetLength * palettes.Length) / (float)wrap);

                tex = TextureHelpers.CreateTexture2D(tilesX * CellSize, tilesY * CellSize);

                for (int palIndex = 0; palIndex < palettes.Length; palIndex++)
                {
                    var totalIndex = tilesetLength * palIndex;

                    for (int i = 0; i < tilesetLength; i++)
                    {
                        int tileY = (((i + totalIndex) / wrap)) * CellSize;
                        int tileX = ((i + totalIndex) % wrap) * CellSize;

                        tex.FillInTile(
                            imgData: tileSet,
                            imgDataOffset: i * tileSize,
                            pal: palettes[palIndex],
                            encoding: Util.TileEncoding.Linear_4bpp,
                            tileWidth: CellSize,
                            flipTextureY: false,
                            tileX: tileX,
                            tileY: tileY);
                    }
                }

                tex.Apply();
            }

            return new Unity_TileSet(tex, CellSize);
        }

        public Unity_Tile[] GetTileMap(GBACrash_MapLayer layer, GBACrash_MapData2DDataBlock.GBACrash_TileLayerData tileLayerData, bool is8bit = false, int tileSetLength = 0, bool isCollision = false)
        {
            var tileMap = new Unity_Tile[layer.MapWidth * layer.MapHeight];

            const int blockWidth = 16;
            const int blockHeight = 8;

            for (int y = 0; y < layer.TileMapHeight; y++)
            {
                for (int x = 0; x < layer.TileMapWidth; x++)
                {
                    var actualX = x * blockWidth;
                    var actualY = y * blockHeight;

                    var cmds = tileLayerData.TileMapTileCommands[layer.TileMap[y * layer.TileMapWidth + x]];

                    var cmdIndex = 0;
                    var parsedTiles = 0x7f;
                    var index = 0;

                    do
                    {
                        var cmd = cmds.TileCommands[cmdIndex++];
                        var count = cmd.TilesCount;

                        if ((cmd.CommandType & 0x80) == 0)
                        {
                            if ((cmd.CommandType & 0x40) == 0)
                            {
                                parsedTiles -= count;

                                for (int i = 0; i < count; i++)
                                    setTileAt(index++, cmd.TileIndices[i], cmd.CommandType);
                            }
                            else
                            {
                                parsedTiles -= count;

                                // Get the initial tile index
                                int curTileIndex = cmd.TileIndices[0];

                                // Set the initial tile index
                                setTileAt(index++, curTileIndex, cmd.CommandType);

                                // Set every changed tile index
                                for (int i = 0; i < cmd.TilesCount - 1; i++)
                                {
                                    // Change the tile index
                                    curTileIndex += cmd.TileChanges[i];

                                    // Set changed index
                                    setTileAt(index++, curTileIndex, cmd.CommandType);
                                }
                            }
                        }
                        else
                        {
                            parsedTiles -= count;

                            for (int i = 0; i < count; i++)
                                setTileAt(index++, cmd.TileIndices[0], cmd.CommandType);
                        }
                    } while (-1 < parsedTiles);

                    void setTileAt(int blockIndex, int tileIndex, byte cmd)
                    {
                        int offY = Mathf.FloorToInt(blockIndex / (float)blockWidth);
                        int offX = blockIndex - offY * blockWidth;

                        var outputX = actualX + offX;
                        var outputY = actualY + offY;

                        if (outputX >= layer.MapWidth)
                            return;

                        var tileMapIndex = outputY * layer.MapWidth + outputX;

                        if (tileMap.Length <= tileMapIndex)
                            return;

                        MapTile mapTile;

                        if (isCollision)
                        {
                            mapTile = new MapTile()
                            {
                                CollisionType = (ushort)tileIndex
                            };
                        }
                        else if (is8bit)
                        {
                            mapTile = new MapTile()
                            {
                                TileMapY = (ushort)(BitHelpers.ExtractBits(tileIndex, 14, 0)),
                                HorizontalFlip = BitHelpers.ExtractBits(tileIndex, 1, 14) == 1,
                                VerticalFlip = BitHelpers.ExtractBits(tileIndex, 1, 15) == 1,
                            };
                        }
                        else
                        {
                            mapTile = new MapTile()
                            {
                                TileMapY = (ushort)(BitHelpers.ExtractBits(tileIndex, 10, 0) + (tileSetLength * BitHelpers.ExtractBits(tileIndex, 4, 12))),
                                HorizontalFlip = BitHelpers.ExtractBits(tileIndex, 1, 10) == 1,
                                VerticalFlip = BitHelpers.ExtractBits(tileIndex, 1, 11) == 1,
                            };
                        }

                        tileMap[tileMapIndex] = new Unity_Tile(mapTile)
                        {
                            DebugText = $"CMD: {cmd}{Environment.NewLine}" +
                                        $"Index: {blockIndex}{Environment.NewLine}" +
                                        $"TileIndex: {tileIndex}{Environment.NewLine}"
                        };
                    }
                }
            }

            return tileMap;
        }

        public Unity_ObjectManager_GBACrash.AnimSet[] LoadAnimSets(GBACrash_ROM rom)
        {
            return rom.AnimSets.Select(animSet => new Unity_ObjectManager_GBACrash.AnimSet(animSet.Animations.Select((anim, i) => new Unity_ObjectManager_GBACrash.AnimSet.Animation(
                animFrameFunc: () => GetAnimFrames(animSet, i, rom.ObjTileSet, Util.ConvertGBAPalette(rom.ObjPalettes[anim.PaletteIndex].Palette)).Select(frame => frame.CreateSprite()).ToArray(),
                crashAnim: anim,
                xPos: animSet.GetMinX(i),
                yPos: animSet.GetMinY(i)
            )).ToArray())).ToArray();
        }

        public Texture2D[] GetAnimFrames(GBACrash_AnimSet animSet, int animIndex, byte[] tileSet, Color[] pal)
        {
            var shapes = TileShapes;

            var frames = animSet.Animations[animIndex].FrameTable.Select(x => animSet.AnimationFrames[x]).ToArray();

            if (!frames.Any())
                return new Texture2D[0];

            var output = new Texture2D[frames.Length];

            var minX = animSet.GetMinX(animIndex);
            var minY = animSet.GetMinY(animIndex);
            var maxX = frames.SelectMany(f => Enumerable.Range(0, f.TilesCount).Select(x => f.TilePositions[x].XPos + shapes[f.TileShapes[x].ShapeIndex].x)).Max();
            var maxY = frames.SelectMany(f => Enumerable.Range(0, f.TilesCount).Select(x => f.TilePositions[x].YPos + shapes[f.TileShapes[x].ShapeIndex].y)).Max();

            var width = (int)maxX - minX;
            var height = (int)maxY - minY;

            var frameCache = new Dictionary<GBACrash_AnimationFrame, Texture2D>();

            for (int frameIndex = 0; frameIndex < frames.Length; frameIndex++)
            {
                var frame = frames[frameIndex];

                if (!frameCache.ContainsKey(frame))
                {
                    var tex = TextureHelpers.CreateTexture2D(width, height, clear: true);

                    int offset = (int)frame.TileOffset.Value;

                    for (int i = 0; i < frame.TilesCount; i++)
                    {
                        var shape = shapes[frame.TileShapes[i].ShapeIndex];
                        var pos = frame.TilePositions[i];

                        for (int y = 0; y < shape.y; y += 8)
                        {
                            for (int x = 0; x < shape.x; x += 8)
                            {
                                tex.FillInTile(
                                    imgData: tileSet,
                                    imgDataOffset: offset,
                                    pal: pal,
                                    encoding: Util.TileEncoding.Linear_4bpp,
                                    tileWidth: CellSize,
                                    flipTextureY: true,
                                    tileX: pos.XPos + x - minX,
                                    tileY: pos.YPos + y - minY);

                                offset += 32;
                            }
                        }
                    }

                    tex.Apply();

                    frameCache.Add(frame, tex);
                }

                output[frameIndex] = frameCache[frame];
            }

            return output;
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, GBA_ROMBase.Address_ROM);

        public class LevInfo
        {
            public LevInfo(int levelIndex, int mapIndex, string displayName)
            {
                LevelIndex = levelIndex;
                MapIndex = mapIndex;
                MapType = Type.Normal;
                DisplayName = displayName;
            }
            public LevInfo(int levelIndex, Type mapType, string displayName)
            {
                LevelIndex = levelIndex;
                MapType = mapType;
                DisplayName = $"{displayName} - {mapType}";
            }

            public int LevelIndex { get; }
            public int MapIndex { get; }
            public Type MapType { get; }

            public string DisplayName { get; set; }

            public enum Type
            {
                Normal,
                Bonus,
                Challenge
            }
        }

        public LevInfo[] LevInfos => Levels;

        public static LevInfo[] Levels = new LevInfo[]
        {
            new LevInfo(0, 0, "Island Intro"), 
            new LevInfo(1, 0, "Prints of Persia"), 
            new LevInfo(1, LevInfo.Type.Bonus, "Prints of Persia"), 
            new LevInfo(2, 0, "Lagoony Tunes"), 
            new LevInfo(3, 0, "Globe Trottin'"), 
            new LevInfo(4, 0, "Pharaoh's Funhouse"), 
            new LevInfo(4, LevInfo.Type.Bonus, "Pharaoh's Funhouse"), 
            new LevInfo(5, 0, "Runaway Rug"), 
            new LevInfo(5, LevInfo.Type.Bonus, "Runaway Rug"), 
            new LevInfo(6, 0, "Tiki Torture"), 
            new LevInfo(6, LevInfo.Type.Bonus, "Tiki Torture"), 
            new LevInfo(7, 0, "Hoppin' Coffins"), 
            new LevInfo(7, LevInfo.Type.Bonus, "Hoppin' Coffins"), 
            new LevInfo(8, 0, "Barrel Roll"), 
            new LevInfo(9, 0, "Flockful of Seagulls"), 
            new LevInfo(10, 0, "Magma Mania"), 
            new LevInfo(10, LevInfo.Type.Bonus, "Magma Mania"), 
            new LevInfo(11, 0, "Run from the Sun"), 
            new LevInfo(12, 0, "Now it's Istanbul"), 
            new LevInfo(12, LevInfo.Type.Bonus, "Now it's Istanbul"), 
            new LevInfo(13, 0, "Mister Lava Lava"), 
            new LevInfo(13, LevInfo.Type.Bonus, "Mister Lava Lava"), 
            new LevInfo(14, 0, "Water Logged"), 
            new LevInfo(15, 0, "Slip-n-slidin' Sphinx"), 
            new LevInfo(15, LevInfo.Type.Bonus, "Slip-n-slidin' Sphinx"), 
            new LevInfo(16, 0, "Rocks can Roll"), 
            new LevInfo(17, 0, "Rock the Casaba"), 
            new LevInfo(17, LevInfo.Type.Bonus, "Rock the Casaba"), 
            new LevInfo(18, 0, "Eruption Disruption"), 
            new LevInfo(18, LevInfo.Type.Bonus, "Eruption Disruption"), 
            new LevInfo(19, 0, "Spaced Out"), 
            new LevInfo(20, 0, "King too Uncommon"), 
            new LevInfo(20, LevInfo.Type.Bonus, "King too Uncommon"), 
            new LevInfo(21, 0, "Wild Nile Ride"), 
            new LevInfo(22, 0, "101 Arabian Kites"), 
            new LevInfo(23, 0, "Fire Walker"), 
            new LevInfo(24, 0, "Evil Crunch"), 
            new LevInfo(25, 0, "Evil Coco"), 
            new LevInfo(26, 0, "Fake Crash"), 
            new LevInfo(27, 0, "N. Trance - Part 1"), 
            new LevInfo(27, 1, "N. Trance - Part 2"), 
            new LevInfo(28, 0, "N. Tropy - Part 1"), 
            new LevInfo(28, 1, "N. Tropy - Part 2"), 
            new LevInfo(28, 2, "N. Tropy - Part 3"), 
        };

        public Vector2[] TileShapes = new Vector2[]
        {
            new Vector2(0x08, 0x08), 
            new Vector2(0x10, 0x10), 
            new Vector2(0x20, 0x20), 
            new Vector2(0x40, 0x40), 
            new Vector2(0x10, 0x08), 
            new Vector2(0x20, 0x08), 
            new Vector2(0x20, 0x10), 
            new Vector2(0x40, 0x20), 
            new Vector2(0x08, 0x10), 
            new Vector2(0x08, 0x20), 
            new Vector2(0x10, 0x20), 
            new Vector2(0x20, 0x40), 
        };
    }
}