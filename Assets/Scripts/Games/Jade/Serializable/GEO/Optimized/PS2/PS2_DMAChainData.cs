using BinarySerializer;

namespace Ray1Map.Jade {
    public class PS2_DMAChainData : BinarySerializable {
		public int DMAChainDataID { get; set; } // Used as address (ADDR = ID << 24)
		public uint DataSize { get; set; }
		public byte[] Bytes { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			DMAChainDataID = s.Serialize<int>(DMAChainDataID, name: nameof(DMAChainDataID));
			DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
			Bytes = s.SerializeArray<byte>(Bytes, DataSize, name: nameof(Bytes));
		}
	}
}