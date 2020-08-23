using System.Collections.Generic;
using static R1Engine.GBA_AnimationLayer;

namespace R1Engine
{
    public class GBA_BatmanVengeance_AnimationLayer : R1Serializable {
        #region Data
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public ushort ImageIndex { get; set; }

		#endregion

		#region Parsed
        public int XSize { get; set; } = 1;
        public int YSize { get; set; } = 1;
        public int PaletteIndex { get; set; } = 0;
        public bool IsFlippedHorizontally { get; set; } = false;
        public bool IsFlippedVertically { get; set; } = false;
        public Shape SpriteShape { get; set; }
        public int SpriteSize { get; set; }
        #endregion

        #region Public Methods

        public override void SerializeImpl(SerializerObject s) {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            ImageIndex = s.Serialize<ushort>(ImageIndex, name: nameof(ImageIndex));


            IsFlippedHorizontally = BitHelpers.ExtractBits(Byte_00, 1, 6) == 1;
            IsFlippedVertically = BitHelpers.ExtractBits(Byte_00, 1, 7) == 1;
            SpriteShape = (Shape)BitHelpers.ExtractBits(Byte_01, 2, 2);
            SpriteSize = BitHelpers.ExtractBits(Byte_01, 2, 0);


            // Calculate size
            XSize = 1;
            YSize = 1;
            switch (SpriteShape) {
                case Shape.Square:
                    XSize = 1 << SpriteSize;
                    YSize = XSize;
                    break;
                case Shape.Wide:
                    switch (SpriteSize) {
                        case 0: XSize = 2; YSize = 1; break;
                        case 1: XSize = 4; YSize = 1; break;
                        case 2: XSize = 4; YSize = 2; break;
                        case 3: XSize = 8; YSize = 4; break;
                    }
                    break;
                case Shape.Tall:
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