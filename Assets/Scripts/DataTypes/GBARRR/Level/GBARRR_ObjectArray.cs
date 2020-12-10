namespace R1Engine
{
    public class GBARRR_ObjectArray : R1Serializable
    {
        public bool IsUnusedMode7 { get; set; }

        public ushort Unk1 { get; set; } // Always 0?
        public ushort Unk2 { get; set; }
        public ushort Unk3 { get; set; }
        public uint ObjectsCount { get; set; }
        public uint OffsetsCount { get; set; }
        public ushort UShort_0E { get; set; }
        public uint[] Offsets { get; set; }
        public uint[] UnusedOffsets { get; set; }
        public uint ObjectsOffset { get; set; }
        public byte[] UnkData0 { get; set; }
        public uint ObjectsEndOffset { get; set; }
        public byte[] UnkData1 { get; set; }
        public byte[][] UnkDataBlocks { get; set; }

        public GBARRR_Object[] Objects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<ushort>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<ushort>(Unk2, name: nameof(Unk2));
            Unk3 = s.Serialize<ushort>(Unk3, name: nameof(Unk3));
            ObjectsCount = s.Serialize<uint>(ObjectsCount, name: nameof(ObjectsCount));
            OffsetsCount = s.Serialize<uint>(OffsetsCount, name: nameof(OffsetsCount));
            UShort_0E = s.Serialize<ushort>(UShort_0E, name: nameof(UShort_0E));
            Offsets = s.SerializeArray<uint>(Offsets, OffsetsCount, name: nameof(Offset));
            UnusedOffsets = s.SerializeArray<uint>(UnusedOffsets, 4-OffsetsCount, name: nameof(UnusedOffsets));
            ObjectsOffset = s.Serialize<uint>(ObjectsOffset, name: nameof(ObjectsOffset));
            UnkData0 = s.SerializeArray<byte>(UnkData0, 24, name: nameof(UnkData0));
            ObjectsEndOffset = s.Serialize<uint>(ObjectsEndOffset, name: nameof(ObjectsEndOffset));
            UnkData1 = s.SerializeArray<byte>(UnkData1, 76 - 24 - 4, name: nameof(UnkData1));
            if (UnkDataBlocks == null) UnkDataBlocks = new byte[OffsetsCount][];
            for (int i = 0; i < OffsetsCount; i++) {
                UnkDataBlocks[i] = s.SerializeArray<byte>(UnkDataBlocks[i], 64, name: $"{nameof(UnkDataBlocks)}[{i}]");
            }

            Objects = s.SerializeObjectArray<GBARRR_Object>(Objects, ObjectsCount, name: nameof(Objects));
            s.Serialize<uint>(0, "Padding");
        }
    }
}