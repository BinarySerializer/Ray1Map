using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.GBA;
using Cysharp.Threading.Tasks;

using UnityEngine;

namespace R1Engine
{
    public abstract class GBAVV_Generic_BaseManager : GBAVV_BaseManager
    {
        // Tools
        public override async UniTask ExportAnimFramesAsync(GameSettings settings, string outputDir, bool saveAsGif, bool includePointerInNames = true)
        {
            // Export 2D animations
            await base.ExportAnimFramesAsync(settings, outputDir, saveAsGif, includePointerInNames);

            var exportedAnimSets = new HashSet<(Pointer, Pointer)>();

            // Export Mode7 animations
            for (int mode7Level = 0; mode7Level < Mode7LevelsCount; mode7Level++)
            {
                using (var context = new R1Context(settings))
                {
                    // Load the files
                    await LoadFilesAsync(context);

                    // Read the rom
                    var rom = LoadROMForMode7Export(context, mode7Level);

                    if (rom == null)
                        continue;

                    var levInfo = rom.CurrentMode7LevelInfo;

                    // For Crash 1 we skip type 1 (blimp levels) and use type 2 for the export instead (N. Gin) since it uses correct palettes
                    if (settings.EngineVersion == EngineVersion.GBAVV_Crash1 && levInfo.LevelType == 1)
                        continue;

                    var tilePal = rom.Mode7_GetTilePal(levInfo);
                    var pal = Util.ConvertAndSplitGBAPalette(levInfo.ObjPalette.Concat(tilePal).ToArray());
                    var animSetIndex = -1;

                    // Enumerate every anim set
                    foreach (var animSet in levInfo.GetAllAnimSets)
                    {
                        animSetIndex++;

                        if (animSet?.Animations == null || exportedAnimSets.Any(x => x.Item1.AbsoluteOffset == animSet.AnimationsPointer.AbsoluteOffset && x.Item2.AbsoluteOffset == animSet.FrameOffsetsPointer.AbsoluteOffset))
                            continue;

                        exportedAnimSets.Add((animSet.AnimationsPointer, animSet.FrameOffsetsPointer));

                        for (int animIndex = 0; animIndex < animSet.Animations.Length; animIndex++)
                        {
                            await Controller.WaitFrame();
                            var frames = GetMode7AnimFrames(animSet, animSetIndex, animIndex, pal, levInfo);

                            if (frames.Length == 0)
                                continue;

                            Util.ExportAnim(
                                frames: frames,
                                speed: 4,
                                center: true,
                                saveAsGif: saveAsGif,
                                outputDir: Path.Combine(outputDir, "Mode7"),
                                primaryName: $"{mode7Level}_{animSetIndex}{(animSet.Offset != null ? $"_0x{animSet.Offset.StringAbsoluteOffset}" : "")}",
                                secondaryName: $"{animIndex}");
                        }
                    }

                    // Export special frames
                    if (levInfo.SpecialFrames != null && exportedAnimSets.All(x => x.Item1.AbsoluteOffset != levInfo.SpecialFrames.Offset.AbsoluteOffset))
                    {
                        exportedAnimSets.Add((levInfo.SpecialFrames.Offset, null));

                        Util.ExportAnim(
                            frames: GetMode7SpecialAnimFrames(levInfo.SpecialFrames),
                            speed: 4,
                            center: true,
                            saveAsGif: saveAsGif,
                            outputDir: Path.Combine(outputDir, "Mode7"),
                            primaryName: $"{mode7Level}_0x{levInfo.SpecialFrames.Offset.StringAbsoluteOffset}",
                            secondaryName: $"0");
                    }
                }
            }

            Debug.Log($"Finished exporting Mode7 animations");
        }
        public abstract GBAVV_ROM_Generic LoadROMForMode7Export(Context context, int level);

        // Map2D
        public async UniTask<Unity_Level> LoadMap2DAsync(Context context, GBAVV_BaseROM rom, GBAVV_Generic_MapInfo map, int theme = -1, bool hasAssignedObjTypeGraphics = true)
        {
            Controller.DetailedState = "Loading tilesets";
            await Controller.WaitIfNecessary();

            // Load tilemaps
            var tileMaps = map.MapData2D.MapLayers.Select((x, i) =>
            {
                if (x == null)
                    return null;

                return GetTileMap(x, map.MapData2D.LayersBlock.TileLayerDatas[i], i == 3);
            }).ToArray();

            var tileSets = new Dictionary<GBAVV_Map2D_TileSetBlock, Unity_TileSet>();

            // Load tilesets
            foreach (var tileSetGroup in map.MapData2D.MapLayers.GroupBy(x => x?.TileSet).Where(x => x?.Key != null))
            {
                tileSets.Add(tileSetGroup.Key, LoadTileSet(
                    tileSet: tileSetGroup.Key.TileSet,
                    pal: map.TilePalette2D,
                    is8bit: map.MapData2D.MapLayers[3]?.TileSet == tileSetGroup.Key,
                    engineVersion: context.GetR1Settings().EngineVersion,
                    levelTheme: theme,
                    mapTiles_4: map.MapData2D.MapLayers.Select((x, i) => new
                    {
                        Tiles = tileMaps[i],
                        TileSet = x?.TileSet
                    }).Where(x => x.TileSet == tileSetGroup.Key).SelectMany(x => x.Tiles.Select(t => t.Data)).ToArray(),
                    paletteShifts: map.SpongeBob_PaletteShifts));
            }

            Controller.DetailedState = "Loading maps";
            await Controller.WaitIfNecessary();

            // Load map layers
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
                        MapTiles = tileMaps[i],
                        Type = Unity_Map.MapType.Graphics,
                        Layer = x.LayerPrio == (4 - map.MapData2D.MapLayers.Count(y => y != null)) && i != 3 ? Unity_Map.MapLayer.Front : Unity_Map.MapLayer.Middle,
                        // TODO: Is this correct?
                        Alpha = i == 2 && map.Alpha_BG3 < 0x10 && map.Alpha_BG3 != 0
                            ? map.Alpha_BG3 / 16f
                            : i == 1 && map.Alpha_BG2 < 0x10 && map.Alpha_BG2 != 0
                                ? map.Alpha_BG2 / 16f
                                : (float?)null
                    },
                    Prio = x.LayerPrio
                };
            }).Where(x => x != null).OrderByDescending(x => x.Prio).Select(x => x.Map).Append(new Unity_Map()
            {
                Width = map.MapData2D.CollisionLayer.MapWidth,
                Height = map.MapData2D.CollisionLayer.MapHeight,
                TileSet = new Unity_TileSet[0],
                MapTiles = GetTileMap(map.MapData2D.CollisionLayer, map.MapData2D.LayersBlock.CollisionLayerData, isCollision: true),
                Type = Unity_Map.MapType.Collision,
            }).ToArray();

            Controller.DetailedState = "Loading localization";
            await Controller.WaitIfNecessary();

            // Load localization
            var loc = LoadLocalization(rom);

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            // Load objects
            var objmanager = new Unity_ObjectManager_GBAVV(
                context: context, 
                animSets: LoadAnimSets(rom), 
                objData: map.MapData2D.ObjData, 
                mapType: map.Crash_MapType, 
                locPointerTable: loc.Item2,
                addDummyAnimSet: !hasAssignedObjTypeGraphics);
            var objects = map.MapData2D.ObjData.ObjGroups.SelectMany((x, groupIndex) => x.Objects.Reverse().Select((obj, i) => new Unity_Object_GBAVV(objmanager, obj, groupIndex, i)));

            return new Unity_Level()
            {
                Maps = maps,
                ObjManager = objmanager,
                EventData = new List<Unity_SpriteObject>(objects),
                CellSize = CellSize,
                GetCollisionTypeGraphicFunc = x => ((GBAVV_Map2D_CollisionType)x).GetCollisionTypeGraphic(),
                GetCollisionTypeNameFunc = x => ((GBAVV_Map2D_CollisionType)x).ToString(),
                Localization = loc.Item1
            };
        }
        public Unity_Tile[] GetTileMap(GBAVV_Map2D_MapLayer layer, GBAVV_Map2D_LayersBlock.GBAVV_TileLayerData tileLayerData, bool is8bit = false, bool isCollision = false)
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
                                GBAVV_CollisionShape = (MapTile.GBAVV_CollisionTileShape)BitHelpers.ExtractBits(tileIndex, 7, 0),
                                GBAVV_UnknownCollisionFlag = BitHelpers.ExtractBits(tileIndex, 1, 7) == 1, // Only used in Crash 1
                                CollisionType = (ushort)BitHelpers.ExtractBits(tileIndex, 4, 8),
                                GBAVV_UnknownData = (byte)BitHelpers.ExtractBits(tileIndex, 4, 12), // Only used in Crash 1 - set for tiles under diagonal ones
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
                                TileMapY = (ushort)BitHelpers.ExtractBits(tileIndex, 10, 0),
                                PaletteIndex = (byte)BitHelpers.ExtractBits(tileIndex, 4, 12),
                                HorizontalFlip = BitHelpers.ExtractBits(tileIndex, 1, 10) == 1,
                                VerticalFlip = BitHelpers.ExtractBits(tileIndex, 1, 11) == 1,
                            };
                        }

                        tileMap[tileMapIndex] = new Unity_Tile(mapTile)
                        {
                            DebugText = Settings.ShowDebugInfo ? ($"CMD: {cmd}{Environment.NewLine}" +
                                                                  $"Index: {blockIndex}{Environment.NewLine}" +
                                                                  $"TileIndex: {tileIndex}{Environment.NewLine}") : null
                        };
                    }
                }
            }

            return tileMap;
        }

        // Mode7
        public async UniTask<Unity_Level> LoadMode7Async(Context context, GBAVV_ROM_Generic rom, GBAVV_Mode7_LevelInfo levelInfo)
        {
            var tileSetFrames = levelInfo.TileSetFrames;

            Controller.DetailedState = "Loading maps & tilesets";
            await Controller.WaitIfNecessary();

            const int width = GBAConstants.ScreenWidth / CellSize;
            const int height = GBAConstants.ScreenHeight / CellSize;

            var tilePal = rom.Mode7_GetTilePal(levelInfo);

            Unity_Map[] maps = null;

            // TODO: Do not hard-code this
            if (context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash1)
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
            else if (context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_SpongeBobRevengeOfTheFlyingDutchman)
            {
                var paddingHeight = levelInfo.Crash1_Background.Height - tileSetFrames.Height;
                var padding = Enumerable.Range(0, tileSetFrames.Width * 2 * paddingHeight).Select(t => new Unity_Tile(new MapTile()));

                var cavernMapTiles = new MapTile[tileSetFrames.Width * 2 * tileSetFrames.Height];

                // Mirror the cavern background
                for (int blockY = 0; blockY < 1; blockY++)
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
                    // Background
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

                    // Cavern
                    new Unity_Map
                    {
                        Width = (ushort)(tileSetFrames.Width * 2),
                        Height = (ushort)(tileSetFrames.Height + paddingHeight),
                        TileSet = new Unity_TileSet[]
                        {
                            LoadMode7FramesTileSet(tileSetFrames, tilePal, true)
                        },
                        MapTiles = padding.Concat(cavernMapTiles.Select(t => new Unity_Tile(new MapTile()
                        {
                            TileMapY = (ushort)(t.TileMapY + 1),
                            HorizontalFlip = t.HorizontalFlip,
                            VerticalFlip = t.VerticalFlip
                        }))).ToArray(),
                        Type = Unity_Map.MapType.Graphics,
                    }
                };
            }
            else if (context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash2)
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

            Controller.DetailedState = "Loading localization";
            await Controller.WaitIfNecessary();

            var loc = LoadLocalization(rom);

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            var objmanager = new Unity_ObjectManager_GBAVVMode7(context, LoadMode7AnimSets(levelInfo, tilePal));
            var objects = levelInfo.ObjData.Objects.Select(x => new Unity_Object_GBAVVMode7(objmanager, x));

            // Spawn the chase object for type 0 or special object for Crash 1 (blimp or N. Gin) // TODO: Spawn blimps at correct positions - array at 0x0817a420?
            if ((levelInfo.LevelType == 0 || context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash1) && 
                context.GetR1Settings().EngineVersion != EngineVersion.GBAVV_SpongeBobRevengeOfTheFlyingDutchman)
                objects = objects.Append(new Unity_Object_GBAVVMode7(objmanager, new GBAVV_Mode7_Object()
                {
                    ObjType_Normal = (byte)(objmanager.AnimSets.Length - 1),
                    ObjType_TimeTrial = (byte)(objmanager.AnimSets.Length - 1),
                    ObjType_Unknown = (byte)(objmanager.AnimSets.Length - 1),
                    ZPos = -50 // Have it start a bit behind the player
                }));
            else if (context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_SpongeBobRevengeOfTheFlyingDutchman)
            {
                objects = objects.Append(new Unity_Object_GBAVVMode7(objmanager, new GBAVV_Mode7_Object()
                {
                    ObjType_Normal = 32,
                    ObjType_TimeTrial = 32,
                    ObjType_Unknown = 32,
                    ZPos = -5 // Have it start a bit behind the player
                }));
            }

            // Spawn the main character (always type 0)
            objects = objects.Append(new Unity_Object_GBAVVMode7(objmanager, new GBAVV_Mode7_Object()));

            return new Unity_Level()
            {
                Maps = maps,
                ObjManager = objmanager,
                EventData = new List<Unity_SpriteObject>(objects),
                IsometricData = new Unity_IsometricData
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
                    ObjectScale = new Vector3(1, 1, 0.5f) * CellSize
                },
                CellSize = CellSize,
                Localization = loc.Item1,
                TrackManager = new Unity_TrackManager_ObjectsLinear(levelInfo.LevelType == 0),
            };
        }
        public virtual int[] Mode7AnimSetCounts => new int[0];
        public virtual int Mode7LevelsCount => 0;

        // Mode7 tileset
        public Unity_TileSet LoadMode7FramesTileSet(GBAVV_Mode7_TileFrames tileFrames, RGBA5551Color[] pal, bool prependTransparent)
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
                    AnimationSpeed = 2,
                    TileIndices = Enumerable.Range(0, tileFrames.TileFrames.Length).Select(f => f * length + t + (prependTransparent ? 1 : 0)).ToArray()
                }).ToArray()
            };
        }
        public Unity_TileSet LoadMode7FramesTileSet(MapTile[][] tileMaps, byte[][] tileSets, RGBA5551Color[] pal)
        {
            var palettes = Util.ConvertAndSplitGBAPalette(pal);

            const int width = GBAConstants.ScreenWidth / CellSize;
            const int height = GBAConstants.ScreenHeight / CellSize;
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
                    AnimationSpeed = 4,
                    TileIndices = Enumerable.Range(0, framesCount).Select(f => f * width * height + t + 0).ToArray()
                }).ToArray()
            };
        }
        public Unity_TileSet LoadMode7BackgroundTileSet(GBAVV_Mode7_Background background)
        {
            if (background.PaletteIndices == null)
            {
                var pal = Util.ConvertGBAPalette(background.Palette);

                var tex = Util.ToTileSetTexture(background.TileSet, pal, Util.TileEncoding.Linear_8bpp, CellSize, false);

                return new Unity_TileSet(tex, CellSize);
            }
            else
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
        }

        // Mode7 animations
        public Unity_ObjectManager_GBAVVMode7.AnimSet[] LoadMode7AnimSets(GBAVV_Mode7_LevelInfo level, RGBA5551Color[] tilePal)
        {
            var pal = Util.ConvertAndSplitGBAPalette(level.ObjPalette.Concat(tilePal).ToArray());

            var animSets = level.GetAllAnimSets.Select((animSet, animSetIndex) =>
            {
                var animations = animSet.Animations?.Where(x => x.FrameIndex + x.FramesCount <= animSet.ObjFrames.Length).Select((anim, i) =>
                {
                    var frames = GetMode7AnimFrames(animSet, animSetIndex, i, pal, level);
                    return new Unity_ObjectManager_GBAVVMode7.AnimSet.Animation(frames.Select(frame => frame.CreateSprite()).ToArray());
                }).ToArray() ?? new Unity_ObjectManager_GBAVVMode7.AnimSet.Animation[0];

                return new Unity_ObjectManager_GBAVVMode7.AnimSet(animations, animSet);
            });

            // Load special frames animation
            if (level.SpecialFrames != null)
            {
                animSets = animSets.Append(new Unity_ObjectManager_GBAVVMode7.AnimSet(new Unity_ObjectManager_GBAVVMode7.AnimSet.Animation[]
                {
                    new Unity_ObjectManager_GBAVVMode7.AnimSet.Animation(GetMode7SpecialAnimFrames(level.SpecialFrames).Select(x => x.CreateSprite()).ToArray()),
                }, null));
            }

            return animSets.ToArray();
        }
        public Texture2D[] GetMode7AnimFrames(GBAVV_Mode7_AnimSet animSet, int animSetIndex, int animIndex, Color[][] pal, GBAVV_Mode7_LevelInfo levInfo)
        {
            var palette = pal;

            if (levInfo.Context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash1 && animSetIndex == 0 && animIndex == 5 && levInfo.LevelType == 0)
                palette = Util.ConvertAndSplitGBAPalette(levInfo.Crash1_PolarDeathPalette);

            var anim = animSet.Animations[animIndex];
            var frames = animSet.ObjFrames.Skip(anim.FrameIndex).Take(anim.FramesCount).ToArray();

            var output = new Texture2D[frames.Length];

            for (int frameIndex = 0; frameIndex < frames.Length; frameIndex++)
            {
                var frame = frames[frameIndex];

                output[frameIndex] = Util.ToTileSetTexture(frame.TileSet, palette[animSet.PaletteIndex], Util.TileEncoding.Linear_4bpp, CellSize, true, wrap: frame.Width);
            }

            return output;
        }
        public Texture2D[] GetMode7SpecialAnimFrames(GBAVV_Mode7_SpecialFrames frames)
        {
            var output = new Texture2D[frames.FramesCount];
            var palette = Util.ConvertAndSplitGBAPalette(frames.Palette);
            var tileSet = new byte[0];

            for (int i = 0; i < frames.FramesCount; i++)
            {
                var frame = frames.Frames[i];

                var tex = TextureHelpers.CreateTexture2D(frames.Width * CellSize, frames.Height * CellSize);

                tileSet = tileSet.Concat(frame.TileSet).ToArray();

                for (int y = 0; y < frames.Height; y++)
                {
                    for (int x = 0; x < frames.Width; x++)
                    {
                        var tile = frame.TileMap[y * frames.Width + x];

                        tex.FillInTile(
                            imgData: tileSet,
                            imgDataOffset: tile.TileMapY * 0x20,
                            pal: palette[tile.PaletteIndex],
                            encoding: Util.TileEncoding.Linear_4bpp,
                            tileWidth: CellSize,
                            flipTextureY: true,
                            tileX: x * CellSize,
                            tileY: y * CellSize);
                    }
                }

                tex.Apply();

                output[i] = tex;
            }

            return output;
        }
    }
}