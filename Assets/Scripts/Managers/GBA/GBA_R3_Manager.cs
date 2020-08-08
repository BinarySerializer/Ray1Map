using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    public class GBA_R3_Manager : IGameManager
    {
        public KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => new KeyValuePair<World, int[]>[]
        {
            new KeyValuePair<World, int[]>(World.Jungle, Enumerable.Range(1, 65).ToArray()), 
        };

        public string[] GetEduVolumes(GameSettings settings) => new string[0];

        public virtual string GetROMFilePath => $"ROM.gba";

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[0];

        public async Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            Controller.status = $"Loading data";
            await Controller.WaitIfNecessary();

            // Read the rom
            var rom = FileFactory.Read<GBA_R3_ROM>(GetROMFilePath, context);

            // Get the primary map
            var map = rom.BG_2;

            var tilemapLength = (rom.Tilemap.Length / 32);

            // Convert levelData to common level format
            Common_Lev commonLev = new Common_Lev
            {
                // Create the map
                Maps = new Common_LevelMap[]
                {
                    new Common_LevelMap()
                    {
                        // Set the dimensions
                        Width = map.Width,
                        Height = map.Height,

                        // Create the tile arrays
                        TileSet = new Common_Tileset[3],
                        MapTiles = map.MapData.Select((x, i) => new Editor_MapTile(new MapTile()
                        {
                            CollisionType = (byte)rom.CollisionMap.CollisionData[i],
                            TileMapY = (ushort)(BitHelpers.ExtractBits(x, 11, 0) + (BitHelpers.ExtractBits(x, 4, 12) * tilemapLength)),
                            HorizontalFlip = BitHelpers.ExtractBits(x, 1, 11) == 1,
                        })
                        {
                            DebugText = Convert.ToString(x, 2).PadLeft(16, '0')
                        }).ToArray(),
                        TileSetWidth = 1
                    }
                },

                // Create the events list
                EventData = new List<Editor_EventData>(),
            };

            const int numPalettes = 16;
            const int paletteSize = 16;
            const int tileWidth = 8;
            const int tileSize = (tileWidth * tileWidth) / 2;

            var tiles = new Tile[tilemapLength * numPalettes];

            // Hack: Create a tilemap for each palette
            for (int p = 0; p < numPalettes; p++)
            {
                for (int i = 0; i < tilemapLength; i++)
                {
                    var tex = new Texture2D(Settings.CellSize, Settings.CellSize)
                    {
                        filterMode = FilterMode.Point,
                        wrapMode = TextureWrapMode.Clamp
                    };

                    for (int y = 0; y < tileWidth; y++)
                    {
                        for (int x = 0; x < tileWidth; x++)
                        {
                            var b = rom.Tilemap[(i * tileSize) + ((y * tileWidth + x) / 2)];
                            var v = BitHelpers.ExtractBits(b, 4, x % 2 == 0 ? 0 : 4);

                            var c = rom.BGPalette[p * paletteSize + v].GetColor();

                            if (v != 0 && i != 0)
                                c = new Color(c.r, c.g, c.b, 1f);

                            // Upscale to 16x16 for now...
                            tex.SetPixel(x * 2, y * 2, c);
                            tex.SetPixel(x * 2 + 1, y * 2, c);
                            tex.SetPixel(x * 2 + 1, y * 2 + 1, c);
                            tex.SetPixel(x * 2, y * 2 + 1, c);
                        }
                    }

                    tex.Apply();

                    // Create a tile
                    Tile t = ScriptableObject.CreateInstance<Tile>();
                    t.sprite = Sprite.Create(tex, new Rect(0, 0, Settings.CellSize, Settings.CellSize), new Vector2(0.5f, 0.5f), Settings.CellSize, 20);

                    tiles[p * tilemapLength + i] = t;
                }
            }

            commonLev.Maps[0].TileSet[0] = new Common_Tileset(tiles);

            commonLev.EventData = rom.ObjBlocks[0].MapObjects.Append(rom.ObjBlocks[0].StartPosObj).Select(x => new Editor_EventData(new EventData()
            {
                XPosition = x.XPos * 2,
                YPosition = x.YPos * 2
            })
            {
                DESKey = String.Empty,
                ETAKey = String.Empty,
                DebugText = $"{nameof(GBA_R3_MapObj.Unk_00)}: {x.Unk_00}{Environment.NewLine}" +
                            $"{nameof(GBA_R3_MapObj.Unk_08)}: {x.Unk_08}{Environment.NewLine}" +
                            $"{nameof(GBA_R3_MapObj.Unk_09)}: {x.Unk_09}{Environment.NewLine}" +
                            $"{nameof(GBA_R3_MapObj.Unk_0A)}: {x.Unk_0A}{Environment.NewLine}" +
                            $"{nameof(GBA_R3_MapObj.Unk_0B)}: {x.Unk_0B}{Environment.NewLine}"
            }).ToList();

            return new GBA_EditorManager(commonLev, context);
        }

        public void SaveLevel(Context context, BaseEditorManager editorManager) => throw new NotImplementedException();

        public async Task LoadFilesAsync(Context context)
        {
            await FileSystem.PrepareFile(context.BasePath + GetROMFilePath);

            var file = new GBAMemoryMappedFile(context, 0x08000000)
            {
                filePath = GetROMFilePath,
            };
            context.AddFile(file);
        }
    }
}