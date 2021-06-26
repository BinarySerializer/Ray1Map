using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_MapSector : BinarySerializable
    {
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<ushort>(X, name: nameof(X));
            Y = s.Serialize<ushort>(Y, name: nameof(Y));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
        }
    }
}