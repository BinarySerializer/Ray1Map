namespace BinarySerializer.PSP
{
    /// <see href="http://hitmen.c02.at/files/yapspd/psp_doc/chap11.html#sec11.5.15">GE Vertex Type</see>
    public class GE_VertexColor : BinarySerializable
    {
        public Pointer Pre_AlignOffset { get; set; }
        public GE_ColorFormat Pre_Format { get; set; }

        public SerializableColor Color { get; set; }

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
                    Color = s.SerializeObject<RGB565Color>(Color, name: nameof(Color));
                    //s.SystemLogger?.LogWarning($"{Offset}: Check color format: {Pre_Format}");
                    break;
                case GE_ColorFormat.ABGR5551:
                    s.Align(2, Pre_AlignOffset);
                    Color = s.SerializeObject<RGBA5551Color>(Color, name: nameof(Color));
                    //s.SystemLogger?.LogWarning($"{Offset}: Check color format: {Pre_Format}");
                    break;
                case GE_ColorFormat.ABGR4444:
                    s.Align(2, Pre_AlignOffset);
                    Color = s.SerializeObject<RGBA4444Color>(Color, name: nameof(Color));
                    //s.SystemLogger?.LogWarning($"{Offset}: Check color format: {Pre_Format}");
                    break;
                case GE_ColorFormat.ABGR8888:
                    s.Align(4, Pre_AlignOffset);
                    Color = s.SerializeInto<SerializableColor>(Color, BytewiseColor.RGBA8888, name: nameof(Color));
                    //s.SystemLogger?.LogWarning($"{Offset}: Check color format: {Pre_Format}");
                    break;
            }
        }
    }
}