using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace R1Engine
{
    public abstract class GBAVV_BaseManager : IGameManager
    {
        public const string LocTableID = "LocTable";
        public const int CellSize = 8;
        public string GetROMFilePath => "ROM.gba";

        public abstract LevInfo[] LevInfos { get; }
        public virtual int LocTableCount => 0;
        public virtual int LanguagesCount => 1;

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, LevInfos.Length).ToArray()),
        });

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Animation Frames", false, true, (input, output) => ExportAnimFramesAsync(settings, output, false)),
            new GameAction("Export Animations as GIF", false, true, (input, output) => ExportAnimFramesAsync(settings, output, true)),
            new GameAction("Export isometric character icons", false, true, (input, output) => ExportIsometricCharacterIcons(settings, output)),
            new GameAction("Export level icons", false, true, (input, output) => ExportLevelIcons(settings, output)),
        };

        public virtual async UniTask ExportAnimFramesAsync(GameSettings settings, string outputDir, bool saveAsGif, bool includePointerInNames = true)
        {
            // Export 2D animations
            using (var context = new Context(settings))
            {
                // Load the files
                await LoadFilesAsync(context);

                // Read the rom
                var rom = FileFactory.Read<GBAVV_ROM>(GetROMFilePath, context, (s, d) => d.CurrentLevInfo = LevInfos.First());

                await UniTask.WaitForEndOfFrame();

                // Enumerate every graphics data
                for (int graphicsIndex = 0; graphicsIndex < rom.Map2D_Graphics.Length; graphicsIndex++)
                {
                    var graphicsData = rom.Map2D_Graphics[graphicsIndex];

                    // Enumerate every anim set
                    for (int animSetIndex = 0; animSetIndex < graphicsData.AnimSets.Length; animSetIndex++)
                    {
                        var animSet = graphicsData.AnimSets[animSetIndex];

                        // Enumerate every animation
                        for (var animIndex = 0; animIndex < animSet.Animations.Length; animIndex++)
                        {
                            await UniTask.WaitForEndOfFrame();

                            var anim = animSet.Animations[animIndex];
                            var frames = GetAnimFrames(animSet, animIndex, graphicsData.TileSet, graphicsData.Palettes);

                            if (!frames.Any())
                                continue;

                            var dirName = $"2D";
                            var animSetName = $"{animSetIndex}";
                            var animName = $"{animIndex}";

                            if (rom.Map2D_Graphics.Length > 1)
                                dirName += $"_{graphicsIndex}";

                            if (includePointerInNames)
                            {
                                dirName += $" - 0x{graphicsData.Offset.AbsoluteOffset:X8}";
                                animSetName += $" 0x{animSet.Offset.AbsoluteOffset:X8}";
                                animName += $" 0x{anim.Offset.AbsoluteOffset:X8}";
                            }

                            Util.ExportAnim(
                                frames: frames,
                                speed: anim.GetAnimSpeed,
                                center: false,
                                saveAsGif: saveAsGif,
                                outputDir: Path.Combine(outputDir, dirName),
                                primaryName: animSetName,
                                secondaryName: animName);
                        }
                    }
                }
            }

            // Export Mode7 animations
            if (!settings.GBAVV_IsFusion && settings.EngineVersion != EngineVersion.GBAVV_CrashNitroKart)
            {
                for (short mode7Level = 0; mode7Level < 7; mode7Level++)
                {
                    var exportedAnimSets = new HashSet<Pointer>();

                    using (var context = new Context(settings))
                    {
                        // Load the files
                        await LoadFilesAsync(context);

                        // Read the rom
                        var rom = FileFactory.Read<GBAVV_ROM>(GetROMFilePath, context, (s, d) => d.CurrentLevInfo = new LevInfo(GBAVV_MapInfo.GBAVV_MapType.Mode7, mode7Level, null));

                        var levInfo = rom.CurrentMode7LevelInfo;

                        var tilePal = rom.Mode7_GetTilePal(levInfo);
                        var pal = Util.ConvertAndSplitGBAPalette(levInfo.ObjPalette.Concat(tilePal).ToArray());
                        var animSetIndex = -1;

                        // Enumerate every anim set
                        foreach (var animSet in levInfo.GetAllAnimSets)
                        {
                            animSetIndex++;

                            if (animSet?.Animations == null || exportedAnimSets.Contains(animSet.AnimationsPointer))
                                continue;

                            exportedAnimSets.Add(animSet.AnimationsPointer);

                            for (int animIndex = 0; animIndex < animSet.Animations.Length; animIndex++)
                            {
                                await UniTask.WaitForEndOfFrame();
                                var frames = GetMode7AnimFrames(animSet, animSetIndex, animIndex, pal, levInfo);

                                Util.ExportAnim(
                                    frames: frames,
                                    speed: 4,
                                    center: true,
                                    saveAsGif: saveAsGif,
                                    outputDir: Path.Combine(outputDir, "Mode7"),
                                    primaryName: $"0x{animSet.AnimationsPointer.AbsoluteOffset:X8}",
                                    secondaryName: $"{animIndex}");
                            }
                        }

                        // Export special frames
                        if (levInfo.SpecialFrames != null && !exportedAnimSets.Contains(levInfo.SpecialFrames.Offset))
                        {
                            exportedAnimSets.Add(levInfo.SpecialFrames.Offset);

                            Util.ExportAnim(
                                frames: GetMode7SpecialAnimFrames(levInfo.SpecialFrames),
                                speed: 4,
                                center: true,
                                saveAsGif: saveAsGif,
                                outputDir: Path.Combine(outputDir, "Mode7"),
                                primaryName: $"0x{levInfo.SpecialFrames.Offset.AbsoluteOffset:X8}",
                                secondaryName: $"0");
                        }
                    }
                }
            }

            if (settings.EngineVersion == EngineVersion.GBAVV_Crash2)
            {
                // Export isometric animations
                using (var context = new Context(settings))
                {
                    // Load the files
                    await LoadFilesAsync(context);

                    // Read the rom
                    var rom = FileFactory.Read<GBAVV_ROM>(GetROMFilePath, context, (s, d) => d.CurrentLevInfo = new LevInfo(GBAVV_MapInfo.GBAVV_MapType.Isometric, 0, null));

                    var pal = Util.ConvertAndSplitGBAPalette(rom.Isometric_GetObjPalette);

                    var animations = rom.Isometric_GetAnimations.ToArray();

                    // Enumerate every animation
                    for (var i = 1; i < animations.Length; i++)
                    {
                        await UniTask.WaitForEndOfFrame();

                        var frames = GetIsometricAnimFrames(animations[i], pal);

                        Util.ExportAnim(
                            frames: frames, 
                            speed: 4, 
                            center: true, 
                            saveAsGif: saveAsGif, 
                            outputDir: Path.Combine(outputDir, "Isometric"), 
                            primaryName: $"{i}", 
                            secondaryName: $"0");
                    }
                }
            }

            Debug.Log($"Finished export");
        }

        public async UniTask ExportIsometricCharacterIcons(GameSettings settings, string outputDir)
        {
            using (var context = new Context(settings))
            {
                // Load the files
                await LoadFilesAsync(context);

                // Read the rom
                var rom = FileFactory.Read<GBAVV_ROM>(GetROMFilePath, context, (s, d) => d.CurrentLevInfo = new LevInfo(GBAVV_MapInfo.GBAVV_MapType.Isometric, 0, null));

                var pal = Util.ConvertAndSplitGBAPalette(rom.Isometric_GetObjPalette);

                // Enumerate every character
                for (int i = 0; i < rom.Isometric_CharacterIcons.Length; i++)
                {
                    var tex = Util.ToTileSetTexture(rom.Isometric_CharacterIcons[i].TileSet.TileSet, pal[rom.Isometric_CharacterIcons[i].PaletteIndex], Util.TileEncoding.Linear_4bpp, CellSize, true, wrap: 2);

                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{i:00}_{rom.Isometric_CharacterInfos[i].Name}.png"), tex.EncodeToPNG());
                }
            }
        }

        public async UniTask ExportLevelIcons(GameSettings settings, string outputDir)
        {
            using (var context = new Context(settings))
            {
                // Load the files
                await LoadFilesAsync(context);

                // Read the rom
                var rom = FileFactory.Read<GBAVV_ROM>(GetROMFilePath, context, (s, d) => d.CurrentLevInfo = new LevInfo(GBAVV_MapInfo.GBAVV_MapType.WorldMap, 0, null));

                // Enumerate every level icon
                for (int i = 0; i < rom.WorldMap_Crash1_LevelIcons.Length; i++)
                {
                    var tex = rom.WorldMap_Crash1_LevelIcons[i].ToTexture2D();

                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{i}.png"), tex.EncodeToPNG());
                }
            }
        }

        public virtual async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBAVV_ROM>(GetROMFilePath, context, (s, r) => r.CurrentLevInfo = LevInfos[context.Settings.Level]);
            var map = rom.CurrentMapInfo;

            if (map.MapType == GBAVV_MapInfo.GBAVV_MapType.Mode7)
                return await LoadMode7Async(context, rom);
            else if (map.MapType == GBAVV_MapInfo.GBAVV_MapType.Isometric)
                return await LoadIsometricAsync(context, rom);
            else if (map.MapType == GBAVV_MapInfo.GBAVV_MapType.WorldMap)
                return await LoadWorldMapAsync(context, rom);
            else if (map.MapType == GBAVV_MapInfo.GBAVV_MapType.Kart)
                return await LoadNitroKartAsync(context, rom);
            else
                return await Load2DAsync(context, rom);
        }

        public async UniTask<Unity_Level> Load2DAsync(Context context, GBAVV_ROM rom)
        {
            var map = rom.CurrentMapInfo;

            Controller.DetailedState = "Loading tilesets";
            await Controller.WaitIfNecessary();

            // Load tilemaps
            var tileMaps = map.MapData2D.MapLayers.Select((x, i) =>
            {
                if (x == null)
                    return null;

                return GetTileMap(x, map.MapData2D.LayersBlock.TileLayerDatas[i], i == 3, x.TileSet.TileSet.Length / 32);
            }).ToArray();

            var tileSets = new Dictionary<GBAVV_Map2D_TileSet, Unity_TileSet>();

            foreach (var tileSetGroup in map.MapData2D.MapLayers.GroupBy(x => x?.TileSet).Where(x => x?.Key != null))
            {
                tileSets.Add(tileSetGroup.Key, LoadTileSet(
                    tileSet: tileSetGroup.Key.TileSet, 
                    pal: map.TilePalette2D, 
                    is8bit: map.MapData2D.MapLayers[3]?.TileSet == tileSetGroup.Key, 
                    engineVersion: context.Settings.EngineVersion, 
                    levelTheme: rom.LevelInfos[rom.CurrentLevInfo.LevelIndex].LevelTheme, 
                    mapTiles_4: map.MapData2D.MapLayers.Select((x, i) => new
                    {
                        Tiles = tileMaps[i],
                        TileSet = x?.TileSet
                    }).Where(x => x.TileSet == tileSetGroup.Key).SelectMany(x => x.Tiles.Select(t => t.Data)).ToArray()));
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
                        MapTiles = tileMaps[i],
                        Type = Unity_Map.MapType.Graphics,
                        Layer = x.LayerPrio == (4 - map.MapData2D.MapLayers.Count(y => y != null)) && i != 3 ? Unity_Map.MapLayer.Front : Unity_Map.MapLayer.Middle,
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

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            var objmanager = new Unity_ObjectManager_GBAVV(context, LoadAnimSets(rom), map.MapData2D.ObjData, map.MapType);
            var objects = map.MapData2D.ObjData.ObjGroups.SelectMany((x, groupIndex) => x.Objects.Reverse().Select((obj, i) => new Unity_Object_GBAVV(objmanager, obj, groupIndex, i)));

            return new Unity_Level(
                maps: maps,
                objManager: objmanager,
                eventData: new List<Unity_Object>(objects),
                cellSize: CellSize,
                getCollisionTypeGraphicFunc: x => ((GBAVV_Map2D_CollisionType)x).GetCollisionTypeGraphic(),
                getCollisionTypeNameFunc: x => ((GBAVV_Map2D_CollisionType)x).ToString(),
                localization: LoadLocalization(rom));
        }

        public async UniTask<Unity_Level> LoadMode7Async(Context context, GBAVV_ROM rom)
        {
            var levelInfo = rom.CurrentMode7LevelInfo;
            var tileSetFrames = levelInfo.TileSetFrames;

            Controller.DetailedState = "Loading maps & tilesets";
            await Controller.WaitIfNecessary();

            const int width = 240 / CellSize;
            const int height = 160 / CellSize;

            var tilePal = rom.Mode7_GetTilePal(levelInfo);

            Unity_Map[] maps = null;

            if (context.Settings.EngineVersion == EngineVersion.GBAVV_Crash1)
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
            else if (context.Settings.EngineVersion == EngineVersion.GBAVV_Crash2)
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

            var objmanager = new Unity_ObjectManager_GBAVVMode7(context, LoadMode7AnimSets(levelInfo, tilePal));
            var objects = levelInfo.ObjData.Objects.Select(x => new Unity_Object_GBAVVMode7(objmanager, x));

            // Spawn the chase object for type 0 or special object for Crash 1 (blimp or N. Gin) // TODO: Spawn blimps at correct positions - array at 0x0817a420?
            if (levelInfo.LevelType == 0 || context.Settings.EngineVersion == EngineVersion.GBAVV_Crash1)
                objects = objects.Append(new Unity_Object_GBAVVMode7(objmanager, new GBAVV_Mode7_Object()
                {
                    ObjType_Normal = (byte)(objmanager.AnimSets.Length - 1),
                    ObjType_TimeTrial = (byte)(objmanager.AnimSets.Length - 1),
                    ObjType_Unknown = (byte)(objmanager.AnimSets.Length - 1),
                    ZPos = -50 // Have it start a bit behind the player
                }));

            // Spawn the main character (always type 0)
            objects = objects.Append(new Unity_Object_GBAVVMode7(objmanager, new GBAVV_Mode7_Object()));

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
                cellSize: CellSize,
                localization: LoadLocalization(rom));
        }

        public async UniTask<Unity_Level> LoadIsometricAsync(Context context, GBAVV_ROM rom)
        {
            var mapData = rom.CurrentIsometricMapData;
            var objData = rom.CurrentIsometricObjData;

            Controller.DetailedState = "Loading maps & tilesets";
            await Controller.WaitIfNecessary();

            var tileSet = LoadIsometricTileSet(mapData.TileSet, mapData.TilePalette);

            var maps = mapData.MapLayers.Select((map, i) => new Unity_Map
            {
                Width = (ushort)(map.Width * 2),
                Height = (ushort)(map.Height * 2),
                TileSet = new Unity_TileSet[]
                {
                    tileSet,
                },
                MapTiles = GetIsometricTileMap(map, mapData.MapTiles),
                Type = Unity_Map.MapType.Graphics,
                Layer = i == 0 ? Unity_Map.MapLayer.Front : Unity_Map.MapLayer.Middle
            }).Reverse().ToArray();

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();
            int baseType = (int)Unity_IsometricCollisionTile.CollisionType.GBAVV_Solid_0;

            float minHeight = Mathf.Min(0, mapData.CollisionMap.Min(c => mapData.CollisionTiles[c].Height.AsFloat));
            var collision = mapData.CollisionMap.Select(c => new Unity_IsometricCollisionTile() {
                GBAVV_Rotation = mapData.CollisionTiles[c].Shape % 4,
                GBAVV_AdditionalHeight = mapData.CollisionTypes[mapData.CollisionTiles[c].TypeIndex].AdditionalHeight,
                DebugText = Settings.ShowDebugInfo ? ($"Height: {(mapData.CollisionTiles[c].Height - minHeight)}{Environment.NewLine}" +
                $"Tile index: {c}{Environment.NewLine}" +
                $"Offset: {mapData.CollisionTiles[c].Offset}{Environment.NewLine}" +
                $"Type: {mapData.CollisionTiles[c].TypeIndex}{Environment.NewLine}" +
                $"Shape: {mapData.CollisionTiles[c].Shape}") : null,
                Height = (mapData.CollisionTiles[c].Height - minHeight),
                Type = (Unity_IsometricCollisionTile.CollisionType)(baseType+GetIsometricCollisionType(rom.CurrentIsometricIndex, mapData.CollisionTiles[c].TypeIndex))
            }).ToArray();
            var mirroredCollision = new Unity_IsometricCollisionTile[mapData.CollisionWidth * mapData.CollisionHeight];
            for (int x = 0; x < mapData.CollisionWidth; x++) {
                for (int y = 0; y < mapData.CollisionHeight; y++) {
                    mirroredCollision[x * mapData.CollisionHeight + y] = collision[y * mapData.CollisionWidth + x];
                }
            }

            // X/Z dimensions: the diagonal of one collision tile is 12 graphics tiles. Height is 6 which matches 12 * sin(30 deg).
            // Height: a 0,1875 is 6 tiles => 6/0.1875 = 32 => viewed at an angle, so divide by cos(angle)
            float tileDiagonal = 12/2f;
            float tileWidth = Mathf.Sqrt(tileDiagonal * tileDiagonal/2);
            float heightScale = 32f / Mathf.Cos(Mathf.Deg2Rad * 30f) / 2f;

            // Create the object manager and load animations
            var objManager = new Unity_ObjectManager_GBAVVIsometric(context, LoadIsometricAnimations(rom), mapData, x => GetIsometricCollisionType(rom.CurrentIsometricIndex, x));

            // Load normal objects
            var objects = objData.Objects.Select(x => (Unity_Object_BaseGBAVVIsometric)new Unity_Object_GBAVVIsometric_Obj(x, objManager));

            // Load target objects
            var index = objData.Objects.Length;

            foreach (var targetObj in objData.TargetObjects)
            {
                objects = objects.Append(new Unity_Object_GBAVVIsometric_TargetObj(targetObj, objManager, index + 1));
                objects = objects.Append(new Unity_Object_GBAVVIsometric_TargetObjTarget(targetObj, objManager));

                index += 2;
            }

            // Load start positions
            for (int i = 0; i < objData.StartPositions.Length; i++)
                objects = objects.Append(new Unity_Object_GBAVVIsometric_StartPos(objData.StartPositions[i], i, objManager));

            // Load multiplayer flags
            objects = objData.MultiplayerFlags.Aggregate(objects, (current, o) => current.Append(new Unity_Object_GBAVVIsometric_MultiplayerFlag(new GBAVV_Isometric_Position()
            {
                XPos = new FixedPointInt() { Value = o.XPos * 0x3000 + 0x1800 }, 
                YPos = new FixedPointInt() { Value = o.YPos * 0x3000 + 0x1800 },
            }, objManager)));

            // Load multiplayer crowns
            objects = objData.MultiplayerCrowns.Aggregate(objects, (current, o) => current.Append(new Unity_Object_GBAVVIsometric_MultiplayerCrown(new GBAVV_Isometric_Position()
            {
                XPos = new FixedPointInt() { Value = o.XPos << 8 }, 
                YPos = new FixedPointInt() { Value = o.YPos << 8 },
            }, objManager)));

            float w = mapData.MapWidth * 0.5f;
            float h = mapData.MapHeight * 0.5f;

            return new Unity_Level(
                maps: maps,
                objManager: objManager,
                eventData: new List<Unity_Object>(objects),
                cellSize: CellSize,
                isometricData: new Unity_IsometricData()
                {
                    CollisionWidth = mapData.CollisionHeight,
                    CollisionHeight = mapData.CollisionWidth,
                    TilesWidth = mapData.MapWidth,
                    TilesHeight = mapData.MapHeight,
                    Collision = mirroredCollision,
                    Scale = new Vector3(tileWidth, heightScale, tileWidth),
                    // Multiply X & Y displacement by 2 as it is divided by 2 later
                    CalculateXDisplacement = () => w - 16 * mapData.XPosition * 2,
                    CalculateYDisplacement = () => h - 16 * mapData.YPosition * 2 + (minHeight * heightScale * 2 * Mathf.Cos(Mathf.Deg2Rad * 30f)),
                    ObjectScale = Vector3.one * 12/64f
                },
                localization: LoadLocalization(rom));
        }

        public async UniTask<Unity_Level> LoadWorldMapAsync(Context context, GBAVV_ROM rom)
        {
            var map = rom.CurrentWorldMapData;

            Controller.DetailedState = "Loading tilesets";
            await Controller.WaitIfNecessary();

            // Load tilemaps
            var tileMaps = map.MapLayers.Select((x, i) =>
            {
                if (x == null)
                    return null;

                return GetWorldMapTileMap(x.TileMap, x.MapTiles);
            }).ToArray();

            var tileSet4bpp = LoadTileSet(map.TileSets.TileSet4bpp.TileSet, map.TilePalette, false, context.Settings.EngineVersion, 0xFF, tileMaps.Take(3).Where(x => x != null).SelectMany(x => x).Select(x => x.Data).ToArray());
            var tileSet8bpp = LoadTileSet(map.TileSets.TileSet8bpp.TileSet, map.TilePalette, true, context.Settings.EngineVersion, 0xFF, null);

            Controller.DetailedState = "Loading maps";
            await Controller.WaitIfNecessary();

            var maps = map.MapLayers.Select((x, i) =>
            {
                if (x == null)
                    return null;

                return new
                {
                    Map = new Unity_Map
                    {
                        Width = (ushort)(x.TileMap.Width * 2),
                        Height = (ushort)(x.TileMap.Height * 2),
                        TileSet = new Unity_TileSet[]
                        {
                            i == 3 ? tileSet8bpp : tileSet4bpp
                        },
                        MapTiles = tileMaps[i],
                        Type = Unity_Map.MapType.Graphics,
                        Layer = Unity_Map.MapLayer.Middle,
                    },
                    Prio = x.LayerPrio
                };
            }).Where(x => x != null).OrderByDescending(x => x.Prio).Select(x => x.Map).ToArray();

            List<Unity_CollisionLine> collisionLines = null;

            // Load collision
            if (map.Fusion_Collision != null)
            {
                collisionLines = new List<Unity_CollisionLine>();
                var dummyCollision = new GBAVV_Fusion_MapCollisionLineData();

                addCollision(map.Fusion_Collision);

                void addCollision(GBAVV_Fusion_MapCollisionSector mapCollision)
                {
                    // Add every line
                    foreach (var line in mapCollision.CollisionLines ?? new GBAVV_Fusion_MapCollisionLine[0])
                    {
                        // Get vectors for the points
                        Vector2 p1 = new Vector2(line.X1, line.Y1);
                        Vector2 p2 = new Vector2(line.X2, line.Y2);

                        var c = line.CollisionData ?? dummyCollision;

                        // Add the line
                        collisionLines.Add(new Unity_CollisionLine()
                        {
                            Pos_0 = p1,
                            Pos_1 = p2,
                            LineColor = c.GetColor(),
                            TypeName = c.GetCollisionType().ToString(),
                            DebugText = $"Direction: {line.Direction}{Environment.NewLine}" +
                                        $"Data: {(c.Data != null ? Util.ByteArrayToHexString(c.Data) : "")}{Environment.NewLine}"
                        });
                    }

                    // Add every sub-sector
                    foreach (var mc in mapCollision.SubSectors.Where(x => x != null))
                        addCollision(mc);
                }
            }

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            var objmanager = new Unity_ObjectManager_GBAVV(context, LoadAnimSets(rom), map.ObjData, GBAVV_MapInfo.GBAVV_MapType.WorldMap, rom.Scripts, rom.Map2D_Graphics, rom.DialogScripts);
            var objects = new List<Unity_Object>();

            if (map.ObjData?.Objects != null)
                objects.AddRange(map.ObjData.Objects.Select(obj => new Unity_Object_GBAVV(objmanager, obj, -1, -1)));

            return new Unity_Level(
                maps: maps,
                objManager: objmanager,
                eventData: objects,
                cellSize: CellSize,
                localization: LoadLocalization(rom),
                collisionLines: collisionLines?.ToArray());
        }

        public async UniTask<Unity_Level> LoadNitroKartAsync(Context context, GBAVV_ROM rom)
        {
            var map = rom.CurrentNitroKartLevelInfo.MapData;

            Controller.DetailedState = "Loading tilesets";
            await Controller.WaitIfNecessary();

            // Set the collision type for the tiles
            foreach (var t in map.Mode7MapLayer.MapTiles)
                t.CollisionType = (ushort)map.Mode7TileSetCollision[t.TileMapY];

            var tilePalettes = map.AdditionalTilePalettesCount > 0 ? map.AdditionalTilePalettes : new RGBA5551Color[][]
            {
                map.TilePalette
            };

            var mode7TileSets = tilePalettes.Select(p => LoadTileSet(map.Mode7TileSet, p, true, context.Settings.EngineVersion, 0, null)).ToArray();
            var bgTileSets = tilePalettes.Select(p => LoadTileSet(map.BackgroundTileSet.TileSet, p, false, context.Settings.EngineVersion, 0, map.BackgroundMapLayers.SelectMany(x => x.TileMap.MapTiles).ToArray(), map.BackgroundTileAnimations)).ToArray();

            Controller.DetailedState = "Loading maps";
            await Controller.WaitIfNecessary();

            Unity_Map getMap(GBAVV_NitroKart_BackgroundMapLayer m)
            {
                var width = (ushort)(m.Width / CellSize);
                var height = (ushort)(m.Height / CellSize);

                var tileMap = new Unity_Tile[width * height];

                const int screenBlockWidth = 32;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int screenblock = (x / screenBlockWidth) * 1024;
                        tileMap[y * width + x] = new Unity_Tile(m.TileMap.MapTiles[y * screenBlockWidth + (x % screenBlockWidth) + screenblock]);
                    }
                }

                return new Unity_Map
                {
                    Width = width,
                    Height = height,
                    TileSet = bgTileSets,
                    MapTiles = tileMap,
                    Type = Unity_Map.MapType.Graphics,
                    Layer = Unity_Map.MapLayer.Middle,
                };
            }

            var maps = new Unity_Map[]
            {
                new Unity_Map
                {
                    Width = (ushort)(map.Mode7MapLayer.TileMap.Width * 2),
                    Height = (ushort)(map.Mode7MapLayer.TileMap.Height * 2),
                    TileSet = mode7TileSets,
                    MapTiles = GetIsometricTileMap(map.Mode7MapLayer.TileMap, map.Mode7MapLayer.MapTiles),
                    Type = Unity_Map.MapType.Graphics | Unity_Map.MapType.Collision,
                    Layer = Unity_Map.MapLayer.Middle,
                },
                getMap(map.BackgroundMapLayers[2]),
                getMap(map.BackgroundMapLayers[1]),
                getMap(map.BackgroundMapLayers[0]),
            };

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            Unity_ObjectManager_GBAVV objManager = new Unity_ObjectManager_GBAVV(context, LoadAnimSets(rom), null, GBAVV_MapInfo.GBAVV_MapType.Kart, graphics: rom.Map2D_Graphics, nitroKart_ObjTypeData: rom.NitroKart_ObjTypeData);

            var objGroups = new List<(GBAVV_NitroKart_Object[], string)>();

            objGroups.Add((map.Objects.Objects_Normal, "Normal"));

            if (map.Objects.ObjectsPointer_TimeTrial != map.Objects.ObjectsPointer_Normal)
                objGroups.Add((map.Objects.Objects_TimeTrial, "Time Trial"));

            if (map.Objects.ObjectsPointer_BossRace != map.Objects.ObjectsPointer_Normal)
                objGroups.Add((map.Objects.Objects_BossRace, "Boss Race"));

            var objects = objGroups.SelectMany((x, i) => x.Item1.Select(o => (Unity_Object)new Unity_Object_GBAVVNitroKart(objManager, o, i))).ToList();

            var waypointsGroupIndex = 0;

            void addTrackWaypoints(GBAVV_NitroKart_TrackWaypoint[] waypoints, string groupName, int trackDataIndex)
            {
                if (waypoints == null)
                    return;

                if (objGroups.Any(x => x.Item2 == groupName))
                {
                    objects.AddRange(waypoints.Select(w => new Unity_Object_GBAVVNitroKartWaypoint(w, waypointsGroupIndex, trackDataIndex)));
                    waypointsGroupIndex++;
                }
            }

            addTrackWaypoints(map.TrackData1.TrackWaypoints_Normal, "Normal", 0);
            addTrackWaypoints(map.TrackData1.TrackWaypoints_TimeTrial, "Time Trial", 0);
            addTrackWaypoints(map.TrackData1.TrackWaypoints_BossRace, "Boss Race", 0);
            waypointsGroupIndex = 0;
            addTrackWaypoints(map.TrackData2.TrackWaypoints_Normal, "Normal", 1);
            addTrackWaypoints(map.TrackData2.TrackWaypoints_TimeTrial, "Time Trial", 1);
            addTrackWaypoints(map.TrackData2.TrackWaypoints_BossRace, "Boss Race", 1);

            return new Unity_Level(
                maps: maps,
                objManager: objManager,
                eventData: objects,
                cellSize: CellSize,
                objectGroups: objGroups.Select(x => x.Item2).ToArray(),
                getCollisionTypeGraphicFunc: x => ((GBAVV_NitroKart_CollisionType)x).GetCollisionTypeGraphic(),
                getCollisionTypeNameFunc: x => ((GBAVV_NitroKart_CollisionType)x).ToString(),
                localization: LoadLocalization(rom),
                isometricData: new Unity_IsometricData {
                    CollisionWidth = 0,
                    CollisionHeight = 0,
                    TilesWidth = 0,
                    TilesHeight = 0,
                    Collision = null,
                    Scale = Vector3.one / 2,
                    ViewAngle = Quaternion.Euler(90,0,0),
                    CalculateYDisplacement = () => 0,
                    CalculateXDisplacement = () => 0,
                    ObjectScale = Vector3.one * CellSize
                });
        }

        public static byte GetIsometricCollisionType(int level, int index)
        {
            switch (level)
            {
                case 0:
                    switch (index)
                    {
                        case 0: return 0;
                        case 1: return 1;
                        case 2: return 2;
                    }
                    break;
                case 1:
                    switch (index)
                    {
                        case 0: return 0;
                        case 1: return 3;
                        case 2: return 1;
                        case 3: return 4;
                        case 4: return 5;
                    }
                    break;
                case 2:
                    switch (index)
                    {
                        case 0: return 0;
                        case 1: return 3;
                    }
                    break;
                case 3:
                    switch (index)
                    {
                        case 0: return 0;
                        case 1: return 6;
                        case 2: return 7;
                        case 3: return 8;
                        case 4: return 9;
                        case 5: return 10;
                        case 6: return 11;
                    }
                    break;
                case 4:
                    switch (index)
                    {
                        case 0: return 0;
                        case 1: return 12;
                        case 2: return 13;
                        case 3: return 1;
                        case 4: return 14;
                        case 5: return 15;
                        case 6: return 16;
                        case 7: return 17;
                        case 8: return 18;
                        case 9: return 19;
                        case 10: return 20;
                        case 11: return 21;
                        case 12: return 22;
                        case 13: return 23;
                        case 14: return 24;
                        case 15: return 25;
                        case 16: return 26;
                    }
                    break;
                case 5:
                    switch (index)
                    {
                        case 0: return 0;
                        case 1: return 27;
                        case 2: return 12;
                        case 3: return 13;
                        case 4: return 26;
                        case 5: return 28;
                        case 6: return 1;
                        case 7: return 29;
                        case 8: return 30;
                        case 9: return 19;
                        case 10: return 31;
                        case 11: return 32;
                        case 12: return 24;
                        case 13: return 21;
                    }
                    break;
                case 6:
                    switch (index)
                    {
                        case 0: return 0;
                        case 1: return 1;
                        case 2: return 24;
                        case 3: return 17;
                        case 4: return 28;
                        case 5: return 32;
                        case 6: return 33;
                        case 7: return 25;
                        case 8: return 26;
                        case 9: return 34;
                        case 10: return 12;
                        case 11: return 19;
                        case 12: return 21;
                        case 13: return 35;
                    }
                    break;
            }

            throw new Exception($"Invalid collision type {index} in level {level}");
        }

        public Unity_TileSet LoadTileSet(byte[] tileSet, RGBA5551Color[] pal, bool is8bit, EngineVersion engineVersion, uint levelTheme, MapTile[] mapTiles_4, GBAVV_NitroKart_TileAnimations nitroKartTileAnimations = null)
        {
            Texture2D tex;
            var additionalTiles = new List<Texture2D>();
            var tileSize = is8bit ? 0x40 : 0x20;
            var paletteIndices = Enumerable.Range(0, tileSet.Length / tileSize).Select(x => new List<byte>()).ToArray();
            var tilesCount = tileSet.Length / tileSize;
            var tileSetPaletteIndices = new List<byte>();
            var originalTileSetIndices = new List<int>();
            var tileAnimations = new List<Unity_AnimatedTile>();

            if (is8bit)
            { 
                tex = Util.ToTileSetTexture(tileSet, Util.ConvertGBAPalette(pal), Util.TileEncoding.Linear_8bpp, CellSize, false);
            }
            else
            {
                var palettes = Util.ConvertAndSplitGBAPalette(pal);

                foreach (var m in mapTiles_4)
                {
                    if (!paletteIndices[m.TileMapY].Contains(m.PaletteIndex))
                        paletteIndices[m.TileMapY].Add(m.PaletteIndex);
                }

                tex = Util.ToTileSetTexture(tileSet, palettes[0], Util.TileEncoding.Linear_4bpp, CellSize, false, getPalFunc: x =>
                {
                    var p = paletteIndices[x].ElementAtOrDefault(0);
                    tileSetPaletteIndices.Add(p);
                    return palettes[p];
                });

                // Add additional tiles for tiles with multiple palettes
                for (int tileIndex = 0; tileIndex < paletteIndices.Length; tileIndex++)
                {
                    for (int palIndex = 1; palIndex < paletteIndices[tileIndex].Count; palIndex++)
                    {
                        var p = paletteIndices[tileIndex][palIndex];

                        var tileTex = TextureHelpers.CreateTexture2D(CellSize, CellSize);

                        // Create a new tile
                        tileTex.FillInTile(
                            imgData: tileSet,
                            imgDataOffset: tileSize * tileIndex,
                            pal: palettes[p],
                            encoding: Util.TileEncoding.Linear_4bpp,
                            tileWidth: CellSize,
                            flipTextureY: false,
                            tileX: 0,
                            tileY: 0);

                        // Modify all tiles where this is used
                        foreach (MapTile t in mapTiles_4.Where(x => x.TileMapY == tileIndex && x.PaletteIndex == p))
                        {
                            t.TileMapY = (ushort)(tilesCount + additionalTiles.Count);
                        }

                        // Add to additional tiles list
                        additionalTiles.Add(tileTex);
                        tileSetPaletteIndices.Add(p);
                        originalTileSetIndices.Add(tileIndex);
                    }
                }

                // NOTE: Currently this does not support the same tile having multiple palettes, but it's not used in Nitro Kart
                // Add Nitro Kart animated tiles
                if (nitroKartTileAnimations != null)
                {
                    // Enumerate every animation
                    foreach (var anim in nitroKartTileAnimations.Animations)
                    {
                        // Enumerate every tile
                        for (int tileIndex = 0; tileIndex < anim.TilesCount; tileIndex++)
                        {
                            // Make sure the tile is used in the level
                            if (mapTiles_4.All(x => x.TileMapY != anim.TileIndices[tileIndex]))
                                continue;

                            // Get the palette index
                            var palIndex = mapTiles_4.First(x => x.TileMapY == anim.TileIndices[tileIndex]).PaletteIndex;

                            var framesCount = anim.FramesCount - 1; // Don't include the first frame since that's already in the normal tileset

                            // Enumerate every frame
                            for (int frameIndex = 1; frameIndex < anim.FramesCount; frameIndex++)
                            {
                                var tileTex = TextureHelpers.CreateTexture2D(CellSize, CellSize);

                                // Create a new tile
                                tileTex.FillInTile(
                                    imgData: anim.Frames[frameIndex].TileSet,
                                    imgDataOffset: tileSize * tileIndex,
                                    pal: palettes[palIndex],
                                    encoding: Util.TileEncoding.Linear_4bpp,
                                    tileWidth: CellSize,
                                    flipTextureY: false,
                                    tileX: 0,
                                    tileY: 0);

                                // Add to additional tiles list
                                additionalTiles.Add(tileTex);
                            }

                            tileAnimations.Add(new Unity_AnimatedTile
                            {
                                AnimationSpeeds = anim.Frames.Select(x => x.Speed / 2f).ToArray(),
                                TileIndices = new int[]
                                {
                                    anim.TileIndices[tileIndex]
                                }.Concat(Enumerable.Range(tilesCount + additionalTiles.Count - framesCount, framesCount)).ToArray()
                            });
                        }
                    }
                }
            }

            // Some levels use animated tile palettes based on the level theme. These are all hard-coded in the level load function.
            RGBA5551Color[][] animatedPalettes = null;
            HashSet<byte> modifiedPaletteIndices = null;
            var animSpeed = 0;
            bool isReversed = false;

            //Debug.Log($"Theme: {levelTheme}");

            // NOTE: Speeds might not be correct
            if (engineVersion == EngineVersion.GBAVV_Crash1)
            {
                if (levelTheme == 1 || levelTheme == 6) // Jungle
                {
                    var anim = new byte[][]
                    {
                        // 0x0816c35e
                        new byte[] { 0x39, 0x3a, 0x3d, 0x3e, 0x75, 0x82, 0xbc, 0xea, 0xfc },
                        // 0x0816c370
                        new byte[] { 0x51, 0x52, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0xf1 }, // Speed on this one is slightly different
                    };

                    modifiedPaletteIndices = new HashSet<byte>(anim.SelectMany(x => x));

                    animatedPalettes = GetAnimatedPalettes(anim, pal);

                    isReversed = true;
                    animSpeed = 2;
                }
                else if (levelTheme == 2) // Sewer
                {
                    var anim = new byte[][]
                    {
                        // 0x0816c354
                        new byte[] { 0xb1, 0xb2, 0xb3, 0xb4, 0xb5 },
                    };

                    modifiedPaletteIndices = new HashSet<byte>(anim.SelectMany(x => x));

                    animatedPalettes = GetAnimatedPalettes(anim, pal);

                    animSpeed = 5;
                }
                else if (levelTheme == 3) // Underwater
                {
                    var anim = new byte[][]
                    {
                        // 0x0816c382
                        new byte[] { 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f },
                    };

                    modifiedPaletteIndices = new HashSet<byte>(anim.SelectMany(x => x));

                    animatedPalettes = GetAnimatedPalettes(anim, pal);

                    isReversed = true;
                    animSpeed = 4;
                }
                else if (levelTheme == 5) // Space
                {
                    var anim = new byte[][]
                    {
                        // 0x0816c3a2
                        new byte[] { 0x97, 0xb4, 0xf7, 0xf8, 0xff },
                    };

                    modifiedPaletteIndices = new HashSet<byte>(anim.SelectMany(x => x));

                    animatedPalettes = GetAnimatedPalettes(anim, pal);

                    isReversed = true;
                    animSpeed = 3;
                }
            }
            else if (engineVersion == EngineVersion.GBAVV_Crash2)
            {
                if (levelTheme == 1 || levelTheme == 9) // Egyptian
                {
                    var anim = new byte[][]
                    {
                        // 0x081d26ea
                        new byte[] { 0xfb, 0xfc, 0xfd, 0xfe, 0xff },
                        // 0x081d26f4
                        new byte[] { 0x22, 0x23, 0x24 },
                    };

                    modifiedPaletteIndices = new HashSet<byte>(anim.SelectMany(x => x));

                    animatedPalettes = GetAnimatedPalettes(anim, pal).Reverse().ToArray(); // The animation is reversed

                    animSpeed = 4;
                }
                else if (levelTheme == 0) // Arabian
                {
                    var anim = new byte[][]
                    {
                        // 0x081d26e0
                        new byte[] { 0xf3, 0xf4, 0xf5, 0xf6, 0xf7 },
                    };

                    modifiedPaletteIndices = new HashSet<byte>(anim.SelectMany(x => x));

                    animatedPalettes = GetAnimatedPalettes(anim, pal);

                    animSpeed = 4;
                }
                // TODO: Theme 2 (Volcanic) uses a different palette animation system
            }
            else if (engineVersion == EngineVersion.GBAVV_CrashNitroKart)
            {
                if (is8bit)
                {
                    var anim = new byte[][]
                    {
                        new byte[] { 0xfb, 0xfc, 0xfd, 0xfe, 0xff },
                    };

                    modifiedPaletteIndices = new HashSet<byte>(anim.SelectMany(x => x));

                    animatedPalettes = GetAnimatedPalettes(anim, pal);

                    animSpeed = 2;
                }
            }

            var additionalPalTilesCount = additionalTiles.Count;

            if (animatedPalettes != null)
            {
                if (isReversed)
                    animatedPalettes = animatedPalettes.Reverse().ToArray();

                // Convert palettes
                var animatedPalettes_4 = animatedPalettes.Select(x => Util.ConvertAndSplitGBAPalette(x)).ToArray();
                var animatedPalettes_8 = animatedPalettes.Select(x => Util.ConvertGBAPalette(x)).ToArray();

                for (int tileIndex = 0; tileIndex < tilesCount + additionalPalTilesCount; tileIndex++)
                {
                    var originalTileIndex = tileIndex >= tilesCount ? originalTileSetIndices[tileIndex - tilesCount] : tileIndex;

                    var tilePalette = is8bit ? 0 : tileSetPaletteIndices[tileIndex];

                    // Check if the tile uses any of the animated colors
                    // If the tile doesn't contain an animated color we ignore it
                    bool containsAnimatedPalette = false;
                    for (int i = tileSize * originalTileIndex; i < tileSize * originalTileIndex + tileSize; i++) {
                        if (is8bit) {
                            if (modifiedPaletteIndices.Contains(tileSet[i])) {
                                containsAnimatedPalette = true;
                                break;
                            }
                        } else {
                            var palInd = tilePalette * 16;
                            if (modifiedPaletteIndices.Contains((byte)(palInd + (tileSet[i] & 0xF)))) {
                                containsAnimatedPalette = true;
                                break;
                            }
                            if (modifiedPaletteIndices.Contains((byte)(palInd + (tileSet[i] >> 4)))) {
                                containsAnimatedPalette = true;
                                break;
                            }
                        }
                    }
                    if(!containsAnimatedPalette) continue;

                    // Add animation for the tile
                    tileAnimations.Add(new Unity_AnimatedTile
                    {
                        AnimationSpeed = animSpeed,
                        TileIndices = new int[]
                        {
                            tileIndex
                        }.Concat(Enumerable.Range(tilesCount + additionalTiles.Count, animatedPalettes.Length)).ToArray()
                    });

                    // Create a new tile for every animated palette frame
                    for (int animFrame = 0; animFrame < animatedPalettes.Length; animFrame++)
                    {
                        var tileTex = TextureHelpers.CreateTexture2D(CellSize, CellSize);

                        // Create a new tile
                        tileTex.FillInTile(
                            imgData: tileSet,
                            imgDataOffset: tileSize * originalTileIndex,
                            pal: is8bit ? animatedPalettes_8[animFrame] : animatedPalettes_4[animFrame][tilePalette],
                            encoding: is8bit ? Util.TileEncoding.Linear_8bpp : Util.TileEncoding.Linear_4bpp,
                            tileWidth: CellSize,
                            flipTextureY: false,
                            tileX: 0,
                            tileY: 0);

                        // Add to additional tiles list
                        additionalTiles.Add(tileTex);
                    }
                }
            }

            // Create the tile array
            var tiles = new Unity_TileTexture[tilesCount + additionalTiles.Count];

            // Keep track of the index
            var index = 0;

            // Add every normal tile
            for (int y = 0; y < tex.height; y += CellSize)
            {
                for (int x = 0; x < tex.width; x += CellSize)
                {
                    if (index >= tilesCount)
                        break;

                    // Create a tile
                    tiles[index++] = tex.CreateTile(new Rect(x, y, CellSize, CellSize));
                }
            }

            // Add additional tiles
            foreach (Texture2D t in additionalTiles)
                tiles[index++] = t.CreateTile();

            //Debug.Log($"Count: {tilesCount}");
            //Debug.Log($"TotalCount: {totalTilesCount}");
            //Debug.Log($"AnimFrames: {animatedPalettes?.Length}");
            //Debug.Log($"Tiles: {tiles.Length}");

            return new Unity_TileSet(tiles)
            {
                AnimatedTiles = tileAnimations.Count == 0 ? null : tileAnimations.ToArray()
            };
        }

        public RGBA5551Color[][] GetAnimatedPalettes(byte[][] paletteAnimations, RGBA5551Color[] palette)
        {
            // Get the lowest common multiple
            var length = paletteAnimations.Length == 1 ? paletteAnimations[0].Length : Util.LCM(paletteAnimations.Select(y => y.Length).ToArray());

            var output = new RGBA5551Color[length - 1][];

            // Shift colors and create new palettes for every frame
            for (int i = 0; i < length - 1; i++)
            {
                int frame = i + 1;
                var newPal = new RGBA5551Color[palette.Length];

                // Set to original palette
                for (int j = 0; j < newPal.Length; j++)
                    newPal[j] = palette[j];

                foreach (var anim in paletteAnimations)
                {
                    int shift = frame % anim.Length;
                    if (shift > 0)
                    {
                        for (int k = 0; k < anim.Length; k++)
                        {
                            int targetK = (k + shift) % anim.Length;
                            newPal[anim[targetK]] = palette[anim[k]];
                        }
                    }
                }

                output[i] = newPal;
            }

            return output;
        }

        public Unity_TileSet LoadGenericTileSet(byte[] tileSet, RGBA5551Color[] pal, int palIndex)
        {
            var palettes = Util.ConvertAndSplitGBAPalette(pal);

            var tex = Util.ToTileSetTexture(tileSet, palettes[palIndex], Util.TileEncoding.Linear_4bpp, CellSize, false);

            return new Unity_TileSet(tex, CellSize);
        }

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

        public Unity_TileSet LoadMode7BackgroundTileSet(GBAVV_Mode7_Background background)
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

        public Unity_TileSet LoadIsometricTileSet(GBAVV_Isometric_TileSet tileSet, RGBA5551Color[] tilePal)
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

            var completeTileSet = convertedTileSet.Concat(tileSet.TileSet_8bpp).ToArray();

            var tex = Util.ToTileSetTexture(completeTileSet, pal, Util.TileEncoding.Linear_8bpp, CellSize, false);

            // Palette animation
            var palIndices = new byte[] { 0x61, 0x86, 0x96 };
            var animatedPalettes = GetAnimatedPalettes(new byte[][]
            {
                palIndices.Reverse().ToArray()
            }, tilePal);
            var palIndicesLookup = new HashSet<byte>(palIndices);

            var additionalTiles = new List<Texture2D>();
            var tileAnimations = new List<Unity_AnimatedTile>();
            var tilesCount = tileSet.TileSetCount_Total;
            const int tileSize = 0x40;
            const int animSpeed = 3; // Note: Might not be correct

            var animatedPalettes_8 = animatedPalettes.Select(x => Util.ConvertGBAPalette(x)).ToArray();

            for (int tileIndex = 0; tileIndex < tilesCount; tileIndex++)
            {
                // Check if the tile uses any of the animated colors

                // Check if the tile uses any of the animated colors
                // If the tile doesn't contain an animated color we ignore it
                bool containsAnimatedPalette = false;
                for (int i = tileSize * tileIndex; i < tileSize * tileIndex + tileSize; i++) {
                    if (palIndicesLookup.Contains(completeTileSet[i])) {
                        containsAnimatedPalette = true;
                        break;
                    }
                }
                if (!containsAnimatedPalette) continue;

                // Add animation for the tile
                tileAnimations.Add(new Unity_AnimatedTile
                {
                    AnimationSpeed = animSpeed,
                    TileIndices = new int[]
                    {
                        tileIndex
                    }.Concat(Enumerable.Range(tilesCount + additionalTiles.Count, animatedPalettes.Length)).ToArray()
                });

                // Create a new tile for every animated palette frame
                for (int animFrame = 0; animFrame < animatedPalettes.Length; animFrame++)
                {
                    var tileTex = TextureHelpers.CreateTexture2D(CellSize, CellSize);

                    // Create a new tile
                    tileTex.FillInTile(
                        imgData: completeTileSet,
                        imgDataOffset: tileSize * tileIndex,
                        pal: animatedPalettes_8[animFrame],
                        encoding: Util.TileEncoding.Linear_8bpp,
                        tileWidth: CellSize,
                        flipTextureY: false,
                        tileX: 0,
                        tileY: 0);

                    // Add to additional tiles list
                    additionalTiles.Add(tileTex);
                }
            }

            // Create the tile array
            var tiles = new Unity_TileTexture[tilesCount + additionalTiles.Count];

            // Keep track of the index
            var finalTileIndex = 0;

            // Add every normal tile
            for (int y = 0; y < tex.height; y += CellSize)
            {
                for (int x = 0; x < tex.width; x += CellSize)
                {
                    if (finalTileIndex >= tilesCount)
                        break;

                    // Create a tile
                    tiles[finalTileIndex++] = tex.CreateTile(new Rect(x, y, CellSize, CellSize));
                }
            }

            // Add additional tiles
            foreach (Texture2D t in additionalTiles)
                tiles[finalTileIndex++] = t.CreateTile();

            return new Unity_TileSet(tiles)
            {
                AnimatedTiles = tileAnimations.Count == 0 ? null : tileAnimations.ToArray()
            };
        }

        public Unity_Tile[] GetTileMap(GBAVV_Map2D_MapLayer layer, GBAVV_Map2D_LayersBlock.GBAVV_TileLayerData tileLayerData, bool is8bit = false, int tileSetLength = 0, bool isCollision = false)
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

        public Unity_Tile[] GetIsometricTileMap(GBAVV_Isometric_MapLayer mapLayer, MapTile[] mapTiles)
        {
            var tileMap = new Unity_Tile[mapLayer.Width * 2 * mapLayer.Height * 2];
            int actualX = 0, actualY = 0;
            byte cmdType = 0;

            for (int y = 0; y < mapLayer.Height; y++) {
                actualY = y * 2;
                actualX = 0;
                var chunk = mapLayer.TileMapRows[y];
                //var x = 0;

                foreach (var cmd in chunk.Commands)
                {
                    cmdType = cmd.Type;
                    if (cmd.Type == 3)
                    {
                        for (var i = 0; i < cmd.Params.Length; i++)
                        {
                            setTile(cmd.Params[i], i, cmd.Params.Length);
                        }
                    }
                    else if (cmd.Type == 2)
                    {
                        for (int i = 0; i < cmd.Length; i++)
                        {
                            setTile(cmd.Param, i, cmd.Length);
                        }
                    }
                    else
                    {
                        setTile(cmd.Length, 0, 1);
                    }
                }
            }

            void setTile(ushort value, int index, int length) {
                var val = value * 4;

                setTileAt(0, 0, mapTiles[val + 0], index, length, value);
                setTileAt(1, 0, mapTiles[val + 1], index, length, value);
                setTileAt(0, 1, mapTiles[val + 2], index, length, value);
                setTileAt(1, 1, mapTiles[val + 3], index, length, value);
                actualX += 2;
            }

            void setTileAt(int offX, int offY, MapTile tile, int index, int length, ushort value) {
                var outputX = actualX + offX;
                var outputY = actualY + offY;
                tileMap[outputY * mapLayer.Width * 2 + outputX] = new Unity_Tile(tile) {
                    /*DebugText = $"CMD: {cmdType}{Environment.NewLine}" +
                                $"Value: {value}{Environment.NewLine}" +
                                $"Index: {index}{Environment.NewLine}" +
                                $"Length: {length}{Environment.NewLine}" +
                                $"Tile: {tile.TileMapY}{Environment.NewLine}" +
                                $"FlipX: {tile.HorizontalFlip}{Environment.NewLine}" +
                                $"FlipY: {tile.VerticalFlip}{Environment.NewLine}"*/
                };
            }

            return tileMap;
        }
        public Unity_Tile[] GetWorldMapTileMap(GBAVV_Isometric_MapLayer mapLayer, MapTile[] mapTiles)
        {
            var tileMap = new Unity_Tile[mapLayer.Width * 2 * mapLayer.Height * 2];

            for (int columnIndex = 0; columnIndex < mapLayer.Width; columnIndex++)
            {
                var column = mapLayer.TileMapRows[columnIndex];
                var y = 0;

                foreach (var cmd in column.Commands)
                {
                    if (cmd.Type == 3)
                    {
                        for (var i = 0; i < cmd.Params.Length; i++)
                        {
                            setTile(cmd.Params[i], i, cmd.Params.Length);
                            y++;
                        }
                    }
                    else if (cmd.Type == 2)
                    {
                        for (int i = 0; i < cmd.Length; i++)
                        {
                            setTile(cmd.Param, i, cmd.Length);
                            y++;
                        }
                    }
                    else
                    {
                        setTile(cmd.Length, 0, 1);
                        y++;
                    }

                    void setTile(ushort value, int index, int length)
                    {
                        var actualX = columnIndex * 2;
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
                                DebugText = Settings.ShowDebugInfo ? ($"CMD: {cmd.Type}{Environment.NewLine}" +
                                            $"Value: {value}{Environment.NewLine}" +
                                            $"Index: {index}{Environment.NewLine}" +
                                            $"Length: {length}{Environment.NewLine}" +
                                            $"Tile: {tile.TileMapY}{Environment.NewLine}" +
                                            $"FlipX: {tile.HorizontalFlip}{Environment.NewLine}" +
                                            $"FlipY: {tile.VerticalFlip}{Environment.NewLine}") : null
                            };
                        }
                    }
                }
            }

            return tileMap;
        }

        public Unity_ObjectManager_GBAVV.AnimSet[][] LoadAnimSets(GBAVV_ROM rom)
        {
            Unity_ObjectManager_GBAVV.AnimSet.Animation convertAnim(GBAVV_Map2D_Graphics graphics, GBAVV_Map2D_AnimSet animSet, GBAVV_Map2D_Animation anim, int i) => new Unity_ObjectManager_GBAVV.AnimSet.Animation(
                animFrameFunc: () => GetAnimFrames(animSet, i, graphics?.TileSet, graphics?.Palettes).Select(frame => frame.CreateSprite()).ToArray(),
                crashAnim: anim,
                xPos: animSet.GetMinX(i),
                yPos: animSet.GetMinY(i)
            );

            Unity_ObjectManager_GBAVV.AnimSet convertAnimSet(GBAVV_Map2D_Graphics graphics, GBAVV_Map2D_AnimSet animSet) => new Unity_ObjectManager_GBAVV.AnimSet(animSet.Animations.Select((anim, i) => convertAnim(graphics, animSet, anim, i)).ToArray());

            var animSets = rom.Map2D_Graphics?.Select(graphics => graphics.AnimSets.Select(animSet => convertAnimSet(graphics, animSet)).ToArray()).ToArray() ?? new Unity_ObjectManager_GBAVV.AnimSet[0][];

            // Create an anim set for Fake Crash for Crash 2
            if (rom.Context.Settings.EngineVersion == EngineVersion.GBAVV_Crash2)
            {
                var crash = rom.Map2D_Graphics[0].AnimSets[0];

                animSets[0] = animSets[0].Append(convertAnimSet(rom.Map2D_Graphics[0], new GBAVV_Map2D_AnimSet
                {
                    Animations = crash.Animations.Select(x => new GBAVV_Map2D_Animation
                    {
                        HitBox = x.HitBox,
                        RenderBox = x.RenderBox,
                        PaletteIndex = 5,
                        AnimSpeed = x.AnimSpeed,
                        FrameIndexTable = x.FrameIndexTable
                    }).ToArray(),
                    AnimationFrames = crash.AnimationFrames
                })).ToArray();
            }

            return animSets;
        }

        public Unity_ObjectManager_GBAVVMode7.AnimSet[] LoadMode7AnimSets(GBAVV_Mode7_LevelInfo level, RGBA5551Color[] tilePal)
        {
            var pal = Util.ConvertAndSplitGBAPalette(level.ObjPalette.Concat(tilePal).ToArray());

            var animSets = level.GetAllAnimSets.Select((animSet, animSetIndex) => new Unity_ObjectManager_GBAVVMode7.AnimSet(
                animations: animSet.Animations?.Select((anim, i) => new Unity_ObjectManager_GBAVVMode7.AnimSet.Animation(GetMode7AnimFrames(animSet, animSetIndex, i, pal, level).Select(frame => frame.CreateSprite()).ToArray())).ToArray() ?? new Unity_ObjectManager_GBAVVMode7.AnimSet.Animation[0]
            ));

            // Load special frames animation
            if (level.SpecialFrames != null)
            {
                animSets = animSets.Append(new Unity_ObjectManager_GBAVVMode7.AnimSet(new Unity_ObjectManager_GBAVVMode7.AnimSet.Animation[]
                {
                    new Unity_ObjectManager_GBAVVMode7.AnimSet.Animation(GetMode7SpecialAnimFrames(level.SpecialFrames).Select(x => x.CreateSprite()).ToArray()),
                }));
            }

            return animSets.ToArray();
        }

        public Unity_ObjectManager_GBAVVIsometric.GraphicsData[] LoadIsometricAnimations(GBAVV_ROM rom)
        {
            var pal = Util.ConvertAndSplitGBAPalette(rom.Isometric_GetObjPalette);

            return rom.Isometric_GetAnimations.Select(anim => new Unity_ObjectManager_GBAVVIsometric.GraphicsData(
                animFrames: GetIsometricAnimFrames(anim, pal).Select(x => x.CreateSprite()).ToArray(),
                animSpeed: 4,
                crashAnim: anim)).ToArray();
        }

        public Texture2D[] GetAnimFrames(GBAVV_Map2D_AnimSet animSet, int animIndex, byte[] tileSet, GBAVV_Map2D_ObjPal[] palettes)
        {
            // Get properties
            var shapes = TileShapes;
            var anim = animSet.Animations[animIndex];
            var frames = anim.FrameIndexTable.Select(x => animSet.AnimationFrames[x]).ToArray();
            var pal = palettes != null ? Util.ConvertGBAPalette(palettes[anim.PaletteIndex].Palette) : Util.ConvertGBAPalette(anim.Palette);

            // Return empty animation if there are no frames
            if (!frames.Any(x => x.TilesCount > 0))
                return new Texture2D[0];

            var output = new Texture2D[frames.Length];

            var minX = animSet.GetMinX(animIndex);
            var minY = animSet.GetMinY(animIndex);
            var maxX = frames.SelectMany(f => Enumerable.Range(0, f.TilesCount).Select(x => f.TilePositions[x].XPos + shapes[f.GetTileShape(x)].x)).Max();
            var maxY = frames.SelectMany(f => Enumerable.Range(0, f.TilesCount).Select(x => f.TilePositions[x].YPos + shapes[f.GetTileShape(x)].y)).Max();

            var width = maxX - minX;
            var height = maxY - minY;

            var frameCache = new Dictionary<GBAVV_Map2D_AnimationFrame, Texture2D>();

            for (int frameIndex = 0; frameIndex < frames.Length; frameIndex++)
            {
                var frame = frames[frameIndex];

                if (!frameCache.ContainsKey(frame))
                {
                    var tex = TextureHelpers.CreateTexture2D(width, height, clear: true);

                    int offset = (int)frame.TileOffset.Value;

                    for (int i = 0; i < frame.TilesCount; i++)
                    {
                        var shape = shapes[frame.GetTileShape(i)];
                        var pos = frame.TilePositions[i];

                        for (int y = 0; y < shape.y; y += 8)
                        {
                            for (int x = 0; x < shape.x; x += 8)
                            {
                                tex.FillInTile(
                                    imgData: tileSet ?? frame.Fusion_TileSet,
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

        public Texture2D[] GetMode7AnimFrames(GBAVV_Mode7_AnimSet animSet, int animSetIndex, int animIndex, Color[][] pal, GBAVV_Mode7_LevelInfo levInfo)
        {
            var palette = pal;

            if (levInfo.Context.Settings.EngineVersion == EngineVersion.GBAVV_Crash1 && animSetIndex == 0 && animIndex == 5)
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

        public Texture2D[] GetIsometricAnimFrames(GBAVV_Isometric_Animation anim, Color[][] palette)
        {
            if (anim.AnimFrames[0] == null)
                return new Texture2D[0];

            var output = new Texture2D[anim.AnimFrames.Length];
            var pal = anim.Palette != null ? Util.ConvertGBAPalette(anim.Palette) : palette[anim.PaletteIndex];

            for (int frameIndex = 0; frameIndex < anim.AnimFrames.Length; frameIndex++)
                output[frameIndex] = Util.ToTileSetTexture(anim.AnimFrames[frameIndex], pal, Util.TileEncoding.Linear_4bpp, CellSize, true, wrap: anim.Width / CellSize, flipX: anim.IsFlipped);

            return output;
        }

        public Dictionary<string, string[]> LoadLocalization(GBAVV_ROM rom)
        {
            var langages = new string[]
            {
                "English",
                "French",
                "German",
                "Spanish",
                "Italian",
                "Dutch"
            };

            return rom.LocTables?.Select((x, i) => new
            {
                Lang = langages[i],
                Strings = x.Strings
            }).ToDictionary(x => x.Lang, x => x.Strings);
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, GBA_ROMBase.Address_ROM);

        public void GetAvailableCollisionTypes(GBAVV_ROM rom)
        {
            var handledTypes = new HashSet<KeyValuePair<ushort, int>>();

            var levelIndex = 0;
            foreach (var level in rom.LevelInfos)
            {
                foreach (var map in level.LevelData.Maps)
                    handleMap(map);

                handleMap(level.LevelData.BonusMap);
                handleMap(level.LevelData.ChallengeMap);

                void handleMap(GBAVV_MapInfo map)
                {
                    if (map?.MapData2D == null)
                        return;

                    var collisionTiles = GetTileMap(map.MapData2D.CollisionLayer, map.MapData2D.LayersBlock.CollisionLayerData, false, isCollision: true);

                    foreach (var c in collisionTiles.Select(x => x.Data.CollisionType))
                    {
                        if (handledTypes.Any(x => x.Key == c))
                            continue;

                        handledTypes.Add(new KeyValuePair<ushort, int>(c, levelIndex));
                    }
                }

                levelIndex++;
            }

            var str = new StringBuilder();

            foreach (var c in handledTypes.OrderBy(x => BitHelpers.ExtractBits(x.Key, 8, 8)).ThenBy(x => BitHelpers.ExtractBits(x.Key, 8, 0)))
                str.AppendLine($"Type: {BitHelpers.ExtractBits(c.Key, 8, 8):00}, Shape: {BitHelpers.ExtractBits(c.Key, 8, 0):00}, Value: {c.Key:0000}, Level: {c.Value:00}");

            str.ToString().CopyToClipboard();
        }

        public void GetCommonIsometricCollisionIndices(IEnumerable<GBAVV_Isometric_CollisionType[]> types)
        {
            var commonTypes = new List<KeyValuePair<Pointer, byte[]>>();

            var levelIndex = 0;

            var output = new StringBuilder();

            foreach (var levelTypes in types)
            {
                output.AppendLine($"case {levelIndex}:");
                output.AppendLine($"    switch (index)");
                output.AppendLine($"    {{");

                var tIndex = 0;
                foreach (var t in levelTypes)
                {
                    if (!commonTypes.Any(x => x.Key == t.FunctionPointer_0 && x.Value.SequenceEqual(t.Bytes_10)))
                        commonTypes.Add(new KeyValuePair<Pointer, byte[]>(t.FunctionPointer_0, t.Bytes_10));

                    output.AppendLine($"        case {tIndex}: return {commonTypes.FindIndex(x => x.Key == t.FunctionPointer_0 && x.Value.SequenceEqual(t.Bytes_10))};");
                    tIndex++;
                }

                output.AppendLine($"    }}");
                output.AppendLine($"    break;");

                levelIndex++;
            }

            output.ToString().CopyToClipboard();
        }

        public class LevInfo
        {
            public LevInfo(int levelIndex, int mapIndex, string displayName)
            {
                LevelIndex = levelIndex;
                MapIndex = mapIndex;
                MapType = Type.Normal;
                DisplayName = displayName;
            }
            public LevInfo(int levelIndex, string displayName, FusionType fusionType = FusionType.Normal) : this(levelIndex, 0, displayName)
            {
                Fusion_Type = fusionType;
            }
            public LevInfo(int levelIndex, Type mapType, string displayName)
            {
                LevelIndex = levelIndex;
                MapType = mapType;
                DisplayName = $"{displayName} - {mapType.ToString().Replace("Challenge", "Gem Route")}";
            }
            public LevInfo(GBAVV_MapInfo.GBAVV_MapType specialMapType, short index3D, string displayName)
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
            public GBAVV_MapInfo.GBAVV_MapType SpecialMapType { get; }
            public short Index3D { get; }
            public FusionType Fusion_Type { get; }

            public string DisplayName { get; set; }

            public enum Type
            {
                Normal,
                Bonus,
                Challenge
            }

            public enum FusionType
            {
                Normal,
                LevTime,
                LevInt,
                LevIntInt,
                IntLevel,
                Unknown
            }
        }

        public static Vector2Int[] TileShapes { get; } = new Vector2Int[]
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