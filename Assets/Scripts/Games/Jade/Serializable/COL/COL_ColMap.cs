using BinarySerializer;

namespace Ray1Map.Jade
{
    public class COL_ColMap : Jade_File {
        public override bool HasHeaderBFFile => true;
        public override string Export_Extension => "map";

        public byte CobsCount { get; set; }
        public byte Activation { get; set; }
        public byte CustomBits1 { get; set; }
        public byte CustomBits2 { get; set; }
        public Jade_Reference<COL_Cob>[] Cobs { get; set; }

        protected override void SerializeFile(SerializerObject s) 
        {
            if (FileSize - HeaderBFFileSize == 4)
            {
                CobsCount = 1;
                Activation = 0xFF;
                CustomBits1 = 0;
                CustomBits2 = 0;
            }
            else
            {
                CobsCount = s.Serialize<byte>(CobsCount, name: nameof(CobsCount));
                Activation = s.Serialize<byte>(Activation, name: nameof(Activation));
                CustomBits1 = s.Serialize<byte>(CustomBits1, name: nameof(CustomBits1));
                CustomBits2 = s.Serialize<byte>(CustomBits2, name: nameof(CustomBits2));
            }

            Cobs = s.SerializeObjectArray<Jade_Reference<COL_Cob>>(Cobs, CobsCount, name: nameof(Cobs))?.Resolve();
        }
    }
}