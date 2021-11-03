﻿namespace Ray1Map.Gameloft
{
	public class Gameloft_RK_240x320_s40v3a_Manager : Gameloft_RK_Manager {
		public override string[] ResourceFiles => new string[] {
			"0",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9",
			"10",
			"11",
			"12",
			"13",
			"14",
			"15",
			"16",
			"17",
			"18",
			"19",
			"20",
			"21",
			"22",
			"23",
			"24",
			"25",
			"26",
			"27",
			"28",
			"29",
			"30",
			"31",
			"32"
		};

		public override string GetLevelPath(int level) => (32).ToString();
		public override int GetLevelResourceIndex(int level) => level;
		public override int BasePuppetsResourceFile => 10;
		public override int PuppetsPerResourceFile => 3;
		public override int PuppetCount => 62;
		public override int ExtraPuppetsInLastFile => 14;
		public override int LocalizationResourceFile => 28;

		public override string[] SingleResourceFiles => new string[] {
		};
	}
}