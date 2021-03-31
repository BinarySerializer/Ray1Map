using BinarySerializer;

namespace R1Engine
{
    public class GBARRR_GraphicsTableEntry : BinarySerializable
    {
        public uint Key { get; set; }
        public uint Value { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Key = s.Serialize<uint>(Key, name: nameof(Key));
            Value = s.Serialize<uint>(Value, name: nameof(Value));    
        }
    }
}