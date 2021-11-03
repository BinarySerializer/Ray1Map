using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierEyeTrail : MDF_Modifier {
		public uint Version { get; set; }
		public uint UInt_0 { get; set; }
		public Jade_Vector Vector_1 { get; set; }
		public uint UInt_2 { get; set; }
		public float Float_3 { get; set; }
		public float Float_4 { get; set; }
		public float Float_5 { get; set; }
		public float Float_6 { get; set; }
		public uint UInt_7 { get; set; }
		public uint UInt_8 { get; set; }
		public uint V1_UInt_0 { get; set; }
		public uint V2_UInt_0 { get; set; }
		public float V2_Float_1 { get; set; }
		public float V2_Float_2 { get; set; }
		public uint V2_UInt_3 { get; set; }
		public Jade_Reference<GEO_Object> GeometricObject { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version >= 1) {
				UInt_0 = s.Serialize<uint>(UInt_0, name: nameof(UInt_0));
				Vector_1 = s.SerializeObject<Jade_Vector>(Vector_1, name: nameof(Vector_1));
				UInt_2 = s.Serialize<uint>(UInt_2, name: nameof(UInt_2));
				Float_3 = s.Serialize<float>(Float_3, name: nameof(Float_3));
				Float_4 = s.Serialize<float>(Float_4, name: nameof(Float_4));
				Float_5 = s.Serialize<float>(Float_5, name: nameof(Float_5));
				Float_6 = s.Serialize<float>(Float_6, name: nameof(Float_6));
				UInt_7 = s.Serialize<uint>(UInt_7, name: nameof(UInt_7));
				UInt_8 = s.Serialize<uint>(UInt_8, name: nameof(UInt_8));
				if (Version == 1) V1_UInt_0 = s.Serialize<uint>(V1_UInt_0, name: nameof(V1_UInt_0));
				if (Version >= 2) {
					V2_UInt_0 = s.Serialize<uint>(V2_UInt_0, name: nameof(V2_UInt_0));
					V2_Float_1 = s.Serialize<float>(V2_Float_1, name: nameof(V2_Float_1));
					V2_Float_2 = s.Serialize<float>(V2_Float_2, name: nameof(V2_Float_2));
					V2_UInt_3 = s.Serialize<uint>(V2_UInt_3, name: nameof(V2_UInt_3));
				}
				GeometricObject = s.SerializeObject<Jade_Reference<GEO_Object>>(GeometricObject, name: nameof(GeometricObject))?.Resolve();
			}
		}
	}
}
