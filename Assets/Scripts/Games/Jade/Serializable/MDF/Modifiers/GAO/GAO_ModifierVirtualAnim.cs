using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierVirtualAnim : MDF_Modifier {
		public uint Version { get; set; }
		public Jade_Reference<OBJ_GameObject>[] GameObjects { get; set; }
		public uint ObjectsCount { get; set; }
		public Object[] Objects { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			GameObjects = s.SerializeObjectArray<Jade_Reference<OBJ_GameObject>>(GameObjects, 5, name: nameof(GameObjects))?.Resolve();
			ObjectsCount = s.Serialize<uint>(ObjectsCount, name: nameof(ObjectsCount));
			Objects = s.SerializeObjectArray<Object>(Objects, ObjectsCount, name: nameof(Objects));
		}

		public class Object : BinarySerializable {
			public Jade_Matrix Matrix { get; set; }
			public uint Animation { get; set; }
			public uint Activation { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Matrix = s.SerializeObject<Jade_Matrix>(Matrix, name: nameof(Matrix));
				Animation = s.Serialize<uint>(Animation, name: nameof(Animation));
				Activation = s.Serialize<uint>(Activation, name: nameof(Activation));
			}
		}
	}
}
