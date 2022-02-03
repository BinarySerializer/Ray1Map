using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinarySerializer;
using BinarySerializer.GBA;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro1_Manager : GBAIsometric_IceDragon_BaseManager
    {
        public const int World_Levels3D = 0;
        public const int World_Mode7 = 1;
        public const int World_Sparx = 2;
        public const int World_Cutscenes = 3;

        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(World_Levels3D, Enumerable.Range(0, 17).ToArray()), // Levels 3D
            //new GameInfo_World(World_Mode7, Enumerable.Range(0, 4).ToArray()), // Mode7
            new GameInfo_World(World_Sparx, Enumerable.Range(0, 5).ToArray()), // Sparx
            new GameInfo_World(World_Cutscenes, Enumerable.Range(0, 21).ToArray()), // Cutscenes
        });

        public override GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Resources", false, true, (input, output) => ExportResourcesAsync(settings, output, false)),
            new GameAction("Export Resources (categorized)", false, true, (input, output) => ExportResourcesAsync(settings, output, true)),
            new GameAction("Export Cutscene Maps", false, true, (input, output) => ExportCutsceneMapsAsync(settings, output)),
            new GameAction("Find and Export Sprites (no pal)", false, true, (input, output) => FindAndExportAllSpritesAsync(settings, output)),
            new GameAction("Export Sprites", false, true, (input, output) => ExportSpritesAsync(settings, output, false)),
            new GameAction("Export Sprite Animations as GIF", false, true, (input, output) => ExportSpritesAsync(settings, output, true)),
            new GameAction("Export Sparx Animation Frames", false, true, (input, output) => ExportSparxAnimSets(settings, output, false)),
            new GameAction("Export Sparx Animations as GIF", false, true, (input, output) => ExportSparxAnimSets(settings, output, true)),
            new GameAction("Export Portraits", false, true, (input, output) => ExportPortraitsAsync(settings, output)),
            new GameAction("Export Scripts", false, true, (input, output) => ExportScriptsAsync(settings, output)),
            new GameAction("Export Font", false, true, (input, output) => ExportFont(settings, output)),
            new GameAction("Export Strings", false, true, (input, output) => ExportStrings(settings, output)),
            new GameAction("Export Localization", false, true, (input, output) => ExportLocalization(settings, output)),
            new GameAction("Export All Graphics", false, true, (input, output) => ExportAllGraphicsAsync(settings, output)),
        };

        public async UniTask ExportResourcesAsync(GameSettings settings, string outputPath, bool categorize)
        {
            await DoGameActionAsync<GBAIsometric_Ice_ROM>(settings, (rom, context) =>
            {
                ExportResources(context, rom.Resources, outputPath, categorize, false);
            });
        }

        public async UniTask ExportCutsceneMapsAsync(GameSettings settings, string outputPath)
        {
            await DoGameActionAsync<GBAIsometric_Ice_ROM>(settings, (rom, _) =>
            {
                for (var i = 0; i < rom.CutsceneMaps.Length; i++)
                {
                    GBAIsometric_IceDragon_CutsceneMap cutscene = rom.CutsceneMaps[i];
                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{i}.png"), cutscene.ToTexture2D().EncodeToPNG());
                }
            });
        }

        public async UniTask FindAndExportAllSpritesAsync(GameSettings settings, string outputPath)
        {
            using var context = new Ray1MapContext(settings);

            BinaryDeserializer s = context.Deserializer;
            await LoadFilesAsync(context);

            Color[] pal4 = PaletteHelpers.CreateDummyPalette(16).Select(x => x.GetColor()).ToArray();

            s.Goto(context.FilePointer(GetROMFilePath) + 3);

            while (s.CurrentFileOffset < s.CurrentLength - 4)
            {
                string str = s.SerializeString(default, 3);
                s.Goto(s.CurrentPointer + 1);
                
                if (str != "CRS")
                    continue;

                try
                {
                    s.DoAt(s.CurrentPointer - 7, () =>
                    {
                        GBAIsometric_Ice_SpriteSet spriteSet = s.SerializeObject<GBAIsometric_Ice_SpriteSet>(default);

                        for (int i = 0; i < spriteSet.Sprites.Length; i++)
                        {
                            Texture2D tex = GetSpriteTexture(spriteSet, i, pal4);
                            Util.ByteArrayToFile(Path.Combine(outputPath, $"0x{spriteSet.Offset.StringAbsoluteOffset}", $"{i}.png"), tex.EncodeToPNG());
                        }
                    });
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"{s.CurrentPointer}: {ex}");
                }
            }
        }

        public async UniTask ExportSpritesAsync(GameSettings settings, string outputPath, bool saveAsGif)
        {
            await DoGameActionAsync<GBAIsometric_Ice_ROM>(settings, rom => rom.Pre_SerializeSprites = true, (rom, context) =>
            {
                Color[] pal4 = PaletteHelpers.CreateDummyPalette(16).Select(x => x.GetColor()).ToArray();

                for (int spriteSetIndex = 0; spriteSetIndex < rom.SpriteSets.Length; spriteSetIndex++)
                {
                    GBAIsometric_Ice_SpriteSet spriteSet = rom.SpriteSets[spriteSetIndex];

                    Color[] pal = rom.SpriteSetPalettes[spriteSetIndex] == null
                        ? pal4
                        : Util.ConvertGBAPalette(rom.SpriteSetPalettes[spriteSetIndex].Value.Colors);

                    Texture2D[] textures = spriteSet.Sprites.Select((s, i) => GetSpriteTexture(spriteSet, i, pal)).ToArray();

                    sbyte minX = spriteSet.Sprites.Min(x => x.XPos);
                    sbyte minY = spriteSet.Sprites.Min(x => x.YPos);

                    Util.ExportAnim(
                        frames: textures,
                        speed: 4,
                        center: false,
                        saveAsGif: saveAsGif,
                        outputDir: outputPath,
                        primaryName: $"{spriteSetIndex}",
                        frameOffsets: spriteSet.Sprites.Select(x => new Vector2Int(x.XPos - minX, x.YPos - minY)).ToArray(),
                        trim: false);
                }
            });
        }

        public async UniTask ExportSparxAnimSets(GameSettings settings, string outputPath, bool saveAsGif)
        {
            await DoGameActionAsync<GBAIsometric_Ice_ROM>(settings, rom =>
            {
                rom.Pre_SerializeSparx = true;
                rom.Pre_SparxIndex = -2; // Don't serialize any levels
            }, (rom, context) =>
            {
                Color[][] objPal = Util.ConvertAndSplitGBAPalette(rom.Sparx_ObjPalette.Colors);

                for (int animSetIndex = 0; animSetIndex < rom.Sparx_ObjectTypes.Length; animSetIndex++)
                {
                    Sparx_ObjectType objType = rom.Sparx_ObjectTypes[animSetIndex];
                    Sparx_AnimSet animSet = objType.AnimSet;

                    if (animSet == null)
                        continue;

                    for (int animIndex = 0; animIndex < animSet.Animations.Length; animIndex++)
                    {
                        Sparx_Animation anim = animSet.Animations[animIndex];
                        Texture2D[] frames;

                        (frames, _) = GetAnimFrames(anim, objPal[objType.PaletteIndex]);

                        if (frames == null)
                            continue;

                        Util.ExportAnim(
                            frames: frames,
                            speed: 4,
                            center: false,
                            saveAsGif: saveAsGif,
                            outputDir: Path.Combine(outputPath, "Final"),
                            primaryName: $"{animSetIndex}",
                            secondaryName: $"{animIndex}");

                        (frames, _) = GetAnimFrames(anim);

                        Util.ExportAnim(
                            frames: frames,
                            speed: 4,
                            center: false,
                            saveAsGif: saveAsGif,
                            outputDir: Path.Combine(outputPath, "Source"),
                            primaryName: $"{animSetIndex}",
                            secondaryName: $"{animIndex}");
                    }
                }
            });
        }

        public async UniTask ExportPortraitsAsync(GameSettings settings, string outputPath)
        {
            await DoGameActionAsync<GBAIsometric_Ice_ROM>(settings, x => x.Pre_SerializePortraits = true, (rom, _) =>
            {
                for (int i = 0; i < rom.PortraitTileMaps.Length; i++)
                {
                    Color[] pal = Util.ConvertGBAPalette(rom.PortraitPalettes[i].Value.Colors);
                    BinarySerializer.GBA.MapTile[] map = rom.PortraitTileMaps[i].Value;
                    byte[] tileSet = rom.PortraitTileSets[i].Value;

                    Texture2D tex = TextureHelpers.CreateTexture2D(GBAConstants.TileSize * 4, GBAConstants.TileSize * 4);

                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            int tile = map[y * 4 + x].TileIndex;
                            tex.FillInTile(tileSet, tile * 0x20, pal, Util.TileEncoding.Linear_4bpp, GBAConstants.TileSize, true, x * GBAConstants.TileSize, y * GBAConstants.TileSize);
                        }
                    }

                    tex.Apply();

                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{i}.png"), tex.EncodeToPNG());
                }
            });
        }

        public async UniTask ExportScriptsAsync(GameSettings settings, string outputPath)
        {
            await DoGameActionAsync<GBAIsometric_Ice_ROM>(settings, (rom, context) =>
            {
                ExportScripts(context, null, rom.MenuPages, outputPath);
            });
        }

        public async UniTask ExportFont(GameSettings settings, string outputPath)
        {
            await DoGameActionAsync<GBAIsometric_Ice_ROM>(settings, (rom, _) =>
            {
                ExportFont(rom.Localization, outputPath);
            });
        }

        public async UniTask ExportStrings(GameSettings settings, string outputPath)
        {
            await DoGameActionAsync<GBAIsometric_Ice_ROM>(settings, (rom, context) =>
            {
                ExportStrings(context, rom.Localization, outputPath);
            });
        }

        public async UniTask ExportLocalization(GameSettings settings, string outputPath)
        {
            await DoGameActionAsync<GBAIsometric_Ice_ROM>(settings, (rom, context) =>
            {
                JsonHelpers.SerializeToFile(LoadLocalization(context, rom.Localization), Path.Combine(outputPath, "Localization.json"));
            });
        }

        public async UniTask ExportAllGraphicsAsync(GameSettings settings, string outputPath)
        {
            await ExportCutsceneMapsAsync(settings, Path.Combine(outputPath, "Cutscenes"));
            await ExportSpritesAsync(settings, Path.Combine(outputPath, "Sprites"), false);
            await ExportSpritesAsync(settings, Path.Combine(outputPath, "Sprite Animations"), true);
            await ExportSparxAnimSets(settings, Path.Combine(outputPath, "Sparx Animation Frames"), false);
            await ExportSparxAnimSets(settings, Path.Combine(outputPath, "Sparx Animations"), true);
            await ExportPortraitsAsync(settings, Path.Combine(outputPath, "Portraits"));
            await ExportFont(settings, Path.Combine(outputPath, "Font"));
        }

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            Controller.DetailedState = $"Loading data";
            await Controller.WaitIfNecessary();

            GameSettings settings = context.GetR1Settings();
            int world = settings.World;
            int level = settings.Level;

            return world switch
            {
                World_Levels3D => await LoadLevel3DAsync(context, level),
                World_Mode7 => throw new NotImplementedException("Mode7 levels are currently not supported"),
                World_Sparx => await LoadSparxAsync(context, level),
                World_Cutscenes => await LoadCutsceneMapAsync(context, FileFactory.Read<GBAIsometric_Ice_ROM>(context, GetROMFilePath)),
                _ => throw new InvalidOperationException("Invalid world")
            };
        }

        public async UniTask<Unity_Level> LoadLevel3DAsync(Context context, int level)
        {
            // Read the ROM
            GBAIsometric_Ice_ROM rom = FileFactory.Read<GBAIsometric_Ice_ROM>(context, GetROMFilePath, (s, r) =>
            {
                r.Pre_SerializeLevel3D = true;
                r.Pre_Level3DIndex = level;
                r.Pre_SerializeSprites = true;
            });

            // Get the level data
            GBAIsometric_Ice_Level3D_MapLayers mapLayers = rom.Level3D_MapLayers[level];
            MapTile[][] mapTiles = mapLayers.Layers.Select(x => x.Value.GetFullMap().Select(m => new MapTile
            {
                TileMapY = (ushort)(m.TileIndex + x.Value.CharacterBaseBlock * 512),
                HorizontalFlip = m.FlipX,
                VerticalFlip = m.FlipY,
                PaletteIndex = (byte)m.PaletteIndex,
            }).ToArray()).ToArray();
            byte[] tileSetData = rom.Level3D_TileSets[level].Value;
            Palette pal = rom.Level3D_Palettes[level];

            // Create the level
            var lev = new Unity_Level();

            Controller.DetailedState = $"Loading tileset";
            await Controller.WaitIfNecessary();

            Unity_TileSet tileSet = LoadTileSet(pal.Colors, tileSetData, mapTiles.SelectMany(x => x));

            Controller.DetailedState = $"Loading collision";
            await Controller.WaitIfNecessary();

            const float isoTileDiagonal = 8 / 2f; // 8 tiles, divide by 2 as 1 tile = half unit
            float isoTileWidth = Mathf.Sqrt(isoTileDiagonal * isoTileDiagonal / 2); // Side of square = sqrt(diagonal^2 / 2)

            lev.IsometricData = new Unity_IsometricData
            {
                //TilesWidth = mapLayers.Layers.Max(x => x.Value.Width),
                //TilesHeight = mapLayers.Layers.Max(x => x.Value.Height),
                CollisionObjects = rom.Level3D_MapCollision[level].Value.Items.
                    // Ignore boxes with lines as they seem to just be for layering?
                    Where(x => x.Lines.Length == 0).Select(x =>
                    {
                        float factor = lev.PixelsPerUnit;
                        float height = (float)x.Height / (1 << 14);

                        return new Unity_IsometricCollisionObject
                        {
                            Points = new Vector3[]
                                {
                                new Vector3(x.MinX, x.MinY, height) / factor,
                                new Vector3(x.MaxX, x.MinY, height) / factor,
                                new Vector3(x.MaxX, x.MaxY, height) / factor,
                                new Vector3(x.MinX, x.MaxY, height) / factor,
                        }
                        };
                    }).ToArray(),
                Scale = new Vector3(isoTileWidth, 1f / Mathf.Cos(Mathf.Deg2Rad * 30f), isoTileWidth) // Height = 1.15 tiles, Length of the diagonal of 1 block = 8 tiles
            };

            Controller.DetailedState = $"Loading maps";
            await Controller.WaitIfNecessary();

            IEnumerable<Unity_Map> maps = mapLayers.Layers.Select((map, i) =>
            {
                return new Unity_Map()
                {
                    Type = Unity_Map.MapType.Graphics,
                    Width = map.Value.Width,
                    Height = map.Value.Height,
                    TileSet = new Unity_TileSet[] { tileSet },
                    MapTiles = mapTiles[i].Select(x => new Unity_Tile(x)).ToArray(),
                };
            }).Reverse();
            lev.CellSize = GBAConstants.TileSize;

            // Add the level map if available
            if (rom.Level3D_LevelMaps != null && rom.Level3D_LevelMaps.Length > level && Settings.LoadIsometricMapLayer)
            {
                GBAIsometric_Ice_Level3D_LevelMap lvlMap = rom.Level3D_LevelMaps[level];

                Texture2D tileSetTex = Util.ToTileSetTexture(
                    imgData: lvlMap.ImgData,
                    pal: Util.ConvertGBAPalette(lvlMap.Palette.Colors),
                    encoding: Util.TileEncoding.Linear_4bpp,
                    tileWidth: GBAConstants.TileSize,
                    flipY: false);

                maps = maps.Append(new Unity_Map()
                {
                    Type = Unity_Map.MapType.Graphics,
                    Width = 30,
                    Height = 20,
                    TileSet = new Unity_TileSet[]
                    {
                            new Unity_TileSet(tileSetTex, CellSize)
                    },
                    MapTiles = Enumerable.Range(0, 30 * 20).Select(x => new Unity_Tile(new MapTile()
                    {
                        TileMapY = (ushort)x
                    })).ToArray()
                });
            }

            lev.Maps = maps.ToArray();

            Controller.DetailedState = $"Loading objects";
            await Controller.WaitIfNecessary();

            Color[] dummyPal = PaletteHelpers.CreateDummyPalette(16).Select(x => x.GetColor()).ToArray();

            var objManager = new Unity_ObjectManager_GBAIsometricSpyro1(
                context: context,
                spriteSets: rom.SpriteSets.
                    Select((x, spriteSetIndex) => new Unity_ObjectManager_GBAIsometricSpyro1.SpriteSet(
                        spriteSetObj: x,
                        animFramesFunc: () => x.Value.Sprites.
                            Select((s, spriteIndex) => GetSpriteTexture(x, spriteIndex,
                                    rom.SpriteSetPalettes[spriteSetIndex] == null
                                            ? dummyPal
                                            : Util.ConvertGBAPalette(rom.SpriteSetPalettes[spriteSetIndex].Value.Colors)).
                                    CreateSprite()).ToArray())).ToArray());

            lev.ObjManager = objManager;

            // Load Spyro object
            GBAIsometric_Ice_Vector startPos = rom.Level3D_StartPositions[level];
            lev.Rayman = new Unity_Object_GBAIsometricSpyro1_Level3D(new GBAIsometric_Ice_Level3D_Object(1, startPos), objManager);

            // NOTE: Missing objects include mission objects, fairies and NPCs. They are all sadly hard-coded per level.

            // Load level objects
            foreach (GBAIsometric_Ice_Level3D_Object obj in rom.Level3D_Objects[level].Value.Objects)
                lev.EventData.Add(new Unity_Object_GBAIsometricSpyro1_Level3D(obj, objManager));

            Controller.DetailedState = $"Loading localization";
            await Controller.WaitIfNecessary();

            lev.Localization = LoadLocalization(context, rom.Localization);

            return lev;
        }

        public async UniTask<Unity_Level> LoadSparxAsync(Context context, int level)
        {
            // Read the ROM
            GBAIsometric_Ice_ROM rom = FileFactory.Read<GBAIsometric_Ice_ROM>(context, GetROMFilePath, (s, r) =>
            {
                r.Pre_SerializeSparx = true;
                r.Pre_SparxIndex = level;
            });

            // Get the level data
            GBAIsometric_Ice_Sparx_LevelData levelData = level == 4 ? rom.Sparx_MenuMap : rom.Sparx_Levels[level];

            // Create the level
            var lev = new Unity_Level();

            var mapLayers = levelData.Maps.Select((map, i) => new
            {
                Width = map.Value.Width * 2,
                Height = map.Value.Height * 2,
                Index = i,
                TileMap = map.Value.GetMap(levelData.TileSetMap).Select(x => new MapTile
                {
                    TileMapY = (ushort)x.TileIndex,
                    HorizontalFlip = x.FlipX,
                    VerticalFlip = x.FlipY,
                    PaletteIndex = (byte)x.PaletteIndex,
                }).ToArray()
            }).ToArray();

            Controller.DetailedState = $"Loading tileset";
            await Controller.WaitIfNecessary();

            Unity_TileSet tileSet = LoadTileSet(levelData.Palette.Value.Colors, levelData.TileSet.Value.ImgData, mapLayers.SelectMany(x => x.TileMap));

            Controller.DetailedState = $"Loading maps";
            await Controller.WaitIfNecessary();

            // Add maps
            IEnumerable<Unity_Map> maps = mapLayers.Select(map =>
            {
                return new Unity_Map()
                {
                    Type = Unity_Map.MapType.Graphics,
                    Layer = map.Index > 0 ? Unity_Map.MapLayer.Front : Unity_Map.MapLayer.Middle,
                    Width = (ushort)map.Width,
                    Height = (ushort)map.Height,
                    TileSet = new Unity_TileSet[] { tileSet },
                    MapTiles = map.TileMap.Select(x => new Unity_Tile(x)).ToArray(),
                };
            });

            // Add collision map
            if (levelData.ObjectMap != null)
            {
                maps = maps.Append(new Unity_Map()
                {
                    Type = Unity_Map.MapType.Collision,
                    Width = levelData.ObjectMap.Value.Width,
                    Height = levelData.ObjectMap.Value.Height,
                    TileSet = Array.Empty<Unity_TileSet>(),
                    MapTiles = levelData.ObjectMap.Value.MapData.Select(x => new Unity_Tile(new MapTile()
                    {
                        CollisionType = (ushort)(x < 0x40 ? x : 0)
                    })).ToArray(),
                });
            }

            lev.CellSize = GBAConstants.TileSize;
            lev.CellSizeOverrideCollision = GBAConstants.TileSize * 2;
            lev.GetCollisionTypeGraphicFunc = x => (GBAIsometric_Ice_Sparx_CollisionType)x switch
            {
                GBAIsometric_Ice_Sparx_CollisionType.None => Unity_MapCollisionTypeGraphic.None,
                GBAIsometric_Ice_Sparx_CollisionType.Solid => Unity_MapCollisionTypeGraphic.Solid,
                GBAIsometric_Ice_Sparx_CollisionType.Wall => Unity_MapCollisionTypeGraphic.Passthrough,
                GBAIsometric_Ice_Sparx_CollisionType.BossTrigger => Unity_MapCollisionTypeGraphic.Reactionary,
                _ => Unity_MapCollisionTypeGraphic.Unknown0
            };
            lev.GetCollisionTypeNameFunc = x => ((GBAIsometric_Ice_Sparx_CollisionType)x).ToString();
            lev.Maps = maps.ToArray();

            Controller.DetailedState = $"Loading objects";
            await Controller.WaitIfNecessary();

            // Add object graphics
            var animSets = new List<Unity_ObjectManager_GBAIsometricSpyro1_Sparx.AnimSet>();

            Color[][] objPal = Util.ConvertAndSplitGBAPalette(rom.Sparx_ObjPalette.Colors);

            foreach (Sparx_ObjectType objType in rom.Sparx_ObjectTypes)
            {
                Sparx_Animation[] anims = objType.AnimSet.Value?.Animations;

                animSets.Add(new Unity_ObjectManager_GBAIsometricSpyro1_Sparx.AnimSet(anims?.
                    Select(anim =>
                    {
                        Texture2D[] frames;
                        Vector2Int offset;

                        (frames, offset) = GetAnimFrames(anim, objPal[objType.PaletteIndex]);

                        return new Unity_ObjectManager_GBAIsometricSpyro1_Sparx.AnimSet.Animation(
                            frames: frames?.Select(t => t.CreateSprite()).ToArray() ?? Array.Empty<Sprite>(),
                            offset: offset);
                    }).
                    ToArray() ?? Array.Empty<Unity_ObjectManager_GBAIsometricSpyro1_Sparx.AnimSet.Animation>()));
            }

            var objManager = new Unity_ObjectManager_GBAIsometricSpyro1_Sparx(context, animSets.ToArray(), rom.Sparx_ObjectTypes);
            lev.ObjManager = objManager;

            // Add objects
            if (levelData.ObjectMap != null)
            {
                GBAIsometric_Ice_Sparx_MapLayer map = levelData.ObjectMap;

                for (int y = 0; y < map.Height; y++)
                {
                    for (int x = 0; x < map.Width; x++)
                    {
                        byte objValue = map.MapData[y * map.Width + x];

                        if (objValue >= 0x40)
                        {
                            var objType = objValue - 0x40;

                            var obj = new Unity_Object_GBAIsometricSpyro1_Sparx(objType, objManager)
                            {
                                XPosition = (short)(x * GBAConstants.TileSize * 2),
                                YPosition = (short)(y * GBAConstants.TileSize * 2),
                            };

                            lev.EventData.Add(obj);

                            // Blocker has a child object
                            if (objType >= 17 && objType <= 24)
                            {
                                // Horizontal
                                if (obj.AnimIndex == 1)
                                {
                                    lev.EventData.Add(new Unity_Object_GBAIsometricSpyro1_Sparx(objType, objManager)
                                    {
                                        XPosition = (short)(obj.XPosition - 48),
                                        YPosition = obj.YPosition,
                                        AnimIndex = 0,
                                    });
                                }
                                // Vertical
                                else
                                {
                                    lev.EventData.Add(new Unity_Object_GBAIsometricSpyro1_Sparx(objType, objManager)
                                    {
                                        XPosition = obj.XPosition,
                                        YPosition = (short)(obj.YPosition - 48),
                                        AnimIndex = 2,
                                    });
                                }
                            }
                        }
                    }
                }
            }

            Controller.DetailedState = $"Loading localization";
            await Controller.WaitIfNecessary();

            lev.Localization = LoadLocalization(context, rom.Localization);

            return lev;
        }

        public Texture2D GetSpriteTexture(GBAIsometric_Ice_SpriteSet spriteSet, int spriteIndex, Color[] pal)
        {
            // 8-bit sprites are never used
            if (spriteSet.Is8Bit)
                throw new InvalidOperationException("8-bit sprites are currently not supported");

            GBAIsometric_Ice_Sprite sprite = spriteSet.Sprites[spriteIndex];
            int shape = 0;

            if (sprite.Height < sprite.Width)
                shape = 1;
            else if (sprite.Width < sprite.Height)
                shape = 2;

            GBAConstants.Size size = GBAConstants.GetSpriteShape(shape, sprite.SpriteSize);

            int spritesWidth = 1;
            int spritesHeight = 1;

            // The 3 sprite sets with the map length of 256 consist of 4 OAM sprites for a single frame as a 2x2 image.
            // This is a hacky fix for it as I'm unsure how the game handles it. Appears to be hard-coded?
            if (spriteSet.SpriteMapLength == 256)
            {
                if (sprite.Height < sprite.Width)
                {
                    spritesWidth = 2;
                    spritesHeight = 1;
                }
                else if (sprite.Width < sprite.Height)
                {
                    spritesWidth = 1;
                    spritesHeight = 2;
                }
                else
                {
                    spritesWidth = 2;
                    spritesHeight = 2;
                }

                size = new GBAConstants.Size(64, 64);
            }

            Texture2D tex = TextureHelpers.CreateTexture2D(size.Width * spritesWidth, size.Height * spritesHeight, clear: true);

            int imgDataOffset = sprite.TileIndex * (spriteSet.Is8Bit ? 0x40 : 0x20);
            int mapIndex = spriteSet.SpriteMapLength * spriteIndex;

            for (int spriteY = 0; spriteY < spritesHeight; spriteY++)
            {
                for (int spriteX = 0; spriteX < spritesWidth; spriteX++)
                {
                    for (int y = 0; y < size.Height / GBAConstants.TileSize; y++)
                    {
                        for (int x = 0; x < size.Width / GBAConstants.TileSize; x++)
                        {
                            if (!spriteSet.SpriteMaps[mapIndex])
                            {
                                mapIndex++;
                                continue;
                            }

                            tex.FillInTile(
                                imgData: spriteSet.ImgData,
                                imgDataOffset: imgDataOffset,
                                pal: pal,
                                encoding: Util.TileEncoding.Linear_4bpp,
                                tileWidth: GBAConstants.TileSize,
                                flipTextureY: true,
                                tileX: spriteX * size.Width + x * GBAConstants.TileSize,
                                tileY: spriteY * size.Height + y * GBAConstants.TileSize);

                            imgDataOffset += spriteSet.Is8Bit ? 0x40 : 0x20;

                            mapIndex++;
                        }
                    }
                }
            }

            tex.Apply();

            return tex;
        }

        public (Texture2D[] Frames, Vector2Int Offset) GetAnimFrames(Sparx_Animation anim, Color[] overridePal = null)
        {
            // Get the frames
            Sparx_Frame[] frames = anim.Frames.TakeWhile(x => x.Sprites != null).ToArray();

            if (!frames.Any())
                return default;

            var output = new Texture2D[frames.Length];

            int minX = frames.SelectMany(x => x.Sprites).
                Select(sprite => new { Graphics = sprite.GraphicsPointer.Value, sprite.XPos }).
                SelectMany(sprite => sprite.Graphics.Parts.
                    Select(part => part.XPos + sprite.XPos)).
                Min();
            int minY = frames.SelectMany(x => x.Sprites).
                Select(sprite => new { Graphics = sprite.GraphicsPointer.Value, sprite.YPos }).
                SelectMany(sprite => sprite.Graphics.Parts.
                    Select(part => part.YPos + sprite.YPos)).
                Min();
            int maxX = frames.SelectMany(x => x.Sprites).
                Select(sprite => new { Graphics = sprite.GraphicsPointer.Value, sprite.XPos }).
                SelectMany(sprite => sprite.Graphics.Parts.
                    Select(part => part.XPos + sprite.XPos + part.Attribute.GetSpriteShape().Width)).
                Max();
            int maxY = frames.SelectMany(x => x.Sprites).
                Select(sprite => new { Graphics = sprite.GraphicsPointer.Value, sprite.YPos }).
                SelectMany(sprite => sprite.Graphics.Parts.
                    Select(part => part.YPos + sprite.YPos + part.Attribute.GetSpriteShape().Height)).
                Max();

            int width = maxX - minX;
            int height = maxY - minY;

            for (int frameIndex = 0; frameIndex < frames.Length; frameIndex++)
            {
                Sparx_Frame frame = frames[frameIndex];

                Texture2D tex = TextureHelpers.CreateTexture2D(width, height, clear: true);

                foreach (Sparx_Sprite sprite in frame.Sprites)
                {
                    Sparx_SpriteGraphics spriteGraphics = sprite.GraphicsPointer;

                    Color[] pal = overridePal ?? Util.ConvertGBAPalette(spriteGraphics.Palette.Value.Colors);

                    foreach (Sparx_SpriteGraphicsPart part in spriteGraphics.Parts)
                    {
                        int offset = (int)part.TileSetOffset;

                        var size = part.Attribute.GetSpriteShape();

                        for (int y = 0; y < size.Height; y += 8)
                        {
                            for (int x = 0; x < size.Width; x += 8)
                            {
                                tex.FillInTile(
                                    imgData: spriteGraphics.TileSet,
                                    imgDataOffset: offset,
                                    pal: pal,
                                    encoding: Util.TileEncoding.Linear_4bpp,
                                    tileWidth: GBAConstants.TileSize,
                                    flipTextureY: true,
                                    tileX: part.XPos + sprite.XPos + x - minX,
                                    tileY: part.YPos + sprite.YPos + y - minY);

                                offset += 0x20;
                            }
                        }
                    }
                }

                tex.Apply();

                output[frameIndex] = tex;
            }

            return (output, new Vector2Int(minX, minY));
        }

        public void PatchJPROM(
            string baseDir, string jpName, string euName, uint startOffset, 
            bool replacePortraits, bool replaceCutscenes)
        {
            // Changes made:

            // Text:
            // 0x08279c50 - Replaced font tiles (<)
            // 0x0827c2f0 - Replaced font map (<)
            // 0x8275C00 -> 0x08835D28 (0x0884DB04 for the maps edit patch) - Replaced text (remapped, pointer at 0x08002c60)

            // Portraits (excluding 8 & 23):
            // 0x081b0b58 - Replaced palettes (16)
            // 0x081b0bb8 - Replaced maps (4x4)
            // 0x08063614 - Replaced tile set lengths (24)
            // 0x081b0af8 - Replaced tile sets (some have been remapped)

            // Cutscenes
            // Replaced palettes, remapped tile sets and maps

            // Logo
            // 0x082de200 - Replaced tile map
            // 0x082de05c - Replaced palette
            // 0x082da5b8 -> 0x08831D0C - Replaced tile set (remapped, pointer at 0x0800e59c)

            // Level maps (maps edit patch only)
            // Remapped all level maps to 0x08835D28

            using var euContext = new Ray1MapContext(baseDir, new GameSettings(GameModeSelection.SpyroSeasonIceEU, baseDir, 0, 0));
            using var jpContext = new Ray1MapContext(baseDir, new GameSettings(GameModeSelection.SpyroSeasonIceJP, baseDir, 0, 0));

            euContext.AddFile(new GBAMemoryMappedFile(euContext, euName, GBAConstants.Address_ROM));
            jpContext.AddFile(new GBAMemoryMappedFile(jpContext, jpName, GBAConstants.Address_ROM)
            {
                RecreateOnWrite = false
            });

            GBAIsometric_Ice_ROM euRom = FileFactory.Read<GBAIsometric_Ice_ROM>(euContext, euName, 
                (_, x) => x.Pre_SerializePortraits = true);
            GBAIsometric_Ice_ROM jpRom = FileFactory.Read<GBAIsometric_Ice_ROM>(jpContext, jpName, 
                (_, x) => x.Pre_SerializePortraits = true);

            var d = euContext.Deserializer;
            var s = jpContext.Serializer;

            Pointer remapOffset = new Pointer(startOffset, jpContext.GetFile(jpName));
            Dictionary<Spyro_DefinedPointer, Pointer> jpPointers = PointerTables.GBAIsometric_Spyro_PointerTable(
                jpContext.GetR1Settings().GameModeSelection, jpContext.GetFile(jpName));

            // Replace portraits
            if (replacePortraits)
            {
                for (int i = 0; i < 24; i++)
                {
                    // Replace palette
                    s.Goto(jpRom.PortraitPalettes[i]);
                    s.SerializeObject<Palette>(euRom.PortraitPalettes[i]);

                    // Replace map
                    s.Goto(jpRom.PortraitTileMaps[i]);
                    s.SerializeObject<ObjectArray<BinarySerializer.GBA.MapTile>>(euRom.PortraitTileMaps[i]);

                    // Replace tile set
                    if (euRom.PortraitTileSetLengths[i] == jpRom.PortraitTileSetLengths[i])
                    {
                        s.Goto(jpRom.PortraitTileSets[i]);
                        s.SerializeObject<Array<byte>>(euRom.PortraitTileSets[i]);
                    }
                    else
                    {
                        // Update pointer
                        s.Goto(jpPointers[Spyro_DefinedPointer.Ice_PortraitTileSets] + 4 * i);
                        s.SerializePointer(remapOffset);

                        s.Goto(remapOffset);
                        s.SerializeObject<Array<byte>>(euRom.PortraitTileSets[i]);

                        Debug.Log($"Remapped portrait {i} to 0x{remapOffset.StringAbsoluteOffset}");

                        s.Align();
                        remapOffset = s.CurrentPointer;
                    }

                    // Replace tile set length
                    s.Goto(jpPointers[Spyro_DefinedPointer.Ice_PortraitTileSetLengths] + 2 * i);
                    s.Serialize<ushort>(euRom.PortraitTileSetLengths[i]);
                }
            }

            // Replace cutscenes
            if (replaceCutscenes)
            {
                for (int i = 0; i < euRom.CutsceneMaps.Length; i++)
                {
                    GBAIsometric_IceDragon_CutsceneMap euCutscene = euRom.CutsceneMaps[i];
                    GBAIsometric_IceDragon_CutsceneMap jpCutscene = jpRom.CutsceneMaps[i];

                    // Replace tile sets
                    for (int j = 0; j < 4; j++)
                    {
                        Pointer euPointer = ((GBAIsometric_IceDragon_DataPointer)euCutscene.TileSetIndices[j]).DataPointer;

                        // Get the length of the encoded data
                        long encodedLength = d.DoAt(euPointer, () =>
                        {
                            d.DoEncoded(new GBA_LZSSEncoder(), () => { });
                            d.Align();
                            return d.CurrentPointer - euPointer;
                        });

                        // Read the encoded bytes
                        byte[] encodedBytes = d.DoAt(euPointer, () => d.SerializeArray<byte>(default, encodedLength));

                        // Update pointer
                        s.Goto(jpCutscene.TileSetIndices[j].Offset);
                        s.SerializePointer(remapOffset);

                        s.Goto(remapOffset);
                        s.SerializeArray<byte>(encodedBytes, encodedBytes.Length);

                        Debug.Log($"Remapped cutscene tile set {i}-{j} to 0x{remapOffset.StringAbsoluteOffset}");

                        s.Align();
                        remapOffset = s.CurrentPointer;
                    }

                    // Replace map
                    Pointer euMapPointer = ((GBAIsometric_IceDragon_DataPointer)euCutscene.MapIndex).DataPointer;

                    // Get the length of the encoded data
                    long encodedMapLength = d.DoAt(euMapPointer, () =>
                    {
                        d.DoEncoded(new GBA_LZSSEncoder(), () => { });
                        d.Align();
                        return d.CurrentPointer - euMapPointer;
                    });

                    // Read the encoded bytes
                    byte[] encodedMapBytes = d.DoAt(euMapPointer, () => d.SerializeArray<byte>(default, encodedMapLength));

                    // Update pointer
                    s.Goto(jpCutscene.MapIndex.Offset);
                    s.SerializePointer(remapOffset);

                    s.Goto(remapOffset);
                    s.SerializeArray<byte>(encodedMapBytes, encodedMapBytes.Length);

                    Debug.Log($"Remapped cutscene map {i} to 0x{remapOffset.StringAbsoluteOffset}");

                    s.Align();
                    remapOffset = s.CurrentPointer;

                    // Replace palette
                    s.Goto(jpCutscene.Palette.First().Offset);
                    s.SerializeObjectArray<RGBA5551Color>(euCutscene.Palette, euCutscene.Palette.Length);
                }
            }

            Debug.Log($"Remap end is 0x{remapOffset.StringAbsoluteOffset}");
        }

        public void ImportLevelMaps(string baseDir, string romName, uint startOffset, string mapsDir, string compressFilePath)
        {
            for (int i = 0; i < 15; i++)
            {
                string mapFilePath = Path.Combine(mapsDir, $"{i}.gba");

                // Trim away palette
                using (var f = File.OpenWrite(mapFilePath))
                    f.SetLength(19200);

                // Compress
                ProcessHelpers.RunProcess(compressFilePath, new string[]
                {
                    "-evf", // VRAM fast
                    $"\"{mapFilePath}\""
                }, waitForExit: true);
            }

            using var context = new Ray1MapContext(baseDir, new GameSettings(GameModeSelection.SpyroSeasonIceJP, baseDir, 0, 0));

            context.AddFile(new GBAMemoryMappedFile(context, romName, GBAConstants.Address_ROM)
            {
                RecreateOnWrite = false
            });

            GBAIsometric_Ice_ROM rom = FileFactory.Read<GBAIsometric_Ice_ROM>(context, romName, (s, x) => x.Pre_SerializeLevel3D = true);

            var s = context.Serializer;

            var pointerOffsets = new uint[]
            {
                0x08011f7c, 0x08011f84, 0x08011f8c, 0x08011f94, 0x08011f9c,
                0x08011fa4, 0x08011fac, 0x08011fb4, 0x08011fbc, 0x08011fc4,
                0x08011fcc, 0x08011fd4, 0x08011fdc, 0x08011fe4, 0x08012088,
            };

            s.Goto(new Pointer(startOffset, context.GetFile(romName)));

            for (int i = 0; i < 15; i++)
            {
                byte[] bytes = File.ReadAllBytes(Path.Combine(mapsDir, $"{i}.gba"));

                Pointer remapPointer = s.CurrentPointer;

                // Update the pointer
                s.DoAt(new Pointer(pointerOffsets[i], context.GetFile(romName)), () => s.SerializePointer(remapPointer));

                // Write the palette
                s.SerializeObject<Palette>(rom.Level3D_LevelMaps[i].Palette);

                // Write the compressed level map
                s.SerializeArray<byte>(bytes, bytes.Length);

                s.Align();
            }

            Debug.Log($"Remap end is 0x{s.CurrentPointer.StringAbsoluteOffset}");
        }

        public void CopySpriteSetOffsets(Context context)
        {
            var str = new StringBuilder();

            BinaryDeserializer s = context.Deserializer;

            s.Goto(context.FilePointer(GetROMFilePath) + 3);

            int index = 0;
            while (s.CurrentFileOffset < s.CurrentLength - 4)
            {
                string magic = s.SerializeString(default, 3);
                s.Goto(s.CurrentPointer + 1);

                if (magic != "CRS")
                    continue;

                str.AppendLine($"(0x{(s.CurrentPointer - 7).StringAbsoluteOffset}, 0x00), // {index}");

                index++;
            }

            str.ToString().CopyToClipboard();
        }

        public async UniTask CopySpriteSetOffsetsFromEUBaseAsync(Context context)
        {
            using var euContext = new Ray1MapContext(new GameSettings(GameModeSelection.SpyroSeasonIceEU, Settings.GameDirectories[GameModeSelection.SpyroSeasonIceEU], 0, 0));
            await LoadFilesAsync(euContext);
            GBAIsometric_Ice_ROM euRom = FileFactory.Read<GBAIsometric_Ice_ROM>(euContext, GetROMFilePath, (s, r) =>
            {
                r.Pre_SerializeSprites = true;
            });

            var spriteSetPointers = Enumerable.Repeat((Pointer)null, euRom.SpriteSets.Length).ToList();

            BinaryDeserializer s = context.Deserializer;

            s.Goto(context.FilePointer(GetROMFilePath) + 3);

            while (s.CurrentFileOffset < s.CurrentLength - 4)
            {
                string magic = s.SerializeString(default, 3);
                s.Goto(s.CurrentPointer + 1);

                if (magic != "CRS")
                    continue;

                s.Goto(s.CurrentPointer - 7);

                var set = s.SerializeObject<GBAIsometric_Ice_SpriteSet>(default);

                var index = euRom.SpriteSets.FindItemIndex(x => x.Value.ImgData.SequenceEqual(set.ImgData));

                if (index != -1 && spriteSetPointers[index] == null)
                    spriteSetPointers[index] = set.Offset;
                else
                    spriteSetPointers.Add(set.Offset);

                s.Align();
                s.Goto(s.CurrentPointer - 1);
            }

            var str = new StringBuilder();

            for (int i = 0; i < spriteSetPointers.Count; i++)
                str.AppendLine($"(0x{spriteSetPointers[i]?.StringAbsoluteOffset ?? "00"}, 0x00), // {i}");

            str.ToString().CopyToClipboard();
        }

        public async UniTask CopySpriteSetPaletteOffsetsFromEUBaseAsync(Context context)
        {
            using var euContext = new Ray1MapContext(new GameSettings(GameModeSelection.SpyroSeasonIceEU, Settings.GameDirectories[GameModeSelection.SpyroSeasonIceEU], 0, 0));
            await LoadFilesAsync(euContext);
            GBAIsometric_Ice_ROM euRom = FileFactory.Read<GBAIsometric_Ice_ROM>(euContext, GetROMFilePath, (s, r) =>
            {
                r.Pre_SerializeSprites = true;
            });

            BinaryDeserializer s = context.Deserializer;
            BinaryDeserializer s_eu = euContext.Deserializer;

            var palOffsets = new Pointer[euRom.SpriteSetPalettes.Length];

            for (int i = 0; i < euRom.SpriteSetPalettes.Length; i++)
            {
                var pal = euRom.SpriteSetPalettes[i];

                if (pal != null)
                {
                    var rawPal = s_eu.DoAt(pal.PointerValue, () => s_eu.SerializeArray<byte>(default, 32));

                    s.Goto(context.FilePointer(GetROMFilePath));

                    while (s.CurrentFileOffset < s.CurrentLength - 32)
                    {
                        var raw = s.DoAt(s.CurrentPointer, () => s.SerializeArray<byte>(default, 32));

                        if (raw.SequenceEqual(rawPal))
                        {
                            palOffsets[i] = s.CurrentPointer;
                            break;
                        }

                        s.Goto(s.CurrentPointer + 4);
                    }
                }
            }

            var str = new StringBuilder();

            foreach (Pointer p in palOffsets)
                str.AppendLine($"0x{p?.StringAbsoluteOffset ?? "00"}");

            str.ToString().CopyToClipboard();
        }
    }
}