using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GEO_p_CreateFromBuffer
	public class GEO_GeometricObject_MRM_Levels : BinarySerializable {
		public GEO_GeometricObject GeometricObject { get; set; }

		public float Type4_Float { get; set; }
		public uint LevelsCount { get; set; }
		public uint[] ElementCounts { get; set; }
		public float[] Thresholds { get; set; }
		public short[] ReorderBuffer { get; set; }

		public uint TotalVerticesCount { get; set; }
		public uint[] LevelsVerticesCount { get; set; }
		public uint TotalUVsCount { get; set; }
		public uint[] LevelsUVsCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {			
			if(GeometricObject.Version >= 4) Type4_Float = s.Serialize<float>(Type4_Float, name: nameof(Type4_Float));
			LevelsCount = s.Serialize<uint>(LevelsCount, name: nameof(LevelsCount));

			ElementCounts = s.SerializeArray<uint>(ElementCounts, LevelsCount, name: nameof(ElementCounts));
			Thresholds = s.SerializeArray<float>(Thresholds, (LevelsCount > 0) ? LevelsCount - 1 : 0, name: nameof(Thresholds));

			if(GeometricObject.HasReorderBuffer != 0)
				ReorderBuffer = s.SerializeArray<short>(ReorderBuffer, GeometricObject.VerticesCount, name: nameof(ReorderBuffer));

			if (GeometricObject.Version >= 3) {
				TotalVerticesCount = s.Serialize<uint>(TotalVerticesCount, name: nameof(TotalVerticesCount));
				LevelsVerticesCount = s.SerializeArray<uint>(LevelsVerticesCount, LevelsCount, name: nameof(LevelsVerticesCount));
				TotalUVsCount = s.Serialize<uint>(TotalUVsCount, name: nameof(TotalUVsCount));
				LevelsUVsCount = s.SerializeArray<uint>(LevelsUVsCount, LevelsCount, name: nameof(LevelsUVsCount));
			}
		}
	}
}
