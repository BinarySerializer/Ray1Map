namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_PVS : R1Serializable
    {
        public int Int_00 { get; set; }
        public string Magic { get; set; } // File begins here
        public int Int_04 { get; set; }
        public int Int_08 { get; set; }
        public int Int_0C { get; set; }
        public int Int_10 { get; set; }
        public Pointer Pointer_14 { get; set; }
        public int Data_14_Length { get; set; }
        public Pointer Pointer_1C { get; set; }
        public int Int_20 { get; set; }
        public int Int_24 { get; set; }
        public int TexturesCount { get; set; }
        public Pointer TextureFilePathsPointer { get; set; }
        public int Int_34 { get; set; }
        public Pointer Pointer_38 { get; set; }

        // Serialized from pointers
        public GBAVV_NitroKart_NGage_FilePath[] TextureFilePaths { get; set; }
        
        public GBAVV_NitroKart_NGage_TEX[] Textures { get; set; }
        public GBAVV_NitroKart_NGage_PAL[] Palettes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));

            var fileStartPointer = s.CurrentPointer;

            Magic = s.SerializeString(Magic, 4, name: nameof(Magic));

            // TODO: What is this?
            if (Int_00 == 1)
                Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));

            Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
            Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            Pointer_14 = s.SerializePointer(Pointer_14, anchor: fileStartPointer, name: nameof(Pointer_14));
            Data_14_Length = s.Serialize<int>(Data_14_Length, name: nameof(Data_14_Length));
            Pointer_1C = s.SerializePointer(Pointer_1C, anchor: fileStartPointer, name: nameof(Pointer_1C));
            Int_20 = s.Serialize<int>(Int_20, name: nameof(Int_20));
            Int_24 = s.Serialize<int>(Int_24, name: nameof(Int_24));
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

            s.Goto(Offset + s.CurrentLength);
        }
    }
}