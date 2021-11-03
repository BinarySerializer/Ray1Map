using System;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class TEXT_TextList : Jade_File {
		public override string Export_Extension => "txl";
		public bool HasSound { get; set; }
		public TEXT_TextList TXLNoSound { get; set; }

		public uint Count { get; set; }
		public TEXT_OneText[] Text { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			if (Loader.IsBinaryData && HasSound && TXLNoSound != null) {
				TXLNoSound.SerializeSoundData(s);
				//HasSound = TXLNoSound.HasSound;
				Count = TXLNoSound.Count;
				Text = TXLNoSound.Text;
			} else {
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				Text = s.SerializeObjectArray<TEXT_OneText>(Text, Count, onPreSerialize: ot => ot.HasSound = HasSound, name: nameof(Text));
				var bufferPointer = s.CurrentPointer;
				foreach (var t in Text) t.SerializeString(s, bufferPointer);
				s.Goto(Offset + FileSize);
			}
		}

		public void SerializeSoundData(SerializerObject s) {
			foreach (var t in Text) {
				t.HasSound = true;
				t.SerializeImpl(s);
				t.HasSound = false;
			}
		}
		public void ResolveSounds(SerializerObject s) {
			foreach (var t in Text) {
				t.ResolveSound(s);
			}
		}
	}
}
