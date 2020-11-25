namespace R1Engine
{
    public class GBC_PlayField : GBC_BaseBlock
    {
        public ushort ActorsCount { get; set; }
        public ushort ActorsOffset { get; set; }
        public ushort Ushort_04 { get; set; }
        public ushort SectorsOffset { get; set; }
        public ushort Ushort_06 { get; set; }

        // Parsed from offsets
        public GBC_Actor[] Actors { get; set; }
        public GBC_Sector[] Sectors { get; set; }

        // Parsed from offset table
        public GBC_Map Map { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);
            SerializeOffsetTable(s);

            var blockOffset = s.CurrentPointer;

            ActorsCount = s.Serialize<ushort>(ActorsCount, name: nameof(ActorsCount));
            ActorsOffset = s.Serialize<ushort>(ActorsOffset, name: nameof(ActorsOffset));
            Ushort_04 = s.Serialize<ushort>(Ushort_04, name: nameof(Ushort_04));
            SectorsOffset = s.Serialize<ushort>(SectorsOffset, name: nameof(SectorsOffset));
            Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
            // TODO: Parse remaining data

            // Parse actors
            s.DoAt(blockOffset + ActorsOffset, () =>
            {
                // TODO: Parse actors (they have different lengths, usually around 0x10 bytes)
                //Actors = s.SerializeObjectArray<GBC_Actor>(Actors, ActorsCount, name: nameof(Actors));
            });

            // Parse sectors
            s.DoAt(blockOffset + SectorsOffset, () =>
            {
                // TODO: Get sector count
            });

            // Parse data from pointers
            Map = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_Map>(Map, name: nameof(Map)));
        }
    }
}