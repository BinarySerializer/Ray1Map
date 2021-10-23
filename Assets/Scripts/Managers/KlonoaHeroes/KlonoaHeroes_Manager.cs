using System.Collections.Generic;
using BinarySerializer;
using BinarySerializer.GBA;
using BinarySerializer.Klonoa;
using BinarySerializer.Klonoa.KH;
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class KlonoaHeroes_Manager : BaseGameManager
    {
        public const int CellSize = GBAConstants.TileSize;
        public const int CollisionCellSize = 32;
        public const string GetROMFilePath = "ROM.gba";

        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, "Game", Enumerable.Range(0, LevelEntries.Length).ToArray(), LevelEntries.Select(x => $"{x.ID1}-{x.ID2}-{x.ID3}").ToArray())
        });

        public static LevelEntry[] LevelEntries { get; } =
        {
            new LevelEntry(null, 1, 1, 1),
            new LevelEntry(null, 1, 2, 1),
            new LevelEntry(null, 1, 2, 2),
            new LevelEntry(null, 1, 2, 3),
            new LevelEntry(null, 1, 3, 1),
            new LevelEntry(null, 1, 3, 2),
            new LevelEntry(null, 1, 3, 3),
            new LevelEntry(null, 1, 3, 4),
            new LevelEntry(null, 1, 4, 1),
            new LevelEntry(null, 1, 4, 2),
            new LevelEntry(null, 1, 4, 3),
            new LevelEntry(null, 1, 4, 4),
            new LevelEntry(null, 1, 4, 5),
            new LevelEntry(null, 1, 5, 1),
            new LevelEntry(null, 1, 5, 2),
            new LevelEntry(null, 1, 5, 3),
            new LevelEntry(null, 1, 5, 4),
            new LevelEntry(null, 1, 5, 5),
            new LevelEntry(null, 1, 5, 6),
            new LevelEntry(null, 1, 6, 1),
            new LevelEntry(null, 1, 6, 2),
            new LevelEntry(null, 1, 6, 3),
            new LevelEntry(null, 1, 6, 4),
            new LevelEntry(null, 1, 6, 5),
            new LevelEntry(null, 2, 1, 1),
            new LevelEntry(null, 2, 1, 2),
            new LevelEntry(null, 2, 1, 3),
            new LevelEntry(null, 2, 1, 4),
            new LevelEntry(null, 2, 2, 1),
            new LevelEntry(null, 2, 2, 2),
            new LevelEntry(null, 2, 2, 3),
            new LevelEntry(null, 2, 2, 4),
            new LevelEntry(null, 2, 2, 5),
            new LevelEntry(null, 2, 3, 1),
            new LevelEntry(null, 2, 3, 2),
            new LevelEntry(null, 2, 3, 3),
            new LevelEntry(null, 2, 3, 4),
            new LevelEntry(null, 2, 3, 5),
            new LevelEntry(null, 2, 4, 1),
            new LevelEntry(null, 2, 4, 2),
            new LevelEntry(null, 2, 4, 3),
            new LevelEntry(null, 2, 4, 4),
            new LevelEntry(null, 2, 4, 5),
            new LevelEntry(null, 2, 4, 6),
            new LevelEntry(null, 3, 1, 1),
            new LevelEntry(null, 3, 1, 2),
            new LevelEntry(null, 3, 1, 3),
            new LevelEntry(null, 3, 1, 4),
            new LevelEntry(null, 3, 1, 5),
            new LevelEntry(null, 3, 2, 1),
            new LevelEntry(null, 3, 2, 2),
            new LevelEntry(null, 3, 2, 3),
            new LevelEntry(null, 3, 2, 4),
            new LevelEntry(null, 3, 2, 5),
            new LevelEntry(null, 3, 3, 1),
            new LevelEntry(null, 3, 3, 2),
            new LevelEntry(null, 3, 3, 3),
            new LevelEntry(null, 3, 3, 4),
            new LevelEntry(null, 3, 3, 5),
            new LevelEntry(null, 3, 3, 6),
            new LevelEntry(null, 3, 4, 1),
            new LevelEntry(null, 3, 4, 2),
            new LevelEntry(null, 3, 4, 3),
            new LevelEntry(null, 3, 4, 4),
            new LevelEntry(null, 3, 4, 5),
            new LevelEntry(null, 3, 4, 6),
            new LevelEntry(null, 3, 5, 1),
            new LevelEntry(null, 3, 5, 2),
            new LevelEntry(null, 3, 5, 3),
            new LevelEntry(null, 3, 5, 4),
            new LevelEntry(null, 3, 5, 5),
            new LevelEntry(null, 3, 5, 6),
            new LevelEntry(null, 3, 5, 7),
            new LevelEntry(null, 3, 7, 1),
            new LevelEntry(null, 3, 7, 2),
            new LevelEntry(null, 3, 7, 3),
            new LevelEntry(null, 4, 1, 1),
            new LevelEntry(null, 4, 1, 2),
            new LevelEntry(null, 4, 1, 3),
            new LevelEntry(null, 4, 1, 4),
            new LevelEntry(null, 4, 1, 5),
            new LevelEntry(null, 4, 1, 6),
            new LevelEntry(null, 4, 1, 7),
            new LevelEntry(null, 4, 1, 8),
            new LevelEntry(null, 4, 2, 1),
            new LevelEntry(null, 4, 2, 2),
            new LevelEntry(null, 4, 2, 3),
            new LevelEntry(null, 4, 2, 4),
            new LevelEntry(null, 4, 2, 5),
            new LevelEntry(null, 4, 3, 1),
            new LevelEntry(null, 4, 3, 2),
            new LevelEntry(null, 4, 3, 3),
            new LevelEntry(null, 4, 3, 4),
            new LevelEntry(null, 4, 3, 5),
            new LevelEntry(null, 4, 3, 6),
            new LevelEntry(null, 4, 4, 1),
            new LevelEntry(null, 4, 4, 2),
            new LevelEntry(null, 4, 4, 3),
            new LevelEntry(null, 4, 4, 4),
            new LevelEntry(null, 4, 4, 5),
            new LevelEntry(null, 4, 4, 6),
            new LevelEntry(null, 4, 4, 7),
            new LevelEntry(null, 5, 1, 1),
            new LevelEntry(null, 5, 1, 2),
            new LevelEntry(null, 5, 1, 3),
            new LevelEntry(null, 5, 1, 4),
            new LevelEntry(null, 5, 1, 5),
            new LevelEntry(null, 5, 2, 1),
            new LevelEntry(null, 5, 2, 2),
            new LevelEntry(null, 5, 2, 3),
            new LevelEntry(null, 5, 2, 4),
            new LevelEntry(null, 5, 2, 5),
            new LevelEntry(null, 5, 2, 6),
            new LevelEntry(null, 5, 3, 1),
            new LevelEntry(null, 5, 3, 2),
            new LevelEntry(null, 5, 3, 3),
            new LevelEntry(null, 5, 3, 4),
            new LevelEntry(null, 5, 3, 5),
            new LevelEntry(null, 5, 3, 6),
            new LevelEntry(null, 5, 4, 1),
            new LevelEntry(null, 5, 4, 2),
            new LevelEntry(null, 5, 4, 3),
            new LevelEntry(null, 5, 4, 4),
            new LevelEntry(null, 5, 4, 5),
            new LevelEntry(null, 5, 4, 6),
            new LevelEntry(null, 5, 4, 7),
            new LevelEntry(null, 6, 1, 1),
            new LevelEntry(null, 6, 1, 2),
            new LevelEntry(null, 6, 1, 3),
            new LevelEntry(null, 6, 1, 4),
            new LevelEntry(null, 6, 1, 5),
            new LevelEntry(null, 6, 1, 6),
            new LevelEntry(null, 6, 2, 1),
            new LevelEntry(null, 6, 2, 2),
            new LevelEntry(null, 6, 2, 3),
            new LevelEntry(null, 6, 2, 4),
            new LevelEntry(null, 6, 2, 5),
            new LevelEntry(null, 6, 2, 6),
            new LevelEntry(null, 6, 3, 1),
            new LevelEntry(null, 6, 3, 2),
            new LevelEntry(null, 6, 3, 3),
            new LevelEntry(null, 6, 3, 4),
            new LevelEntry(null, 6, 3, 5),
            new LevelEntry(null, 6, 3, 6),
            new LevelEntry(null, 6, 3, 7),
            new LevelEntry(null, 6, 3, 8),
            new LevelEntry(null, 7, 1, 1),
            new LevelEntry(null, 7, 1, 2),
            new LevelEntry(null, 7, 1, 3),
            new LevelEntry(null, 7, 1, 4),
            new LevelEntry(null, 7, 1, 5),
            new LevelEntry(null, 7, 1, 6),
            new LevelEntry(null, 7, 1, 7),
            new LevelEntry(null, 7, 2, 1),
            new LevelEntry(null, 7, 2, 2),
            new LevelEntry(null, 7, 2, 3),
            new LevelEntry(null, 7, 2, 4),
            new LevelEntry(null, 7, 2, 5),
            new LevelEntry(null, 7, 2, 6),
            new LevelEntry(null, 7, 2, 7),
            new LevelEntry(null, 7, 3, 1),
            new LevelEntry(null, 7, 3, 2),
            new LevelEntry(null, 7, 3, 3),
            new LevelEntry(null, 7, 3, 4),
            new LevelEntry(null, 7, 3, 5),
            new LevelEntry(null, 7, 3, 6),
            new LevelEntry(null, 7, 3, 7),
            new LevelEntry(null, 7, 3, 8),
            new LevelEntry(null, 7, 3, 9),
            new LevelEntry(null, 7, 3, 10),
            new LevelEntry(null, 8, 1, 1),
            new LevelEntry(null, 8, 1, 2),
            new LevelEntry(null, 8, 1, 3),
            new LevelEntry(null, 10, 1, 1),
            new LevelEntry(null, 10, 2, 1),
            new LevelEntry(null, 10, 3, 1),
            new LevelEntry(null, 10, 4, 1),
            new LevelEntry(null, 10, 5, 1),
            new LevelEntry(null, 10, 6, 1),
            new LevelEntry(null, 10, 7, 1),
            new LevelEntry(null, 10, 8, 1),
            new LevelEntry(null, 10, 8, 2),
            new LevelEntry(null, 10, 8, 3),
            new LevelEntry(null, 11, 1, 1),
            new LevelEntry(null, 11, 1, 2),
            new LevelEntry(null, 11, 1, 3),
            new LevelEntry(null, 11, 1, 4),
            new LevelEntry(null, 12, 1, 1),
            new LevelEntry(null, 12, 1, 2),
            new LevelEntry(null, 12, 1, 3),
            new LevelEntry(null, 12, 1, 4),
            new LevelEntry(null, 13, 1, 1),
            new LevelEntry(null, 13, 1, 2),
            new LevelEntry(null, 13, 1, 3),
            new LevelEntry(null, 13, 1, 4),
            new LevelEntry(null, 13, 1, 5),
            new LevelEntry(null, 14, 1, 1),
            new LevelEntry(null, 14, 1, 2),
            new LevelEntry(null, 14, 1, 3),
            new LevelEntry(null, 14, 1, 4),
            new LevelEntry(null, 15, 1, 1),
            new LevelEntry(null, 15, 1, 2),
            new LevelEntry(null, 15, 1, 3),
            new LevelEntry(null, 15, 1, 4),
            new LevelEntry(null, 16, 1, 1),
            new LevelEntry(null, 16, 1, 2),
            new LevelEntry(null, 16, 1, 3),
            new LevelEntry(null, 16, 1, 4),
            new LevelEntry(null, 17, 1, 1),
            new LevelEntry(null, 17, 1, 2),
            new LevelEntry(null, 17, 1, 3),
            new LevelEntry(null, 17, 1, 4),
        };

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            // Get the current level entry
            LevelEntry lvlEntry = LevelEntries[context.GetR1Settings().Level];

            // Add settings
            context.AddKlonoaSettings(new KlonoaSettings_KH()
            {
                SerializeMap = new KlonoaSettings_KH.MapID(lvlEntry.ID1, lvlEntry.ID2, lvlEntry.ID3)
            });

            // Create the level
            var lvl = new Unity_Level();

            // Read the ROM
            var rom = FileFactory.Read<KlonoaHeroesROM>(GetROMFilePath, context);

            // Get the map
            var map = rom.MapsPack.GetMap(lvlEntry.ID1, lvlEntry.ID2, lvlEntry.ID3);

            Controller.DetailedState = "Loading maps";
            await Controller.WaitIfNecessary();

            var tileSet = LoadTileSet(
                tileSet: map.SpecialMapLayer.SharedTileSet.Concat(map.SpecialMapLayer.TileSet ?? Enumerable.Empty<byte>()).ToArray(),
                pal: map.SpecialMapLayer.Palette,
                mapTiles: map.MapLayers.Where(x => x != null).SelectMany(x => x.TileMap).ToArray());

            lvl.CellSize = CellSize;
            var maps = map.MapLayers.Where(x => x != null).Reverse().OrderByDescending(x => x.Priority).Select(x =>
            {
                return new Unity_Map
                {
                    Width = (ushort)(x.Width / CellSize),
                    Height = (ushort)(x.Height / CellSize),
                    TileSet = new Unity_TileSet[]
                    {
                        tileSet
                    },
                    MapTiles = x.TileMap.Select(t => new Unity_Tile(new MapTile
                    {
                        TileMapY = (ushort)t.TileSetIndex,
                        HorizontalFlip = t.FlipX,
                        VerticalFlip = t.FlipY,
                        PaletteIndex = (byte)t.PaletteIndex,
                    })).ToArray(),
                    Type = Unity_Map.MapType.Graphics,
                };
            });

            if (map.SpecialMapLayer.CollisionMap != null)
            {
                maps = maps.Append(new Unity_Map
                {
                    Width = (ushort)(map.SpecialMapLayer.Width / CollisionCellSize),
                    Height = (ushort)(map.SpecialMapLayer.Height / CollisionCellSize),
                    MapTiles = map.SpecialMapLayer.CollisionMap.Select(x => new Unity_Tile(new MapTile()
                    {
                        CollisionType = x
                    })).ToArray(),
                    Type = Unity_Map.MapType.Collision,
                });

                lvl.CellSizeOverrideCollision = CollisionCellSize;

                // TODO: Fill out rest of the types and add a names func
                lvl.GetCollisionTypeGraphicFunc = c => c switch
                {
                    0 => Unity_MapCollisionTypeGraphic.None,
                    1 => Unity_MapCollisionTypeGraphic.Solid,
                    2 => Unity_MapCollisionTypeGraphic.Hill_Steep_Left,
                    3 => Unity_MapCollisionTypeGraphic.Hill_Steep_Right,
                    4 => Unity_MapCollisionTypeGraphic.Angle_Top_Left,
                    5 => Unity_MapCollisionTypeGraphic.Angle_Top_Right,

                    6 => Unity_MapCollisionTypeGraphic.Passthrough,
                    7 => Unity_MapCollisionTypeGraphic.Hill_Steep_Left,
                    8 => Unity_MapCollisionTypeGraphic.Hill_Steep_Right,
                    9 => Unity_MapCollisionTypeGraphic.Angle_Top_Left,
                    10 => Unity_MapCollisionTypeGraphic.Angle_Top_Right,

                    _ => Unity_MapCollisionTypeGraphic.Unknown0,
                };
            }

            lvl.Maps = maps.ToArray();

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            // TODO: Create obj class
            lvl.EventData = map.MapObjects.Objects.Select(x => (Unity_SpriteObject)new Unity_Object_Dummy(x, Unity_ObjectType.Object)
            {
                Position = new Vector3(x.XPos, x.YPos, x.ZPos),
                XPosition = x.XPos,
                YPosition = x.YPos
            }).ToList();

            // TODO: Create manager and parse animations
            lvl.ObjManager = new Unity_ObjectManager(context);

            return lvl;
        }

        public Unity_TileSet LoadTileSet(byte[] tileSet, RGBA5551Color[] pal, GraphicsTile[] mapTiles)
        {
            var additionalTiles = new List<Texture2D>();
            const int tileSize = 0x20;
            var paletteIndices = Enumerable.Range(0, tileSet.Length / tileSize).Select(x => new List<int>()).ToArray();
            var tilesCount = tileSet.Length / tileSize;

            var palettes = Util.ConvertAndSplitGBAPalette(pal);

            foreach (var m in mapTiles)
            {
                if (m.TileSetIndex < paletteIndices.Length && !paletteIndices[m.TileSetIndex].Contains(m.PaletteIndex))
                    paletteIndices[m.TileSetIndex].Add(m.PaletteIndex);
            }

            Texture2D tex = Util.ToTileSetTexture(tileSet, palettes[0], Util.TileEncoding.Linear_4bpp, CellSize, false,
                getPalFunc: x =>
                {
                    var p = paletteIndices[x].ElementAtOrDefault(0);
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
                    foreach (GraphicsTile t in mapTiles.Where(x => x.TileSetIndex == tileIndex && x.PaletteIndex == p))
                        t.TileSetIndex = (ushort)(tilesCount + additionalTiles.Count);

                    // Add to additional tiles list
                    additionalTiles.Add(tileTex);
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

            return new Unity_TileSet(tiles);
        }

        public override async UniTask LoadFilesAsync(Context context) => await context.AddMemoryMappedFile(GetROMFilePath, GBAConstants.Address_ROM);

        public class LevelEntry
        {
            public LevelEntry(string name, int id1, int id2, int id3)
            {
                Name = name;
                ID1 = id1;
                ID2 = id2;
                ID3 = id3;
            }

            public string Name { get; }
            public int ID1 { get; }
            public int ID2 { get; }
            public int ID3 { get; }
        }
    }
}