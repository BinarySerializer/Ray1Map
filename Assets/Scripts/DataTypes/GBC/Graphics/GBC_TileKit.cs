namespace R1Engine
{
    public class GBC_TileKit : GBC_BaseBlock 
    {
        public uint TilesCount { get; set; }
        public byte[] TileData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);
            SerializeOffsetTable(s);

            // Serialize data
            TilesCount = s.Serialize<uint>(TilesCount, name: nameof(TilesCount));
            TileData = s.SerializeArray<byte>(TileData, TilesCount * 0x40, name: nameof(TileData));
        }
    }
}