using BinarySerializer;
using System.Text;

namespace Ray1Map.Jade {
	public class SND_WaveChunk_List : SND_WaveChunk {
		public string Type { get; set; }
		public SND_WaveChunk_Container[] Chunks { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Type = s.SerializeString(Type, 4, encoding: Encoding.ASCII, name: nameof(Type)); // always "adtl" - Associated Data List
			EditorSize = 4;
			if (EditorSize < Container.ChunkDataSize) {
				Chunks = s.SerializeObjectArrayUntil<SND_WaveChunk_Container>(Chunks, c => {
					EditorSize += c.EditorSize;
					return EditorSize >= Container.ChunkDataSize;
				}, name: nameof(Chunks));
			}
		}
	}
}
