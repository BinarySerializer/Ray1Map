using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierPlant : MDF_Modifier {
		public uint Version { get; set; }
		public uint V2_UInt_0 { get; set; }
		public uint V2_UInt_1 { get; set; }
		public uint V0_UInt_0 { get; set; }
		public uint V0_UInt_1 { get; set; }
		public uint V0_UInt_2 { get; set; }
		public uint V0_ElementsCount { get; set; }
		public V0_Element[] V0_Elements { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version == 2) {
				V2_UInt_0 = s.Serialize<uint>(V2_UInt_0, name: nameof(V2_UInt_0));
				V2_UInt_1 = s.Serialize<uint>(V2_UInt_1, name: nameof(V2_UInt_1));
			}
			if (Version < 3) {
				V0_UInt_0 = s.Serialize<uint>(V0_UInt_0, name: nameof(V0_UInt_0));
				V0_UInt_1 = s.Serialize<uint>(V0_UInt_1, name: nameof(V0_UInt_1));
				V0_UInt_2 = s.Serialize<uint>(V0_UInt_2, name: nameof(V0_UInt_2));
				V0_ElementsCount = s.Serialize<uint>(V0_ElementsCount, name: nameof(V0_ElementsCount));
				V0_Elements = s.SerializeObjectArray<V0_Element>(V0_Elements, V0_ElementsCount, name: nameof(V0_Elements));
			}
		}

		public class V0_Element : BinarySerializable {
			public Jade_Matrix Matrix0 { get; set; }
			public Jade_Matrix Matrix1 { get; set; }
			public GAO_ModifierRotC RotC { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Matrix0 = s.SerializeObject<Jade_Matrix>(Matrix0, name: nameof(Matrix0));
				Matrix1 = s.SerializeObject<Jade_Matrix>(Matrix1, name: nameof(Matrix1));
				RotC = s.SerializeObject<GAO_ModifierRotC>(RotC, name: nameof(RotC));
			}
		}
	}
}
