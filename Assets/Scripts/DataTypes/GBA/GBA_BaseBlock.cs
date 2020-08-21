namespace R1Engine
{
    /// <summary>
    /// A base GBA block
    /// </summary>
    public abstract class GBA_BaseBlock : R1Serializable
    {
        /// <summary>
        /// The size of the block in bytes, not counting this value
        /// </summary>
        public uint BlockSize { get; set; }

        public uint? CompressedBlockSize { get; set; }
        public uint? DecompressedBlockSize { get; set; }
        public Pointer DecompressedBlockOffset { get; set; }

        /// <summary>
        /// The block offset table
        /// </summary>
        public GBA_OffsetTable OffsetTable { get; set; }

        public bool IsBlockCompressed { get; set; }

		public override void SerializeImpl(SerializerObject s) {
            SerializeBlockSize(s);
            if (IsBlockCompressed) {
                DecompressedBlockSize = s.Serialize<uint>(DecompressedBlockSize ?? 0, name: nameof(DecompressedBlockSize));
                s.DoEncoded(new ZlibEncoder(CompressedBlockSize.Value, DecompressedBlockSize.Value), () => {
                    BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));
                    DecompressedBlockOffset = s.CurrentPointer;
                    SerializeOffsetTable(s);
                    SerializeBlock(s);
                    CheckBlockSize(s);
                    // Serialize data from the offset table
                    SerializeOffsetData(s);
                    s.Goto(s.CurrentPointer.file.StartPointer + s.CurrentLength); // no warning
                });
            } else {
                SerializeOffsetTable(s);
                SerializeBlock(s);
                CheckBlockSize(s);
                // Serialize data from the offset table
                SerializeOffsetData(s);
            }
        }

        private void CheckBlockSize(SerializerObject s) {
            if (IsBlockCompressed) {
                if (DecompressedBlockOffset + BlockSize != s.CurrentPointer) {
                    UnityEngine.Debug.LogWarning($"{GetType()} @ {Offset}: Serialized size: {(s.CurrentPointer - DecompressedBlockOffset - 4)} != BlockSize: {BlockSize}");
                }
            } else {
                if (Offset + BlockSize != s.CurrentPointer) {
                    UnityEngine.Debug.LogWarning($"{GetType()} @ {Offset}: Serialized size: {(s.CurrentPointer - Offset)} != BlockSize: {BlockSize}");
                }
            }
        }
        private void SerializeBlockSize(SerializerObject s) {
            s.DoAt(Offset - 4, () => {
                // Serialize the size
                BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));
                if (s.GameSettings.EngineVersion == EngineVersion.GBA_SplinterCell_NGage) {
                    if (BitHelpers.ExtractBits((int)BlockSize, 1, 31) == 1) {
                        IsBlockCompressed = true;
                        CompressedBlockSize = (uint)BitHelpers.ExtractBits((int)BlockSize, 31, 0);
                    }
                }
            });
        }
        private void SerializeOffsetTable(SerializerObject s) {
            s.DoAt(s.CurrentPointer + BlockSize, () => {
                // Align
                s.Align();
                // Serialize the offset table
                OffsetTable = s.SerializeObject<GBA_OffsetTable>(OffsetTable, onPreSerialize: ot => ot.Block = this, name: nameof(OffsetTable));
            });
        }

        public abstract void SerializeBlock(SerializerObject s);
        public virtual void SerializeOffsetData(SerializerObject s) { }
    }
}