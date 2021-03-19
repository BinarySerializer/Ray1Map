using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R1Engine.Jade {
	public class AI_Vars : Jade_File {
		public uint BufferSize { get; set; }
		public Var[] Vars { get; set; }
		public uint NameBufferSize { get; set; }
		public string[] Names { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			BufferSize = s.Serialize<uint>(BufferSize, name: nameof(BufferSize));
			Vars = s.SerializeObjectArray<Var>(Vars, BufferSize / 12, name: nameof(Vars));
			NameBufferSize = s.Serialize<uint>(NameBufferSize, name: nameof(NameBufferSize));
			if (NameBufferSize > 0) {
				Names = s.SerializeStringArray(Names, Vars.Length, 30, name: nameof(Names));
			}
		}

		public class Var : R1Serializable {
			public int Int_00 { get; set; }
			public int Int_04 { get; set; }
			public short Type { get; set; }
			public short Short_0A { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
				Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
				Type = s.Serialize<short>(Type, name: nameof(Type));
				Short_0A = s.Serialize<short>(Short_0A, name: nameof(Short_0A));
			}
		}
	}
}
