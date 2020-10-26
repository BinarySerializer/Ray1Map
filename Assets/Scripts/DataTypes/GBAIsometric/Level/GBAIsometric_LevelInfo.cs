namespace R1Engine
{
    public class GBAIsometric_LevelInfo : R1Serializable
    {
        public Pointer<GBAIsometric_LevelData> LevelDataPointer { get; set; }

        public short MurfyStampScore1 { get; set; }
        public short MurfyStampScore2 { get; set; }
        public short MurfyStampScore3 { get; set; }

        public short Short_0A { get; set; }
        public short Short_0C { get; set; }

        public short Short_0E { get; set; }
        public short Short_10 { get; set; }
        public short Short_12 { get; set; }
        public short Short_14 { get; set; }

        public byte[] Bytes_16 { get; set; }

        public Pointer<GBAIsometric_MapLayer> MapPointer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LevelDataPointer = s.SerializePointer<GBAIsometric_LevelData>(LevelDataPointer, resolve: true, name: nameof(LevelDataPointer));
            MurfyStampScore1 = s.Serialize<short>(MurfyStampScore1, name: nameof(MurfyStampScore1));
            MurfyStampScore2 = s.Serialize<short>(MurfyStampScore2, name: nameof(MurfyStampScore2));
            MurfyStampScore3 = s.Serialize<short>(MurfyStampScore3, name: nameof(MurfyStampScore3));
            Short_0A = s.Serialize<short>(Short_0A, name: nameof(Short_0A));
            Short_0C = s.Serialize<short>(Short_0C, name: nameof(Short_0C));
            Short_0E = s.Serialize<short>(Short_0E, name: nameof(Short_0E));
            Short_10 = s.Serialize<short>(Short_10, name: nameof(Short_10));
            Short_12 = s.Serialize<short>(Short_12, name: nameof(Short_12));
            Short_14 = s.Serialize<short>(Short_14, name: nameof(Short_14));
            Bytes_16 = s.SerializeArray<byte>(Bytes_16, 14, name: nameof(Bytes_16));
            MapPointer = s.SerializePointer(MapPointer, resolve: true, name: nameof(MapPointer));
        }
    }
}