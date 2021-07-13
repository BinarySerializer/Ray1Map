namespace R1Engine
{
    public class Jade_KingKong_Xbox360_20050926_Manager : Jade_KingKong_Xbox360_Manager {
		// Levels
		public override LevelInfo[] LevelInfos => new LevelInfo[] {
			new LevelInfo(0x0002EA59, "ROOT/EngineDatas/06 Levels/_main/_main_bootup", "_main_bootup.wol"),
			new LevelInfo(0x0000C452, "ROOT/EngineDatas/06 Levels/_main/_main_fix", "_main_fix.wol"),
			new LevelInfo(0x0000C456, "ROOT/EngineDatas/06 Levels/_main/_main_logo", "_main_logo.wol"),
			new LevelInfo(0x0000C45A, "ROOT/EngineDatas/06 Levels/_main/_main_menu", "_main_menu.wol"),
			new LevelInfo(0x00001B17, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/03E_Chased_by_The_Trex", "03E_Chased_by_The_Trex.wol"),
			new LevelInfo(0x0001EABC, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/05C_Kong_vs_first_Trex", "05C_Kong_vs_first_Trex.wol"),
		};

        // Version properties
		public override string[] BFFiles => new string[] {
			"KingKongTheGame_clean.bf"
		};
	}
}
