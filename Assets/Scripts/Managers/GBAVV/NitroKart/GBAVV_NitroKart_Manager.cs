using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public abstract class GBAVV_NitroKart_Manager : GBAVV_BaseManager
    {
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 26).ToArray()), 
        });

        public override string[] Languages => new string[]
        {
            "Dutch",
            "English",
            "French",
            "German",
            "Italian",
            "Spanish",
        };

        public override GBAVV_BaseROM LoadROMForExport(Context context) => FileFactory.Read<GBAVV_ROM_NitroKart>(GetROMFilePath, context);
        public override async UniTask ExportCutscenesAsync(GameSettings settings, string outputDir)
        {
            using (var context = new R1Context(settings))
            {
                await LoadFilesAsync(context);

                // Read the rom
                var rom = FileFactory.Read<GBAVV_ROM_NitroKart>(GetROMFilePath, context, (s, d) => d.SerializeFLC = true);

                ExportCutscenesFromScripts(rom.GetAllScripts, outputDir);
            }

            Debug.Log($"Finished export");
        }

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            //FindDataInROM(context.Deserializer, context.FilePointer(GetROMFilePath));
            //FindObjTypeData(context);

            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBAVV_ROM_NitroKart>(GetROMFilePath, context);

            return await LoadNitroKartAsync(context, rom);
        }

        public async UniTask<Unity_Level> LoadNitroKartAsync(Context context, GBAVV_ROM_NitroKart rom)
        {
            var map = rom.CurrentLevelInfo.MapData;

            Controller.DetailedState = "Loading tilesets";
            await Controller.WaitIfNecessary();

            // Set the collision type for the tiles
            foreach (var t in map.Mode7MapLayer.MapTiles)
                t.CollisionType = (ushort)map.Mode7TileSetCollision[t.TileMapY];

            var tilePalettes = map.AdditionalTilePalettesCount > 0 ? map.AdditionalTilePalettes : new RGBA5551Color[][]
            {
                map.TilePalette
            };

            var mode7TileSets = tilePalettes.Select(p => LoadTileSet(map.Mode7TileSet, p, true, context.GetR1Settings().EngineVersion, 0, null)).ToArray();
            var bgTileSets = tilePalettes.Select(p => LoadTileSet(map.BackgroundTileSet.TileSet, p, false, context.GetR1Settings().EngineVersion, 0, map.BackgroundMapLayers.SelectMany(x => x.TileMap.MapTiles).ToArray(), map.BackgroundTileAnimations)).ToArray();

            Controller.DetailedState = "Loading maps";
            await Controller.WaitIfNecessary();

            Unity_Map getMap(GBAVV_NitroKart_BackgroundMapLayer m)
            {
                var width = (ushort)(m.Width / CellSize);
                var height = (ushort)(m.Height / CellSize);

                var tileMap = new Unity_Tile[width * height];

                const int screenBlockWidth = 32;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int screenblock = (x / screenBlockWidth) * 1024;
                        tileMap[y * width + x] = new Unity_Tile(m.TileMap.MapTiles[y * screenBlockWidth + (x % screenBlockWidth) + screenblock]);
                    }
                }

                return new Unity_Map
                {
                    Width = width,
                    Height = height,
                    TileSet = bgTileSets,
                    MapTiles = tileMap,
                    Type = Unity_Map.MapType.Graphics,
                    Layer = Unity_Map.MapLayer.Middle,
                    /*Settings3D = new Unity_Map.FreeCameraSettings() {
                        Mode = Unity_Map.FreeCameraSettings.Mode3D.Billboard,
                        Position = Vector3.forward * 50f,
                        Scale = new Vector3(1,-1,1)
                    }*/
                };
            }

            var maps = new Unity_Map[]
            {
                new Unity_Map
                {
                    Width = (ushort)(map.Mode7MapLayer.TileMap.Width * 2),
                    Height = (ushort)(map.Mode7MapLayer.TileMap.Height * 2),
                    TileSet = mode7TileSets,
                    MapTiles = GetTileMap(map.Mode7MapLayer.TileMap, map.Mode7MapLayer.MapTiles),
                    Type = Unity_Map.MapType.Graphics | Unity_Map.MapType.Collision,
                    Layer = Unity_Map.MapLayer.Middle,
                    Settings3D = Unity_Map.FreeCameraSettings.Mode7
                },
                getMap(map.BackgroundMapLayers[2]),
                getMap(map.BackgroundMapLayers[1]),
                getMap(map.BackgroundMapLayers[0]),
            };

            Controller.DetailedState = "Loading localization";
            await Controller.WaitIfNecessary();

            var loc = LoadLocalization(rom);

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            Unity_ObjectManager_GBAVV objManager = new Unity_ObjectManager_GBAVV(
                context: context, 
                animSets: LoadAnimSets(rom), 
                graphics: rom.Map2D_Graphics, 
                nitroKart_ObjTypeData: rom.ObjTypeData, 
                scripts: rom.GetAllScripts.ToArray(), 
                locPointerTable: loc.Item2);

            var objGroups = new List<(GBAVV_NitroKart_Object[], string)>();

            objGroups.Add((map.Objects.Objects_Normal, "Normal"));

            if (map.Objects.ObjectsPointer_TimeTrial != map.Objects.ObjectsPointer_Normal)
                objGroups.Add((map.Objects.Objects_TimeTrial, "Time Trial"));

            if (map.Objects.ObjectsPointer_BossRace != map.Objects.ObjectsPointer_Normal)
                objGroups.Add((map.Objects.Objects_BossRace, "Boss Race"));

            var objects = objGroups.SelectMany((x, i) => x.Item1.Select(o => (Unity_SpriteObject)new Unity_Object_GBAVVNitroKart(objManager, o, i))).ToList();

            var waypointsGroupIndex = 0;

            void addTrackWaypoints(IReadOnlyList<GBAVV_NitroKart_TrackWaypoint> waypoints, string groupName, int trackDataIndex)
            {
                if (waypoints == null)
                    return;

                if (objGroups.Any(x => x.Item2 == groupName))
                {

                    var objCount = objects.Count;
                    for (int i = 0; i < waypoints.Count; i++)
                    {
                        var w = new Unity_Object_GBAVVNitroKartWaypoint(objManager, waypoints[i], waypointsGroupIndex, trackDataIndex);
                        w.LinkedWayPointIndex = objCount + ((i == waypoints.Count - 1) ? 0 : (i + 1));
                        objects.Add(w);
                    }
                    //objects.AddRange(waypoints.Select(w => new Unity_Object_GBAVVNitroKartWaypoint(objManager, w, waypointsGroupIndex, trackDataIndex)));
                    waypointsGroupIndex++;
                }
            }

            addTrackWaypoints(map.TrackData1.TrackWaypoints_Normal, "Normal", 0);
            addTrackWaypoints(map.TrackData1.TrackWaypoints_TimeTrial, "Time Trial", 0);
            addTrackWaypoints(map.TrackData1.TrackWaypoints_BossRace, "Boss Race", 0);
            waypointsGroupIndex = 0;
            addTrackWaypoints(map.TrackData2.TrackWaypoints_Normal, "Normal", 1);
            addTrackWaypoints(map.TrackData2.TrackWaypoints_TimeTrial, "Time Trial", 1);
            addTrackWaypoints(map.TrackData2.TrackWaypoints_BossRace, "Boss Race", 1);

            return new Unity_Level(
                maps: maps,
                objManager: objManager,
                eventData: objects,
                cellSize: CellSize,
                objectGroups: objGroups.Select(x => x.Item2).ToArray(),
                getCollisionTypeGraphicFunc: x => ((GBAVV_NitroKart_CollisionType)x).GetCollisionTypeGraphic(),
                getCollisionTypeNameFunc: x => ((GBAVV_NitroKart_CollisionType)x).ToString(),
                localization: loc.Item1,
                isometricData: Unity_IsometricData.Mode7(CellSize),
                trackManager: new Unity_TrackManager_GBAVV_NitroKart());
        }

        public virtual void FindObjTypeData(Context context)
        {
            var rom = FileFactory.Read<GBAVV_ROM_NitroKart>(GetROMFilePath, context);
            var s = context.Deserializer;

            var str = new StringBuilder();

            var initFunctionPointers = s.DoAt(new Pointer(ObjTypesPointer, s.Context.GetFile(GetROMFilePath)), () => s.SerializePointerArray(default, ObjTypesCount));
            var orderedPointers = initFunctionPointers.OrderBy(x => x.AbsoluteOffset).Distinct().ToArray();

            // Enumerate every obj init function
            for (int i = 0; i < initFunctionPointers.Length; i++)
            {
                var nextPointer = orderedPointers.ElementAtOrDefault(orderedPointers.FindItemIndex(x => x == initFunctionPointers[i]) + 1);

                s.DoAt(initFunctionPointers[i], () =>
                {
                    s.Align();

                    var foundPointer = false;

                    // Try and read every int as a pointer until we get a valid one 25 times
                    for (int j = 0; j < 25; j++)
                    {
                        if (nextPointer != null && s.CurrentPointer.AbsoluteOffset >= nextPointer.AbsoluteOffset)
                            break;

                        var p = s.SerializePointer(default);

                        s.DoAt(p, () =>
                        {
                            s.Goto(s.CurrentPointer + 20);
                            var graphicsPointer = s.SerializePointer(default);

                            if (rom.Map2D_Graphics.Any(x => x.Offset == graphicsPointer))
                            {
                                str.AppendLine($"0x{p.StringAbsoluteOffset}, // {i}");
                                foundPointer = true;
                            }
                        });

                        if (foundPointer)
                            return;
                    }

                    // No pointer found...
                    str.AppendLine($"null, // {i}");
                });
            }

            str.ToString().CopyToClipboard();
        }
		public override int ObjTypesCount => 114;
        public abstract uint?[] ObjTypesDataPointers { get; }

        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands => new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [0700] = GBAVV_ScriptCommand.CommandType.FLC,

            [0902] = GBAVV_ScriptCommand.CommandType.Script,
            [0907] = GBAVV_ScriptCommand.CommandType.Name,
            [0911] = GBAVV_ScriptCommand.CommandType.Return,
            
            [1000] = GBAVV_ScriptCommand.CommandType.Credits,
            [1102] = GBAVV_ScriptCommand.CommandType.Dialog,
        };
    }
    public class GBAVV_NitroKartUS_Manager : GBAVV_NitroKart_Manager
    {
        public override uint ObjTypesPointer => 0x080080d0;
        public override uint?[] ObjTypesDataPointers => new uint?[]
        {
            0x08086D94, // 0
            0x08086D94, // 1
            0x08086D94, // 2
            null, // 3
            null, // 4
            null, // 5
            null, // 6
            null, // 7
            null, // 8
            null, // 9
            null, // 10
            null, // 11
            null, // 12
            0x08086D94, // 13
            null, // 14
            null, // 15
            0x08086D94, // 16
            0x08086D94, // 17
            0x08086D94, // 18
            0x08086D94, // 19
            0x08086D94, // 20
            0x08086D94, // 21
            0x08086D94, // 22
            0x08086D94, // 23
            0x08086D94, // 24
            0x08086D94, // 25
            0x08086D94, // 26
            0x08086D94, // 27
            0x08086D94, // 28
            0x08086D94, // 29
            0x08086D94, // 30
            0x080867D8, // 31
            0x0808680C, // 32
            0x08086840, // 33
            0x08033A94, // 34
            0x08033AD8, // 35
            0x08033B1C, // 36
            0x08033A50, // 37
            0x080338FC, // 38
            0x08033940, // 39
            0x08033B60, // 40
            0x08033984, // 41
            0x080339C8, // 42
            0x08033A0C, // 43
            0x080608A4, // 44
            0x080602F4, // 45
            0x0806035C, // 46
            0x08060738, // 47
            0x08060328, // 48
            0x08060704, // 49
            0x08060390, // 50
            0x080603C4, // 51
            0x080603F8, // 52
            0x0806042C, // 53
            0x08060460, // 54
            0x08060494, // 55
            0x080604C8, // 56
            0x0806069C, // 57
            0x080606D0, // 58
            0x080604FC, // 59
            0x0806076C, // 60
            0x080607A0, // 61
            0x080607D4, // 62
            0x08060808, // 63
            0x08060530, // 64
            0x0806083C, // 65
            0x08060870, // 66
            0x08060564, // 67
            0x08060598, // 68
            0x08060634, // 69
            0x08060668, // 70
            0x080605CC, // 71
            0x08060600, // 72
            0x08060940, // 73
            0x08060974, // 74
            0x080609A8, // 75
            0x080609E8, // 76
            0x08033BE4, // 77
            0x08033C18, // 78
            0x08086D94, // 79
            0x08086D94, // 80
            0x08086D94, // 81
            0x08086D94, // 82
            0x08086D94, // 83
            0x08086D94, // 84
            0x08086D94, // 85
            0x080868B4, // 86
            0x080868E8, // 87
            0x0808691C, // 88
            0x08086950, // 89
            0x08086984, // 90
            0x080869B8, // 91
            0x080869EC, // 92
            0x08086A20, // 93
            0x08086A54, // 94
            0x08086A88, // 95
            0x08086ABC, // 96
            0x08086AF0, // 97
            0x08086B24, // 98
            0x08086B58, // 99
            0x08086880, // 100
            0x08086B8C, // 101
            0x08086BC0, // 102
            0x08086C90, // 103
            0x08086C5C, // 104
            0x08086CC4, // 105
            0x08086CF8, // 106
            0x08086D2C, // 107
            0x08086D60, // 108
            null, // 109
            0x08086D94, // 110
            0x080608D8, // 111
            0x0806090C, // 112
            0x08086D94, // 113
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0808709C,
            0x08089A80,
            0x0808D3DC,
            0x0808F464,
            0x0808F4FC,
            0x0808FC88,
            0x08090428,
            0x08090B90,
            0x08090C78,
            0x08091404,
            0x08091874,
            0x08092014,
            0x08093DA4,
            0x08094510,
            0x08094D08,
            0x0809588C,
            0x08096018,
            0x080967A8,
            0x08096F64,
            0x0809770C,
            0x08098570,
            0x0809CE8C,
            0x0809D600,
            0x0809DD8C,
            0x080A05AC,
            0x080A0C9C,
            0x080A143C,
            0x080A14F4
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08032FBC, // script_waitForInputOrTime
            0x0805CA50, // movie_intro
            0x0805CBEC, // movie_garage
            0x0805CC48, // movie_credits
            0x0805CCDC, // movie_gameIntro
            0x0805D240, // movie_earthBossIntro
            0x0805D3D8, // movie_earthBossCrashWin
            0x0805D624, // movie_earthBossEvilWin
            0x0805D810, // movie_barinBossIntro
            0x0805D9A8, // movie_barinBossCrashWin
            0x0805DB94, // movie_barinBossEvilWin
            0x0805DDE0, // movie_fenomBossIntro
            0x0805DF9C, // movie_fenomBossCrashWin
            0x0805E1B8, // movie_fenomBossEvilWin
            0x0805E3F8, // movie_tekneeBossIntro
            0x0805E570, // movie_tekneeBossCrashWin
            0x0805E6A8, // movie_tekneeBossEvilWin
            0x0805E7DC, // movie_veloBossIntro
            0x0805E89C, // movie_veloBossCrashWin
            0x0805EBFC, // movie_veloBossEvilWin
            0x0805EDE4, // movie_findFakeCrash
            0x0805EF2C, // SCRIPT_pagedTextLoop
            0x08064F1C, // script_license
            0x08064F5C, // script_intro
            0x08064F9C, // script_credits
            0x08064FE4, // script_findFakeCrash
            0x0806502C, // script_earthBossIntro
            0x08065078, // script_earthBossCrashWin
            0x080650C0, // script_earthBossEvilWin
            0x08065108, // script_barinBossIntro
            0x08065154, // script_barinBossCrashWin
            0x0806519C, // script_barinBossEvilWin
            0x080651E4, // script_fenomBossIntro
            0x08065230, // script_fenomBossCrashWin
            0x08065278, // script_fenomBossEvilWin
            0x080652C0, // script_tekneeBossIntro
            0x0806530C, // script_tekneeBossCrashWin
            0x08065358, // script_tekneeBossEvilWin
            0x080653A0, // script_veloBossIntro
            0x080653E8, // script_veloBossCrashWin
            0x08065430, // script_veloBossEvilWin
        };
    }
    public class GBAVV_NitroKartEU_Manager : GBAVV_NitroKart_Manager
    {
        public override uint ObjTypesPointer => 0x0800812c;
        public override uint?[] ObjTypesDataPointers => new uint?[]
        {
            0x0809DAF0, // 0
            0x0809DAF0, // 1
            0x0809DAF0, // 2
            null, // 3
            null, // 4
            null, // 5
            null, // 6
            null, // 7
            null, // 8
            null, // 9
            null, // 10
            null, // 11
            null, // 12
            0x0809DAF0, // 13
            null, // 14
            null, // 15
            0x0809DAF0, // 16
            0x0809DAF0, // 17
            0x0809DAF0, // 18
            0x0809DAF0, // 19
            0x0809DAF0, // 20
            0x0809DAF0, // 21
            0x0809DAF0, // 22
            0x0809DAF0, // 23
            0x0809DAF0, // 24
            0x0809DAF0, // 25
            0x0809DAF0, // 26
            0x0809DAF0, // 27
            0x0809DAF0, // 28
            0x0809DAF0, // 29
            0x0809DAF0, // 30
            0x0809D534, // 31
            0x0809D568, // 32
            0x0809D59C, // 33
            0x08035DFC, // 34
            0x08035E40, // 35
            0x08035E84, // 36
            0x08035DB8, // 37
            0x08035C64, // 38
            0x08035CA8, // 39
            0x08035EC8, // 40
            0x08035CEC, // 41
            0x08035D30, // 42
            0x08035D74, // 43
            0x08077600, // 44
            0x08077050, // 45
            0x080770B8, // 46
            0x08077494, // 47
            0x08077084, // 48
            0x08077460, // 49
            0x080770EC, // 50
            0x08077120, // 51
            0x08077154, // 52
            0x08077188, // 53
            0x080771BC, // 54
            0x080771F0, // 55
            0x08077224, // 56
            0x080773F8, // 57
            0x0807742C, // 58
            0x08077258, // 59
            0x080774C8, // 60
            0x080774FC, // 61
            0x08077530, // 62
            0x08077564, // 63
            0x0807728C, // 64
            0x08077598, // 65
            0x080775CC, // 66
            0x080772C0, // 67
            0x080772F4, // 68
            0x08077390, // 69
            0x080773C4, // 70
            0x08077328, // 71
            0x0807735C, // 72
            0x0807769C, // 73
            0x080776D0, // 74
            0x08077704, // 75
            0x08077744, // 76
            0x08035F4C, // 77
            0x08035F80, // 78
            0x0809DAF0, // 79
            0x0809DAF0, // 80
            0x0809DAF0, // 81
            0x0809DAF0, // 82
            0x0809DAF0, // 83
            0x0809DAF0, // 84
            0x0809DAF0, // 85
            0x0809D610, // 86
            0x0809D644, // 87
            0x0809D678, // 88
            0x0809D6AC, // 89
            0x0809D6E0, // 90
            0x0809D714, // 91
            0x0809D748, // 92
            0x0809D77C, // 93
            0x0809D7B0, // 94
            0x0809D7E4, // 95
            0x0809D818, // 96
            0x0809D84C, // 97
            0x0809D880, // 98
            0x0809D8B4, // 99
            0x0809D5DC, // 100
            0x0809D8E8, // 101
            0x0809D91C, // 102
            0x0809D9EC, // 103
            0x0809D9B8, // 104
            0x0809DA20, // 105
            0x0809DA54, // 106
            0x0809DA88, // 107
            0x0809DABC, // 108
            null, // 109
            0x0809DAF0, // 110
            0x08077634, // 111
            0x08077668, // 112
            0x0809DAF0, // 113
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0809DDF8,
            0x080A07DC,
            0x080A4138,
            0x080A61C0,
            0x080A6258,
            0x080A69E4,
            0x080A7184,
            0x080A78EC,
            0x080A79D4,
            0x080A8160,
            0x080A85D0,
            0x080A8D70,
            0x080AAB00,
            0x080AB26C,
            0x080ABA64,
            0x080AC5E8,
            0x080ACD74,
            0x080AD504,
            0x080ADCC0,
            0x080AE468,
            0x080AF2CC,
            0x080B3BE8,
            0x080B435C,
            0x080B4AE8,
            0x080B7308,
            0x080B79F8,
            0x080B8198,
            0x080B8250,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08035324, // script_waitForInputOrTime
            0x080737AC, // movie_intro
            0x08073948, // movie_garage
            0x080739A4, // movie_credits
            0x08073A38, // movie_gameIntro
            0x08073F9C, // movie_earthBossIntro
            0x08074134, // movie_earthBossCrashWin
            0x08074380, // movie_earthBossEvilWin
            0x0807456C, // movie_barinBossIntro
            0x08074704, // movie_barinBossCrashWin
            0x080748F0, // movie_barinBossEvilWin
            0x08074B3C, // movie_fenomBossIntro
            0x08074CF8, // movie_fenomBossCrashWin
            0x08074F14, // movie_fenomBossEvilWin
            0x08075154, // movie_tekneeBossIntro
            0x080752CC, // movie_tekneeBossCrashWin
            0x08075404, // movie_tekneeBossEvilWin
            0x08075538, // movie_veloBossIntro
            0x080755F8, // movie_veloBossCrashWin
            0x08075958, // movie_veloBossEvilWin
            0x08075B40, // movie_findFakeCrash
            0x08075C88, // SCRIPT_pagedTextLoop
            0x0807BC78, // script_license
            0x0807BCB8, // script_intro
            0x0807BCF8, // script_credits
            0x0807BD40, // script_findFakeCrash
            0x0807BD88, // script_earthBossIntro
            0x0807BDD4, // script_earthBossCrashWin
            0x0807BE1C, // script_earthBossEvilWin
            0x0807BE64, // script_barinBossIntro
            0x0807BEB0, // script_barinBossCrashWin
            0x0807BEF8, // script_barinBossEvilWin
            0x0807BF40, // script_fenomBossIntro
            0x0807BF8C, // script_fenomBossCrashWin
            0x0807BFD4, // script_fenomBossEvilWin
            0x0807C01C, // script_tekneeBossIntro
            0x0807C068, // script_tekneeBossCrashWin
            0x0807C0B4, // script_tekneeBossEvilWin
            0x0807C0FC, // script_veloBossIntro
            0x0807C144, // script_veloBossCrashWin
            0x0807C18C, // script_veloBossEvilWin
        };
    }
    public class GBAVV_NitroKartJP_Manager : GBAVV_NitroKart_Manager
    {
        public override string[] Languages => new string[]
        {
            "Dutch",
            "English",
            "French",
            "German",
            "Italian",
            "Spanish",
            "Japanese",
        };

        public override uint ObjTypesPointer => 0x08008344;
        public override uint?[] ObjTypesDataPointers => new uint?[]
        {
            0x0808A9D8, // 0
            0x0808A9D8, // 1
            0x0808A9D8, // 2
            null, // 3
            null, // 4
            null, // 5
            null, // 6
            null, // 7
            null, // 8
            null, // 9
            null, // 10
            null, // 11
            null, // 12
            0x0808A9D8, // 13
            null, // 14
            null, // 15
            0x0808A9D8, // 16
            0x0808A9D8, // 17
            0x0808A9D8, // 18
            0x0808A9D8, // 19
            0x0808A9D8, // 20
            0x0808A9D8, // 21
            0x0808A9D8, // 22
            0x0808A9D8, // 23
            0x0808A9D8, // 24
            0x0808A9D8, // 25
            0x0808A9D8, // 26
            0x0808A9D8, // 27
            0x0808A9D8, // 28
            0x0808A9D8, // 29
            0x0808A9D8, // 30
            0x0808A41C, // 31
            0x0808A450, // 32
            0x0808A484, // 33
            0x080367F8, // 34
            0x0803683C, // 35
            0x08036880, // 36
            0x080367B4, // 37
            0x08036660, // 38
            0x080366A4, // 39
            0x080368C4, // 40
            0x080366E8, // 41
            0x0803672C, // 42
            0x08036770, // 43
            0x080644E8, // 44
            0x08063F38, // 45
            0x08063FA0, // 46
            0x0806437C, // 47
            0x08063F6C, // 48
            0x08064348, // 49
            0x08063FD4, // 50
            0x08064008, // 51
            0x0806403C, // 52
            0x08064070, // 53
            0x080640A4, // 54
            0x080640D8, // 55
            0x0806410C, // 56
            0x080642E0, // 57
            0x08064314, // 58
            0x08064140, // 59
            0x080643B0, // 60
            0x080643E4, // 61
            0x08064418, // 62
            0x0806444C, // 63
            0x08064174, // 64
            0x08064480, // 65
            0x080644B4, // 66
            0x080641A8, // 67
            0x080641DC, // 68
            0x08064278, // 69
            0x080642AC, // 70
            0x08064210, // 71
            0x08064244, // 72
            0x08064584, // 73
            0x080645B8, // 74
            0x080645EC, // 75
            0x0806462C, // 76
            0x08036948, // 77
            0x0803697C, // 78
            0x0808A9D8, // 79
            0x0808A9D8, // 80
            0x0808A9D8, // 81
            0x0808A9D8, // 82
            0x0808A9D8, // 83
            0x0808A9D8, // 84
            0x0808A9D8, // 85
            0x0808A4F8, // 86
            0x0808A52C, // 87
            0x0808A560, // 88
            0x0808A594, // 89
            0x0808A5C8, // 90
            0x0808A5FC, // 91
            0x0808A630, // 92
            0x0808A664, // 93
            0x0808A698, // 94
            0x0808A6CC, // 95
            0x0808A700, // 96
            0x0808A734, // 97
            0x0808A768, // 98
            0x0808A79C, // 99
            0x0808A4C4, // 100
            0x0808A7D0, // 101
            0x0808A804, // 102
            0x0808A8D4, // 103
            0x0808A8A0, // 104
            0x0808A908, // 105
            0x0808A93C, // 106
            0x0808A970, // 107
            0x0808A9A4, // 108
            null, // 109
            0x0808A9D8, // 110
            0x0806451C, // 111
            0x08064550, // 112
            0x0808A9D8, // 113
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0808ACE0,
            0x0808D7D8,
            0x08091134,
            0x080931BC,
            0x08093254,
            0x080939E0,
            0x08094180,
            0x080948E8,
            0x080949D0,
            0x0809515C,
            0x080955CC,
            0x08095D6C,
            0x08097AFC,
            0x08098268,
            0x08098A60,
            0x080995E4,
            0x08099D70,
            0x0809A500,
            0x0809ACBC,
            0x0809B464,
            0x0809C2C8,
            0x080A0BE4,
            0x080A1358,
            0x080A1AE4,
            0x080A4DC4,
            0x080A54B4,
            0x080A5C54,
            0x080A5D0C,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08035D20, // script_waitForInputOrTime
            0x08060688, // movie_intro
            0x08060830, // movie_garage
            0x0806088C, // movie_credits
            0x08060920, // movie_gameIntro
            0x08060E84, // movie_earthBossIntro
            0x0806101C, // movie_earthBossCrashWin
            0x08061268, // movie_earthBossEvilWin
            0x08061454, // movie_barinBossIntro
            0x080615EC, // movie_barinBossCrashWin
            0x080617D8, // movie_barinBossEvilWin
            0x08061A24, // movie_fenomBossIntro
            0x08061BE0, // movie_fenomBossCrashWin
            0x08061DFC, // movie_fenomBossEvilWin
            0x0806203C, // movie_tekneeBossIntro
            0x080621B4, // movie_tekneeBossCrashWin
            0x080622EC, // movie_tekneeBossEvilWin
            0x08062420, // movie_veloBossIntro
            0x080624E0, // movie_veloBossCrashWin
            0x08062840, // movie_veloBossEvilWin
            0x08062A28, // movie_findFakeCrash
            0x08062B70, // SCRIPT_pagedTextLoop
            0x08068B60, // script_license
            0x08068BA0, // script_intro
            0x08068BE0, // script_credits
            0x08068C28, // script_findFakeCrash
            0x08068C70, // script_earthBossIntro
            0x08068CBC, // script_earthBossCrashWin
            0x08068D04, // script_earthBossEvilWin
            0x08068D4C, // script_barinBossIntro
            0x08068D98, // script_barinBossCrashWin
            0x08068DE0, // script_barinBossEvilWin
            0x08068E28, // script_fenomBossIntro
            0x08068E74, // script_fenomBossCrashWin
            0x08068EBC, // script_fenomBossEvilWin
            0x08068F04, // script_tekneeBossIntro
            0x08068F50, // script_tekneeBossCrashWin
            0x08068F9C, // script_tekneeBossEvilWin
            0x08068FE4, // script_veloBossIntro
            0x0806902C, // script_veloBossCrashWin
            0x08069074, // script_veloBossEvilWin
        };
    }
}