using BinarySerializer;

namespace Ray1Map.GBC {
	public abstract class LUDI_BaseDataFile : BinarySerializable {
		public abstract LUDI_FileIdentifier FileID { get; }
		public abstract LUDI_OffsetTable OffsetTable { get; }
		public abstract LUDI_DataInfo DataInfo { get; }
		public virtual bool MatchesDependency(GBC_Dependency dependency) => FileID.Match(dependency.FileID);

		public virtual Pointer Resolve(GBC_Dependency dependency) {
			if (!MatchesDependency(dependency)) return null;
			return Resolve(dependency.BlockID.BlockID);
		}
		public abstract Pointer Resolve(ushort blockID);

		public abstract long? GetLength(ushort blockID);

		public virtual uint BlockCount => OffsetTable?.NumEntries ?? DataInfo?.NumDataBlocks ?? 0;
	}
}
