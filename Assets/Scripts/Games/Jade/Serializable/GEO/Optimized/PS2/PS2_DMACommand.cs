using BinarySerializer;
using BinarySerializer.PS2;

namespace Ray1Map.Jade {
    public class PS2_DMACommand : BinarySerializable {
        public Chain_DMAtag DMATag { get; set; }
        public byte[] DataToTransfer { get; set; }

        public override void SerializeImpl(SerializerObject s) {
			DMATag = s.SerializeObject<Chain_DMAtag>(DMATag, name: nameof(DMATag));
			DataToTransfer = s.SerializeArray<byte>(DataToTransfer, 8, name: nameof(DataToTransfer));
		}
    }
}