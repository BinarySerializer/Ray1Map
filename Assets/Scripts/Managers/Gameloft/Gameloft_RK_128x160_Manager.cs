using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Linq;

namespace R1Engine
{
	public class Gameloft_RK_128x160_Manager : Gameloft_RK_Manager {
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
		};

		public override string GetLevelPath(GameSettings settings) => (22+(settings.Level/2)).ToString();
		public override int GetLevelResourceIndex(GameSettings settings) => settings.Level % 2;
		public override int BasePuppetsResourceFile => 10;
		public override int PuppetsPerResourceFile => 10;
		public override int PuppetCount => 64;
		public override int ExtraPuppetsInLastFile => 4;
		public override int LocalizationResourceFile => 18;

		public override string[] SingleResourceFiles => new string[] {
		};
	}
}