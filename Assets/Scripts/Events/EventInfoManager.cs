using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Event info manager class
    /// </summary>
    public static class EventInfoManager
    {
        /// <summary>
        /// Generated a JSON file with the common event information based on the current settings
        /// </summary>
        /// <param name="csvPath">The .csv file path</param>
        /// <param name="outputFilePath">The output file path</param>
        public static void GenerateEventInfo(string csvPath, string outputFilePath)
        {
            var rdManager = new PC_RD_Manager();

            // Read the event localization files
            var loc = rdManager.GetEventLocFiles(Settings.GameDirectories[GameModeSelection.RaymanDesignerPC])["USA"].SelectMany(x => x.LocItems).ToArray();

            // Create the event info to serialize
            var eventInfo = new List<EventInfoData>();

            // Enumerate each Designer world
            foreach (World world in EnumHelpers.GetValues<World>())
            {
                // Get the event manifest file path
                var eventFilePath = Path.Combine(Settings.GameDirectories[GameModeSelection.RaymanDesignerPC], rdManager.GetWorldName(world), "EVE.MLT");

                // Read the manifest
                var eventFile = FileFactory.Read<PC_RD_EventManifestFile>(eventFilePath, new GameSettings(GameMode.RayKit, Settings.GameDirectories[GameModeSelection.RaymanDesignerPC]));

                // Add each entry
                foreach (PC_RD_EventManifestFile.PC_RD_EventManifestItem e in eventFile.Items)
                {
                    // Attempt to find the matching localization entry
                    PC_RD_EventLocItem locItem = loc.FindItem(x => x.LocKey == e.Name);
                    
                    // Create the event info data
                    var data = new EventInfoData();

                    // Import the data
                    data.Import(e, locItem, world);

                    // Check if the data has already been added, if so use that instead
                    data = eventInfo.Find(x => x.MatchesType(data)) ?? data;

                    if (!data.Names.ContainsKey(world))
                        // Add the world
                        data.Names.Add(world, new EventInfoData.EventInfoItemName()
                        {
                            DesignerName = locItem?.Name,
                            DesignerDescription = locItem?.Description
                        }); 

                    if (data.PCDesignerManifest == null)
                        data.PCDesignerManifest = new EventInfoData.PC_DesignerEventManifestData(e);

                    if (!eventInfo.Contains(data))
                        eventInfo.Add(data);
                }
            }

            // Enumerate the PC versions
            foreach (GameModeSelection mode in new GameModeSelection[]
            {
                GameModeSelection.RaymanPC,
                GameModeSelection.RaymanDesignerPC,
                GameModeSelection.RaymanByHisFansPC,
                GameModeSelection.Rayman60LevelsPC,
                GameModeSelection.RaymanEducationalPC,
            })
            {
                // Get the attribute data
                var attr = mode.GetAttribute<GameModeAttribute>();

                // Get the manager
                var manager = (PC_Manager)Activator.CreateInstance(attr.ManagerType);

                // Enumerate each PC world
                foreach (World world in EnumHelpers.GetValues<World>())
                {
                    // Get the settings
                    var s = new GameSettings(attr.GameMode, Settings.GameDirectories[mode], world)
                    {
                        EduVolume = Settings.EduVolume
                    };

                    // Enumerate each Rayman 1 level
                    foreach (var i in manager.GetLevels(s))
                    {
                        // Set the level
                        s.Level = i;

                        // Get the level file path
                        var lvlFilePath = manager.GetLevelFilePath(s);

                        // Read the level
                        var lvl = FileFactory.Read<PC_LevFile>(lvlFilePath, s);

                        var index = 0;

                        // Add every event
                        foreach (var e in lvl.Events)
                        {
                            // Create the event info data
                            var data = new EventInfoData();

                            // Import the data
                            data.Import(e, world);

                            // Check if the data has already been added, if so use that instead
                            data = eventInfo.Find(x => x.MatchesType(data)) ?? data;

                            if (!data.Names.ContainsKey(world))
                                // Add the world
                                data.Names.Add(world, new EventInfoData.EventInfoItemName());

                            if (!data.PCInfo.ContainsKey(attr.GameMode))
                                data.PCInfo.Add(attr.GameMode, new EventInfoData.PC_EventInfo(e, lvl.EventCommands[index], world));

                            if (!data.PCInfo[attr.GameMode].ETA.ContainsKey(world))
                                data.PCInfo[attr.GameMode].ETA.Add(world, e.ETA);

                            if (!data.PCInfo[attr.GameMode].DES.ContainsKey(world))
                                data.PCInfo[attr.GameMode].DES.Add(world, e.DES);

                            if (!eventInfo.Contains(data))
                                eventInfo.Add(data);

                            index++;
                        }
                    }
                }
            }

            // Add names from .csv file
            using (var csvFile = File.OpenRead(csvPath))
            {
                using (var reader = new StreamReader(csvFile))
                {
                    // Ignore header line
                    reader.ReadLine();

                    // Enumerate each line
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine()?.Split(',');

                        if (line == null)
                            break;

                        var desc = line[1].Trim('"');
                        var type = Int32.Parse(line[6]);
                        var subEtat = Int32.Parse(line[9]);
                        var etat = Int32.Parse(line[10]);

                        // Find matching item
                        var item = eventInfo.Find(x => x.Type == type &&
                                                       x.Etat == etat &&
                                                       x.SubEtat == subEtat);

                        if (item == null)
                        {
                            Debug.LogWarning($"No matching even for {desc} of type {type}");
                            continue;
                        }

                        // Get the world
                        var world = Path.GetFileName(Path.GetDirectoryName(line[17].Trim('"')));
                        
                        // Set the name
                        item.Names[(World)Enum.Parse(typeof(World), world, true)].CustomName = desc;
                    }
                }
            }

            // Serialize to the file
            JsonHelpers.SerializeToFile(eventInfo.OrderBy(x => x.Type), outputFilePath);
        }
        
        /// <summary>
        /// Loads the event info from a file
        /// </summary>
        /// <param name="filePath">The file path to load from</param>
        /// <returns>The loaded event info</returns>
        public static EventInfoData[] LoadEventInfo(string filePath = null)
        {
            // Default the path
            if (filePath == null)
                filePath = "CommonEvents.json";

            return Cache ?? (Cache = JsonHelpers.DeserializeFromFile<EventInfoData[]>(filePath));
        }

        /// <summary>
        /// The loaded event info cache
        /// </summary>
        private static EventInfoData[] Cache { get; set; }
    }
}