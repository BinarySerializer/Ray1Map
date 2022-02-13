using BinarySerializer;

namespace Ray1Map.Jade {
	public class GEO_GaoVisu_PC : BinarySerializable {
		public uint UInt_00 { get; set; }
		public uint Count_04 { get; set; }
		public byte[] Bytes_08 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			if (UInt_00 != 0) {
				Count_04 = s.Serialize<uint>(Count_04, name: nameof(Count_04));
				Bytes_08 = s.SerializeArray<byte>(Bytes_08, Count_04, name: nameof(Bytes_08));
			}
		}
	}
}
