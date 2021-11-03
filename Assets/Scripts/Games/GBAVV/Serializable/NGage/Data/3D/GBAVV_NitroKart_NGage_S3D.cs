using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_NitroKart_NGage_S3D : BinarySerializable
    {
        public string Magic { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Pointer TrianglesPointer { get; set; }
        public Pointer VerticesPointer { get; set; }
        public int[] TriangleCounts { get; set; }
        public int[] VerticesCounts { get; set; }
        public int[] Counts3 { get; set; }
        public int[] VerticesOffsets { get; set; }
        public int TexturesCount { get; set; }
        public Pointer TextureFilePathsPointer { get; set; }

        // Serialized from pointers
        public GBAVV_NitroKart_NGage_FilePath[] TextureFilePaths { get; set; }

        public GBAVV_NitroKart_NGage_TEX[] Textures { get; set; }
        public GBAVV_NitroKart_NGage_PAL[] Palettes { get; set; }

        public GBAVV_NitroKart_NGage_Triangle[][] Triangles { get; set; }
        public uint VerticesUInt0 { get; set; }
        public uint VerticesUInt1 { get; set; }
        public GBAVV_NitroKart_NGage_Vertex[][] Vertices { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Magic = s.SerializeString(Magic, 4, name: nameof(Magic));
            Width = s.Serialize<int>(Width, name: nameof(Width));
            Height = s.Serialize<int>(Height, name: nameof(Height));
            TrianglesPointer = s.SerializePointer(TrianglesPointer, name: nameof(TrianglesPointer));
            VerticesPointer = s.SerializePointer(VerticesPointer, name: nameof(VerticesPointer));
            TriangleCounts = s.SerializeArray<int>(TriangleCounts, 12, name: nameof(TriangleCounts));
            VerticesCounts = s.SerializeArray<int>(VerticesCounts, 12, name: nameof(VerticesCounts));
            Counts3 = s.SerializeArray<int>(Counts3, 12, name: nameof(Counts3));
            VerticesOffsets = s.SerializeArray<int>(VerticesOffsets, 12, name: nameof(VerticesOffsets));
            TexturesCount = s.Serialize<int>(TexturesCount, name: nameof(TexturesCount));
            TextureFilePathsPointer = s.SerializePointer(TextureFilePathsPointer, name: nameof(TextureFilePathsPointer));

            TextureFilePaths = s.DoAt(TextureFilePathsPointer, () => s.SerializeObjectArray(TextureFilePaths, TexturesCount, x => x.StringLength = 52, name: nameof(TextureFilePaths)));

            if (Textures == null) Textures = new GBAVV_NitroKart_NGage_TEX[TexturesCount];
            if (Palettes == null) Palettes = new GBAVV_NitroKart_NGage_PAL[TexturesCount];
            if (Triangles == null) Triangles = new GBAVV_NitroKart_NGage_Triangle[TexturesCount][];
            if(Vertices == null) Vertices = new GBAVV_NitroKart_NGage_Vertex[TexturesCount][];

            for (int i = 0; i < TexturesCount; i++) {
                Textures[i] = TextureFilePaths[i].DoAtFile(".tex", () => s.SerializeObject(Textures[i], name: $"{nameof(Textures)}[{i}]"));
                Palettes[i] = TextureFilePaths[i].DoAtFile(".pal", () => s.SerializeObject(Palettes[i], name: $"{nameof(Palettes)}[{i}]"));
            }

            s.DoAt(TrianglesPointer, () => {
                for (int i = 0; i < Triangles.Length; i++) {
                    Triangles[i] = s.SerializeObjectArray<GBAVV_NitroKart_NGage_Triangle>(Triangles[i], TriangleCounts[i], name: $"{nameof(Triangles)}[{i}]");
                }
            });
            s.DoAt(VerticesPointer, () => {
                VerticesUInt0 = s.Serialize<uint>(VerticesUInt0, name: nameof(VerticesUInt0));
                VerticesUInt1 = s.Serialize<uint>(VerticesUInt1, name: nameof(VerticesUInt1));

                for (int i = 0; i < Vertices.Length; i++) {
                    s.Goto(VerticesPointer + VerticesOffsets[i]);
                    Vertices[i] = s.SerializeObjectArray<GBAVV_NitroKart_NGage_Vertex>(Vertices[i], VerticesCounts[i], name: $"{nameof(Vertices)}[{i}]");
                }
            });

            s.Goto(Offset + s.CurrentLength);
        }
    }
}