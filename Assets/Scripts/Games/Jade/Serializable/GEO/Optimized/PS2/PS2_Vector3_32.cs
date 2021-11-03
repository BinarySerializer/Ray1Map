using BinarySerializer;

namespace Ray1Map.Jade
{
    /// <summary>
    /// A vector with 32-bit values
    /// </summary>
    public class PS2_Vector3_32 : BinarySerializable
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<float>(X, name: nameof(X));
            Y = s.Serialize<float>(Y, name: nameof(Y));
            Z = s.Serialize<float>(Z, name: nameof(Z));
		}
    }
}