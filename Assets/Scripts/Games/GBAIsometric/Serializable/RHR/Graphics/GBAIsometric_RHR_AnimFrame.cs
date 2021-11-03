using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_RHR_AnimFrame : BinarySerializable
    {
        public ushort PatternIndex { get; set; }
        public ushort TileIndicesIndex { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            PatternIndex = s.Serialize<ushort>(PatternIndex, name: nameof(PatternIndex));
            TileIndicesIndex = s.Serialize<ushort>(TileIndicesIndex, name: nameof(TileIndicesIndex));
        }
    }
}