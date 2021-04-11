using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierPhoto : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint UInt_04 { get; set; }
		public uint UInt_08 { get; set; }
		public uint UInt_0C { get; set; }
		public float Float_10 { get; set; }
		public float Float_14 { get; set; }
		public float Float_18 { get; set; }
		public float Float_1C { get; set; }
		public Jade_Vector Vector_20 { get; set; }
		public Jade_Vector Vector_2C { get; set; }
		public float Float_38 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
			UInt_0C = s.Serialize<uint>(UInt_0C, name: nameof(UInt_0C));
			Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
			Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));
			Float_18 = s.Serialize<float>(Float_18, name: nameof(Float_18));
			Float_1C = s.Serialize<float>(Float_1C, name: nameof(Float_1C));
			if (Float_1C == 1.001f) {
				Vector_20 = s.SerializeObject<Jade_Vector>(Vector_20, name: nameof(Vector_20));
				Vector_2C = s.SerializeObject<Jade_Vector>(Vector_2C, name: nameof(Vector_2C));
			}
			Float_38 = s.Serialize<float>(Float_38, name: nameof(Float_38));
		}
	}
}
