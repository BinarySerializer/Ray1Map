namespace R1Engine
{
    public class GBC_PlayField : GBC_Block
    {
        public byte[] GBC_UnkBytes { get; set; }
        public uint PointersCount { get; set; }
        public GBC_Pointer[] Pointers { get; set; }
        public byte[] UnkData { get; set; } // This data is identical across GBC and PalmOS, so most likely no 16/32-bit values

        // Parsed
        public GBC_Map Map { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);

            // Serialize data
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1)
            {
                GBC_UnkBytes = s.SerializeArray<byte>(GBC_UnkBytes, 9, name: nameof(GBC_UnkBytes));
                PointersCount = s.Serialize<byte>((byte)PointersCount, name: nameof(PointersCount));
            }
            else
            {
                PointersCount = s.Serialize<uint>(PointersCount, name: nameof(PointersCount));
            }

            Pointers = s.SerializeObjectArray<GBC_Pointer>(Pointers, PointersCount, name: nameof(Pointers));
            UnkData = s.SerializeArray<byte>(UnkData, 68, name: nameof(UnkData));

            // Parse data from pointers
            // TODO: Why is the pointer count different per level? Seems the first one always points to the map, and another one usually points to the next level.
            Map = Pointers[0].DoAtBlock(() => s.SerializeObject<GBC_Map>(Map, name: nameof(Map)));
        }
    }
}