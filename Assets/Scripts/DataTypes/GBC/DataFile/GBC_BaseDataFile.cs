namespace R1Engine {
	public abstract class GBC_BaseDataFile : R1Serializable {
		public abstract ushort UnkFileIndex { get; }
		public abstract ushort FileIndex { get; }
		public abstract LUDI_OffsetTable OffsetTable { get; }
		public virtual bool MatchesPointer(GBC_Pointer pointer) => 
			pointer != null && pointer.UnkFileIndex == UnkFileIndex && pointer.FileIndex == FileIndex;

		public virtual Pointer Resolve(GBC_Pointer pointer) {
			if (!MatchesPointer(pointer)) return null;
			return Resolve(pointer.BlockIndex);
		}
		public abstract Pointer Resolve(ushort blockIndex);

	}
}
