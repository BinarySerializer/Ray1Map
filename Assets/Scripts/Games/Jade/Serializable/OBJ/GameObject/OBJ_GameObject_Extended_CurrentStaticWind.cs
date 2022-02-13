using BinarySerializer;

namespace Ray1Map.Jade {
	public class OBJ_GameObject_Extended_CurrentStaticWind : BinarySerializable {
		public uint UInt_00 { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
		}
	}
}
