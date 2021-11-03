using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_AnimatedTileKit : GBA_BaseBlock {
        public byte Flags { get; set; }
        public byte AnimationSpeed { get; set; }
        public byte TileIndicesCount { get; set; }
        public byte TilesStep { get; set; }
        public ushort[] TileIndices { get; set; }

        // Parsed
        public bool Is8Bpp => (Flags & 0x80) == 0x80;
        public int NumFrames => Flags & 0xF;

        public override void SerializeBlock(SerializerObject s) {
            Flags = s.Serialize<byte>(Flags, name: nameof(Flags));
            AnimationSpeed = s.Serialize<byte>(AnimationSpeed, name: nameof(AnimationSpeed));
            TileIndicesCount = s.Serialize<byte>(TileIndicesCount, name: nameof(TileIndicesCount));
            TilesStep = s.Serialize<byte>(TilesStep, name: nameof(TilesStep));
            TileIndices = s.SerializeArray<ushort>(TileIndices, TileIndicesCount, name: nameof(TileIndices));
        }
    }
}