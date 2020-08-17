namespace R1Engine
{
    /// <summary>
    /// Status bar save data for Rayman 1 on PC
    /// </summary>
    public class R1_PC_SaveDataStatusBar : R1Serializable
    {
        /// <summary>
        /// The current number of lives, 0-99
        /// </summary>
        public byte LivesCount { get; set; }

        // Some flags
        public byte Unk_01 { get; set; }
        public byte Unk_02 { get; set; }
        public byte Unk_03 { get; set; }
        public byte Unk_04 { get; set; }
        public byte Unk_05 { get; set; }

        /// <summary>
        /// The current number of tings, 0-99
        /// </summary>
        public byte TingsCount { get; set; }
        public byte Unk_07 { get; set; }
        public byte Unk_08 { get; set; }

        /// <summary>
        /// The current maximum health count (always one less)
        /// </summary>
        public byte MaxHealth { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            LivesCount = s.Serialize<byte>(LivesCount, name: nameof(LivesCount));
            Unk_01 = s.Serialize<byte>(Unk_01, name: nameof(Unk_01));
            Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
            Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));
            Unk_04 = s.Serialize<byte>(Unk_04, name: nameof(Unk_04));
            Unk_05 = s.Serialize<byte>(Unk_05, name: nameof(Unk_05));
            TingsCount = s.Serialize<byte>(TingsCount, name: nameof(TingsCount));
            Unk_07 = s.Serialize<byte>(Unk_07, name: nameof(Unk_07));
            Unk_08 = s.Serialize<byte>(Unk_08, name: nameof(Unk_08));
            MaxHealth = s.Serialize<byte>(MaxHealth, name: nameof(MaxHealth));
        }
    }
}