using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_BatmanVengeance_AnimationChannel : BinarySerializable {
        #region Data
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public ushort ImageIndex { get; set; }

		#endregion

		#region Parsed
        public int XSize { get; set; } = 1;
        public int YSize { get; set; } = 1;
        public int PaletteIndex { get; set; }
        public bool IsFlippedHorizontally { get; set; }
        public bool IsFlippedVertically { get; set; }
        public GBA_AnimationChannel.Shape SpriteShape { get; set; }
        public int SpriteSize { get; set; }
        #endregion

        #region Public Methods

        public override void SerializeImpl(SerializerObject s) 
        {
            if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_CrouchingTiger) {
                s.DoBits<ushort>(b => {
                    YPosition = (short)b.SerializeBits<int>(YPosition, 8, name: nameof(YPosition));
                    if (YPosition >= 128) YPosition -= 256;
                    b.SerializeBits<int>(default, 6, name: "Unknown");
                    SpriteShape = (GBA_AnimationChannel.Shape)b.SerializeBits<int>((int)SpriteShape, 2, name: nameof(SpriteShape));
                });
                s.DoBits<ushort>(b => {
                    XPosition = (short)b.SerializeBits<int>(XPosition, 8, name: nameof(XPosition));
                    if (XPosition >= 128) XPosition -= 256;
                    b.SerializeBits<int>(default, 4, name: "Unknown");
                    IsFlippedHorizontally = b.SerializeBits<int>(IsFlippedHorizontally ? 1 : 0, 1, name: nameof(IsFlippedHorizontally)) == 1;
                    IsFlippedVertically = b.SerializeBits<int>(IsFlippedVertically ? 1 : 0, 1, name: nameof(IsFlippedHorizontally)) == 1;
                    SpriteSize = b.SerializeBits<int>(SpriteSize, 2, name: nameof(SpriteSize));
                });
                s.DoBits<ushort>(b => {
                    b.SerializeBits<int>(default, 4, name: "Unknown");
                    b.SerializeBits<int>(default, 4, name: "Unknown");
                    b.SerializeBits<int>(default, 4, name: "Unknown");
                    PaletteIndex = b.SerializeBits<int>(PaletteIndex, 4, name: nameof(PaletteIndex));
                });
                ImageIndex = s.Serialize<ushort>(ImageIndex, name: nameof(ImageIndex));
            } else {
                s.DoBits<byte>(b => {
                    b.SerializeBits<int>(default, 6, name: "Padding");
                    IsFlippedHorizontally = b.SerializeBits<int>(IsFlippedHorizontally ? 1 : 0, 1, name: nameof(IsFlippedHorizontally)) == 1;
                    IsFlippedVertically = b.SerializeBits<int>(IsFlippedVertically ? 1 : 0, 1, name: nameof(IsFlippedVertically)) == 1;
                });
                s.DoBits<byte>(b => {
                    SpriteSize = b.SerializeBits<int>(SpriteSize, 2, name: nameof(SpriteSize));
                    SpriteShape = (GBA_AnimationChannel.Shape)b.SerializeBits<int>((int)SpriteShape, 2, name: nameof(SpriteShape));
                    PaletteIndex = b.SerializeBits<int>(PaletteIndex, 4, name: nameof(PaletteIndex));
                });

                YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
                XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
                ImageIndex = s.Serialize<ushort>(ImageIndex, name: nameof(ImageIndex));
            }

            // Calculate size
            XSize = 1;
            YSize = 1;
            switch (SpriteShape) {
                case GBA_AnimationChannel.Shape.Square:
                    XSize = 1 << SpriteSize;
                    YSize = XSize;
                    break;
                case GBA_AnimationChannel.Shape.Wide:
                    switch (SpriteSize) {
                        case 0: XSize = 2; YSize = 1; break;
                        case 1: XSize = 4; YSize = 1; break;
                        case 2: XSize = 4; YSize = 2; break;
                        case 3: XSize = 8; YSize = 4; break;
                    }
                    break;
                case GBA_AnimationChannel.Shape.Tall:
                    switch (SpriteSize) {
                        case 0: XSize = 1; YSize = 2; break;
                        case 1: XSize = 1; YSize = 4; break;
                        case 2: XSize = 2; YSize = 4; break;
                        case 3: XSize = 4; YSize = 8; break;
                    }
                    break;
            }
        }

        #endregion
    }
}