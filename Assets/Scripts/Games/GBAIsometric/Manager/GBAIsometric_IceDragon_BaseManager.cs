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
    public abstract class GBAIsometric_IceDragon_BaseManager : BaseGameManager
    {
        #region Constant Fields

        public const int CellSize = GBAConstants.TileSize;
        public const string GetROMFilePath = "ROM.gba";

        #endregion

        #region Public Methods

        public UniTask DoGameActionAsync<ROM>(GameSettings settings, Action<ROM, Context> action) 
            where ROM : BinarySerializable, new() => DoGameActionAsync(settings, null, action);

        public async UniTask DoGameActionAsync<ROM>(GameSettings settings, Action<ROM> onPreSerialize, Action<ROM, Context> action)
            where ROM : BinarySerializable, new()
        {
            using var context = new Ray1MapContext(settings);

            await LoadFilesAsync(context);

            ROM rom = FileFactory.Read<ROM>(context, GetROMFilePath, (_, r) => onPreSerialize?.Invoke(r));

            action(rom, context);
        }

        public void ExportResources(Context context, GBAIsometric_IceDragon_Resources resources, string outputPath, bool categorize, bool ignoreUsedBlocks)
        {
            BinaryDeserializer s = context.Deserializer;
            Color[] palette = PaletteHelpers.CreateDummyPalette(16).Select(x => x.GetColor()).ToArray();

            for (ushort i = 0; i < resources.DataEntries.Length; i++)
            {
                if (ignoreUsedBlocks && GBAIsometric_IceDragon_ResourceRef.UsedIndices.Contains(i))
                    continue;

                var length = resources.DataEntries[i].DataLength;

                if (categorize && length == 512)
                {
                    RGBA5551Color[] pal = null;
                    resources.DoAtResource(context, i, size => pal = s.SerializeObjectArray<RGBA5551Color>(default, 256, name: $"Pal[{i}]"));

                    PaletteHelpers.ExportPalette(Path.Combine(outputPath, "Palettes", $"{i:000}_0x{resources.DataEntries[i].DataPointer.StringAbsoluteOffset}.png"), pal, optionalWrap: 16);
                }
                else
                {
                    byte[] data = null;
                    resources.DoAtResource(context, i, size => data = s.SerializeArray<byte>(default, size, name: $"Block[{i}]"));

                    if (categorize && length % 32 == 0)
                    {
                        var tex = Util.ToTileSetTexture(data, palette, Util.TileEncoding.Linear_4bpp, CellSize, true, wrap: 32);
                        Util.ByteArrayToFile(Path.Combine(outputPath, "ObjTileSets", $"{i:000}_0x{resources.DataEntries[i].DataPointer.StringAbsoluteOffset}.png"), tex.EncodeToPNG());
                    }
                    else
                    {
                        Util.ByteArrayToFile(Path.Combine(outputPath, $"{i:000}_0x{resources.DataEntries[i].DataPointer.StringAbsoluteOffset}.dat"), data);
                    }
                }
            }
        }

        public void ExportScripts(Context context, GBAIsometric_Spyro_Dialog[] dialogEntries, GBAIsometric_IceDragon_MenuPage[] menuPages, string outputPath)
        {
            var langIndex = 0;

            foreach (var lang in context.GetSettings<GBAIsometricSettings>().Languages)
            {
                if (dialogEntries != null)
                {
                    using var w = new StreamWriter(Path.Combine(outputPath, $"Cutscenes_{lang}.txt"));

                    foreach (var d in dialogEntries.OrderBy(x => x.ID))
                    {
                        var data = d.DialogData;

                        w.WriteLine($"# Cutscene {d.ID} (0x{d.Offset.StringAbsoluteOffset})");

                        foreach (var e in data.Entries)
                        {
                            switch (e.Values.First().Instruction)
                            {
                                case GBAIsometric_Spyro_DialogData.Instruction.DrawPortrait:
                                    w.WriteLine($"[Draw portrait {e.PortraitIndex}]");
                                    break;

                                case GBAIsometric_Spyro_DialogData.Instruction.DrawText:
                                case GBAIsometric_Spyro_DialogData.Instruction.DrawMultiChoiceText:
                                    w.WriteLine($"{String.Join(" ", e.LocIndices.Select(x => x.GetString(langIndex)))}");
                                    break;

                                case GBAIsometric_Spyro_DialogData.Instruction.MoveCamera:
                                    w.WriteLine("[Move camera]");
                                    break;
                            }

                            if (e.Values.First().Instruction == GBAIsometric_Spyro_DialogData.Instruction.DrawMultiChoiceText)
                            {
                                w.WriteLine($"  > {e.MultiChoiceLocIndices[0].GetString(langIndex)} > {e.MultiChoiceLocIndices[2].GetString(langIndex)}");
                                w.WriteLine($"  > {e.MultiChoiceLocIndices[1].GetString(langIndex)} > {e.MultiChoiceLocIndices[3].GetString(langIndex)}");
                            }
                        }

                        w.WriteLine();
                    }
                }

                if (menuPages != null)
                {
                    using var w = new StreamWriter(Path.Combine(outputPath, $"Menus_{lang}.txt"));

                    var menuIndex = 0;

                    foreach (var d in menuPages)
                    {
                        w.WriteLine($"# Menu {menuIndex} (0x{d.Offset.StringAbsoluteOffset})");

                        var header = d.Header.GetString(langIndex);
                        var subHeader = d.SubHeader.GetString(langIndex);

                        if (header != null)
                            w.WriteLine($"{header}");
                        if (subHeader != null)
                            w.WriteLine($"{subHeader}");

                        foreach (var o in d.Options ?? Array.Empty<GBAIsometric_IceDragon_MenuOption>())
                            w.WriteLine($"> {o.LocIndex.GetString(langIndex)}");

                        w.WriteLine();

                        menuIndex++;
                    }
                }

                langIndex++;
            }
        }

        public void ExportFont(GBAIsometric_IceDragon_Localization loc, string outputPath)
        {
            if (loc?.FontTileMap == null)
                throw new Exception($"Font data has not been parsed!");

            GBAIsometric_IceDragon_SpriteMap map = loc.FontTileMap;

            const int cellSize = GBAConstants.TileSize;
            const int tileSize = 0x20;

            var tex = TextureHelpers.CreateTexture2D(map.Width * cellSize, map.Height * cellSize);
            var pal = Util.ConvertGBAPalette(PaletteHelpers.CreateDummyPalette(256, wrap: 16));

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    var tile = map.MapData[y * map.Width + x];
                    tex.FillInTile(loc.FontTileSet, tile.TileMapY * tileSize, pal, Util.TileEncoding.Linear_4bpp, cellSize, true, x * cellSize, y * cellSize, tile.HorizontalFlip, tile.VerticalFlip);
                }
            }

            tex.Apply();

            Util.ByteArrayToFile(Path.Combine(outputPath, "Font.png"), tex.EncodeToPNG());
        }

        public void ExportStrings(Context context, GBAIsometric_IceDragon_Localization loc, string outputPath)
        {
            if (loc?.FontTileMap == null)
                throw new Exception($"Font data has not been parsed!");

            GBAIsometric_IceDragon_SpriteMap map = loc.FontTileMap;

            const int cellSize = GBAConstants.TileSize;
            const int tileSize = 0x20;

            Color[] pal = Util.ConvertGBAPalette(PaletteHelpers.CreateDummyPalette(256, wrap: 16));
            string[] langs = context.GetSettings<GBAIsometricSettings>().Languages;

            for (int lang = 0; lang < loc.LocBlocks.Length; lang++)
            {
                for (int i = 0; i < loc.LocBlocks[lang].StringTileIndices.Length; i++)
                {
                    byte[] indices = loc.LocBlocks[lang].StringTileIndices[i];

                    var tex = TextureHelpers.CreateTexture2D(2 * cellSize * indices.Length, 2 * cellSize, clear: true);

                    for (int c = 0; c < indices.Length; c++)
                    {
                        byte charIndex = indices[c];

                        if (charIndex == 0xFF)
                            continue;

                        for (int y = 0; y < 2; y++)
                        {
                            for (int x = 0; x < 2; x++)
                            {
                                MapTile tile = map.MapData[(charIndex * 4) + (y * 2 + x)];
                                tex.FillInTile(
                                    imgData: loc.FontTileSet, 
                                    imgDataOffset: tile.TileMapY * tileSize, 
                                    pal: pal, 
                                    encoding: Util.TileEncoding.Linear_4bpp, 
                                    tileWidth: cellSize, 
                                    flipTextureY: true, 
                                    tileX: c * 2 * cellSize + x * cellSize, 
                                    tileY: y * cellSize, 
                                    flipTileX: tile.HorizontalFlip, 
                                    flipTileY: tile.VerticalFlip);
                            }
                        }
                    }

                    tex.Apply();

                    Util.ByteArrayToFile(Path.Combine(outputPath, langs[lang], $"{i}.png"), tex.EncodeToPNG());
                }
            }
        }

        public Unity_TileSet LoadTileSet(RGB555Color[] tilePal, byte[] tileSet) => 
            new Unity_TileSet(tileSet, Unity_Palette.SplitMultiple(tilePal, 16, true), Unity_TextureFormat.Indexed_4, GBAConstants.TileSize);

        public async UniTask<Unity_Level> LoadCutsceneMapAsync(Context context, GBAIsometric_IceDragon_BaseROM rom)
        {
            GBAIsometric_IceDragon_CutsceneMap cutsceneMap = rom.CutsceneMaps[context.GetR1Settings().Level];

            Controller.DetailedState = $"Loading tileset";
            await Controller.WaitIfNecessary();

            byte[] fullTileSet = cutsceneMap.TileSets.SelectMany(x => x).ToArray();
            var cutsceneTileSet = new Unity_TileSet(fullTileSet, new Unity_Palette(cutsceneMap.Palette, true), Unity_TextureFormat.Indexed_8, GBAConstants.TileSize);

            Controller.DetailedState = $"Loading map";
            await Controller.WaitIfNecessary();

            Unity_Map map = new Unity_Map()
            {
                Type = Unity_Map.MapType.Graphics,
                Width = cutsceneMap.Map.Width,
                Height = cutsceneMap.Map.Height,
                TileSet = new Unity_TileSet[]
                {
                    cutsceneTileSet
                },
                MapTiles = cutsceneMap.Map.MapData.Select(x => new Unity_Tile(x)).ToArray(),
            };

            Controller.DetailedState = $"Loading localization";
            await Controller.WaitIfNecessary();

            return new Unity_Level()
            {
                Maps = new Unity_Map[]
                {
                    map
                },
                ObjManager = new Unity_ObjectManager(context),
                CellSize = CellSize,
                Localization = LoadLocalization(context, rom.Localization),
            };
        }

        public KeyValuePair<string, string[]>[] LoadLocalization(Context context, GBAIsometric_IceDragon_Localization loc)
        {
            string[] langages = context.GetSettings<GBAIsometricSettings>().Languages;

            return loc?.LocBlocks?.Select((x, i) => new KeyValuePair<string, string[]>(langages[i], x.Strings)).ToArray();
        }

        public override void AddContextSettings(Context context)
        {
            // Add settings
            context.AddSettings(context.GetR1Settings().GetGBAIsometricSettings());
        }

        public override async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, GBAConstants.Address_ROM);

        #endregion
    }
}