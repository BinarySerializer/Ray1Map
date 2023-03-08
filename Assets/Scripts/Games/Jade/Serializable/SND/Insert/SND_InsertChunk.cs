using BinarySerializer;

namespace Ray1Map.Jade {
	public abstract class SND_InsertChunk : BinarySerializable {
		public SND_InsertChunk_Container Container { get; set; }
	}
}
