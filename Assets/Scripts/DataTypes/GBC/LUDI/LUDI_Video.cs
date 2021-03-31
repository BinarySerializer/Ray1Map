using BinarySerializer;

namespace R1Engine
{
    public class LUDI_Video : LUDI_BaseBlock {
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint Speed { get; set; }
        public uint FrameCount { get; set; }
        public uint[] FrameOffsets { get; set; }
        public BGR565Color[][] FrameDataPPC { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            Width = s.Serialize<uint>(Width, name: nameof(Width));
            Height = s.Serialize<uint>(Height, name: nameof(Height));
            Speed = s.Serialize<uint>(Speed, name: nameof(Speed));
            FrameCount = s.Serialize<uint>(FrameCount, name: nameof(FrameCount));
            FrameOffsets = s.SerializeArray<uint>(FrameOffsets, FrameCount, name: nameof(FrameOffsets));
            if (s.GetR1Settings().EngineVersion == EngineVersion.GBC_R1_PocketPC) {
                if (FrameDataPPC == null) FrameDataPPC = new BGR565Color[FrameCount][];
                for (int i = 0; i < FrameDataPPC.Length; i++) {
                    uint decompressedSize = Width * Height * 2;
                    uint nextOff = i < FrameDataPPC.Length - 1 ? FrameOffsets[i + 1] : BlockSize;
                    uint compressedSize = nextOff - FrameOffsets[i];
                    s.DoAt(BlockStartPointer + FrameOffsets[i], () => {
                        s.DoEncoded(new Lzo1xEncoder(compressedSize, decompressedSize), () => {
                            FrameDataPPC[i] = s.SerializeObjectArray<BGR565Color>(FrameDataPPC[i], Width * Height, name: $"{nameof(FrameDataPPC)}[{i}]");
                        });
                    });
                }
            }
        }
    }
}