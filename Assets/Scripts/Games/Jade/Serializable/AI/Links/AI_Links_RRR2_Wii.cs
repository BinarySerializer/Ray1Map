using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray1Map.Jade {
	public class AI_Links_RRR2_Wii : AI_Links_RRR_Wii {
		protected override void InitFunctionDefs() {
			FunctionDefs = new AI_FunctionDef[] { // They are uncompiled in this game :)
				new AI_FunctionDef(0xFFFFFFFF, "testtest"),
			};
		}
	}
}
