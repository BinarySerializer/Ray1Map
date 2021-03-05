using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Linq;

namespace R1Engine
{
	public class Gameloft_RK_128x128_s40v2_Manager : Gameloft_RK_Manager {
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
		};

		public override string GetLevelPath(GameSettings settings) => (10).ToString();
		public override int GetLevelResourceIndex(GameSettings settings) => settings.Level;
		public override int BasePuppetsResourceFile => 5;
		public override int PuppetsPerResourceFile => 51;
		public override int PuppetCount => 51;
		public override int ExtraPuppetsInLastFile => 0;

		public override string[] SingleResourceFiles => new string[] {
		};
		public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
		{
			new GameInfo_World(0, Enumerable.Range(0, 4).ToArray()),
		});
	}
}