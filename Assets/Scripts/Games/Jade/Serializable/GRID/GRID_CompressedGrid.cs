using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class GRID_CompressedGrid : Jade_File {
		public Group[] Groups { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Pointer endPtr = Offset + FileSize;
			Groups = s.SerializeObjectArrayUntil<Group>(
				Groups,
				u => s.CurrentPointer.AbsoluteOffset >= endPtr.AbsoluteOffset,
				name: nameof(Groups));
		}

		public class Group : BinarySerializable {
			public uint GroupSize { get; set; }
			public Entry[] Entries { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				GroupSize = s.Serialize<uint>(GroupSize, name: nameof(GroupSize));
				Pointer endGroup = Offset + GroupSize;
				Entries = s.SerializeObjectArrayUntil<Entry>(
					Entries,
					u => s.CurrentPointer.AbsoluteOffset >= endGroup.AbsoluteOffset,
					name: nameof(Entries));
			}

			public class Entry : BinarySerializable {
				public byte Mask { get; set; }
				public byte Byte_80_0 { get; set; }
				public byte Byte_80_1 { get; set; }
				public short Short_40_0 { get; set; }
				public byte Byte_40_1 { get; set; }
				public override void SerializeImpl(SerializerObject s) {
					Mask = s.Serialize<byte>(Mask, name: nameof(Mask));
					switch (Mask) {
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
