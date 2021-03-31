using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_NitroKart_TileAnimation : BinarySerializable
    {
        public ushort FramesCount { get; set; }
        public ushort TilesCount { get; set; }
        public int[] TileIndices { get; set; }
        public GBAVV_NitroKart_TileAnimationFrame[] Frames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
            TilesCount = s.Serialize<ushort>(TilesCount, name: nameof(TilesCount));
            TileIndices = s.SerializeArray<int>(TileIndices, TilesCount, name: nameof(TileIndices));
            Frames = s.SerializeObjectArray<GBAVV_NitroKart_TileAnimationFrame>(Frames, FramesCount, x => x.TilesCount = TilesCount, name: nameof(Frames));
        }
    }
}