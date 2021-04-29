using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.DWARF1 {
	public class DWARF_Segment : BinarySerializable {
		public DWARF_Struct[] Files { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Files = s.SerializeObjectArrayUntil<DWARF_Struct>(Files,
				f => f.NextStructPointer.AbsoluteOffset >= Offset.AbsoluteOffset + s.CurrentLength,
				includeLastObj: true, name: nameof(Files));
		}
	}
}
