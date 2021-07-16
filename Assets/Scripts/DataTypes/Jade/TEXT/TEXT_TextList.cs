using System;
using System.Linq;
using BinarySerializer;

namespace R1Engine.Jade {
	public class TEXT_TextList : Jade_File {
		public bool HasSound { get; set; }

		public uint Count { get; set; }
		public TEXT_OneText[] Text { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Count = s.Serialize<uint>(Count, name: nameof(Count));
			Text = s.SerializeObjectArray<TEXT_OneText>(Text, Count, onPreSerialize: ot => ot.HasSound = HasSound, name: nameof(Text));
			var bufferPointer = s.CurrentPointer;
			foreach(var t in Text) t.SerializeString(s, bufferPointer);
			s.Goto(Offset + FileSize);
		}
	}
}
