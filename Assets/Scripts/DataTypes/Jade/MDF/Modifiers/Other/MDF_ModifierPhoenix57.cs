using BinarySerializer;

namespace R1Engine.Jade {
	public class MDF_ModifierPhoenix57 : MDF_Modifier {
		public uint Version { get; set; }
		public uint Type { get; set; }
		public uint UInt0 { get; set; }
		public float Float1 { get; set; }
		public uint UInt2 { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
		public uint UInt4 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) Version = s.Serialize<uint>(Version, name: nameof(Version));

			Type = s.Serialize<uint>(Type, name: nameof(Type));
			if (Type >= 1) {
				UInt0 = s.Serialize<uint>(UInt0, name: nameof(UInt0));
				Float1 = s.Serialize<float>(Float1, name: nameof(Float1));
				if (Type < 2)
					UInt2 = s.Serialize<uint>(UInt2, name: nameof(UInt2));
				else
					GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();

				if (Type == 4) UInt4 = s.Serialize<uint>(UInt4, name: nameof(UInt4));
			}
		}
	}
}
