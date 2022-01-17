﻿using System;
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
            new GameAction("Export Font", false, true, (input, output) => ExportFont(settings, output)),
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
                        GBAIsometric_Ice_AnimSet animSet = s.SerializeObject<GBAIsometric_Ice_AnimSet>(default);

                        for (int i = 0; i < animSet.Sprites.Length; i++)
                        {
                            Texture2D tex = GetSpriteTexture(animSet, i, pal4);
                            Util.ByteArrayToFile(Path.Combine(outputPath, $"0x{animSet.Offset.StringAbsoluteOffset}", $"{i}.png"), tex.EncodeToPNG());
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

        public async UniTask ExportFont(GameSettings settings, string outputPath)
        {
            await DoGameActionAsync<GBAIsometric_Ice_ROM>(settings, (rom, _) =>
            {
                ExportFont(rom.Localization, outputPath);
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

                // TODO: Implement

                Controller.DetailedState = $"Loading maps";
                await Controller.WaitIfNecessary();

                lev.Maps = mapLayers.Layers.Select((map, i) =>
                {
                    return new Unity_Map()
                    {
                        Type = Unity_Map.MapType.Graphics,
                        Width = map.Value.Width,
                        Height = map.Value.Height,
                        TileSet = new Unity_TileSet[] { tileSet },
                        MapTiles = mapTiles[i].Select(x => new Unity_Tile(x)).ToArray(),
                    };
                }).Reverse().ToArray();
                lev.CellSize = GBAConstants.TileSize;

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

        public Texture2D GetSpriteTexture(GBAIsometric_Ice_AnimSet animSet, int spriteIndex, Color[] pal)
        {
            // 8-bit sprites are never used
            if (animSet.Is8Bit)
                throw new InvalidOperationException("8-bit sprites are currently not supported");

            GBAIsometric_Ice_Sprite sprite = animSet.Sprites[spriteIndex];
            int shape = 0;

            if (sprite.Height < sprite.Width)
                shape = 1;
            else if (sprite.Width < sprite.Height)
                shape = 2;

            GBAConstants.Size size = GBAConstants.GetSpriteShape(shape, sprite.SpriteSize);

            Texture2D tex = TextureHelpers.CreateTexture2D(size.Width, size.Height, clear: true);

            int imgDataOffset = sprite.TileIndex * (animSet.Is8Bit ? 0x40 : 0x20);
            int tileIndex = animSet.SpriteMapLength * spriteIndex;

            for (int y = 0; y < size.Height / GBAConstants.TileSize; y++)
            {
                for (int x = 0; x < size.Width / GBAConstants.TileSize; x++)
                {
                    if (!animSet.SpriteMaps[tileIndex])
                    {
                        tileIndex++;
                        continue;
                    }

                    tex.FillInTile(
                        imgData: animSet.ImgData,
                        imgDataOffset: imgDataOffset,
                        pal: pal,
                        encoding: Util.TileEncoding.Linear_4bpp,
                        tileWidth: GBAConstants.TileSize,
                        flipTextureY: true,
                        tileX: x * GBAConstants.TileSize,
                        tileY: y * GBAConstants.TileSize);

                    imgDataOffset += animSet.Is8Bit ? 0x40 : 0x20;

                    tileIndex++;
                }
            }

            tex.Apply();

            return tex;
        }

        public void PatchJPROM(string baseDir, string jpName, string euName, uint startOffset)
        {
            // Changes made:

            // Text:
            // 0x08279c50 - Replaced font tiles (<)
            // 0x0827c2f0 - Replaced font map (<)
            // 0x8275C00 -> 0x087F14A0 - Text (remapped)

            // Portraits (excluding 6, 8, 23):
            // 0x081b0b58 - Replaced palettes (16)
            // 0x081b0bb8 - Replaced maps (4x4)
            // 0x08063614 - Replaced tile set lengths (24)
            // 0x081b0af8 - Replaced tile sets (some have been remapped)

            // TODO:
            // Replace cutscenes
            // Replace logo

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

            var s = jpContext.Serializer;

            Pointer remapOffset = new Pointer(startOffset, jpContext.GetFile(jpName));
            Dictionary<Spyro_DefinedPointer, Pointer> jpPointers = PointerTables.GBAIsometric_Spyro_PointerTable(
                jpContext.GetR1Settings().GameModeSelection, jpContext.GetFile(jpName));

            // Replace portraits
            for (int i = 0; i < 24; i++)
            {
                if (i == 6 || i == 8 || i == 23)
                    continue;

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

            Debug.Log($"Remap end is 0x{remapOffset.StringAbsoluteOffset}");
        }
    }
}