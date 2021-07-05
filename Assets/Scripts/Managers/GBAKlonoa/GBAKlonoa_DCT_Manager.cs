using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    public class GBAKlonoa_DCT_Manager : GBAKlonoa_BaseManager
    {
        public const int NormalWorldsCount = 6; // World 0 is reserved for either menus or cutscenes, so normal worlds are 1-6
        public const int WorldLevelsCount = 11;
        public const int LevelsCount = NormalWorldsCount * WorldLevelsCount;
        public const int NormalLevelsCount = NormalWorldsCount * 10;

        public static int GetGlobalLevelIndex(int world, int level) => (world - 1) * WorldLevelsCount + level;
        public static int GetNormalLevelIndex(int world, int level) => (world - 1) * 10 + (level - 1);

        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(Enumerable.Range(1, 6).Select(w => new GameInfo_World(w, Enumerable.Range(0, 11).ToArray())).ToArray());

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            var rom = FileFactory.Read<GBAKlonoa_DCT_ROM>(GetROMFilePath, context);
            var settings = context.GetR1Settings();
            var globalLevelIndex = GetGlobalLevelIndex(settings.World, settings.Level);
            var normalLevelIndex = GetNormalLevelIndex(settings.World, settings.Level);

            Controller.DetailedState = "Loading maps";
            await Controller.WaitIfNecessary();

            var tilePal = rom.Maps[globalLevelIndex].Palette.Take(0x1E0).Concat(rom.FixTilePalette).ToArray();

            var maps = rom.Maps[globalLevelIndex].MapLayers.Select(layer =>
            {
                if (layer == null)
                    return null;

                var is8Bit = layer.Is8Bit;
                var imgData = layer.TileSet;

                return new Unity_Map
                {
                    Width = layer.Width,
                    Height = layer.Height,
                    TileSet = new Unity_TileSet[]
                    {
                        LoadTileSet(imgData, tilePal, is8Bit, layer.Map)
                    },
                    MapTiles = layer.Map.Select(x => new Unity_Tile(x)).ToArray(),
                    Type = Unity_Map.MapType.Graphics,
                };
            }).ToArray();

            // Reorder maps (BG3 should be behind BG2)
            var bg2 = maps[2];
            var bg3 = maps[3];
            maps[2] = bg3;
            maps[3] = bg2;

            // Remove null maps
            maps = maps.Where(x => x != null).ToArray();

            // Add line collision for the sector bounds
            var collisionLines = new List<Unity_CollisionLine>();

            if (settings.Level != 0 && settings.Level != 4)
            {
                var sectorIndex = 0;

                foreach (var sector in rom.MapSectors[normalLevelIndex].Sectors)
                {
                    var color = new Color(1, 0.92f - (0.1f * sectorIndex), 0.016f + (0.15f * sectorIndex));

                    collisionLines.Add(new Unity_CollisionLine(
                        new Vector2(sector.X, sector.Y),
                        new Vector2(sector.X, sector.Y + sector.Height),
                        color));

                    collisionLines.Add(new Unity_CollisionLine(
                        new Vector2(sector.X, sector.Y + sector.Height),
                        new Vector2(sector.X + sector.Width, sector.Y + sector.Height),
                        color));

                    collisionLines.Add(new Unity_CollisionLine(
                        new Vector2(sector.X + sector.Width, sector.Y),
                        new Vector2(sector.X + sector.Width, sector.Y + sector.Height),
                        color));

                    collisionLines.Add(new Unity_CollisionLine(
                        new Vector2(sector.X, sector.Y),
                        new Vector2(sector.X + sector.Width, sector.Y),
                        color));

                    sectorIndex++;
                }
            }

            return new Unity_Level(
                maps: maps,
                objManager: new Unity_ObjectManager(context),
                eventData: new List<Unity_Object>(),
                cellSize: CellSize,
                defaultLayer: 2,
                collisionLines: collisionLines.ToArray());
        }
    }
}