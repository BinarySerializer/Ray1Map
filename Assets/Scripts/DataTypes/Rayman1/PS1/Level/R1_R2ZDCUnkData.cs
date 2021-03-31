using BinarySerializer;

namespace R1Engine
{
    public class R1_R2ZDCUnkData : BinarySerializable
    {
        public ushort Data1 { get; set; }
        public byte Data2 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeBitValues<ushort>(bitFunc =>
            {
                Data1 = (ushort)bitFunc(Data1, 12, name: nameof(Data1));
                Data2 = (byte)bitFunc(Data2, 4, name: nameof(Data2));
            });
        }
    }
}