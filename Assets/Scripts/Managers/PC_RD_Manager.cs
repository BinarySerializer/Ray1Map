using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Designer (PC)
    /// </summary>
    public class PC_RD_Manager : IGameManager
    {
        /// <summary>
        /// Generated a JSON file with the common event information
        /// </summary>
        /// <param name="basePath">The Rayman Designer base path</param>
        /// <param name="outputFilePath">The JSON output path</param>
        public void GenerateEventInfo(string basePath, string outputFilePath)
        {
            // Read the event localization files
            var loc = GetEventLocFiles(basePath)["USA"].SelectMany(x => x.LocItems).ToArray();
         
            // Create the event info to serialize
            var eventInfo = new Dictionary<World, List<Common_EventInfo>>();

            // Enumerate each world
            foreach (World world in EnumHelpers.GetValues<World>())
            {
                // Create the world entry
                eventInfo.Add(world, new List<Common_EventInfo>());

                // Get the event manifest file path
                var eventFilePath = Path.Combine(basePath, GetWorldName(world), "EVE.MLT");

                // Read the manifest
                var eventFile = FileFactory.Read<PC_RD_EventManifestFile>(eventFilePath);

                // Add each entry
                foreach (var e in eventFile.Items)
                {
                    // Attempt to find the matching localization entry
                    var locItem = loc.FindItem(x => x.LocKey == e.Name);

                    // Add the common info
                    eventInfo[world].Add(new Common_EventInfo
                    {
                        Type = Int32.TryParse(e.Obj_type, out var v) ? v : -1,
                        Etat = (int)e.Etat,
                        SubEtat = e.SubEtat,
                        DesignerName = locItem?.Name ?? "N/A",
                        CustomName = null,
                        DesignerDescription = locItem?.Description ?? "N/A",
                        IsAlways = e.DesignerGroup == -1
                    });
                }
            }

            // Serialize to the file
            JsonHelpers.SerializeToFile(eventInfo, outputFilePath);
        }

        /// <summary>
        /// Gets the localization files for each event, with the language tag as the key
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <returns>The localization files</returns>
        public Dictionary<string, PC_RD_EventLocFile[]> GetEventLocFiles(string basePath)
        {
            var pcDataDir = Path.Combine(basePath, "PCMAP");
            
            var output = new Dictionary<string, PC_RD_EventLocFile[]>();

            foreach (var langDir in Directory.GetDirectories(pcDataDir, "???", SearchOption.TopDirectoryOnly))
            {
                output.Add(Path.GetFileName(langDir), Directory.GetFiles(langDir, "*.wld", SearchOption.TopDirectoryOnly).Select(locFile => FileFactory.Read<PC_RD_EventLocFile>(locFile)).ToArray());
            }

            return output;
        }

        /// <summary>
        /// Gets the name for the world
        /// </summary>
        /// <returns>The world name</returns>
        public string GetWorldName(World world)
        {
            switch (world)
            {
                case World.Jungle:
                    return "JUNGLE";
                case World.Music:
                    return "MUSIC";
                case World.Mountain:
                    return "MOUNTAIN";
                case World.Image:
                    return "IMAGE";
                case World.Cave:
                    return "CAVE";
                case World.Cake:
                    return "CAKE";
                default:
                    throw new ArgumentOutOfRangeException(nameof(world), world, null);
            }
        }

        /// <summary>
        /// Gets the level count for the specified world
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <returns>The level count</returns>
        public int GetLevelCount(string basePath, World world)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <param name="level">The level</param>
        /// <returns>The level</returns>
        public Common_Lev LoadLevel(string basePath, World world, int level)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <param name="level">The level</param>
        /// <param name="levelData">The common level data</param>
        public void SaveLevel(string basePath, World world, int level, Common_Lev levelData)
        {
            throw new NotImplementedException();
        }
    }
}