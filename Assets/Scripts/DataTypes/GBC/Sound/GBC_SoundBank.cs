namespace R1Engine
{
    public class GBC_SoundBank : GBC_BaseBlock
    {
        public ushort DataOffsetsOffset { get; set; }
        public ushort UShort_02 { get; set; }
        public ushort SoundIDOffsetCount { get; set; }
        public ushort SoundOffsetCount { get; set; }

        // Parsed
        public ushort[] SoundIDOffsets { get; set; }
        public ushort[] SoundOffsets { get; set; }
        public SoundID[] SoundIDs { get; set; }
        public Sound[] Sounds { get; set; }
        public override void SerializeBlock(SerializerObject s)
        {
            DataOffsetsOffset = s.Serialize<ushort>(DataOffsetsOffset, name: nameof(DataOffsetsOffset));
            UShort_02 = s.Serialize<ushort>(UShort_02, name: nameof(UShort_02));
            SoundIDOffsetCount = s.Serialize<ushort>(SoundIDOffsetCount, name: nameof(SoundIDOffsetCount));
            SoundOffsetCount = s.Serialize<ushort>(SoundOffsetCount, name: nameof(SoundOffsetCount));

            s.DoAt(BlockStartPointer + DataOffsetsOffset, () => {
                SoundIDOffsets = s.SerializeArray<ushort>(SoundIDOffsets, SoundIDOffsetCount, name: nameof(SoundIDOffsets));
                SoundOffsets = s.SerializeArray<ushort>(SoundOffsets, SoundOffsetCount, name: nameof(SoundOffsets));
            });
            if (SoundIDs == null) SoundIDs = new SoundID[SoundIDOffsetCount];
            if (Sounds == null) Sounds = new Sound[SoundOffsetCount];
            for (int i = 0; i < SoundIDs.Length; i++) {
                s.Goto(BlockStartPointer + SoundIDOffsets[i]);
                SoundIDs[i] = s.SerializeObject<SoundID>(SoundIDs[i], name: $"{nameof(SoundIDs)}[{i}]");
            }
            for (int i = 0; i < Sounds.Length; i++) {
                int nextOff = i < Sounds.Length - 1 ? SoundOffsets[i+1] : (int)BlockSize;
                int length = nextOff - SoundOffsets[i];
                s.Goto(BlockStartPointer + SoundOffsets[i]);
                Sounds[i] = s.SerializeObject<Sound>(Sounds[i], onPreSerialize: d => d.Length = length, name: $"{nameof(Sounds)}[{i}]");
                if (Sounds[i].SoundProgramID.HasValue) {
                    Sounds[i].SoundProgram = s.DoAt(DependencyTable.GetPointer(Sounds[i].SoundProgramID.Value-1), () => {
                        return s.SerializeObject<GBC_SoundProgram>(Sounds[i].SoundProgram, name: nameof(Sound.SoundProgram));
                    });
                }
            }
        }

		public class SoundID : R1Serializable {
            public byte[] Data { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Data = s.SerializeArray<byte>(Data, 4, name: nameof(Data));
			}
        }

        public class Sound : R1Serializable {
            public int Length { get; set; }

            public byte Command { get; set; }
            public byte? SoundProgramID { get; set; }
            public byte[] Data { get; set; }
            public GBC_SoundProgram SoundProgram { get; set; }
            public override void SerializeImpl(SerializerObject s) {
                Command = s.Serialize<byte>(Command, name: nameof(Command));
                if (Command == 0) {
                    if (s.GameSettings.Game == Game.GBC_DD || s.GameSettings.Game == Game.GBC_Mowgli) {
                        Data = s.SerializeArray<byte>(Data, 1, name: nameof(Data));
                    }
                    SoundProgramID = s.Serialize<byte>(SoundProgramID.HasValue ? SoundProgramID.Value : (byte)0, name: nameof(SoundProgramID));
                } else {
                    Data = s.SerializeArray<byte>(Data, Length - 2, name: nameof(Data));
                }
            }
        }
    }
}