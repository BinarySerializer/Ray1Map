using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public class OBJ_GameObject_VisualUnknownData : R1Serializable {
		public byte Unk_Type { get; set; }
		public bool Unk_Bool { get; set; }
		public byte Unk_Byte_02 { get; set; }
		public byte Unk_Byte_03 { get; set; }
		public Jade_Vector Unk_Type2_Vector { get; set; }
		public Jade_Matrix Unk_Type7_Matrix { get; set; }
		public float Unk_Type6_Float_0 { get; set; }
		public float Unk_Type6_Float_1 { get; set; }
		public float Unk_Float_00 { get; set; }
		public float Unk_Bool_Float_0 { get; set; }
		public float Unk_Bool_Float_1 { get; set; }
		public float Unk_Bool_Float_2 { get; set; }
		public uint Unk_Type4_UInt { get; set; }
		public float[] Unk_Type4_Floats { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Unk_Type = s.Serialize<byte>(Unk_Type, name: nameof(Unk_Type));
			Unk_Bool = s.Serialize<bool>(Unk_Bool, name: nameof(Unk_Bool));
			Unk_Byte_02 = s.Serialize<byte>(Unk_Byte_02, name: nameof(Unk_Byte_02));
			Unk_Byte_03 = s.Serialize<byte>(Unk_Byte_03, name: nameof(Unk_Byte_03));
			if (Unk_Type >= 2)
				Unk_Type2_Vector = s.SerializeObject<Jade_Vector>(Unk_Type2_Vector, name: nameof(Unk_Type2_Vector));
			if (Unk_Type >= 7)
				Unk_Type7_Matrix = s.SerializeObject<Jade_Matrix>(Unk_Type7_Matrix, name: nameof(Unk_Type7_Matrix));
			if (Unk_Type >= 6) {
				Unk_Type6_Float_0 = s.Serialize<float>(Unk_Type6_Float_0, name: nameof(Unk_Type6_Float_0));
				Unk_Type6_Float_1 = s.Serialize<float>(Unk_Type6_Float_1, name: nameof(Unk_Type6_Float_1));
			}
			Unk_Float_00 = s.Serialize<float>(Unk_Float_00, name: nameof(Unk_Float_00));
			if (Unk_Bool) {
				Unk_Bool_Float_0 = s.Serialize<float>(Unk_Bool_Float_0, name: nameof(Unk_Bool_Float_0));
				Unk_Bool_Float_1 = s.Serialize<float>(Unk_Bool_Float_1, name: nameof(Unk_Bool_Float_1));
				Unk_Bool_Float_2 = s.Serialize<float>(Unk_Bool_Float_2, name: nameof(Unk_Bool_Float_2));
			}
			if (Unk_Type >= 4) {
				Unk_Type4_UInt = s.Serialize<uint>(Unk_Type4_UInt, name: nameof(Unk_Type4_UInt));
				Unk_Type4_Floats = s.SerializeArray<float>(Unk_Type4_Floats, 10, name: nameof(Unk_Type4_Floats));
			}
		}
	}
}
