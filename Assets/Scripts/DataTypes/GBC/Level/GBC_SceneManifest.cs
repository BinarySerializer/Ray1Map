namespace R1Engine
{
    public class GBC_SceneManifest : GBC_BaseBlock
    {
        // Parsed
        public GBC_SceneList SceneList { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);
            SerializeOffsetTable(s);

            // Parse data from pointers
            // Read SceneList at 0 or read Unknown1 at (s.GameSettings.Level + 1)
            SceneList = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_SceneList>(SceneList, name: nameof(SceneList)));
        }
    }
}