using BinarySerializer;
using System;
using System.Text;

namespace Ray1Map.Jade {
	public class SND_InsertChunk_Container : BinarySerializable {
		public ChunkType Type { get; set; }
		public uint ChunkSize { get; set; }
		public SND_InsertChunk Chunk { get; set; }
		public bool HasPadding { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Type = s.Serialize<ChunkType>(Type, name: nameof(Type));
			ChunkSize = s.Serialize<uint>(ChunkSize, name: nameof(ChunkSize));

			Chunk = Type switch
			{
				ChunkType.XAxisDistance => SerializeChunk<SND_InsertChunk_XAxisDistance>(s),
				ChunkType.XAxisTime => SerializeChunk<SND_InsertChunk_XAxisTime>(s),
				ChunkType.XAxisVar => SerializeChunk<SND_InsertChunk_XAxisVar>(s),
				ChunkType.YAxisVol => SerializeChunk<SND_InsertChunk_YAxisVol>(s),
				ChunkType.YAxisPan => SerializeChunk<SND_InsertChunk_YAxisPan>(s),
				ChunkType.YAxisFreq => SerializeChunk<SND_InsertChunk_YAxisFreq>(s),
				ChunkType.KeyArray => SerializeChunk<SND_InsertChunk_KeyArray>(s),
				_ => throw new NotImplementedException($"SND_InsertChunk type {Type} not implemented!"),
			};

			// Check size
			if (ChunkSize != s.CurrentAbsoluteOffset - Offset.AbsoluteOffset) {
				s?.SystemLogger?.LogWarning($"{Offset}: Incorrect Insert chunk size");
			}
		}

		private T SerializeChunk<T>(SerializerObject s) where T : SND_InsertChunk, new()
			=> s.SerializeObject<T>((T)Chunk, onPreSerialize: c => c.Container = this, name: nameof(Chunk));

		public enum ChunkType : int {
			Dummy = -1,
			XAxisDistance = 0,
			XAxisTime = 1,
			XAxisVar = 2,
			YAxisVol = 3,
			YAxisPan = 4,
			YAxisFreq = 5,
			KeyArray = 6,
		}
	}
}
