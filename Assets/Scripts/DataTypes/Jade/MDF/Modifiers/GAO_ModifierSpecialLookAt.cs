using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierSpecialLookAt : MDF_Modifier {
		public uint Type { get; set; }
		public byte Byte_04 { get; set; }
		public byte Byte_05 { get; set; }
		public byte Byte_06 { get; set; }
		public byte Byte_07 { get; set; }
		public float Float_08 { get; set; }
		public int Int_0C_Type28 { get; set; } = -1;
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
		public float Float_10 { get; set; } = 1000f;
		public float Float_14 { get; set; } = 1000f;

		public override void SerializeImpl(SerializerObject s) {
			Type = s.Serialize<uint>(Type, name: nameof(Type));
			Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
			Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
			Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));
			Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));
			Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
			if(Type == 28) Int_0C_Type28 = s.Serialize<int>(Int_0C_Type28, name: nameof(Int_0C_Type28));
			GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject));
			if(Int_0C_Type28 == -1) GameObject?.Resolve();
			if (Type != 4) {
				Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
				Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));
			}
		}
	}
}
