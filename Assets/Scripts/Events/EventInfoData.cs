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
        /// <param name="info">The general event info</param>
        public EventInfoData(GeneralEventInfoData info)
        {
            Info = info;
            PCInfo = new Dictionary<GameMode, PC_EventInfo>();
        }

        /// <summary>
        /// The general event info
        /// </summary>
        public GeneralEventInfoData Info { get; }

        /// <summary>
        /// The event ID
        /// </summary>
        public EventID ID => Info.ID;

        /// <summary>
        /// The Rayman PC event info
        /// </summary>
        public Dictionary<GameMode, PC_EventInfo> PCInfo { get; set; }

        /// <summary>
        /// The Rayman Designer (PC) event manifest data
        /// </summary>
        public PC_DesignerEventManifestData PCDesignerManifest { get; set; }

        /// <summary>
        /// Event info data for Rayman PC
        /// </summary>
        public class PC_EventInfo
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            public PC_EventInfo()
            {

            }

            /// <summary>
            /// Constructor from an existing event
            /// </summary>
            /// <param name="e">The event</param>
            /// <param name="cmd">The event command</param>
            /// <param name="world">The world the event is for</param>
            public PC_EventInfo(PC_Event e, PC_EventCommand cmd, World world)
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
        /// Event manifest data for Rayman Designer
        /// </summary>
        public class PC_DesignerEventManifestData
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            public PC_DesignerEventManifestData()
            {

            }

            /// <summary>
            /// Constructor from an existing event
            /// </summary>
            /// <param name="e">The event</param>
            public PC_DesignerEventManifestData(PC_RD_EventManifestFile.PC_RD_EventManifestItem e)
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
    }
}