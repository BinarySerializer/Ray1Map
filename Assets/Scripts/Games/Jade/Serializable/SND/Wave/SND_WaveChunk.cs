using BinarySerializer;

namespace Ray1Map.Jade {
	public abstract class SND_WaveChunk : BinarySerializable {
		public SND_WaveChunk_Container Container { get; set; }
		public uint EditorSize { get; protected set; }
	}
}
