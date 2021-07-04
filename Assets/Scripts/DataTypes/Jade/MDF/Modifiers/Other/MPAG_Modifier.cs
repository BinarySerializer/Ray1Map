using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class MPAG_Modifier : MDF_Modifier {
        public PAG_ParticleGenerator Data { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Data = s.SerializeObject<PAG_ParticleGenerator>(Data, name: nameof(Data));
        }
    }
}