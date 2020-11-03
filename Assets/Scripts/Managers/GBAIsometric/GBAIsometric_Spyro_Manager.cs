using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public abstract class GBAIsometric_Spyro_Manager : IGameManager
    {
        public const int CellSize = 8;

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(LevelInfos.Select((x, i) => new GameInfo_World(i, x.LevelIDs)).ToArray());

        public virtual string GetROMFilePath => $"ROM.gba";

        public abstract int DataTableCount { get; }
        public abstract LevelInfo[] LevelInfos { get; }

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Data Blocks", false, true, (input, output) => ExportDataBlocks(settings, output, false)),
            new GameAction("Export Data Blocks (categorized)", false, true, (input, output) => ExportDataBlocks(settings, output, true)),
        };

        public async UniTask ExportDataBlocks(GameSettings settings, string outputPath, bool categorize) {
            using (var context = new Context(settings)) {
                var s = context.Deserializer;
                await LoadFilesAsync(context);

                var rom = FileFactory.Read<GBAIsometric_Spyro_ROM>(GetROMFilePath, context);

                var palettes = categorize ? Enumerable.Range(0, 16).Select(x => rom.LevelData[context.Settings.World].First(lev => lev.ID == context.Settings.Level).ObjPalette.Skip(16 * x).Take(16).Select((c, i) =>
                {
                    if (i != 0)
                        c.Alpha = 255;
                    return c.GetColor();
                }).ToArray()).ToArray() : null;

                for (int i = 0; i < rom.DataTable.DataEntries.Length; i++)
                {
                    var data = rom.DataTable.DoAtBlock(context, i, size => s.SerializeArray<byte>(default, size, name: $"Block[{i}]"));

                    if (categorize && data.Length % 32 == 0)
                    {
                        for (int j = 0; j < 16; j++)
                        {
                            var tex = Util.ToTileSetTexture(data, palettes[j], false, CellSize, true, wrap: 32);
                            Util.ByteArrayToFile(Path.Combine(outputPath, "ObjTileSet", $"{i:000}_Pal{j}_0x{rom.DataTable.DataEntries[i].DataPointer.AbsoluteOffset:X8}.png"), tex.EncodeToPNG());
                        }
                    }
                    else
                    {
                        Util.ByteArrayToFile(Path.Combine(outputPath, $"{i:000}_0x{rom.DataTable.DataEntries[i].DataPointer.AbsoluteOffset:X8}.dat"), data);
                    }
                }
            }
        }

        public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            var rom = FileFactory.Read<GBAIsometric_Spyro_ROM>(GetROMFilePath, context);

            var is2D = LevelInfos[context.Settings.World].Is2D;
            var levelInfo = rom.LevelData[context.Settings.World].First(x => x.ID == context.Settings.Level);

            // Convert map arrays to map tiles
            Dictionary<GBAIsometric_Spyro_MapLayer, MapTile[]> mapTiles = levelInfo.MapLayers.Where(x => x != null).ToDictionary(x => x, GetMapTiles);

            // Load tileset
            var tileSet = LoadTileSet(levelInfo.TilePalette, levelInfo.MapLayers.Select(x => x.TileSet).ToArray(), mapTiles);

            var maps = levelInfo.MapLayers.Select(x => x).Select((map, i) =>
            {
                if (map == null)
                    return null;

                var width = map.Map.Width * map.TileAssemble.GroupWidth;
                var height = map.Map.Height * map.TileAssemble.GroupHeight;

                return new Unity_Map()
                {
                    Width = (ushort)width,
                    Height = (ushort)height,
                    TileSetWidth = 1,
                    TileSet = new Unity_MapTileMap[]
                    {
                        tileSet
                    },
                    MapTiles = mapTiles[map].Select(x => new Unity_Tile(x)).ToArray(),
                };
            });

            if (context.Settings.EngineVersion == EngineVersion.GBAIsometric_Spyro2 && is2D)
                maps = maps.Reverse();

            return UniTask.FromResult(new Unity_Level(
                maps: maps.Where(x => x != null).ToArray(),
                objManager: new Unity_ObjectManager(context),
                eventData: new List<Unity_Object>(),
                cellSize: CellSize,
                defaultMap: 1));
        }

        public MapTile[] GetMapTiles(GBAIsometric_Spyro_MapLayer mapLayer)
        {
            var width = mapLayer.Map.Width * mapLayer.TileAssemble.GroupWidth;
            var height = mapLayer.Map.Height * mapLayer.TileAssemble.GroupHeight;
            var tiles = new MapTile[width * height];

            for (int blockY = 0; blockY < mapLayer.Map.Height; blockY++)
            {
                for (int blockX = 0; blockX < mapLayer.Map.Width; blockX++)
                {
                    var tileBlock = mapLayer.TileAssemble.TileGroups[mapLayer.Map.MapData[blockY * mapLayer.Map.Width + blockX]];

                    var actualX = blockX * mapLayer.TileAssemble.GroupWidth;
                    var actualY = blockY * mapLayer.TileAssemble.GroupHeight;

                    for (int y = 0; y < mapLayer.TileAssemble.GroupHeight; y++)
                    {
                        for (int x = 0; x < mapLayer.TileAssemble.GroupWidth; x++)
                        {
                            MapTile mt = tileBlock[y * mapLayer.TileAssemble.GroupWidth + x];
                            tiles[(actualY + y) * width + (actualX + x)] = new MapTile() {
                                TileMapY = (ushort)(mt.TileMapY + (mapLayer.TileSet.Region * 512)),
                                VerticalFlip = mt.VerticalFlip,
                                HorizontalFlip = mt.HorizontalFlip,
                                PaletteIndex = mt.PaletteIndex
                            };
                        }
                    }
                }
            }

            return tiles;
        }

        public Unity_MapTileMap LoadTileSet(ARGB1555Color[] tilePal, GBAIsometric_Spyro_TileSet[] tileSets, Dictionary<GBAIsometric_Spyro_MapLayer, MapTile[]> mapTiles)
        {
            var palettes = Enumerable.Range(0, 16).Select(x => tilePal.Skip(16 * x).Take(16).Select((c, i) =>
            {
                if (i != 0)
                    c.Alpha = 255;
                return c.GetColor();
            }).ToArray()).ToArray();

            const int tileSize = 32;
            const int regionSize = tileSize * 512;
            
            var tileSet = new byte[tileSets.Select(t => t.RegionOffset * tileSize + t.Region * regionSize + t.TileData.Length).Max()];

            // Fill regions with tile data
            foreach (var t in tileSets)
                Buffer.BlockCopy(t.TileData, 0, tileSet, t.RegionOffset * tileSize + t.Region * regionSize, t.TileData.Length);

            var tileSetTex = Util.ToTileSetTexture(tileSet, palettes.First(), false, CellSize, false, getPalFunc: i => palettes[mapTiles.SelectMany(x => x.Value).FirstOrDefault(x => x.TileMapY == i)?.PaletteIndex ?? 0]);

            return new Unity_MapTileMap(tileSetTex, CellSize);
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public virtual async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, 0x08000000);

        public class LevelInfo
        {
            public LevelInfo(int[] levelIDs, bool usesPointerArray, bool is2D, Dictionary<GameModeSelection, uint> offsets)
            {
                LevelIDs = levelIDs;
                UsesPointerArray = usesPointerArray;
                Is2D = is2D;
                Offsets = offsets;
            }

            public int[] LevelIDs { get; }
            public int Length => LevelIDs.Length;
            public bool UsesPointerArray { get; }
            public bool Is2D { get; }
            public Dictionary<GameModeSelection, uint> Offsets { get; }
        }
    }
}