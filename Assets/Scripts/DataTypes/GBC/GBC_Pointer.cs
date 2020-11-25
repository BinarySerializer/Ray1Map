using System;

namespace R1Engine
{
    public class GBC_Pointer : R1Serializable
    {
        public ushort UnkFileIndex { get; set; }
        public ushort FileIndex { get; set; }
        public ushort BlockIndex { get; set; }
        public ushort UShort_06 { get; set; } // Padding?

        public ushort GBC_Offset { get; set; }
        public ushort GBC_Bank { get; set; }

        public T DoAtBlock<T>(Func<T> action)
            where T : class
        {
            var s = Context.Deserializer;

            if (Context.Settings.EngineVersion == EngineVersion.GBC_R1_Palm)
            {
                var offTable = Context.GetStoredObject<GBC_GlobalOffsetTable>("GlobalOffsetTable");
                var ptr = offTable?.Resolve(this);

                return s.DoAt(ptr, action);
            }
            else
            {
                var offset = Offset.file.StartPointer + (0x4000 * GBC_Bank) + (GBC_Offset - 0x4000); // The ROM is split into memory banks, with the size 0x4000 which get loaded at 0x4000 in RAM.
                return s.DoAt(offset, action);
            }
        }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1_Palm)
            {
                UnkFileIndex = s.Serialize<ushort>(UnkFileIndex, name: nameof(UnkFileIndex));
                FileIndex = s.Serialize<ushort>(FileIndex, name: nameof(FileIndex));
                BlockIndex = s.Serialize<ushort>(BlockIndex, name: nameof(BlockIndex));
                UShort_06 = s.Serialize<ushort>(UShort_06, name: nameof(UShort_06));
            }
            else
            {
                GBC_Offset = s.Serialize<ushort>(GBC_Offset, name: nameof(GBC_Offset));
                GBC_Bank = s.Serialize<ushort>(GBC_Bank, name: nameof(GBC_Bank));
            }
        }
    }
}