using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class EVE_Event_MagicKey : BinarySerializable
    {
        public uint UInt_00 { get; set; }
        public uint UInt_04 { get; set; }
        public uint UInt_08 { get; set; }
        public uint UInt_0C { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
            UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
            UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
            UInt_0C = s.Serialize<uint>(UInt_0C, name: nameof(UInt_0C));
        }
    }
}