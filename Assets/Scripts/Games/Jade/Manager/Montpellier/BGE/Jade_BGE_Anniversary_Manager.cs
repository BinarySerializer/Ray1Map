using Ray1Map.Jade;

namespace Ray1Map
{
    public class Jade_BGE_Anniversary_Manager : Jade_BGE_Manager 
    {
        // Levels
        public override LevelInfo[] LevelInfos => new LevelInfo[] {
            new LevelInfo(0x5E0084DF, "ROOT/EngineDatas/06 Levels/_main/_main_fix", "_main_fix.wol", type: LevelInfo.FileType.WOLUnbinarized),
			new LevelInfo(0x100045A2, "ROOT/EngineDatas/06 Levels/00_home/00_03_dehors_maison_Intro", "00_03_dehors_maison_Intro.wol", type: LevelInfo.FileType.WOLUnbinarized),
			new LevelInfo(0x02002953, "ROOT/EngineDatas/06 Levels/10_Lune/10_04_Lune_cachot_Toyl", "10_04_Lune_cachot_Toyl.wol", type: LevelInfo.FileType.WOLUnbinarized),
			new LevelInfo(0x020022E6, "ROOT/EngineDatas/06 Levels/10_Lune/10_00_lune_boyaux", "10_00_lune_boyaux.wol", type: LevelInfo.FileType.WOLUnbinarized),
			new LevelInfo(0x620017AB, "ROOT/EngineDatas/06 Levels/100_test graphiques/hill_batis_test", "Hill_bathis_test.wol", type: LevelInfo.FileType.WOLUnbinarized),
			new LevelInfo(0x10002F72, "ROOT/EngineDatas/06 Levels/06_Animaux/06_11_animaux_chez_nino", "06_11_animaux_chez_nino.wol", type: LevelInfo.FileType.WOLUnbinarized),
			new LevelInfo(0x62000988, "ROOT/EngineDatas/06 Levels/04_vaisseau/04_00_vaisseau_hyllis_planete", "04_00_vaisseau_hyllis_planete.wol", type: LevelInfo.FileType.WOLUnbinarized),
			new LevelInfo(0x3D00170C, "ROOT/EngineDatas/06 Levels/04_vaisseau/04_04_hyllis_minimap_tesla", "04_04_hyllis_minimap_tesla.wol", type: LevelInfo.FileType.WOLUnbinarized),

		};

        // Version properties
        public override string[] PakFiles => new string[] {
			"Data/Paks/Resources.pak",
			//"Data/Paks/Audio-common.pak",
			//"Data/Paks/Audio-en_US.pak",
			"Data/Paks/Text-en_US.pak",
		};
        public override string JadeSpePath => "Config/jade.spe";
		public override bool HasUnbinarizedData => true;

		public override string[] BFFiles => new string[] { };

		public override uint? PresetUniverseKey => 0x71003FF9;
	}
}