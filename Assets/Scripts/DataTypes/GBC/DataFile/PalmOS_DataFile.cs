namespace R1Engine {
	public class PalmOS_DataFile : GBC_BaseDataFile {
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
		public override ushort UnkFileIndex => AppInfo.Header.Unknown;
		public override ushort FileIndex => AppInfo.Header.FileIndex;
		public override LUDI_OffsetTable OffsetTable => AppInfo.OffsetTable;

		public override Pointer Resolve(ushort blockIndex) {
			if(OffsetTable == null) return null;
			if (!OffsetTable.EntriesDictionary.ContainsKey(blockIndex)) return null;
			uint recordID = OffsetTable.EntriesDictionary[blockIndex].RecordID;
			if (!Database.RecordsDictionary.ContainsKey(recordID)) return null;
			var record = Database.RecordsDictionary[recordID];
			return record.DataPointer;
		}
	}
}
