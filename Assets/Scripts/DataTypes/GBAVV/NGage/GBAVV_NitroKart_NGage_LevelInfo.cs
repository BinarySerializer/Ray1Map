namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_LevelInfo : R1Serializable
    {
        public bool SerializeData { get; set; } = true; // Set before serializing

        public Pointer PVSFilePathPointer { get; set; }
        public Pointer POPFilePathPointer { get; set; }
        public int Int_08 { get; set; }
        public Pointer ParallaxBaseFilePathPointer { get; set; }
        public int Int_10 { get; set; }
        public int Int_14 { get; set; }

        // Serialized from pointers
        public GBAVV_NitroKart_NGage_FilePath PVSFilePath { get; set; }
        public GBAVV_NitroKart_NGage_FilePath POPFilePath { get; set; }
        public GBAVV_NitroKart_NGage_FilePath ParallaxBaseFilePath { get; set; }

        public GBAVV_NitroKart_NGage_PVS PVS { get; set; }
        public GBAVV_NitroKart_NGage_POP POP { get; set; }
        public GBAVV_NitroKart_NGage_PAL[] ParallaxPalettes { get; set; }
        public byte[][] ParallaxData { get; set; } // TileSets? Animated like on GBA?

        public override void SerializeImpl(SerializerObject s)
        {
            PVSFilePathPointer = s.SerializePointer(PVSFilePathPointer, name: nameof(PVSFilePathPointer));
            POPFilePathPointer = s.SerializePointer(POPFilePathPointer, name: nameof(POPFilePathPointer));
            Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
            ParallaxBaseFilePathPointer = s.SerializePointer(ParallaxBaseFilePathPointer, name: nameof(ParallaxBaseFilePathPointer));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));

            PVSFilePath = s.DoAt(PVSFilePathPointer, () => s.SerializeObject<GBAVV_NitroKart_NGage_FilePath>(PVSFilePath, name: nameof(PVSFilePath)));
            POPFilePath = s.DoAt(POPFilePathPointer, () => s.SerializeObject<GBAVV_NitroKart_NGage_FilePath>(POPFilePath, name: nameof(POPFilePath)));
            ParallaxBaseFilePath = s.DoAt(ParallaxBaseFilePathPointer, () => s.SerializeObject<GBAVV_NitroKart_NGage_FilePath>(ParallaxBaseFilePath, name: nameof(ParallaxBaseFilePath)));

            if (!SerializeData)
                return;

            PVS = PVSFilePath.DoAtFile(() => s.SerializeObject<GBAVV_NitroKart_NGage_PVS>(PVS, name: nameof(PVS)));
            POP = POPFilePath.DoAtFile(() => s.SerializeObject<GBAVV_NitroKart_NGage_POP>(POP, name: nameof(POP)));

            if (ParallaxPalettes == null) ParallaxPalettes = new GBAVV_NitroKart_NGage_PAL[3];
            if (ParallaxData == null) ParallaxData = new byte[3][];

            // Every level has 3 background layers
            for (int i = 0; i < 3; i++)
            {
                ParallaxPalettes[i] = ParallaxBaseFilePath.DoAtFile($"\\parallax{i}.pal", () => s.SerializeObject(ParallaxPalettes[i], name: $"{nameof(ParallaxPalettes)}[{i}]"));
                ParallaxData[i] = ParallaxBaseFilePath.DoAtFile($"\\parallax{i}.rle", () => s.SerializeArray<byte>(ParallaxData[i], s.CurrentLength, name: $"{nameof(ParallaxData)}[{i}]"));
            }
        }
    }
}