using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        /// <param name="outputFilePath">The JSON output path</param>
        public static void GenerateEventInfo(string designerBasePath, string pcBasePath, string outputFilePath)
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
                var eventFile = FileFactory.Read<PC_RD_EventManifestFile>(eventFilePath);

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
                for (int i = 1; i < pcManager.GetLevelCount(pcBasePath, world) + 1; i++)
                {
                    // Get the level file path
                    var lvlFilePath = pcManager.GetLevelFilePath(pcBasePath, world, i);

                    // Read the level
                    var lvl = FileFactory.Read<PC_R1_LevFile>(lvlFilePath);

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
                            data.PC_R1_Info = new EventInfoData.PC_R1_EventInfoData(e, lvl.EventCommands[index]);

                        if (!eventInfo.Contains(data))
                            eventInfo.Add(data);

                        index++;
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

        /// <summary>
        /// The event info data
        /// </summary>
        public class EventInfoData
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            public EventInfoData()
            {
                Worlds = new List<World>();
            }

            /// <summary>
            /// Imports common data from Rayman Designer (PC)
            /// </summary>
            /// <param name="e">The event manifest item</param>
            /// <param name="locItem">The localization item, if available</param>
            /// <param name="world">The world the event appears in</param>
            public void Import(PC_RD_EventManifestFile.PC_RD_EventManifestItem e, PC_RD_EventLocItem locItem, World world)
            {
                DesignerName = locItem?.Name;
                DesignerDescription = locItem?.Description;
                Type = Int32.TryParse(e.Obj_type, out var v) ? v : -1;
                Etat = (int)e.Etat;
                SubEtat = Int32.TryParse(e.SubEtat, out var vv) ? vv : -1;
                IsAlways = e.DesignerGroup == -1;

                if (!Worlds.Contains(world))
                    Worlds.Add(world);
            }

            /// <summary>
            /// Imports common data from Rayman 1 (PC)
            /// </summary>
            /// <param name="e">The event item</param>
            /// <param name="world">The world the event appears in</param>
            public void Import(PC_R1_Event e, World world)
            {
                Type = (int)e.Type;
                Etat = e.Etat;
                SubEtat = e.SubEtat;

                if (!Worlds.Contains(world))
                    Worlds.Add(world);
            }

            /// <summary>
            /// Checks if the specified info data item matches the current one in term of types
            /// </summary>
            /// <param name="eventInfoData">The info data item to compare to</param>
            /// <returns>True if they match</returns>
            public bool MatchesType(EventInfoData eventInfoData)
            {
                return Type == eventInfoData.Type && Etat == eventInfoData.Etat && SubEtat == eventInfoData.SubEtat;
            }

            /// <summary>
            /// The localized name from Rayman Designer
            /// </summary>
            public string DesignerName { get; set; }

            /// <summary>
            /// The custom name, if none was found in Rayman Designer
            /// </summary>
            public string CustomName { get; set; }

            /// <summary>
            /// The localized description from Rayman Designer
            /// </summary>
            public string DesignerDescription { get; set; }

            /// <summary>
            /// The event type
            /// </summary>
            public int Type { get; set; }

            /// <summary>
            /// The Etat index
            /// </summary>
            public int Etat { get; set; }

            /// <summary>
            /// The SubEtat index
            /// </summary>
            public int SubEtat { get; set; }

            /// <summary>
            /// Indicates if the event is an always event or not
            /// </summary>
            public bool? IsAlways { get; set; }

            /// <summary>
            /// The worlds the event can appear in
            /// </summary>
            public List<World> Worlds { get; set; }

            /// <summary>
            /// The Rayman 1 (PC) info
            /// </summary>
            public PC_R1_EventInfoData PC_R1_Info { get; set; }

            /// <summary>
            /// The Rayman Designer (PC) info
            /// </summary>
            public PC_RD_EventInfoData PC_RD_Info { get; set; }

            /// <summary>
            /// Event info data for Rayman 1 (PC)
            /// </summary>
            public class PC_R1_EventInfoData
            {
                /// <summary>
                /// Default constructor
                /// </summary>
                public PC_R1_EventInfoData()
                {
                    
                }

                /// <summary>
                /// Constructor from an existing event
                /// </summary>
                /// <param name="e">The event</param>
                /// <param name="cmd">The event command</param>
                public PC_R1_EventInfoData(PC_R1_Event e, PC_R1_EventCommand cmd)
                {
                    DES = e.DES;
                    DES2 = e.DES2;
                    DES3 = e.DES3;
                    ETA = e.ETA;
                    OffsetBX = e.OffsetBX;
                    OffsetBY = e.OffsetBY;
                    OffsetHY = e.OffsetHY;
                    FollowSprite = e.FollowSprite;
                    HitPoints = e.HitPoints;
                    UnkGroup = e.UnkGroup;
                    HitSprite = e.HitSprite;
                    FollowEnabled = e.FollowEnabled;
                    LabelOffsets = cmd.LabelOffsetTable;
                    Commands = cmd.EventCode;
                }

                public uint DES { get; set; }

                public uint DES2 { get; set; }

                public uint DES3 { get; set; }

                public uint ETA { get; set; }

                public byte OffsetBX { get; set; }

                public byte OffsetBY { get; set; }

                public byte OffsetHY { get; set; }

                public byte FollowSprite { get; set; }

                public ushort HitPoints { get; set; }

                public byte UnkGroup { get; set; }

                public byte HitSprite { get; set; }

                public byte FollowEnabled { get; set; }

                public ushort[] LabelOffsets { get; set; }
                
                public byte[] Commands { get; set; }
            }

            /// <summary>
            /// Event info data for Rayman Designer (PC)
            /// </summary>
            public class PC_RD_EventInfoData
            {
                /// <summary>
                /// Default constructor
                /// </summary>
                public PC_RD_EventInfoData()
                {
                    
                }

                /// <summary>
                /// Constructor from an existing event
                /// </summary>
                /// <param name="e">The event</param>
                public PC_RD_EventInfoData(PC_RD_EventManifestFile.PC_RD_EventManifestItem e)
                {
                    DESFile = e.DESFile;
                    IfCommand = e.IfCommand;
                    UnkGroup = e.UnkGroup;
                    ETAFile = e.ETAFile;
                    EventCommands = e.EventCommands;
                    Obj_typeString = Int32.TryParse(e.Obj_type, out _) ? null : e.Obj_type;
                    SubEtatString = Int32.TryParse(e.SubEtat, out _) ? null : e.SubEtat;
                    Offset_BX = e.Offset_BX;
                    Offset_BY = e.Offset_BY;
                    Offset_HY = e.Offset_HY;
                    Follow_enabled = e.Follow_enabled;
                    Follow_sprite = e.Follow_sprite;
                    Hitpoints = e.Hitpoints;
                    Hit_sprite = e.Hit_sprite;
                    DesignerGroup = e.DesignerGroup;
                }

                public string DESFile { get; set; }

                public string[] IfCommand { get; set; }

                public uint UnkGroup { get; set; }

                public string ETAFile { get; set; }

                public int[] EventCommands { get; set; }

                public string Obj_typeString { get; set; }

                public string SubEtatString { get; set; }

                public uint Offset_BX { get; set; }

                public uint Offset_BY { get; set; }

                public uint Offset_HY { get; set; }

                public uint Follow_enabled { get; set; }

                public uint Follow_sprite { get; set; }

                public uint Hitpoints { get; set; }

                public string Obj_type { get; set; }

                public uint Hit_sprite { get; set; }

                public int DesignerGroup { get; set; }
            }
        }
    }
}