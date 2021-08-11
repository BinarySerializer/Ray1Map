using BinarySerializer;
using System.Text;

namespace R1Engine.Jade {
	public class SND_WaveChunk_Format : SND_WaveChunk {
		public short FormatTag { get; set; } // Codec
		public ushort Channels { get; set; }
		public uint SamplesPerSecond { get; set; }
		public uint AverageBytesPerSecond { get; set; }
		public ushort BlockAlign { get; set; }

		public ushort BitsPerSample { get; set; }
		public ushort ExtraFormatBytesCount { get; set; }
		public byte[] ExtraFormatBytes { get; set; }
		public ushort SamplesPerBlock { get; set; }

		public ushort Format2Count { get; set; }
		public Format2Struct[] Format2Structs { get; set; }

		public ushort FormatFFFEPadding { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			FormatTag = s.Serialize<short>(FormatTag, name: nameof(FormatTag));
			Channels = s.Serialize<ushort>(Channels, name: nameof(Channels));
			SamplesPerSecond = s.Serialize<uint>(SamplesPerSecond, name: nameof(SamplesPerSecond));
			AverageBytesPerSecond = s.Serialize<uint>(AverageBytesPerSecond, name: nameof(AverageBytesPerSecond));
			BlockAlign = s.Serialize<ushort>(BlockAlign, name: nameof(BlockAlign));

			EditorSize = 14;

			switch (FormatTag) {
				case -2: // GC/Wii DSP
					BitsPerSample = s.Serialize<ushort>(BitsPerSample, name: nameof(BitsPerSample));
					FormatFFFEPadding = s.Serialize<ushort>(FormatFFFEPadding, name: nameof(FormatFFFEPadding));
					SamplesPerBlock = 0xE;
					EditorSize += 4;
					break;
				case -1: // PS2 ADPCM
					BitsPerSample = s.Serialize<ushort>(BitsPerSample, name: nameof(BitsPerSample));
					ExtraFormatBytesCount = s.Serialize<ushort>(ExtraFormatBytesCount, name: nameof(ExtraFormatBytesCount));
					SamplesPerBlock = 0x1C;
					ExtraFormatBytes = s.SerializeArray<byte>(ExtraFormatBytes, ExtraFormatBytesCount, name: nameof(ExtraFormatBytes));
					EditorSize += 4 + (uint)ExtraFormatBytesCount;
					break;
				case 1: // PCM
					if (Container.ChunkDataSize >= 16) BitsPerSample = s.Serialize<ushort>(BitsPerSample, name: nameof(BitsPerSample));
					if (Container.ChunkDataSize >= 18) ExtraFormatBytesCount = s.Serialize<ushort>(ExtraFormatBytesCount, name: nameof(ExtraFormatBytesCount));
					ExtraFormatBytes = s.SerializeArray<byte>(ExtraFormatBytes, ExtraFormatBytesCount, name: nameof(ExtraFormatBytes));
					EditorSize += 4 + (uint)ExtraFormatBytesCount + (uint)((Container.ChunkDataSize >= 16) ? 2 : 0) + (uint)((Container.ChunkDataSize >= 18) ? 2 : 0);
					break;
				case 2: // PC (MS ADPCM)
					BitsPerSample = s.Serialize<ushort>(BitsPerSample, name: nameof(BitsPerSample));
					ExtraFormatBytesCount = s.Serialize<ushort>(ExtraFormatBytesCount, name: nameof(ExtraFormatBytesCount));
					SamplesPerBlock = s.Serialize<ushort>(SamplesPerBlock, name: nameof(SamplesPerBlock));
					Format2Count = s.Serialize<ushort>(Format2Count, name: nameof(Format2Count));
					Format2Structs = s.SerializeObjectArray<Format2Struct>(Format2Structs, Format2Count, name: nameof(Format2Structs));
					ExtraFormatBytes = s.SerializeArray<byte>(ExtraFormatBytes, ExtraFormatBytesCount, name: nameof(ExtraFormatBytes));

					EditorSize += 8 + (uint)ExtraFormatBytesCount + (uint)(Format2Count * 8);
					break;
				case 0x69: // Xbox (IMA ADPCM)
					BitsPerSample = s.Serialize<ushort>(BitsPerSample, name: nameof(BitsPerSample));
					ExtraFormatBytesCount = s.Serialize<ushort>(ExtraFormatBytesCount, name: nameof(ExtraFormatBytesCount));
					if (ExtraFormatBytesCount == 2) {
						SamplesPerBlock = s.Serialize<ushort>(SamplesPerBlock, name: nameof(SamplesPerBlock));
					} else {
						ExtraFormatBytes = s.SerializeArray<byte>(ExtraFormatBytes, ExtraFormatBytesCount, name: nameof(ExtraFormatBytes));
					}

					EditorSize += 4 + (uint)ExtraFormatBytesCount;
					break;
			}
		}

		public class Format2Struct : BinarySerializable {
			public uint UInt0 { get; set; }
			public uint UInt1 { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				UInt0 = s.Serialize<uint>(UInt0, name: nameof(UInt0));
				UInt1 = s.Serialize<uint>(UInt1, name: nameof(UInt1));
			}
		}
	}
}
