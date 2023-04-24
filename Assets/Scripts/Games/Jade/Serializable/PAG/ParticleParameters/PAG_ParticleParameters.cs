using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in PAG_p_CreateFromBuffer
	public abstract class PAG_ParticleParameters : BinarySerializable {
		public PAG_ParticleGenerator Pre_Generator { get; set; }
	}
}
