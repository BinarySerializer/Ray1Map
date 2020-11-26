namespace R1Engine
{
    public class GBC_PlayField : GBC_BaseBlock
    {
        public ushort ActorsCount { get; set; }
        public ushort ActorsOffset { get; set; }
        public ushort Ushort_04 { get; set; }
        public ushort SectorsOffset { get; set; }
        public ushort Ushort_06 { get; set; }
        public byte[] Bytes_08 { get; set; }

        public ARGB1555Color[] GBC_ObjPalette { get; set; }
        public GBC_UnkActorStruct[] UnkActorStructs { get; set; }

        // Parsed from offsets
        public GBC_Actor[] Actors { get; set; }
        public GBC_Sector[] Sectors { get; set; }

        // Parsed from offset table
        public GBC_Map Map { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);

            var blockOffset = s.CurrentPointer;

            // Serialize data (always little endian)
            s.DoEndian(R1Engine.Serialize.BinaryFile.Endian.Little, () => 
            {
                ActorsCount = s.Serialize<ushort>(ActorsCount, name: nameof(ActorsCount));
                ActorsOffset = s.Serialize<ushort>(ActorsOffset, name: nameof(ActorsOffset));
                Ushort_04 = s.Serialize<ushort>(Ushort_04, name: nameof(Ushort_04));
                SectorsOffset = s.Serialize<ushort>(SectorsOffset, name: nameof(SectorsOffset));
                Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
                Bytes_08 = s.SerializeArray<byte>(Bytes_08, 6, name: nameof(Bytes_08));

                if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1)
                {
                    GBC_ObjPalette = s.SerializeObjectArray<ARGB1555Color>(GBC_ObjPalette, 8 * 4, name: nameof(GBC_ObjPalette));
                    // TODO: Parse unknown bytes
                    // TODO: Parse array of UnkActorStructs (exists on PalmOS too!)
                }
                else
                {
                    // TODO: Parse data
                    // The PalmOS version has different data here (same length though), although it appears to begin with a palette as well which would be irrelevant since it uses the system palette. The data ends with the UnkActorStructs just like on GBC.
                }

                // Parse actors
                Actors = s.DoAt(blockOffset + ActorsOffset, () => s.SerializeObjectArray<GBC_Actor>(Actors, ActorsCount, name: nameof(Actors)));

                // Parse sectors
                s.DoAt(blockOffset + SectorsOffset, () =>
                {
                    // TODO: Get sector count (0x0D in first level)
                });
            });

            // Parse data from pointers
            Map = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_Map>(Map, name: nameof(Map)));
        }

    }
}