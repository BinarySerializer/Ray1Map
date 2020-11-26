using System;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class LUDI_GlobalOffsetTable {
        public LUDI_BaseDataFile[] Files { get; set; }

        public Pointer Resolve(GBC_Offset offset) {
			LUDI_BaseDataFile file = Files.FirstOrDefault(f => f.MatchesOffset(offset));
			return file?.Resolve(offset);
		}
        public Pointer Resolve(ushort unkFileID, ushort fileID, ushort blockID) {
			LUDI_BaseDataFile file = Files.FirstOrDefault(f => f.UnkFileID == unkFileID && f.FileID == fileID);
			return file?.Resolve(blockID);
		}

		public uint? GetBlockLength(LUDI_BlockHeader blockHeader) {
			LUDI_BaseDataFile file = Files.FirstOrDefault(f => f.Offset.file == blockHeader.Offset.file);
			return file?.GetLength(blockHeader.BlockID);
		}
	}
}