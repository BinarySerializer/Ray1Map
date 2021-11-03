using System;
using BinarySerializer;

namespace Ray1Map.Jade
{
    public class MDF_LoadingSound : MDF_Modifier {
        public uint Version { get; set; }
        public uint Flags { get; set; }
        public Jade_Key SoundKey { get; set; }
        public SND_SModifier.SoundRef.SoundFlags SoundFlags { get; set; }
        public int SoundIndex { get; set; }
        public float LoadingDistance { get; set; }
        public byte[] Reserved { get; set; }

        public Jade_Reference<SND_SModifier> SModifier { get; set; }
        public Jade_Reference<SND_Wave> Wave { get; set; }

        public uint BGE_Flags { get; set; }

        public float BGE_Float_3 { get; set; }
        public float BGE_Float_4 { get; set; }
        public float BGE_Float_5 { get; set; }
        public float BGE_Float_6 { get; set; }

        public uint BGE_UInt_8 { get; set; }
        public uint BGE_UInt_9 { get; set; }
        public uint BGE_UInt_10 { get; set; }
        public uint BGE_UInt_11 { get; set; }
        public uint BGE_UInt_12 { get; set; }
        public uint BGE_UInt_13 { get; set; }
        public uint BGE_UInt_14 { get; set; }

        public float BGE_Float_16 { get; set; }
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

        public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
                BGE_Flags = s.Serialize<uint>(BGE_Flags, name: nameof(BGE_Flags));

                if (BGE_Flags == 0x100) {
                    Version = s.Serialize<uint>(Version, name: nameof(Version));
                    Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
                    SoundKey = s.SerializeObject<Jade_Key>(SoundKey, name: nameof(SoundKey));

                    BGE_Float_3 = s.Serialize<float>(BGE_Float_3, name: nameof(BGE_Float_3));
                    BGE_Float_4 = s.Serialize<float>(BGE_Float_4, name: nameof(BGE_Float_4));
                    BGE_Float_5 = s.Serialize<float>(BGE_Float_5, name: nameof(BGE_Float_5));
                    BGE_Float_6 = s.Serialize<float>(BGE_Float_6, name: nameof(BGE_Float_6));

                    BGE_UInt_8 = s.Serialize<uint>(BGE_UInt_8, name: nameof(BGE_UInt_8));
                    BGE_UInt_9 = s.Serialize<uint>(BGE_UInt_9, name: nameof(BGE_UInt_9));
                    BGE_UInt_10 = s.Serialize<uint>(BGE_UInt_10, name: nameof(BGE_UInt_10));
                    BGE_UInt_11 = s.Serialize<uint>(BGE_UInt_11, name: nameof(BGE_UInt_11));
                    BGE_UInt_12 = s.Serialize<uint>(BGE_UInt_12, name: nameof(BGE_UInt_12));
                    BGE_UInt_13 = s.Serialize<uint>(BGE_UInt_13, name: nameof(BGE_UInt_13));
                    BGE_UInt_14 = s.Serialize<uint>(BGE_UInt_14, name: nameof(BGE_UInt_14));

                    BGE_Float_16 = s.Serialize<float>(BGE_Float_16, name: nameof(BGE_Float_16));
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

                    if (!Loader.IsBinaryData) Reserved = s.SerializeArray(Reserved, 0x100, name: nameof(Reserved));
                }
            } else {
                Version = s.Serialize<uint>(Version, name: nameof(Version));
                Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
                SoundKey = s.SerializeObject<Jade_Key>(SoundKey, name: nameof(SoundKey));
                SoundFlags = s.Serialize<SND_SModifier.SoundRef.SoundFlags>(SoundFlags, name: nameof(SoundFlags));
                SoundIndex = s.Serialize<int>(SoundIndex, name: nameof(SoundIndex));
                LoadingDistance = s.Serialize<float>(LoadingDistance, name: nameof(LoadingDistance));

                if (!Loader.IsBinaryData) Reserved = s.SerializeArray(Reserved, 0x100, name: nameof(Reserved));

                if (SoundFlags.HasFlag(SND_SModifier.SoundRef.SoundFlags.LoadingSound)) {
                    Wave = new Jade_Reference<SND_Wave>(Context, SoundKey);
                    if (Context.GetR1Settings().EngineVersion == EngineVersion.Jade_KingKong_Xenon) return;
                    Wave?.Resolve(onPreSerialize: (_,w) => w.SoundType = SND_Wave.Type.LoadingSound, flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);
                } else if (SoundFlags.HasFlag(SND_SModifier.SoundRef.SoundFlags.SModifier)) {
                    SModifier = new Jade_Reference<SND_SModifier>(Context, SoundKey);
                    SModifier?.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);
                }
            }
        }
    }
}
