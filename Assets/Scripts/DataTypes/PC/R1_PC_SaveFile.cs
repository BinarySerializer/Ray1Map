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

        public byte[] Wi_Save_Zone { get; set; }

        public byte[] RayEvts { get; set; }

        public byte[] Poing { get; set; }

        public byte[] StatusBar { get; set; }

        public byte Unk { get; set; }

        public byte[] SaveZone { get; set; }

        public byte[] BonusPerfect { get; set; }

        public ushort WorldIndex { get; set; }

        public ushort FinBossLevel { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            SaveName = s.SerializeString(SaveName, 4, name: nameof(SaveName));
            ContinuesCount = s.Serialize<byte>(ContinuesCount, name: nameof(ContinuesCount));
            Wi_Save_Zone = s.SerializeArray<byte>(Wi_Save_Zone, 24, name: nameof(Wi_Save_Zone));
            RayEvts = s.SerializeArray<byte>(RayEvts, 2, name: nameof(RayEvts));
            Poing = s.SerializeArray<byte>(Poing, 20, name: nameof(Poing));
            StatusBar = s.SerializeArray<byte>(StatusBar, 10, name: nameof(StatusBar));
            Unk = s.Serialize<byte>(Unk, name: nameof(Unk));
            SaveZone = s.SerializeArray<byte>(SaveZone, 2592, name: nameof(SaveZone));
            BonusPerfect = s.SerializeArray<byte>(BonusPerfect, 24, name: nameof(BonusPerfect));
            WorldIndex = s.Serialize<ushort>(WorldIndex, name: nameof(WorldIndex));
            FinBossLevel = s.Serialize<ushort>(FinBossLevel, name: nameof(FinBossLevel));
        }
    }
}