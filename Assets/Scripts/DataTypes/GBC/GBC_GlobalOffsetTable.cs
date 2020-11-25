using System;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBC_GlobalOffsetTable {
        public GBC_BaseDataFile[] Files { get; set; }

        public Pointer Resolve(GBC_Offset offset) {
			GBC_BaseDataFile file = Files.FirstOrDefault(f => f.MatchesPointer(offset));
			return file?.Resolve(offset);
		}
        public Pointer Resolve(ushort unkFileIndex, ushort fileIndex, ushort blockIndex) {
			GBC_BaseDataFile file = Files.FirstOrDefault(f => f.UnkFileIndex == unkFileIndex && f.FileIndex == fileIndex);
			return file?.Resolve(blockIndex);
		}
	}
}