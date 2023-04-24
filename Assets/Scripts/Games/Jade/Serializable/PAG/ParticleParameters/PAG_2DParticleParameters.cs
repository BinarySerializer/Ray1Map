using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in PAG_p_CreateFromBuffer
	public class PAG_2DParticleParameters : PAG_ParticleParameters {
		public float PivotX { get; set; }
		public float PivotY { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS)) {
				PivotX = s.Serialize<float>(PivotX, name: nameof(PivotX));
				PivotY = s.Serialize<float>(PivotY, name: nameof(PivotY));
			}
		}
	}
}
