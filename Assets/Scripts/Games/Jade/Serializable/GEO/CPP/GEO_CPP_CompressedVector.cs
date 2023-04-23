using BinarySerializer;
using System;

namespace Ray1Map.Jade {
	public class GEO_CPP_CompressedVector : BinarySerializable, ISerializerShortLog {
		public GEO_CPP_VertexBuffer.CompressionMode Pre_Compression { get; set; }

		public GEO_CompressedFloat X { get; set; }
		public GEO_CompressedFloat Y { get; set; }
		public GEO_CompressedFloat Z { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			X = s.SerializeObject<GEO_CompressedFloat>(X, onPreSerialize: cf => cf.Pre_Compression = Pre_Compression, name: nameof(X));
			Y = s.SerializeObject<GEO_CompressedFloat>(Y, onPreSerialize: cf => cf.Pre_Compression = Pre_Compression, name: nameof(Y));
			Z = s.SerializeObject<GEO_CompressedFloat>(Z, onPreSerialize: cf => cf.Pre_Compression = Pre_Compression, name: nameof(Z));
		}

		public override string ToString() => $"GEO_CPP_CompressedVector({X}, {Y}, {Z})";

		public string ShortLog => ToString();
	}
}
