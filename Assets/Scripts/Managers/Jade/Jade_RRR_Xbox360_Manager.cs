using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine {
	public class Jade_RRR_Xbox360_Manager : Jade_BaseManager {
		public override string[] BFFiles => new string[] {
			"RM4Maps.bf",
			"RM4Textures.bf",
			"Sound/Sound_Common.bf"
		};
	}
}
