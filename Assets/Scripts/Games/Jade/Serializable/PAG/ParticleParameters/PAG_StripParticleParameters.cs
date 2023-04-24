using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in PAG_p_CreateFromBuffer
	public class PAG_StripParticleParameters : PAG_ParticleParameters {
		public Jade_Reference<OBJ_GameObject> Object { get; set; }
		public SD StripDirection { get; set; }
		public uint UseEmbryonQuad { get; set; }
		public uint OrientParticle { get; set; }
		public float PivotOffset { get; set; }
		public byte TVP_Byte { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (Pre_Generator.Version >= 39) {
				StripDirection = s.Serialize<SD>(StripDirection, name: nameof(StripDirection));

				bool isTVP = s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRRTVParty);

				if (Pre_Generator.Version >= (isTVP ? 45 : 41)) {
					if (Pre_Generator.Version >= (isTVP ? 46 : 42)) {
						UseEmbryonQuad = s.Serialize<uint>(UseEmbryonQuad, name: nameof(UseEmbryonQuad));
						OrientParticle = s.Serialize<uint>(OrientParticle, name: nameof(OrientParticle));
						if(!isTVP && Pre_Generator.Version >= 46)
							PivotOffset = s.Serialize<float>(PivotOffset, name: nameof(PivotOffset));
						else if(isTVP && Pre_Generator.Version >= 48)
							TVP_Byte = s.Serialize<byte>(TVP_Byte, name: nameof(TVP_Byte));
					} else {
						UseEmbryonQuad = s.Serialize<byte>((byte)UseEmbryonQuad, name: nameof(UseEmbryonQuad));
					}
				}
			}
		}

		public enum SD : byte {
			WorldX = 0,
			WorldY = 1,
			WorldZ = 2,
			StripOrientation = 3
		}
	}
}
