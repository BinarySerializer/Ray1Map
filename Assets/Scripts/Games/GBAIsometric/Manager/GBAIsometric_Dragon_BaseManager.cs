using BinarySerializer;
using BinarySerializer.GBA;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

namespace Ray1Map.GBAIsometric
{
    public abstract class GBAIsometric_Dragon_BaseManager : GBAIsometric_IceDragon_BaseManager
    {
        public override GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Resources", false, true, (input, output) => ExportResourcesAsync(settings, output, false, false)),
            new GameAction("Export Resources (categorized)", false, true, (input, output) => ExportResourcesAsync(settings, output, true, false)),
            new GameAction("Export Resources (Unused)", false, true, (input, output) => ExportResourcesAsync(settings, output, false, true)),
            new GameAction("Export Assets", false, true, (input, output) => ExportAssetsAsync(settings, output)),
            new GameAction("Export Cutscenes", false, true, (input, output) => ExportCutscenes(settings, output)),
            new GameAction("Export Font", false, true, (input, output) => ExportFont(settings, output)),
            new GameAction("Export Localization", false, true, (input, output) => ExportLocalization(settings, output)),
        };

        public async UniTask ExportResourcesAsync(GameSettings settings, string outputPath, bool categorize, bool ignoreUsedBlocks)
        {
            if (ignoreUsedBlocks)
                GBAIsometric_Spyro_LevelData.ForceSerializeAll = true;

            await DoGameActionAsync<GBAIsometric_Dragon_ROM>(settings, (rom, context) =>
            {
                ExportResources(context, rom.Resources, outputPath, categorize, ignoreUsedBlocks);
            });
        }

        public async UniTask ExportCutscenes(GameSettings settings, string outputPath)
        {
            using (var context = new Ray1MapContext(settings))
            {
                await LoadFilesAsync(context);

                var rom = FileFactory.Read<GBAIsometric_Dragon_ROM>(context, GetROMFilePath);

                var langIndex = 0;

                foreach (var lang in context.GetSettings<GBAIsometricSettings>().Languages)
                {
                    using (var w = new StreamWriter(Path.Combine(outputPath, $"Cutscenes_{lang}.txt")))
                    {
                        foreach (var d in rom.DialogEntries.OrderBy(x => x.ID))
                        {
                            var data = d.DialogData;

                            w.WriteLine($"# Cutscene {d.ID} (0x{d.Offset.StringAbsoluteOffset})");

                            foreach (var e in data.Entries)
                            {
                                switch (e.Values.First().Instruction)
                                {
                                    case GBAIsometric_Spyro_DialogData.Instruction.DrawPortrait:
                                        w.WriteLine($"[Draw portrait {e.PortraitIndex}]");
                                        break;

                                    case GBAIsometric_Spyro_DialogData.Instruction.DrawText:
                                    case GBAIsometric_Spyro_DialogData.Instruction.DrawMultiChoiceText:
                                        w.WriteLine($"{String.Join(" ", e.LocIndices.Select(x => x.GetString(langIndex)))}");
                                        break;

                                    case GBAIsometric_Spyro_DialogData.Instruction.MoveCamera:
                                        w.WriteLine("[Move camera]");
                                        break;
                                }

                                if (e.Values.First().Instruction == GBAIsometric_Spyro_DialogData.Instruction.DrawMultiChoiceText)
                                {
                                    w.WriteLine($"  > {e.MultiChoiceLocIndices[0].GetString(langIndex)} > {e.MultiChoiceLocIndices[2].GetString(langIndex)}");
                                    w.WriteLine($"  > {e.MultiChoiceLocIndices[1].GetString(langIndex)} > {e.MultiChoiceLocIndices[3].GetString(langIndex)}");
                                }
                            }

                            w.WriteLine();
                        }
                    }

                    if (rom.MenuPages != null)
                    {
                        using (var w = new StreamWriter(Path.Combine(outputPath, $"Menus_{lang}.txt")))
                        {
                            var menuIndex = 0;

                            foreach (var d in rom.MenuPages)
                            {
                                w.WriteLine($"# Menu {menuIndex} (0x{d.Offset.StringAbsoluteOffset})");

                                var header = d.Header.GetString(langIndex);
                                var subHeader = d.SubHeader.GetString(langIndex);

                                if (header != null)
                                    w.WriteLine($"{header}");
                                if (subHeader != null)
                                    w.WriteLine($"{subHeader}");

                                foreach (var o in d.Options ?? new GBAIsometric_Spyro_MenuOption[0])
                                    w.WriteLine($"> {o.LocIndex.GetString(langIndex)}");

                                w.WriteLine();

                                menuIndex++;
                            }
                        }
                    }

                    langIndex++;
                }
            }
        }

        public async UniTask ExportFont(GameSettings settings, string outputPath)
        {
            await DoGameActionAsync<GBAIsometric_Dragon_ROM>(settings, (rom, _) =>
            {
                ExportFont(rom.Localization, outputPath);
            });
        }

        public async UniTask ExportLocalization(GameSettings settings, string outputPath)
        {
            await DoGameActionAsync<GBAIsometric_Dragon_ROM>(settings, (rom, context) =>
            {
                JsonHelpers.SerializeToFile(LoadLocalization(context, rom.Localization), Path.Combine(outputPath, "Localization.json"));
            });
        }

        public async UniTask ExportAssetsAsync(GameSettings settings, string outputPath)
        {
            using (var context = new Ray1MapContext(settings))
            {
                await LoadFilesAsync(context);

                var rom = FileFactory.Read<GBAIsometric_Dragon_ROM>(context, GetROMFilePath);

                foreach (var portrait in rom.PortraitSprites ?? new GBAIsometric_Spyro_PortraitSprite[0])
                    Util.ByteArrayToFile(Path.Combine(outputPath, "Portraits", $"{portrait.ID}.png"), portrait.ToTexture2D().EncodeToPNG());

                var index = 0;
                foreach (var cutscene in rom.CutsceneMaps ?? new GBAIsometric_IceDragon_CutsceneMap[0])
                    Util.ByteArrayToFile(Path.Combine(outputPath, "Cutscenes", $"{index++}.png"), cutscene.ToTexture2D().EncodeToPNG());

                foreach (var map in rom.LevelMaps ?? new GBAIsometric_Spyro_LevelMap[0])
                    Util.ByteArrayToFile(Path.Combine(outputPath, "Maps", $"{map.LevelID}.png"), map.ToTexture2D().EncodeToPNG());

                var animSetPalettes = GetAnimSetPalettes(context, rom);

                // Export animation sets
                for (var i = 0; i < rom.AnimSets.Length; i++)
                {
                    var animSet = rom.AnimSets[i];
                    var outPath = Path.Combine(outputPath, "AnimSets", $"{i}");

                    await ExportAnimSetAsync(outPath, animSet, animSetPalettes[i]);
                }
            }
        }

        public async UniTask ExportAnimSetAsync(string outputPath, GBAIsometric_Spyro_AnimSet animSet, Color[][][] pal)
        {
            if (animSet?.AnimGroupsPointer == null)
                return;

            for (int a = 0; a < animSet.AnimBlock.Animations.Length; a++)
            {
                if (a % 10 == 0)
                    await Controller.WaitIfNecessary();

                var f = 0;

                var anim = animSet.AnimBlock.Animations[a];

                foreach (var tex in GetAnimationFrames(animSet, anim, pal[Mathf.Clamp(a, 0, pal.Length - 1)], isExport: true))
                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{a}-{anim.AnimSpeed}", $"{f++}.png"), tex.EncodeToPNG());
            }
        }

        private Unity_IsometricCollisionTile GetCollisionTile(Context context, GBAIsometric_TileCollision block)
        {
            Unity_IsometricCollisionTile.AdditionalTypeFlags GetAddType()
            {
                Unity_IsometricCollisionTile.AdditionalTypeFlags addType = Unity_IsometricCollisionTile.AdditionalTypeFlags.None;

                if (block.AddType_Spyro.HasFlag(GBAIsometric_TileCollision.AdditionalTypeFlags_Spyro.FenceUpLeft))
                    addType |= Unity_IsometricCollisionTile.AdditionalTypeFlags.FenceUpLeft;

                if (block.AddType_Spyro.HasFlag(GBAIsometric_TileCollision.AdditionalTypeFlags_Spyro.FenceUpRight))
                    addType |= Unity_IsometricCollisionTile.AdditionalTypeFlags.FenceUpRight;

                if (block.AddType_Spyro.HasFlag(GBAIsometric_TileCollision.AdditionalTypeFlags_Spyro.FenceDownLeft))
                    addType |= Unity_IsometricCollisionTile.AdditionalTypeFlags.FenceDownLeft;

                if (block.AddType_Spyro.HasFlag(GBAIsometric_TileCollision.AdditionalTypeFlags_Spyro.FenceDownRight))
                    addType |= Unity_IsometricCollisionTile.AdditionalTypeFlags.FenceDownRight;

                return addType;
            }
            Unity_IsometricCollisionTile.ShapeType GetShapeType()
            {
                switch (block.Shape_Spyro)
                {
                    case GBAIsometric_TileCollision.ShapeType_Spyro.None:
                        return Unity_IsometricCollisionTile.ShapeType.None;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.Normal:
                        return Unity_IsometricCollisionTile.ShapeType.Normal;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.SlopeUpLeft:
                        return Unity_IsometricCollisionTile.ShapeType.SlopeUpLeft;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.SlopeUpRight:
                        return Unity_IsometricCollisionTile.ShapeType.SlopeUpRight;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.LevelEdgeTop:
                        return Unity_IsometricCollisionTile.ShapeType.LevelEdgeTop;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.LevelEdgeBottom:
                        return Unity_IsometricCollisionTile.ShapeType.LevelEdgeBottom;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.LevelEdgeLeft:
                        return Unity_IsometricCollisionTile.ShapeType.LevelEdgeLeft;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.LevelEdgeRight:
                        return Unity_IsometricCollisionTile.ShapeType.LevelEdgeRight;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.OutOfBounds:
                        return Unity_IsometricCollisionTile.ShapeType.OutOfBounds;
                    default:
                        return Unity_IsometricCollisionTile.ShapeType.Unknown;
                }
            }
            Unity_IsometricCollisionTile.CollisionType GetCollisionType()
            {
                switch (block.Type_Spyro)
                {
                    case GBAIsometric_TileCollision.CollisionType_Spyro.Solid:
                        return Unity_IsometricCollisionTile.CollisionType.Solid;
                    case GBAIsometric_TileCollision.CollisionType_Spyro.Water:
                        return Unity_IsometricCollisionTile.CollisionType.Water;
                    case GBAIsometric_TileCollision.CollisionType_Spyro.FreezableWater:
                        return Unity_IsometricCollisionTile.CollisionType.FreezableWater;
                    case GBAIsometric_TileCollision.CollisionType_Spyro.Wall:
                        return Unity_IsometricCollisionTile.CollisionType.Wall;
                    case GBAIsometric_TileCollision.CollisionType_Spyro.Lava:
                        return Unity_IsometricCollisionTile.CollisionType.Lava;
                    case GBAIsometric_TileCollision.CollisionType_Spyro.Pit:
                        return Unity_IsometricCollisionTile.CollisionType.Pit;
                    case GBAIsometric_TileCollision.CollisionType_Spyro.HubworldPit:
                        return Unity_IsometricCollisionTile.CollisionType.HubworldPit;
                    case GBAIsometric_TileCollision.CollisionType_Spyro.Trigger:
                        return Unity_IsometricCollisionTile.CollisionType.Trigger;
                    default:
                        return Unity_IsometricCollisionTile.CollisionType.Unknown;
                }
            }
            return new Unity_IsometricCollisionTile()
            {
                Height = block.Height,
                AddType = GetAddType(),
                Shape = GetShapeType(),
                Type = GetCollisionType(),
                DebugText = Settings.ShowDebugInfo ? ($"Depth:{block.Depth} HeightFlags:{block.HeightFlags} UnkSpyro:{block.Unk_Spyro:X1} Shape:{block.Shape_Spyro} Type:{block.Type_Spyro} Add:{block.AddType_Spyro}") : null,
            };
        }

        public Unity_IsometricData GetIsometricData(Context context, GBAIsometric_Spyro_Collision3DMapData collisionData, int width, int height, int groupWidth, int groupHeight) {
            float tileDiagonal = 8 / 2f; // 8 Tiles, divide by 2 as 1 tile = half unit
            float tileWidth = Mathf.Sqrt(tileDiagonal * tileDiagonal / 2); // Side of square = sqrt(diagonal^2 / 2)
            return new Unity_IsometricData()
            {
                CollisionWidth = collisionData.Width,
                CollisionHeight = collisionData.Height,
                TilesWidth = width * groupWidth,
                TilesHeight = height * groupHeight,
                Collision = collisionData.Collision.Select(c => GetCollisionTile(context, c)).ToArray(),
                Scale = new Vector3(tileWidth, 1f/Mathf.Cos(Mathf.Deg2Rad * 30f), tileWidth) // Height = 1.15 tiles, Length of the diagonal of 1 block = 8 tiles
            };
        }

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            Controller.DetailedState = $"Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBAIsometric_Dragon_ROM>(context, GetROMFilePath);

            Func<ushort, Unity_MapCollisionTypeGraphic> collGraphicFunc = x => ((GBAIsometric_Spyro3_TileCollisionType2D)x).GetCollisionTypeGraphic();
            Func<ushort, string> collNameFunc = x => ((GBAIsometric_Spyro3_TileCollisionType2D)x).ToString();
            if (context.GetR1Settings().EngineVersion < EngineVersion.GBAIsometric_Spyro3) {
                collGraphicFunc = x => ((GBAIsometric_Spyro2_TileCollisionType2D)x).GetCollisionTypeGraphic();
                collNameFunc = x => ((GBAIsometric_Spyro2_TileCollisionType2D)x).ToString();
            }

            // Spyro 2 cutscenes
            if (context.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro2 && context.GetR1Settings().World == 4)
                return await LoadCutsceneMapAsync(context, rom);

            var levelData = rom.GetLevelData(context.GetR1Settings());

            // Convert map arrays to map tiles
            Dictionary<GBAIsometric_Spyro_MapLayer, MapTile[]> mapTiles = levelData.MapLayers.Where(x => x != null).ToDictionary(x => x, GetMapTiles);

            Controller.DetailedState = $"Loading tileset";
            await Controller.WaitIfNecessary();

            // Load tileset
            var tileSet = LoadTileSet(levelData.TilePalette, levelData.MapLayers.Where(x => x != null).Select(x => x.TileSet).ToArray(), mapTiles);

            Controller.DetailedState = $"Loading collision";
            await Controller.WaitIfNecessary();

            var firstValidMap = levelData.MapLayers.First(x => x != null);
            Unity_IsometricData isometricData = levelData.Collision3D == null ? null : GetIsometricData(context, levelData.Collision3D, firstValidMap.Map.Width, firstValidMap.Map.Height, firstValidMap.TileAssemble.GroupWidth, firstValidMap.TileAssemble.GroupHeight);

            Controller.DetailedState = $"Loading maps";
            await Controller.WaitIfNecessary();

            var maps = levelData.MapLayers.Select(x => x).Select((map, i) =>
            {
                if (map == null)
                    return null;

                var width = map.Map.Width * map.TileAssemble.GroupWidth;
                var height = map.Map.Height * map.TileAssemble.GroupHeight;

                return new Unity_Map() {
                    Type = Unity_Map.MapType.Graphics,
                    Width = (ushort)width,
                    Height = (ushort)height,
                    TileSet = new Unity_TileSet[]
                    {
                        tileSet
                    },
                    MapTiles = mapTiles[map].Select(x => new Unity_Tile(x)).ToArray(),
                };
            });

            if (context.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro2 && context.GetR1Settings().World == 1)
            {
                var reversedMaps = maps.Reverse().ToArray();
                reversedMaps.Last().Layer = Unity_Map.MapLayer.Front;
                maps = reversedMaps;
            }

            // Create a collision map for 2D levels
            if (levelData.Collision2D != null)
            {
                int width, height;
                if (context.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro3) {
                    width = levelData.Collision2D.Width / levelData.Collision2D.TileWidth;
                    height = levelData.Collision2D.Height / levelData.Collision2D.TileHeight;
                } else {
                    width = levelData.Collision2D.Width * 4;
                    height = levelData.Collision2D.Height;
                }
                maps = maps.Append(new Unity_Map() {
                    Type = Unity_Map.MapType.Collision,
                    Width = (ushort)(width),
                    Height = (ushort)(height),
                    TileSet = new Unity_TileSet[]
                    {
                        tileSet
                    },
                    MapTiles = GetCollision2DMapTiles(context, levelData.Collision2D).Select(x => new Unity_Tile(x)).ToArray(),
                });
            }

            // Add the map if available
            var lvlMap = rom.LevelMaps?.FirstOrDefault(x => x.LevelID == rom.GetLevelDataID(context.GetR1Settings()));
            if (context.GetR1Settings().World == 0 && lvlMap != null && Settings.LoadIsometricMapLayer)
            {
                maps = maps.Append(new Unity_Map() {
                    Type = Unity_Map.MapType.Graphics,
                    Width = lvlMap.Map.Width,
                    Height = lvlMap.Map.Height,
                    TileSet = new Unity_TileSet[]
                    {
                        LoadTileSet(lvlMap.Palette, lvlMap.TileSet, lvlMap.Map.MapData)
                    },
                    MapTiles = lvlMap.Map.MapData.Select(x => new Unity_Tile(x)).ToArray()
                });
            }

            var validMaps = maps.Where(x => x != null).ToArray();
            var objManager = new Unity_ObjectManager_GBAIsometricSpyro(context, rom.ObjectTypes, GetAnimSets(context, rom).ToArray());

            var objects = new List<Unity_SpriteObject>();

            // Load objects
            if (context.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro2 && context.GetR1Settings().World == 1)
            {
                var objTable = rom.LevelObjects_Spyro2_Agent9[context.GetR1Settings().Level];

                objects.AddRange(objTable.DoorObjects.Concat(objTable.CharacterObjects).Concat(objTable.CollectibleObjects).Select(x => new Unity_Object_GBAIsometricSpyro2_2D(x, objManager)));
            }
            else
            {
                var objTable = rom.GetObjectTable(context.GetR1Settings());

                // Init the objects
                if (objTable != null)
                {
                    InitObjects(objTable.Objects);
                    objects.AddRange(objTable.Objects.Select(x => new Unity_Object_GBAIsometricSpyro(x, objManager)));
                }
            }

            // Spyro 2: Snap object height to height of collision tile
            if (context.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro2 && isometricData != null) {
                foreach (var obj in objects.OfType<Unity_Object_GBAIsometricSpyro>().Where(x => !x.IsWaypoint)) {
                    var objX = obj.XPosition / 16f;
                    var objY = obj.YPosition / 16f;
                    if (objX < 0 || objX >= isometricData.CollisionWidth ||
                        objY < 0 || objY >= isometricData.CollisionHeight) continue;
                    var col = isometricData.Collision[Mathf.FloorToInt(objY) * isometricData.CollisionWidth + Mathf.FloorToInt(objX)];

                    ((Unity_Object_GBAIsometricSpyro)obj).Object.Height = (short)(col.Height * 16);
                }
            }

            Controller.DetailedState = $"Loading localization";
            await Controller.WaitIfNecessary();

            return new Unity_Level()
            {
                Maps = validMaps,
                ObjManager = objManager,
                EventData = objects,
                CellSize = CellSize,
                GetCollisionTypeNameFunc = collNameFunc,
                GetCollisionTypeGraphicFunc = collGraphicFunc,
                DefaultLayer = 1,
                IsometricData = isometricData,
                Localization = LoadLocalization(context, rom.Localization),
                DefaultCollisionLayer = validMaps.Length - 1,
                CellSizeOverrideCollision = context.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro3 ? (int?)16 : null,
            };
        }

        public IEnumerable<Unity_ObjectManager_GBAIsometricSpyro.AnimSet> GetAnimSets(Context context, GBAIsometric_Dragon_ROM rom)
        {
            var animSetPalettes = GetAnimSetPalettes(context, rom);

            var index = 0;

            // Add animation sets
            foreach (var animSet in rom.AnimSets)
            {
                var animSetIndex = index;

                yield return new Unity_ObjectManager_GBAIsometricSpyro.AnimSet(animSet, animSet.AnimBlock?.Animations.Select((x, i) =>
                {
                    return new Unity_ObjectManager_GBAIsometricSpyro.AnimSet.Animation(
                        animFrameFunc: () => GetAnimationFrames(animSet, x, animSetPalettes[animSetIndex][Mathf.Clamp(i, 0, animSetPalettes[animSetIndex].Length - 1)]).Select(f => f.CreateSprite()).ToArray(),
                        animSpeed: x.AnimSpeed,
                        positions: GetFramePositions(x));
                }).ToArray() ?? new Unity_ObjectManager_GBAIsometricSpyro.AnimSet.Animation[0]);

                index++;
            }
        }

        // [AnimSet][Anim][Pal][Color]
        public Color[][][][] GetAnimSetPalettes(Context context, GBAIsometric_Dragon_ROM rom)
        {
            var animSetPalettes = new Color[rom.AnimSets.Length][][][];

            if (context.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro3 || context.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Tron2)
            {
                var objPal = rom.GetLevelData(context.GetR1Settings()).ObjPalette;
                var pal = Util.ConvertAndSplitGBAPalette(objPal);

                for (int i = 0; i < animSetPalettes.Length; i++)
                    animSetPalettes[i] = new Color[][][]
                    {
                        pal
                    };
            }
            else
            {
                var cachedPalettes = new Dictionary<long, Color[][]>();
                var palInfo = context.GetR1Settings().GameModeSelection == GameModeSelection.SpyroSeasonFlameUS ? rom.Spyro2_PalInfoUS : rom.Spyro2_PalInfoEU;
                var commonPal = rom.Spyro2_CommonPalette;

                for (int i = 0; i < palInfo.Length; i++)
                {
                    var p = palInfo[i];

                    animSetPalettes[i] = p.BlockIndices?.Select((x, animIndex) =>
                    {
                        if (p.UsesCommonPalette)
                        {
                            if (!cachedPalettes.ContainsKey(x))
                            {
                                var newPal = (RGBA5551Color[])commonPal.Clone();
                                var pal = rom.Spyro2_AnimSetPalettes[i][animIndex];

                                for (int j = 0; j < 256; j++)
                                {
                                    if (pal[j].ColorValue != 0)
                                        newPal[j] = pal[j];
                                }

                                cachedPalettes[x] = Util.ConvertAndSplitGBAPalette(newPal);
                            }

                            return cachedPalettes[x];
                        }
                        else
                        {
                            return Util.ConvertAndSplitGBAPalette(rom.Spyro2_AnimSetPalettes[i][animIndex]);
                        }
                    }).ToArray() ?? new Color[][][] // Default to level obj palette if no dedicated palette is specified
                    {
                        Util.ConvertAndSplitGBAPalette(rom.GetLevelData(context.GetR1Settings()).ObjPalette)
                    };
                }
            }

            return animSetPalettes;
        }

        // Recreated from function at 0x08050200 (US rom for Spyro3)
        public void InitObjects(GBAIsometric_Object[] objects)
        {
            var readingWaypoints = false;
            var readingObjIndex = 0;
            int currentWayPointCount = 0;

            for (int index = 0; index < objects.Length; index++)
            {
                //var obj = objects[index];

                if (!readingWaypoints)
                {
                    // If the next object is a waypoint we read them until we reach the next normal object
                    if (index < objects.Length - 1 && !objects[index + 1].IsNormalObj)
                    {
                        // Save current object as it's the parent of the waypoints
                        readingObjIndex = index;

                        // Keep track of the amount of waypoints we read
                        currentWayPointCount = 0;

                        // Indicate that we're reading waypoints now
                        readingWaypoints = true;
                    }
                    else
                    {
                        // Init a normal object without waypoints
                        //FUN_0801534c(objects[index + 1].ObjectType, objects[index + 1].Value1, obj.XPosition);
                    }
                }
                else
                {
                    // Increment the waypoint count
                    currentWayPointCount += 1;

                    // If the next object is not a waypoint we stop reading them
                    if (index == objects.Length - 1 || objects[index + 1].IsNormalObj)
                    {
                        // Get the waypoints parent
                        var readingObj = objects[readingObjIndex];

                        // Init an object with waypoints
                        //FUN_08015408(readingObj.ObjectType, readingObj, currentWayPointCount);
                        
                        // Indicate that we're no longer reading waypoints
                        readingWaypoints = false;

                        // Save waypoint info
                        readingObj.WaypointIndex = (short)(readingObjIndex + 1);
                        readingObj.WaypointCount = (byte)currentWayPointCount;
                    }
                }

                //FUN_08003bbc();
            }
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
                                TileMapY = (ushort)(mt.TileMapY + (mapLayer.TileSet.Region * 512)),
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
        public MapTile[] GetCollision2DMapTiles(Context context, GBAIsometric_Spyro_Collision2DMapData collision2D)
        {

            if (context.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro2) {
                int width = collision2D.Width;
                int height = collision2D.Height;
                MapTile[] tiles = new MapTile[width * height * 4];

                for (int y = 0; y < collision2D.Height; y++) {
                    for (int x = 0; x < collision2D.Width; x++) {
                        int ind = y * width + x;
                        var c = collision2D.Collision[ind];
                        for (int i = 0; i < 4; i++) {
                            tiles[ind * 4 + i] = new MapTile() { CollisionType = (byte)BitHelpers.ExtractBits(c, 2, (4-i-1) * 2) };
                        }
                    }
                }
                return tiles;
            } else {
                return collision2D.Collision.Select(c => new MapTile() { CollisionType = c }).ToArray();
            }
        }

        public Unity_TileSet LoadTileSet(RGBA5551Color[] tilePal, GBAIsometric_Spyro_TileSet[] tileSets, Dictionary<GBAIsometric_Spyro_MapLayer, MapTile[]> mapTiles)
        {
            var palettes = Util.ConvertAndSplitGBAPalette(tilePal);

            const int tileSize = 32;
            const int regionSize = tileSize * 512;
            
            var tileSet = new byte[tileSets.Select(t => t.RegionOffset * tileSize + t.Region * regionSize + t.TileData.Length).Max()];

            // Fill regions with tile data
            foreach (var t in tileSets)
                Array.Copy(t.TileData, 0, tileSet, t.RegionOffset * tileSize + t.Region * regionSize, t.TileData.Length);

            int[] paletteIndices = new int[tileSet.Length];
            foreach (MapTile mt in mapTiles.SelectMany(mta => mta.Value))
                paletteIndices[mt.TileMapY] = mt.PaletteIndex;

            var tileSetTex = Util.ToTileSetTexture(tileSet, palettes.First(), Util.TileEncoding.Linear_4bpp, CellSize, false, getPalFunc: i => palettes[paletteIndices[i]]);

            return new Unity_TileSet(tileSetTex, CellSize);
        }

        public Vector2Int[] GetFramePositions(GBAIsometric_Spyro_Animation anim) {
            Vector2Int[] pos = new Vector2Int[anim.Frames.Length + (anim.PingPong ? (anim.Frames.Length - 2) : 0)];
            for (int i = 0; i < anim.Frames.Length; i++) {
                var f = anim.Frames[i];
                pos[i] = new Vector2Int(f.XPosition, f.YPosition);
            }
            if (anim.PingPong) {
                for (int i = anim.Frames.Length; i < pos.Length; i++) {
                    pos[i] = pos[pos.Length - i];
                }
            }
            return pos;
        }

        public Texture2D[] GetAnimationFrames(GBAIsometric_Spyro_AnimSet animSet, GBAIsometric_Spyro_Animation anim, Color[][] pal, bool isExport = false)
        {
            int minX1 = 0, minY1 = 0, maxX2 = int.MinValue, maxY2 = int.MinValue;
            if (isExport) {
                if (anim.Frames.Length > 0) {
                    var fs = anim.Frames.Where(f => !animSet.AnimFrameImages[f.FrameImageIndex].IsNullFrame);
                    minX1 = fs.Min(f => f.XPosition);
                    minY1 = fs.Min(f => f.YPosition);
                    maxX2 = fs.Max(f => f.XPosition + animSet.AnimFrameImages[f.FrameImageIndex].Width);
                    maxY2 = fs.Max(f => f.YPosition + animSet.AnimFrameImages[f.FrameImageIndex].Height);
                } else {
                    maxX2 = 0;
                    maxY2 = 0;
                }
            }
            Texture2D[] texs = new Texture2D[anim.Frames.Length + (anim.PingPong ? (anim.Frames.Length - 2) : 0)];
            for(int i = 0; i < anim.Frames.Length; i++) {
                var frame = anim.Frames[i];
                var frameImg = animSet.AnimFrameImages[frame.FrameImageIndex];
                int w, h;
                if (isExport) {
                    w = (maxX2 - minX1);
                    h = (maxY2 - minY1);
                } else {
                    w = frameImg.Width;
                    h = frameImg.Height;
                    //frameImg.GetActualSize(out w, out h);
                }
                /*var w = isExport ? (maxW - minX) : frameImg.Width;
                var h = isExport ? (maxH - minY) : frameImg.Height;*/
                Texture2D tex = TextureHelpers.CreateTexture2D(w, h, clear: true);
                int totalTileInd = 0;

                void addObjToFrame(byte spriteSize, GBAIsometric_Spyro_AnimPattern.Shape spriteShape, int xpos, int ypos, int relativeTile, int palIndex)
                {
                    // Get size
                    Util.GetGBASize((byte)spriteShape, spriteSize, out int width, out int height);

                    //var tileIndex = relativeTile;
                    var tileIndex = frameImg.TileIndex + totalTileInd;
                    totalTileInd += width * height;

                    // Clamp palette index
                    palIndex = Mathf.Clamp(palIndex, 0, pal.Length - 1);

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int actualX = (x * CellSize) + xpos + (isExport ? (frame.XPosition - minX1) : 0);
                            int actualY = (y * CellSize) + ypos + (isExport ? (frame.YPosition - minY1) : 0);

                            tex.FillInTile(animSet.TileSet, tileIndex * 32, pal[palIndex], Util.TileEncoding.Linear_4bpp, CellSize, true, actualX, actualY, ignoreTransparent: true);

                            tileIndex++;
                        }
                    }
                }

                if (!frameImg.HasPatterns)
                {
                    addObjToFrame(frameImg.SpriteSize, frameImg.SpriteShape, 0, 0, frameImg.TileIndex, frameImg.PalIndex);
                }
                else
                {
                    foreach (var pattern in frameImg.Patterns)
                        addObjToFrame(pattern.SpriteSize, pattern.SpriteShape, pattern.X, pattern.Y, pattern.RelativeTileIndex + frameImg.TileIndex, pattern.PalIndex);
                }

                tex.Apply();

                texs[i] = tex;
            }
            if (anim.PingPong) {
                for (int i = anim.Frames.Length; i < texs.Length; i++) {
                    texs[i] = texs[texs.Length - i];
                }
            }
            return texs;
        }

        public async UniTask CreateInitFuncUSToEUTableAsync(GameSettings usSettings, GameSettings euSettings)
        {
            using (var usContext = new Ray1MapContext(usSettings))
            {
                using (var euContext = new Ray1MapContext(euSettings))
                {
                    // Load rom files
                    await LoadFilesAsync(usContext);
                    await LoadFilesAsync(euContext);

                    // Read files
                    var usRom = FileFactory.Read<GBAIsometric_Dragon_ROM>(usContext, GetROMFilePath);
                    var euRom = FileFactory.Read<GBAIsometric_Dragon_ROM>(euContext, GetROMFilePath);

                    var outputStr = String.Empty;

                    var usedOffsets = new HashSet<long>();

                    for (int i = 0; i < usRom.ObjectTypes.Length; i++)
                    {
                        var usInit = usRom.ObjectTypes[i].Data?.InitFunctionPointer?.AbsoluteOffset;
                        var euInit = euRom.ObjectTypes[i].Data?.InitFunctionPointer?.AbsoluteOffset;

                        if (usInit == null || euInit == null)
                            continue;

                        if (usedOffsets.Contains(usInit.Value))
                            continue;
                        
                        usedOffsets.Add(usInit.Value);

                        outputStr += $"case 0x{euInit:X8}: return 0x{usInit:X8};{Environment.NewLine}";
                    }

                    outputStr.CopyToClipboard();
                }
            }
        }
        public async UniTask CreateAnimSetIndexUSToEUTableAsync(GameSettings usSettings, GameSettings euSettings)
        {
            using var usContext = new Ray1MapContext(usSettings);
            using var euContext = new Ray1MapContext(euSettings);
            
            // Load rom files
            await LoadFilesAsync(usContext);
            await LoadFilesAsync(euContext);

            // Read files
            var usRom = FileFactory.Read<GBAIsometric_Dragon_ROM>(usContext, GetROMFilePath);
            var euRom = FileFactory.Read<GBAIsometric_Dragon_ROM>(euContext, GetROMFilePath);

            // Calculate hash for every anim set
            var usHash = new string[usRom.AnimSets.Length];
            var euHash = new string[euRom.AnimSets.Length];

            calcHash(usHash, usRom.AnimSets);
            calcHash(euHash, euRom.AnimSets);

            void calcHash(IList<string> hash, IReadOnlyList<GBAIsometric_Spyro_AnimSet> animSets)
            {
                // Check the hash
                using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                {
                    var s = animSets.First().Context.Deserializer;

                    for (int i = 0; i < animSets.Count; i++)
                    {
                        var a = animSets[i];

                        byte[] data = null;
                        a.AnimBlockIndex.DoAt(size => data = s.SerializeArray<byte>(default, size));

                        hash[i] = Convert.ToBase64String(sha1.ComputeHash(data));
                    }
                }
            }

            var outputStr = String.Empty;

            for (int i = 0; i < euHash.Length; i++)
            {
                var matchingIndex = usHash.FindItemIndex(x => x == euHash[i]);

                if (matchingIndex == -1)
                    outputStr += $"case null: return {i};{Environment.NewLine}";
                else
                    outputStr += $"case {matchingIndex}: return {i};{Environment.NewLine}";
            }

            outputStr.CopyToClipboard();
        }
        public async UniTask CreateAnimSetPalBlockTableAsync(int animSetCount, GameSettings settings)
        {
            // Keep track of palettes
            var palTable = new int?[animSetCount];

            // Enumerate every level
            foreach (var world in GetLevels(settings).First().Worlds)
            {
                foreach (var lev in world.Maps)
                {
                    if (settings.EngineVersion == EngineVersion.GBAIsometric_Spyro2 && world.Index == 4)
                        continue;

                    var levSettings = new GameSettings(settings.GameModeSelection, settings.GameDirectory, world.Index, lev);

                    using (var context = new Ray1MapContext(levSettings))
                    {
                        await LoadFilesAsync(context);

                        // Load level
                        var level = await LoadAsync(context);

                        // Init the objects
                        level.ObjManager.InitObjects(level);

                        // Get the pal block index
                        var palBlockIndex = FileFactory.Read<GBAIsometric_Dragon_ROM>(context, GetROMFilePath).GetLevelData(levSettings).ObjPaletteIndex.Index;

                        // Enumerate every object
                        foreach (var obj in level.EventData.OfType<Unity_Object_GBAIsometricSpyro>().Where(x => x.AnimSetIndex != -1))
                            palTable[obj.AnimSetIndex] = palBlockIndex;
                        foreach (var obj in level.EventData.OfType<Unity_Object_GBAIsometricSpyro2_2D>().Where(x => x.AnimSetIndex != -1))
                            palTable[obj.AnimSetIndex] = palBlockIndex;
                    }

                    Debug.Log($"Finished level {world.Index}-{lev}");
                    await Controller.WaitIfNecessary();
                }
            }

            var output = String.Empty;

            for (int i = 0; i < palTable.Length; i++)
                output += $"new PalInfo({palTable[i]}), // AnimSet {i}{Environment.NewLine}";

            output.CopyToClipboard();
        }
        public async UniTask GetPalBlockIndexPerLevelAsync(GameSettings settings)
        {
            var output = String.Empty;

            using (var context = new Ray1MapContext(settings))
            {
                await LoadFilesAsync(context);

                // Load the rom
                var rom = FileFactory.Read<GBAIsometric_Dragon_ROM>(context, GetROMFilePath);

                // Enumerate every level
                foreach (var world in GetLevels(settings).First().Worlds)
                {
                    foreach (var lev in world.Maps)
                    {
                        if (settings.EngineVersion == EngineVersion.GBAIsometric_Spyro2 && world.Index == 4)
                            continue;

                        var levSettings = new GameSettings(settings.GameModeSelection, settings.GameDirectory, world.Index, lev);

                        output += $"{rom.GetLevelDataID(levSettings):00}: {rom.GetLevelData(levSettings).ObjPaletteIndex.Index}{Environment.NewLine}";
                    }
                }
            }

            output.CopyToClipboard();
        }
        public async UniTask CreatePalBlockIndexPerLevelUSToEUTableAsync(GameSettings usSettings, GameSettings euSettings)
        {
            using (var usContext = new Ray1MapContext(usSettings))
            {
                using (var euContext = new Ray1MapContext(euSettings))
                {
                    // Load rom files
                    await LoadFilesAsync(usContext);
                    await LoadFilesAsync(euContext);

                    // Read files
                    var usRom = FileFactory.Read<GBAIsometric_Dragon_ROM>(usContext, GetROMFilePath);
                    var euRom = FileFactory.Read<GBAIsometric_Dragon_ROM>(euContext, GetROMFilePath);

                    // Calculate hash for every table entry
                    var usHash = new string[usRom.Resources.DataEntries.Length];
                    var euHash = new string[euRom.Resources.DataEntries.Length];

                    calcHash(usHash, usRom.Resources);
                    calcHash(euHash, euRom.Resources);

                    void calcHash(IList<string> hash, GBAIsometric_IceDragon_Resources table)
                    {
                        // Check the hash
                        using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                        {
                            var s = table.Context.Deserializer;

                            for (int i = 0; i < table.DataEntries.Length; i++)
                            {
                                byte[] data = null;
                                table.DoAtResource(table.Context, i, size => data = s.SerializeArray<byte>(default, size));

                                hash[i] = Convert.ToBase64String(sha1.ComputeHash(data));
                            }
                        }
                    }

                    int lazyWayToGetUSAnimSetIndex(int euIndex)
                    {
                        for (int i = 0; i < usRom.AnimSets.Length; i++)
                        {
                            if (GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(euSettings.GameModeSelection, i) == euIndex)
                                return i;
                        }

                        return -1;
                    }

                    var outputStr = String.Empty;

                    for (int i = 0; i < euRom.AnimSets.Length; i++)
                    {
                        var usIndex = lazyWayToGetUSAnimSetIndex(i);

                        if (usIndex == -1)
                        {
                            outputStr += $"new PalInfo(null), // AnimSet {i}{Environment.NewLine}";
                        }
                        else
                        {
                            var usPalInfo = usRom.Spyro2_PalInfoUS[usIndex];

                            if (usPalInfo.BlockIndices == null)
                                outputStr += $"new PalInfo(), // AnimSet {i} ({usIndex}){Environment.NewLine}";
                            else
                                outputStr += $"new PalInfo({usPalInfo.UsesCommonPalette.ToString().ToLower()}, {String.Join(", ", usPalInfo.BlockIndices.Select(x => euHash.FindItemIndex(b => b == usHash[x])))}), // AnimSet {i} ({usIndex}){Environment.NewLine}";
                        }
                    }

                    outputStr.CopyToClipboard();
                }
            }
        }

    }
}