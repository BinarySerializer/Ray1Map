using System;
using System.Collections.Generic;

namespace R1Engine
{
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
            Names = new Dictionary<World, EventInfoItemName>();
        }

        /// <summary>
        /// Imports common data from Rayman Designer (PC)
        /// </summary>
        /// <param name="e">The event manifest item</param>
        /// <param name="locItem">The localization item, if available</param>
        /// <param name="world">The world the event appears in</param>
        public void Import(PC_RD_EventManifestFile.PC_RD_EventManifestItem e, PC_RD_EventLocItem locItem, World world)
        {
            Type = Int32.TryParse(e.Obj_type, out var v) ? v : -1;
            Etat = (int)e.Etat;
            SubEtat = Int32.TryParse(e.SubEtat, out var vv) ? vv : -1;
            IsAlways = e.DesignerGroup == -1;

            if (!Names.ContainsKey(world))
                Names.Add(world, new EventInfoItemName()
                {
                    DesignerName = locItem?.Name,
                    DesignerDescription = locItem?.Description
                });
        }

        /// <summary>
        /// Imports common data from Rayman 1 (PC)
        /// </summary>
        /// <param name="e">The event item</param>
        /// <param name="world">The world the event appears in</param>
        public void Import(PC_Event e, World world)
        {
            Type = (int)e.Type;
            Etat = e.Etat;
            SubEtat = e.SubEtat;

            if (!Names.ContainsKey(world))
                Names.Add(world, new EventInfoItemName());
        }

        /// <summary>
        /// Checks if the specified info data item matches the current one in term of types
        /// </summary>
        /// <param name="eventInfoData">The info data item to compare to</param>
        /// <returns>True if they match</returns>
        public bool MatchesType(EventInfoData eventInfoData) => GetEventID() == eventInfoData.GetEventID();

        /// <summary>
        /// Gets the event ID
        /// </summary>
        /// <returns>The event ID</returns>
        public string GetEventID() => $"{Type.ToString().PadLeft(3, '0')}{Etat.ToString().PadLeft(3, '0')}{SubEtat.ToString().PadLeft(3, '0')}";

        /// <summary>
        /// The event names, based on the world it appears in
        /// </summary>
        public Dictionary<World, EventInfoItemName> Names { get; set; }

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
            /// <param name="world">The world the event is for</param>
            public PC_R1_EventInfoData(PC_Event e, PC_EventCommand cmd, World world)
            {
                DES = new Dictionary<World, uint>()
                {
                    {
                        world,
                        e.DES
                    }
                };
                ETA = new Dictionary<World, uint>()
                {
                    {
                        world,
                        e.ETA
                    }
                };
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

            /// <summary>
            /// Converts the info data to an event data type
            /// </summary>
            /// <param name="world">The world to get the event for</param>
            /// <returns>The event data type</returns>
            public PC_Event ToEvent(World world)
            {
                return new PC_Event()
                {
                    DES = DES[world],
                    DES2 = DES[world],
                    DES3 = DES[world],
                    ETA = ETA[world],
                    OffsetBX = OffsetBX,
                    OffsetBY = OffsetBY,
                    OffsetHY = OffsetHY,
                    FollowSprite = FollowSprite,
                    HitPoints = HitPoints,
                    UnkGroup = UnkGroup,
                    HitSprite = HitSprite,
                    FollowEnabled = FollowEnabled,
                    Unknown3 = new byte[16],
                    Unknown4 = new byte[20],
                    Unknown5 = new byte[28],
                    Unknown10 = new byte[6]
                };
            }

            public Dictionary<World, uint> DES { get; set; }

            public Dictionary<World, uint> ETA { get; set; }

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

            public uint Hit_sprite { get; set; }

            public int DesignerGroup { get; set; }
        }

        /// <summary>
        /// Name for an event info item
        /// </summary>
        public class EventInfoItemName
        {
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
        }
    }
}