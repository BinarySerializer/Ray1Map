using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierODE : MDF_Modifier {
		public byte Version { get; set; }
		public byte Byte_01 { get; set; }
		public short Short_02 { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject0 { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject1 { get; set; }

		public float V1_Float_0 { get; set; }
		public float V1_Float_1 { get; set; }
		public float V1_Float_2 { get; set; }
		public float V1_Float_3 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<byte>(Version, name: nameof(Version));
			Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
			Short_02 = s.Serialize<short>(Short_02, name: nameof(Short_02));
			GameObject0 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject0, name: nameof(GameObject0))?.Resolve();
			GameObject1 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject1, name: nameof(GameObject1))?.Resolve();
			if (Version > 1) {
				V1_Float_0 = s.Serialize<float>(V1_Float_0, name: nameof(V1_Float_0));
				V1_Float_1 = s.Serialize<float>(V1_Float_1, name: nameof(V1_Float_1));
				V1_Float_2 = s.Serialize<float>(V1_Float_2, name: nameof(V1_Float_2));
				V1_Float_3 = s.Serialize<float>(V1_Float_3, name: nameof(V1_Float_3));
			}
		}
	}
}
