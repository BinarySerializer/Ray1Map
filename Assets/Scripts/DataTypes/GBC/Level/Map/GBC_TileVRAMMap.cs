namespace R1Engine
{
    public class GBC_TileVRAMMap : R1Serializable {
        public byte TileCount { get; set; }
        public ushort TileOffset { get; set; } // TileIndex = TileOffset / 16
        public ushort VRAMPointer { get; set; }


        public override void SerializeImpl(SerializerObject s) {
            TileCount = s.Serialize<byte>(TileCount, name: nameof(TileCount));
            TileOffset = s.Serialize<ushort>(TileOffset, name: nameof(TileOffset));
            VRAMPointer = s.Serialize<ushort>(VRAMPointer, name: nameof(VRAMPointer));
        }
    }
}