using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBACrash_Crash2_Manager : IGameManager
    {
        public const string LocTableID = "LocTable";

        public const int CellSize = 8;
        public string GetROMFilePath => "ROM.gba";

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 1).ToArray()),
        });

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[0];

        public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBACrash_ROM>(GetROMFilePath, context);
            var map = rom.LevelInfos[0].LevelData.Maps[0];

            Controller.DetailedState = "Loading tilesets";
            await Controller.WaitIfNecessary();

            var tileSets = new Dictionary<GBACrash_TileSet, Unity_TileSet>();

            for (int i = 0; i < map.MapData2D.MapLayers.Length; i++)
            {
                var l = map.MapData2D.MapLayers[i];

                if (l == null)
                    continue;

                if (!tileSets.ContainsKey(l.TileSet))
                    tileSets.Add(l.TileSet, LoadTileSet(l.TileSet.TileSet, map.TilePalette2D, i == 3));
            }

            Controller.DetailedState = "Loading maps";
            await Controller.WaitIfNecessary();

            var maps = map.MapData2D.MapLayers.Select((x, i) =>
            {
                if (x == null)
                    return null;

                return new
                {
                    Map = new Unity_Map
                    {
                        Width = x.MapWidth,
                        Height = x.MapHeight,
                        TileSet = new Unity_TileSet[]
                        {
                            tileSets[x.TileSet]
                        },
                        MapTiles = GetTileMap(x, map.MapData2D.DataBlock.TileLayerDatas[i], i == 3, x.TileSet.TileSet.Length / 32),
                        Type = Unity_Map.MapType.Graphics,
                    },
                    Prio = x.LayerPrio
                };
            }).Where(x => x != null).OrderByDescending(x => x.Prio).Select(x => x.Map).ToArray();

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            var objmanager = new Unity_ObjectManager(context);
            var objects = map.MapData2D.ObjData.ObjGroups.SelectMany(x => x.Objects).Select(x => new Unity_Object_GBACrash(objmanager, x));

            return new Unity_Level(
                maps: maps,
                objManager: objmanager,
                eventData: new List<Unity_Object>(objects),
                cellSize: CellSize,
                defaultMap: 2);
        }

        public Unity_TileSet LoadTileSet(byte[] tileSet, RGBA5551Color[] pal, bool is8bit)
        {
            Texture2D tex;

            if (is8bit)
            { 
                tex = Util.ToTileSetTexture(tileSet, Util.ConvertGBAPalette(pal), Util.TileEncoding.Linear_8bpp, CellSize, false);
            }
            else
            {
                var palettes = Util.ConvertAndSplitGBAPalette(pal);

                const int wrap = 32;
                const int tileSize = 32;
                int tilesetLength = tileSet.Length / tileSize;

                int tilesX = Math.Min((tilesetLength * palettes.Length), wrap);
                int tilesY = Mathf.CeilToInt((tilesetLength * palettes.Length) / (float)wrap);

                tex = TextureHelpers.CreateTexture2D(tilesX * CellSize, tilesY * CellSize);

                for (int palIndex = 0; palIndex < palettes.Length; palIndex++)
                {
                    var totalIndex = tilesetLength * palIndex;

                    for (int i = 0; i < tilesetLength; i++)
                    {
                        int tileY = (((i + totalIndex) / wrap)) * CellSize;
                        int tileX = ((i + totalIndex) % wrap) * CellSize;

                        tex.FillInTile(
                            imgData: tileSet,
                            imgDataOffset: i * tileSize,
                            pal: palettes[palIndex],
                            encoding: Util.TileEncoding.Linear_4bpp,
                            tileWidth: CellSize,
                            flipTextureY: false,
                            tileX: tileX,
                            tileY: tileY);
                    }
                }

                tex.Apply();
            }

            return new Unity_TileSet(tex, CellSize);
        }

        public Unity_Tile[] GetTileMap(GBACrash_MapLayer layer, GBACrash_MapData2DDataBlock.GBACrash_TileLayerData tileLayerData, bool is8bit, int tileSetLength)
        {
            var tileMap = new Unity_Tile[layer.MapWidth * layer.MapHeight];

            const int blockWidth = 16;
            const int blockHeight = 8;

            for (int y = 0; y < layer.TileMapHeight; y++)
            {
                for (int x = 0; x < layer.TileMapWidth; x++)
                {
                    var actualX = x * blockWidth;
                    var actualY = y * blockHeight;

                    var cmds = tileLayerData.TileMapTileCommands[layer.TileMap[y * layer.TileMapWidth + x]];

                    var cmdIndex = 0;
                    var parsedTiles = 0x7f;
                    var index = 0;

                    do
                    {
                        var cmd = cmds.TileCommands[cmdIndex++];
                        var count = cmd.TilesCount;

                        if ((cmd.CommandType & 0x80) == 0)
                        {
                            if ((cmd.CommandType & 0x40) == 0)
                            {
                                parsedTiles -= count;

                                for (int i = 0; i < count; i++)
                                    setTileAt(index++, cmd.TileIndices[i], cmd.CommandType);
                            }
                            else
                            {
                                parsedTiles -= count;

                                // Get the initial tile index
                                int curTileIndex = cmd.TileIndices[0];

                                // Set the initial tile index
                                setTileAt(index++, curTileIndex, cmd.CommandType);

                                // Set every changed tile index
                                for (int i = 0; i < cmd.TilesCount - 1; i++)
                                {
                                    // Change the tile index
                                    curTileIndex += cmd.TileChanges[i];

                                    // Set changed index
                                    setTileAt(index++, curTileIndex, cmd.CommandType);
                                }
                            }
                        }
                        else
                        {
                            parsedTiles -= count;

                            for (int i = 0; i < count; i++)
                                setTileAt(index++, cmd.TileIndices[0], cmd.CommandType);
                        }
                    } while (-1 < parsedTiles);

                    void setTileAt(int blockIndex, int tileIndex, byte cmd)
                    {
                        int offY = Mathf.FloorToInt(blockIndex / (float)blockWidth);
                        int offX = blockIndex - offY * blockWidth;

                        var outputX = actualX + offX;
                        var outputY = actualY + offY;

                        if (outputX >= layer.MapWidth)
                            return;

                        var tileMapIndex = outputY * layer.MapWidth + outputX;

                        if (tileMap.Length <= tileMapIndex)
                            return;

                        tileMap[tileMapIndex] = new Unity_Tile(is8bit ? new MapTile()
                        {
                            TileMapY = (ushort)(BitHelpers.ExtractBits(tileIndex, 14, 0)),
                            HorizontalFlip = BitHelpers.ExtractBits(tileIndex, 1, 14) == 1,
                            VerticalFlip = BitHelpers.ExtractBits(tileIndex, 1, 15) == 1,
                        } : new MapTile()
                        {
                            TileMapY = (ushort)(BitHelpers.ExtractBits(tileIndex, 10, 0) + (tileSetLength * BitHelpers.ExtractBits(tileIndex, 4, 12))),
                            HorizontalFlip = BitHelpers.ExtractBits(tileIndex, 1, 10) == 1,
                            VerticalFlip = BitHelpers.ExtractBits(tileIndex, 1, 11) == 1,
                        })
                        {
                            DebugText = $"CMD: {cmd}{Environment.NewLine}" +
                                        $"Index: {blockIndex}{Environment.NewLine}" +
                                        $"TileIndex: {tileIndex}{Environment.NewLine}"
                        };
                    }
                }
            }

            return tileMap;
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, GBA_ROMBase.Address_ROM);
    }
}