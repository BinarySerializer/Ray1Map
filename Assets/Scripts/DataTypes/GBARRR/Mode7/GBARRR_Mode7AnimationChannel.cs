using System;
using System.Numerics;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    // Matches https://www.coranac.com/tonc/text/regobj.htm
    public class GBARRR_Mode7AnimationChannel : BinarySerializable {
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
        public GBA_ColorMode Color { get; set; }
        public int AffineMatrixIndex { get; set; }

        public Type ChannelType { get; set; } = Type.Sprite;
        public sbyte Box_MinX { get; set; }
        public sbyte Box_MaxX { get; set; }
        public sbyte Box_MinY { get; set; }
        public sbyte Box_MaxY { get; set; }
        public int Unknown8 { get; set; }
        public int UnknownC { get; set; }

        public bool IsEndAttribute => Attr0 == 0x7F7F;

        public override void SerializeImpl(SerializerObject s) {
            Attr0 = s.Serialize<ushort>(Attr0, name: nameof(Attr0));
            if (IsEndAttribute) return;

            // Parse
            Attr1 = s.Serialize<ushort>(Attr1, name: nameof(Attr1));
            Attr2 = s.Serialize<ushort>(Attr2, name: nameof(Attr2));
            
            // Attr0
            YPosition = (short)BitHelpers.ExtractBits(Attr0, 8, 0);
            if (YPosition >= 128) YPosition -= 256;


            TransformMode = (AffineObjectMode)BitHelpers.ExtractBits(Attr0, 2, 8);
            RenderMode = (GfxMode)BitHelpers.ExtractBits(Attr0, 2, 10);
            Mosaic = BitHelpers.ExtractBits(Attr0, 1, 12) == 1;
            Color = (GBA_ColorMode)BitHelpers.ExtractBits(Attr0, 1, 13);
            SpriteShape = (Shape)BitHelpers.ExtractBits(Attr0, 2, 14);

            // Attr1
            XPosition = (short)BitHelpers.ExtractBits(Attr1, 9, 0);
            bool bit9 = false, bit10 = false, bit11 = false;
            //if (XPosition >= 256) XPosition -= 512;
            if (XPosition >= 128) XPosition -= 256;
            if (TransformMode == AffineObjectMode.Affine || TransformMode == AffineObjectMode.AffineDouble) {
                AffineMatrixIndex = BitHelpers.ExtractBits(Attr1, 5, 9);
            } else {
                bit9 = BitHelpers.ExtractBits(Attr1, 1, 9) == 1;
                bit10 = BitHelpers.ExtractBits(Attr1, 1, 10) == 1;
                bit11 = BitHelpers.ExtractBits(Attr1, 1, 11) == 1;
                IsFlippedHorizontally = BitHelpers.ExtractBits(Attr1, 1, 12) == 1;
                IsFlippedVertically = BitHelpers.ExtractBits(Attr1, 1, 13) == 1;
            }
            SpriteSize = BitHelpers.ExtractBits(Attr1, 2, 14);

            ImageIndex = (short)BitHelpers.ExtractBits(Attr2, 10, 0);
            Priority = BitHelpers.ExtractBits(Attr2, 2, 10);
            PaletteIndex = BitHelpers.ExtractBits(Attr2, 4, 12);

            s.Log($"{nameof(XPosition)}: {XPosition}");
            s.Log($"{nameof(YPosition)}: {YPosition}");
            s.Log($"{nameof(TransformMode)}: {TransformMode}");
            s.Log($"{nameof(RenderMode)}: {RenderMode}");
            s.Log($"{nameof(Color)}: {Color}");
            s.Log($"{nameof(Mosaic)}: {Mosaic}");
            s.Log($"{nameof(SpriteShape)}: {SpriteShape}");
            s.Log($"{nameof(SpriteSize)}: {SpriteSize}");

            s.Log($"{nameof(ImageIndex)}: {ImageIndex}");
            s.Log($"{nameof(Priority)}: {Priority}");
            s.Log($"{nameof(PaletteIndex)}: {PaletteIndex}");
            if (TransformMode == AffineObjectMode.Affine || TransformMode == AffineObjectMode.AffineDouble) {
                s.Log($"{nameof(AffineMatrixIndex)}: {AffineMatrixIndex}");
            } else {
                s.Log($"{nameof(IsFlippedHorizontally)}: {IsFlippedHorizontally}");
                s.Log($"{nameof(IsFlippedVertically)}: {IsFlippedVertically}");

            }
            if (bit9) s.Log("BIT9");
            if (bit10) s.Log("BIT10");
            if (bit11) s.Log("BIT11");

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

        public enum Shape {
            Square,
            Wide,
            Tall
        }
        public enum Type {
            Null,
            Sprite,
            AttackBox,
            VulnerabilityBox,
            Unknown8,
            UnknownC
        }

        public float GetRotation(GBA_Animation anim, GBA_Puppet puppet, int frameIndex) {
            if (TransformMode == AffineObjectMode.Affine || TransformMode == AffineObjectMode.AffineDouble) {
                if (puppet.Matrices.ContainsKey(anim.AffineMatricesIndex)) {
                    var m = puppet.Matrices[anim.AffineMatricesIndex].GetMatrix(AffineMatrixIndex, frameIndex);
                    if (m != null) {
                        var rotation = -Mathf.Atan2(m.Pb / 256f, m.Pa / 256f);
                        return rotation * Mathf.Rad2Deg;

                        // Resources: 
                        // https://stackoverflow.com/questions/45159314/decompose-2d-transformation-matrix
                        // https://wiki.nycresistor.com/wiki/GB101:Affine_Sprites
                        // https://www.coranac.com/tonc/text/affine.htm
                        // https://www.coranac.com/tonc/text/affobj.htm
                    }
                }
            }
            return 0f;
        }

        public UnityEngine.Vector2 GetScale(GBA_Animation anim, GBA_Puppet puppet, int frameIndex) {
            if (TransformMode == AffineObjectMode.Affine || TransformMode == AffineObjectMode.AffineDouble) {
                if (puppet.Matrices.ContainsKey(anim.AffineMatricesIndex)) {
                    var m = puppet.Matrices[anim.AffineMatricesIndex].GetMatrix(AffineMatrixIndex, frameIndex);
                    if (m != null) {
                        var a = m.Pa / 256f;
                        var b = m.Pb / 256f;
                        var c = m.Pc / 256f;
                        var d = m.Pd / 256f;
                        var delta = a * d - b * c;

                        //var rotation = 0f;
                        var scale = UnityEngine.Vector2.zero;
                        //var skew = UnityEngine.Vector2.zero;
                        // Apply the QR-like decomposition.
                        if (a != 0 || b != 0) {
                            var r = Mathf.Sqrt(a * a + b * b);
                            //rotation = b > 0 ? Mathf.Acos(a / r) : -Mathf.Acos(a / r);
                            scale = new UnityEngine.Vector2(r, delta / r);
                            //skew = new UnityEngine.Vector2(Mathf.Atan((a * c + b * d) / (r * r)), 0);
                        } else if (c != 0 || d != 0) {
                            var s = Mathf.Sqrt(c * c + d * d);
                            //rotation =
                            //  Mathf.PI / 2 - (d > 0 ? Mathf.Acos(-c / s) : -Mathf.Acos(c / s));
                            scale = new UnityEngine.Vector2(delta / s, s);
                            //skew = new UnityEngine.Vector2(0, Mathf.Atan((a * c + b * d) / (s * s)));
                        } else {
                            // a = b = c = d = 0
                        }
                        if (scale.x != 0) scale.x = 1f / scale.x;
                        if (scale.y != 0) scale.y = 1f / scale.y;

                        return scale;
                        // Resources: 
                        // https://wiki.nycresistor.com/wiki/GB101:Affine_Sprites
                        // https://www.coranac.com/tonc/text/affine.htm
                        // https://www.coranac.com/tonc/text/affobj.htm
                    }
                }
            }
            return UnityEngine.Vector2.one;
        }
    }
}