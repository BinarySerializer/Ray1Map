using System;

namespace Ray1Map.Jade {
	[Flags]
	public enum OBJ_GameObject_FixFlags : byte {
		None = 0,
		Unknown0      = 0x01,
		Unknown1      = 0x02,
		Unknown2      = 0x04,
		Unknown3      = 0x08,
		HasBeenMerged = 0x10,
		ProcessedAI   = 0x20,
		ProcessedDyn  = 0x40,
		ProcessedHie  = 0x80,
	}
}
