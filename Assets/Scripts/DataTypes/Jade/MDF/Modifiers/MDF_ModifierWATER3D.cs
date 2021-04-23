using BinarySerializer;

namespace R1Engine.Jade {
	public class MDF_ModifierWATER3D : MDF_Modifier {
		public uint Version { get; set; }
		public Jade_Matrix Matrix { get; set; }
		public float Float_0 { get; set; }
		public float Float_1 { get; set; }
		public float Float_2 { get; set; }
		public float Float_3 { get; set; }
		public float Float_4 { get; set; }
		public uint UInt_5 { get; set; }
		public uint UInt_6 { get; set; }
		public float Float_7 { get; set; }
		public float Float_8 { get; set; }
		public uint UInt_9 { get; set; }
		public float Float_10 { get; set; }

		public uint V4_UInt { get; set; }

		public float V5_Float_0 { get; set; }
		public float V5_Float_1 { get; set; }

		public float V6_Float { get; set; }

		public float V7_Float_0 { get; set; }
		public float V7_Float_1 { get; set; }

		public float V8_Float { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Matrix = s.SerializeObject<Jade_Matrix>(Matrix, name: nameof(Matrix));
			Float_0 = s.Serialize<float>(Float_0, name: nameof(Float_0));
			Float_1 = s.Serialize<float>(Float_1, name: nameof(Float_1));
			Float_2 = s.Serialize<float>(Float_2, name: nameof(Float_2));
			Float_3 = s.Serialize<float>(Float_3, name: nameof(Float_3));
			Float_4 = s.Serialize<float>(Float_4, name: nameof(Float_4));
			UInt_5 = s.Serialize<uint>(UInt_5, name: nameof(UInt_5));
			UInt_6 = s.Serialize<uint>(UInt_6, name: nameof(UInt_6));
			Float_7 = s.Serialize<float>(Float_7, name: nameof(Float_7));
			Float_8 = s.Serialize<float>(Float_8, name: nameof(Float_8));
			UInt_9 = s.Serialize<uint>(UInt_9, name: nameof(UInt_9));
			Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));

			if (Version >= 4) V4_UInt = s.Serialize<uint>(V4_UInt, name: nameof(V4_UInt));

			if (Version >= 5) {
				V5_Float_0 = s.Serialize<float>(V5_Float_0, name: nameof(V5_Float_0));
				V5_Float_1 = s.Serialize<float>(V5_Float_1, name: nameof(V5_Float_1));
			}

			if (Version >= 6) V6_Float = s.Serialize<float>(V6_Float, name: nameof(V6_Float));

			if (Version >= 7) {
				V7_Float_0 = s.Serialize<float>(V7_Float_0, name: nameof(V7_Float_0));
				V7_Float_1 = s.Serialize<float>(V7_Float_1, name: nameof(V7_Float_1));
			}

			if (Version >= 8) V8_Float = s.Serialize<float>(V8_Float, name: nameof(V8_Float));
		}
	}
}
