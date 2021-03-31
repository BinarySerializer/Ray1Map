using BinarySerializer;

namespace R1Engine.Jade {
	public class Jade_Matrix : BinarySerializable {

		// Format: M{Column}{Row}
		public float M00 { get; set; }
		public float M01 { get; set; }
		public float M02 { get; set; }
		public float M03 { get; set; }

		public float M10 { get; set; }
		public float M11 { get; set; }
		public float M12 { get; set; }
		public float M13 { get; set; }

		public float M20 { get; set; }
		public float M21 { get; set; }
		public float M22 { get; set; }
		public float M23 { get; set; }

		public float M30 { get; set; }
		public float M31 { get; set; }
		public float M32 { get; set; }
		public float M33 { get; set; }

		public int Type { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			M00 = s.Serialize<float>(M00, name: nameof(M00));
			M01 = s.Serialize<float>(M01, name: nameof(M01));
			M02 = s.Serialize<float>(M02, name: nameof(M02));
			M03 = s.Serialize<float>(M03, name: nameof(M03));

			M10 = s.Serialize<float>(M10, name: nameof(M10));
			M11 = s.Serialize<float>(M11, name: nameof(M11));
			M12 = s.Serialize<float>(M12, name: nameof(M12));
			M13 = s.Serialize<float>(M13, name: nameof(M13));

			M20 = s.Serialize<float>(M20, name: nameof(M20));
			M21 = s.Serialize<float>(M21, name: nameof(M21));
			M22 = s.Serialize<float>(M22, name: nameof(M22));
			M23 = s.Serialize<float>(M23, name: nameof(M23));

			M30 = s.Serialize<float>(M30, name: nameof(M30));
			M31 = s.Serialize<float>(M31, name: nameof(M31));
			M32 = s.Serialize<float>(M32, name: nameof(M32));
			M33 = s.Serialize<float>(M33, name: nameof(M33));

			Type = s.Serialize<int>(Type, name: nameof(Type));
		}
	}
}
