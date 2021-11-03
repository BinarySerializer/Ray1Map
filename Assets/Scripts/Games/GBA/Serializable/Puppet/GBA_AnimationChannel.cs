using System;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBA
{
    // Matches https://www.coranac.com/tonc/text/regobj.htm
    public class GBA_AnimationChannel : BinarySerializable, ICloneable {
        public ushort Attr0 { get; set; }
        public ushort Attr1 { get; set; }
        public ushort Attr2 { get; set; }

        // Parsed
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public ushort ImageIndex { get; set; }
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
        public GBA_ColorMode Color { get; set; }
        public int AffineMatrixIndex { get; set; }

        public Type ChannelType { get; set; } = Type.Sprite;
        public sbyte Box_MinX { get; set; }
        public sbyte Box_MaxX { get; set; }
        public sbyte Box_MinY { get; set; }
        public sbyte Box_MaxY { get; set; }
        public int Unknown8 { get; set; }
        public int UnknownC { get; set; }

        public const ushort BOX_FLAG = 0x3c00;

        public override void SerializeImpl(SerializerObject s) {
            Attr0 = s.Serialize<ushort>(Attr0, name: nameof(Attr0));

            // Parse
            if ((Attr0 & BOX_FLAG) == 0x800) ChannelType = Type.Unknown8;
            if ((Attr0 & BOX_FLAG) == 0xC00) ChannelType = Type.UnknownC;
            if ((Attr0 & BOX_FLAG) == 0x1000) ChannelType = Type.AttackBox;
            if ((Attr0 & BOX_FLAG) == 0x1400) ChannelType = Type.VulnerabilityBox;

            if (ChannelType == Type.Sprite) {
                Attr1 = s.Serialize<ushort>(Attr1, name: nameof(Attr1));
                Attr2 = s.Serialize<ushort>(Attr2, name: nameof(Attr2));
                if (Attr0 == 0 && Attr1 == 0 && Attr2 == 0) {
                    ChannelType = Type.Null;
                    return;
                }
                // Attr0
                YPosition = (short)BitHelpers.ExtractBits(Attr0, 8, 0);
                /*if (s.GetR1Settings().Game == Game.GBA_Rayman3) {
                    if (YPosition >= 96) YPosition -= 256; // Hack. Since usually more of the top sprite is visible, this is more likely.
                } else {
                    if (YPosition >= 128) YPosition -= 256;
                }*/
                if (YPosition >= 128) YPosition -= 256;
                TransformMode = (AffineObjectMode)BitHelpers.ExtractBits(Attr0, 2, 8);
                RenderMode = (GfxMode)BitHelpers.ExtractBits(Attr0, 2, 10);
                //Controller.print(BitHelpers.ExtractBits(Attr0, 2, 10));
                Color = (GBA_ColorMode)BitHelpers.ExtractBits(Attr0, 1, 13);
                SpriteShape = (Shape)BitHelpers.ExtractBits(Attr0, 2, 14);

                // Attr1
                XPosition = (short)BitHelpers.ExtractBits(Attr1, 9, 0);
                bool bit9 = false, bit10 = false, bit11 = false;
                if (XPosition >= 256) XPosition -= 512;
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

                if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_Sabrina) {
                    ImageIndex = (ushort)BitHelpers.ExtractBits(Attr2, 12, 0);
                    PaletteIndex = BitHelpers.ExtractBits(Attr2, 3, 12); // another flag at byte 0xF?
                } else if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_BatmanRiseOfSinTzu) {
                    ImageIndex = (ushort)BitHelpers.ExtractBits(Attr2, 15, 0);
                    PaletteIndex = BitHelpers.ExtractBits(Attr2, 1, 15); // another flag at byte 0xF?
                } else if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_StarWarsTrilogy) {
                    ImageIndex = (ushort)BitHelpers.ExtractBits(Attr2, 14, 0);
                    PaletteIndex = BitHelpers.ExtractBits(Attr2, 1, 14); // another flag at byte 0xF?
                } else if (s.GetR1Settings().EngineVersion >= EngineVersion.GBA_SplinterCell) {
                    ImageIndex = (ushort)BitHelpers.ExtractBits(Attr2, 14, 0);
                    PaletteIndex = BitHelpers.ExtractBits(Attr2, 2, 14); // another flag at byte 0xF?
                } else if(s.GetR1Settings().GBA_IsMilan) {
                    ImageIndex = Attr2;
                } else {
                    ImageIndex = (ushort)BitHelpers.ExtractBits(Attr2, 11, 0);
                    Priority = BitHelpers.ExtractBits(Attr2, 1, 11);
                    PaletteIndex = BitHelpers.ExtractBits(Attr2, 3, 12); // another flag at byte 0xF?
                }
                bool paletteFlag = BitHelpers.ExtractBits(Attr2, 1, 15) == 1;

                s.Log($"{nameof(XPosition)}: {XPosition}");
                s.Log($"{nameof(YPosition)}: {YPosition}");
                s.Log($"{nameof(TransformMode)}: {TransformMode}");
                s.Log($"{nameof(RenderMode)}: {RenderMode}");
                s.Log($"{nameof(Color)}: {Color}");
                s.Log($"{nameof(SpriteShape)}: {SpriteShape}");
                s.Log($"{nameof(SpriteSize)}: {SpriteSize}");

                s.Log($"{nameof(ImageIndex)}: {ImageIndex}");
                s.Log($"{nameof(Priority)}: {Priority}");
                s.Log($"{nameof(PaletteIndex)}: {PaletteIndex}");
                s.Log($"{nameof(paletteFlag)}: {paletteFlag}");
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
            } else if(ChannelType == Type.Unknown8) {
                Unknown8 = s.Serialize<int>(Unknown8, name: nameof(Unknown8));
            } else if (ChannelType == Type.UnknownC) {
                UnknownC = s.Serialize<int>(UnknownC, name: nameof(UnknownC));
            } else {
                s.Log($"{nameof(ChannelType)}: {ChannelType}");
                Box_MinY = s.Serialize<sbyte>(Box_MinY, name: nameof(Box_MinY));
                Box_MaxY = s.Serialize<sbyte>(Box_MaxY, name: nameof(Box_MaxY));
                Box_MinX = s.Serialize<sbyte>(Box_MinX, name: nameof(Box_MinX));
                Box_MaxX = s.Serialize<sbyte>(Box_MaxX, name: nameof(Box_MaxX));
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

        public GBA_AnimationChannel CloneObj() => (GBA_AnimationChannel)MemberwiseClone();
        public object Clone() => MemberwiseClone();
    }
}