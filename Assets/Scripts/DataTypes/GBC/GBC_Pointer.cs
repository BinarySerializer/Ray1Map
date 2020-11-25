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