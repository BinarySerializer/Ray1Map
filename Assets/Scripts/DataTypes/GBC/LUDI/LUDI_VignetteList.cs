namespace R1Engine
{
    public class LUDI_VignetteList : LUDI_BaseBlock {
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint UInt_08 { get; set; }
        public uint VignetteCount { get; set; }
        public uint[] VignetteOffsets { get; set; }
        public BGR565Color[][] VignetteDataPPC { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            Width = s.Serialize<uint>(Width, name: nameof(Width));
            Height = s.Serialize<uint>(Height, name: nameof(Height));
            UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
            VignetteCount = s.Serialize<uint>(VignetteCount, name: nameof(VignetteCount));
            VignetteOffsets = s.SerializeArray<uint>(VignetteOffsets, VignetteCount, name: nameof(VignetteOffsets));
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1_PocketPC) {
                if (VignetteDataPPC == null) VignetteDataPPC = new BGR565Color[VignetteCount][];
                for (int i = 0; i < VignetteDataPPC.Length; i++) {
                    uint decompressedSize = Width * Height * 2;
                    uint nextOff = i < VignetteDataPPC.Length - 1 ? VignetteOffsets[i + 1] : BlockSize;
                    uint compressedSize = nextOff - VignetteOffsets[i];
                    s.DoAt(BlockStartPointer + VignetteOffsets[i], () => {
                        s.DoEncoded(new Lzo1xEncoder(compressedSize, decompressedSize), () => {
                            VignetteDataPPC[i] = s.SerializeObjectArray<BGR565Color>(VignetteDataPPC[i], Width * Height, name: $"{nameof(VignetteDataPPC)}[{i}]");
                        });
                    });
                }
            }
        }
    }
}