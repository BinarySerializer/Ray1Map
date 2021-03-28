using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public enum Jade_Code : uint {
		Unknown = 0,
		ACBD = 0xAACCBBDD,
		Code6660 = 0xC0DE6660,
		All6 = 0x66666666,
		RLI = 0x494C5280, // Spells out "RLI\x80"
	}
}
