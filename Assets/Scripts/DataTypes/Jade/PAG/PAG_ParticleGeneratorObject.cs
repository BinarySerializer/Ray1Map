using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in PAG_p_CreateFromBuffer
	public class PAG_ParticleGeneratorObject : GRO_GraphicRenderObject {
		public PAG_ParticleGenerator Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Data = s.SerializeObject<PAG_ParticleGenerator>(Data, name: nameof(Data));
		}
	}
}
