using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_PVS : BinarySerializable
    {
        public int FileVersion { get; set; }

        public string Magic { get; set; } // File begins here
        public int V1_Data1Count { get; set; }
        public int TrianglesCount { get; set; }
        public Pointer TrianglesPointer { get; set; }
        public int VerticesCount { get; set; }
        public Pointer VerticesPointer { get; set; }
        public int V1_Data4Count { get; set; }
        public Pointer V1_Data4Pointer { get; set; }

        public int V2_Data1_Width { get; set; }
        public int V2_Data1_Height { get; set; }
        public Pointer Data1_TriangleIndexPointer { get; set; }
        public Pointer Data1_VertexIndexPointer { get; set; }
        public int TexturesCount { get; set; }
        public Pointer TextureFilePathsPointer { get; set; }
        public int VertexColorPalettesCount { get; set; }
        public Pointer VertexColorPalettesPointer { get; set; }

        // Serialized from pointers
        public GBAVV_NitroKart_NGage_FilePath[] TextureFilePaths { get; set; }
        
        public GBAVV_NitroKart_NGage_TEX[] Textures { get; set; }
        public GBAVV_NitroKart_NGage_PAL[] Palettes { get; set; }

        public short[] Data1_TriangleIndex { get; set; }
        public short[] Data1_VertexIndex { get; set; }
        public GBAVV_NitroKart_NGage_Triangle[] Triangles { get; set; }
        public GBAVV_NitroKart_NGage_Vertex[] Vertices { get; set; }
        public GBAVV_NitroKart_NGage_Vertex[] V1_Data4 { get; set; }
        public RGBA8888Color[] VertexColorsPalettes { get; set; }
        //public byte[] Data5 { get; set; }
        public byte[] V2_UnknownBytes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FileVersion = s.Serialize<int>(FileVersion, name: nameof(FileVersion));

            var fileStartPointer = s.CurrentPointer;

            Magic = s.SerializeString(Magic, 4, name: nameof(Magic));

            if (FileVersion == 1) {
                V1_Data1Count = s.Serialize<int>(V1_Data1Count, name: nameof(V1_Data1Count));
            } else if (FileVersion == 2) {
                V2_Data1_Width = s.Serialize<int>(V2_Data1_Width, name: nameof(V2_Data1_Width));
                V2_Data1_Height = s.Serialize<int>(V2_Data1_Height, name: nameof(V2_Data1_Height));
            }
            Data1_TriangleIndexPointer = s.SerializePointer(Data1_TriangleIndexPointer, anchor: fileStartPointer, name: nameof(Data1_TriangleIndexPointer));
            Data1_VertexIndexPointer = s.SerializePointer(Data1_VertexIndexPointer, anchor: fileStartPointer, name: nameof(Data1_VertexIndexPointer));
            
            TrianglesCount = s.Serialize<int>(TrianglesCount, name: nameof(TrianglesCount));
            TrianglesPointer = s.SerializePointer(TrianglesPointer, anchor: fileStartPointer, name: nameof(TrianglesPointer));
            VerticesCount = s.Serialize<int>(VerticesCount, name: nameof(VerticesCount));
            VerticesPointer = s.SerializePointer(VerticesPointer, anchor: fileStartPointer, name: nameof(VerticesPointer));
            
            if (FileVersion == 1) {
                V1_Data4Count = s.Serialize<int>(V1_Data4Count, name: nameof(V1_Data4Count));
                V1_Data4Pointer = s.SerializePointer(V1_Data4Pointer, anchor: fileStartPointer, name: nameof(V1_Data4Pointer));
            }

            TexturesCount = s.Serialize<int>(TexturesCount, name: nameof(TexturesCount));
            TextureFilePathsPointer = s.SerializePointer(TextureFilePathsPointer, anchor: fileStartPointer, name: nameof(TextureFilePathsPointer));
            VertexColorPalettesCount = s.Serialize<int>(VertexColorPalettesCount, name: nameof(VertexColorPalettesCount));
            VertexColorPalettesPointer = s.SerializePointer(VertexColorPalettesPointer, anchor: fileStartPointer, name: nameof(VertexColorPalettesPointer));
            if (FileVersion == 2) {
                V2_UnknownBytes = s.SerializeArray<byte>(V2_UnknownBytes, 4, name: nameof(V2_UnknownBytes));
            }

            TextureFilePaths = s.DoAt(TextureFilePathsPointer, () => s.SerializeObjectArray(TextureFilePaths, TexturesCount, x =>
            {
                x.StringLength = 52;
                x.BasePath = $"gfx";
            }, name: nameof(TextureFilePaths)));

            if (Textures == null) Textures = new GBAVV_NitroKart_NGage_TEX[TexturesCount];
            if (Palettes == null) Palettes = new GBAVV_NitroKart_NGage_PAL[TexturesCount];

            for (int i = 0; i < TexturesCount; i++)
            {
                Textures[i] = TextureFilePaths[i].DoAtFile(".tex", () => s.SerializeObject(Textures[i], name: $"{nameof(Textures)}[{i}]"));
                Palettes[i] = TextureFilePaths[i].DoAtFile(".pal", () => s.SerializeObject(Palettes[i], name: $"{nameof(Palettes)}[{i}]"));
            }
            var v1Count = FileVersion == 1 ? V1_Data1Count : FileVersion == 2 ? (V2_Data1_Width * V2_Data1_Height) + 2 : 0;
            Data1_TriangleIndex = s.DoAt(Data1_TriangleIndexPointer, () => s.SerializeArray<short>(Data1_TriangleIndex, V1_Data1Count, name: nameof(Data1_TriangleIndex)));
            Data1_VertexIndex = s.DoAt(Data1_VertexIndexPointer, () => s.SerializeArray<short>(Data1_VertexIndex, V1_Data1Count, name: nameof(Data1_VertexIndex)));
            Triangles = s.DoAt(TrianglesPointer, () => s.SerializeObjectArray<GBAVV_NitroKart_NGage_Triangle>(Triangles, TrianglesCount, name: nameof(Triangles)));
            Vertices = s.DoAt(VerticesPointer, () => s.SerializeObjectArray<GBAVV_NitroKart_NGage_Vertex>(Vertices, VerticesCount, name: nameof(Vertices)));

            if (FileVersion == 1)
            {
                V1_Data4 = s.DoAt(V1_Data4Pointer, () => s.SerializeObjectArray<GBAVV_NitroKart_NGage_Vertex>(V1_Data4, V1_Data4Count + 1, name: nameof(V1_Data4)));
            }
            //Data5 = s.DoAt(Data5Pointer, () => s.SerializeArray<byte>(Data5, Data5Count * 0x40, name: nameof(Data5)));
            VertexColorsPalettes = s.DoAt(VertexColorPalettesPointer, () => s.SerializeObjectArray<RGBA8888Color>(VertexColorsPalettes, 16 * VertexColorPalettesCount, name: nameof(VertexColorsPalettes)));

            s.Goto(Offset + s.CurrentLength);
        }
    }
}