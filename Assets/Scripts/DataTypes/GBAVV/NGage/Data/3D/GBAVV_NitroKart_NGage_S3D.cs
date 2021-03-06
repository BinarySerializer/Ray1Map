using System.Linq;

namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_S3D : R1Serializable
    {
        public string Magic { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Pointer TrianglesPointer { get; set; }
        public Pointer VerticesPointer { get; set; }
        public int[] TriangleCounts { get; set; }
        public int[] Counts2 { get; set; }
        public int[] Counts3 { get; set; }
        public int[] Counts4 { get; set; }
        public int TexturesCount { get; set; }
        public Pointer TextureFilePathsPointer { get; set; }

        // Serialized from pointers
        public GBAVV_NitroKart_NGage_FilePath[] TextureFilePaths { get; set; }

        public GBAVV_NitroKart_NGage_TEX[] Textures { get; set; }
        public GBAVV_NitroKart_NGage_PAL[] Palettes { get; set; }

        public GBAVV_NitroKart_NGage_Triangle[] Triangles { get; set; }
        public GBAVV_NitroKart_NGage_Vertex[] Vertices { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Magic = s.SerializeString(Magic, 4, name: nameof(Magic));
            Width = s.Serialize<int>(Width, name: nameof(Width));
            Height = s.Serialize<int>(Height, name: nameof(Height));
            TrianglesPointer = s.SerializePointer(TrianglesPointer, name: nameof(TrianglesPointer));
            VerticesPointer = s.SerializePointer(VerticesPointer, name: nameof(VerticesPointer));
            TriangleCounts = s.SerializeArray<int>(TriangleCounts, 12, name: nameof(TriangleCounts));
            Counts2 = s.SerializeArray<int>(Counts2, 12, name: nameof(Counts2));
            Counts3 = s.SerializeArray<int>(Counts3, 12, name: nameof(Counts3));
            Counts4 = s.SerializeArray<int>(Counts4, 12, name: nameof(Counts4));
            TexturesCount = s.Serialize<int>(TexturesCount, name: nameof(TexturesCount));
            TextureFilePathsPointer = s.SerializePointer(TextureFilePathsPointer, name: nameof(TextureFilePathsPointer));

            TextureFilePaths = s.DoAt(TextureFilePathsPointer, () => s.SerializeObjectArray(TextureFilePaths, TexturesCount, x => x.StringLength = 52, name: nameof(TextureFilePaths)));

            if (Textures == null) Textures = new GBAVV_NitroKart_NGage_TEX[TexturesCount];
            if (Palettes == null) Palettes = new GBAVV_NitroKart_NGage_PAL[TexturesCount];

            for (int i = 0; i < TexturesCount; i++)
            {
                Textures[i] = TextureFilePaths[i].DoAtFile(".tex", () => s.SerializeObject(Textures[i], name: $"{nameof(Textures)}[{i}]"));
                Palettes[i] = TextureFilePaths[i].DoAtFile(".pal", () => s.SerializeObject(Palettes[i], name: $"{nameof(Palettes)}[{i}]"));
            }

            Triangles = s.DoAt(TrianglesPointer, () => s.SerializeObjectArray<GBAVV_NitroKart_NGage_Triangle>(Triangles, TriangleCounts.Sum(), name: nameof(Triangles)));
            Vertices = s.DoAt(VerticesPointer, () => s.SerializeObjectArray<GBAVV_NitroKart_NGage_Vertex>(Vertices, (Width * Height) / 6, name: nameof(Vertices)));

            s.Goto(Offset + s.CurrentLength);
        }
    }
}