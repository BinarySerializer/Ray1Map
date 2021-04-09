using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class STR_FontDescriptor : Jade_File {
		public string Header { get; set; }
		public int Footer { get; set; }
		public uint MaxCharacter { get; set; }
		public Character[] Characters { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Header = s.SerializeString(Header, 8, encoding: Jade_BaseManager.Encoding, name: nameof(Header));
			if (Header != "FONTDESC") throw new Exception($"Parsing failed: File at {Offset} was not of type {GetType()}");
			s.DoAt(Offset + FileSize - 4, () => {
				Footer = s.Serialize<int>(Footer, name: nameof(Footer));
				if (Footer != -1) throw new Exception($"Parsing failed: File at {Offset} parsed as {GetType()} but had a {nameof(Footer)} of {Footer}");
			});
			MaxCharacter = s.Serialize<uint>(MaxCharacter, name: nameof(MaxCharacter));
			Characters = s.SerializeObjectArray<Character>(Characters, (FileSize - (s.CurrentPointer-Offset) - 4) / 20, name: nameof(Characters));
			/*Characters = s.SerializeObjectArrayUntil<Character>(Characters, c => s.CurrentPointer.AbsoluteOffset >= (Offset.AbsoluteOffset + FileSize - 4),
				includeLastObj: true, name: nameof(Characters));*/
			Footer = s.Serialize<int>(Footer, name: nameof(Footer));
		}

		public class Character : BinarySerializable {
			public uint Char { get; set; }
			public float XMin { get; set; }
			public float YMin { get; set; }
			public float XMax { get; set; }
			public float YMax { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Char = s.Serialize<uint>(Char, name: nameof(Char));
				XMin = s.Serialize<float>(XMin, name: nameof(XMin));
				YMin = s.Serialize<float>(YMin, name: nameof(YMin));
				XMax = s.Serialize<float>(XMax, name: nameof(XMax));
				YMax = s.Serialize<float>(YMax, name: nameof(YMax));
			}
		}
	}
}
