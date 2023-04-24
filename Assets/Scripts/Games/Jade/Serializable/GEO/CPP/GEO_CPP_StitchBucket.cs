using BinarySerializer;
using System;

namespace Ray1Map.Jade {
	public class GEO_CPP_StitchBucket : BinarySerializable {
		public ushort[] StitchMatrices { get; set; }
		public GEO_CPP_IndexBuffer IndexBuffer { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			StitchMatrices = s.SerializeArray<ushort>(StitchMatrices, 10, name: nameof(StitchMatrices));
			IndexBuffer = s.SerializeObject<GEO_CPP_IndexBuffer>(IndexBuffer, name: nameof(IndexBuffer));
		}
	}
}
