namespace R1Engine
{
    public class GBAIsometric_LevelInfo : R1Serializable
    {
        public Pointer LevelDataPointer { get; set; } // Leads to 4 layer structs, followed by more data

        public ushort MurfyStampScore1 { get; set; }
        public ushort MurfyStampScore2 { get; set; }
        public ushort MurfyStampScore3 { get; set; }

        public ushort Ushort_0A { get; set; }
        public ushort Ushort_0C { get; set; }

        public byte[] Bytes_0E { get; set; }

        public Pointer UnkPointer { get; set; } // Only set in some levels

        public override void SerializeImpl(SerializerObject s)
        {
            LevelDataPointer = s.SerializePointer(LevelDataPointer, name: nameof(LevelDataPointer));
            MurfyStampScore1 = s.Serialize<ushort>(MurfyStampScore1, name: nameof(MurfyStampScore1));
            MurfyStampScore2 = s.Serialize<ushort>(MurfyStampScore2, name: nameof(MurfyStampScore2));
            MurfyStampScore3 = s.Serialize<ushort>(MurfyStampScore3, name: nameof(MurfyStampScore3));
            Ushort_0A = s.Serialize<ushort>(Ushort_0A, name: nameof(Ushort_0A));
            Ushort_0C = s.Serialize<ushort>(Ushort_0C, name: nameof(Ushort_0C));
            Bytes_0E = s.SerializeArray<byte>(Bytes_0E, 22, name: nameof(Bytes_0E));
            UnkPointer = s.SerializePointer(UnkPointer, name: nameof(UnkPointer));
        }
    }
}