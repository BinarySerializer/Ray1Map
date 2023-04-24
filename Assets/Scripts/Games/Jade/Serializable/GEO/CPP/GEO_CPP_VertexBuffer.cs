using BinarySerializer;
using System;

namespace Ray1Map.Jade {
	public class GEO_CPP_VertexBuffer : BinarySerializable {
		public uint Version { get; set; }
		public UsageType Usage { get; set; }
		public CompressionMode Compression { get; set; } = CompressionMode.Uncompressed;
		public uint VerticesCount { get; set; }
		public uint NormalsCount { get; set; }
		public uint ColorsCount { get; set; }
		public uint Tex0Count { get; set; }
		public uint Tex1Count { get; set; }

		public GEO_CPP_CompressedVector[] Vertices { get; set; }
		public GEO_CPP_CompressedVector[] Normals { get; set; }
		public Jade_Color[] Colors { get; set; }
		public GEO_CPP_CompressedUV[] Tex0 { get; set; }
		public GEO_CPP_CompressedUV[] Tex1 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			bool isTVP = s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRRTVParty);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Usage = s.Serialize<UsageType>(Usage, name: nameof(Usage));
			if (Version >= 2) {
				Compression = s.Serialize<CompressionMode>(Compression, name: nameof(Compression));
			} else {
				Compression = CompressionMode.Uncompressed;
			}
			VerticesCount = s.Serialize<uint>(VerticesCount, name: nameof(VerticesCount));
			NormalsCount = s.Serialize<uint>(NormalsCount, name: nameof(NormalsCount));
			ColorsCount = s.Serialize<uint>(ColorsCount, name: nameof(ColorsCount));
			Tex0Count = s.Serialize<uint>(Tex0Count, name: nameof(Tex0Count));
			Tex1Count = s.Serialize<uint>(Tex1Count, name: nameof(Tex1Count));

			Vertices = s.SerializeObjectArray<GEO_CPP_CompressedVector>(Vertices, VerticesCount, onPreSerialize: v => v.Pre_Compression = Compression, name: nameof(Vertices));
			Normals  = s.SerializeObjectArray<GEO_CPP_CompressedVector>(Normals,  NormalsCount,  onPreSerialize: v => v.Pre_Compression = (Compression == CompressionMode.Uncompressed ? Compression : CompressionMode.HighCompression), name: nameof(Normals));
			Colors = s.SerializeObjectArray<Jade_Color>(Colors, ColorsCount, name: nameof(Colors));
			Tex0 = s.SerializeObjectArray<GEO_CPP_CompressedUV>(Tex0, Tex0Count, onPreSerialize: v => v.Pre_Compression = (Compression == CompressionMode.Uncompressed ? Compression : CompressionMode.LowCompression), name: nameof(Tex0));
			Tex1 = s.SerializeObjectArray<GEO_CPP_CompressedUV>(Tex1, Tex1Count, onPreSerialize: v => v.Pre_Compression = (Compression == CompressionMode.Uncompressed ? Compression : (isTVP ? CompressionMode.LowCompression : CompressionMode.HighCompression)), name: nameof(Tex1));
		}

		public enum UsageType : uint {
			Static = 0,
			CPUAccess = 1,
			Dynamic = 2,
		}
		public enum CompressionMode : uint {
			Uncompressed = 0,
			LowCompression = 1,
			HighCompression = 2
		}
	}
}
