using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierODE : MDF_Modifier {
		public byte Version { get; set; }
		public byte Type { get; set; }
		public short Dummy { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject0 { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject1 { get; set; }

		public float LoLimit { get; set; }
		public float HiLimit { get; set; }
		public float BounceStop { get; set; }
		public float Friction { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<byte>(Version, name: nameof(Version));
			Type = s.Serialize<byte>(Type, name: nameof(Type));
			Dummy = s.Serialize<short>(Dummy, name: nameof(Dummy));
			GameObject0 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject0, name: nameof(GameObject0))?.Resolve();
			GameObject1 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject1, name: nameof(GameObject1))?.Resolve();
			if (Version > 1) {
				LoLimit = s.Serialize<float>(LoLimit, name: nameof(LoLimit));
				HiLimit = s.Serialize<float>(HiLimit, name: nameof(HiLimit));
				BounceStop = s.Serialize<float>(BounceStop, name: nameof(BounceStop));
				Friction = s.Serialize<float>(Friction, name: nameof(Friction));
			}
		}
	}
}
