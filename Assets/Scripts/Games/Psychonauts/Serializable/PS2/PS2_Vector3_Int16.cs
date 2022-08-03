using BinarySerializer;

namespace Ray1Map.Psychonauts
{
    public class PS2_Vector3_Int16 : BinarySerializable
    {
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public ushort Z { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<ushort>(X, name: nameof(X));
            Y = s.Serialize<ushort>(Y, name: nameof(Y));
            Z = s.Serialize<ushort>(Z, name: nameof(Z));
        }
    }
}