namespace R1Engine
{
    public class GBAIsometric_Spyro_UnkStruct2 : R1Serializable
    {
        public uint ID { get; set; }
        public Pointer Pointer_04 { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex Index1 { get; set; } // 2D map of 2 byte structs
        public GBAIsometric_Spyro_DataBlockIndex Index2 { get; set; } // Image data?
        public GBAIsometric_Spyro_DataBlockIndex Index3 { get; set; } // 32 bytes?

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<uint>(ID, name: nameof(ID));
            Pointer_04 = s.SerializePointer(Pointer_04, name: nameof(Pointer_04));
            Index1 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index1, name: nameof(Index1));
            Index2 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index2, name: nameof(Index2));
            Index3 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index3, name: nameof(Index3));
            s.Serialize<ushort>(default, name: "Padding");
        }
    }
}