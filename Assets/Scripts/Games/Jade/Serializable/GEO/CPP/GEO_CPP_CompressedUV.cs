using BinarySerializer;
using System;

namespace Ray1Map.Jade {
	public class GEO_CPP_CompressedUV : BinarySerializable, ISerializerShortLog {
		public GEO_CPP_VertexBuffer.CompressionMode Pre_Compression { get; set; }

		public GEO_CompressedFloat U { get; set; }
		public GEO_CompressedFloat V { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			U = s.SerializeObject<GEO_CompressedFloat>(U, onPreSerialize: cf => cf.Pre_Compression = Pre_Compression, name: nameof(U));
			V = s.SerializeObject<GEO_CompressedFloat>(V, onPreSerialize: cf => cf.Pre_Compression = Pre_Compression, name: nameof(V));
		}

		public override string ToString() => $"GEO_CPP_CompressedUV({U}, {V})";

		public string ShortLog => ToString();


		public GEO_GeometricObject.UV ToUV(GEO_CompressedFloat.FloatType type) {
			return new GEO_GeometricObject.UV(U.ToFloat(type), V.ToFloat(type));
		}
	}
}
