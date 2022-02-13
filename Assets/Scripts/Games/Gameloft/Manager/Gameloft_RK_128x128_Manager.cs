using System.Linq;

namespace Ray1Map.Gameloft
{
	public class Gameloft_RK_128x128_Manager : Gameloft_RK_Manager {
		public override string[] ResourceFiles => new string[] {
			"0",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
		};

		public override string GetLevelPath(int level) => (6).ToString();
		public override int GetLevelResourceIndex(int level) => level;
		public override int BasePuppetsResourceFile => 3;
		public override int PuppetsPerResourceFile => 16;
		public override int PuppetCount => 16;
		public override int ExtraPuppetsInLastFile => 0;
		public override int LocalizationResourceFile => 5;

		public override PuppetReference[] PuppetReferences => Enumerable.Range(0, PuppetCount - ExtraPuppetsInLastFile).Select(pi => new PuppetReference() {
			FileIndex = GetPuppetFileIndex(pi),
			ResourceIndex = pi == 8 ? 24 : GetPuppetResourceIndex(pi) // Hack. Index 8 is never used it seems, and it's not a puppet
		}).ToArray();

		public override string[] SingleResourceFiles => new string[] {
		};
		public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
		{
			new GameInfo_World(0, Enumerable.Range(0, 4).ToArray()),
		});
	}
}