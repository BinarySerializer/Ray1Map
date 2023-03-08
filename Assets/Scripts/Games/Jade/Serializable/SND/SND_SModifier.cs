using System;
using BinarySerializer;

namespace Ray1Map.Jade {
    public class SND_SModifier : Jade_File {
		public override string Export_Extension => "smd";

		public uint FormatVersion { get; set; }
        public uint HeaderDataSize { get; set; }

        public uint SModifierId { get; set; }
        public uint DataSize { get; set; }
        public SoundRef Sound { get; set; }
        public int SndIndex { get; set; }
        public uint Version { get; set; }
        public Jade_Key FileKey { get; set; }
        public uint SndExtFlags { get; set; }
        public uint Template { get; set; }
        public uint PlayerFlag { get; set; }
        public float DryVol { get; set; }
        public float DryVol_FactMin { get; set; }
        public float DryVol_FactMax { get; set; }
        public float FxVolLeft { get; set; }
        public Jade_Reference<SND_Insert> FadeIn { get; set; }
        public int FadeInPointer { get; set; }
        public Jade_Reference<SND_Insert> FadeOut { get; set; }
        public int FadeOutPointer { get; set; }
        public int Pan { get; set; }
        public int Span { get; set; }
        public float MinPan { get; set; }
        public float FreqCoef { get; set; }
        public uint Freq { get; set; }
        public float Freq_FactMin { get; set; }
        public float Freq_FactMax { get; set; }
        public float Doppler { get; set; }
        public float[] Near { get; set; }
        public float[] Far { get; set; }
        public float[] MiddleBlend { get; set; }
        public float FarCoeff { get; set; }
        public float MiddleCoeff { get; set; }
        public float CylinderHeight { get; set; }
        public float FxVolRight { get; set; }
        public byte[] Editor_Bytes_21 { get; set; }
        public ushort PlayListSize { get; set; }
        public ushort PlayListFlags { get; set; }
        public ushort InsertListSize { get; set; }
        public ushort InsertListFlags { get; set; }

        public SoundRef[] PlayList { get; set; }
        public Jade_Reference<SND_Insert>[] InsertList { get; set; }
        
        public uint FooterId { get; set; } = uint.MaxValue;
        public uint FooterDataSize { get; set; } = 0;

        public byte[] Bytes { get; set; }

        protected override void SerializeFile(SerializerObject s) {
            if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
                Bytes = s.SerializeArray<byte>(Bytes, FileSize, name: nameof(Bytes));
                return;
            }
            // Header chunk
            FormatVersion = s.Serialize<uint>(FormatVersion, name: nameof(FormatVersion));
            HeaderDataSize = s.Serialize<uint>(HeaderDataSize, name: nameof(HeaderDataSize));

            // ExtPlayer chunk
            SModifierId = s.Serialize<uint>(SModifierId, name: nameof(SModifierId));
            DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
            Sound = s.SerializeObject<SoundRef>(Sound, name: nameof(Sound));
            SndIndex = s.Serialize<int>(SndIndex, name: nameof(SndIndex));
            Version = s.Serialize<uint>(Version, name: nameof(Version));
            FileKey = s.SerializeObject<Jade_Key>(FileKey, name: nameof(FileKey));
            SndExtFlags = s.Serialize<uint>(SndExtFlags, name: nameof(SndExtFlags));
            if(!Loader.IsBinaryData) Template = s.Serialize<uint>(Template, name: nameof(Template));
            PlayerFlag = s.Serialize<uint>(PlayerFlag, name: nameof(PlayerFlag));
            DryVol = s.Serialize<float>(DryVol, name: nameof(DryVol));
            DryVol_FactMin = s.Serialize<float>(DryVol_FactMin, name: nameof(DryVol_FactMin));
            DryVol_FactMax = s.Serialize<float>(DryVol_FactMax, name: nameof(DryVol_FactMax));
            FxVolLeft = s.Serialize<float>(FxVolLeft, name: nameof(FxVolLeft));
            FadeIn = s.SerializeObject<Jade_Reference<SND_Insert>>(FadeIn, name: nameof(FadeIn));
            if (!Loader.IsBinaryData) FadeInPointer = s.Serialize<int>(FadeInPointer, name: nameof(FadeInPointer));
            FadeOut = s.SerializeObject<Jade_Reference<SND_Insert>>(FadeOut, name: nameof(FadeOut));
            if (!Loader.IsBinaryData) FadeOutPointer = s.Serialize<int>(FadeOutPointer, name: nameof(FadeOutPointer));
            Pan = s.Serialize<int>(Pan, name: nameof(Pan));
            Span = s.Serialize<int>(Span, name: nameof(Span));
            MinPan = s.Serialize<float>(MinPan, name: nameof(MinPan));
            if (FormatVersion >= 3 && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRRPrototype)) {
                FreqCoef = s.Serialize<float>(FreqCoef, name: nameof(FreqCoef));
            } else {
                Freq = s.Serialize<uint>(Freq, name: nameof(Freq));
                FreqCoef = 1f;
            }
            Freq_FactMin = s.Serialize<float>(Freq_FactMin, name: nameof(Freq_FactMin));
            Freq_FactMax = s.Serialize<float>(Freq_FactMax, name: nameof(Freq_FactMax));
            Doppler = s.Serialize<float>(Doppler, name: nameof(Doppler));
            Near = s.SerializeArray<float>(Near, 3, name: nameof(Near));
            Far = s.SerializeArray<float>(Far, 3, name: nameof(Far));
            MiddleBlend = s.SerializeArray<float>(MiddleBlend, 3, name: nameof(MiddleBlend));
            FarCoeff = s.Serialize<float>(FarCoeff, name: nameof(FarCoeff));
            MiddleCoeff = s.Serialize<float>(MiddleCoeff, name: nameof(MiddleCoeff));
            CylinderHeight = s.Serialize<float>(CylinderHeight, name: nameof(CylinderHeight));
            FxVolRight = s.Serialize<float>(FxVolRight, name: nameof(FxVolRight));

            if(!Loader.IsBinaryData) Editor_Bytes_21 = s.SerializeArray<byte>(Editor_Bytes_21, 72, name: nameof(Editor_Bytes_21));
            PlayListSize = s.Serialize<ushort>(PlayListSize, name: nameof(PlayListSize));
            PlayListFlags = s.Serialize<ushort>(PlayListFlags, name: nameof(PlayListFlags));
            InsertListSize = s.Serialize<ushort>(InsertListSize, name: nameof(InsertListSize));
            InsertListFlags = s.Serialize<ushort>(InsertListFlags, name: nameof(InsertListFlags));

            PlayList = s.SerializeObjectArray<SoundRef>(PlayList, PlayListSize, name: nameof(PlayList));
            InsertList = s.SerializeObjectArray<Jade_Reference<SND_Insert>>(InsertList, InsertListSize, name: nameof(InsertList));
            
            // Closing chunk
            FooterId = s.Serialize<uint>(FooterId, name: nameof(FooterId));
            FooterDataSize = s.Serialize<uint>(FooterDataSize, name: nameof(FooterDataSize));

            if (PlayListSize > 0) {
                foreach(var sound in PlayList)
                    sound.Resolve(s);
            } else {
                Sound.Resolve(s);
            }
            FadeIn.Resolve(onPreSerialize: (_,f) => f.IsFade = true, flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.HasUserCounter);
            FadeOut.Resolve(onPreSerialize: (_, f) => f.IsFade = true, flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.HasUserCounter);
            foreach(var insert in InsertList)
                insert.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.HasUserCounter);
        }

		public class SoundRef : BinarySerializable {
            public Jade_Reference<SND_Wave> Wave { get; set; }
            public SoundFlags Flags { get; set; }
            public bool RRR2_Bool { get; set; }
            public override void SerializeImpl(SerializerObject s) {
				Wave = s.SerializeObject<Jade_Reference<SND_Wave>>(Wave, name: nameof(Wave));
                Flags = s.Serialize<SoundFlags>(Flags, name: nameof(Flags));
			}

            public void Resolve(SerializerObject s) {
                // All of these are just waves
                var soundType = SND_Wave.Type.Sound;
                if (Flags.HasFlag(SoundFlags.Dialog)) {
                    soundType = SND_Wave.Type.Dialog;
                } else if (Flags.HasFlag(SoundFlags.Music)) {
                    soundType = SND_Wave.Type.Music;
                } else if (Flags.HasFlag(SoundFlags.Ambience)) {
                    soundType = SND_Wave.Type.Ambience;
                // else if modifier in BG&E only, in GEN_ModifierSound
                } else if (Flags.HasFlag(SoundFlags.LoadingSound)) {
                    soundType = SND_Wave.Type.LoadingSound;
                } else { // Sound
                    soundType = SND_Wave.Type.Sound;
                }
                if (Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRR2)) {
                    LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
                    if (!Wave.IsNull && Loader.IsBinaryData) {
                        RRR2_Bool = s.Serialize<bool>(RRR2_Bool, name: nameof(RRR2_Bool));
                        if(RRR2_Bool) return;
                    }
                }
                if (Context.GetR1Settings().EngineVersion == EngineVersion.Jade_KingKong_Xenon) return;
                Wave.Resolve(onPreSerialize: (_, w) => w.SoundType = soundType, flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.HasUserCounter);
            }

            [Flags]
            public enum SoundFlags : uint {
                None = 0,
                Flag_0 = 1 << 0,
                Flag_1 = 1 << 1,
                Flag_2 = 1 << 2,
                LoadingSound = 1 << 3,
                Flag_4 = 1 << 4,
                Flag_5 = 1 << 5,
                Flag_6 = 1 << 6,
                Flag_7 = 1 << 7,
                Flag_8 = 1 << 8,
                Flag_9 = 1 << 9,
                Flag_10 = 1 << 10,
                Flag_11 = 1 << 11,
                Flag_12 = 1 << 12,
                Music = 1 << 13,
                Dialog = 1 << 14,
                Flag_15 = 1 << 15,
                SModifier = 1 << 16,
                Flag_17 = 1 << 17,
                Flag_18 = 1 << 18,
                Ambience = 1 << 19,
                Flag_20 = 1 << 20,
                Flag_21 = 1 << 21,
                Flag_22 = 1 << 22,
                Flag_23 = 1 << 23,
                Flag_24 = 1 << 24,
                Flag_25 = 1 << 25,
                Flag_26 = 1 << 26,
                Flag_27 = 1 << 27,
                Flag_28 = 1 << 28,
                Flag_29 = 1 << 29,
                Flag_30 = 1 << 30,
                Flag_31 = (uint)1 << 31,
            }
        }
	}
}