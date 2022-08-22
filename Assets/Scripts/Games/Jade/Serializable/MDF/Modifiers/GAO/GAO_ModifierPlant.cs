using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierPlant : MDF_Modifier {
		public uint Version { get; set; }
		public uint V2_UInt_0 { get; set; }
		public float V2_Float_1 { get; set; }
		public float V0_Float_0 { get; set; }
		public float V0_Float_1 { get; set; }
		public float V0_Float_2 { get; set; }
		public uint V0_ElementsCount { get; set; }
		public V0_Element[] V0_Elements { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version >= 2) {
				V2_UInt_0 = s.Serialize<uint>(V2_UInt_0, name: nameof(V2_UInt_0));
				V2_Float_1 = s.Serialize<float>(V2_Float_1, name: nameof(V2_Float_1));
			}
			if (Version <= 3) {
				V0_Float_0 = s.Serialize<float>(V0_Float_0, name: nameof(V0_Float_0));
				V0_Float_1 = s.Serialize<float>(V0_Float_1, name: nameof(V0_Float_1));
				V0_Float_2 = s.Serialize<float>(V0_Float_2, name: nameof(V0_Float_2));
				V0_ElementsCount = s.Serialize<uint>(V0_ElementsCount, name: nameof(V0_ElementsCount));
				V0_Elements = s.SerializeObjectArray<V0_Element>(V0_Elements, V0_ElementsCount, onPreSerialize: el => el.Modifier = this, name: nameof(V0_Elements));
			}
		}

		public class V0_Element : BinarySerializable {
			public GAO_ModifierPlant Modifier { get; set; }

			public Jade_Matrix Matrix0 { get; set; }
			public Jade_Matrix Matrix1 { get; set; }
			public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
			public uint V3_UInt { get; set; }
			public GAO_ModifierRotC RotC { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Matrix0 = s.SerializeObject<Jade_Matrix>(Matrix0, name: nameof(Matrix0));
				Matrix1 = s.SerializeObject<Jade_Matrix>(Matrix1, name: nameof(Matrix1));
				if (Modifier.Version == 3 && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_WW_20040920)) {
					GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
					V3_UInt = s.Serialize<uint>(V3_UInt, name: nameof(V3_UInt));
				}
				RotC = s.SerializeObject<GAO_ModifierRotC>(RotC, name: nameof(RotC));
			}
		}
	}
}
