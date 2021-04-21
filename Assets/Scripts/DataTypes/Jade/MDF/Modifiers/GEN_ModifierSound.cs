using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class GEN_ModifierSound : MDF_Modifier 
    {
        public uint UInt_00 { get; set; }
        public uint Uint_04_Editor { get; set; }
        public uint Uint_04 { get; set; }
        public Jade_Reference<SND_SModifier> SModifier { get; set; }
        public uint Uint_0C_Editor { get; set; }
        public uint Uint_10_Editor { get; set; }
        public float Float_0C { get; set; }
        public uint Uint_10 { get; set; }
        public uint Uint_14 { get; set; }
        public float Float_18 { get; set; }
        public float Float_1C { get; set; }
        public int Int_20 { get; set; }
        public byte[] Bytes_24_Editor { get; set; }

        public uint BGE_Flags { get; set; }
        public uint BGE_UInt_1 { get; set; }
        public Jade_Key BGE_SoundKey { get; set; }
        public Jade_Reference<SND_UnknownBank> BGE_UnknownBank { get; set; }
        public float BGE_Float_3 { get; set; }
        public float BGE_Float_4 { get; set; }
        public float BGE_Float_5 { get; set; }
        public float BGE_Float_6 { get; set; }
        public float BGE_Float_7 { get; set; }
        public uint BGE_UInt_8 { get; set; }
        public uint BGE_UInt_9 { get; set; }
        public uint BGE_UInt_10 { get; set; }
        public uint BGE_UInt_11 { get; set; }
        public uint BGE_UInt_12 { get; set; }
        public SND_SModifier.SoundRef.SoundFlags BGE_SoundFlags { get; set; }
        public uint BGE_UInt_14 { get; set; }
        public uint BGE_UInt_15_Editor { get; set; }
        public uint BGE_UInt_16_Editor { get; set; }
        public float BGE_Float_17 { get; set; }
        public float BGE_Float_18 { get; set; }
        public float BGE_Float_19 { get; set; }
        public float BGE_Float_20 { get; set; }
        public float BGE_Float_21 { get; set; }
        public float BGE_Float_22 { get; set; }
        public float BGE_Float_23 { get; set; }
        public uint BGE_UInt_24 { get; set; }
        public float BGE_Float_25 { get; set; }
        public float BGE_Float_26 { get; set; }
        public float BGE_Float_27 { get; set; }
        public float BGE_Float_28 { get; set; }
        public float BGE_Float_29 { get; set; }
        public float BGE_Float_30 { get; set; }
        public uint BGE_UInt_31 { get; set; }
        public uint BGE_UInt_32 { get; set; }
        public float BGE_Float_33 { get; set; }
        public byte[] BGE_Bytes_34_Editor { get; set; }
        public byte[] BGE_Bytes_35_Editor { get; set; }
        public byte[] BGE_Bytes_36_Editor { get; set; }
        public byte[] BGE_Bytes_37_Editor { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
            if (s.GetR1Settings().EngineVersion < EngineVersion.Jade_KingKong) {
                if (UInt_00 >= 266 && UInt_00 <= 273) {
                    BGE_Flags = s.Serialize<uint>(BGE_Flags, name: nameof(BGE_Flags));
                    BGE_UInt_1 = s.Serialize<uint>(BGE_UInt_1, name: nameof(BGE_UInt_1));
                    BGE_SoundKey = s.SerializeObject<Jade_Key>(BGE_SoundKey, name: nameof(BGE_SoundKey));
                    BGE_Float_3 = s.Serialize<float>(BGE_Float_3, name: nameof(BGE_Float_3));
                    BGE_Float_4 = s.Serialize<float>(BGE_Float_4, name: nameof(BGE_Float_4));
                    BGE_Float_5 = s.Serialize<float>(BGE_Float_5, name: nameof(BGE_Float_5));
                    BGE_Float_6 = s.Serialize<float>(BGE_Float_6, name: nameof(BGE_Float_6));
                    BGE_Float_7 = s.Serialize<float>(BGE_Float_7, name: nameof(BGE_Float_7));
                    BGE_UInt_8 = s.Serialize<uint>(BGE_UInt_8, name: nameof(BGE_UInt_8));
                    BGE_UInt_9 = s.Serialize<uint>(BGE_UInt_9, name: nameof(BGE_UInt_9));
                    BGE_UInt_10 = s.Serialize<uint>(BGE_UInt_10, name: nameof(BGE_UInt_10));
                    BGE_UInt_11 = s.Serialize<uint>(BGE_UInt_11, name: nameof(BGE_UInt_11));
                    BGE_UInt_12 = s.Serialize<uint>(BGE_UInt_12, name: nameof(BGE_UInt_12));
                    BGE_SoundFlags = s.Serialize<SND_SModifier.SoundRef.SoundFlags>(BGE_SoundFlags, name: nameof(BGE_SoundFlags));
                    BGE_UInt_14 = s.Serialize<uint>(BGE_UInt_14, name: nameof(BGE_UInt_14));
                    if (!Loader.IsBinaryData) BGE_UInt_15_Editor = s.Serialize<uint>(BGE_UInt_15_Editor, name: nameof(BGE_UInt_15_Editor));
                    if (UInt_00 <= 268 || ((BGE_Flags & 0x800) == 0)) {
                        if (BGE_SoundFlags.HasFlag(SND_SModifier.SoundRef.SoundFlags.Dialog)) {
                        } else if (BGE_SoundFlags.HasFlag(SND_SModifier.SoundRef.SoundFlags.Music)) {
                        } else if (BGE_SoundFlags.HasFlag(SND_SModifier.SoundRef.SoundFlags.Ambience)) {
                        } else if (BGE_SoundFlags.HasFlag(SND_SModifier.SoundRef.SoundFlags.SModifier)) {
                            SModifier = new Jade_Reference<SND_SModifier>(Context, BGE_SoundKey);
                            SModifier?.Resolve(immediate: true, flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);
                        } else if (BGE_SoundFlags.HasFlag(SND_SModifier.SoundRef.SoundFlags.LoadingSound)) {
                        } else { // Sound
                        }
                    } else {
                        BGE_UnknownBank = new Jade_Reference<SND_UnknownBank>(Context, BGE_SoundKey);
                        BGE_UnknownBank?.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.IsIrregularFileFormat);
                    }
                    if (!Loader.IsBinaryData) BGE_UInt_16_Editor = s.Serialize<uint>(BGE_UInt_16_Editor, name: nameof(BGE_UInt_16_Editor));
                    BGE_Float_17 = s.Serialize<float>(BGE_Float_17, name: nameof(BGE_Float_17));
                    BGE_Float_18 = s.Serialize<float>(BGE_Float_18, name: nameof(BGE_Float_18));
                    BGE_Float_19 = s.Serialize<float>(BGE_Float_19, name: nameof(BGE_Float_19));
                    BGE_Float_20 = s.Serialize<float>(BGE_Float_20, name: nameof(BGE_Float_20));
                    BGE_Float_21 = s.Serialize<float>(BGE_Float_21, name: nameof(BGE_Float_21));
                    BGE_Float_22 = s.Serialize<float>(BGE_Float_22, name: nameof(BGE_Float_22));
                    BGE_Float_23 = s.Serialize<float>(BGE_Float_23, name: nameof(BGE_Float_23));
                    BGE_UInt_24 = s.Serialize<uint>(BGE_UInt_24, name: nameof(BGE_UInt_24));
                    BGE_Float_25 = s.Serialize<float>(BGE_Float_25, name: nameof(BGE_Float_25));
                    BGE_Float_26 = s.Serialize<float>(BGE_Float_26, name: nameof(BGE_Float_26));
                    BGE_Float_27 = s.Serialize<float>(BGE_Float_27, name: nameof(BGE_Float_27));
                    BGE_Float_28 = s.Serialize<float>(BGE_Float_28, name: nameof(BGE_Float_28));
                    BGE_Float_29 = s.Serialize<float>(BGE_Float_29, name: nameof(BGE_Float_29));
                    BGE_Float_30 = s.Serialize<float>(BGE_Float_30, name: nameof(BGE_Float_30));
                    BGE_UInt_31 = s.Serialize<uint>(BGE_UInt_31, name: nameof(BGE_UInt_31));
                    BGE_UInt_32 = s.Serialize<uint>(BGE_UInt_32, name: nameof(BGE_UInt_32));
                    BGE_Float_33 = s.Serialize<float>(BGE_Float_33, name: nameof(BGE_Float_33));
                    if (!Loader.IsBinaryData) {
                        BGE_Bytes_34_Editor = s.SerializeArray<byte>(BGE_Bytes_34_Editor, 4, name: nameof(BGE_Bytes_34_Editor));
                        BGE_Bytes_35_Editor = s.SerializeArray<byte>(BGE_Bytes_35_Editor, 4, name: nameof(BGE_Bytes_35_Editor));
                        BGE_Bytes_36_Editor = s.SerializeArray<byte>(BGE_Bytes_36_Editor, 4, name: nameof(BGE_Bytes_36_Editor));
                        BGE_Bytes_37_Editor = s.SerializeArray<byte>(BGE_Bytes_37_Editor, 0x28, name: nameof(BGE_Bytes_37_Editor));
                    }
                }
            } else if (UInt_00 == 274) {
                if (!Loader.IsBinaryData) Uint_04_Editor = s.Serialize<uint>(Uint_04_Editor, name: nameof(Uint_04_Editor));

                Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));
                SModifier = s.SerializeObject<Jade_Reference<SND_SModifier>>(SModifier, name: nameof(SModifier))?
                    .Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);

                if (!Loader.IsBinaryData)
                {
                    Uint_0C_Editor = s.Serialize<uint>(Uint_0C_Editor, name: nameof(Uint_0C_Editor));
                    Uint_10_Editor = s.Serialize<uint>(Uint_10_Editor, name: nameof(Uint_10_Editor));
                }

                Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
                Uint_10 = s.Serialize<uint>(Uint_10, name: nameof(Uint_10));
                Uint_14 = s.Serialize<uint>(Uint_14, name: nameof(Uint_14));
                Float_18 = s.Serialize<float>(Float_18, name: nameof(Float_18));
                Float_1C = s.Serialize<float>(Float_1C, name: nameof(Float_1C));
                Int_20 = s.Serialize<int>(Int_20, name: nameof(Int_20));

                if (!Loader.IsBinaryData) Bytes_24_Editor = s.SerializeArray(Bytes_24_Editor, 40, name: nameof(Bytes_24_Editor));
            }
        }
    }
}
