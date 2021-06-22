namespace R1Engine
{
    public class Jade_KingKong_PSP_Manager : Jade_Montpellier_BaseManager {
		// Levels
		public override LevelInfo[] LevelInfos => new LevelInfo[] {
			new LevelInfo(0x0002EA59, "ROOT/EngineDatas/06 Levels/_main/_main_bootup", "_main_bootup.wol"),
			new LevelInfo(0x000129BE, "ROOT/EngineDatas/06 Levels/_main/_main_credits", "_main_credits.wol"),
			new LevelInfo(0x0002D989, "ROOT/EngineDatas/06 Levels/_main/_main_E3EndScreeen", "_main_E3EndScreeen.wol"),
			new LevelInfo(0x0000C452, "ROOT/EngineDatas/06 Levels/_main/_main_fix", "_main_fix.wol"),
			new LevelInfo(0x0000C456, "ROOT/EngineDatas/06 Levels/_main/_main_logo", "_main_logo.wol"),
			new LevelInfo(0x0000C45A, "ROOT/EngineDatas/06 Levels/_main/_main_menu", "_main_menu.wol"),
			new LevelInfo(0x0002DE8F, "ROOT/EngineDatas/06 Levels/_main/_main_pad", "_main_pad.wol"),
			new LevelInfo(0x00006177, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/01B_Skull_Island", "01B_Skull_Island.wol"),
			new LevelInfo(0x00011976, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/02C_Ann_s_capture_Part_1", "02C_Ann_s_capture_Part_1.wol"),
			new LevelInfo(0x0000975D, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/03A_Escape_from_natives", "03A_Escape_from_natives.wol"),
			new LevelInfo(0x000134E8, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/03B_On_Kongs_Tracks", "03B_On_Kongs_Tracks.wol"),
			new LevelInfo(0x00008887, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/03C_Hayes_is_back", "03C_Hayes_is_back.wol"),
			new LevelInfo(0x00002001, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/04A_Brontosaurs", "04A_Brontosaurs.wol"),
			new LevelInfo(0x0000BECA, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/05A_On_the_Raft_Part_2", "05A_On_the_Raft_Part_2.wol"),
			new LevelInfo(0x0000373A, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/12A_In_the_temple", "12A_In_the_temple.wol"),
			new LevelInfo(0x000016AF, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/17A_Jack_Gets_To_The_Lair_Bis", "17A_Jack_Gets_To_The_Lair_Bis.wol"),
			new LevelInfo(0x0000EBEF, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/03D_Kong_On_His_Killing_Ground", "03D_Kong_On_His_Killing_Ground.wol"),
			new LevelInfo(0x0001EABC, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/05C_Kong_vs_first_Trex", "05C_Kong_vs_first_Trex.wol"),
			new LevelInfo(0x00000213, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/14B_Kong_to_the_rescue", "14B.wol"),
			new LevelInfo(0x00000C5A, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/17C_Fight_in_the_lair", "17C_Fight_in_the_lair.wol"),
			new LevelInfo(0x0001FEDB, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/20A_New_York_theater", "20A_NY_theater.wol"),
			new LevelInfo(0x00000105, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/20C_Plane_In_New-York", "20C_Plane_In_New-York.wol"),
		};

        // Version properties
		public override string[] BFFiles => new string[] {
			"KingKong.bf"
		};
	}
}
