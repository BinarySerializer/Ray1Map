using System;
using System.Numerics;

namespace R1Engine
{
    // Matches https://www.coranac.com/tonc/text/regobj.htm
    public class GBA_AnimationLayer : R1Serializable {
        public ushort Attr0 { get; set; }
        public ushort Attr1 { get; set; }
        public ushort Attr2 { get; set; }

        // Parsed
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public short ImageIndex { get; set; }
        public int PaletteIndex { get; set; }
        public Shape SpriteShape { get; set; }
        public int SpriteSize { get; set; }
        public int XSize { get; set; }
        public int YSize { get; set; }
        public int Priority { get; set; }
        public bool IsFlippedHorizontally { get; set; }
        public bool IsFlippedVertically { get; set; }
        public AffineObjectMode TransformMode { get; set; }
        public GfxMode RenderMode { get; set; }
        public bool Mosaic { get; set; }
        public ColorMode Color { get; set; }
        public int AffineMatrixIndex { get; set; }


        public override void SerializeImpl(SerializerObject s) {
            Attr0 = s.Serialize<ushort>(Attr0, name: nameof(Attr0));
            Attr1 = s.Serialize<ushort>(Attr1, name: nameof(Attr1));
            Attr2 = s.Serialize<ushort>(Attr2, name: nameof(Attr2));

            // Parse

            // Attr0
            YPosition = (short)BitHelpers.ExtractBits(Attr0, 8, 0);
            if (YPosition >= 128) YPosition -= 256;
            TransformMode = (AffineObjectMode)BitHelpers.ExtractBits(Attr0, 2, 8);
            RenderMode = (GfxMode)BitHelpers.ExtractBits(Attr0, 2, 10);
            //Controller.print(BitHelpers.ExtractBits(Attr0, 2, 10));
            Mosaic = BitHelpers.ExtractBits(Attr0, 1, 12) == 1;
            Color = (ColorMode)BitHelpers.ExtractBits(Attr0, 1, 13);
            SpriteShape = (Shape)BitHelpers.ExtractBits(Attr0, 2, 14);

            // Attr1
            XPosition = (short)BitHelpers.ExtractBits(Attr1, 9, 0);
            if (XPosition >= 256) XPosition -= 512;
            if (TransformMode == AffineObjectMode.Affine || TransformMode == AffineObjectMode.AffineDouble) {
                AffineMatrixIndex = BitHelpers.ExtractBits(Attr1, 5, 9);
            } else {
                IsFlippedHorizontally = BitHelpers.ExtractBits(Attr1, 1, 12) == 1;
                IsFlippedVertically = BitHelpers.ExtractBits(Attr1, 1, 13) == 1;
            }
            SpriteSize = BitHelpers.ExtractBits(Attr1, 2, 14);

            ImageIndex = (short)BitHelpers.ExtractBits(Attr2, 10, 0);
            Priority = BitHelpers.ExtractBits(Attr2, 2, 10);
            PaletteIndex = BitHelpers.ExtractBits(Attr2, 3, 12); // another flag at byte 0xF?

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

        public enum AffineObjectMode {
            Regular = 0,
            Affine,
            Hide,
            AffineDouble
        }

        public enum GfxMode {
            Regular = 0,
            Blend,
            Window
        }

        public enum ColorMode {
            Color4bpp,
            Color8bpp
        }

        public enum Shape {
            Square,
            Wide,
            Tall
        }

        public float GetRotation(GBA_Animation anim, GBA_SpriteGroup spriteGroup) {
            if (TransformMode == AffineObjectMode.Affine || TransformMode == AffineObjectMode.AffineDouble) {
                if (spriteGroup.Matrices.ContainsKey(anim.AffineMatricesIndex)) {
                    var matrices = spriteGroup.Matrices[anim.AffineMatricesIndex].Matrices;
                    if (matrices.Length > AffineMatrixIndex) {
                        var m = matrices[AffineMatrixIndex];
                        Vector4 v = m.ToFloat();
                        // TODO: using this Vector4 of Pa, Pb, Pc, Pd: get the rotation and scale values
                        // Resources: 
                        // https://wiki.nycresistor.com/wiki/GB101:Affine_Sprites
                        // https://www.coranac.com/tonc/text/affine.htm
                        // https://www.coranac.com/tonc/text/affobj.htm
                    }
                }
            }
            return 0f;
        }
    }
}