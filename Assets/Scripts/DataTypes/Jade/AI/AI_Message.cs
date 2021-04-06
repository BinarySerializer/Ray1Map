using BinarySerializer;
using System;

namespace R1Engine.Jade {
	public class AI_Message : BinarySerializable {
		public uint UInt_00 { get; set; }
		public uint UInt_01 { get; set; }
		public uint UInt_02 { get; set; }
		public uint UInt_03 { get; set; }
		public uint UInt_04 { get; set; }
		public uint UInt_05 { get; set; }
		public Jade_Vector Vector_06 { get; set; }
		public Jade_Vector Vector_07 { get; set; }
		public Jade_Vector Vector_08 { get; set; }
		public Jade_Vector Vector_09 { get; set; }
		public Jade_Vector Vector_10 { get; set; }
		public int Int_11 { get; set; }
		public int Int_12 { get; set; }
		public int Int_13 { get; set; }
		public int Int_14 { get; set; }
		public int Int_15 { get; set; }
		public int Int_16 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			UInt_01 = s.Serialize<uint>(UInt_01, name: nameof(UInt_01));
			UInt_02 = s.Serialize<uint>(UInt_02, name: nameof(UInt_02));
			UInt_03 = s.Serialize<uint>(UInt_03, name: nameof(UInt_03));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			UInt_05 = s.Serialize<uint>(UInt_05, name: nameof(UInt_05));
			Vector_06 = s.SerializeObject<Jade_Vector>(Vector_06, name: nameof(Vector_06));
			Vector_07 = s.SerializeObject<Jade_Vector>(Vector_07, name: nameof(Vector_07));
			Vector_08 = s.SerializeObject<Jade_Vector>(Vector_08, name: nameof(Vector_08));
			Vector_09 = s.SerializeObject<Jade_Vector>(Vector_09, name: nameof(Vector_09));
			Vector_10 = s.SerializeObject<Jade_Vector>(Vector_10, name: nameof(Vector_10));
			Int_11 = s.Serialize<int>(Int_11, name: nameof(Int_11));
			Int_12 = s.Serialize<int>(Int_12, name: nameof(Int_12));
			Int_13 = s.Serialize<int>(Int_13, name: nameof(Int_13));
			Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));
			Int_15 = s.Serialize<int>(Int_15, name: nameof(Int_15));
			Int_16 = s.Serialize<int>(Int_16, name: nameof(Int_16));
		}
	}
}
