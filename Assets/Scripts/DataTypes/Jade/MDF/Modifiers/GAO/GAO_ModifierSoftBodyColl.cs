using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierSoftBodyColl : MDF_Modifier {
		public uint Version { get; set; }
		public Jade_Vector Vector_0 { get; set; }
		public Jade_Vector Vector_1 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Vector_0 = s.SerializeObject<Jade_Vector>(Vector_0, name: nameof(Vector_0));
			Vector_1 = s.SerializeObject<Jade_Vector>(Vector_1, name: nameof(Vector_1));
		}
	}
}
