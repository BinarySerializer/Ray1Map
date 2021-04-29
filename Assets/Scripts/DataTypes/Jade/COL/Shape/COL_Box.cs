using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class COL_Box : BinarySerializable {
        public Jade_Vector Max { get; set; }
        public Jade_Vector Min { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Max = s.SerializeObject<Jade_Vector>(Max, name: nameof(Max));
            Min = s.SerializeObject<Jade_Vector>(Min, name: nameof(Min));
        }
    }
}