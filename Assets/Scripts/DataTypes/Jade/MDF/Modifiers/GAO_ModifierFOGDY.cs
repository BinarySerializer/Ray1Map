using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierFOGDY : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint Version { get; set; }
		public float Float_08 { get; set; }
		public float Float_0C { get; set; }
		public float Float_10 { get; set; }

		public uint V1_UInt_0 { get; set; }
		public uint V1_UInt_1 { get; set; }
		public uint V1_UInt_2 { get; set; }
		public float V1_Float_3 { get; set; }
		public float V1_Float_4 { get; set; }
		public float V1_Float_5 { get; set; }
		public float V1_Float_6 { get; set; }
		public float V1_Float_7 { get; set; }
		public float V1_Float_8 { get; set; }
		public float V1_Float_9 { get; set; }
		public float V1_Float_10 { get; set; }
		public float V1_Float_11 { get; set; }
		public float V1_Float_12 { get; set; }
		public float V1_Float_13 { get; set; }
		public float V1_Float_14 { get; set; }
		public float V1_Float_15 { get; set; }

		public float V2_Float_0 { get; set; }
		public uint V2_UInt_1 { get; set; }
		public float V2_Float_2 { get; set; }

		public float V4_Float { get; set; }
		public uint V5_UInt { get; set; }
		public float V6_Float { get; set; }
		public float V7_Float { get; set; }
		public float V8_Float { get; set; }

		public uint V9_UInt_0 { get; set; }
		public uint V9_UInt_1 { get; set; }

		public uint V10_UInt_0 { get; set; }
		public uint V10_UInt_1 { get; set; }
		public uint V10_UInt_2 { get; set; }

		public float V11_Float_0 { get; set; }
		public float V11_Float_1 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version < 3) {
				Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
				Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
				Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
			}
			if (Version >= 1) {
				V1_UInt_0 = s.Serialize<uint>(V1_UInt_0, name: nameof(V1_UInt_0));
				V1_UInt_1 = s.Serialize<uint>(V1_UInt_1, name: nameof(V1_UInt_1));
				V1_UInt_2 = s.Serialize<uint>(V1_UInt_2, name: nameof(V1_UInt_2));
				V1_Float_3  = s.Serialize<float>(V1_Float_3 , name: nameof(V1_Float_3 ));
				V1_Float_4  = s.Serialize<float>(V1_Float_4 , name: nameof(V1_Float_4 ));
				V1_Float_5  = s.Serialize<float>(V1_Float_5 , name: nameof(V1_Float_5 ));
				V1_Float_6  = s.Serialize<float>(V1_Float_6 , name: nameof(V1_Float_6 ));
				V1_Float_7  = s.Serialize<float>(V1_Float_7 , name: nameof(V1_Float_7 ));
				V1_Float_8  = s.Serialize<float>(V1_Float_8 , name: nameof(V1_Float_8 ));
				V1_Float_9  = s.Serialize<float>(V1_Float_9 , name: nameof(V1_Float_9 ));
				V1_Float_10 = s.Serialize<float>(V1_Float_10, name: nameof(V1_Float_10));
				V1_Float_11 = s.Serialize<float>(V1_Float_11, name: nameof(V1_Float_11));
				V1_Float_12 = s.Serialize<float>(V1_Float_12, name: nameof(V1_Float_12));
				V1_Float_13 = s.Serialize<float>(V1_Float_13, name: nameof(V1_Float_13));
				V1_Float_14 = s.Serialize<float>(V1_Float_14, name: nameof(V1_Float_14));
				V1_Float_15 = s.Serialize<float>(V1_Float_15, name: nameof(V1_Float_15));
			}
			if (Version >= 2) {
				V2_Float_0 = s.Serialize<float>(V2_Float_0, name: nameof(V2_Float_0));
				V2_UInt_1 = s.Serialize<uint>(V2_UInt_1, name: nameof(V2_UInt_1));
				V2_Float_2 = s.Serialize<float>(V2_Float_2, name: nameof(V2_Float_2));
			}
			if (Version >= 4) V4_Float = s.Serialize<float>(V4_Float, name: nameof(V4_Float));
			if (Version >= 5) V5_UInt = s.Serialize<uint>(V5_UInt, name: nameof(V5_UInt));
			if (Version >= 6) V6_Float = s.Serialize<float>(V6_Float, name: nameof(V6_Float));
			if (Version >= 7) V7_Float = s.Serialize<float>(V7_Float, name: nameof(V7_Float));
			if (Version >= 8) V8_Float = s.Serialize<float>(V8_Float, name: nameof(V8_Float));
			if (Version >= 9) {
				V9_UInt_0 = s.Serialize<uint>(V9_UInt_0, name: nameof(V9_UInt_0));
				V9_UInt_1 = s.Serialize<uint>(V9_UInt_1, name: nameof(V9_UInt_1));
			}
			if (Version >= 10) {
				V10_UInt_0 = s.Serialize<uint>(V10_UInt_0, name: nameof(V10_UInt_0));
				V10_UInt_1 = s.Serialize<uint>(V10_UInt_1, name: nameof(V10_UInt_1));
				V10_UInt_2 = s.Serialize<uint>(V10_UInt_2, name: nameof(V10_UInt_2));
			}
			if (Version >= 11) {
				V11_Float_0 = s.Serialize<float>(V11_Float_0, name: nameof(V11_Float_0));
				V11_Float_1 = s.Serialize<float>(V11_Float_1, name: nameof(V11_Float_1));
			}
		}
	}
}
