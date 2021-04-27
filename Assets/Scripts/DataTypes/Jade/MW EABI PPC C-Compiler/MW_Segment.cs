using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.MW {
	public class MW_Segment : BinarySerializable {
		public MW_Struct[] Files { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Files = s.SerializeObjectArrayUntil<MW_Struct>(Files,
				f => f.NextStructPointer.AbsoluteOffset >= Offset.AbsoluteOffset + s.CurrentLength,
				includeLastObj: true, name: nameof(Files));
		}
	}
}
