using BinarySerializer;

namespace Ray1Map.Jade {
    public class PS2_DMAChainData : BinarySerializable {
		public int ID { get; set; } // Used as address (ADDR = ID << 24)
		public uint DataSize { get; set; }
		public byte[] Bytes { get; set; }

		public bool Pre_IsInstance { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetR1Settings().Platform == Platform.PSP && !Pre_IsInstance) {
				ID = s.Serialize<ushort>((ushort)ID, name: nameof(ID));
			} else {
				ID = s.Serialize<int>(ID, name: nameof(ID));
			}
			DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
			Bytes = s.SerializeArray<byte>(Bytes, DataSize, name: nameof(Bytes));
		}
	}
}