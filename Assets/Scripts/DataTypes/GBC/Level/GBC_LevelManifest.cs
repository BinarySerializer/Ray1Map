namespace R1Engine
{
    public class GBC_LevelManifest : GBC_BaseBlock
    {
        // Parsed
        public GBC_LevelList LevelList { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            // This block has no data

            // Parse data from pointers (first pointer leads to scene list, remaining pointers lead to the level scenes)
            LevelList = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_LevelList>(LevelList, name: nameof(LevelList)));
        }
    }
}