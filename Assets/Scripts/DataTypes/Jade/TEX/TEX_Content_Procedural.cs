using System;
using BinarySerializer;
using UnityEngine;

namespace R1Engine.Jade
{
    public class TEX_Content_Procedural : BinarySerializable {
        public uint FileSize { get; set; } // Set in onPreSerialize

        public uint UInt_00 { get; set; }
        public ushort Flags { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public ushort Type { get; set; }

        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
            Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Type = s.Serialize<ushort>(Type, name: nameof(Type));

            Data = s.SerializeArray<byte>(Data, FileSize - 12, name: nameof(Data));
        }
	}
}