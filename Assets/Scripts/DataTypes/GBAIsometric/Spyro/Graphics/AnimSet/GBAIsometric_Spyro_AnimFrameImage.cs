namespace R1Engine
{
    public class GBAIsometric_Spyro_AnimFrameImage : R1Serializable
    {
        public ushort TileIndex { get; set; }
        public byte Width { get; set; } // In pixels
        public byte Height { get; set; }
        public byte UnkFlags { get; set; }
        public bool HasPatterns { get; set; }
        public byte NumPatterns { get; set; }
        public ushort PatternsOffset { get; set; }
        public byte NumTiles { get; set; }
        public byte[] Bytes_09 { get; set; }
        // Pattern
        public byte UnkPatternValue { get; set; }
        public byte PalIndex { get; set; }
        public byte SpriteSize { get; set; }
        public GBAIsometric_Spyro_AnimPattern.Shape SpriteShape { get; set; }

        public GBAIsometric_Spyro_AnimPattern[] Patterns { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileIndex = s.Serialize<ushort>(TileIndex, name: nameof(TileIndex));
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            Height = s.Serialize<byte>(Height, name: nameof(Height));
            s.SerializeBitValues<byte>(bitFunc => {
                UnkFlags = (byte)bitFunc(UnkFlags, 7, name: nameof(UnkFlags));
                HasPatterns = bitFunc(HasPatterns ? 1 : 0, 1, name: nameof(HasPatterns)) == 1;
            });
            NumPatterns = s.Serialize<byte>(NumPatterns, name: nameof(NumPatterns));
            if (HasPatterns) {
                PatternsOffset = s.Serialize<ushort>(PatternsOffset, name: nameof(PatternsOffset));
            } else {
                s.SerializeBitValues<ushort>(bitFunc => {
                    SpriteSize = (byte)bitFunc(SpriteSize, 2, name: nameof(SpriteSize));
                    SpriteShape = (GBAIsometric_Spyro_AnimPattern.Shape)bitFunc((int)SpriteShape, 2, name: nameof(SpriteShape));
                    PalIndex = (byte)bitFunc(PalIndex, 4, name: nameof(PalIndex));
                    UnkPatternValue = (byte)bitFunc(UnkPatternValue, 8, name: nameof(UnkPatternValue));
                });
            }
            NumTiles = s.Serialize<byte>(NumTiles, name: nameof(NumTiles));
            Bytes_09 = s.SerializeArray<byte>(Bytes_09, 7, name: nameof(Bytes_09));

            if (HasPatterns) {
                s.DoAt(Offset + 4 * PatternsOffset, () => {
                    Patterns = s.SerializeObjectArray<GBAIsometric_Spyro_AnimPattern>(Patterns, NumPatterns, name: nameof(Patterns));
                });
            }
        }
    }
}