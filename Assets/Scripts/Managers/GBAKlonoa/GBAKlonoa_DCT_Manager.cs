using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    public class GBAKlonoa_DCT_Manager : GBAKlonoa_BaseManager
    {
        public const int FixCount = 0x0D; // Fixed objects loaded into every level
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
            var isMap = settings.Level == 0;
            var isWaterSkii = settings.Level == 4;

            GenerateAnimSetTable(context, rom);

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

            var collisionLines = !isMap && !isWaterSkii ? GetSectorCollisionLines(rom.MapSectors[normalLevelIndex]) : null;

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            var fixObjects = rom.FixObjects;

            GBAKlonoa_LoadedObject[][] levelObjects;

            if (isMap)
            {
                //levelObjects = new GBAKlonoa_LoadedObject[][]
                //{
                //    rom.WorldMapObjectCollection.Objects.Select((x, index) => x.ToLoadedObject((short)(FixCount + index))).ToArray()
                //};
                throw new NotImplementedException();
            }
            else if (isWaterSkii)
            {
                throw new NotImplementedException();
            }
            else
            {
                // Get the objects for every sector
                levelObjects = Enumerable.Range(0, 5).Select(sector => rom.LevelObjectCollection.Objects.Select((obj, index) => obj.ToLoadedObject((short)(FixCount + index), sector)).ToArray()).ToArray();
            }

            var firstLoadedObjects = new List<GBAKlonoa_LoadedObject>();

            firstLoadedObjects.AddRange(fixObjects);

            for (int lvlObjIndex = 0; lvlObjIndex < levelObjects[0].Length; lvlObjIndex++)
                firstLoadedObjects.Add(levelObjects.Select(x => x[lvlObjIndex]).FirstOrDefault(x => isMap || x.Value_8 != 28));

            var objmanager = new Unity_ObjectManager_GBAKlonoa(
                context: context,
                animSets: LoadAnimSets(rom));

            var objects = new List<Unity_Object>();

            var hasStartPositions = !isMap && !isWaterSkii;

            // Add Klonoa to each defined start position
            if (hasStartPositions)
            {
                var startInfos = new GBAKlonoa_LevelStartInfo[0].
                    Append(rom.LevelStartInfos[normalLevelIndex].StartInfo_Entry).
                    Concat(rom.LevelStartInfos[normalLevelIndex].StartInfos_Yellow).
                    Concat(rom.LevelStartInfos[normalLevelIndex].StartInfos_Green).
                    Where(x => !(x.XPos == 0 && x.YPos == 0));

                // Add Klonoa at each start position
                objects.AddRange(startInfos.Select(x => new Unity_Object_GBAKlonoa(objmanager, new GBAKlonoa_LoadedObject(0, 0, x.XPos, x.YPos, 0, 0, 0, 0, 0x6e), x, rom.FixObjectOAMCollections[0])));
            }

            // Add fixed objects, except Klonoa (first one) and object 11/12 for maps since it's not used
            objects.AddRange(fixObjects.Skip(!hasStartPositions ? 0 : 1).Where(x => !isMap || (x.Index != 11 && x.Index != 12)).Select(x => new Unity_Object_GBAKlonoa(objmanager, x, null, rom.FixObjectOAMCollections[x.OAMIndex])));

            // Add level objects, duplicating them for each sector they appear in (if flags is 28 we assume it's unused in the sector)
            objects.AddRange(levelObjects.SelectMany(x => x).Where(x => isMap || x.Value_8 != 31).Select(x => new Unity_Object_GBAKlonoa(objmanager, x, (BinarySerializable)x.LevelObj ?? x.WorldMapObj, null)));

            return new Unity_Level(
                maps: maps,
                objManager: objmanager,
                eventData: objects,
                cellSize: CellSize,
                defaultLayer: 2,
                collisionLines: collisionLines);
        }

        public Unity_ObjectManager_GBAKlonoa.AnimSet[] LoadAnimSets(GBAKlonoa_DCT_ROM rom)
        {
            var loadedAnimSets = new List<Unity_ObjectManager_GBAKlonoa.AnimSet>();

            // Start by adding a null entry to use as default
            loadedAnimSets.Add(new Unity_ObjectManager_GBAKlonoa.AnimSet(new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation[0], $"NULL", new GBAKlonoa_OAM[0][]));

            var fixPal = Util.ConvertAndSplitGBAPalette(rom.FixObjectPalettes.SelectMany(x => x.Colors).ToArray());

            // Load animations declared through the graphics data
            LoadAnimSets_ObjGraphics(loadedAnimSets, rom.FixObjectGraphics, rom.FixObjectOAMCollections, fixPal, rom.FixObjects);

            var graphicsIndex = 0;

            // Load animations sets from the graphics datas
            foreach (var graphics in rom.GraphicsDatas)
            {
                GBAKlonoa_Animation[] anims;

                if (graphics.Flags2.HasFlag(GBAKlonoa_DCT_GraphicsData.GraphicsFlags2.HasAnimations))
                    anims = graphics.Animations;
                else
                    anims = new GBAKlonoa_Animation[]
                    {
                        new GBAKlonoa_Animation()
                        {
                            Frames = new GBAKlonoa_AnimationFrame[]
                            {
                                new GBAKlonoa_AnimationFrame()
                                {
                                    ImgData = graphics.ImgData
                                }
                            }
                        }
                    };

                loadedAnimSets.Add(LoadAnimSet(anims, graphics.OAMs, new Color[][]
                {
                    Util.ConvertGBAPalette(graphics.Palette.Colors)
                }, singlePal: true, dct_GraphicsIndex: graphicsIndex));

                graphicsIndex++;
            }

            return loadedAnimSets.ToArray();
        }

        public void GenerateAnimSetTable(Context context, GBAKlonoa_DCT_ROM rom)
        {
            var s = context.Deserializer;
            var file = rom.Offset.File;

            var graphicsDatas = rom.GraphicsDatas;

            var fixGraphics1 = s.DoAt(new Pointer(0x08070ae8, file), () => s.SerializeObjectArray<GBAKlonoa_ObjectGraphics>(null, 9));
            var fixGraphics2 = s.DoAt(new Pointer(0x08070b54, file), () => s.SerializeObjectArray<GBAKlonoa_ObjectGraphics>(null, 9));
            var fixGraphics3 = s.DoAt(new Pointer(0x08070a10, file), () => s.SerializeObjectArray<GBAKlonoa_ObjectGraphics>(null, 9));

            var str = new StringBuilder();

            var allGraphics = graphicsDatas.
                Select(x => x.AnimationsPointer).
                Concat(fixGraphics1.Select(x => x.AnimationsPointer)).
                Concat(fixGraphics2.Select(x => x.AnimationsPointer)).
                Concat(fixGraphics3.Select(x => x.AnimationsPointer)).
                Where(x => x != null).
                Distinct().
                OrderBy(x => x.AbsoluteOffset).
                ToArray();

            foreach (var p in allGraphics)
            {
                var index = allGraphics.FindItemIndex(x => x == p) + 1;

                long length = 1;

                if (index < allGraphics.Length)
                    length = (allGraphics[index].AbsoluteOffset - p.AbsoluteOffset) / 4;

                str.AppendLine($"new AnimSetInfo(0x{p.AbsoluteOffset:X8}, {length}),");
            }

            str.ToString().CopyToClipboard();
        }

        // TODO: Support EU and JP versions
        public override AnimSetInfo[] AnimSetInfos => new AnimSetInfo[]
        {
            new AnimSetInfo(0x081D57F4, 39),
            new AnimSetInfo(0x081D5890, 76),
            new AnimSetInfo(0x081D59C0, 39),
            new AnimSetInfo(0x081D5A5C, 3),
            new AnimSetInfo(0x081D5A68, 5),
            new AnimSetInfo(0x081D5A7C, 3),
            new AnimSetInfo(0x081D5A88, 3),
            new AnimSetInfo(0x081D5A94, 3),
            new AnimSetInfo(0x081D5AA0, 3),
            new AnimSetInfo(0x081D5AAC, 3),
            new AnimSetInfo(0x081D5AB8, 3),
            new AnimSetInfo(0x081D5AC4, 2),
            new AnimSetInfo(0x081D5ACC, 1),
            new AnimSetInfo(0x081D5AD0, 3),
            new AnimSetInfo(0x081D5ADC, 2),
            new AnimSetInfo(0x081D5AE4, 3),
            new AnimSetInfo(0x081D5AF0, 1),
            new AnimSetInfo(0x081D5AF4, 3),
            new AnimSetInfo(0x081D5B00, 2),
            new AnimSetInfo(0x081D5B08, 2),
            new AnimSetInfo(0x081D5B10, 2),
            new AnimSetInfo(0x081D5B18, 1),
            new AnimSetInfo(0x081D5B1C, 2),
            new AnimSetInfo(0x081D5B24, 2),
            new AnimSetInfo(0x081D5B2C, 2),
            new AnimSetInfo(0x081D5B34, 4),
            new AnimSetInfo(0x081D5B44, 2),
            new AnimSetInfo(0x081D5B4C, 2),
            new AnimSetInfo(0x081D5B54, 2),
            new AnimSetInfo(0x081D5B5C, 5),
            new AnimSetInfo(0x081D5B70, 2),
            new AnimSetInfo(0x081D5B78, 2),
            new AnimSetInfo(0x081D5B80, 1),
            new AnimSetInfo(0x081D5B84, 3),
            new AnimSetInfo(0x081D5B90, 20),
            new AnimSetInfo(0x081D5BE0, 8),
            new AnimSetInfo(0x081D5C00, 2),
            new AnimSetInfo(0x081D5C08, 6),
            new AnimSetInfo(0x081D5C20, 3),
            new AnimSetInfo(0x081D5C2C, 5),
            new AnimSetInfo(0x081D5C40, 2),
            new AnimSetInfo(0x081D5C48, 2),
            new AnimSetInfo(0x081D5C50, 2),
            new AnimSetInfo(0x081D5C58, 1),
            new AnimSetInfo(0x081D5C5C, 15),
            new AnimSetInfo(0x081D5C98, 2),
            new AnimSetInfo(0x081D5CA0, 1),
            new AnimSetInfo(0x081D5CA4, 1),
            new AnimSetInfo(0x081D5CA8, 7),
            new AnimSetInfo(0x081D5CC4, 8),
            new AnimSetInfo(0x081D5CE4, 4),
            new AnimSetInfo(0x081D5CF4, 1),
            new AnimSetInfo(0x081D5CF8, 1),
            new AnimSetInfo(0x081D5CFC, 4),
            new AnimSetInfo(0x081D5D0C, 3),
            new AnimSetInfo(0x081D5D18, 9),
            new AnimSetInfo(0x081D5D3C, 1),
            new AnimSetInfo(0x081D5D40, 4),
            new AnimSetInfo(0x081D5D50, 1),
            new AnimSetInfo(0x081D5D54, 2),
            new AnimSetInfo(0x081D5D5C, 3),
            new AnimSetInfo(0x081D5D68, 2),
            new AnimSetInfo(0x081D5D70, 2),
            new AnimSetInfo(0x081D5D78, 2),
            new AnimSetInfo(0x081D5D80, 2),
            new AnimSetInfo(0x081D5D88, 3),
            new AnimSetInfo(0x081D5D94, 2),
            new AnimSetInfo(0x081D5D9C, 4),
            new AnimSetInfo(0x081D5DAC, 4),
            new AnimSetInfo(0x081D5DBC, 3),
            new AnimSetInfo(0x081D5DC8, 2),
            new AnimSetInfo(0x081D5DD0, 1),
            new AnimSetInfo(0x081D5DD4, 1),
            new AnimSetInfo(0x081D5DD8, 1),
            new AnimSetInfo(0x081D5DDC, 1),
            new AnimSetInfo(0x081D5DE0, 1),
            new AnimSetInfo(0x081D5DE4, 1),
            new AnimSetInfo(0x081D5DE8, 4),
            new AnimSetInfo(0x081D5DF8, 20),
            new AnimSetInfo(0x081D5E48, 4),
            new AnimSetInfo(0x081D5E58, 4),
            new AnimSetInfo(0x081D5E68, 1),
        };
    }
}