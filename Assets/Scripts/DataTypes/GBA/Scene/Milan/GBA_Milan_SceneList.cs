namespace R1Engine
{
    public class GBA_Milan_SceneList : GBA_BaseBlock
    {
        public byte[][] Data { get; set; }

        // Parsed from offsets
        public GBA_Milan_Scene Scene { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            if (Data == null)
                Data = new byte[ShanghaiOffsetTable.Length][];

            for (int i = 0; i < Data.Length; i++)
            {
                var next = i == Data.Length - 1 ? Offset + BlockSize : ShanghaiOffsetTable.GetPointer(i + 1);

                Data[i] = s.DoAt(ShanghaiOffsetTable.GetPointer(i), () => s.SerializeArray<byte>(Data[i], next - ShanghaiOffsetTable.GetPointer(i), name: $"{Data}[{i}]"));
            }

            s.Goto(Offset + BlockSize);
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            Scene = s.DoAt(OffsetTable.GetPointer(s.GameSettings.Level), () => s.SerializeObject<GBA_Milan_Scene>(Scene, name: nameof(Scene)));
        }

        public override long GetShanghaiOffsetTableLength => Context.Settings.EngineVersion == EngineVersion.GBA_TheMummy ? 6 : 3;
    }
}