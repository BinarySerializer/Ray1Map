using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Save file data for Rayman 1 (PC)
    /// </summary>
    public class R1_PC_SaveFile : BinarySerializable
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

        public R1_RayEvtsFlags RayEvts { get; set; }

        public R1_Poing Poing { get; set; }

        public R1_StatusBar StatusBar { get; set; }

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
            RayEvts = s.Serialize<R1_RayEvtsFlags>(RayEvts, name: nameof(RayEvts));
            Poing = s.SerializeObject<R1_Poing>(Poing, name: nameof(Poing));
            StatusBar = s.SerializeObject<R1_StatusBar>(StatusBar, name: nameof(StatusBar));
            CurrentHealth = s.Serialize<byte>(CurrentHealth, name: nameof(CurrentHealth));

            if (SaveZone == null)
                SaveZone = new byte[81][];

            for (int i = 0; i < SaveZone.Length; i++)
                SaveZone[i] = s.SerializeArray<byte>(SaveZone[i], 32, name: $"{nameof(SaveZone)}[{i}]");

            BonusPerfect = s.SerializeArray<byte>(BonusPerfect, 24, name: nameof(BonusPerfect));
            WorldIndex = s.Serialize<ushort>(WorldIndex, name: nameof(WorldIndex));
            FinBossLevel = s.Serialize<ushort>(FinBossLevel, name: nameof(FinBossLevel));
        }
    }
}