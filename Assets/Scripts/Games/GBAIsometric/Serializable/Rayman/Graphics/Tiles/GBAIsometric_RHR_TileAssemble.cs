using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_RHR_TileAssemble : BinarySerializable
    {
        public Pointer DataPointer { get; set; }
        public Pointer DataOffsetsPointer { get; set; } // Compressed

        // Parsed
        public ushort[] DataOffsets { get; set; }
        public TileData[] Data { get; set; }
        public Compression TileCompression { get; set; }

        public enum Compression {
            RLE1 = 0,
            RLE2 = 1,
            BlockDiff = 2,
            Block = 3,
        }

        public override void SerializeImpl(SerializerObject s)
        {
            DataPointer = s.SerializePointer(DataPointer, name: nameof(DataPointer));
            DataOffsetsPointer = s.SerializePointer(DataOffsetsPointer, name: nameof(DataOffsetsPointer));
            
            s.DoAt(DataOffsetsPointer, () =>
            {
                s.DoEncoded(new RHREncoder(), () => DataOffsets = s.SerializeArray<ushort>(DataOffsets, s.CurrentLength / 2, name: nameof(DataOffsets)));
            });
            if (Data == null) {
                if (DataOffsets != null) {
                    Data = new TileData[DataOffsets.Length];
                    for (int i = 0; i < DataOffsets.Length; i++) {
                        s.DoAt(DataPointer + DataOffsets[i] * 4, () => {
                            Data[i] = s.SerializeObject<TileData>(Data[i], onPreSerialize: td => td.TileCompression = TileCompression, name: $"{nameof(Data)}[{i}]");
                        });
                    }
                } else {
                    Data = new TileData[0];
                }
            }
        }
        public class TileData : BinarySerializable {
            public ushort BaseMapEntry { get; set; }
            public ushort Padding { get; set; }
            public ushort[] TileIndices { get; set; }
            public Compression TileCompression { get; set; }

            public override void SerializeImpl(SerializerObject s) {
                switch (TileCompression) {
                    case Compression.RLE1:
                        s.DoEncoded(new RHR_RLETileEncoder(RHR_RLETileEncoder.RLEMode.RLE1), () => {
                            TileIndices = s.SerializeArray<ushort>(TileIndices, 64, name: nameof(TileIndices));
                        });
                        break;
                    case Compression.RLE2:
                        s.DoEncoded(new RHR_RLETileEncoder(RHR_RLETileEncoder.RLEMode.RLE2), () => {
                            TileIndices = s.SerializeArray<ushort>(TileIndices, 64, name: nameof(TileIndices));
                        });
                        break;
                    case Compression.BlockDiff:
                        BaseMapEntry = s.Serialize<ushort>(BaseMapEntry, name: nameof(BaseMapEntry));
                        Padding = s.Serialize<ushort>(Padding, name: nameof(Padding));
                        s.DoEncoded(new RHREncoder(RHREncoder.EncoderMode.TileData), () => {
                            TileIndices = s.SerializeArray<ushort>(TileIndices, 64, name: nameof(TileIndices));
                        });
                        break;
                    case Compression.Block:
                        s.DoEncoded(new RHREncoder(RHREncoder.EncoderMode.TileData), () => {
                            TileIndices = s.SerializeArray<ushort>(TileIndices, 64, name: nameof(TileIndices));
                        });
                        break;
                }
            }
        }
    }
}