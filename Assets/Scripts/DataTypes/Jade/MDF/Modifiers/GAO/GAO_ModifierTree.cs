using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierTree : MDF_Modifier {
		public uint Version { get; set; }
		public uint V1_ElementsCount { get; set; }
		public uint V1_UInt1 { get; set; }
		public V1_Element[] V1_Elements { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version == 1) {
				V1_ElementsCount = s.Serialize<uint>(V1_ElementsCount, name: nameof(V1_ElementsCount));
				V1_UInt1 = s.Serialize<uint>(V1_UInt1, name: nameof(V1_UInt1));
				V1_Elements = s.SerializeObjectArray<V1_Element>(V1_Elements, V1_ElementsCount, name: nameof(V1_Elements));
			}
		}

		public class V1_Element : BinarySerializable {
			public uint UInt0 { get; set; }
			public Jade_Matrix Matrix1 { get; set; }
			public GAO_ModifierRotC RotC { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				UInt0 = s.Serialize<uint>(UInt0, name: nameof(UInt0));
				Matrix1 = s.SerializeObject<Jade_Matrix>(Matrix1, name: nameof(Matrix1));
				RotC = s.SerializeObject<GAO_ModifierRotC>(RotC, name: nameof(RotC));
			}
		}
	}
}
