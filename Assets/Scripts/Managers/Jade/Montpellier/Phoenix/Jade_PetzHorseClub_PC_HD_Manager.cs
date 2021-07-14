using Cysharp.Threading.Tasks;
using R1Engine.Jade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Jade_PetzHorseClub_PC_HD_Manager : Jade_Montpellier_BaseManager {
		public override bool HasUnbinarizedData => true;

		// Levels
		public override LevelInfo[] LevelInfos => null;

		public override string[] BFFiles => new string[] {
			"Horsez2_HD.bf"
		};
	}
}
