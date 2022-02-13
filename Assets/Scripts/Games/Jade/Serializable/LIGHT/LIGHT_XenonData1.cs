using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in LIGHT_p_CreateFromBuffer
	public class LIGHT_XenonData1 : BinarySerializable {
		public uint Type { get; set; }
		public uint LightFlags { get; set; }
		public float Type1_Float_00 { get; set; }
		public float Type1_Float_01 { get; set; }
		public float Type2_Float_00 { get; set; }
		public float Type2_Float_01 { get; set; }
		public uint Type3_UInt { get; set; }
		public float Type3_Float { get; set; }
		public float Type4_Float { get; set; }
		public Jade_TextureReference Type4_Texture { get; set; }
		public uint Type5_UInt_00 { get; set; }
		public uint Type5_UInt_01 { get; set; }
		public float Type5_Float_02 { get; set; }
		public float Type5_Float_03 { get; set; }
		public float Type6_Float { get; set; }
		public uint Type7_UInt_00 { get; set; }
		public uint Type7_UInt_01 { get; set; }
		public uint Type8_UInt { get; set; }
		public float Type9_Float_00 { get; set; }
		public float Type9_Float_01 { get; set; }
		public float Type10_Float_00 { get; set; }
		public float Type10_Float_01 { get; set; }
		public byte Type11_Byte_00 { get; set; }
		public byte Type11_Byte_01 { get; set; }
		public uint Type12_UInt { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Type = s.Serialize<uint>(Type, name: nameof(Type));
			LightFlags = s.Serialize<uint>(LightFlags, name: nameof(LightFlags));
			if (Type >= 1) {
				Type1_Float_00 = s.Serialize<float>(Type1_Float_00, name: nameof(Type1_Float_00));
				Type1_Float_01 = s.Serialize<float>(Type1_Float_01, name: nameof(Type1_Float_01));
			}
			if (Type >= 2) {
				Type2_Float_00 = s.Serialize<float>(Type2_Float_00, name: nameof(Type2_Float_00));
				Type2_Float_01 = s.Serialize<float>(Type2_Float_01, name: nameof(Type2_Float_01));
			}
			if (Type >= 3) {
				Type3_UInt = s.Serialize<uint>(Type3_UInt, name: nameof(Type3_UInt));
				Type3_Float = s.Serialize<float>(Type3_Float, name: nameof(Type3_Float));
			}
			if (Type >= 4) {
				Type4_Float = s.Serialize<float>(Type4_Float, name: nameof(Type4_Float));
				Type4_Texture = s.SerializeObject<Jade_TextureReference>(Type4_Texture, name: nameof(Type4_Texture))?.Resolve();
			}
			if (Type >= 5) {
				Type5_UInt_00 = s.Serialize<uint>(Type5_UInt_00, name: nameof(Type5_UInt_00));
				Type5_UInt_01 = s.Serialize<uint>(Type5_UInt_01, name: nameof(Type5_UInt_01));
				Type5_Float_02 = s.Serialize<float>(Type5_Float_02, name: nameof(Type5_Float_02));
				Type5_Float_03 = s.Serialize<float>(Type5_Float_03, name: nameof(Type5_Float_03));
			}
			if (Type >= 6) Type6_Float = s.Serialize<float>(Type6_Float, name: nameof(Type6_Float));
			if (Type >= 7) {
				Type7_UInt_00 = s.Serialize<uint>(Type7_UInt_00, name: nameof(Type7_UInt_00));
				Type7_UInt_01 = s.Serialize<uint>(Type7_UInt_01, name: nameof(Type7_UInt_01));
			}
			if (Type >= 8) Type8_UInt = s.Serialize<uint>(Type8_UInt, name: nameof(Type8_UInt));
			if (Type >= 9) {
				Type9_Float_00 = s.Serialize<float>(Type9_Float_00, name: nameof(Type9_Float_00));
				Type9_Float_01 = s.Serialize<float>(Type9_Float_01, name: nameof(Type9_Float_01));
			}
			if (Type >= 10) {
				Type10_Float_00 = s.Serialize<float>(Type10_Float_00, name: nameof(Type10_Float_00));
				Type10_Float_01 = s.Serialize<float>(Type10_Float_01, name: nameof(Type10_Float_01));
			}
			if (Type >= 11) {
				Type11_Byte_00 = s.Serialize<byte>(Type11_Byte_00, name: nameof(Type11_Byte_00));
				Type11_Byte_01 = s.Serialize<byte>(Type11_Byte_01, name: nameof(Type11_Byte_01));
			}
			if (Type >= 12) Type12_UInt = s.Serialize<uint>(Type12_UInt, name: nameof(Type12_UInt));
		}
	}
}
