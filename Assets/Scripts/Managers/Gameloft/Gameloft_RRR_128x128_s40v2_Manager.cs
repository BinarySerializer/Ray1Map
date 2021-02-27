using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Gameloft_RRR_128x128_s40v2_Manager : Gameloft_RRR_Manager
    {

		public override string[] ResourceFiles => new string[] {
			"d1",
			"d2",
			"d3",
			"d4",
			"l0a01",
			"l0a02",
			"l0a03",
			"l0a04",
			"l0a05",
			"l0a06",
			"l0a07",
			"l0b",
			"s",
			"s1",
			"s2",
			"s3",
			"s4",
			"s5",
			"s6",
			"s7",
			"s8",
			"s30",
			"t0",
			"t1",
			"t2",
			"ts0",
			"ts1",
			"ts2",
		};

		public override string[] SingleResourceFiles => new string[] {
			"u",
			"sc",
			"st",
			"lj21",
			"lj22",
			"lj23",
			"lj41",
			"lj42",
			"lj43",
			"lj44",
			"g",
		};

		public override int[] HardcodedPuppetImageBufferIndices => new int[] { 0, 1, 2, 1, 3, 4, 5, 0, 6, 7, 2, 3, 8, 4, 9, 9, 10, 11, 5, 2, 5, 11, 12, 13, 14 };

		public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
		{
			new GameInfo_World(0, Enumerable.Range(0, 7).ToArray()),
		});
	}
}