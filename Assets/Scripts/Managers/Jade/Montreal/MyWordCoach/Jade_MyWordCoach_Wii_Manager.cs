using Cysharp.Threading.Tasks;
using R1Engine.Jade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Jade_MyWordCoach_Wii_Manager : Jade_Montpellier_BaseManager {

		// Game actions
		public override GameAction[] GetGameActions(GameSettings settings) {
			return base.GetGameActions(settings).Concat(new GameAction[]
			{
				new GameAction("Export textures (unbinarized)", false, true, (input, output) => ExportTexturesUnbinarized(settings, output)),
			}).ToArray();
		}

		// Levels
		public override LevelInfo[] LevelInfos => null;

		public override string[] BFFiles => new string[] {
			"DATA/spree.bf"
		};
	}
}
