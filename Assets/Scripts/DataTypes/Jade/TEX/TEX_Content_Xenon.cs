using System;
using BinarySerializer;
using UnityEngine;

namespace R1Engine.Jade
{
    public class TEX_Content_Xenon : BinarySerializable {
        public uint FileSize { get; set; } // Set in onPreSerialize

        public string Header { get; set; }
        public const string XenonHeader = "D2KK";

        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint Format { get; set; }
        public uint UInt_10 { get; set; }
        public short Short_14 { get; set; }
        public short Short_16 { get; set; }
        public uint DataSize { get; set; }
        public uint UInt_1C { get; set; }

        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            s.DoEndian(Endian.Big, () => {
                Header = s.SerializeString(Header, length: 4, encoding: Jade_BaseManager.Encoding, name: nameof(Header));
                if(Header != XenonHeader) throw new Exception($"File at {Offset} is not a XenonTexture");

                Width = s.Serialize<uint>(Width, name: nameof(Width));
                Height = s.Serialize<uint>(Height, name: nameof(Height));
                Format = s.Serialize<uint>(Format, name: nameof(Format));
                UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
                Short_14 = s.Serialize<short>(Short_14, name: nameof(Short_14));
                Short_16 = s.Serialize<short>(Short_16, name: nameof(Short_16));
                DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
                UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));

                Data = s.SerializeArray<byte>(Data, DataSize, name: nameof(Data));
            });
        }
	}
}