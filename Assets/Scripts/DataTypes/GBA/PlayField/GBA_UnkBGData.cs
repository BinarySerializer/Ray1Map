namespace R1Engine
{
    public class GBA_UnkBGData : GBA_BaseBlock
    {
        public ushort Count1 { get; set; }
        public ushort Count2 { get; set; }
        public MapTile[] Data1 { get; set; }
        public MapTile[] Data2 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Count1 = s.Serialize<ushort>(Count1, name: nameof(Count1));
            Count2 = s.Serialize<ushort>(Count2, name: nameof(Count2));
            Data1 = s.SerializeObjectArray<MapTile>(Data1, Count1, name: nameof(Data1));
            Data2 = s.SerializeObjectArray<MapTile>(Data2, Count2, name: nameof(Data2));
        }
    }
}