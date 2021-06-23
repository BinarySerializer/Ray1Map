using System;
using BinarySerializer;
using UnityEngine;

namespace R1Engine.Jade
{
    public class TEX_Content_JTX_PS2 : BinarySerializable {
        public TEX_Content_JTX JTX { get; set; }

        public byte[] Bytes_00 { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public byte[] Bytes_01 { get; set; }
        public byte[] Content { get; set; }
        public byte[] Bytes_02 { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            uint FileSize = JTX.PS2_Size;
            if (FileSize == 0) return;
			Bytes_00 = s.SerializeArray<byte>(Bytes_00, 0x20, name: nameof(Bytes_00));
			Width = s.Serialize<uint>(Width, name: nameof(Width));
			Height = s.Serialize<uint>(Height, name: nameof(Height));
			Bytes_01 = s.SerializeArray<byte>(Bytes_01, 0x38, name: nameof(Bytes_01));
			Content = s.SerializeArray<byte>(Content, FileSize - 0x60 - 0x20, name: nameof(Content));
			Bytes_02 = s.SerializeArray<byte>(Bytes_02, 0x20, name: nameof(Bytes_02));
		}
	}
}