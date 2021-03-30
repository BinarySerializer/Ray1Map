using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public class PROTEX_Modifier : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint UInt_04 { get; set; }
		public uint Bytes0Count { get; set; }
		public byte[] Bytes0 { get; set; }
		public uint Bytes1Count { get; set; }
		public byte[] Bytes1 { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			if (UInt_04 != 0) {
				Bytes0Count = s.Serialize<uint>(Bytes0Count, name: nameof(Bytes0Count));
				Bytes0 = s.SerializeArray<byte>(Bytes0, Bytes0Count, name: nameof(Bytes0));
				Bytes1Count = s.Serialize<uint>(Bytes1Count, name: nameof(Bytes1Count));
				Bytes1 = s.SerializeArray<byte>(Bytes1, Bytes1Count, name: nameof(Bytes1));
			}

		}
	}
}
