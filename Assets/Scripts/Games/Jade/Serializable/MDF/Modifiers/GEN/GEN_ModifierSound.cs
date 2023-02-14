using BinarySerializer;

namespace Ray1Map.Jade
{
    public class GEN_ModifierSound : MDF_Modifier 
    {
        public uint Version { get; set; }
        public uint EditorFlags { get; set; }
        public uint ID { get; set; } = uint.MaxValue;
        public Jade_Reference<SND_SModifier> SModifier { get; set; }
        public uint SoundIndex { get; set; } = uint.MaxValue;
        public uint SoundInstance { get; set; } = uint.MaxValue;
        public float PrefetchDistance { get; set; } = 20f;
        public uint ConfigFlags { get; set; }
        public uint CurrentFlags { get; set; }
        public float Delay { get; set; }
        public float DeltaFar { get; set; }
        public int SndTrack { get; set; } = -1;
        public byte[] Reserved { get; set; }

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

            Version = s.Serialize<uint>(Version, name: nameof(Version));
            if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
                if (Version >= 266 && Version <= 273) {
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
                    if (Version <= 268 || ((BGE_Flags & 0x800) == 0)) {
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
            } else if (Version == 274) {
                if (!Loader.IsBinaryData) EditorFlags = s.Serialize<uint>(EditorFlags, name: nameof(EditorFlags));

                ID = s.Serialize<uint>(ID, name: nameof(ID));
                SModifier = s.SerializeObject<Jade_Reference<SND_SModifier>>(SModifier, name: nameof(SModifier))?
                    .Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);

                if (!Loader.IsBinaryData)
                {
                    SoundIndex = s.Serialize<uint>(SoundIndex, name: nameof(SoundIndex));
                    SoundInstance = s.Serialize<uint>(SoundInstance, name: nameof(SoundInstance));
                }

                PrefetchDistance = s.Serialize<float>(PrefetchDistance, name: nameof(PrefetchDistance));
                ConfigFlags = s.Serialize<uint>(ConfigFlags, name: nameof(ConfigFlags));
                CurrentFlags = s.Serialize<uint>(CurrentFlags, name: nameof(CurrentFlags));
                Delay = s.Serialize<float>(Delay, name: nameof(Delay));
                DeltaFar = s.Serialize<float>(DeltaFar, name: nameof(DeltaFar));
                SndTrack = s.Serialize<int>(SndTrack, name: nameof(SndTrack));

                if (!Loader.IsBinaryData) Reserved = s.SerializeArray(Reserved, 40, name: nameof(Reserved));
            }
        }
    }
}
