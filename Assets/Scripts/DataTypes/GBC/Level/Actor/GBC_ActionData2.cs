using BinarySerializer;

namespace R1Engine
{
    public class GBC_ActionData2 : BinarySerializable {
        public byte[] Unknown2Data { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Unknown2Data = s.SerializeArray<byte>(Unknown2Data, 4, name: nameof(Unknown2Data));
        }
    }
}