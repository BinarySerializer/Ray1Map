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

        public MDF_LoadingSoundState State { get; set; }

        public float HighVolume { get; set; }
        public float LowVolume { get; set; }
        public float FadeInDuration { get; set; }
        public float FadeOutDuration { get; set; }

        public int Family { get; set; }
        public uint Frequency { get; set; }
        public int Pan { get; set; }
		public uint SndExtFlags { get; set; }
        public int SoundInstance { get; set; }

		public float[] Near { get; set; }
		public float[] Far { get; set; }
		public float DeltaFar { get; set; }
        public int SoundTrack { get; set; }
		public float[] MiddleBlend { get; set; }
		public float FarCoeff { get; set; }
		public float MiddleCoeff { get; set; }
		public float MinPan { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
				Version = s.Serialize<uint>(Version, name: nameof(Version));

				if (Version == 256) {
                    State = s.Serialize<MDF_LoadingSoundState>(State, name: nameof(State));
                    SoundKey = s.SerializeObject<Jade_Key>(SoundKey, name: nameof(SoundKey));

                    HighVolume = s.Serialize<float>(HighVolume, name: nameof(HighVolume));
                    LowVolume = s.Serialize<float>(LowVolume, name: nameof(LowVolume));
                    FadeInDuration = s.Serialize<float>(FadeInDuration, name: nameof(FadeInDuration));
                    FadeOutDuration = s.Serialize<float>(FadeOutDuration, name: nameof(FadeOutDuration));

                    Family = s.Serialize<int>(Family, name: nameof(Family));
                    Frequency = s.Serialize<uint>(Frequency, name: nameof(Frequency));
                    Pan = s.Serialize<int>(Pan, name: nameof(Pan));
					SoundFlags = s.Serialize<SND_SModifier.SoundRef.SoundFlags>(SoundFlags, name: nameof(SoundFlags));
					SndExtFlags = s.Serialize<uint>(SndExtFlags, name: nameof(SndExtFlags));
					SoundIndex = s.Serialize<int>(SoundIndex, name: nameof(SoundIndex));
					SoundInstance = s.Serialize<int>(SoundInstance, name: nameof(SoundInstance));

					Near = s.SerializeArray<float>(Near, 3, name: nameof(Near));
					Far = s.SerializeArray<float>(Far, 3, name: nameof(Far));
					DeltaFar = s.Serialize<float>(DeltaFar, name: nameof(DeltaFar));
					LoadingDistance = s.Serialize<float>(LoadingDistance, name: nameof(LoadingDistance));

                    SoundTrack = s.Serialize<int>(SoundTrack, name: nameof(SoundTrack));
					MiddleBlend = s.SerializeArray<float>(MiddleBlend, 3, name: nameof(MiddleBlend));
					FarCoeff = s.Serialize<float>(FarCoeff, name: nameof(FarCoeff));
					MiddleCoeff = s.Serialize<float>(MiddleCoeff, name: nameof(MiddleCoeff));
					MinPan = s.Serialize<float>(MinPan, name: nameof(MinPan));

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
                    Wave?.Resolve(onPreSerialize: (_,w) => w.SoundType = SND_Wave.Type.LoadingSound, flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.HasUserCounter);
                } else if (SoundFlags.HasFlag(SND_SModifier.SoundRef.SoundFlags.SModifier)) {
                    SModifier = new Jade_Reference<SND_SModifier>(Context, SoundKey);
                    SModifier?.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.HasUserCounter);
                }
            }
        }

        public enum MDF_LoadingSoundState : uint {
            None = 0,
			WaitingForLoading = 1,
			WaitingForPlaying = 2,
			Playing = 3,
			Disabled = 0xFFFFFFFE
        }
    }
}
