namespace R1Engine
{
    public class GBA_Data : R1Serializable
    {
        public GBA_OffsetTable UiOffsetTable { get; set; }

        public GBA_LevelBlock LevelBlock { get; set; }

        public GBA_PlayField MenuLevelPlayfield { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Get the pointer table
            var pointerTable = PointerTables.GBA_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Serialize the offset table
            s.DoAt(pointerTable[GBA_Pointer.UiOffsetTable], () => UiOffsetTable = s.SerializeObject<GBA_OffsetTable>(UiOffsetTable, name: nameof(UiOffsetTable)));

            var manager = (GBA_Manager)s.Context.Settings.GetGameManager;

            switch (manager.GetLevelType(s.Context.Settings.World))
            {
                case GBA_Manager.LevelType.Game:
                    // Serialize the level block for the current level
                    LevelBlock = s.DoAt(UiOffsetTable.GetPointer(s.Context.Settings.Level), () => s.SerializeObject<GBA_LevelBlock>(LevelBlock, name: nameof(LevelBlock)));
                    break;
                
                case GBA_Manager.LevelType.Menu:
                    // Serialize the playfield for the current menu
                    MenuLevelPlayfield = s.DoAt(UiOffsetTable.GetPointer(s.Context.Settings.Level), () => s.SerializeObject<GBA_PlayField>(MenuLevelPlayfield, name: nameof(MenuLevelPlayfield)));
                    break;

                case GBA_Manager.LevelType.DLC:
                    // Nothing more to do here if it's a DLC map...
                    break;
            }
        }
    }
}