using BinarySerializer;
using System;

namespace Ray1Map.Jade {
	public class GEO_CompressedFloat : BinarySerializable, ISerializerShortLog {
		public GEO_CPP_VertexBuffer.CompressionMode Pre_Compression { get; set; }

		public float Uncompressed { get; set; }
		public short LowCompression { get; set; }
		public sbyte HighCompression { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			switch (Pre_Compression) {
				case GEO_CPP_VertexBuffer.CompressionMode.Uncompressed:
					Uncompressed = s.Serialize<float>(Uncompressed, name: nameof(Uncompressed));
					break;
				case GEO_CPP_VertexBuffer.CompressionMode.LowCompression:
					LowCompression = s.Serialize<short>(LowCompression, name: nameof(LowCompression));
					break;
				case GEO_CPP_VertexBuffer.CompressionMode.HighCompression:
					HighCompression = s.Serialize<sbyte>(HighCompression, name: nameof(HighCompression));
					break;
			}
		}

		public override string ToString() => Pre_Compression switch {
			GEO_CPP_VertexBuffer.CompressionMode.Uncompressed => Uncompressed.ToString(),
			GEO_CPP_VertexBuffer.CompressionMode.LowCompression => LowCompression.ToString(),
			GEO_CPP_VertexBuffer.CompressionMode.HighCompression => HighCompression.ToString(),
			_ => "Unknown"
		};
		public string ShortLog => ToString();


		public float ToFloat(FloatType type) {
			switch (Pre_Compression) {
				case GEO_CPP_VertexBuffer.CompressionMode.Uncompressed:
					return Uncompressed;
				case GEO_CPP_VertexBuffer.CompressionMode.LowCompression:
					switch (type) {
						case FloatType.Vertex: return LowCompression / 512f;
						case FloatType.Normal: return LowCompression / 64f;
						case FloatType.TexCoord0: return LowCompression / 512f;//0x10000f;
						case FloatType.TexCoord1: return (LowCompression < 0 ? ((uint)0x10000 + LowCompression) : LowCompression) / 256f;//0x10000f;
					}
					break;
				case GEO_CPP_VertexBuffer.CompressionMode.HighCompression:
					switch (type) {
						case FloatType.Vertex: return HighCompression / 128f;
						case FloatType.Normal: return HighCompression / 128f;
						case FloatType.TexCoord0: return HighCompression / 512f;// 0x10000f;
						case FloatType.TexCoord1: return (HighCompression < 0 ? ((uint)0x100 + HighCompression) : HighCompression) / 256f;// 0x10000f;
					}
					break;
			}
			throw new NotImplementedException();
		}

		public enum FloatType {
			Vertex,
			Normal,
			TexCoord0,
			TexCoord1
		}
	}
}
