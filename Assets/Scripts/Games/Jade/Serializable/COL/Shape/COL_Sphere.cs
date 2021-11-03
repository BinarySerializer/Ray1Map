using System;
using BinarySerializer;

namespace Ray1Map.Jade {
    public class COL_Sphere : BinarySerializable {
        public Jade_Vector Center { get; set; }
        public float Radius { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Center = s.SerializeObject<Jade_Vector>(Center, name: nameof(Center));
            Radius = s.Serialize<float>(Radius, name: nameof(Radius));
        }
    }
}