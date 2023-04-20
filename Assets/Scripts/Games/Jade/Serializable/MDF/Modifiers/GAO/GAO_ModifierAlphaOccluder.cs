using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierAlphaOccluder : MDF_Modifier {
		public uint Version { get; set; }
		public uint Flags { get; set; } = 1;
		public float Float_08 { get; set; }
		public float Float_0C { get; set;}
		public float Float_10 { get; set; }
		public Jade_Vector Vector_14 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if(Version >= 2)
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
			Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
			if (Version >= 3) {
				Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
				Vector_14 = s.SerializeObject<Jade_Vector>(Vector_14, name: nameof(Vector_14));
			}
		}
	}
}
