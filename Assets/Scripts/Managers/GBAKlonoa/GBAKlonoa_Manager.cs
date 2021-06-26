using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.GBA;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    public class GBAKlonoa_Manager : BaseGameManager
    {
        public const int CellSize = GBA_ROMBase.TileSize;
        public const string GetROMFilePath = "ROM.gba";

        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(Enumerable.Range(1, 6).Select(w => new GameInfo_World(w, Enumerable.Range(0, 9).ToArray())).ToArray());

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            var rom = FileFactory.Read<GBAKlonoa_ROM>(GetROMFilePath, context);
            var settings = context.GetR1Settings();
            var globalLevel = (settings.World - 1) * 9 + settings.Level;

            Controller.DetailedState = "Loading maps";
            await Controller.WaitIfNecessary();

            var pal_8 = Util.ConvertGBAPalette(rom.MapPalettes[globalLevel]);
            var pal_4 = Util.ConvertAndSplitGBAPalette(rom.MapPalettes[globalLevel]);

            var maps = Enumerable.Range(0, 3).Select(mapIndex =>
            {
                var width = rom.MapWidths[globalLevel].Widths[mapIndex];
                var is8Bit = mapIndex == 2;
                var map = rom.Maps[globalLevel].Maps[mapIndex];

                Func<int, Color[]> getPalFunc = null;

                if (!is8Bit)
                {
                    Dictionary<int, int> tilesetPalettes = new Dictionary<int, int>();

                    foreach (var m in map)
                    {
                        if (!tilesetPalettes.ContainsKey(m.TileMapY))
                            tilesetPalettes[m.TileMapY] = m.PaletteIndex;
                        else if (tilesetPalettes[m.TileMapY] != m.PaletteIndex && m.TileMapY != 0)
                            Debug.LogWarning($"Tile {m.TileMapY} has several possible palettes: {tilesetPalettes[m.TileMapY]} - {m.PaletteIndex}");
                    }

                    getPalFunc = x => pal_4[tilesetPalettes.TryGetItem(x)];
                }

                return new Unity_Map
                {
                    Width = width,
                    Height = (ushort)(map.Length / width),
                    TileSet = new Unity_TileSet[]
                    {
                        new Unity_TileSet(Util.ToTileSetTexture(
                            imgData: rom.TileSets[globalLevel].TileSets[mapIndex],
                            pal: pal_8,
                            encoding: is8Bit ? Util.TileEncoding.Linear_8bpp : Util.TileEncoding.Linear_4bpp,
                            tileWidth: CellSize, 
                            flipY: false, 
                            getPalFunc: getPalFunc), CellSize)
                    },
                    MapTiles = map.Select(x => new Unity_Tile(x)).ToArray(),
                    Type = Unity_Map.MapType.Graphics,
                };
            }).ToArray();

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            var objmanager = new Unity_ObjectManager_GBAKlonoa(
                context: context,
                animSets: new Unity_ObjectManager_GBAKlonoa.AnimSet[0]);

            var objects = Enumerable.Range(0, 5).Select(sector => rom.Objects.Objects.Select(x => new Unity_Object_GBAKlonoa(objmanager, x, sector))).SelectMany(x => x);

            return new Unity_Level(
                maps: maps,
                objManager: objmanager,
                eventData: new List<Unity_Object>(objects),
                cellSize: CellSize,
                defaultLayer: 2);
        }

        public override async UniTask LoadFilesAsync(Context context) => await context.AddMemoryMappedFile(GetROMFilePath, GBA_ROMBase.Address_ROM);
    }
}