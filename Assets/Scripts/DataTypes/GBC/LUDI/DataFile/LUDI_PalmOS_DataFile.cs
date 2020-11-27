namespace R1Engine {
	public class LUDI_PalmOS_DataFile : LUDI_BaseDataFile {
		// Serialized properties
		public Palm_Database Database { get; set; }
		public LUDI_AppInfo AppInfo { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Database = s.SerializeObject<Palm_Database>(Database, onPreSerialize: pdb => pdb.Type = Palm_Database.DatabaseType.PDB, name: nameof(Database));
			s.DoAt(Database.AppInfoAreaPointer, () => {
				AppInfo = s.SerializeObject<LUDI_AppInfo>(AppInfo, name: nameof(AppInfo));
			});
		}

		// Implemented properties & methods
		public override LUDI_FileIdentifier FileID => AppInfo.Header.FileID;
		public override LUDI_OffsetTable OffsetTable => AppInfo.OffsetTable;
		public override LUDI_DataInfo DataInfo => AppInfo.DataInfo;

		private Palm_DatabaseRecord GetRecord(ushort blockID) {
			if (OffsetTable != null) {
				if (!OffsetTable.EntriesDictionary.ContainsKey(blockID)) return null;
				var entryIndex = OffsetTable.EntriesDictionary[blockID];
				uint recordID = OffsetTable.Entries[entryIndex].RecordID;
				if (!Database.RecordsDictionary.ContainsKey(recordID)) return null;
				return Database.RecordsDictionary[recordID];
			} else if (DataInfo != null) {
				if(blockID > DataInfo.NumDataBlocks) return null;
				var recordID = (uint)0x100000 | (uint)(blockID-1);
				if (!Database.RecordsDictionary.ContainsKey(recordID)) return null;
				return Database.RecordsDictionary[recordID];
			}
			return null;
		}

		public override Pointer Resolve(ushort blockID) {
			return GetRecord(blockID)?.DataPointer;
		}

		public override uint? GetLength(ushort blockID) {
			if(DataInfo != null) return DataInfo.DataSize + 4;
			return GetRecord(blockID)?.Length;
		}
	}
}
