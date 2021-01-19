using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using ImageMagick;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public abstract class GBACrash_BaseManager : IGameManager
    {
        public const string LocTableID = "LocTable";
        public const int CellSize = 8;
        public string GetROMFilePath => "ROM.gba";

        public abstract LevInfo[] LevInfos { get; }
        public abstract int LocTableCount { get; }
        public abstract int AnimSetsCount { get; }

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, LevInfos.Length).ToArray()),
        });

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Animation Frames", false, true, (input, output) => ExportAnimFramesAsync(settings, output, false)),
            new GameAction("Export Animations as GIF", false, true, (input, output) => ExportAnimFramesAsync(settings, output, true)),
            new GameAction("Export isometric character icons", false, true, (input, output) => ExportIsometricCharacterIcons(settings, output)),
        };

        public async UniTask ExportAnimFramesAsync(GameSettings settings, string outputDir, bool saveAsGif)
        {
            // Export 2D animations
            using (var context = new Context(settings))
            {
                // Load the files
                await LoadFilesAsync(context);

                // Read the rom
                var rom = FileFactory.Read<GBACrash_ROM>(GetROMFilePath, context, (s, d) => d.CurrentLevInfo = new LevInfo(0, 0, null));

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

                        exportAnim(frames, anim.AnimSpeed + 1, "2D", $"{animSetIndex}", $"{animIndex}");
                    }
                }
            }

            // Export Mode7 animations
            for (short mode7Level = 0; mode7Level < 7; mode7Level++)
            {
                var exportedAnimSets = new HashSet<Pointer>();

                using (var context = new Context(settings))
                {
                    // Load the files
                    await LoadFilesAsync(context);

                    // Read the rom
                    var rom = FileFactory.Read<GBACrash_ROM>(GetROMFilePath, context, (s, d) => d.CurrentLevInfo = new LevInfo(GBACrash_MapInfo.GBACrash_MapType.Mode7, mode7Level, null));

                    var levInfo = rom.CurrentMode7LevelInfo;

                    var tilePal = rom.Mode7_GetTilePal(levInfo);
                    var pal = Util.ConvertAndSplitGBAPalette(levInfo.ObjPalette.Concat(tilePal).ToArray());

                    // Enumerate every anim set
                    foreach (var animSet in levInfo.GetAllAnimSets)
                    {
                        if (animSet?.Animations == null || exportedAnimSets.Contains(animSet.AnimationsPointer))
                            continue;

                        exportedAnimSets.Add(animSet.AnimationsPointer);

                        for (int animIndex = 0; animIndex < animSet.Animations.Length; animIndex++)
                        {
                            await UniTask.WaitForEndOfFrame();
                            var frames = GetMode7AnimFrames(animSet, animIndex, pal);

                            exportAnim(frames, 4, "Mode7", $"0x{animSet.AnimationsPointer.AbsoluteOffset:X8}", $"{animIndex}");
                        }
                    }
                }
            }

            if (settings.EngineVersion == EngineVersion.GBACrash_Crash2)
            {
                // Export isometric animations
                using (var context = new Context(settings))
                {
                    // Load the files
                    await LoadFilesAsync(context);

                    // Read the rom
                    var rom = FileFactory.Read<GBACrash_ROM>(GetROMFilePath, context, (s, d) => d.CurrentLevInfo = new LevInfo(GBACrash_MapInfo.GBACrash_MapType.Isometric, 0, null));

                    var pal = Util.ConvertAndSplitGBAPalette(rom.Isometric_GetObjPalette);

                    var animations = rom.Isometric_GetAnimations.ToArray();

                    // Enumerate every animation
                    for (var i = 1; i < animations.Length; i++)
                    {
                        await UniTask.WaitForEndOfFrame();

                        var frames = GetIsometricAnimFrames(animations[i], pal);

                        exportAnim(frames, 4, "Isometric", $"{i}", $"0");
                    }
                }
            }

            Debug.Log($"Finished export");

            // Helper for exporting an animation
            void exportAnim(IEnumerable<Texture2D> frames, int speed, string modeName, string animSetName, string animName)
            {
                var modeDir = Path.Combine(outputDir, modeName);

                Directory.CreateDirectory(modeDir);

                if (saveAsGif)
                {
                    using (MagickImageCollection collection = new MagickImageCollection())
                    {
                        int index = 0;

                        foreach (var frameTex in frames)
                        {
                            var img = frameTex.ToMagickImage();
                            collection.Add(img);
                            collection[index].AnimationDelay = speed;
                            collection[index].AnimationTicksPerSecond = 60;
                            collection[index].Trim();

                            collection[index].GifDisposeMethod = GifDisposeMethod.Background;
                            index++;
                        }

                        // Save gif
                        collection.Write(Path.Combine(modeDir, $"{animSetName} - {animName}.gif"));
                    }
                }
                else
                {
                    var frameIndex = 0;

                    foreach (var tex in frames)
                    {
                        Util.ByteArrayToFile(Path.Combine(modeDir, $"{animSetName}", $"{animName}", $"{frameIndex}.png"), tex.EncodeToPNG());
                        frameIndex++;
                    }
                }
            }
        }

        public async UniTask ExportIsometricCharacterIcons(GameSettings settings, string outputDir)
        {
            // Export 2D animations
            using (var context = new Context(settings))
            {
                // Load the files
                await LoadFilesAsync(context);

                // Read the rom
                var rom = FileFactory.Read<GBACrash_ROM>(GetROMFilePath, context, (s, d) => d.CurrentLevInfo = new LevInfo(GBACrash_MapInfo.GBACrash_MapType.Isometric, 0, null));

                // Enumerate every character
                for (int i = 0; i < rom.Isometric_CharacterIcons.Length; i++)
                {
                    var pal = Util.CreateDummyPalette(16).Select(x => x.GetColor()).ToArray();
                    var tex = Util.ToTileSetTexture(rom.Isometric_CharacterIcons[i].TileSet.TileSet, pal, Util.TileEncoding.Linear_4bpp, CellSize, true, wrap: 2);

                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{i:00}_{rom.Isometric_CharacterInfos[i].Name}.png"), tex.EncodeToPNG());
                }
            }
        }

        public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBACrash_ROM>(GetROMFilePath, context, (s, r) => r.CurrentLevInfo = LevInfos[context.Settings.Level]);
            var map = rom.CurrentMapInfo;

            if (map.MapType == GBACrash_MapInfo.GBACrash_MapType.Mode7)
                return await LoadMode7Async(context, rom);
            else if (map.MapType == GBACrash_MapInfo.GBACrash_MapType.Isometric)
                return await LoadIsometricAsync(context, rom);
            else
                return await Load2DAsync(context, rom);
        }

        public async UniTask<Unity_Level> Load2DAsync(Context context, GBACrash_ROM rom)
        {
            var map = rom.CurrentMapInfo;

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
                        Layer = i == 2 ? Unity_Map.MapLayer.Front : Unity_Map.MapLayer.Middle
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

            var objmanager = new Unity_ObjectManager_GBACrash(context, LoadAnimSets(rom), map);
            var objects = map.MapData2D.ObjData.ObjGroups.SelectMany(x => x.Objects.Reverse()).Select(x => new Unity_Object_GBACrash(objmanager, x));

            return new Unity_Level(
                maps: maps,
                objManager: objmanager,
                eventData: new List<Unity_Object>(objects),
                cellSize: CellSize,
                getCollisionTypeGraphicFunc: x => ((GBACrash_Crash2_CollisionType)x).GetCollisionTypeGraphic(),
                getCollisionTypeNameFunc: x => ((GBACrash_Crash2_CollisionType)x).ToString());
        }

        public async UniTask<Unity_Level> LoadMode7Async(Context context, GBACrash_ROM rom)
        {
            var levelInfo = rom.CurrentMode7LevelInfo;
            var tileSetFrames = levelInfo.TileSetFrames;

            Controller.DetailedState = "Loading maps & tilesets";
            await Controller.WaitIfNecessary();

            const int width = 240 / CellSize;
            const int height = 160 / CellSize;

            var tilePal = rom.Mode7_GetTilePal(levelInfo);

            Unity_Map[] maps = null;

            if (context.Settings.EngineVersion == EngineVersion.GBACrash_Crash1)
            {
                // Snow
                if (levelInfo.LevelType == 0)
                {
                    var cavernMapTiles = new MapTile[tileSetFrames.Width * 2 * tileSetFrames.Height * 2];

                    // Duplicate the tiles 4x times on a 2x2 grid and mirror each section
                    for (int blockY = 0; blockY < 2; blockY++)
                    {
                        var flipY = blockY % 2 != 0;

                        for (int blockX = 0; blockX < 2; blockX++)
                        {
                            var flipX = blockX % 2 != 0;

                            for (int y = 0; y < tileSetFrames.Height; y++)
                            {
                                for (int x = 0; x < tileSetFrames.Width; x++)
                                {
                                    var actualX = blockX * tileSetFrames.Width + x;
                                    var actualY = blockY * tileSetFrames.Height + y;

                                    cavernMapTiles[actualY * tileSetFrames.Width * 2 + actualX] = new MapTile()
                                    {
                                        TileMapY = (ushort)((flipY ? tileSetFrames.Height - y - 1 : y) * tileSetFrames.Width + (flipX ? tileSetFrames.Width - x - 1 : x)),
                                        HorizontalFlip = flipX,
                                        VerticalFlip = flipY
                                    };
                                }
                            }
                        }
                    }

                    maps = new Unity_Map[]
                    {
                        // Cavern
                        new Unity_Map
                        {
                            Width = (ushort)(tileSetFrames.Width * 2),
                            Height = (ushort)(tileSetFrames.Height * 2),
                            TileSet = new Unity_TileSet[]
                            {
                                LoadMode7FramesTileSet(tileSetFrames, tilePal, false)
                            },
                            MapTiles = cavernMapTiles.Select(t => new Unity_Tile(t)).ToArray(),
                            Type = Unity_Map.MapType.Graphics,
                        }
                    };
                }
                // Air
                else
                {
                    var paddingHeight = levelInfo.Crash1_Background.Height; // Note: It should be slightly less, but we can only change it in steps by cellsize (8) which would be too much
                    var cloudsPadding = Enumerable.Range(0, tileSetFrames.Width * paddingHeight).Select(t => new Unity_Tile(new MapTile()));

                    maps = new Unity_Map[]
                    {
                        // Sky
                        new Unity_Map()
                        {
                            Width = levelInfo.Crash1_Background.Width,
                            Height = levelInfo.Crash1_Background.Height,
                            TileSet = new Unity_TileSet[]
                            {
                                LoadMode7BackgroundTileSet(levelInfo.Crash1_Background)
                            },
                            MapTiles = levelInfo.Crash1_Background.TileMap.Select(t => new Unity_Tile(t)).ToArray(),
                            Type = Unity_Map.MapType.Graphics,
                        }, 

                        // Clouds
                        new Unity_Map
                        {
                            Width = tileSetFrames.Width,
                            Height = (ushort)(tileSetFrames.Height + paddingHeight),
                            TileSet = new Unity_TileSet[]
                            {
                                LoadMode7FramesTileSet(tileSetFrames, tilePal, true)
                            },
                            MapTiles = cloudsPadding.Concat(Enumerable.Range(0, tileSetFrames.Width * tileSetFrames.Height).Select(t => new Unity_Tile(new MapTile()
                            {
                                TileMapY = (ushort)(t + 1)
                            }))).ToArray(),
                            Type = Unity_Map.MapType.Graphics,
                        }
                    };
                }
            }
            else if (context.Settings.EngineVersion == EngineVersion.GBACrash_Crash2)
            {
                // Water
                if (levelInfo.LevelType == 0)
                {
                    const ushort skyWidth = 38;
                    const ushort skyHeight = 9;

                    var waterPadding = Enumerable.Range(0, tileSetFrames.Width * skyHeight).Select(t => new Unity_Tile(new MapTile()));

                    maps = new Unity_Map[]
                    {
                        // Sky
                        new Unity_Map
                        {
                            Width = skyWidth,
                            Height = skyHeight,
                            TileSet = new Unity_TileSet[]
                            {
                                LoadGenericTileSet(rom.Mode7_Crash2_Type0_BG1, tilePal, 1)
                            },
                            MapTiles = Enumerable.Range(0, skyWidth * skyHeight).Select(t => new Unity_Tile(new MapTile()
                            {
                                TileMapY = (ushort)t
                            })).ToArray(),
                            Type = Unity_Map.MapType.Graphics,
                        },

                        // Water
                        new Unity_Map
                        {
                            Width = tileSetFrames.Width,
                            Height = (ushort)(tileSetFrames.Height + skyHeight),
                            TileSet = new Unity_TileSet[]
                            {
                                LoadMode7FramesTileSet(tileSetFrames, tilePal, true)
                            },
                            MapTiles = waterPadding.Concat(Enumerable.Range(0, tileSetFrames.Width * tileSetFrames.Height).Select(t => new Unity_Tile(new MapTile()
                            {
                                TileMapY = (ushort)(t + 1)
                            }))).ToArray(),
                            Type = Unity_Map.MapType.Graphics,
                        }
                    };
                }
                // Space
                else
                {
                    maps = new Unity_Map[]
                    {
                        // Space (just make it all black for now since the stars appear to be generated dynamically)
                        new Unity_Map
                        {
                            Width = width,
                            Height = height,
                            TileSet = new Unity_TileSet[]
                            {
                                new Unity_TileSet(CellSize, Color.black), 
                            },
                            MapTiles = Enumerable.Range(0, width * height).Select(t => new Unity_Tile(new MapTile())).ToArray(),
                            Type = Unity_Map.MapType.Graphics,
                        },

                        // Fire
                        new Unity_Map
                        {
                            Width = width,
                            Height = height,
                            TileSet = new Unity_TileSet[]
                            {
                                LoadMode7FramesTileSet(rom.Mode7_Crash2_Type1_FlamesTileMaps, rom.Mode7_Crash2_Type1_FlamesTileSets, tilePal)
                            },
                            MapTiles = Enumerable.Range(0, width * height).Select(t => new Unity_Tile(new MapTile()
                            {
                                TileMapY = (ushort)t
                            })).ToArray(),
                            Type = Unity_Map.MapType.Graphics,
                        }
                    };
                }
            }

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            var objmanager = new Unity_ObjectManager_GBACrashMode7(context, LoadMode7AnimSets(levelInfo, tilePal));
            var objects = levelInfo.ObjData.Objects.Select(x => new Unity_Object_GBACrashMode7(objmanager, x));

            // Spawn the chase object for type 0
            if (levelInfo.LevelType == 0)
                objects = objects.Append(new Unity_Object_GBACrashMode7(objmanager, new GBACrash_Mode7_Object()
                {
                    ObjType_Normal = (byte)(objmanager.AnimSets.Length - 1),
                    ZPos = -5 // Have it start a bit behind the player
                }));

            // Spawn the main character (always type 0)
            objects = objects.Append(new Unity_Object_GBACrashMode7(objmanager, new GBACrash_Mode7_Object()));

            return new Unity_Level(
                maps: maps,
                objManager: objmanager,
                eventData: new List<Unity_Object>(objects),
                isometricData: new Unity_IsometricData
                {
                    CollisionWidth = 0,
                    CollisionHeight = 0,
                    TilesWidth = 38,
                    TilesHeight = 24,
                    Collision = null,
                    Scale = Vector3.one / 2,
                    ViewAngle = Quaternion.identity,
                    CalculateYDisplacement = () => 0,
                    CalculateXDisplacement = () => 0,
                    ObjectScale = new Vector3(1,1,0.5f) * CellSize
                },
                cellSize: CellSize);
        }

        public async UniTask<Unity_Level> LoadIsometricAsync(Context context, GBACrash_ROM rom)
        {
            var levelInfo = rom.CurrentIsometricLevelInfo;
            var objData = rom.CurrentIsometricObjData;

            Controller.DetailedState = "Loading maps & tilesets";
            await Controller.WaitIfNecessary();

            var tileSet = LoadIsometricTileSet(levelInfo.TileSet, levelInfo.TilePalette);

            var maps = levelInfo.MapLayers.Select((map, i) => new Unity_Map
            {
                Width = (ushort)(map.Width * 2),
                Height = (ushort)(map.Height * 2),
                TileSet = new Unity_TileSet[]
                {
                    tileSet,
                },
                MapTiles = GetIsometricTileMap(map, levelInfo.MapTiles),
                Type = Unity_Map.MapType.Graphics,
            }).Reverse().ToArray();

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            float minHeight = Mathf.Min(0, levelInfo.CollisionMap.Min(c => levelInfo.CollisionTiles[c].Height.AsFloat)); // TODO: include in height displacement calculation
            var collision = levelInfo.CollisionMap.Select(c => new Unity_IsometricCollisionTile() {
                Height = (levelInfo.CollisionTiles[c].Height - minHeight) * 3 // TODO: fix scale
            }).ToArray();
            var mirroredCollision = new Unity_IsometricCollisionTile[levelInfo.CollisionWidth * levelInfo.CollisionHeight];
            for (int x = 0; x < levelInfo.CollisionWidth; x++) {
                for (int y = 0; y < levelInfo.CollisionHeight; y++) {
                    mirroredCollision[x * levelInfo.CollisionHeight + y] = collision[y * levelInfo.CollisionWidth + x];
                }
            }

            var objManager = new Unity_ObjectManager_GBACrashIsometric(context, LoadIsometricAnimations(rom));
            var objects = objData.Objects.Select(x => new Unity_Object_GBACrashIsometric(x, objManager));

            return new Unity_Level(
                maps: maps,
                objManager: new Unity_ObjectManager(context),
                eventData: new List<Unity_Object>(objects),
                cellSize: CellSize,
                isometricData: new Unity_IsometricData()
                {
                    CollisionWidth = levelInfo.CollisionHeight,
                    CollisionHeight = levelInfo.CollisionWidth,
                    TilesWidth = levelInfo.MapWidth * CellSize,
                    TilesHeight = levelInfo.MapHeight * CellSize,
                    Collision = mirroredCollision,
                    Scale = new Vector3(Mathf.Sqrt(16), 2f / Mathf.Cos(Mathf.Deg2Rad * 30f), Mathf.Sqrt(16)),
                    ObjectScale = Vector3.one / 5.33f
                });
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

        public Unity_TileSet LoadGenericTileSet(byte[] tileSet, RGBA5551Color[] pal, int palIndex)
        {
            var palettes = Util.ConvertAndSplitGBAPalette(pal);

            var tex = Util.ToTileSetTexture(tileSet, palettes[palIndex], Util.TileEncoding.Linear_4bpp, CellSize, false);

            return new Unity_TileSet(tex, CellSize);
        }

        public Unity_TileSet LoadMode7FramesTileSet(GBACrash_Mode7_TileFrames tileFrames, RGBA5551Color[] pal, bool prependTransparent)
        {
            var palettes = Util.ConvertAndSplitGBAPalette(pal);

            var completeTileSet = tileFrames.TileFrames.SelectMany(x => x.TileSet).ToArray();

            if (prependTransparent)
                completeTileSet = new byte[0x20].Concat(completeTileSet).ToArray();

            var tex = Util.ToTileSetTexture(completeTileSet, palettes[0], Util.TileEncoding.Linear_4bpp, CellSize, false, getPalFunc: tileIndex =>
            {
                if (!tileFrames.HasPaletteIndices)
                    return palettes[0];

                var frameTilesCount = tileFrames.Width * tileFrames.Height;
                var frameIndex = Mathf.FloorToInt(tileIndex / (float)frameTilesCount);
                var frameTileIndex = tileIndex % frameTilesCount;
                var frame = tileFrames.TileFrames[frameIndex];
                var palIndexByte = frame.PaletteIndices[Mathf.FloorToInt(frameTileIndex / 2f)];

                return palettes[BitHelpers.ExtractBits(palIndexByte, 4, frameTileIndex % 2 == 0 ? 0 : 4)];
            });

            var length = tileFrames.Width * tileFrames.Height;

            return new Unity_TileSet(tex, CellSize)
            {
                AnimatedTiles = Enumerable.Range(0, length).Select(t => new Unity_AnimatedTile
                {
                    AnimationSpeed = 1,
                    TileIndices = Enumerable.Range(0, tileFrames.TileFrames.Length).Select(f => f * length + t + (prependTransparent ? 1 : 0)).ToArray()
                }).ToArray()
            };
        }

        public Unity_TileSet LoadMode7FramesTileSet(MapTile[][] tileMaps, byte[][] tileSets, RGBA5551Color[] pal)
        {
            var palettes = Util.ConvertAndSplitGBAPalette(pal);

            const int width = 240 / CellSize;
            const int height = 160 / CellSize;
            int framesCount = tileMaps.Length;

            var tex = TextureHelpers.CreateTexture2D(width * CellSize, height * CellSize * framesCount);

            for (int frameIndex = 0; frameIndex < framesCount; frameIndex++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var mapTile = tileMaps[frameIndex][y * width + x];

                        tex.FillInTile(
                            imgData: tileSets[frameIndex],
                            imgDataOffset: mapTile.TileMapY * 0x20,
                            pal: palettes[mapTile.PaletteIndex],
                            encoding: Util.TileEncoding.Linear_4bpp,
                            tileWidth: CellSize,
                            flipTextureY: false,
                            tileX: x * CellSize,
                            tileY: y * CellSize + frameIndex * height * CellSize,
                            flipTileX: mapTile.HorizontalFlip,
                            flipTileY: mapTile.VerticalFlip);
                    }
                }
            }

            return new Unity_TileSet(tex, CellSize)
            {
                AnimatedTiles = Enumerable.Range(0, width * height).Select(t => new Unity_AnimatedTile
                {
                    AnimationSpeed = 2,
                    TileIndices = Enumerable.Range(0, framesCount).Select(f => f * width * height + t + 0).ToArray()
                }).ToArray()
            };
        }

        public Unity_TileSet LoadMode7BackgroundTileSet(GBACrash_Mode7_Background background)
        {
            var pal = Util.ConvertAndSplitGBAPalette(background.Palette);

            int[] paletteIndices = new int[background.TileSetCount];

            for (var tileIndex = 0; tileIndex < background.TileMap.Length; tileIndex++)
            {
                var mt = background.TileMap[tileIndex];
                paletteIndices[mt.TileMapY] = BitHelpers.ExtractBits(background.PaletteIndices[Mathf.FloorToInt(tileIndex / 2f)], 4, tileIndex % 2 == 0 ? 0 : 4);
            }

            var tex = Util.ToTileSetTexture(background.TileSet, pal[0], Util.TileEncoding.Linear_4bpp, CellSize, false, getPalFunc: x => pal[paletteIndices[x]]);

            return new Unity_TileSet(tex, CellSize);
        }

        public Unity_TileSet LoadIsometricTileSet(GBACrash_Isometric_TileSet tileSet, RGBA5551Color[] tilePal)
        {
            var pal = Util.ConvertGBAPalette(tilePal);

            // The game converts the 4bpp tileset to an 8bpp tileset using the convert data

            var convertedTileSet = new byte[tileSet.TileSetCount_4bpp * 0x40];

            for (int i = 0; i < tileSet.TileSetCount_4bpp; i++)
            {
                var convertData = tileSet.TileSet_4bpp_ConvertDatas[tileSet.TileSet_4bpp_ConvertIndexTable[i]];

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        var index = y * 8 + x;

                        var b = convertData.Data[BitHelpers.ExtractBits(tileSet.TileSet_4bpp[i * 0x20 + Mathf.FloorToInt(index / 2f)], 4, index % 2 == 0 ? 0 : 4)];
                        convertedTileSet[i * 0x40 + (y * 8 + (8 - x - 1))] = b;
                    }
                }
            }

            var tex = Util.ToTileSetTexture(convertedTileSet.Concat(tileSet.TileSet_8bpp).ToArray(), pal, Util.TileEncoding.Linear_8bpp, CellSize, false);

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

        public Unity_Tile[] GetIsometricTileMap(GBACrash_Isometric_MapLayer mapLayer, MapTile[] mapTiles)
        {
            var tileMap = new Unity_Tile[mapLayer.Width * 2 * mapLayer.Height * 2];

            for (int y = 0; y < mapLayer.Height; y++)
            {
                var chunk = mapLayer.TileMapRows[y];
                var x = 0;

                foreach (var cmd in chunk.Commands)
                {
                    if (cmd.Type == 3)
                    {
                        for (var i = 0; i < cmd.Params.Length; i++)
                        {
                            setTile(cmd.Params[i], i, cmd.Params.Length);
                            x++;
                        }
                    }
                    else if (cmd.Type == 2)
                    {
                        for (int i = 0; i < cmd.Length; i++)
                        {
                            setTile(cmd.Param, i, cmd.Length);
                            x++;
                        }
                    }
                    else
                    {
                        setTile(cmd.Length, 0, 1);
                        x++;
                    }

                    void setTile(ushort value, int index, int length)
                    {
                        var actualX = x * 2;
                        var actualY = y * 2;

                        setTileAt(0, 0, mapTiles[value * 4 + 0]);
                        setTileAt(1, 0, mapTiles[value * 4 + 1]);
                        setTileAt(0, 1, mapTiles[value * 4 + 2]);
                        setTileAt(1, 1, mapTiles[value * 4 + 3]);

                        void setTileAt(int offX, int offY, MapTile tile)
                        {
                            var outputX = actualX + offX;
                            var outputY = actualY + offY;

                            tileMap[outputY * mapLayer.Width * 2 + outputX] = new Unity_Tile(tile)
                            {
                                DebugText = $"CMD: {cmd.Type}{Environment.NewLine}" +
                                            $"Value: {value}{Environment.NewLine}" +
                                            $"Index: {index}{Environment.NewLine}" +
                                            $"Length: {length}{Environment.NewLine}" +
                                            $"Tile: {tile.TileMapY}{Environment.NewLine}" +
                                            $"FlipX: {tile.HorizontalFlip}{Environment.NewLine}" +
                                            $"FlipY: {tile.VerticalFlip}{Environment.NewLine}"
                            };
                        }
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
                xPos: anim.RenderBox.X,
                yPos: anim.RenderBox.Y
            )).ToArray())).ToArray();
        }

        public Unity_ObjectManager_GBACrashMode7.AnimSet[] LoadMode7AnimSets(GBACrash_Mode7_LevelInfo level, RGBA5551Color[] tilePal)
        {
            var pal = Util.ConvertAndSplitGBAPalette(level.ObjPalette.Concat(tilePal).ToArray());

            return level.GetAllAnimSets.Select(animSet => new Unity_ObjectManager_GBACrashMode7.AnimSet(
                animations: animSet.Animations?.Select((anim, i) => new Unity_ObjectManager_GBACrashMode7.AnimSet.Animation(GetMode7AnimFrames(animSet, i, pal).Select(frame => frame.CreateSprite()).ToArray())).ToArray() ?? new Unity_ObjectManager_GBACrashMode7.AnimSet.Animation[0]
                )).ToArray();
        }

        public Unity_ObjectManager_GBACrashIsometric.GraphicsData[] LoadIsometricAnimations(GBACrash_ROM rom)
        {
            var pal = Util.ConvertAndSplitGBAPalette(rom.Isometric_GetObjPalette);

            return rom.Isometric_GetAnimations.Select(anim => new Unity_ObjectManager_GBACrashIsometric.GraphicsData(
                animFrames: GetIsometricAnimFrames(anim, pal).Select(x => x.CreateSprite()).ToArray(),
                animSpeed: 4)).ToArray();
        }

        public Texture2D[] GetAnimFrames(GBACrash_AnimSet animSet, int animIndex, byte[] tileSet, Color[] pal)
        {
            var shapes = TileShapes;

            var anim = animSet.Animations[animIndex];
            var frames = anim.FrameTable.Select(x => animSet.AnimationFrames[x]).ToArray();

            if (!frames.Any())
                return new Texture2D[0];

            var output = new Texture2D[frames.Length];

            var minX = anim.RenderBox.X;
            var minY = anim.RenderBox.Y;

            var width = anim.RenderBox.Width + 1;
            var height = anim.RenderBox.Height + 1;

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

        public Texture2D[] GetMode7AnimFrames(GBACrash_Mode7_AnimSet animSet, int animIndex, Color[][] pal)
        {
            var anim = animSet.Animations[animIndex];
            var frames = animSet.ObjFrames.Skip(anim.FrameIndex).Take(anim.FramesCount).ToArray();

            var output = new Texture2D[frames.Length];

            for (int frameIndex = 0; frameIndex < frames.Length; frameIndex++)
            {
                var frame = frames[frameIndex];

                output[frameIndex] = Util.ToTileSetTexture(frame.TileSet, pal[animSet.PaletteIndex], Util.TileEncoding.Linear_4bpp, CellSize, true, wrap: frame.Width);
            }

            return output;
        }

        public Texture2D[] GetIsometricAnimFrames(GBACrash_Isometric_Animation anim, Color[][] palette)
        {
            if (anim.AnimFrames[0] == null)
                return new Texture2D[0];

            var output = new Texture2D[anim.AnimFrames.Length];
            var pal = anim.Palette != null ? Util.ConvertGBAPalette(anim.Palette) : palette[anim.PaletteIndex];

            for (int frameIndex = 0; frameIndex < anim.AnimFrames.Length; frameIndex++)
                output[frameIndex] = Util.ToTileSetTexture(anim.AnimFrames[frameIndex], pal, Util.TileEncoding.Linear_4bpp, CellSize, true, wrap: anim.Width / CellSize);

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
            public LevInfo(GBACrash_MapInfo.GBACrash_MapType specialMapType, short index3D, string displayName)
            {
                LevelIndex = -1;
                MapIndex = -1;
                SpecialMapType = specialMapType;
                Index3D = index3D;
                DisplayName = displayName;
            }

            public int LevelIndex { get; }
            public int MapIndex { get; }
            public Type MapType { get; }
            public GBACrash_MapInfo.GBACrash_MapType SpecialMapType { get; }
            public short Index3D { get; }

            public string DisplayName { get; set; }

            public enum Type
            {
                Normal,
                Bonus,
                Challenge
            }
        }

        public Vector2Int[] TileShapes { get; } = new Vector2Int[]
        {
            new Vector2Int(0x08, 0x08), 
            new Vector2Int(0x10, 0x10), 
            new Vector2Int(0x20, 0x20), 
            new Vector2Int(0x40, 0x40), 
            new Vector2Int(0x10, 0x08), 
            new Vector2Int(0x20, 0x08), 
            new Vector2Int(0x20, 0x10), 
            new Vector2Int(0x40, 0x20), 
            new Vector2Int(0x08, 0x10), 
            new Vector2Int(0x08, 0x20), 
            new Vector2Int(0x10, 0x20), 
            new Vector2Int(0x20, 0x40), 
        };
    }
}