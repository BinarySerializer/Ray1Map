using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public static class LevelEditorData
    {
        public static GeneralEventInfoData[] EventInfoData { get; private set; } = new GeneralEventInfoData[0];
        public static string[][] NameTable_R1PCDES { get; private set; }
        public static string[][] NameTable_R1PCETA { get; private set; }
        public static string[][] NameTable_EDUDES { get; private set; }
        public static string[][] NameTable_EDUETA { get; private set; }

        public static Context MainContext { get; set; }
        public static GameSettings CurrentSettings => MainContext?.Settings;

        public static Unity_Level Level { get; set; }
        public static Unity_ObjectManager ObjManager => Level.ObjManager;

        public static int MaxWidth => Level.Maps.Max(x => x.Width);
        public static int MaxHeight => Level.Maps.Max(x => x.Height);

        public static int CurrentMap { get; set; }
        public static int CurrentCollisionMap { get; set; }
        public static bool[] ShowEventsForMaps { get; set; }
        public static bool ShowOnlyActiveSector { get; set; }
        public static int ActiveSector { get; set; }

        public static async UniTask InitAsync(GameSettings settings, bool loadAll = false)
        {
            async UniTask loadFile(string fileName, Action<string> func)
            {
                await FileSystem.PrepareFile(fileName);

                if (FileSystem.FileExists(fileName))
                {
                    func(fileName);
                }
                else
                {
                    Debug.Log($"{fileName} has not been loaded");
                }
            }

            // Only load event manifests for Rayman 1
            if (loadAll || (settings.MajorEngineVersion == MajorEngineVersion.Rayman1 && settings.EngineVersion != EngineVersion.R2_PS1))
            {
                const string dir = "EventManifests/";

                // Load global event manifest
                await loadFile(dir + "Events.csv", file =>
                {
                    // Load the event info data
                    using (var csvFile = FileSystem.GetFileReadStream(file))
                        EventInfoData = GeneralEventInfoData.ReadCSV(csvFile);
                });

                // Load version specific mapping tables
                if (loadAll || settings.EngineVersion == EngineVersion.R1_PC || settings.EngineVersion == EngineVersion.R1_PocketPC)
                {
                    await loadFile(dir + "r1_des.json",
                        file => NameTable_R1PCDES = JsonHelpers.DeserializeFromFile<string[][]>(file));
                    await loadFile(dir + "r1_eta.json",
                        file => NameTable_R1PCETA = JsonHelpers.DeserializeFromFile<string[][]>(file));
                }

                if (loadAll || settings.EngineVersion == EngineVersion.R1_PC_Edu || settings.EngineVersion == EngineVersion.R1_PS1_Edu)
                {
                    await loadFile(dir + "edu_des.json",
                        file => NameTable_EDUDES = JsonHelpers.DeserializeFromFile<string[][]>(file));
                    await loadFile(dir + "edu_eta.json",
                        file => NameTable_EDUETA = JsonHelpers.DeserializeFromFile<string[][]>(file));
                }
                
                if (loadAll || settings.EngineVersion == EngineVersion.R1_PC_Kit)
                {
                    // Do nothing - Kit has its own mapping table
                }
                
                if (loadAll || settings.EngineVersion == EngineVersion.R1_PS1 || settings.EngineVersion == EngineVersion.R1_PS1_JP || settings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 || settings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6 || settings.EngineVersion == EngineVersion.R1_Saturn || settings.EngineVersion == EngineVersion.R1_GBA || settings.EngineVersion == EngineVersion.R1_DSi)
                {
                    // TODO: Load pointer mapping tables
                }
            }
        }
    }
}