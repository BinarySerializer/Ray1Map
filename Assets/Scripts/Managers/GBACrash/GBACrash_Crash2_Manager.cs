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

            var tileSets = map.MapData2D.MapLayers.Where(x => x != null).Select(x => x.TileSet).Distinct().ToDictionary(x => x, x => LoadTileSet(x.TileSet, Util.ConvertAndSplitGBAPalette(map.TilePalette2D)));

            Controller.DetailedState = "Loading maps";
            await Controller.WaitIfNecessary();

            var maps = map.MapData2D.MapLayers.Where(x => x != null).Select(x =>
            {
                return new Unity_Map
                {
                    Width = x.MapWidth,
                    Height = x.MapHeight,
                    TileSet = new Unity_TileSet[]
                    {
                        tileSets[x.TileSet]
                    },
                    MapTiles = Enumerable.Range(0, x.MapWidth * x.MapHeight).Select(t => new Unity_Tile(new MapTile())).ToArray(),
                    Type = Unity_Map.MapType.Graphics,
                };
            }).ToArray();

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

        public Unity_TileSet LoadTileSet(byte[] tileSet, Color[][] palettes)
        {
            var tex = Util.ToTileSetTexture(tileSet, palettes[0], Util.TileEncoding.Linear_4bpp, CellSize, false);

            return new Unity_TileSet(tex, CellSize);
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, GBA_ROMBase.Address_ROM);
    }
}