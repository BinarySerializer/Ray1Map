namespace Ray1Map
{
    public class Jade_JustDance_Wii_Manager : Jade_Montreal_BaseManager {
        // Levels
        public override LevelInfo[] LevelInfos => new LevelInfo[] {
			new LevelInfo(0x0D0BA66B, "ROOT/Bin", "Base_wow_0d0ba66b.bin", worldName: "WOW", mapName: "Base", type: LevelInfo.FileType.WOW),
			new LevelInfo(0x1800E771, "ROOT/Bin", "Camera_wow_1800e771.bin", worldName: "WOW", mapName: "Camera", type: LevelInfo.FileType.WOW),
			new LevelInfo(0x22000C15, "ROOT/Bin", "DS_Base_wow_22000c15.bin", worldName: "WOW", mapName: "DS_Base", type: LevelInfo.FileType.WOW),
			new LevelInfo(0x2C001025, "ROOT/Bin", "DS_SFX_wow_2c001025.bin", worldName: "WOW", mapName: "DS_SFX", type: LevelInfo.FileType.WOW),
			new LevelInfo(0x200001B8, "ROOT/Bin", "DS_SND_wow_200001b8.bin", worldName: "WOW", mapName: "DS_SND", type: LevelInfo.FileType.WOW),
			new LevelInfo(0x22000F55, "ROOT/Bin", "DS01_GPP_wow_22000f55.bin", worldName: "WOW", mapName: "DS01_GPP", type: LevelInfo.FileType.WOW),
			new LevelInfo(0x22000F56, "ROOT/Bin", "DS01_wol_22000f56.bin", worldName: "WOL", mapName: "DS01", type: LevelInfo.FileType.WOL),
			new LevelInfo(0x22001912, "ROOT/Bin", "DS2_Base_wow_22001912.bin", worldName: "WOW", mapName: "DS2_Base", type: LevelInfo.FileType.WOW),
			new LevelInfo(0x11000AF8, "ROOT/Bin", "DynamicTexture_wol_11000af8.bin", worldName: "WOL", mapName: "DynamicTexture", type: LevelInfo.FileType.WOL),
			new LevelInfo(0x11000AF7, "ROOT/Bin", "DynamicTexture_wow_11000af7.bin", worldName: "WOW", mapName: "DynamicTexture", type: LevelInfo.FileType.WOW),
			new LevelInfo(0x1800EACE, "ROOT/Bin", "Interface_wow_1800eace.bin", worldName: "WOW", mapName: "Interface", type: LevelInfo.FileType.WOW),
			new LevelInfo(0x22000092, "ROOT/Bin", "RG_Base_wow_22000092.bin", worldName: "WOW", mapName: "RG_Base", type: LevelInfo.FileType.WOW),
			new LevelInfo(0x3A00009A, "ROOT/Bin", "Sound_wow_3a00009a.bin", worldName: "WOW", mapName: "Sound", type: LevelInfo.FileType.WOW),
			new LevelInfo(0x0D0BA662, "ROOT/Bin", "Tools_wow_0d0ba662.bin", worldName: "WOW", mapName: "Tools", type: LevelInfo.FileType.WOW),
		};

		public override string[] BFFiles => new string[] {
            "WD_bin_wii.bf"
        };
	}
}
