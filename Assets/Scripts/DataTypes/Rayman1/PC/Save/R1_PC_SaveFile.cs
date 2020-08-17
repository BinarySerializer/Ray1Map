using System;

namespace R1Engine
{
    /// <summary>
    /// Save file data for Rayman 1 (PC)
    /// </summary>
    public class R1_PC_SaveFile : R1Serializable
    {
        /// <summary>
        /// The save file name (maximum of 3 characters)
        /// </summary>
        public string SaveName { get; set; }

        /// <summary>
        /// The number of remaining continues
        /// </summary>
        public byte ContinuesCount { get; set; }

        // World map data for every level (last 6 are the save points)
        public R1_PC_SaveDataLevel[] Wi_Save_Zone { get; set; }

        public RayEvtsFlags RayEvts { get; set; }

        // Fist stuff
        // Byte 12 gets set to 0xC when you enter gold fist cheat
        public byte[] Poing { get; set; }

        public R1_PC_SaveDataStatusBar StatusBar { get; set; }

        // Always one less than actual health
        public byte CurrentHealth { get; set; }

        // 32 bytes per map (not counting Breakout). Consists of 256 bits, where each is a flag for an event if it's been collected (cages & lives).
        public byte[][] SaveZone { get; set; }

        // 32 bits for each world, where each bit indicates if the bonus has been completed for that map
        public byte[] BonusPerfect { get; set; }

        /// <summary>
        /// The placement on the world map to start
        /// </summary>
        public ushort WorldIndex { get; set; }

        // First byte is a flag for having beaten specific bosses
        public ushort FinBossLevel { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            SaveName = s.SerializeString(SaveName, 4, name: nameof(SaveName));
            ContinuesCount = s.Serialize<byte>(ContinuesCount, name: nameof(ContinuesCount));
            Wi_Save_Zone = s.SerializeObjectArray<R1_PC_SaveDataLevel>(Wi_Save_Zone, 24, name: nameof(Wi_Save_Zone));
            RayEvts = s.Serialize<RayEvtsFlags>(RayEvts, name: nameof(RayEvts));
            Poing = s.SerializeArray<byte>(Poing, 20, name: nameof(Poing));
            StatusBar = s.SerializeObject<R1_PC_SaveDataStatusBar>(StatusBar, name: nameof(StatusBar));
            CurrentHealth = s.Serialize<byte>(CurrentHealth, name: nameof(CurrentHealth));

            if (SaveZone == null)
                SaveZone = new byte[81][];

            for (int i = 0; i < SaveZone.Length; i++)
                SaveZone[i] = s.SerializeArray<byte>(SaveZone[i], 32, name: $"{nameof(SaveZone)}[{i}]");

            BonusPerfect = s.SerializeArray<byte>(BonusPerfect, 24, name: nameof(BonusPerfect));
            WorldIndex = s.Serialize<ushort>(WorldIndex, name: nameof(WorldIndex));
            FinBossLevel = s.Serialize<ushort>(FinBossLevel, name: nameof(FinBossLevel));
        }

        [Flags]
        public enum RayEvtsFlags : ushort
        {
            None = 0,

            Fist = 1 << 0,
            Hang = 1 << 1,

            Helico = 1 << 2,
            SuperHelico = 1 << 3,

            Unk_4 = 1 << 4,
            Unk_5 = 1 << 5,

            Seed = 1 << 6,
            Grab = 1 << 7,

            Run = 1 << 8,

            // Rest are for temp stuff, like Mr Dark spells etc.
            Unk_9 = 1 << 9,
            Unk_10 = 1 << 10,
            Unk_11 = 1 << 11,
            Unk_12 = 1 << 12,
            Unk_13 = 1 << 13,
            Unk_14 = 1 << 14,
            Unk_15 = 1 << 15,
        }
    }
}