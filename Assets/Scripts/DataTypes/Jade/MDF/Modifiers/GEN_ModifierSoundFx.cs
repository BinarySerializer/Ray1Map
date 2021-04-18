using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class GEN_ModifierSoundFx : MDF_Modifier {
        public uint UInt_00 { get; set; }
        public uint UInt_04 { get; set; }
        public float Float_08 { get; set; }
        public float Float_0C { get; set; }
        public float Float_10 { get; set; }
        public float Float_14 { get; set; }
        public uint UInt_18 { get; set; }
        public uint UInt_1C { get; set; }
        public uint UInt_20 { get; set; }
        public uint UInt_24 { get; set; }
        public float Float_28 { get; set; }
        public float Float_2C { get; set; }
        public float Float_30 { get; set; }
        public float Float_34 { get; set; }
        public uint UInt_38 { get; set; }
        public uint Uint_3C_Editor { get; set; }
        public byte[] Bytes_40_Editor { get; set; }

        public uint BGE_Flags { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            if (s.GetR1Settings().Game == Game.Jade_BGE) {
                BGE_Flags = s.Serialize<uint>(BGE_Flags, name: nameof(BGE_Flags));
                if (BGE_Flags == 0x100) {
                    UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
                    UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
                    Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
                    Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
                    UInt_18 = s.Serialize<uint>(UInt_18, name: nameof(UInt_18));
                    UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
                    UInt_20 = s.Serialize<uint>(UInt_20, name: nameof(UInt_20));
                    Float_28 = s.Serialize<float>(Float_28, name: nameof(Float_28));
                    UInt_38 = s.Serialize<uint>(UInt_38, name: nameof(UInt_38));
                    if (!Loader.IsBinaryData) Bytes_40_Editor = s.SerializeArray(Bytes_40_Editor, 0x100, name: nameof(Bytes_40_Editor));
                }
            } else {
                UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
                UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));

                Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
                Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
                Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
                Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));

                UInt_18 = s.Serialize<uint>(UInt_18, name: nameof(UInt_18));
                UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
                UInt_20 = s.Serialize<uint>(UInt_20, name: nameof(UInt_20));
                UInt_24 = s.Serialize<uint>(UInt_24, name: nameof(UInt_24));

                Float_28 = s.Serialize<float>(Float_28, name: nameof(Float_28));
                Float_2C = s.Serialize<float>(Float_2C, name: nameof(Float_2C));
                Float_30 = s.Serialize<float>(Float_30, name: nameof(Float_30));
                Float_34 = s.Serialize<float>(Float_34, name: nameof(Float_34));

                UInt_38 = s.Serialize<uint>(UInt_38, name: nameof(UInt_38));

                if (!Loader.IsBinaryData) {
                    Uint_3C_Editor = s.Serialize<uint>(Uint_3C_Editor, name: nameof(Uint_3C_Editor));
                    Bytes_40_Editor = s.SerializeArray(Bytes_40_Editor, 48, name: nameof(Bytes_40_Editor));
                }
            }
        }
    }
}
