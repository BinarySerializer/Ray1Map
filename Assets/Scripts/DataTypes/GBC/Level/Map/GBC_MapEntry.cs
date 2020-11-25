namespace R1Engine
{
    public class GBC_MapEntry : R1Serializable {
        public byte Byte_00 { get; set; }
        public ushort TileOffset { get; set; } // TileIndex = TileOffset / 16
        public GBC_Pointer GBCPointer { get; set; } // TODO: Is this a ushort bit value or a pointer?
        //public ushort UShort_03 { get; set; }


        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            TileOffset = s.Serialize<ushort>(TileOffset, name: nameof(TileOffset));
            GBCPointer = s.SerializeObject<GBC_Pointer>(GBCPointer, onPreSerialize: p => p.HasMemoryBankValue = false, name: nameof(GBCPointer));
            //UShort_03 = s.Serialize<ushort>(UShort_03, name: nameof(UShort_03));
        }
    }
}