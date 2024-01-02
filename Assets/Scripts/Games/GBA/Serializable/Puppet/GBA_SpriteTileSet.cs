using BinarySerializer;
using BinarySerializer.Nintendo.GBA;


namespace Ray1Map.GBA
{
    /// <summary>
    /// Tile map data for sprites on GBA
    /// </summary>
    public class GBA_SpriteTileSet : GBA_BaseBlock
    {
        public bool? IsDataCompressed { get; set; }

        public ushort TileSetLength { get; set; }
        public bool IsCompressed { get; set; }
        public bool Is8Bit { get; set; }
        public byte UnknownTileSet1 { get; set; }
		public byte UnknownTileSet2 { get; set; }

		public byte[] TileSet { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersion <= EngineVersion.GBA_BatmanVengeance) {
                Is8Bit = s.Serialize<bool>(Is8Bit, name: nameof(Is8Bit));
                IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
                TileSetLength = s.Serialize<ushort>(TileSetLength, name: nameof(TileSetLength));
            } else {
                TileSetLength = s.Serialize<ushort>(TileSetLength, name: nameof(TileSetLength));
                if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_R3_20020418_NintendoE3Approval ||
                    s.GetR1Settings().EngineVersion == EngineVersion.GBA_R3_20020301_PreAlpha) {
                    UnknownTileSet1 = s.Serialize<byte>(UnknownTileSet1, name: nameof(UnknownTileSet1));
                    UnknownTileSet2 = s.Serialize<byte>(UnknownTileSet2, name: nameof(UnknownTileSet2));
                } else {
                    IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
                    Is8Bit = s.Serialize<bool>(Is8Bit, name: nameof(Is8Bit));
                }
            }

            var tileLength = s.GetR1Settings().EngineVersion < EngineVersion.GBA_BatmanVengeance && Is8Bit ? 64 : 32;

            if (IsDataCompressed ?? IsCompressed) {
                s.DoEncoded(new BinarySerializer.Nintendo.GBA.LZSSEncoder(), () => TileSet = s.SerializeArray<byte>(TileSet, TileSetLength * tileLength, name: nameof(TileSet)));
                s.Align();
            } else {
                TileSet = s.SerializeArray<byte>(TileSet, TileSetLength * tileLength, name: nameof(TileSet));
            }
        }
    }
}