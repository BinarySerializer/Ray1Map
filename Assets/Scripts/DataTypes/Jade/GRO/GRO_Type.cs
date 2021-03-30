using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	// TODO: Use similar system to AI_Links - this array is probably different for other games
	public enum GRO_Type : int {
		None = -1,
		Unknown = 0,
		GEO = 1,
		LIGHT = 2,
		MAT_SIN = 3, // Single
		MAT_MSM = 4, // Multi
		MAT_MTT = 5, // MultiTexture
		CAM = 6,
		WAY = 7,
		GEO_StaticLOD = 8,
		Unknown9 = 9,
		STR = 10,
		PAG = 11,

	}
}
