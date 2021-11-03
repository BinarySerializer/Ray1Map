using BinarySerializer;
using System.Text;

namespace Ray1Map.Jade {
	public class SND_WaveChunk_Data : SND_WaveChunk {
		public byte[] Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if(!Loader.IsBinaryData) SerializeData(s);

			EditorSize = Container.ChunkDataSize;
		}

		public void SerializeData(SerializerObject s) {
			Data = s.SerializeArray<byte>(Data, Container.ChunkDataSize, name: nameof(Data));
		}
	}
}
