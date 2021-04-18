using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GRID_CompressedGrid : Jade_File {
		public Unknown[] Unknowns { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Pointer endPtr = Offset + FileSize;
			Unknowns = s.SerializeObjectArrayUntil<Unknown>(
				Unknowns,
				u => s.CurrentPointer.AbsoluteOffset >= endPtr.AbsoluteOffset,
				includeLastObj: true,
				name: nameof(Unknowns));
		}

		public class Unknown : BinarySerializable {
			public uint FileSize { get; set; }
			public Entry[] Entries { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
				Pointer endPtr = Offset + FileSize;
				Entries = s.SerializeObjectArrayUntil<Entry>(
					Entries,
					u => s.CurrentPointer.AbsoluteOffset >= endPtr.AbsoluteOffset,
					includeLastObj: true,
					name: nameof(Entries));
			}

			public class Entry : BinarySerializable {
				public byte Byte_00 { get; set; }
				public byte Byte_80_0 { get; set; }
				public byte Byte_80_1 { get; set; }
				public short Short_40_0 { get; set; }
				public byte Byte_40_1 { get; set; }
				public override void SerializeImpl(SerializerObject s) {
					Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
					switch (Byte_00) {
						case 0x40:
							Short_40_0 = s.Serialize<short>(Short_40_0, name: nameof(Short_40_0));
							Byte_40_1 = s.Serialize<byte>(Byte_40_1, name: nameof(Byte_40_1));
							break;
						case 0x80:
							Byte_80_0 = s.Serialize<byte>(Byte_80_0, name: nameof(Byte_80_0));
							Byte_80_1 = s.Serialize<byte>(Byte_80_1, name: nameof(Byte_80_1));
							break;
					}
				}
			}
		}
	}
}
