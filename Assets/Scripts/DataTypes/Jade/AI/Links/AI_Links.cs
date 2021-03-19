using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public partial class AI_Links {

		public AI_FunctionDef[] FunctionDefs { get; protected set; }

		public AI_Link[] Categories { get; protected set; }
		public AI_Link[] Types { get; protected set; }
		public AI_Link[] Keywords { get; protected set; }
		public AI_Link[] Functions { get; protected set; }
		public AI_Link[] Fields { get; protected set; }

		public Dictionary<uint, AI_Link> Links { get; protected set; }
		public Dictionary<uint, AI_FunctionDef> CompiledFunctions { get; protected set; }
		private void CreateDictionaries() {
			Links = new Dictionary<uint, AI_Link>();
			foreach (var l in Categories) Links[l.Key] = l;
			foreach (var l in Types) Links[l.Key] = l;
			foreach (var l in Keywords) Links[l.Key] = l;
			foreach (var l in Functions) Links[l.Key] = l;
			foreach (var l in Fields) Links[l.Key] = l;
			CompiledFunctions = new Dictionary<uint, AI_FunctionDef>();
			foreach(var l in FunctionDefs) CompiledFunctions[l.Key] = l;
		}

		public static AI_Links GetAILinks(GameSettings settings) {
			AI_Links links = null;
			switch (settings.GameModeSelection) {
				case GameModeSelection.RaymanRavingRabbidsPC:
				case GameModeSelection.RaymanRavingRabbidsXbox360:
					links = RRR_Wii;
					break;
			}
			if (links != null) {
				links.CreateDictionaries();
			}
			return links;
		}
	}
}
