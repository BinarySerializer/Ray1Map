namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_PVS : R1Serializable
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
        public int V2_Data1And2Count { get; set; }
        public Pointer V2_Data3Pointer { get; set; }
        public int V2_Int_1C { get; set; }
        public Pointer V2_Data4Pointer { get; set; }

        public int TexturesCount { get; set; }
        public Pointer TextureFilePathsPointer { get; set; }
        public int Data5Count { get; set; }
        public Pointer Data5Pointer { get; set; }

        // Serialized from pointers
        public GBAVV_NitroKart_NGage_FilePath[] TextureFilePaths { get; set; }
        
        public GBAVV_NitroKart_NGage_TEX[] Textures { get; set; }
        public GBAVV_NitroKart_NGage_PAL[] Palettes { get; set; }

        public short[] Data1_TriangleIndex { get; set; }
        public short[] Data1_VertexIndex { get; set; }
        public Triangle[] Triangles { get; set; }
        public Vertex[] Vertices { get; set; }
        public Vertex[] V1_Data4 { get; set; }
        public byte[] Data5 { get; set; }
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
            Data5Count = s.Serialize<int>(Data5Count, name: nameof(Data5Count));
            Data5Pointer = s.SerializePointer(Data5Pointer, anchor: fileStartPointer, name: nameof(Data5Pointer));
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
            Triangles = s.DoAt(TrianglesPointer, () => s.SerializeObjectArray<Triangle>(Triangles, TrianglesCount, name: nameof(Triangles)));
            Vertices = s.DoAt(VerticesPointer, () => s.SerializeObjectArray<Vertex>(Vertices, VerticesCount, name: nameof(Vertices)));

            if (FileVersion == 1)
            {
                V1_Data4 = s.DoAt(V1_Data4Pointer, () => s.SerializeObjectArray<Vertex>(V1_Data4, V1_Data4Count + 1, name: nameof(V1_Data4)));
            }
            Data5 = s.DoAt(Data5Pointer, () => s.SerializeArray<byte>(Data5, Data5Count * 0x40, name: nameof(Data5)));


            s.Goto(Offset + s.CurrentLength);
        }

		public class Vertex : R1Serializable {
            public short X { get; set; }
            public short Y { get; set; }
            public short Z { get; set; }

            public override void SerializeImpl(SerializerObject s) {
				X = s.Serialize<short>(X, name: nameof(X));
                Y = s.Serialize<short>(Y, name: nameof(Y));
                Z = s.Serialize<short>(Z, name: nameof(Z));
            }
        }

        public class Triangle : R1Serializable {
            public byte Byte_00 { get; set; }
            public byte TextureIndex { get; set; }
            public byte Byte_02 { get; set; }
            public byte Byte_03 { get; set; }
            public ushort Vertex0 { get; set; }
            public ushort Vertex1 { get; set; }
            public ushort Vertex2 { get; set; }
            public UV UV0 { get; set; }
            public UV UV1 { get; set; }
            public UV UV2 { get; set; }
            public byte Unk0 { get; set; }
            public byte Unk1 { get; set; }
            public byte Unk2 { get; set; }
            public byte Data5Index { get; set; }

            public override void SerializeImpl(SerializerObject s) {
                Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
                TextureIndex = s.Serialize<byte>(TextureIndex, name: nameof(TextureIndex));
                Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
                Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
                Vertex0 = s.Serialize<ushort>(Vertex0, name: nameof(Vertex0));
                Vertex1 = s.Serialize<ushort>(Vertex1, name: nameof(Vertex1));
                Vertex2 = s.Serialize<ushort>(Vertex2, name: nameof(Vertex2));
                UV0 = s.SerializeObject<UV>(UV0, name: nameof(UV0));
                UV1 = s.SerializeObject<UV>(UV1, name: nameof(UV1));
                UV2 = s.SerializeObject<UV>(UV2, name: nameof(UV2));
                Unk0 = s.Serialize<byte>(Unk0, name: nameof(Unk0));
                Unk1 = s.Serialize<byte>(Unk1, name: nameof(Unk1));
                Unk2 = s.Serialize<byte>(Unk2, name: nameof(Unk2));
                Data5Index = s.Serialize<byte>(Data5Index, name: nameof(Data5Index));
            }
        }

        public class UV : R1Serializable {
            public byte U { get; set; }
            public byte V { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				U = s.Serialize<byte>(U, name: nameof(U));
                V = s.Serialize<byte>(V, name: nameof(V));
            }
		}
    }
}