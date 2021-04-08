using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject_ExtendedXenonData : BinarySerializable {
		public uint UInt_00 { get; set; }
		public uint UInt_04 { get; set; }
		public Jade_Vector Vector_08 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			Vector_08 = s.SerializeObject<Jade_Vector>(Vector_08, name: nameof(Vector_08));
		}
	}
}
