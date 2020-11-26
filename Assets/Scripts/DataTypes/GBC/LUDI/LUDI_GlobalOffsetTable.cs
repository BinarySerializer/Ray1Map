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
			LUDI_BaseDataFile file = Files.FirstOrDefault(f => f.FileID.FileID == fileID && f.FileID.Unknown == unkFileID);
			return file?.Resolve(blockID);
		}

		public uint? GetBlockLength(LUDI_BlockIdentifier blockHeader) {
			LUDI_BaseDataFile file = Files.FirstOrDefault(f => f.Offset.file == blockHeader.Offset.file);
			return file?.GetLength(blockHeader.BlockID);
		}
	}
}