using BinarySerializer;
using System;

namespace Ray1Map.Jade {
	public class GEO_CPP_IndexBuffer : BinarySerializable {
		public uint Version { get; set; }
		public UsageType Usage { get; set; }
		public uint Count { get; set; }
		public uint Use16BitIndices { get; set; }
		public IndexType IndexTypes { get; set; }
		public uint UseStrips { get; set; }
		public uint UInt_V3_TFS { get; set; }

		public ushort[] IndicesPos { get; set; }
		public ushort[] IndicesNor { get; set; }
		public ushort[] IndicesCol { get; set; }
		public ushort[] IndicesTex0 { get; set; }
		public ushort[] IndicesTex1 { get; set; }
		public byte[] IndicesMat { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Usage = s.Serialize<UsageType>(Usage, name: nameof(Usage));
			Count = s.Serialize<uint>(Count, name: nameof(Count));
			Use16BitIndices = s.Serialize<uint>(Use16BitIndices, name: nameof(Use16BitIndices));
			IndexTypes = s.Serialize<IndexType>(IndexTypes, name: nameof(IndexTypes));
			UseStrips = s.Serialize<uint>(UseStrips, name: nameof(UseStrips));
			if (Version >= 3 && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS)) {
				UInt_V3_TFS = s.Serialize<uint>(UInt_V3_TFS, name: nameof(UInt_V3_TFS));
			}
			if(Version < (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRRTVParty) ? 3 : 4)) IndexTypes &= ~IndexType.Mat;
			if (IndexTypes.HasFlag(IndexType.Pos)) IndicesPos = s.SerializeArray<ushort>(IndicesPos, Count, name: nameof(IndicesPos));
			if (IndexTypes.HasFlag(IndexType.Nor)) IndicesNor = s.SerializeArray<ushort>(IndicesNor, Count, name: nameof(IndicesNor));
			if (IndexTypes.HasFlag(IndexType.Col)) IndicesCol = s.SerializeArray<ushort>(IndicesCol, Count, name: nameof(IndicesCol));
			if (IndexTypes.HasFlag(IndexType.Tex0)) IndicesTex0 = s.SerializeArray<ushort>(IndicesTex0, Count, name: nameof(IndicesTex0));
			if (IndexTypes.HasFlag(IndexType.Tex1)) IndicesTex1 = s.SerializeArray<ushort>(IndicesTex1, Count, name: nameof(IndicesTex1));
			if (IndexTypes.HasFlag(IndexType.Mat)) IndicesMat = s.SerializeArray<byte>(IndicesMat, Count, name: nameof(IndicesMat));
		}

		public enum UsageType : uint {
			Static = 0,
			Dynamic = 1,
		}

		[Flags]
		public enum IndexType : uint {
			None = 0,
			Pos = 1 << 0,
			Nor = 1 << 1,
			Col = 1 << 2,
			Tex0 = 1 << 3,
			Tex1 = 1 << 4,
			Mat = 1 << 5,
		}
	}
}
