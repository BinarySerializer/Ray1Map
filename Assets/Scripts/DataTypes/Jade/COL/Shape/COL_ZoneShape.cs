using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	[Flags]
	public enum COL_ZoneShape : byte {
		Unknown = 0,
		Box = 1,
		Sphere = 2,
		Cylinder = 3,
		Triangles = 5
	}
}
