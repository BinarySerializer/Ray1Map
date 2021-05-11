using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class LOA_BinFileHeader : BinarySerializable {
		public uint FileSize { get; set; }
		public uint Mark { get; set; } // 0x99C0FFEE
		public Jade_Key Key { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
			if (s.GetR1Settings().Jade_Version >= Jade_Version.Montreal) {
				Mark = s.Serialize<uint>(Mark, name: nameof(Mark));
				Key = s.SerializeObject<Jade_Key>(Key, name: nameof(Key));
			}
		}

		public override bool IsShortLog => true;
		public override string ShortLog {
			get {
				if (Context.GetR1Settings().Jade_Version == Jade_Version.Montreal) {
					return $"BinFileHeader(Size: 0x{FileSize:X}, Mark: {BitHelpers.ExtractBits((int)Mark,8,0):X2}{BitHelpers.ExtractBits((int)Mark, 8, 8):X2}{BitHelpers.ExtractBits((int)Mark, 8, 16):X2}{BitHelpers.ExtractBits((int)Mark, 8, 24):X2}, Key: {Key})";
				} else {
					return $"BinFileHeader(Size: 0x{FileSize:X})";
				}
			}
		}
	}
}
