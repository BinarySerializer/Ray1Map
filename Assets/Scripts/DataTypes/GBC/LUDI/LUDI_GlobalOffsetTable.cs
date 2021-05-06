using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    public class LUDI_GlobalOffsetTable {
        public LUDI_BaseDataFile[] Files { get; set; }

        public Pointer Resolve(GBC_Dependency dependency) {
			LUDI_BaseDataFile file = Files.FirstOrDefault(f => f.MatchesDependency(dependency));
			return file?.Resolve(dependency);
		}
        public Pointer Resolve(ushort unkFileID, ushort fileID, ushort blockID) {
			LUDI_BaseDataFile file = Files.FirstOrDefault(f => f.FileID.FileID == fileID && f.FileID.Unknown == unkFileID);
			return file?.Resolve(blockID);
		}

		public long? GetBlockLength(LUDI_BlockIdentifier blockHeader) {
			LUDI_BaseDataFile file = Files.FirstOrDefault(f => f.Offset.File == blockHeader.Offset.File);
			return file?.GetLength(blockHeader.BlockID);
		}
	}
}