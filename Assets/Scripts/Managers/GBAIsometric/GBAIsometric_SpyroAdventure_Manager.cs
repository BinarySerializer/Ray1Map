using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_SpyroAdventure_Manager : IGameManager
    {
        public const int CellSize = 8;

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 83).ToArray()), // Levels 0 and 1 are not in the normal array!
        });

        public virtual string GetROMFilePath => $"ROM.gba";

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Data Blocks", false, true, (input, output) => ExportDataBlocks(settings, output))
        };

        public async UniTask ExportDataBlocks(GameSettings settings, string outputPath) {
            using (var context = new Context(settings)) {
                var s = context.Deserializer;
                await LoadFilesAsync(context);

                var rom = FileFactory.Read<GBAIsometric_Spyro_ROM>(GetROMFilePath, context);

                for (int i = 0; i < rom.DataTable.DataEntries.Length; i++)
                {
                    var data = rom.DataTable.DoAtBlock(context, i, size => s.SerializeArray<byte>(default, size, name: $"Block[{i}]"));
                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{i:000}_0x{rom.DataTable.DataEntries[i].DataPointer.AbsoluteOffset:X8}.dat"), data);
                }
            }
        }

        public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            var rom = FileFactory.Read<GBAIsometric_Spyro_ROM>(GetROMFilePath, context);

            var levelInfo = rom.LevelInfos.First(x => x.Value.ID == context.Settings.Level).Value;

            var maps = levelInfo.MapLayers.Select(x => x.Value).Select(map =>
            {
                var width = map.Map.Width * map.TileAssemble.GroupWidth;
                var height = map.Map.Height * map.TileAssemble.GroupHeight;
                var tiles = GetMapTiles(map);

                return new Unity_Map()
                {
                    Width = (ushort)width,
                    Height = (ushort)height,
                    TileSetWidth = 1,
                    TileSet = new Unity_MapTileMap[]
                    {
                        LoadTileSet(levelInfo.TilePalette, map.TileSet.TileData, tiles),
                    },
                    MapTiles = tiles.Select(x => new Unity_Tile(x)).ToArray(),
                };
            }).ToArray();

            return UniTask.FromResult(new Unity_Level(
                maps: maps,
                objManager: new Unity_ObjectManager(context),
                eventData: new List<Unity_Object>(),
                cellSize: CellSize));
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
                                TileMapY = (ushort)(mt.TileMapY - (mapLayer.TileSet.Uint_00 & 0x3fff)),
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

        public Unity_MapTileMap LoadTileSet(ARGB1555Color[] tilePal, byte[] tileSet, MapTile[] tiles)
        {
            var palettes = Enumerable.Range(0, 16).Select(x => tilePal.Skip(16 * x).Take(16).Select((c, i) =>
            {
                if (i != 0)
                    c.Alpha = 255;
                return c.GetColor();
            }).ToArray()).ToArray();

            var tileSetTex = Util.ToTileSetTexture(tileSet, palettes.First(), false, CellSize, false, getPalFunc: i => palettes[tiles.FirstOrDefault(x => x.TileMapY == i)?.PaletteIndex ?? 0]);

            return new Unity_MapTileMap(tileSetTex, CellSize);
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public virtual async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, 0x08000000);
    }
}