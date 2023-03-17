using System;

namespace Ray1Map.Jade {
	[Flags]
	public enum AI_VarInfoFlags : ushort {
		None = 0,

		Private = 0x0001,
		Enum    = 0x0002,
		Sep     = 0x0004,
		Help    = 0x0008,
		Save    = 0x0010,
		Reinit  = 0x0020,
		SaveAl  = 0x0040,
		ByRef   = 0x0080,
		Pointer = 0x0100,

		Unknown09 = 0x0200,
		Unknown10 = 0x0400,
		Unknown11 = 0x0800,
		Unknown12 = 0x1000,
		Unknown13 = 0x2000,
		Unknown14 = 0x4000,
		Unknown15 = 0x8000,
	}
}
