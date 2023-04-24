using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in PAG_p_CreateFromBuffer
	public class PAG_3DParticleParameters : PAG_ParticleParameters {
		public Jade_Reference<OBJ_GameObject> Object { get; set; }
		public byte Flags { get; set; }
		public byte UVAnimationLayer { get; set; }
		public float SpeedU { get; set; }
		public float SpeedV { get; set; }
		public float SizeZMin { get; set; }
		public float SizeZMax { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Object = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Object, name: nameof(Object))?.Resolve();
			Flags = s.Serialize<byte>(Flags, name: nameof(Flags));
			if (Pre_Generator.Version >= 38) {
				UVAnimationLayer = s.Serialize<byte>(UVAnimationLayer, name: nameof(UVAnimationLayer));
				SpeedU = s.Serialize<float>(SpeedU, name: nameof(SpeedU));
				SpeedV = s.Serialize<float>(SpeedV, name: nameof(SpeedV));
				if (Pre_Generator.Version >= 40) {
					SizeZMin = s.Serialize<float>(SizeZMin, name: nameof(SizeZMin));
					SizeZMax = s.Serialize<float>(SizeZMax, name: nameof(SizeZMax));
				}
			}
		}
	}
}
