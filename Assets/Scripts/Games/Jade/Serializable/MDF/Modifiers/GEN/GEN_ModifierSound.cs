using BinarySerializer;

namespace Ray1Map.Jade
{
    public class GEN_ModifierSound : MDF_Modifier 
    {
        public uint Version { get; set; }
        public uint EditorFlags { get; set; }
        public uint ID { get; set; } = uint.MaxValue;
        public Jade_Reference<SND_SModifier> SModifier { get; set; }
        public int SoundIndex { get; set; } = -1;
        public int SoundInstance { get; set; } = -1;
        public float PrefetchDistance { get; set; } = 20f;
        public uint ConfigFlags { get; set; }
        public uint CurrentFlags { get; set; }
        public float Delay { get; set; }
        public float DeltaFar { get; set; }
        public int SoundTrack { get; set; } = -1;
        public byte[] Reserved { get; set; }

        public uint ModifierFlags { get; set; }
        public Jade_Key BGE_SoundKey { get; set; }
        public Jade_Reference<SND_UnknownBank> BGE_UnknownBank { get; set; }
        public float HighVolume { get; set; }
        public float LowVolume { get; set; }
        public float FadeInDuration { get; set; }
        public float FadeOutDuration { get; set; }
        public int Family { get; set; }
        public uint Frequency { get; set; }
        public int Pan { get; set; }
        public uint FadeInType { get; set; }
        public uint FadeOutType { get; set; }
        public SND_SModifier.SoundRef.SoundFlags SoundFlags { get; set; }
        public uint SndExtFlags { get; set; }
		public float[] Near { get; set; }
		public float[] Far { get; set; }
		public float[] MiddleBlend { get; set; }
		public float FarCoeff { get; set; }
		public float MiddleCoeff { get; set; }
		public float MinPan { get; set; }
        public uint IndexIntoBank { get; set; }
        public uint SoundBankPtr { get; set; }
        public float Tn0 { get; set; }
        public float Tn1 { get; set; }
        public float FreqRef { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            Version = s.Serialize<uint>(Version, name: nameof(Version));
            if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
                if (Version >= 266 && Version <= 273) {
                    ModifierFlags = s.Serialize<uint>(ModifierFlags, name: nameof(ModifierFlags));
					ID = s.Serialize<uint>(ID, name: nameof(ID));
					BGE_SoundKey = s.SerializeObject<Jade_Key>(BGE_SoundKey, name: nameof(BGE_SoundKey));
                    HighVolume = s.Serialize<float>(HighVolume, name: nameof(HighVolume));
                    LowVolume = s.Serialize<float>(LowVolume, name: nameof(LowVolume));
                    FadeInDuration = s.Serialize<float>(FadeInDuration, name: nameof(FadeInDuration));
					PrefetchDistance = s.Serialize<float>(PrefetchDistance, name: nameof(PrefetchDistance));
					FadeOutDuration = s.Serialize<float>(FadeOutDuration, name: nameof(FadeOutDuration));
                    Family = s.Serialize<int>(Family, name: nameof(Family));
                    Frequency = s.Serialize<uint>(Frequency, name: nameof(Frequency));
                    Pan = s.Serialize<int>(Pan, name: nameof(Pan));
                    FadeInType = s.Serialize<uint>(FadeInType, name: nameof(FadeInType));
                    FadeOutType = s.Serialize<uint>(FadeOutType, name: nameof(FadeOutType));
                    SoundFlags = s.Serialize<SND_SModifier.SoundRef.SoundFlags>(SoundFlags, name: nameof(SoundFlags));
                    SndExtFlags = s.Serialize<uint>(SndExtFlags, name: nameof(SndExtFlags));
                    if (!Loader.IsBinaryData) SoundIndex = s.Serialize<int>(SoundIndex, name: nameof(SoundIndex));
					if (Version <= 268 || ((ModifierFlags & 0x800) == 0)) {
                        if (SoundFlags.HasFlag(SND_SModifier.SoundRef.SoundFlags.Dialog)) {
                        } else if (SoundFlags.HasFlag(SND_SModifier.SoundRef.SoundFlags.Music)) {
                        } else if (SoundFlags.HasFlag(SND_SModifier.SoundRef.SoundFlags.Ambience)) {
                        } else if (SoundFlags.HasFlag(SND_SModifier.SoundRef.SoundFlags.SModifier)) {
                            SModifier = new Jade_Reference<SND_SModifier>(Context, BGE_SoundKey);
                            SModifier?.Resolve(immediate: true, flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.HasUserCounter);
                        } else if (SoundFlags.HasFlag(SND_SModifier.SoundRef.SoundFlags.LoadingSound)) {
                        } else { // Sound
                        }
                    } else {
                        BGE_UnknownBank = new Jade_Reference<SND_UnknownBank>(Context, BGE_SoundKey);
                        BGE_UnknownBank?.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.IsIrregularFileFormat);
                    }
                    if (!Loader.IsBinaryData) SoundInstance = s.Serialize<int>(SoundInstance, name: nameof(SoundInstance));

					Near = s.SerializeArray<float>(Near, 3, name: nameof(Near));
					Far = s.SerializeArray<float>(Far, 3, name: nameof(Far));
					DeltaFar = s.Serialize<float>(DeltaFar, name: nameof(DeltaFar));
					SoundTrack = s.Serialize<int>(SoundTrack, name: nameof(SoundTrack));
					MiddleBlend = s.SerializeArray<float>(MiddleBlend, 3, name: nameof(MiddleBlend));
					FarCoeff = s.Serialize<float>(FarCoeff, name: nameof(FarCoeff));
					MiddleCoeff = s.Serialize<float>(MiddleCoeff, name: nameof(MiddleCoeff));
					MinPan = s.Serialize<float>(MinPan, name: nameof(MinPan));
                    IndexIntoBank = s.Serialize<uint>(IndexIntoBank, name: nameof(IndexIntoBank));
                    SoundBankPtr = s.Serialize<uint>(SoundBankPtr, name: nameof(SoundBankPtr));
					Delay = s.Serialize<float>(Delay, name: nameof(Delay));
					if (!Loader.IsBinaryData) {
                        Tn0 = s.Serialize<float>(Tn0, name: nameof(Tn0));
                        Tn1 = s.Serialize<float>(Tn1, name: nameof(Tn1));
                        FreqRef = s.Serialize<float>(FreqRef, name: nameof(FreqRef));
                        Reserved = s.SerializeArray(Reserved, 40, name: nameof(Reserved));
					}
                }
            } else if (Version == 274) {
                if (!Loader.IsBinaryData) EditorFlags = s.Serialize<uint>(EditorFlags, name: nameof(EditorFlags));

                ID = s.Serialize<uint>(ID, name: nameof(ID));
                SModifier = s.SerializeObject<Jade_Reference<SND_SModifier>>(SModifier, name: nameof(SModifier))?
                    .Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.HasUserCounter);

                if (!Loader.IsBinaryData)
                {
                    SoundIndex = s.Serialize<int>(SoundIndex, name: nameof(SoundIndex));
                    SoundInstance = s.Serialize<int>(SoundInstance, name: nameof(SoundInstance));
                }

                PrefetchDistance = s.Serialize<float>(PrefetchDistance, name: nameof(PrefetchDistance));
                ConfigFlags = s.Serialize<uint>(ConfigFlags, name: nameof(ConfigFlags));
                CurrentFlags = s.Serialize<uint>(CurrentFlags, name: nameof(CurrentFlags));
                Delay = s.Serialize<float>(Delay, name: nameof(Delay));
                DeltaFar = s.Serialize<float>(DeltaFar, name: nameof(DeltaFar));
                SoundTrack = s.Serialize<int>(SoundTrack, name: nameof(SoundTrack));

                if (!Loader.IsBinaryData) Reserved = s.SerializeArray(Reserved, 40, name: nameof(Reserved));
            }
        }
    }
}
