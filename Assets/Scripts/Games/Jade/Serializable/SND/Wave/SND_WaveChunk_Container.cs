using BinarySerializer;
using System;
using System.Text;

namespace Ray1Map.Jade {
	public class SND_WaveChunk_Container : BinarySerializable {
		public string ChunkID { get; set; }
		public uint ChunkDataSize { get; set; }
		public SND_WaveChunk Chunk { get; set; }
		public bool HasPadding { get; set; }

		public uint EditorSize => 4 + (uint)(HasSize ? 4 : 0) + (uint)(Chunk?.EditorSize ?? 0) + (uint)(HasPadding ? 1 : 0);

		public override void SerializeImpl(SerializerObject s) {

			ChunkID = s.SerializeString(ChunkID, 4, Encoding.ASCII, name: nameof(ChunkID));
			if(HasSize) ChunkDataSize = s.Serialize<uint>(ChunkDataSize, name: nameof(ChunkDataSize));

			Chunk = Type switch
			{
				ChunkType.Data => SerializeChunk<SND_WaveChunk_Data>(s),
				ChunkType.Format => SerializeChunk<SND_WaveChunk_Format>(s),
				ChunkType.Cue => SerializeChunk<SND_WaveChunk_Cue>(s),
				ChunkType.List => SerializeChunk<SND_WaveChunk_List>(s),
				ChunkType.Label => SerializeChunk<SND_WaveChunk_Label>(s),
				ChunkType.LabeledText => SerializeChunk<SND_WaveChunk_LabeledText>(s),
				_ => throw new NotImplementedException($"SND_WaveChunk type {Type} not implemented!"),
			};
			if (((s.CurrentAbsoluteOffset - Offset.AbsoluteOffset) % 2) != 0) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				if(!Loader.IsBinaryData) s.SerializePadding(1, logIfNotNull: true);
				HasPadding = true;
			}
		}
		private T SerializeChunk<T>(SerializerObject s) where T : SND_WaveChunk, new()
			=> s.SerializeObject<T>((T)Chunk, onPreSerialize: c => c.Container = this, name: nameof(Chunk));

		public enum ChunkType {
			Unknown,
			Data,
			Format,
			Cue,
			List,
			Label,
			LabeledText,

		}
		public bool HasSize {
			get {
				switch (Type) {
					default: return true;
				}
			}
		}

		public ChunkType Type {
			get {
				switch (ChunkID) {
					case "fmt ": return ChunkType.Format;
					case "cue ": return ChunkType.Cue;
					case "data": return ChunkType.Data;
					case "LIST": return ChunkType.List;
					case "labl": return ChunkType.Label;
					case "ltxt": return ChunkType.LabeledText;
					case "note":
					case "file":
					case "smpl":
					case "fact":
					case "plst":
					case "JUNK":
					case "MEXT":
					case "INFO":
					case "DISP":
					default: throw new NotImplementedException($"SND_WaveChunk type {ChunkID} is not supported!");
				}
			}
		}
	}
}
