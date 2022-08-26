using System;

namespace BinarySerializer.PSP
{
    /// <see href="http://hitmen.c02.at/files/yapspd/psp_doc/chap11.html#sec11.5.15">GE Vertex Type</see>
    public class GE_VertexColor : BinarySerializable
    {
        public Pointer Pre_AlignOffset { get; set; }
        public GE_ColorFormat Pre_Format { get; set; }

        public RGB565Color Color565 { get; set; }
        public RGBA4444Color Color4444 { get; set; }
        public RGBA5551Color Color5551 { get; set; }
        public RGBA8888Color Color8888 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            switch (Pre_Format) {
                case GE_ColorFormat.None:
                    break;
                case GE_ColorFormat.Invalid1:
                case GE_ColorFormat.Invalid2:
                case GE_ColorFormat.Invalid3:
                    throw new BinarySerializableException(this, $"Invalid color format");
                case GE_ColorFormat.BGR565:
                    s.Align(2, Pre_AlignOffset);
                    Color565 = s.SerializeObject<RGB565Color>(Color565, name: nameof(Color565));
                    //s.SystemLog?.LogWarning($"{Offset}: Check color format: {Pre_Format}");
                    break;
                case GE_ColorFormat.ABGR5551:
                    s.Align(2, Pre_AlignOffset);
                    Color5551 = s.SerializeObject<RGBA5551Color>(Color5551, name: nameof(Color5551));
                    //s.SystemLog?.LogWarning($"{Offset}: Check color format: {Pre_Format}");
                    break;
                case GE_ColorFormat.ABGR4444:
                    s.Align(2, Pre_AlignOffset);
                    Color4444 = s.SerializeObject<RGBA4444Color>(Color4444, name: nameof(Color4444));
                    //s.SystemLog?.LogWarning($"{Offset}: Check color format: {Pre_Format}");
                    break;
                case GE_ColorFormat.ABGR8888:
                    s.Align(4, Pre_AlignOffset);
                    Color8888 = s.SerializeObject<RGBA8888Color>(Color8888, name: nameof(Color8888));
                    //s.SystemLog?.LogWarning($"{Offset}: Check color format: {Pre_Format}");
                    break;
            }
        }

        public BaseColor Color => Pre_Format switch {
            GE_ColorFormat.BGR565 => Color565,
            GE_ColorFormat.ABGR5551 => Color5551,
            GE_ColorFormat.ABGR4444 => Color4444,
            GE_ColorFormat.ABGR8888 => Color8888,
            _ => null
        };
    }
}