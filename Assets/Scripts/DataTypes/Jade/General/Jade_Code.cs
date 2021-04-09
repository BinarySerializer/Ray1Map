using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public enum Jade_Code : uint {
		Unknown = 0,
		ACBD = 0xAACCBBDD,
		Code2002 = 0xC0DE2002,
		Code2009 = 0xC0DE2009,
		Code6660 = 0xC0DE6660,
		CodeCode  = 0xC0DEC0DE,
		CAD01234 = 0xCAD01234,
		FF00FF = 0xFF00FF,
		All6 = 0x66666666,
		RLI = 0x494C5280, // Spells out "RLI\x80"
	}
}
