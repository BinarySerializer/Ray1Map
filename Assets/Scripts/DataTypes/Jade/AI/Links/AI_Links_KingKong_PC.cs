using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public class AI_Links_KingKong_PC : AI_Links_KingKong_GC {
		protected override void InitFunctionDefs() {
			base.InitFunctionDefs();

			#region Function Defs (Unnamed)
			uint[] overrides = new uint[] {
				0xBE0000FB,
				0xBE0000FC,
				0xBE0000FE,
				0xBE000100,
				0xBE000102,
			};
			#endregion

			HashSet<uint> fdLookup = new HashSet<uint>(FunctionDefs.Select(fd => fd.Key));
			List<uint> addedKeys = new List<uint>();
			foreach (var u in overrides) {
				if(fdLookup.Contains(u)) continue;
				addedKeys.Add(u);
			}
			if (addedKeys.Any()) {
				var fdefs = FunctionDefs;
				int len = fdefs.Length;
				Array.Resize(ref fdefs, len + addedKeys.Count);
				for (int i = 0; i < addedKeys.Count; i++) {
					fdefs[len + i] = new AI_FunctionDef(addedKeys[i], $"Custom_{addedKeys[i]:X8}");
				}
				FunctionDefs = fdefs;
			}
		}
	}
}
