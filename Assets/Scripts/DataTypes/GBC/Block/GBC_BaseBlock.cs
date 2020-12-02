using UnityEngine;

namespace R1Engine
{
    public abstract class GBC_BaseBlock : R1Serializable 
    {
        public GBC_BlockHeader GBC_Header { get; set; }
        public ushort GBC_DataLength { get; set; } // The size of the data in the block
        public GBC_Pointer GBC_DataPointer { get; set; } // Offset to the end of the offset table (using the current memory bank). The game uses this to skip the offset table and start parsing the data.

        public LUDI_BlockIdentifier LUDI_Header { get; set; }

        public GBC_DependencyTable DependencyTable { get; set; }

        public Pointer BlockStartPointer {
            get {
                if (Context.Settings.EngineVersion == EngineVersion.GBC_R1) {
                    return GBC_DataPointer.GetPointer();
                } else {
                    return Offset + DependencyTable.Size + 4;
                }
            }
        }
        private uint? _cachedBlockLength { get; set; }
        public uint BlockSize {
            get {
                if (Context.Settings.EngineVersion == EngineVersion.GBC_R1) {
                    return GBC_DataLength;
                } else {
                    if (!_cachedBlockLength.HasValue) {
                        var offTable = Context.GetStoredObject<LUDI_GlobalOffsetTable>(GBC_BaseManager.GlobalOffsetTableKey);
                        uint? size = offTable?.GetBlockLength(LUDI_Header);
                        if (size.HasValue) {
                            _cachedBlockLength = size.Value - DependencyTable.Size - 4;
                        } else {
                            _cachedBlockLength = 0;
                        }
                    }
                    return _cachedBlockLength.Value;
                }
            }
        }

        public override void SerializeImpl(SerializerObject s) 
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1)
            {
                GBC_Header = s.SerializeObject<GBC_BlockHeader>(GBC_Header, name: nameof(GBC_Header));
                GBC_DataLength = s.Serialize<ushort>(GBC_DataLength, name: nameof(GBC_DataLength));
                GBC_DataPointer = s.SerializeObject<GBC_Pointer>(GBC_DataPointer, onPreSerialize: x => x.HasMemoryBankValue = false, name: nameof(GBC_DataPointer));
            }
            else
            {
                LUDI_Header = s.SerializeObject<LUDI_BlockIdentifier>(LUDI_Header, name: nameof(LUDI_Header));
            }

            DependencyTable = s.SerializeObject<GBC_DependencyTable>(DependencyTable, name: nameof(DependencyTable));
            s.Goto(BlockStartPointer);
            SerializeBlock(s);

            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1_Palm || s.GameSettings.EngineVersion == EngineVersion.GBC_R1_PocketPC) {
                s.Align(baseOffset: BlockStartPointer);
            }
            CheckBlockSize(s);
        }

        public abstract void SerializeBlock(SerializerObject s);



        private void CheckBlockSize(SerializerObject s) {
             if (BlockStartPointer + BlockSize != s.CurrentPointer) {
                Debug.LogWarning($"{GetType()} @ {Offset}: Serialized size: {(s.CurrentPointer - BlockStartPointer)} != BlockSize: {BlockSize}");
             }
        }
    }
}