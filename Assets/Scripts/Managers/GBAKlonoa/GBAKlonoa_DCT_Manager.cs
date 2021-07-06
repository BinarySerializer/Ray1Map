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
            var isWaterSki = settings.Level == 4;

            //GenerateAnimSetTable(context, rom);

            Controller.DetailedState = "Loading maps";
            await Controller.WaitIfNecessary();

            var tilePal = rom.Maps[globalLevelIndex].Palette.Take(0x1E0).Concat(rom.FixTilePalette).ToArray();
            var tileSet = CreateTileSet(rom.Maps[globalLevelIndex].MapLayers);

            var unityTileSet_4bit = new Unity_TileSet[1];

            var maps = rom.Maps[globalLevelIndex].MapLayers.Select((layer, mapIndex) =>
            {
                if (layer == null)
                    return null;

                var tileOffset = 0;

                if (!layer.Is8Bit)
                {
                    // TODO: How does the game determine this?
                    if (isWaterSki)
                    {
                        if (mapIndex == 2)
                            tileOffset = 512;
                        else if (mapIndex == 3)
                            tileOffset = 512 * 2;
                    }
                    else
                    {
                        tileOffset = mapIndex * 512;
                    }

                    // Correct map tiles
                    if (tileOffset != 0)
                    {
                        foreach (var tile in layer.Map)
                        {
                            tile.TileMapY = (ushort)(tile.TileMapY + tileOffset);
                        }
                    }
                }

                return new
                {
                    Map = new Unity_Map
                    {
                        Width = layer.Width,
                        Height = layer.Height,
                        TileSet = layer.Is8Bit ? new Unity_TileSet[]
                        {
                            LoadTileSet(layer.TileSet, tilePal, true, null)
                        }: unityTileSet_4bit,
                        MapTiles = layer.Map.Select(x => new Unity_Tile(x)).ToArray(),
                        Type = Unity_Map.MapType.Graphics,
                        Settings3D = isMap && mapIndex == 2 ? Unity_Map.FreeCameraSettings.Mode7 : null,
                    },
                    CNT = layer.CNT
                };
            }).Where(x => x != null).OrderBy(x => -x.CNT.Priority).Select(x => x.Map).ToArray();

            unityTileSet_4bit[0] = LoadTileSet(tileSet, tilePal, false, rom.Maps[globalLevelIndex].MapLayers.Where(x => x != null && !x.Is8Bit).SelectMany(x => x.Map).ToArray());

            var collisionLines = !isMap && !isWaterSki ? GetSectorCollisionLines(rom.MapSectors[normalLevelIndex]) : null;

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            var fixObjects = rom.FixObjects;

            GBAKlonoa_LoadedObject[][] levelObjects;

            if (isMap)
            {
                levelObjects = new GBAKlonoa_LoadedObject[][]
                {
                    rom.WorldMapObjectCollection.Objects.Select((x, index) => x.ToLoadedObject((short)(FixCount + index))).ToArray()
                };
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
                animSets: LoadAnimSets(context, rom, isMap));

            var objects = new List<Unity_Object>();

            var hasStartPositions = !isMap && !isWaterSki;

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
            objects.AddRange(levelObjects.SelectMany(x => x).Where(x => isMap || x.Value_8 != 31).Select(x => new Unity_Object_GBAKlonoa(
                objManager: objmanager, 
                obj: x, 
                serializable: (BinarySerializable)x.LevelObj ?? x.WorldMapObj, 
                oamCollection: isMap ? rom.WorldMapObjectOAMCollections[settings.World - 1][x.OAMIndex - FixCount] : null)));

            if (isMap)
                CorrectWorldMapObjectPositions(objects, maps.Last().Width, maps.Last().Height);

            return new Unity_Level(
                maps: maps,
                objManager: objmanager,
                eventData: objects,
                cellSize: CellSize,
                defaultLayer: 2,
                isometricData: isMap ? Unity_IsometricData.Mode7(CellSize) : null,
                collisionLines: collisionLines);
        }

        public byte[] CreateTileSet(GBAKlonoa_DCT_MapLayer[] layers)
        {
            var tileSet = new byte[4 * 512 * 32]; // 4 regions, 512 tiles each

            foreach (GBAKlonoa_DCT_MapLayer layer in layers.Where(x => x != null && !x.Is8Bit))
            {
                var baseOffset = layer.CNT.CharacterBaseBlock * 512 * 32;
                Array.Copy(layer.TileSet, 0, tileSet, baseOffset, layer.TileSet.Length);
            }

            return tileSet;
        }

        public Unity_ObjectManager_GBAKlonoa.AnimSet[] LoadAnimSets(Context context, GBAKlonoa_DCT_ROM rom, bool isMap)
        {
            var settings = context.GetR1Settings();
            var isBoss = settings.Level == 9;
            var isEx = settings.Level == 10;
            var loadedAnimSets = new List<Unity_ObjectManager_GBAKlonoa.AnimSet>();

            // Start by adding a null entry to use as default
            loadedAnimSets.Add(new Unity_ObjectManager_GBAKlonoa.AnimSet(new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation[0], $"NULL", new GBAKlonoa_OAM[0][]));

            var fixPal = Util.ConvertAndSplitGBAPalette(rom.FixObjectPalettes.SelectMany(x => x.Colors).ToArray());

            // Load animations declared through the graphics data
            LoadAnimSets_ObjGraphics(loadedAnimSets, rom.FixObjectGraphics, rom.FixObjectOAMCollections, fixPal, rom.FixObjects);

            var graphicsIndex = 0;

            // Load animations sets from the graphics datas
            if (!isMap)
            {
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
            }

            // Load special (hard-coded) animations
            IEnumerable<SpecialAnimation> specialAnims = new SpecialAnimation[0];

            // Manually append the level text to the special animations (VISION/BOSS)
            if (isBoss)
            {
                // READY
                specialAnims = specialAnims.Append(new SpecialAnimation(0x080cf980, true, 11));

                // GO!!!
                specialAnims = specialAnims.Append(new SpecialAnimation(0x080d0180, true, 12));
            }
            else
            {
                // VISION
                specialAnims = specialAnims.Append(new SpecialAnimation(0x080758a8, true, 11));

                byte[] levNumSprite = new byte[16 * 4 * 0x20]; // 16x4 tiles

                // Re-implemented from 0x080318a4
                void fillSprite(Pointer spritePointer, int offsetX, int width)
                {
                    var s = context.Deserializer;

                    s.DoAt(spritePointer, () =>
                    {
                        for (int y = 0; y < 4; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                var actualX = offsetX + x;

                                var bytes = s.SerializeArray<byte>(null, 0x20, name: $"Tile[{y}][{x}");

                                if (actualX < 8)
                                {
                                    Array.Copy(bytes, 0, levNumSprite, (actualX + y * 8) * 0x20, 0x20);
                                }
                                else
                                {
                                    Array.Copy(bytes, 0, levNumSprite, ((actualX - 8) + 32 + y * 8) * 0x20, 0x20);
                                }
                            }
                        }
                    });
                }

                if (isEx)
                {
                    fillSprite(new Pointer(0x080d0980, rom.Offset.File), 2, 8); // EX
                    fillSprite(rom.LevelNumSpritePointers[settings.World - 1], 10, 4); // World
                }
                else
                {
                    fillSprite(rom.LevelNumSpritePointers[settings.World - 1], 2, 4); // World
                    fillSprite(new Pointer(0x080cf780, rom.Offset.File), 6, 4); // -
                    fillSprite(rom.LevelNumSpritePointers[settings.Level - 1], 10, 4); // Level
                }

                // Manually load the filled sprite
                var frames = new GBAKlonoa_AnimationFrame[]
                {
                    new GBAKlonoa_AnimationFrame()
                    {
                        ImgData = levNumSprite
                    }
                };

                var oams = new GBAKlonoa_OAM[][]
                {
                    rom.FixObjectOAMCollections[12].OAMs
                };

                var animSet = new Unity_ObjectManager_GBAKlonoa.AnimSet(new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation[]
                {
                    new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation(() => GetAnimFrames(frames, oams[0], fixPal).Select(x => x.CreateSprite()).ToArray(), oams[0])
                }, $"0x081d6700", oams);

                loadedAnimSets.Add(animSet);
            }

            LoadAnimSets_SpecialAnims(loadedAnimSets, context, specialAnims, fixPal, rom.FixObjects, rom.FixObjectOAMCollections, FixCount);

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