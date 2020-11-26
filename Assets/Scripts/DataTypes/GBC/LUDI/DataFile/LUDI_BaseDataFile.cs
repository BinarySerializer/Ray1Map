namespace R1Engine {
	public abstract class LUDI_BaseDataFile : R1Serializable {
		public abstract ushort UnkFileID { get; }
		public abstract ushort FileID { get; }
		public abstract LUDI_OffsetTable OffsetTable { get; }
		public virtual bool MatchesOffset(GBC_Offset offset) => 
			offset != null && offset.UnkFileID == UnkFileID && offset.FileID == FileID;

		public virtual Pointer Resolve(GBC_Offset offset) {
			if (!MatchesOffset(offset)) return null;
			return Resolve(offset.BlockID);
		}
		public abstract Pointer Resolve(ushort blockID);

		public abstract uint? GetLength(ushort blockID);
	}
}
