using BinarySerializer;
using System.Text;

namespace Ray1Map.Jade {
	public class SND_WaveChunk_LabeledText : SND_WaveChunk {
		public uint CuePointID { get; set; }
		public uint SampleLength { get; set; }
		public string PurposeID { get; set; }
		public ushort Country { get; set; }
		public ushort Language { get; set; }
		public ushort Dialect { get; set; }
		public ushort CodePage { get; set; }
		public string Text { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			CuePointID = s.Serialize<uint>(CuePointID, name: nameof(CuePointID));
			SampleLength = s.Serialize<uint>(SampleLength, name: nameof(SampleLength));
			PurposeID = s.SerializeString(PurposeID, 4, encoding: Encoding.ASCII, name: nameof(PurposeID));
			Country = s.Serialize<ushort>(Country, name: nameof(Country));
			Language = s.Serialize<ushort>(Language, name: nameof(Language));
			Dialect = s.Serialize<ushort>(Dialect, name: nameof(Dialect));
			CodePage = s.Serialize<ushort>(CodePage, name: nameof(CodePage));
			EditorSize = 20;
			if (Container.ChunkDataSize > 20) {
				Text = s.SerializeString(Text, encoding: Encoding.ASCII, name: nameof(Text));
				EditorSize += (uint)Text.Length + 1;
			}
		}
	}
}
