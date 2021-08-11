using BinarySerializer;
using System.Text;

namespace R1Engine.Jade {
	public class SND_WaveChunk_Label : SND_WaveChunk {
		public uint CuePointID { get; set; } // Corresponds to a "Name" value in the Cue chunk
		public string Text { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			CuePointID = s.Serialize<uint>(CuePointID, name: nameof(CuePointID));
			Text = s.SerializeString(Text, encoding: Encoding.ASCII, name: nameof(Text));

			EditorSize = 4 + (uint)Text.Length + 1;
		}
	}
}
