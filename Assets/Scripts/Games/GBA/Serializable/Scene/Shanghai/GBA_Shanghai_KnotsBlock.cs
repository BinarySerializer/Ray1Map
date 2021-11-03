using BinarySerializer;
using Ray1Map.GBC;

namespace Ray1Map.GBA
{
    public class GBA_Shanghai_KnotsBlock : BinarySerializable
    {
        public long Length { get; set; } // Set before serializing

        public GBC_Knot[] Knots { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Knots = s.SerializeObjectArray<GBC_Knot>(Knots, Length, name: nameof(Knots));
        }
    }
}