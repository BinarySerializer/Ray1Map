using BinarySerializer;

namespace R1Engine
{
    public class LUDI_UncompressedBlock<T> : LUDI_BaseBlock where T : BinarySerializable, new() {
        public T Value { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            Value = s.SerializeObject<T>(Value, name: nameof(Value));
        }
    }
}