namespace R1Engine
{
    public class GBA_AnimationLayer : R1Serializable {
        public sbyte YPosition { get; set; }
        public byte Flags0 { get; set; }
        public sbyte XPosition { get; set; }
        public byte LayerSize { get; set; }
        public ushort UShort_04 { get; set; }

        // Parsed
        public short ImageIndex { get; set; }
        public int PaletteIndex { get; set; }
        public int XSize { get; set; }
        public int YSize { get; set; }
        public bool IsVisible { get; set; }
        public bool IsRotated { get; set; }
        public bool IsFlippedHorizontally { get; set; }
        public bool IsFlippedVertically { get; set; }


        public override void SerializeImpl(SerializerObject s) {
            YPosition = s.Serialize<sbyte>(YPosition, name: nameof(YPosition));
            Flags0 = s.Serialize<byte>(Flags0, name: nameof(Flags0));
            XPosition = s.Serialize<sbyte>(XPosition, name: nameof(XPosition));
            LayerSize = s.Serialize<byte>(LayerSize, name: nameof(LayerSize));
            UShort_04 = s.Serialize<ushort>(UShort_04, name: nameof(UShort_04));

            // Parse
            ImageIndex = (short)BitHelpers.ExtractBits(UShort_04, 11, 0);
            PaletteIndex = BitHelpers.ExtractBits(UShort_04, 4, 12);
            IsFlippedHorizontally = BitHelpers.ExtractBits(LayerSize, 1, 4) == 1;

            /*XSize = 1 + BitHelpers.ExtractBits(LayerSize, 4, 0);
            YSize = 1 + BitHelpers.ExtractBits(LayerSize, 2, 6);*/
            XSize = 1;
            YSize = 1;
            int Size0 = BitHelpers.ExtractBits(Flags0, 2, 6);
            int Size1 = BitHelpers.ExtractBits(LayerSize, 2, 6);
            int calc = (Size0 * 4) + Size1;
            switch (calc) {
                case 0: break;
                case 1: XSize = 2; YSize = 2; break;
                case 2: XSize = 4; YSize = 4; break;
                case 3: XSize = 8; YSize = 8; break;

                case 4: XSize = 2; YSize = 1; break;
                case 5: XSize = 4; YSize = 1; break;
                case 6: XSize = 4; YSize = 2; break;
                case 7: XSize = 8; YSize = 4; break;

                case 8: XSize = 1; YSize = 2; break;
                case 9: XSize = 1; YSize = 4; break;
                case 10: XSize = 2; YSize = 4; break;
                case 11: XSize = 4; YSize = 8; break;
            }

            IsVisible = BitHelpers.ExtractBits(Flags0, 1, 2) == 1;
            IsRotated = BitHelpers.ExtractBits(Flags0, 2, 0) == 3; // 2 bits? so 2 different flags
            IsFlippedVertically = BitHelpers.ExtractBits(LayerSize, 1, 5) == 1;
            //IsFlippedVertically = BitHelpers.ExtractBits(Flags0, 1, 4) == 1;

        }
    }
}