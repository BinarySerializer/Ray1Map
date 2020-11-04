namespace R1Engine
{
    public class GBAIsometric_Spyro_UnkStruct1 : R1Serializable
    {
        public Pointer Pointer_00 { get; set; } // Leads to an array of byte structs consisting of 2 ushorts
        public GBAIsometric_Spyro_DataBlockIndex Index1 { get; set; } // Image data
        public GBAIsometric_Spyro_DataBlockIndex Index2 { get; set; } // 12 bytes + 4 byte-structs
        public GBAIsometric_Spyro_DataBlockIndex Index3 { get; set; } // 16-byte structs, same length as previous structs

        public override void SerializeImpl(SerializerObject s)
        {
            Pointer_00 = s.SerializePointer(Pointer_00, name: nameof(Pointer_00));
            Index1 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index1, name: nameof(Index1));
            Index2 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index2, name: nameof(Index2));
            Index3 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index3, name: nameof(Index3));
            s.Serialize<ushort>(default, name: "Padding");
        }
    }
}