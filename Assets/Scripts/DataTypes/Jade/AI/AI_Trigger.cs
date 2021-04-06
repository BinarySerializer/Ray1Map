using BinarySerializer;
using System;

namespace R1Engine.Jade {
	public class AI_Trigger : BinarySerializable {
		public int Int_00 { get; set; }
		public int Int_01 { get; set; }
		public byte[] Bytes_02 { get; set; }
		public AI_Message Message { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
			Int_01 = s.Serialize<int>(Int_01, name: nameof(Int_01));
			Bytes_02 = s.SerializeArray<byte>(Bytes_02, 64, name: nameof(Bytes_02));
			Message = s.SerializeObject<AI_Message>(Message, name: nameof(Message));
		}
	}
}
