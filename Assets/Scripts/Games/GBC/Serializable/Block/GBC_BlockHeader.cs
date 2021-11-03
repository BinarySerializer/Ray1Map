using System;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBC
{
    public class GBC_BlockHeader : BinarySerializable
    {
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public GBC_BlockType Type { get; set; }
        public byte Byte_04 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            Type = s.Serialize<GBC_BlockType>(Type, name: nameof(Type));
            Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));

            if (!Enum.IsDefined(typeof(GBC_BlockType), Type))
                Debug.LogWarning($"Block type {Type} is not defined");
        }
    }
}