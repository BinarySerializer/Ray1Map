using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    public class SNES_Prototype_Manager : IGameManager
    {
        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, new int[]
            {
                0
            })
        });

        public virtual string GetROMFilePath => $"ROM.sfc";

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[0];

        public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = $"Loading data";
            await Controller.WaitIfNecessary();

            // Read the rom
            var rom = FileFactory.Read<SNES_Proto_ROM>(GetROMFilePath, context);

            // Get the map
            var map = rom.MapData;

            var maps = new Unity_Map[]
            {
                new Unity_Map()
                {
                    // Set the dimensions
                    Width = map.Width,
                    Height = map.Height,

                    // Create the tile arrays
                    TileSet = new Unity_MapTileMap[1],
                    MapTiles = map.Tiles.Select(x => new Unity_Tile(x)).ToArray(),
                }
            };

            // Convert levelData to common level format
            Unity_Level level = new Unity_Level(maps, new Unity_ObjectManager(context), getCollisionTypeGraphicFunc: x => ((R1Jaguar_TileCollisionType)x).GetCollisionTypeGraphic());

            Controller.DetailedState = $"Loading tile set";
            await Controller.WaitIfNecessary();

            // Load tile set and treat black as transparent
            level.Maps[0].TileSet[0] = GetTileSet(context, rom);

            return level;
        }

        public virtual Unity_MapTileMap GetTileSet(Context context, SNES_Proto_ROM rom)
        {
            // Read the tiles
            const int block_size = 0x20;

            uint length = (uint)rom.TileDescriptors.Length * 8 * 8;

            // Get the tile-set texture
            var tex = TextureHelpers.CreateTexture2D(256, Mathf.CeilToInt(length / 256f / Settings.CellSize) * Settings.CellSize);

            for (int i = 0; i < rom.TileDescriptors.Length; i++)
            {
                var descriptor = rom.TileDescriptors[i];

                var x = ((i / 4) * 2) % (256 / 8) + ((i % 2) == 0 ? 0 : 1);
                var y = (((i / 4) * 2) / (256 / 8)) * 2 + ((i % 4) < 2 ? 0 : 1);

                var curOff = block_size * descriptor.TileIndex;

                FillTextureBlock(tex, 0, 0, x, y, rom.TileMap, curOff, rom.Palettes, descriptor.Palette, descriptor.FlipX, descriptor.FlipY);
            }

            tex.Apply();

            return new Unity_MapTileMap(tex, Settings.CellSize);
        }

        public void FillTextureBlock(Texture2D tex, int blockX, int blockY, int relX, int relY, byte[] imageBuffer, int imageBufferOffset, IList<ARGB1555Color> pal, int paletteInd, bool flipX, bool flipY)
        {
            var offset1 = imageBufferOffset;
            var offset2 = imageBufferOffset + 16;

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    int actualX = blockX + relX * 8 + (flipX ? (8 - x - 1) : x);
                    int actualY = blockY + relY * 8 + (flipY ? (8 - y - 1) : y);

                    int off = y * 8 + (8 - x - 1);

                    var bit0 = BitHelpers.ExtractBits(imageBuffer[offset1 + ((off / 8) * 2)], 1, off % 8);
                    var bit1 = BitHelpers.ExtractBits(imageBuffer[offset1 + ((off / 8) * 2 + 1)], 1, off % 8);
                    var bit2 = BitHelpers.ExtractBits(imageBuffer[offset2 + ((off / 8) * 2)], 1, off % 8);
                    var bit3 = BitHelpers.ExtractBits(imageBuffer[offset2 + ((off / 8) * 2 + 1)], 1, off % 8);

                    int b = 0;

                    b = BitHelpers.SetBits(b, bit0, 1, 0);
                    b = BitHelpers.SetBits(b, bit1, 1, 1);
                    b = BitHelpers.SetBits(b, bit2, 1, 2);
                    b = BitHelpers.SetBits(b, bit3, 1, 3);
                    
                    Color c = pal[paletteInd * 0x10 + b].GetColor();

                    if (b != 0)
                        c = new Color(c.r, c.g, c.b, 1f);

                    tex.SetPixel(actualX, actualY, c);
                }
            }
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public async UniTask LoadFilesAsync(Context context) => await context.AddLinearSerializedFileAsync(GetROMFilePath);
    }
}