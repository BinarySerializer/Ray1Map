using BinarySerializer;

namespace R1Engine.Jade {
	public class SPG2_Modifier : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint Type { get; set; }
		public uint UInt_04 { get; set; }
		public float Float_08 { get; set; }
		public float Float_0C { get; set; }
		public float Float_10 { get; set; }
		public uint UInt_14 { get; set; }
		public float Float_18 { get; set; }

		public float Type1_Float_0 { get; set; }
		public float Type1_Float_1 { get; set; }

		public uint Type2_UInt_0 { get; set; }
		public uint Type2_UInt_1 { get; set; }

		public float Type3_Float { get; set; }
		public uint Type3_UInt { get; set; }

		public float Type4_Float { get; set; }

		public float Type5_Float_00 { get; set; }
		public uint Type5_UInt_01 { get; set; }
		public uint Type5_UInt_02 { get; set; }
		public uint Type5_UInt_03 { get; set; }
		public float Type5_Float_04 { get; set; }
		public float Type5_Float_05 { get; set; }
		public float Type5_Float_06 { get; set; }
		public float Type5_Float_07 { get; set; }
		public float Type5_Float_08 { get; set; }
		public uint Type5_UInt_09 { get; set; }
		public float Type5_Float_10 { get; set; }
		public float Type5_Float_11 { get; set; }
		public uint Type5_UInt_12 { get; set; }
		public float Type5_Float_13 { get; set; }
		public float Type5_Float_14 { get; set; }

		public uint Type6_UInt { get; set; }

		public uint Type7_UInt { get; set; }
		
		public float Type8_Float { get; set; }
		public uint Type8_UInt { get; set; }

		public float Type9_Float { get; set; }
		public uint Type9_UInt_0 { get; set; }
		public uint Type9_UInt_1 { get; set; }

		public float Type10_Float { get; set; }

		public uint Type11_UInt { get; set; }

		public uint Type12_UInt_0 { get; set; }
		public uint Type12_UInt_1 { get; set; }

		public float Type13_Float_0 { get; set; }
		public float Type13_Float_1 { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Type = s.Serialize<uint>(Type, name: nameof(Type));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
			Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
			Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
			UInt_14 = s.Serialize<uint>(UInt_14, name: nameof(UInt_14));
			Float_18 = s.Serialize<float>(Float_18, name: nameof(Float_18));

			if (Type >= 1) {
				Type1_Float_0 = s.Serialize<float>(Type1_Float_0, name: nameof(Type1_Float_0));
				Type1_Float_1 = s.Serialize<float>(Type1_Float_1, name: nameof(Type1_Float_1));
			}
			if (Type >= 2) {
				Type2_UInt_0 = s.Serialize<uint>(Type2_UInt_0, name: nameof(Type2_UInt_0));
				Type2_UInt_1 = s.Serialize<uint>(Type2_UInt_1, name: nameof(Type2_UInt_1));
			}
			if (Type >= 3) {
				Type3_Float = s.Serialize<float>(Type3_Float, name: nameof(Type3_Float));
				Type3_UInt = s.Serialize<uint>(Type3_UInt, name: nameof(Type3_UInt));
			}
			if (Type >= 4) {
				Type4_Float = s.Serialize<float>(Type4_Float, name: nameof(Type4_Float));
			}
			if (Type >= 5) {
				Type5_Float_00 = s.Serialize<float>(Type5_Float_00, name: nameof(Type5_Float_00));
				Type5_UInt_01 = s.Serialize<uint>(Type5_UInt_01, name: nameof(Type5_UInt_01));
				Type5_UInt_02 = s.Serialize<uint>(Type5_UInt_02, name: nameof(Type5_UInt_02));
				Type5_UInt_03 = s.Serialize<uint>(Type5_UInt_03, name: nameof(Type5_UInt_03));
				Type5_Float_04 = s.Serialize<float>(Type5_Float_04, name: nameof(Type5_Float_04));
				Type5_Float_05 = s.Serialize<float>(Type5_Float_05, name: nameof(Type5_Float_05));
				Type5_Float_06 = s.Serialize<float>(Type5_Float_06, name: nameof(Type5_Float_06));
				Type5_Float_07 = s.Serialize<float>(Type5_Float_07, name: nameof(Type5_Float_07));
				Type5_Float_08 = s.Serialize<float>(Type5_Float_08, name: nameof(Type5_Float_08));
				Type5_UInt_09 = s.Serialize<uint>(Type5_UInt_09, name: nameof(Type5_UInt_09));
				Type5_Float_10 = s.Serialize<float>(Type5_Float_10, name: nameof(Type5_Float_10));
				Type5_Float_11 = s.Serialize<float>(Type5_Float_11, name: nameof(Type5_Float_11));
				Type5_UInt_12 = s.Serialize<uint>(Type5_UInt_12, name: nameof(Type5_UInt_12));
				Type5_Float_13 = s.Serialize<float>(Type5_Float_13, name: nameof(Type5_Float_13));
				Type5_Float_14 = s.Serialize<float>(Type5_Float_14, name: nameof(Type5_Float_14));
			}
			if (Type >= 6) Type6_UInt = s.Serialize<uint>(Type6_UInt, name: nameof(Type6_UInt));
			if (Type >= 7) Type7_UInt = s.Serialize<uint>(Type7_UInt, name: nameof(Type7_UInt));
			if (Type >= 8) {
				Type8_Float = s.Serialize<float>(Type8_Float, name: nameof(Type8_Float));
				Type8_UInt = s.Serialize<uint>(Type8_UInt, name: nameof(Type8_UInt));
			}
			if (Type >= 9) {
				Type9_Float = s.Serialize<float>(Type9_Float, name: nameof(Type9_Float));
				Type9_UInt_0 = s.Serialize<uint>(Type9_UInt_0, name: nameof(Type9_UInt_0));
				Type9_UInt_1 = s.Serialize<uint>(Type9_UInt_1, name: nameof(Type9_UInt_1));
			}
			if (Type >= 10) Type10_Float = s.Serialize<float>(Type10_Float, name: nameof(Type10_Float));
			if (Type >= 11) Type11_UInt = s.Serialize<uint>(Type11_UInt, name: nameof(Type11_UInt));
			if (Type >= 12) {
				Type12_UInt_0 = s.Serialize<uint>(Type12_UInt_0, name: nameof(Type12_UInt_0));
				Type12_UInt_1 = s.Serialize<uint>(Type12_UInt_1, name: nameof(Type12_UInt_1));
			}
			if (Type >= 13) {
				Type13_Float_0 = s.Serialize<float>(Type13_Float_0, name: nameof(Type13_Float_0));
				Type13_Float_1 = s.Serialize<float>(Type13_Float_1, name: nameof(Type13_Float_1));
			}
		}
	}
}
