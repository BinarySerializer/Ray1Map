using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in LIGHT_p_CreateFromBuffer
	public class LIGHT_XenonData2 : BinarySerializable {
		public uint Version { get; set; }
		public float V1_Float_0 { get; set; }
		public float V1_Float_1 { get; set; }
		public float V1_Float_2 { get; set; }
		public float V1_Float_3 { get; set; }

		public Jade_TextureReference V2_Texture_0 { get; set; }
		public Jade_TextureReference V2_Texture_1 { get; set; }

		public uint V3_UInt { get; set; }

		public float V4_Float_0 { get; set; }
		public float V4_Float_1 { get; set; }
		public float V4_Float_2 { get; set; }
		public float V4_Float_3 { get; set; }
		public float V4_Float_4 { get; set; }

		public float V5_Float_0 { get; set; }
		public float V5_Float_1 { get; set; }

		public float V6_Float_0 { get; set; }
		public float V6_Float_1 { get; set; }

		public uint V7_UInt { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version >= 1) {
				V1_Float_0 = s.Serialize<float>(V1_Float_0, name: nameof(V1_Float_0));
				V1_Float_1 = s.Serialize<float>(V1_Float_1, name: nameof(V1_Float_1));
				V1_Float_2 = s.Serialize<float>(V1_Float_2, name: nameof(V1_Float_2));
				V1_Float_3 = s.Serialize<float>(V1_Float_3, name: nameof(V1_Float_3));
			}
			if (Version >= 2) {
				V2_Texture_0 = s.SerializeObject<Jade_TextureReference>(V2_Texture_0, name: nameof(V2_Texture_0));
				V2_Texture_1 = s.SerializeObject<Jade_TextureReference>(V2_Texture_1, name: nameof(V2_Texture_1));
			}
			if (Version >= 3) V3_UInt = s.Serialize<uint>(V3_UInt, name: nameof(V3_UInt));
			if (Version >= 4) {
				V4_Float_0 = s.Serialize<float>(V4_Float_0, name: nameof(V4_Float_0));
				V4_Float_1 = s.Serialize<float>(V4_Float_1, name: nameof(V4_Float_1));
				V4_Float_2 = s.Serialize<float>(V4_Float_2, name: nameof(V4_Float_2));
				V4_Float_3 = s.Serialize<float>(V4_Float_3, name: nameof(V4_Float_3));
				V4_Float_4 = s.Serialize<float>(V4_Float_4, name: nameof(V4_Float_4));
			}
			if (Version >= 5) {
				V5_Float_0 = s.Serialize<float>(V5_Float_0, name: nameof(V5_Float_0));
				V5_Float_1 = s.Serialize<float>(V5_Float_1, name: nameof(V5_Float_1));
			}
			if (Version >= 6) {
				V6_Float_0 = s.Serialize<float>(V6_Float_0, name: nameof(V6_Float_0));
				V6_Float_1 = s.Serialize<float>(V6_Float_1, name: nameof(V6_Float_1));
			}
			if (Version >= 7) V7_UInt = s.Serialize<uint>(V7_UInt, name: nameof(V7_UInt));

			V2_Texture_0?.Resolve();
			V2_Texture_1?.Resolve();
		}
	}
}
