using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.XMW {
	public class XMW_Segment : BinarySerializable {
		public XMW_Struct[] Files { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Files = s.SerializeObjectArrayUntil<XMW_Struct>(Files,
				f => f.NextStructPointer.AbsoluteOffset >= Offset.AbsoluteOffset + s.CurrentLength,
				includeLastObj: true, name: nameof(Files));
		}
	}
}
