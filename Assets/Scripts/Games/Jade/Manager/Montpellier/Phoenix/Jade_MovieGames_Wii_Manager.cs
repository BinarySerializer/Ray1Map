namespace Ray1Map
{
    public class Jade_MovieGames_Wii_Manager : Jade_Montpellier_BaseManager {
        // Levels
        public override LevelInfo[] LevelInfos => new LevelInfo[] {
            new LevelInfo(0x0000F0AC, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/DDR/DDR_102_P_BOAT", "DDR_102_P_BOAT.wol"),
            new LevelInfo(0x0000EFFB, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/DDR/DDR_112_S_CONTROLBASE", "DDR_112_S_CONTROLBASE.wol"),
            new LevelInfo(0x0000F0A4, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/DDR/DDR_130_E_BADBASE", "DDR_130_E_BADBASE.wol"),
            new LevelInfo(0x0000F0A8, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/DDR/DDR_140_M_OUTSIDE", "DDR_140_M_OUTSIDE.wol"),
            new LevelInfo(0x00000A70, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/DECATHLON/DECA_100_P_POTOON", "DECA_100_P_POTOON.wol"),
            new LevelInfo(0x00000AD4, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/DECATHLON/DECA_103_P_BOAT", "DECA_103_P_BOAT.wol"),
            new LevelInfo(0x000009B1, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/DECATHLON/DECA_110_S_MOON", "DECA_110_S_MOON.wol"),
            new LevelInfo(0x00000A8D, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/DECATHLON/DECA_120_A_JUNGLE", "DECA_120_A_JUNGLE.wol"),
            new LevelInfo(0x00000A68, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/PHOTO/PHO_122_A_JUNGLE", "PHO_122_A_JUNGLE.wol"),
            new LevelInfo(0x0000118B, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/PHOTO/PHO_132_E_MISSILE", "PHO_132_E_MISSILE.wol"),
            new LevelInfo(0x00001209, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/PHOTO/PHO_142_M_HALL", "PHO_142_M_HALL.wol"),
            new LevelInfo(0x00001389, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/PHOTO/PHO_143_M_CORIDOR", "PHO_143_M_CORIDOR.wol"),
            new LevelInfo(0x00001070, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/SHOOT/SHO_101_P_BOAT", "SHO_101_P_BOAT.wol"),
            new LevelInfo(0x00000DA0, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/SHOOT/SHO_111_S_FLOORS", "SHO_111_S_FLOORS.wol"),
            new LevelInfo(0x000009A3, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/SHOOT/SHO_121_A_JUNGLE", "SHO_121_A_JUNGLE.wol"),
            new LevelInfo(0x000008F0, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/SHOOT/SHO_133_E_CLIFFJUNGLE", "SHO_133_E_CLIFFJUNGLE.wol"),
            new LevelInfo(0x00000B50, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ZOOKEEPER/ZOO_113_S_CONTROLBASE", "ZOO_113_S_CONTROLBASE.wol"),
            new LevelInfo(0x00000A0F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ZOOKEEPER/ZOO_123_A_RUINS", "ZOO_123_A_RUINS.wol"),
            new LevelInfo(0x00000AFD, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ZOOKEEPER/ZOO_131_E_BADBASE", "ZOO_131_E_BADBASE.wol"),
            new LevelInfo(0x00000BA9, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ZOOKEEPER/ZOO_141_M_HALL", "ZOO_141_M_HALL.wol"),
            new LevelInfo(0x000006A1, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MENUS/MENU_001_MAIN", "MENU_001_MAIN.wol"),
        };

		public override string[] BFFiles => new string[] {
            "MP08.bf"
        };
	}
}
