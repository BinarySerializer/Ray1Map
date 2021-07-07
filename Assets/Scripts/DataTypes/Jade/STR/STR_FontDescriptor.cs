using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class STR_FontDescriptor : Jade_File {
		public string Header { get; set; }
		public int Footer { get; set; }
		public uint MaxCharacter { get; set; }
		public uint Version { get; set; }
		public Character[] Characters { get; set; }
		public CharacterAC[] CharacterACs { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Header = s.SerializeString(Header, 8, encoding: Jade_BaseManager.Encoding, name: nameof(Header));
			if (Header != "FONTDESC") throw new Exception($"Parsing failed: File at {Offset} was not of type {GetType()}");
			s.DoAt(Offset + FileSize - 4, () => {
				Footer = s.Serialize<int>(Footer, name: nameof(Footer));
				if (Footer != -1) throw new Exception($"Parsing failed: File at {Offset} parsed as {GetType()} but had a {nameof(Footer)} of {Footer}");
			});
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_WW)) {
				Version = s.Serialize<uint>(Version, name: nameof(Version));
			} else {
				MaxCharacter = s.Serialize<uint>(MaxCharacter, name: nameof(MaxCharacter));
			}
			//Characters = s.SerializeObjectArray<Character>(Characters, (FileSize - (s.CurrentPointer-Offset) - 4) / 20, name: nameof(Characters));
			Characters = s.SerializeObjectArrayUntil<Character>(Characters,
				conditionCheckFunc: c => (s.CurrentPointer.AbsoluteOffset >= (Offset.AbsoluteOffset + FileSize)) || c.Char == -1,
				name: nameof(Characters));
			if (Version >= 1 && s.CurrentPointer.AbsoluteOffset < (Offset.AbsoluteOffset + FileSize)) {
				CharacterACs = s.SerializeObjectArrayUntil<CharacterAC>(CharacterACs,
				conditionCheckFunc: c => (s.CurrentPointer.AbsoluteOffset >= (Offset.AbsoluteOffset + FileSize)) || c.Char == -1,
				name: nameof(CharacterACs));
			}
		}

		public class Character : BinarySerializable {
			public int Char { get; set; } = -1;
			public float XMin { get; set; } // U1
			public float YMin { get; set; } // V1
			public float XMax { get; set; } // U2
			public float YMax { get; set; } // V2
			public override void SerializeImpl(SerializerObject s) {
				Char = s.Serialize<int>(Char, name: nameof(Char));
				if(Char == -1) return;
				XMin = s.Serialize<float>(XMin, name: nameof(XMin));
				YMin = s.Serialize<float>(YMin, name: nameof(YMin));
				XMax = s.Serialize<float>(XMax, name: nameof(XMax));
				YMax = s.Serialize<float>(YMax, name: nameof(YMax));
			}
		}

		public class CharacterAC : BinarySerializable {
			public int Char { get; set; } = -1;
			public float A { get; set; }
			public float C { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Char = s.Serialize<int>(Char, name: nameof(Char));
				if (Char == -1) return;
				A = s.Serialize<float>(A, name: nameof(A));
				C = s.Serialize<float>(C, name: nameof(C));
			}
		}
	}
}
