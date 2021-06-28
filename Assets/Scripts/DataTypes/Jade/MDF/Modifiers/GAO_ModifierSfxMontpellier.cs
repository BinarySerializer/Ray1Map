using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierSfxMontpellier : MDF_Modifier {
		public uint Version { get; set; }
		public uint UInt_04 { get; set; }
		public uint Type { get; set; }

		public Jade_Vector Type0_Vector_0 { get; set; }
		public float Type0_Float_1 { get; set; }
		public float Type0_Float_2 { get; set; }
		public float Type0_Float_3 { get; set; }
		public float Type0_Float_4 { get; set; }
		public float Type0_Float_5 { get; set; }

		public float Type1_Float_0 { get; set; }
		public float Type1_Float_1 { get; set; }
		public uint Type1_UInt_2 { get; set; }
		public float Type1_Float_3 { get; set; }

		public float Type1_Float_4 { get; set; }
		public float Type1_Float_5 { get; set; }
		public uint Type1_UInt_6 { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			Type = s.Serialize<uint>(Type, name: nameof(Type));
			if (Type == 0) {
				Type0_Vector_0 = s.SerializeObject<Jade_Vector>(Type0_Vector_0, name: nameof(Type0_Vector_0));
				Type0_Float_1 = s.Serialize<float>(Type0_Float_1, name: nameof(Type0_Float_1));
				Type0_Float_2 = s.Serialize<float>(Type0_Float_2, name: nameof(Type0_Float_2));
				Type0_Float_3 = s.Serialize<float>(Type0_Float_3, name: nameof(Type0_Float_3));
				Type0_Float_4 = s.Serialize<float>(Type0_Float_4, name: nameof(Type0_Float_4));
				Type0_Float_5 = s.Serialize<float>(Type0_Float_5, name: nameof(Type0_Float_5));
			} else if (Type == 1) {
				if (Version < 2) {
					Type1_Float_0 = s.Serialize<float>(Type1_Float_0, name: nameof(Type1_Float_0));
					Type1_Float_1 = s.Serialize<float>(Type1_Float_1, name: nameof(Type1_Float_1));
					Type1_UInt_2 = s.Serialize<uint>(Type1_UInt_2, name: nameof(Type1_UInt_2));
					Type1_Float_3 = s.Serialize<float>(Type1_Float_3, name: nameof(Type1_Float_3));
				}
				Type1_Float_4 = s.Serialize<float>(Type1_Float_4, name: nameof(Type1_Float_4));
				Type1_Float_5 = s.Serialize<float>(Type1_Float_5, name: nameof(Type1_Float_5));
				if (Version < 2) Type1_UInt_6 = s.Serialize<uint>(Type1_UInt_6, name: nameof(Type1_UInt_6));
			}
		}
	}
}
