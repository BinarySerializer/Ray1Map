namespace R1Engine {
	public abstract class LUDI_BaseDataFile : R1Serializable {
		public abstract LUDI_FileIdentifier FileID { get; }
		public abstract LUDI_OffsetTable OffsetTable { get; }
		public abstract LUDI_DataInfo DataInfo { get; }
		public virtual bool MatchesOffset(GBC_Offset offset) => FileID.Match(offset.FileID);

		public virtual Pointer Resolve(GBC_Offset offset) {
			if (!MatchesOffset(offset)) return null;
			return Resolve(offset.BlockID.BlockID);
		}
		public abstract Pointer Resolve(ushort blockID);

		public abstract uint? GetLength(ushort blockID);

		public virtual uint BlockCount => OffsetTable?.NumEntries ?? DataInfo?.NumDataBlocks ?? 0;
	}
}
