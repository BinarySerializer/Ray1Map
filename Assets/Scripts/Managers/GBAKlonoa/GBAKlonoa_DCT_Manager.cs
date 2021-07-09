using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;
using BinarySerializer.GBA;
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
            // Read the ROM
            var rom = FileFactory.Read<GBAKlonoa_DCT_ROM>(GetROMFilePath, context);
            
            // Get settings
            var settings = context.GetR1Settings();
            var normalLevelIndex = GetNormalLevelIndex(settings.World, settings.Level);
            var worldIndex = settings.World - 1;
            var isMap = settings.Level == 0;
            var isWaterSki = settings.Level == 4;
            var isUnderWater = settings.World == 4 && (settings.Level == 5 || settings.Level == 1 || settings.Level == 7);

            Controller.DetailedState = "Loading maps";
            await Controller.WaitIfNecessary();

            // Load maps
            var maps = LoadMaps(context, rom);

            // Create collision lines for the sectors
            var collisionLines = !isMap && !isWaterSki ? GetSectorCollisionLines(rom.MapSectors[normalLevelIndex]) : null;

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            var fixObjects = rom.FixObjects;
            var fixOam = rom.FixObjectOAMCollections;

            // Klonoa uses OAM 0x0D for these levels which is the first OAM from the level objects. This object is only used to allocate the palette
            // and copy the OAM.
            if (isWaterSki || isUnderWater)
            {
                var additionalOAMs = rom.GraphicsDatas[rom.LevelObjectCollection.Objects.First().DCT_GraphicsIndex].OAMs;

                // Fix the palette index
                foreach (GBAKlonoa_OAM o in additionalOAMs)
                    o.PaletteIndex = 0;

                fixOam = fixOam.Append(new GBAKlonoa_ObjectOAMCollection()
                {
                    OAMs = additionalOAMs
                }).ToArray();
            }

            GBAKlonoa_LoadedObject[][] levelObjects;

            if (isMap)
            {
                levelObjects = new GBAKlonoa_LoadedObject[][]
                {
                    rom.WorldMapObjectCollection.Objects.Select((x, index) => x.ToLoadedObject((short)(FixCount + index))).ToArray()
                };
            }
            else if (isWaterSki)
            {
                // Create an object for every position it can be in. The game reuses objects, but we duplicate them so they can all be shown at once.
                var cmds = rom.WaterSkiDatas[worldIndex];
                var objs = new List<GBAKlonoa_LoadedObject>();

                // Enumerate every command. We only care about the object commands.
                foreach (var cmd in cmds.Commands.Commands.Where(x => x.CmdType == GBAKlonoa_DCT_WaterSkiCommand.WaterSkiCommandType.Object_0 || x.CmdType == GBAKlonoa_DCT_WaterSkiCommand.WaterSkiCommandType.Object_1))
                {
                    var obj = rom.LevelObjectCollection.Objects[cmd.ObjIndex - FixCount].ToLoadedObject(cmd.ObjIndex, 0);

                    // We handle positions differently than the game
                    obj.XPos = (short)(cmd.ZPos + GBAConstants.ScreenWidth / 2);
                    obj.YPos = (short)-cmd.XPos;
                    obj.ZPos = (short)-cmd.YPos;

                    objs.Add(obj);
                }

                levelObjects = new GBAKlonoa_LoadedObject[][]
                {
                    objs.ToArray()
                };
            }
            else
            {
                // Get the objects for every sector
                levelObjects = Enumerable.Range(0, 5).Select(sector => rom.LevelObjectCollection.Objects.Select((obj, index) => obj.ToLoadedObject((short)(FixCount + index), sector)).ToArray()).ToArray();
            }

            var objmanager = new Unity_ObjectManager_GBAKlonoa(
                context: context,
                animSets: LoadAnimSets(context, rom, isMap, fixOam));

            var objects = new List<Unity_Object>();

            var hasStartPositions = !isMap && !isWaterSki;

            // Add Klonoa to each defined start position
            if (hasStartPositions)
            {
                var startInfos = new GBAKlonoa_LevelStartInfo[0].
                    Append(rom.LevelStartInfos[normalLevelIndex].StartInfo_Entry).
                    Concat(rom.LevelStartInfos[normalLevelIndex].StartInfos_Yellow).
                    Concat(rom.LevelStartInfos[normalLevelIndex].StartInfos_Green).
                    Append(rom.LevelStartInfos[normalLevelIndex].DCT_StartInfo_Unknown).
                    Where(x => !(x.XPos == 0 && x.YPos == 0));

                var klonoa = fixObjects[0];

                // Add Klonoa at each start position
                objects.AddRange(startInfos.Select(x => new Unity_Object_GBAKlonoa(objmanager, new GBAKlonoa_LoadedObject(
                        index: klonoa.Index, 
                        oamIndex: klonoa.OAMIndex, 
                        xPos: x.XPos, 
                        yPos: x.YPos, 
                        param_1: klonoa.Param_1, 
                        value6: klonoa.Value_6, 
                        param_2: klonoa.Param_2, 
                        value8: klonoa.Value_8, 
                        objType: klonoa.ObjType), x, fixOam[klonoa.OAMIndex])));
            }

            // Add fixed objects, except Klonoa (first one) and object 11/12 for maps since it's not used
            objects.AddRange(fixObjects.Skip(!hasStartPositions ? 0 : 1).Where(x => !isMap || (x.Index != 11 && x.Index != 12)).Select(x => new Unity_Object_GBAKlonoa(objmanager, x, null, fixOam[x.OAMIndex])));

            // Add level objects, duplicating them for each sector they appear in (if flags is 28 we assume it's unused in the sector)
            objects.AddRange(levelObjects.SelectMany(x => x).Where(x => isMap || isWaterSki || x.Value_8 != 31).Select(x => new Unity_Object_GBAKlonoa(
                objManager: objmanager, 
                obj: x, 
                serializable: (BinarySerializable)x.LevelObj ?? x.WorldMapObj, 
                oamCollection: isMap ? rom.WorldMapObjectOAMCollections[settings.World - 1][x.OAMIndex - FixCount] : null)));

            if (isMap)
                CorrectWorldMapObjectPositions(objects, maps.Last().Width, maps.Last().Height);

            Unity_TrackManager trackManager = null;

            if (isWaterSki)
            {
                const float x = GBAConstants.ScreenWidth / 2f;
                const float z = 20;

                var cmds = rom.WaterSkiDatas[worldIndex].Commands.Commands;

                trackManager = new Unity_TrackManager_Linear(
                    start: new Vector3(x, 0, z), 
                    end: new Vector3(x, -cmds.Where(c => c.CmdType == GBAKlonoa_DCT_WaterSkiCommand.WaterSkiCommandType.Object_0).Max(c => c.XPos) - 20, z));
            }

            return new Unity_Level(
                maps: maps,
                objManager: objmanager,
                eventData: objects,
                cellSize: CellSize,
                defaultLayer: 2,
                isometricData: isMap || isWaterSki ? Unity_IsometricData.Mode7(CellSize) : null,
                collisionLines: collisionLines,
                trackManager: trackManager);
        }

        public Unity_Map[] LoadMaps(Context context, GBAKlonoa_DCT_ROM rom)
        {
            var settings = context.GetR1Settings();
            var globalLevelIndex = GetGlobalLevelIndex(settings.World, settings.Level);
            var isMap = settings.Level == 0;
            var isWaterSki = settings.Level == 4;

            var tilePal = rom.Maps[globalLevelIndex].Palette.Take(0x1E0).Concat(rom.FixTilePalette).ToArray();
            var tileSet = CreateTileSet(rom.Maps[globalLevelIndex].MapLayers);

            var unityTileSet_4bit = new Unity_TileSet[1];

            var maps = rom.Maps[globalLevelIndex].MapLayers.Select((layer, mapIndex) =>
            {
                if (layer == null)
                    return null;

                if (!layer.Is8Bit)
                {
                    // The game hard-codes the base block for waterski levels
                    var baseBlock = isWaterSki && mapIndex == 0 ? 0 : layer.CNT.CharacterBaseBlock;

                    var tileOffset = baseBlock * 512;

                    // Correct map tiles
                    if (tileOffset != 0)
                    {
                        foreach (var tile in layer.Map)
                        {
                            tile.TileMapY = (ushort)(tile.TileMapY + tileOffset);
                        }
                    }
                }

                bool is3D = false;

                if (isMap && mapIndex == 2)
                    is3D = true;
                else if (isWaterSki && (mapIndex == 1 || mapIndex == 2))
                    is3D = true;

                return new
                {
                    Map = new Unity_Map
                    {
                        Width = layer.Width,
                        Height = layer.Height,
                        TileSet = layer.Is8Bit ? new Unity_TileSet[]
                        {
                            LoadTileSet(layer.TileSet, tilePal, true, null)
                        } : unityTileSet_4bit,
                        MapTiles = layer.Map.Select(x => new Unity_Tile(x)).ToArray(),
                        Type = Unity_Map.MapType.Graphics,
                        Settings3D = is3D ? Unity_Map.FreeCameraSettings.Mode7 : null,
                    },
                    CNT = layer.CNT
                };
            }).Where(x => x != null).Reverse().OrderBy(x => -x.CNT.Priority).Select(x => x.Map).ToArray();

            var mapGraphics = rom.MapGraphics[globalLevelIndex];

            if (mapGraphics.Any() && rom.Maps[globalLevelIndex].MapLayers.Any(x => x.Is8Bit))
                Debug.LogWarning($"There are map animations in a level with an 8-bit tileset! Currently this is not supported due to them using separate tilesets in Ray1Map.");

            unityTileSet_4bit[0] = LoadTileSet(
                tileSet: tileSet,
                pal: tilePal,
                is8bit: false,
                mapTiles_4: rom.Maps[globalLevelIndex].MapLayers.Where(x => x != null && !x.Is8Bit).SelectMany(x => x.Map).ToArray(),
                mapGraphics: mapGraphics);

            return maps;
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

        public Unity_ObjectManager_GBAKlonoa.AnimSet[] LoadAnimSets(Context context, GBAKlonoa_DCT_ROM rom, bool isMap, GBAKlonoa_ObjectOAMCollection[] fixOam)
        {
            var settings = context.GetR1Settings();
            var worldIndex = settings.World - 1;
            var isBoss = settings.Level == 9;
            var isEx = settings.Level == 10;
            var loadedAnimSets = new List<Unity_ObjectManager_GBAKlonoa.AnimSet>();

            GBAKlonoa_ObjectOAMCollection[] oam;

            if (isMap)
                oam = fixOam.Concat(rom.WorldMapObjectOAMCollections[worldIndex]).ToArray();
            else
                oam = fixOam;

            // Start by adding a null entry to use as default
            loadedAnimSets.Add(new Unity_ObjectManager_GBAKlonoa.AnimSet(new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation[0], $"NULL", new GBAKlonoa_OAM[0][]));

            // We need to allocate the palette like the game does to fix some animations not loaded from the graphics datas
            var pal = rom.FixObjectPalettes.ToList();

            var graphicsIndex = 0;

            // Load animations sets from the graphics datas
            if (!isMap)
            {
                /*
                // Start by filling out the palette. This is based on the available objects in the level.
                foreach (var obj in rom.LevelObjectCollection.Objects)
                {
                    var graphics = rom.GraphicsDatas[obj.DCT_GraphicsIndex];

                    var p = graphics.Palette;

                    if ((graphics.Flags1 & GBAKlonoa_DCT_GraphicsData.GraphicsFlags1.AllocatePalette) != 0 && !pal.Contains(p))
                        pal.Add(p);
                }*/

                // Load the graphics datas. The game only loads the ones used in the current level, but we load all of them.
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

            if (isMap)
            {
                var palPointers = FixedWorldMapPalettes.Concat(WorldMapPalettes[worldIndex]).Select(x => new Pointer(x, context.GetFile(GetROMFilePath)));
                var s = context.Deserializer;

                var index = 0;
                foreach (var p in palPointers)
                {
                    var palette = s.DoAt(p, () => s.SerializeObject<GBAKlonoa_ObjPal>(null, name: $"Palette[{index}]"));
                    pal.Add(palette);
                    index++;
                }
            }

            var fullObjPal = Util.ConvertAndSplitGBAPalette(pal.SelectMany(x => x.Colors).ToArray());

            // Load animations declared through the graphics data
            LoadAnimSets_ObjGraphics(loadedAnimSets, rom.FixObjectGraphics, oam, fullObjPal, rom.FixObjects);

            if (isMap)
            {
                var graphics = rom.WorldMapObjectGraphics[worldIndex];

                if (graphics != null)
                {
                    // Disable the cache here since different animations in the same set use different palettes
                    LoadAnimSets_ObjGraphics(loadedAnimSets, graphics, oam, fullObjPal, null, disableCache: true);
                }

                var allocationInfos = FixedWorldMapVRAMAllocationInfos.Concat(WorldMapVRAMAllocationInfos[worldIndex]).ToArray();

                LoadAnimSets_AllocatedTiles(loadedAnimSets, context, allocationInfos, rom.WorldMapObjectOAMCollections[worldIndex], fullObjPal);
            }

            // Load special (hard-coded) animations
            IEnumerable<SpecialAnimation> specialAnims = SpecialAnimations;

            // Add Klonoa's attack
            specialAnims = specialAnims.Concat(new SpecialAnimation[]
            {
                new SpecialAnimation(new long[]
                {
                    0x080757a8,
                    0x080a3500
                }, true, 9), // Klonoa's attack
                new SpecialAnimation(new long[]
                {
                    0x08075828,
                    0x080a3580,
                }, true, 10), // Klonoa's attack (small)
            });

            // Manually append the level text to the special animations (VISION/BOSS)
            if (isBoss)
            {
                // READY
                specialAnims = specialAnims.Append(new SpecialAnimation(0x080cf980, true, 11));

                // GO!!!
                specialAnims = specialAnims.Append(new SpecialAnimation(0x080d0180, true, 12));
            }
            else if (!isMap)
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
                    new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation(() => GetAnimFrames(frames, oams[0], fullObjPal).Select(x => x.CreateSprite()).ToArray(), oams[0])
                }, $"0x081d6700", oams);

                loadedAnimSets.Add(animSet);
            }

            LoadAnimSets_SpecialAnims(loadedAnimSets, context, specialAnims, fullObjPal, rom.FixObjects, rom.FixObjectOAMCollections, FixCount, rom.GraphicsDatas);

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
            new AnimSetInfo(0x081D5890, 76), // Actually multiple sets, but easiest to read as 1 since the game switches between them
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

            // World map
            new AnimSetInfo(0x081d5e70, 4),
            new AnimSetInfo(0x081d5e80, 4),
            new AnimSetInfo(0x081d5e90, 3),
            new AnimSetInfo(0x081d5e9c, 4),
            new AnimSetInfo(0x081d5eac, 5),
        };

        public SpecialAnimation[] SpecialAnimations => new SpecialAnimation[]
        {
            new SpecialAnimation(0x081d6670, 4, false, -1, dct_GraphisIndex: 41), // Green gem
            new SpecialAnimation(0x081d6680, 4, false, -1, dct_GraphisIndex: 42), // Blue gem
            new SpecialAnimation(0x081d6690, 4, false, -1, dct_GraphisIndex: 43), // Heart
            new SpecialAnimation(0x081d66a0, 4, false, -1, dct_GraphisIndex: 39), // Star
        };

        public MapVRAMAllocationInfo[] FixedWorldMapVRAMAllocationInfos => new MapVRAMAllocationInfo[]
        {
            new MapVRAMAllocationInfo(0x083be710, 0x200, isCompressed: true, tileIndex: 280),
            new MapVRAMAllocationInfo(null, 0x200),
            new MapVRAMAllocationInfo(null, 0x200),
            new MapVRAMAllocationInfo(null, 0x200),
            new MapVRAMAllocationInfo(null, 0x200),
            new MapVRAMAllocationInfo(null, 0x200),
            new MapVRAMAllocationInfo(null, 0x200),
            new MapVRAMAllocationInfo(null, 0x200),
        };

        public MapVRAMAllocationInfo[][] WorldMapVRAMAllocationInfos => new MapVRAMAllocationInfo[][]
        {
            // World 1
            new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x083bec08, 0x800, isCompressed: true),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x800),

                new MapVRAMAllocationInfo(0x083c214c, 0x200, isCompressed: true, tileIndex: 680, framesCount: 8),
            },

            // World 2
            new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x083bf63c, 0x800, isCompressed: true),
                new MapVRAMAllocationInfo(null, 0x800),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),

                new MapVRAMAllocationInfo(0x083c214c, 0x200, isCompressed: true, tileIndex: 728, framesCount: 8),
            },

            // World 3
            new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x083c03cc, 0x200, isCompressed: true),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x800),
                new MapVRAMAllocationInfo(null, 0x200),

                new MapVRAMAllocationInfo(0x083c214c, 0x200, isCompressed: true, tileIndex: 600, framesCount: 8),
            },

            // World 4
            new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x083c0d68, 0x200, isCompressed: true),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x800),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),

                new MapVRAMAllocationInfo(0x083c214c, 0x200, isCompressed: true, tileIndex: 616, framesCount: 8),
            },

            // World 5
            new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x083c1454, 0x200, isCompressed: true),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x800),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x800),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x200),
                new MapVRAMAllocationInfo(null, 0x1c0),
                new MapVRAMAllocationInfo(null, 0x1c0),
                new MapVRAMAllocationInfo(null, 0x1C0),

                new MapVRAMAllocationInfo(0x083c214c, 0x200, isCompressed: true, tileIndex: 754, framesCount: 8),
            },

            // World 6
            new MapVRAMAllocationInfo[]
            {

            },
        };

        public uint[] FixedWorldMapPalettes => new uint[]
        {
            0x0808e308,
            0x0808e328,
        };

        public uint[][] WorldMapPalettes => new uint[][]
        {
            // World 1
            new uint[]
            {
                0x0808e348,
                0x0808e368,
                0x0808e388,
                0x0808e3a8,
                0x0808e3c8
            },

            // World 2
            new uint[]
            {
                0x0808e3e8,
                0x0808e408,
                0x0808e428,
                0x0808e3c8
            },

            // World 3
            new uint[]
            {
                0x0808e448,
                0x0808e468,
                0x0808e488,
                0x0808e4a8,
            },

            // World 4
            new uint[]
            {
                0x0808e4c8,
                0x0808e4e8,
                0x0808e508,
                0x0808e528,
                0x0808e548
            },

            // World 5
            new uint[]
            {
                0x0808e568,
                0x0808e588,
                0x0808e5a8,
                0x0808e5c8,
                0x0808e5e8,
            },

            // World 6
            new uint[]
            {

            },
        };
    }
}