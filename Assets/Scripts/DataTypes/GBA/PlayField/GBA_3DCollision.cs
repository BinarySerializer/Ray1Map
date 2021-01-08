using System;

namespace R1Engine
{
    public class GBA_3DCollision : R1Serializable
    {
        public byte Height { get; set; }
        public byte Type { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            s.SerializeBitValues<byte>(serializeFunc => {
                Height = (byte)serializeFunc(Height, 5, name: nameof(Height));
                Type = (byte)serializeFunc(Type, 3, name: nameof(Type));
            });
        }
    }
}