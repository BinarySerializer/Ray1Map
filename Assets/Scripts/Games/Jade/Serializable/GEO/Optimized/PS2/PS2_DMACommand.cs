using BinarySerializer;
using BinarySerializer.PS2;
using System.Collections.Generic;
using System.Linq;

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

        public void Transfer(Writer w, List<PS2_DMAChainData> chainData) {
            w.Write(DataToTransfer);
            w.Write(ExtraDataToTransfer);
            switch (DMATag.ID) {
                case Chain_DMAtag.TagID.REF:
                    uint addr = DMATag.ADDR;
                    int id = BitHelpers.ExtractBits((int)addr, 8, 24);
                    int offset = BitHelpers.ExtractBits((int)addr, 24, 0);
                    var data = chainData.FirstOrDefault(d => d.ID == id);
                    if(data == null)
                        throw new BinarySerializableException(this, $"No suitable data block found for REF DMATag with ID {id}");
                    w.Write(data.Bytes, offset, DMATag.QWC * 16);
                    break;
                case Chain_DMAtag.TagID.RET:
                    break;
                default:
                    throw new BinarySerializableException(this, $"Untreated DMAtag {DMATag.ID}");
            }
        }
    }
}