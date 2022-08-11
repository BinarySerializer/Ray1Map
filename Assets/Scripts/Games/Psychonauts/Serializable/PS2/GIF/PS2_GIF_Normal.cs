using BinarySerializer;

namespace Ray1Map.Psychonauts
{
    public class PS2_GIF_Normal : BinarySerializable
    {
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Z { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<byte>(X, name: nameof(X));
            s.Align(4, Offset);
            Y = s.Serialize<byte>(Y, name: nameof(Y));
            s.Align(4, Offset);
            Z = s.Serialize<byte>(Z, name: nameof(Z));
            s.Align(4, Offset);
            s.SerializePadding(4);
        }

        public float XFloat => X - 0x80;
        public float YFloat => Y - 0x80;
        public float ZFloat => Z - 0x80;
    }
}