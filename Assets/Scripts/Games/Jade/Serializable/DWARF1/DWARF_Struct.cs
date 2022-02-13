using BinarySerializer;

namespace Ray1Map.DWARF1 {
	public class DWARF_Struct : BinarySerializable {
		public uint StructSize { get; set; }

		public bool HasHeader { get; set; } = true;
		public Type StructType { get; set; }
		public ushort Header_UShort_02 { get; set; }
		public Pointer NextStructPointer { get; set; }
		public Pointer ChildStructsPointer => Offset + StructSize;
		public bool IsEndOfArray => StructSize < 12;

		public DWARF_Command[] Commands { get; set; }

		public DWARF_Struct[] Children { get; set; }


		protected override void OnPreSerialize(SerializerObject s) {
			base.OnPreSerialize(s);
			SerializeHeader(s);
		}

		protected void SerializeHeader(SerializerObject s) {
			StructSize = s.Serialize<uint>(StructSize, name: nameof(StructSize));
			if (HasHeader && StructSize >= 12) {
				StructType = s.Serialize<Type>(StructType, name: nameof(StructType));
				Header_UShort_02 = s.Serialize<ushort>(Header_UShort_02, name: nameof(Header_UShort_02));
				NextStructPointer = s.SerializePointer(NextStructPointer, name: nameof(NextStructPointer));
			} else {
				NextStructPointer = ChildStructsPointer;
			}
		}

		public override void SerializeImpl(SerializerObject s) {
			if (IsEndOfArray) return;

			Commands = s.SerializeObjectArrayUntil<DWARF_Command>(Commands,
				x => s.CurrentPointer.AbsoluteOffset >= ChildStructsPointer.AbsoluteOffset, name: nameof(Commands));

			if (!IsEndOfArray && NextStructPointer != ChildStructsPointer) {
				s.DoAt(ChildStructsPointer, () => {
					Children = s.SerializeObjectArrayUntil<DWARF_Struct>(Children, f => f.IsEndOfArray, name: nameof(Children));
				});
			}
		}

		protected override void OnPostSerialize(SerializerObject s) {
			base.OnPostSerialize(s);
			CheckFileSize(s);
		}

		public void CheckFileSize(SerializerObject s) {
			long readSize = s.CurrentPointer - Offset;
			if (StructSize != readSize) {
				s.LogWarning($"XMW Struct @ {Offset} with type {GetType()} was not fully serialized: Struct Size: {StructSize:X8} / Serialized: {readSize:X8}");
			}
			s.Goto(NextStructPointer);
		}

		public enum Type : ushort {
			Unk05 = 0x05,
			StructMember = 0x0D,
			File = 0x11,
			StructDefinition = 0x13,
			Unk15 = 0x15
		}
	}
}
