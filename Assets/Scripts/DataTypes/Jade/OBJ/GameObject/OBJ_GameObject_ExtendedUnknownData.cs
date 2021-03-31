using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject_ExtendedUnknownData : BinarySerializable {
		public uint UInt_00 { get; set; }
		public uint UInt_04 { get; set; }
		public uint UInt_08 { get; set; }
		public uint UInt_0C { get; set; }
		public float Float_10 { get; set; }
		public float Float_14 { get; set; }
		public Jade_Vector Vector_18 { get; set; }
		public Jade_Vector Vector_24 { get; set; }
		public uint UInt_30 { get; set; }
		public uint UInt_34 { get; set; }
		public uint UInt_38 { get; set; }
		public uint UInt_3C { get; set; }
		public uint UInt_40 { get; set; }
		public uint UInt_44 { get; set; }
		public uint UInt_48 { get; set; }
		public uint UInt_4C { get; set; }
		public uint UInt_50 { get; set; }
		public uint UInt_54 { get; set; }
		public uint UInt_58 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
			UInt_0C = s.Serialize<uint>(UInt_0C, name: nameof(UInt_0C));
			Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
			Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));
			Vector_18 = s.SerializeObject<Jade_Vector>(Vector_18, name: nameof(Vector_18));
			Vector_24 = s.SerializeObject<Jade_Vector>(Vector_24, name: nameof(Vector_24));
			UInt_30 = s.Serialize<uint>(UInt_30, name: nameof(UInt_30));
			UInt_34 = s.Serialize<uint>(UInt_34, name: nameof(UInt_34));
			UInt_38 = s.Serialize<uint>(UInt_38, name: nameof(UInt_38));
			UInt_3C = s.Serialize<uint>(UInt_3C, name: nameof(UInt_3C));
			UInt_40 = s.Serialize<uint>(UInt_40, name: nameof(UInt_40));
			UInt_44 = s.Serialize<uint>(UInt_44, name: nameof(UInt_44));
			UInt_48 = s.Serialize<uint>(UInt_48, name: nameof(UInt_48));
			UInt_4C = s.Serialize<uint>(UInt_4C, name: nameof(UInt_4C));
			UInt_50 = s.Serialize<uint>(UInt_50, name: nameof(UInt_50));
			UInt_54 = s.Serialize<uint>(UInt_54, name: nameof(UInt_54));
			UInt_58 = s.Serialize<uint>(UInt_58, name: nameof(UInt_58));
		}
	}
}
