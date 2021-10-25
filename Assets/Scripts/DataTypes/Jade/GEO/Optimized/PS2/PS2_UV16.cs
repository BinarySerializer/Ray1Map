using BinarySerializer;

namespace R1Engine.Jade
{
    /// <summary>
    /// A texture coordinate with 16-bit values used in level geometry.
    /// </summary>
    public class PS2_UV16 : BinarySerializable
    {
        public short U { get; set; }
        public short V { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            U = s.Serialize<short>(U, name: nameof(U));
            V = s.Serialize<short>(V, name: nameof(V));
        }
    }
}