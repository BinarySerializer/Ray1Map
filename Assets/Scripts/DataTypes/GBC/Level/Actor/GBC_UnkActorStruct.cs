﻿namespace R1Engine
{
    public class GBC_UnkActorStruct : R1Serializable
    {
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; } // Always 0xCD on PalmOS
        public byte Byte_02 { get; set; } // Always 0xCD on PalmOS

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
        }
    }
}