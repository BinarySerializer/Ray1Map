using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.GBA;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro1_Manager : GBAIsometric_IceDragon_BaseManager
    {
        private const int World_Levels3D = 0;
        private const int World_Cutscenes = 1;

        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(World_Levels3D, Enumerable.Range(0, 17).ToArray()), // Levels 3D
            // TODO: Mode7 levels
            // TODO: Sparx levels
            new GameInfo_World(World_Cutscenes, Enumerable.Range(0, 21).ToArray()), // Cutscenes
        });

        public override GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Resources", false, true, (input, output) => ExportResourcesAsync(settings, output, false)),
            new GameAction("Export Resources (categorized)", false, true, (input, output) => ExportResourcesAsync(settings, output, true)),
            new GameAction("Export Cutscene Maps", false, true, (input, output) => ExportCutsceneMapsAsync(settings, output)),
            new GameAction("Export Sprites (full, no pal)", false, true, (input, output) => ExportAllSpritesAsync(settings, output)),
            new GameAction("Export Portraits", false, true, (input, output) => ExportPortraitsAsync(settings, output)),
            new GameAction("Export Scripts", false, true, (input, output) => ExportScriptsAsync(settings, output)),
            new GameAction("Export Font", false, true, (input, output) => ExportFont(settings, output)),
            new GameAction("Export Strings", false, true, (input, output) => ExportStrings(settings, output)),
            new GameAction("Export Localization", false, true, (input, output) => ExportLocalization(settings, output)),
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

        public async UniTask ExportAllSpritesAsync(GameSettings settings, string outputPath)
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

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            Controller.DetailedState = $"Loading data";
            await Controller.WaitIfNecessary();

            GameSettings settings = context.GetR1Settings();
            int world = settings.World;
            int level = settings.Level;

            if (world == World_Levels3D)
            {
                // Read the ROM
                GBAIsometric_Ice_ROM rom = FileFactory.Read<GBAIsometric_Ice_ROM>(context, GetROMFilePath, (s, r) =>
                {
                    r.Pre_SerializeLevel3D = true;
                    r.Pre_Level3D = level;
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
                        // TODO: Include ones with the additional data. Seems to mostly define pits?
                        Where(x => x.AdditionalDataLength == 0).Select(x =>
                    {
                        float factor = lev.PixelsPerUnit;
                        float minX = x.MinX / factor;
                        float minY = x.MinY / factor;
                        float maxX = x.MaxX / factor;
                        float maxY = x.MaxY / factor;
                        float height = ((float)x.Height / (1 << 14)) / factor;

                        float w = maxX - minX;
                        float h = maxY - minY;

                        return new Unity_IsometricCollisionObject
                        {
                            Position = new Vector2(minX + w / 2f, minY + h / 2f),
                            Dimensions = new Vector2(w, h),
                            Height = height,
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

                // TODO: Implement

                lev.ObjManager = new Unity_ObjectManager(context);

                Controller.DetailedState = $"Loading localization";
                await Controller.WaitIfNecessary();

                lev.Localization = LoadLocalization(context, rom.Localization);

                return lev;
            }
            else if (world == World_Cutscenes)
            {
                GBAIsometric_Ice_ROM rom = FileFactory.Read<GBAIsometric_Ice_ROM>(context, GetROMFilePath);
                return await LoadCutsceneMapAsync(context, rom);
            }
            else
            {
                throw new NotImplementedException();
            }
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

            Texture2D tex = TextureHelpers.CreateTexture2D(size.Width, size.Height, clear: true);

            int imgDataOffset = sprite.TileIndex * (spriteSet.Is8Bit ? 0x40 : 0x20);
            int tileIndex = spriteSet.SpriteMapLength * spriteIndex;

            for (int y = 0; y < size.Height / GBAConstants.TileSize; y++)
            {
                for (int x = 0; x < size.Width / GBAConstants.TileSize; x++)
                {
                    if (!spriteSet.SpriteMaps[tileIndex])
                    {
                        tileIndex++;
                        continue;
                    }

                    tex.FillInTile(
                        imgData: spriteSet.ImgData,
                        imgDataOffset: imgDataOffset,
                        pal: pal,
                        encoding: Util.TileEncoding.Linear_4bpp,
                        tileWidth: GBAConstants.TileSize,
                        flipTextureY: true,
                        tileX: x * GBAConstants.TileSize,
                        tileY: y * GBAConstants.TileSize);

                    imgDataOffset += spriteSet.Is8Bit ? 0x40 : 0x20;

                    tileIndex++;
                }
            }

            tex.Apply();

            return tex;
        }

        public void PatchJPROM(
            string baseDir, string jpName, string euName, uint startOffset, 
            bool replacePortraits, bool replaceCutscenes)
        {
            // Changes made:

            // Text:
            // 0x08279c50 - Replaced font tiles (<)
            // 0x0827c2f0 - Replaced font map (<)
            // 0x8275C00 -> 0x08835D28 - Replaced text (remapped, pointer at 0x08002c60)

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
    }
}