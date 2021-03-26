using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R1Engine.Jade {
	public class AI_ProcList : Jade_File {
		public ushort UShort_00 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UShort_00 = s.Serialize<ushort>(UShort_00, name: nameof(UShort_00));
			throw new NotImplementedException();
		}
	}
}
