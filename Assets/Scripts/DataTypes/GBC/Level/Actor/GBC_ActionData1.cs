using BinarySerializer;

namespace R1Engine
{
    public class GBC_ActionData1 : BinarySerializable {
        public byte Length { get; set; }
        public byte[] Unknown1Data { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Length = s.Serialize<byte>(Length, name: nameof(Length));
            Unknown1Data = s.SerializeArray<byte>(Unknown1Data, Length, name: nameof(Unknown1Data));
        }
    }
}