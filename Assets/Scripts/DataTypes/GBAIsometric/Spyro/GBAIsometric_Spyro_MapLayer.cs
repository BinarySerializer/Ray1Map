namespace R1Engine
{
    public class GBAIsometric_Spyro_MapLayer : R1Serializable
    {
        public GBAIsometric_Spyro_DataBlockIndex Index0 { get; set; } // 2D map of 1 byte structs
        public GBAIsometric_Spyro_DataBlockIndex Index1 { get; set; } // ushorts
        public GBAIsometric_Spyro_DataBlockIndex Index2 { get; set; } // ?

        public byte Byte_0C { get; set; }
        public byte Byte_0D { get; set; }
        public byte Byte_0E { get; set; }
        public byte Byte_0F { get; set; }

        public int Int_10 { get; set; }
        public int Int_14 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Index0 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index0, x => x.HasPadding = true, name: nameof(Index0));
            Index1 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index1, x => x.HasPadding = true, name: nameof(Index1));
            Index2 = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(Index2, x => x.HasPadding = true, name: nameof(Index2));
            Byte_0C = s.Serialize<byte>(Byte_0C, name: nameof(Byte_0C));
            Byte_0D = s.Serialize<byte>(Byte_0D, name: nameof(Byte_0D));
            Byte_0E = s.Serialize<byte>(Byte_0E, name: nameof(Byte_0E));
            Byte_0F = s.Serialize<byte>(Byte_0F, name: nameof(Byte_0F));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));
        }
    }
}