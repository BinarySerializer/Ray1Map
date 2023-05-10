using System;

namespace BinarySerializer.PSP
{
    /// <see href="http://hitmen.c02.at/files/yapspd/psp_doc/chap11.html#sec11.5.15">GE Vertex Type</see>
    public class GE_VertexNumber : BinarySerializable
    {
        public Pointer Pre_AlignOffset { get; set; }
        public GE_VertexNumberType Pre_Type { get; set; }
        public GE_VertexNumberFormat Pre_Format { get; set; }

        // 8-Bit
        public sbyte ValueSByte { get; set; }
        public byte ValueByte { get; set; }

        // 16-Bit
        public short ValueShort { get; set; }
        public ushort ValueUShort { get; set; }

        // 32-Bit
        public float ValueFloat { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            switch (Pre_Format) {
                case GE_VertexNumberFormat.None:
                    break;
                case GE_VertexNumberFormat.FixedPoint_8Bit:
                    if (Pre_Type == GE_VertexNumberType.Texture) {
                        ValueByte = s.Serialize<byte>(ValueByte, name: nameof(ValueByte));
                    } else {
                        ValueSByte = s.Serialize<sbyte>(ValueSByte, name: nameof(ValueSByte));
                    }
                    break;
                case GE_VertexNumberFormat.FixedPoint_16Bit:
                    s.Align(2, Pre_AlignOffset);
                    if (Pre_Type == GE_VertexNumberType.Texture) {
                        ValueUShort = s.Serialize<ushort>(ValueUShort, name: nameof(ValueUShort));
                    } else {
                        ValueShort = s.Serialize<short>(ValueShort, name: nameof(ValueShort));
                    }
                    break;
                case GE_VertexNumberFormat.FloatingPoint_32Bit:
                    s.Align(4, Pre_AlignOffset);
                    ValueFloat = s.Serialize<float>(ValueFloat, name: nameof(ValueFloat));
                    break;
			}
		}

        public float Value {
            get {
                switch (Pre_Format) {
                    case GE_VertexNumberFormat.FixedPoint_8Bit:
                        if (Pre_Type == GE_VertexNumberType.Texture) {
                            return ValueByte / (float)(byte.MaxValue);
                        } else {
                            return ValueSByte / (float)sbyte.MaxValue;
                        }
                    case GE_VertexNumberFormat.FixedPoint_16Bit:
                        if (Pre_Type == GE_VertexNumberType.Texture) {
                            return ValueUShort / (float)(ushort.MaxValue);
                        } else {
                            return ValueShort / (float)short.MaxValue;
                        }
                    case GE_VertexNumberFormat.FloatingPoint_32Bit:
                        return ValueFloat;
                    default:
                        throw new BinarySerializableException(this, $"Bad VertexNumberFormat {Pre_Format}");
                }
            }
        }
    }
}