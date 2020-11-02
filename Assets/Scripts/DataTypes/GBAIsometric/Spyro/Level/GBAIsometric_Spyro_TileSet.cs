namespace R1Engine
{
    public class GBAIsometric_Spyro_TileSet : R1Serializable
    {
        public long BlockSize { get; set; }

        public uint Uint_00 { get; set; }
        public byte[] TileData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Uint_00 = s.Serialize<uint>(Uint_00, name: nameof(Uint_00));
            TileData = s.SerializeArray<byte>(TileData, BlockSize - 4, name: nameof(TileData));
            //s.Log($"Offset: {Uint_00 & 0x3fff}, Length: {TileData.Length / 32}");
        }
    }
}