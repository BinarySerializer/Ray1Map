namespace R1Engine {
	public abstract class GBC_BaseDataFile : R1Serializable {
		public abstract ushort UnkFileIndex { get; }
		public abstract ushort FileIndex { get; }
		public abstract LUDI_OffsetTable OffsetTable { get; }
		public virtual bool MatchesPointer(GBC_Offset offset) => 
			offset != null && offset.UnkFileIndex == UnkFileIndex && offset.FileIndex == FileIndex;

		public virtual Pointer Resolve(GBC_Offset offset) {
			if (!MatchesPointer(offset)) return null;
			return Resolve(offset.BlockIndex);
		}
		public abstract Pointer Resolve(ushort blockIndex);

	}
}
