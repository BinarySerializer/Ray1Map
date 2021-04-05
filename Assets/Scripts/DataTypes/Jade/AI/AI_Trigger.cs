using BinarySerializer;
using System;

namespace R1Engine.Jade {
	public class AI_Trigger : BinarySerializable {
		public int Int_00 { get; set; }
		public int Int_01 { get; set; }
		public byte[] Bytes_02 { get; set; }
		public uint UInt_03 { get; set; }
		public uint UInt_04 { get; set; }
		public uint UInt_05 { get; set; }
		public uint UInt_06 { get; set; }
		public uint UInt_07 { get; set; }
		public uint UInt_08 { get; set; }
		public Jade_Vector Vector_09 { get; set; }
		public Jade_Vector Vector_10 { get; set; }
		public Jade_Vector Vector_11 { get; set; }
		public Jade_Vector Vector_12 { get; set; }
		public Jade_Vector Vector_13 { get; set; }
		public int Int_14 { get; set; }
		public int Int_15 { get; set; }
		public int Int_16 { get; set; }
		public int Int_17 { get; set; }
		public int Int_18 { get; set; }
		public int Int_19 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
			Int_01 = s.Serialize<int>(Int_01, name: nameof(Int_01));
			Bytes_02 = s.SerializeArray<byte>(Bytes_02, 64, name: nameof(Bytes_02));
			UInt_03 = s.Serialize<uint>(UInt_03, name: nameof(UInt_03));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			UInt_05 = s.Serialize<uint>(UInt_05, name: nameof(UInt_05));
			UInt_06 = s.Serialize<uint>(UInt_06, name: nameof(UInt_06));
			UInt_07 = s.Serialize<uint>(UInt_07, name: nameof(UInt_07));
			UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
			Vector_09 = s.SerializeObject<Jade_Vector>(Vector_09, name: nameof(Vector_09));
			Vector_10 = s.SerializeObject<Jade_Vector>(Vector_10, name: nameof(Vector_10));
			Vector_11 = s.SerializeObject<Jade_Vector>(Vector_11, name: nameof(Vector_11));
			Vector_12 = s.SerializeObject<Jade_Vector>(Vector_12, name: nameof(Vector_12));
			Vector_13 = s.SerializeObject<Jade_Vector>(Vector_13, name: nameof(Vector_13));
			Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));
			Int_15 = s.Serialize<int>(Int_15, name: nameof(Int_15));
			Int_16 = s.Serialize<int>(Int_16, name: nameof(Int_16));
			Int_17 = s.Serialize<int>(Int_17, name: nameof(Int_17));
			Int_18 = s.Serialize<int>(Int_18, name: nameof(Int_18));
			Int_19 = s.Serialize<int>(Int_19, name: nameof(Int_19));
		}
	}
}
