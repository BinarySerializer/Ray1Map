namespace R1Engine {
	public class PalmOS_DataFile : LUDI_BaseDataFile {
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
		public override ushort UnkFileID => AppInfo.Header.Unknown;
		public override ushort FileID => AppInfo.Header.FileIndex;
		public override LUDI_OffsetTable OffsetTable => AppInfo.OffsetTable;

		private Palm_DatabaseRecord GetRecord(ushort blockID) {
			if (OffsetTable == null) return null;
			if (!OffsetTable.EntriesDictionary.ContainsKey(blockID)) return null;
			uint recordID = OffsetTable.EntriesDictionary[blockID].RecordID;
			if (!Database.RecordsDictionary.ContainsKey(recordID)) return null;
			return Database.RecordsDictionary[recordID];
		}

		public override Pointer Resolve(ushort blockID) {
			return GetRecord(blockID)?.DataPointer;
		}

		public override uint? GetLength(ushort blockID) {
			return GetRecord(blockID)?.Length;
		}
	}
}
