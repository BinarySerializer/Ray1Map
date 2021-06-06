using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class Jade_BinTerminator : Jade_File {
		public Jade_Code Code { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (FileSize > 0) Code = s.Serialize<Jade_Code>(Code, name: nameof(Code));
		}
	}
}
