using BinarySerializer;
using BinarySerializer.PS2;

namespace Ray1Map.Jade {
    public class PS2_DMACommand : BinarySerializable {
        public Chain_DMAtag DMATag { get; set; }
        public byte[] DataToTransfer { get; set; }
        public byte[] ExtraDataToTransfer { get; set; }

        public override void SerializeImpl(SerializerObject s) {
			DMATag = s.SerializeObject<Chain_DMAtag>(DMATag, name: nameof(DMATag));
			DataToTransfer = s.SerializeArray<byte>(DataToTransfer, 8, name: nameof(DataToTransfer));
            switch (DMATag.ID) {
                case Chain_DMAtag.TagID.REFE_CNTS:
                case Chain_DMAtag.TagID.REF:
                case Chain_DMAtag.TagID.REFS:
                    break;
                default:
                    ExtraDataToTransfer = s.SerializeArray<byte>(ExtraDataToTransfer, 16 * DMATag.QWC, name: nameof(ExtraDataToTransfer));
                    break;

            }
		}
    }
}