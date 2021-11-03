using BinarySerializer;

namespace Ray1Map.GBC
{
    public class GBC_SoundBank : GBC_BaseBlock
    {
        // GBC
        public ushort DataOffsetsOffset { get; set; }
        public ushort UShort_02 { get; set; }
        public ushort SoundIDOffsetCount { get; set; }
        public ushort SoundOffsetCount { get; set; }

        // PPC
        public uint[] FileOffsets { get; set; }
        public SoundFile_PPC[] Files { get; set; }

        // Parsed
        public ushort[] SoundIDOffsets { get; set; }
        public ushort[] SoundOffsets { get; set; }
        public SoundID[] SoundIDs { get; set; }
        public Sound[] Sounds { get; set; }
        public override void SerializeBlock(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersion == EngineVersion.GBC_R1_PocketPC) {
                FileOffsets = s.SerializeArraySize<uint, uint>(FileOffsets, name: nameof(FileOffsets));
                FileOffsets = s.SerializeArray<uint>(FileOffsets, FileOffsets.Length, name: nameof(FileOffsets));
                if(Files == null) Files = new SoundFile_PPC[FileOffsets.Length];
                for (int i = 0; i < FileOffsets.Length; i++) {
                    if(FileOffsets[i] == 0) continue;
                    s.Goto(BlockStartPointer + FileOffsets[i]);
                    Files[i] = s.SerializeObject<SoundFile_PPC>(Files[i], name: $"{nameof(Files)}[{i}]");
                }
            } else {
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
                    int nextOff = i < Sounds.Length - 1 ? SoundOffsets[i + 1] : (int)BlockSize;
                    int length = nextOff - SoundOffsets[i];
                    s.Goto(BlockStartPointer + SoundOffsets[i]);
                    Sounds[i] = s.SerializeObject<Sound>(Sounds[i], onPreSerialize: d => d.Length = length, name: $"{nameof(Sounds)}[{i}]");
                    if (Sounds[i].SoundProgramID.HasValue) {
                        Sounds[i].SoundProgram = s.DoAt(DependencyTable.GetPointer(Sounds[i].SoundProgramID.Value - 1), () => {
                            return s.SerializeObject<GBC_SoundProgram>(Sounds[i].SoundProgram, name: nameof(Sound.SoundProgram));
                        });
                    }
                }
            }
        }

		public class SoundID : BinarySerializable {
            public byte[] Data { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Data = s.SerializeArray<byte>(Data, 4, name: nameof(Data));
			}
        }

        public class Sound : BinarySerializable {
            public int Length { get; set; }

            public byte Command { get; set; }
            public byte? SoundProgramID { get; set; }
            public byte[] Data { get; set; }
            public GBC_SoundProgram SoundProgram { get; set; }
            public override void SerializeImpl(SerializerObject s) {
                Command = s.Serialize<byte>(Command, name: nameof(Command));
                if (Command == 0) {
                    if (s.GetR1Settings().Game == Game.GBC_DD || s.GetR1Settings().Game == Game.GBC_Mowgli) {
                        Data = s.SerializeArray<byte>(Data, 1, name: nameof(Data));
                    }
                    SoundProgramID = s.Serialize<byte>(SoundProgramID.HasValue ? SoundProgramID.Value : (byte)0, name: nameof(SoundProgramID));
                } else {
                    Data = s.SerializeArray<byte>(Data, Length - 2, name: nameof(Data));
                }
            }
        }

		public class SoundFile_PPC : BinarySerializable {
            public FileType Type { get; set; }
            public byte[] Data { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Type = s.Serialize<FileType>(Type, name: nameof(Type));
                Data = s.SerializeArraySize<byte, uint>(Data, name: nameof(Data));
                Data = s.SerializeArray<byte>(Data, Data.Length, name: nameof(Data));
			}

            public enum FileType : int {
                XM = 1,
                WAV = 2
            }
		}
	}
}