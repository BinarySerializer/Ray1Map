using BinarySerializer;

namespace R1Engine.Jade {
	public class MDF_ModifierDisturber : MDF_Modifier {
		public uint Version { get; set; }
		public float Float_04 { get; set; }

		public uint V2_UInt_0 { get; set; }
		public float V2_Float_1 { get; set; }
		public float V2_Float_2 { get; set; }
		public uint V2_UInt_3 { get; set; }
		public uint V2_UInt_4 { get; set; }
		public uint V2_UInt_5 { get; set; }
		public uint V2_UInt_6 { get; set; }
		public uint V2_UInt_7 { get; set; }
		public uint V2_UInt_8 { get; set; }
		public uint V2_UInt_9 { get; set; }
		public uint V2_UInt_10 { get; set; }
		public uint V2_UInt_11 { get; set; }
		public uint V2_UInt_12 { get; set; }

		public uint V10_UInt_0 { get; set; }

		public uint V3_UInt_0 { get; set; }
		public Jade_Vector V3_Vector_1 { get; set; }
		public float V3_Float_2 { get; set; }
		public float V3_Float_3 { get; set; }
		public float V3_Float_4 { get; set; }
		public float V3_Float_5 { get; set; }
		public float V3_Float_6 { get; set; }
		public float V3_Float_7 { get; set; }
		public float V3_Float_8 { get; set; }
		public float V3_Float_9 { get; set; }
		public float V3_Float_10 { get; set; }

		public uint V8_UInt_0 { get; set; }
		public float V8_Float_1 { get; set; }
		public float V8_Float_2 { get; set; }
		public float V8_Float_3 { get; set; }
		public float V8_Float_4 { get; set; }
		public float V8_Float_5 { get; set; }
		public float V8_Float_6 { get; set; }
		public float V8_Float_7 { get; set; }

		public uint V9_UInt_0 { get; set; }

		public float V11_Float_0 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));

			if (Version >= 2) {
				V2_UInt_0 = s.Serialize<uint>(V2_UInt_0, name: nameof(V2_UInt_0));
				V2_Float_1 = s.Serialize<float>(V2_Float_1, name: nameof(V2_Float_1));
				V2_Float_2 = s.Serialize<float>(V2_Float_2, name: nameof(V2_Float_2));
			}
			if (Version == 2) {
				V2_UInt_3  = s.Serialize<uint>(V2_UInt_3 , name: nameof(V2_UInt_3 ));
				V2_UInt_4  = s.Serialize<uint>(V2_UInt_4 , name: nameof(V2_UInt_4 ));
				V2_UInt_5  = s.Serialize<uint>(V2_UInt_5 , name: nameof(V2_UInt_5 ));
				V2_UInt_6  = s.Serialize<uint>(V2_UInt_6 , name: nameof(V2_UInt_6 ));
				V2_UInt_7  = s.Serialize<uint>(V2_UInt_7 , name: nameof(V2_UInt_7 ));
				V2_UInt_8  = s.Serialize<uint>(V2_UInt_8 , name: nameof(V2_UInt_8 ));
				V2_UInt_9  = s.Serialize<uint>(V2_UInt_9 , name: nameof(V2_UInt_9 ));
				V2_UInt_10 = s.Serialize<uint>(V2_UInt_10, name: nameof(V2_UInt_10));
				V2_UInt_11 = s.Serialize<uint>(V2_UInt_11, name: nameof(V2_UInt_11));
				V2_UInt_12 = s.Serialize<uint>(V2_UInt_12, name: nameof(V2_UInt_12));
			}
			if (Version >= 10) V10_UInt_0 = s.Serialize<uint>(V10_UInt_0, name: nameof(V10_UInt_0));
			if (Version >= 3) {
				V3_UInt_0 = s.Serialize<uint>(V3_UInt_0, name: nameof(V3_UInt_0));
				V3_Vector_1 = s.SerializeObject<Jade_Vector>(V3_Vector_1, name: nameof(V3_Vector_1));
				V3_Float_2 = s.Serialize<float>(V3_Float_2, name: nameof(V3_Float_2));
				V3_Float_3 = s.Serialize<float>(V3_Float_3, name: nameof(V3_Float_3));
				V3_Float_4 = s.Serialize<float>(V3_Float_4, name: nameof(V3_Float_4));
				V3_Float_5 = s.Serialize<float>(V3_Float_5, name: nameof(V3_Float_5));
				V3_Float_6 = s.Serialize<float>(V3_Float_6, name: nameof(V3_Float_6));
				V3_Float_7 = s.Serialize<float>(V3_Float_7, name: nameof(V3_Float_7));
				V3_Float_8 = s.Serialize<float>(V3_Float_8, name: nameof(V3_Float_8));
				V3_Float_9 = s.Serialize<float>(V3_Float_9, name: nameof(V3_Float_9));
				V3_Float_10 = s.Serialize<float>(V3_Float_10, name: nameof(V3_Float_10));
			}
			if (Version >= 8) {
				V8_UInt_0 = s.Serialize<uint>(V8_UInt_0, name: nameof(V8_UInt_0));
				if (V8_UInt_0 != 0) {
					V8_Float_1 = s.Serialize<float>(V8_Float_1, name: nameof(V8_Float_1));
					V8_Float_2 = s.Serialize<float>(V8_Float_2, name: nameof(V8_Float_2));
					V8_Float_3 = s.Serialize<float>(V8_Float_3, name: nameof(V8_Float_3));
					V8_Float_4 = s.Serialize<float>(V8_Float_4, name: nameof(V8_Float_4));
					V8_Float_5 = s.Serialize<float>(V8_Float_5, name: nameof(V8_Float_5));
					V8_Float_6 = s.Serialize<float>(V8_Float_6, name: nameof(V8_Float_6));
					V8_Float_7 = s.Serialize<float>(V8_Float_7, name: nameof(V8_Float_7));
				}
			}
			if (Version >= 9) V9_UInt_0 = s.Serialize<uint>(V9_UInt_0, name: nameof(V9_UInt_0));
			if (Version >= 11) V11_Float_0 = s.Serialize<float>(V11_Float_0, name: nameof(V11_Float_0));
		}
	}
}
