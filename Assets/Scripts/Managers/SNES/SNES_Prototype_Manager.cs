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
                    Type = Unity_Map.MapType.Graphics | Unity_Map.MapType.Collision,

                    // Set the dimensions
                    Width = map.Width,
                    Height = map.Height,

                    // Create the tile arrays
                    TileSet = new Unity_MapTileMap[1],
                    MapTiles = map.Tiles.Select(x => new Unity_Tile(x)).ToArray(),
                }
            };

            // Convert levelData to common level format
            Unity_Level level = new Unity_Level(maps, new Unity_ObjectManager(context),
                getCollisionTypeNameFunc: x => ((R1Jaguar_TileCollisionType)x).ToString(),
                getCollisionTypeGraphicFunc: x => ((R1Jaguar_TileCollisionType)x).GetCollisionTypeGraphic());

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
            var tex = TextureHelpers.CreateTexture2D(256, Mathf.CeilToInt(length / 256f / Settings.CellSize) * Settings.CellSize, clear: true);

            var pal = Util.ConvertAndSplitGBAPalette(rom.Palettes);

            for (int i = 0; i < rom.TileDescriptors.Length; i++)
            {
                var descriptor = rom.TileDescriptors[i];

                var x = ((i / 4) * 2) % (256 / 8) + ((i % 2) == 0 ? 0 : 1);
                var y = (((i / 4) * 2) / (256 / 8)) * 2 + ((i % 4) < 2 ? 0 : 1);

                var curOff = block_size * descriptor.TileIndex;

                tex.FillInTile(
                    imgData: rom.TileMap, 
                    imgDataOffset: curOff, 
                    pal: pal[descriptor.Palette], 
                    encoding: Util.TileEncoding.Planar_4bpp, 
                    tileWidth: 8, 
                    flipTextureY: false,
                    tileX: x * 8,
                    tileY: y * 8,
                    flipTileX: !descriptor.FlipX,
                    flipTileY: descriptor.FlipY,
                    ignoreTransparent: true);
            }

            tex.Apply();

            return new Unity_MapTileMap(tex, Settings.CellSize);
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public async UniTask LoadFilesAsync(Context context) => await context.AddLinearSerializedFileAsync(GetROMFilePath);
    }
}