﻿using BinarySerializer;

namespace Ray1Map.GBA
{
    /// <summary>
    /// A base GBA block
    /// </summary>
    public abstract class GBA_BaseBlock : BinarySerializable
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
        public bool IsGCNBlock { get; set; }

        public GBA_ShanghaiLocalOffsetTable ShanghaiOffsetTable { get; set; }

		public override void SerializeImpl(SerializerObject s) {
            SerializeBlockSize(s);
            if (IsBlockCompressed) {
                DecompressedBlockSize = s.Serialize<uint>(DecompressedBlockSize ?? 0, name: nameof(DecompressedBlockSize));
                s.DoEncoded(new ZlibEncoder(CompressedBlockSize.Value, DecompressedBlockSize.Value), () => {
                    BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));
                    DecompressedBlockOffset = s.CurrentPointer;
                    SerializeOffsetTable(s);
                    SerializeBlock(s);
                    if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_SplinterCell_NGage) {
                        s.Align();
                    }
                    CheckBlockSize(s);
                    // Serialize data from the offset table
                    SerializeOffsetData(s);
                    s.Goto(s.CurrentPointer.File.StartPointer + s.CurrentLength); // no warning
                });
            } else 
            {
                // GCN blocks don't have offset tables before the block
                if (!IsGCNBlock) 
                    SerializeOffsetTable(s);

                // The Shanghai and Milan branches have a local offset table within each block
                if (s.GetR1Settings().GBA_IsShanghai || s.GetR1Settings().GBA_IsMilan)
                    ShanghaiOffsetTable = s.SerializeObject<GBA_ShanghaiLocalOffsetTable>(ShanghaiOffsetTable, x => x.Length = GetShanghaiOffsetTableLength, name: nameof(ShanghaiOffsetTable));

                // Serialize the block
                SerializeBlock(s);

                // Align for GCN and Splinter Cell N-Gage
                if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_SplinterCell_NGage || IsGCNBlock)
                    s.Align();

                // Verify that we serialized the entire block
                CheckBlockSize(s);

                // Serialize GCN offset table, located after the block data
                if(IsGCNBlock)
                    SerializeOffsetTable(s);

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
            if (IsGCNBlock) {
                // Serialize the size
                BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));
            } else {
                s.DoAt(Offset - 4, () => {
                    // Serialize the size
                    BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));
                    if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_SplinterCell_NGage) {
                        if (BitHelpers.ExtractBits((int)BlockSize, 1, 31) == 1) {
                            IsBlockCompressed = true;
                            CompressedBlockSize = (uint)BitHelpers.ExtractBits((int)BlockSize, 31, 0);
                        }
                    }
                });
            }
        }
        private void SerializeOffsetTable(SerializerObject s) {
            s.DoAt((IsGCNBlock ? Offset : s.CurrentPointer) + BlockSize, () => {
                // Align
                s.Align();
                // Serialize the offset table
                OffsetTable = s.SerializeObject<GBA_OffsetTable>(OffsetTable, onPreSerialize: ot => ot.Block = this, name: nameof(OffsetTable));
            });
        }

        public abstract void SerializeBlock(SerializerObject s);
        public virtual void SerializeOffsetData(SerializerObject s) { }
        public virtual int GetOffsetTableLengthGCN(SerializerObject s) { return 0; }
        public virtual long GetShanghaiOffsetTableLength => 0;
    }
}