using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Linq;

namespace R1Engine
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

		public override string GetLevelPath(GameSettings settings) => (6).ToString();
		public override int GetLevelResourceIndex(GameSettings settings) => settings.Level;
		public override int BasePuppetsResourceFile => 10;
		public override int PuppetsPerResourceFile => 10;
		public override int PuppetCount => 64;
		public override int ExtraPuppetsInLastFile => 4;

		public override string[] SingleResourceFiles => new string[] {
		};
	}
}