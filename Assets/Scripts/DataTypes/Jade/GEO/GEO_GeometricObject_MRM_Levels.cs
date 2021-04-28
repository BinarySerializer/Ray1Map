using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in GEO_p_CreateFromBuffer
	public class GEO_GeometricObject_MRM_Levels : BinarySerializable {
		public uint Type { get; set; }
		public bool HasShortPerVertex { get; set; }
		public uint VerticesCount { get; set; }

		public float Type4_Float { get; set; }
		public uint LevelsCount { get; set; }
		public uint[] ElementCounts { get; set; }
		public float[] Thresholds { get; set; }
		public short[] VertexShorts { get; set; }

		public uint UInt_Type3_0 { get; set; }
		public uint[] UInts_Type3_0 { get; set; }
		public uint UInt_Type3_1 { get; set; }
		public uint[] UInts_Type3_1 { get; set; }

		public override void SerializeImpl(SerializerObject s) {			
			if(Type >= 4) Type4_Float = s.Serialize<float>(Type4_Float, name: nameof(Type4_Float));
			LevelsCount = s.Serialize<uint>(LevelsCount, name: nameof(LevelsCount));

			ElementCounts = s.SerializeArray<uint>(ElementCounts, LevelsCount, name: nameof(ElementCounts));
			Thresholds = s.SerializeArray<float>(Thresholds, (LevelsCount > 0) ? LevelsCount - 1 : 0, name: nameof(Thresholds));

			if(HasShortPerVertex)
				VertexShorts = s.SerializeArray<short>(VertexShorts, VerticesCount, name: nameof(VertexShorts));

			if (Type >= 3) {
				UInt_Type3_0 = s.Serialize<uint>(UInt_Type3_0, name: nameof(UInt_Type3_0));
				UInts_Type3_0 = s.SerializeArray<uint>(UInts_Type3_0, LevelsCount, name: nameof(UInts_Type3_0));
				UInt_Type3_1 = s.Serialize<uint>(UInt_Type3_1, name: nameof(UInt_Type3_1));
				UInts_Type3_1 = s.SerializeArray<uint>(UInts_Type3_1, LevelsCount, name: nameof(UInts_Type3_1));
			}
		}
	}
}
