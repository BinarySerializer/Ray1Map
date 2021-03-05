namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_PVS : R1Serializable
    {
        public int FileVersion { get; set; }

        public string Magic { get; set; } // File begins here
        public int V1_Int_04 { get; set; }
        public Pointer V1_Data1Pointer { get; set; }
        public int V1_Int_0C { get; set; }
        public int V1_Int_10 { get; set; }
        public Pointer V1_Pointer_14 { get; set; }
        public int V1_Data_14_Length { get; set; }
        public Pointer V1_Pointer_1C { get; set; }
        public int V1_Int_20 { get; set; }
        public int V1_Int_24 { get; set; }

        public int V2_Int_04 { get; set; }
        public int V2_Int_08 { get; set; }
        public Pointer V2_Data1Pointer { get; set; }
        public Pointer V2_Data2Pointer { get; set; }
        public int V2_Data1And2Count { get; set; }
        public Pointer V2_Data3Pointer { get; set; }
        public int V2_Int_1C { get; set; }
        public Pointer V2_Data4Pointer { get; set; }

        public int TexturesCount { get; set; }
        public Pointer TextureFilePathsPointer { get; set; }
        public int Int_34 { get; set; }
        public Pointer Pointer_38 { get; set; }

        // Serialized from pointers
        public GBAVV_NitroKart_NGage_FilePath[] TextureFilePaths { get; set; }
        
        public GBAVV_NitroKart_NGage_TEX[] Textures { get; set; }
        public GBAVV_NitroKart_NGage_PAL[] Palettes { get; set; }

        public short[] V2_Data1 { get; set; }
        public short[] V2_Data2 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FileVersion = s.Serialize<int>(FileVersion, name: nameof(FileVersion));

            var fileStartPointer = s.CurrentPointer;

            Magic = s.SerializeString(Magic, 4, name: nameof(Magic));

            if (FileVersion == 1)
            {
                V1_Int_04 = s.Serialize<int>(V1_Int_04, name: nameof(V1_Int_04));
                V1_Data1Pointer = s.SerializePointer(V1_Data1Pointer, anchor: fileStartPointer, name: nameof(V1_Data1Pointer));
                V1_Int_0C = s.Serialize<int>(V1_Int_0C, name: nameof(V1_Int_0C));
                V1_Int_10 = s.Serialize<int>(V1_Int_10, name: nameof(V1_Int_10));
                V1_Pointer_14 = s.SerializePointer(V1_Pointer_14, anchor: fileStartPointer, name: nameof(V1_Pointer_14));
                V1_Data_14_Length = s.Serialize<int>(V1_Data_14_Length, name: nameof(V1_Data_14_Length));
                V1_Pointer_1C = s.SerializePointer(V1_Pointer_1C, anchor: fileStartPointer, name: nameof(V1_Pointer_1C));
                V1_Int_20 = s.Serialize<int>(V1_Int_20, name: nameof(V1_Int_20));
                V1_Int_24 = s.Serialize<int>(V1_Int_24, name: nameof(V1_Int_24));
            }
            else if (FileVersion == 2)
            {
                V2_Int_04 = s.Serialize<int>(V2_Int_04, name: nameof(V2_Int_04));
                V2_Int_08 = s.Serialize<int>(V2_Int_08, name: nameof(V2_Int_08));

                V2_Data1Pointer = s.SerializePointer(V2_Data1Pointer, anchor: fileStartPointer, name: nameof(V2_Data1Pointer));
                V2_Data2Pointer = s.SerializePointer(V2_Data2Pointer, anchor: fileStartPointer, name: nameof(V2_Data2Pointer));
                V2_Data1And2Count = s.Serialize<int>(V2_Data1And2Count, name: nameof(V2_Data1And2Count));

                V2_Data3Pointer = s.SerializePointer(V2_Data3Pointer, anchor: fileStartPointer, name: nameof(V2_Data3Pointer));
                V2_Int_1C = s.Serialize<int>(V2_Int_1C, name: nameof(V2_Int_1C));
                V2_Data4Pointer = s.SerializePointer(V2_Data4Pointer, anchor: fileStartPointer, name: nameof(V2_Data4Pointer));
                // TODO: Version 2 seems to have an additional int after Pointer_38
            }

            TexturesCount = s.Serialize<int>(TexturesCount, name: nameof(TexturesCount));
            TextureFilePathsPointer = s.SerializePointer(TextureFilePathsPointer, anchor: fileStartPointer, name: nameof(TextureFilePathsPointer));
            Int_34 = s.Serialize<int>(Int_34, name: nameof(Int_34));
            Pointer_38 = s.SerializePointer(Pointer_38, anchor: fileStartPointer, name: nameof(Pointer_38));

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

            if (FileVersion == 1)
            {

            }
            else if (FileVersion == 2)
            {
                V2_Data1 = s.DoAt(V2_Data1Pointer, () => s.SerializeArray<short>(V2_Data1, V2_Data1And2Count, name: nameof(V2_Data1)));
                V2_Data2 = s.DoAt(V2_Data2Pointer, () => s.SerializeArray<short>(V2_Data2, V2_Data1And2Count, name: nameof(V2_Data2)));
            }

            s.Goto(Offset + s.CurrentLength);
        }
    }
}