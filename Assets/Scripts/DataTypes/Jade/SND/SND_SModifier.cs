using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class SND_SModifier : Jade_File {
        public uint Type { get; set; }
        public uint UInt_01 { get; set; }
        public uint UInt_02 { get; set; }
        public uint UInt_03 { get; set; }
        public SoundRef Sound { get; set; }
        public int Int_06 { get; set; }
        public uint UInt_07 { get; set; }
        public uint UInt_08 { get; set; }
        public uint UInt_09 { get; set; }
        public uint UInt_09_Editor { get; set; }
        public uint UInt_10 { get; set; }
        public float Float_11 { get; set; }
        public float Float_12 { get; set; }
        public float Float_13 { get; set; }
        public float Float_14 { get; set; }
        public Jade_Reference<SND_Insert> Insert_15 { get; set; }
        public int Int_15_Editor { get; set; }
        public Jade_Reference<SND_Insert> Insert_16 { get; set; }
        public int Int_16_Editor { get; set; }
        public uint UInt_17 { get; set; }
        public uint UInt_18 { get; set; }
        public float Float_19 { get; set; }
        public float Type3_Float_20 { get; set; }
        public uint Type2_UInt_20 { get; set; }
        public float[] Floats_21 { get; set; }
        public byte[] Editor_Bytes_21 { get; set; }
        public ushort SoundsCount { get; set; }
        public ushort UShort_23 { get; set; }
        public ushort UIntCount { get; set; }
        public ushort UShort_25 { get; set; }

        public SoundRef[] Sounds { get; set; }
        public Jade_Reference<SND_Insert>[] Inserts { get; set; }
        
        public uint UInt_26 { get; set; }
        public uint UInt_27 { get; set; }

        public byte[] Bytes { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            if (s.GetR1Settings().EngineVersion < EngineVersion.Jade_KingKong) {
                Bytes = s.SerializeArray<byte>(Bytes, FileSize, name: nameof(Bytes));
                return;
            }
            Type = s.Serialize<uint>(Type, name: nameof(Type));
            UInt_01 = s.Serialize<uint>(UInt_01, name: nameof(UInt_01));
            UInt_02 = s.Serialize<uint>(UInt_02, name: nameof(UInt_02));
            UInt_03 = s.Serialize<uint>(UInt_03, name: nameof(UInt_03));
            Sound = s.SerializeObject<SoundRef>(Sound, name: nameof(Sound));
            Int_06 = s.Serialize<int>(Int_06, name: nameof(Int_06));
            UInt_07 = s.Serialize<uint>(UInt_07, name: nameof(UInt_07));
            UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
            UInt_09 = s.Serialize<uint>(UInt_09, name: nameof(UInt_09));
            if(!Loader.IsBinaryData) UInt_09_Editor = s.Serialize<uint>(UInt_09_Editor, name: nameof(UInt_09_Editor));
            UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
            Float_11 = s.Serialize<float>(Float_11, name: nameof(Float_11));
            Float_12 = s.Serialize<float>(Float_12, name: nameof(Float_12));
            Float_13 = s.Serialize<float>(Float_13, name: nameof(Float_13));
            Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));
            Insert_15 = s.SerializeObject<Jade_Reference<SND_Insert>>(Insert_15, name: nameof(Insert_15));
            if (!Loader.IsBinaryData) Int_15_Editor = s.Serialize<int>(Int_15_Editor, name: nameof(Int_15_Editor));
            Insert_16 = s.SerializeObject<Jade_Reference<SND_Insert>>(Insert_16, name: nameof(Insert_16));
            if (!Loader.IsBinaryData) Int_16_Editor = s.Serialize<int>(Int_16_Editor, name: nameof(Int_16_Editor));
            UInt_17 = s.Serialize<uint>(UInt_17, name: nameof(UInt_17));
            UInt_18 = s.Serialize<uint>(UInt_18, name: nameof(UInt_18));
            Float_19 = s.Serialize<float>(Float_19, name: nameof(Float_19));
            if (Type >= 3) {
                Type3_Float_20 = s.Serialize<float>(Type3_Float_20, name: nameof(Type3_Float_20));
            } else {
                Type2_UInt_20 = s.Serialize<uint>(Type2_UInt_20, name: nameof(Type2_UInt_20));
                Type3_Float_20 = 1f;
            }
            Floats_21 = s.SerializeArray<float>(Floats_21, 16, name: nameof(Floats_21));
            if(!Loader.IsBinaryData) Editor_Bytes_21 = s.SerializeArray<byte>(Editor_Bytes_21, 72, name: nameof(Editor_Bytes_21));
            SoundsCount = s.Serialize<ushort>(SoundsCount, name: nameof(SoundsCount));
            UShort_23 = s.Serialize<ushort>(UShort_23, name: nameof(UShort_23));
            UIntCount = s.Serialize<ushort>(UIntCount, name: nameof(UIntCount));
            UShort_25 = s.Serialize<ushort>(UShort_25, name: nameof(UShort_25));

            Sounds = s.SerializeObjectArray<SoundRef>(Sounds, SoundsCount, name: nameof(Sounds));
            Inserts = s.SerializeObjectArray<Jade_Reference<SND_Insert>>(Inserts, UIntCount, name: nameof(Inserts));
            UInt_26 = s.Serialize<uint>(UInt_26, name: nameof(UInt_26));
            UInt_27 = s.Serialize<uint>(UInt_27, name: nameof(UInt_27));

            if (SoundsCount > 0) {
                foreach(var sound in Sounds)
                    sound.Resolve(s);
            } else {
                Sound.Resolve(s);
            }
            Insert_15.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);
            Insert_16.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);
            foreach(var insert in Inserts)
                insert.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);
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
                /*if (Flags.HasFlag(SoundFlags.Dialog)) {
                } else if (Flags.HasFlag(SoundFlags.Music)) {
                } else if (Flags.HasFlag(SoundFlags.Ambience)) {
                // else if modifier in BG&E only, in GEN_ModifierSound
                } else if (Flags.HasFlag(SoundFlags.LoadingSound)) {
                } else { // Sound
                }*/
                if (Context.GetR1Settings().EngineVersion >= EngineVersion.Jade_RRR2) {
                    LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
                    if (!Wave.IsNull && Loader.IsBinaryData) {
                        RRR2_Bool = s.Serialize<bool>(RRR2_Bool, name: nameof(RRR2_Bool));
                        if(RRR2_Bool) return;
                    }
                }
                if (Context.GetR1Settings().EngineVersion == EngineVersion.Jade_KingKong_PCGamersEdition) return;
                Wave.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);
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