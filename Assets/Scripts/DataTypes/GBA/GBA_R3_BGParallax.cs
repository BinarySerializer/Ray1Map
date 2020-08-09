namespace R1Engine
{
    // First  level is at 0x082E7288
    // Second level is at 0x08362544
    // 0x03000e20 has pointer to this struct for the current level during runtime

    // Always 128 bytes long - appears right before BG_0 for a map
    public class GBA_R3_BGParallax : R1Serializable
    {
        public bool Unk_00 { get; set; }

        public byte Unk_01 { get; set; }

        public byte TilemapIndex { get; set; }
        
        public byte Unk_03 { get; set; }

        public byte TileLayersCount { get; set; }
        public byte UnkIndexesCount { get; set; }
        
        public byte[] TileLayerIDs { get; set; }
        public byte[] UnkIndexes { get; set; }

        // More data (offset table?) - part of the same struct?

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk_00 = s.Serialize<bool>(Unk_00, name: nameof(Unk_00));
            Unk_01 = s.Serialize<byte>(Unk_01, name: nameof(Unk_01));
            TilemapIndex = s.Serialize<byte>(TilemapIndex, name: nameof(TilemapIndex));
            Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));
            TileLayersCount = s.Serialize<byte>(TileLayersCount, name: nameof(TileLayersCount));
            UnkIndexesCount = s.Serialize<byte>(UnkIndexesCount, name: nameof(UnkIndexesCount));
            TileLayerIDs = s.SerializeArray<byte>(TileLayerIDs, 4, name: nameof(TileLayerIDs));
            UnkIndexes = s.SerializeArray<byte>(UnkIndexes, 6, name: nameof(UnkIndexes));
        }
    }
}