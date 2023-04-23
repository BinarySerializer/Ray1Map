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
	}
}
