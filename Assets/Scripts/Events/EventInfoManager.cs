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
        /// Generated a JSON file with the common event information
        /// </summary>
        /// <param name="designerBasePath">The Rayman Designer base path</param>
        /// <param name="pcBasePath">The Rayman 1 PC base path</param>
        /// <param name="csvPath">The .csv file path</param>
        /// <param name="outputFilePath">The JSON output path</param>
        public static void GenerateEventInfo(string designerBasePath, string pcBasePath, string csvPath, string outputFilePath)
        {
            var rdManager = new PC_RD_Manager();
            var pcManager = new PC_R1_Manager();

            // Read the event localization files
            var loc = rdManager.GetEventLocFiles(designerBasePath)["USA"].SelectMany(x => x.LocItems).ToArray();

            // Create the event info to serialize
            var eventInfo = new List<EventInfoData>();

            // Enumerate each Designer world
            foreach (World world in EnumHelpers.GetValues<World>())
            {
                // Get the event manifest file path
                var eventFilePath = Path.Combine(designerBasePath, rdManager.GetWorldName(world), "EVE.MLT");

                // Read the manifest
                var eventFile = FileFactory.Read<PC_RD_EventManifestFile>(eventFilePath, new GameSettings(designerBasePath));

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

                    if (!data.Worlds.Contains(world))
                        // Add the world
                        data.Worlds.Add(world); 

                    if (data.PC_RD_Info == null)
                        data.PC_RD_Info = new EventInfoData.PC_RD_EventInfoData(e);

                    if (!eventInfo.Contains(data))
                        eventInfo.Add(data);
                }
            }

            // Enumerate each PC world
            foreach (World world in EnumHelpers.GetValues<World>())
            {
                // Enumerate each level
                for (int i = 1; i < pcManager.GetLevelCount(new GameSettings(pcBasePath, world)) + 1; i++)
                {
                    // Get the settings
                    var s = new GameSettings(pcBasePath, world, i);

                    // Get the level file path
                    var lvlFilePath = pcManager.GetLevelFilePath(s);

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

                        if (!data.Worlds.Contains(world))
                            // Add the world
                            data.Worlds.Add(world);

                        if (data.PC_R1_Info == null)
                            data.PC_R1_Info = new EventInfoData.PC_R1_EventInfoData(e, lvl.EventCommands[index], world);

                        if (!data.PC_R1_Info.DES.ContainsKey(world))
                            data.PC_R1_Info.DES.Add(world, e.DES);

                        if (!eventInfo.Contains(data))
                            eventInfo.Add(data);

                        index++;
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

                        item.CustomName = desc;
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