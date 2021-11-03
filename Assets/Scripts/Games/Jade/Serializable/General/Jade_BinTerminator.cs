using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class Jade_BinTerminator : Jade_File {
		public Jade_Code Code { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			if (FileSize > 0) Code = s.Serialize<Jade_Code>(Code, name: nameof(Code));
		}
	}
}
