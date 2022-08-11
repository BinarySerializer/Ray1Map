using BinarySerializer;

namespace Ray1Map.Psychonauts
{
    public class PS2_GIF_XYZF : BinarySerializable
    {
        public uint X { get; set; }
        public uint Y { get; set; }
        public uint Z { get; set; }
        public uint F { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<uint>(X, name: nameof(X));
            s.Align(4, Offset);
            Y = s.Serialize<uint>(Y, name: nameof(Y));
            s.Align(4, Offset);
            Z = s.Serialize<uint>(Z, name: nameof(Z));
            s.Align(4, Offset);
			F = s.Serialize<uint>(F, name: nameof(F));
            s.Align(4, Offset);
        }

        public float XFloat => X / 8f;
        public float YFloat => Y / 8f;
        public float ZFloat => Z / 8f;
    }
}