namespace R1Engine
{
    public class GBC_ROM : GBC_ROMBase
    {
        public GBC_SceneList SceneList { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            var pointerTable = PointerTables.GBC_PointerTable(s.GameSettings.GameModeSelection, Offset.file);
            SceneList = s.DoAt(pointerTable[GBC_R1_Pointer.SceneList], () => s.SerializeObject<GBC_SceneList>(SceneList, name: nameof(SceneList)));
        }
    }
}