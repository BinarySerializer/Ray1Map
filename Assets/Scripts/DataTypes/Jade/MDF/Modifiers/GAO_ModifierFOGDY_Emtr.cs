using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierFOGDY_Emtr : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint Version { get; set; }
		public uint UInt_08 { get; set; }
		public Entry[] Entries { get; set; }
		public uint V1_UInt { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
			Entries = s.SerializeObjectArray<Entry>(Entries, 8, onPreSerialize: e => e.Modifier = this, name: nameof(Entries));
			if(Version >= 1) V1_UInt = s.Serialize<uint>(V1_UInt, name: nameof(V1_UInt));
		}

		public class Entry : BinarySerializable {
			public GAO_ModifierFOGDY_Emtr Modifier { get; set; }

			public uint UInt_00 { get; set; }
			public float Float_04 { get; set; }
			public float V2_Float_0 { get; set; }
			public float V2_Float_1 { get; set; }
			public float V2_Float_2 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
				Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
				if (Modifier.Version >= 2) {
					V2_Float_0 = s.Serialize<float>(V2_Float_0, name: nameof(V2_Float_0));
					V2_Float_1 = s.Serialize<float>(V2_Float_1, name: nameof(V2_Float_1));
					V2_Float_2 = s.Serialize<float>(V2_Float_2, name: nameof(V2_Float_2));
				}
			}
		}
	}
}
