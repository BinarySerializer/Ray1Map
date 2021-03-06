namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_Triangle : R1Serializable
    {
        public byte BlendMode { get; set; }
        public byte TextureIndex { get; set; }
        public ushort Flags { get; set; }
        public ushort Vertex0 { get; set; }
        public ushort Vertex1 { get; set; }
        public ushort Vertex2 { get; set; }
        public GBAVV_NitroKart_NGage_UV UV0 { get; set; }
        public GBAVV_NitroKart_NGage_UV UV1 { get; set; }
        public GBAVV_NitroKart_NGage_UV UV2 { get; set; }
        public byte VertexColorIndex0 { get; set; }
        public byte VertexColorIndex1 { get; set; }
        public byte VertexColorIndex2 { get; set; }
        public byte VertexColorPaletteIndex { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            BlendMode = s.Serialize<byte>(BlendMode, name: nameof(BlendMode));
            TextureIndex = s.Serialize<byte>(TextureIndex, name: nameof(TextureIndex));
            Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
            Vertex0 = s.Serialize<ushort>(Vertex0, name: nameof(Vertex0));
            Vertex1 = s.Serialize<ushort>(Vertex1, name: nameof(Vertex1));
            Vertex2 = s.Serialize<ushort>(Vertex2, name: nameof(Vertex2));
            UV0 = s.SerializeObject<GBAVV_NitroKart_NGage_UV>(UV0, name: nameof(UV0));
            UV1 = s.SerializeObject<GBAVV_NitroKart_NGage_UV>(UV1, name: nameof(UV1));
            UV2 = s.SerializeObject<GBAVV_NitroKart_NGage_UV>(UV2, name: nameof(UV2));
            VertexColorIndex0 = s.Serialize<byte>(VertexColorIndex0, name: nameof(VertexColorIndex0));
            VertexColorIndex1 = s.Serialize<byte>(VertexColorIndex1, name: nameof(VertexColorIndex1));
            VertexColorIndex2 = s.Serialize<byte>(VertexColorIndex2, name: nameof(VertexColorIndex2));
            VertexColorPaletteIndex = s.Serialize<byte>(VertexColorPaletteIndex, name: nameof(VertexColorPaletteIndex));
        }
    }
}