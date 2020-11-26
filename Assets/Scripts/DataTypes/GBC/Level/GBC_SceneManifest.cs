namespace R1Engine
{
    public class GBC_SceneManifest : GBC_BaseBlock
    {
        // Parsed
        public GBC_SceneList SceneList { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            // This block has no data

            // Parse data from pointers (first pointer leads to scene list, remaining pointers lead to the level scenes)
            SceneList = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_SceneList>(SceneList, name: nameof(SceneList)));
        }
    }
}