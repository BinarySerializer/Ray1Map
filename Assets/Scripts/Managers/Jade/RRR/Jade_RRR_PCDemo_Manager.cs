namespace R1Engine
{
    public class Jade_RRR_PCDemo_Manager : Jade_BaseManager 
    {
        // Levels
        public override LevelInfo[] LevelInfos => new LevelInfo[]
        {
            new LevelInfo(0x0002EA59, "ROOT/EngineDatas/06 Levels/_main/_main_bootup", "_main_bootup.wol"),
            new LevelInfo(0x000129BE, "ROOT/EngineDatas/06 Levels/_main/_main_credits", "_main_credits.wol"),
            new LevelInfo(0x00000B84, "ROOT/EngineDatas/06 Levels/_main/_main_Menu_Interface", "_main_Menu_Interface.wol"),
            new LevelInfo(0x000053C5, "ROOT/EngineDatas/06 Levels/PRODUCTION_Danses/MGDANSE01_PulpFiction_Easy/MGDANSE01_PulpFiction_Easy_LD", "MGDANSE01_PulpFiction_Easy_LD.wol"),
            new LevelInfo(0x00011497, "ROOT/EngineDatas/06 Levels/PRODUCTION_GLADIATOR/GLADIATOR_ARENE", "GLADIATOR_ARENE.wol"),
            new LevelInfo(0x00001435, "ROOT/EngineDatas/06 Levels/PRODUCTION_GLADIATOR/GLADIATOR_CACHOT", "GLADIATOR_CACHOT.wol"),
            new LevelInfo(0x00003349, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_FPS/FPS00_TUTORIAL/FPS00_TUTORIAL_LD", "FPS00_TUTORIAL_LD.wol"),
            new LevelInfo(0x000272B9, "ROOT/EngineDatas/06 Levels/PRODUCTION_Minigames/MG02_Course_atoll", "RIV_course_atoll.wol"),
            new LevelInfo(0x000272B5, "ROOT/EngineDatas/06 Levels/PRODUCTION_Minigames/MG03_Lancer_vache", "RIV_lancer_vache.wol"),
            new LevelInfo(0x0000566A, "ROOT/EngineDatas/06 Levels/PRODUCTION_Minigames/MG19_Resto_lapins", "MG19_Resto_lapins.wol"),
        };

        // Version properties
        public override string[] BFFiles => new string[] {
            "Rayman4.bf"
        };
    }
}