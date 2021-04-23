using System;
using BinarySerializer;
using UnityEngine;

namespace R1Engine.Jade
{
    public class MAT_SpriteGen : BinarySerializable {
        public uint UInt_00 { get; set; }
        public uint UInt_04 { get; set; }
        public Jade_TextureReference BumpMap { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public float Float_10 { get; set; }
        public float Float_14 { get; set; }
        public uint UInt_18 { get; set; }
        public float Float_1C { get; set; }
        public float Float_20 { get; set; }
        public float Float_24 { get; set; }
        public float Float_28 { get; set; }
        public uint UInt_2C { get; set; }
        public uint UInt_30 { get; set; }
        public float Float_34 { get; set; }
        public uint UInt_38 { get; set; }
        public uint UInt_3C { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
            UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
            BumpMap = s.SerializeObject<Jade_TextureReference>(BumpMap, name: nameof(BumpMap));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
            Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));
            UInt_18 = s.Serialize<uint>(UInt_18, name: nameof(UInt_18));
            Float_1C = s.Serialize<float>(Float_1C, name: nameof(Float_1C));
            Float_20 = s.Serialize<float>(Float_20, name: nameof(Float_20));
            Float_24 = s.Serialize<float>(Float_24, name: nameof(Float_24));
            Float_28 = s.Serialize<float>(Float_28, name: nameof(Float_28));
            UInt_2C = s.Serialize<uint>(UInt_2C, name: nameof(UInt_2C));
            UInt_30 = s.Serialize<uint>(UInt_30, name: nameof(UInt_30));
            Float_34 = s.Serialize<float>(Float_34, name: nameof(Float_34));
            UInt_38 = s.Serialize<uint>(UInt_38, name: nameof(UInt_38));
            UInt_3C = s.Serialize<uint>(UInt_3C, name: nameof(UInt_3C));

            BumpMap?.Resolve(s, RRR2_readBool: true);
        }
	}
}