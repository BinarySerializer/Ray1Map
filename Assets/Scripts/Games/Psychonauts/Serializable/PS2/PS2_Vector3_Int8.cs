using BinarySerializer;

namespace Ray1Map.Psychonauts
{
    public class PS2_Vector3_Int8 : BinarySerializable
    {
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Z { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<byte>(X, name: nameof(X));
            Y = s.Serialize<byte>(Y, name: nameof(Y));
            Z = s.Serialize<byte>(Z, name: nameof(Z));
        }
    }
}