using BinarySerializer;

namespace R1Engine.Jade
{
    public class COL_ColMap : Jade_File 
    {
        public byte CobsCount { get; set; }
        public byte Byte_01 { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }
        public Jade_Reference<COL_Cob>[] Cobs { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            if (FileSize == 4)
            {
                CobsCount = 1;
                Byte_01 = 0xFF;
                Byte_02 = 0;
                Byte_03 = 0;
            }
            else
            {
                CobsCount = s.Serialize<byte>(CobsCount, name: nameof(CobsCount));
                Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
                Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
                Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            }

            Cobs = s.SerializeObjectArray<Jade_Reference<COL_Cob>>(Cobs, CobsCount, name: nameof(Cobs))?.Resolve();
        }
    }
}