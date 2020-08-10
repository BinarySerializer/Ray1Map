namespace R1Engine
{
    public class GBA_R3_LevelBlock : GBA_R3_BaseBlock
    {
        #region Level Data

        public ushort PlayFieldIndex { get; set; }
        public byte Unk_02 { get; set; }
        public byte Unk_03 { get; set; }

        public byte ObjectsCount { get; set; }
        public byte Unk_05 { get; set; }
        public byte Unk_06 { get; set; }
        public byte Unk_07 { get; set; }

        // Might not be the exact same struct, but matches with x and y
        public GBA_R3_Actor StartPosActor { get; set; }

        public GBA_R3_Actor[] Actors { get; set; }

        public byte[] UnkData { get; set; }

        #endregion

        #region Parsed

        public GBA_R3_OffsetTable OffsetTable { get; set; }

        public GBA_R3_PlayField2D PlayField { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize block header
            base.SerializeImpl(s);

            PlayFieldIndex = s.Serialize<ushort>(PlayFieldIndex, name: nameof(PlayFieldIndex));
            Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
            Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));

            ObjectsCount = s.Serialize<byte>(ObjectsCount, name: nameof(ObjectsCount));
            Unk_05 = s.Serialize<byte>(Unk_05, name: nameof(Unk_05));
            Unk_06 = s.Serialize<byte>(Unk_06, name: nameof(Unk_06));
            Unk_07 = s.Serialize<byte>(Unk_07, name: nameof(Unk_07));

            StartPosActor = s.SerializeObject<GBA_R3_Actor>(StartPosActor, name: nameof(StartPosActor));

            Actors = s.SerializeObjectArray<GBA_R3_Actor>(Actors, ObjectsCount, name: nameof(Actors));

            // TODO: What is this data?
            UnkData = s.SerializeArray<byte>(UnkData, BlockSize - (s.CurrentPointer - (Offset + 4)), name: nameof(UnkData));

            s.Align();

            // Serialize offset table
            OffsetTable = s.SerializeObject<GBA_R3_OffsetTable>(OffsetTable, name: nameof(OffsetTable));

            PlayField = s.DoAt(OffsetTable.GetPointer(PlayFieldIndex, true), () =>  s.SerializeObject<GBA_R3_PlayField2D>(PlayField, name: nameof(PlayField)));
        }

        #endregion
    }
}