namespace R1Engine
{
    public class GBA_TileKitBlock : GBA_BaseBlock {
        public byte Unk_00 { get; set; }
        public byte Unk_01 { get; set; }
        public byte TileIndicesCount { get; set; }
        public byte Unk_03 { get; set; }
        public ushort[] TileIndices { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            Unk_00 = s.Serialize<byte>(Unk_00, name: nameof(Unk_00));
            Unk_01 = s.Serialize<byte>(Unk_01, name: nameof(Unk_01));
            TileIndicesCount = s.Serialize<byte>(TileIndicesCount, name: nameof(TileIndicesCount));
            Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));
            TileIndices = s.SerializeArray<ushort>(TileIndices, TileIndicesCount, name: nameof(TileIndices));
        }
    }
}