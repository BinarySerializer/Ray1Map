using BinarySerializer;
using System;
using System.Text;

namespace R1Engine.Jade {
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
				ChunkType.Data => s.SerializeObject<SND_WaveChunk_Data>((SND_WaveChunk_Data)Chunk, onPreSerialize: c => c.Container = this, name: nameof(Chunk)),
				ChunkType.Format => s.SerializeObject<SND_WaveChunk_Format>((SND_WaveChunk_Format)Chunk, onPreSerialize: c => c.Container = this, name: nameof(Chunk)),
				ChunkType.Cue => s.SerializeObject<SND_WaveChunk_Cue>((SND_WaveChunk_Cue)Chunk, onPreSerialize: c => c.Container = this, name: nameof(Chunk)),
				ChunkType.List => s.SerializeObject<SND_WaveChunk_List>((SND_WaveChunk_List)Chunk, onPreSerialize: c => c.Container = this, name: nameof(Chunk)),
				ChunkType.Label => s.SerializeObject<SND_WaveChunk_Label>((SND_WaveChunk_Label)Chunk, onPreSerialize: c => c.Container = this, name: nameof(Chunk)),
				ChunkType.LabeledText => s.SerializeObject<SND_WaveChunk_LabeledText>((SND_WaveChunk_LabeledText)Chunk, onPreSerialize: c => c.Container = this, name: nameof(Chunk)),
				_ => throw new NotImplementedException($"SND_WaveChunk type {Type} not implemented!"),
			};
			if (((s.CurrentAbsoluteOffset - Offset.AbsoluteOffset) % 2) != 0) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				if(!Loader.IsBinaryData) s.SerializePadding(1, logIfNotNull: true);
				HasPadding = true;
			}
		}

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
