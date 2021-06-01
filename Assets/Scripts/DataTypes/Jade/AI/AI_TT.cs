using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.Jade {
	public class AI_TT : Jade_File {
		public uint UInt_00 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if(Loader.IsBinaryData)
				UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
		}
	}
}
