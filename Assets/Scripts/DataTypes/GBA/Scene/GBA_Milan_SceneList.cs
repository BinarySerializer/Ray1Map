namespace R1Engine
{
    public class GBA_Milan_SceneList : GBA_BaseBlock
    {
        public byte[] Data_0 { get; set; }
        public byte[] Data_1 { get; set; }
        public byte[] Data_2 { get; set; }
        public byte[] Data_3 { get; set; }
        public byte[] Data_4 { get; set; }
        public byte[] Data_5 { get; set; }

        // Parsed from offsets
        public GBA_Milan_Scene Scene { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Data_0 = s.DoAt(ShanghaiOffsetTable.GetPointer(0), () => s.SerializeArray<byte>(Data_0, ShanghaiOffsetTable.GetPointer(1) - ShanghaiOffsetTable.GetPointer(0), name: nameof(Data_0)));
            Data_1 = s.DoAt(ShanghaiOffsetTable.GetPointer(1), () => s.SerializeArray<byte>(Data_1, ShanghaiOffsetTable.GetPointer(2) - ShanghaiOffsetTable.GetPointer(1), name: nameof(Data_1)));
            Data_2 = s.DoAt(ShanghaiOffsetTable.GetPointer(2), () => s.SerializeArray<byte>(Data_2, ShanghaiOffsetTable.GetPointer(3) - ShanghaiOffsetTable.GetPointer(2), name: nameof(Data_2)));
            Data_3 = s.DoAt(ShanghaiOffsetTable.GetPointer(3), () => s.SerializeArray<byte>(Data_3, ShanghaiOffsetTable.GetPointer(4) - ShanghaiOffsetTable.GetPointer(3), name: nameof(Data_3)));
            Data_4 = s.DoAt(ShanghaiOffsetTable.GetPointer(4), () => s.SerializeArray<byte>(Data_4, ShanghaiOffsetTable.GetPointer(5) - ShanghaiOffsetTable.GetPointer(4), name: nameof(Data_4)));
            Data_5 = s.DoAt(ShanghaiOffsetTable.GetPointer(5), () => s.SerializeArray<byte>(Data_5, (Offset + BlockSize) - ShanghaiOffsetTable.GetPointer(5), name: nameof(Data_5)));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            Scene = s.DoAt(OffsetTable.GetPointer(s.GameSettings.Level), () => s.SerializeObject<GBA_Milan_Scene>(Scene, name: nameof(Scene)));
        }

        public override long GetShanghaiOffsetTableLength => 6;
    }
}