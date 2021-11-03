using BinarySerializer;

namespace Ray1Map.GBC {
	public class LUDI_PocketPC_DataFile : LUDI_BaseDataFile {
		// Serialized properties
		public LUDI_AppInfo AppInfo { get; set; }
		public Pointer BaseOffset { get; set; }
		public long TotalLength { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			AppInfo = s.SerializeObject<LUDI_AppInfo>(AppInfo, name: nameof(AppInfo));
			BaseOffset = s.CurrentPointer;
			TotalLength = s.CurrentLength - BaseOffset.FileOffset;
		}

		// Implemented properties & methods
		public override LUDI_FileIdentifier FileID => AppInfo.Header.FileID;
		public override LUDI_OffsetTable OffsetTable => AppInfo.OffsetTable;
		public override LUDI_DataInfo DataInfo => AppInfo.DataInfo;

		public override Pointer Resolve(ushort blockID) {
			if (OffsetTable != null) {
				if (!OffsetTable.EntriesDictionary.ContainsKey(blockID)) return null;
				var entryIndex = OffsetTable.EntriesDictionary[blockID];
				uint blockOffset = OffsetTable.Entries[entryIndex].BlockOffset;
				return BaseOffset + blockOffset;
			} else if (DataInfo != null) {
				if (blockID > DataInfo.NumDataBlocks) return null;
				var blockIndex = (uint)(blockID - 1);
				return BaseOffset + DataInfo.DataSize * blockIndex;
			}
			return null;
		}

		public override long? GetLength(ushort blockID) {
			if(DataInfo != null) return DataInfo.DataSize + 4;
			if (OffsetTable != null) {
				if (!OffsetTable.EntriesDictionary.ContainsKey(blockID)) return null;
				var entryIndex = OffsetTable.EntriesDictionary[blockID];
				uint blockOffset = OffsetTable.Entries[entryIndex].BlockOffset;
				long nextBlockoffset = entryIndex < OffsetTable.Entries.Length - 1 ? OffsetTable.Entries[entryIndex+1].BlockOffset : TotalLength;
				return nextBlockoffset - blockOffset;
			}
			return null;
		}
	}
}
