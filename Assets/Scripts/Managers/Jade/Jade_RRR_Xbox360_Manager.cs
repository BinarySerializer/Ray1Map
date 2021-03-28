namespace R1Engine
{
    public class Jade_RRR_Xbox360_Manager : Jade_BaseManager 
    {
		// Levels
		public override LevelInfo[] LevelInfos => null;

        // Version properties
		public override string[] BFFiles => new string[] {
			"RM4Maps.bf",
			"RM4Textures.bf",
			"Sound/Sound_Common.bf"
		};
	}
}
