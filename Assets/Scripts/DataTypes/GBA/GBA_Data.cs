namespace R1Engine
{
    public class GBA_Data : R1Serializable
    {
        public GBA_OffsetTable UiOffsetTable { get; set; }

        public GBA_Scene Scene { get; set; }

        public GBA_PlayField MenuLevelPlayfield { get; set; }

        public GBA_PlayField MadTraxPlayField_BG { get; set; }
        public GBA_PlayField MadTraxPlayField_FG { get; set; }
        public GBA_Palette MadTraxPalette { get; set; }

        public GBA_Shanghai_Scene Shanghai_Scene { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Get the pointer table
            var pointerTable = PointerTables.GBA_PointerTable(s.Context, Offset.file);

            // Serialize the offset table
            s.DoAt(pointerTable[GBA_Pointer.UiOffsetTable], () => UiOffsetTable = s.SerializeObject<GBA_OffsetTable>(UiOffsetTable, name: nameof(UiOffsetTable)));

            // TODO: Serialize scene for Milan branch
            if (s.GameSettings.EngineVersion == EngineVersion.GBA_TheMummy ||
                s.GameSettings.EngineVersion == EngineVersion.GBA_TombRaiderTheProphecy ||
                s.GameSettings.EngineVersion == EngineVersion.GBA_TomClancysRainbowSixRogueSpear)
                return;

            var manager = (GBA_Manager)s.Context.Settings.GetGameManager;

            switch (manager.GetLevelType(s.Context))
            {
                case GBA_Manager.LevelType.Game:
                    // Serialize the level block for the current level
                    if (s.GameSettings.EngineVersion > EngineVersion.GBA_R3_MadTrax)
                    {
                        Scene = s.DoAt(UiOffsetTable.GetPointer(s.Context.Settings.Level), () => s.SerializeObject<GBA_Scene>(Scene, name: nameof(Scene)));
                    }
                    else if (s.GameSettings.EngineVersion == EngineVersion.GBA_R3_MadTrax)
                    {
                        MadTraxPlayField_BG = s.DoAt(UiOffsetTable.GetPointer(0), () => s.SerializeObject<GBA_PlayField>(MadTraxPlayField_BG, name: nameof(MadTraxPlayField_BG)));
                        MadTraxPlayField_FG = s.DoAt(UiOffsetTable.GetPointer(1), () => s.SerializeObject<GBA_PlayField>(MadTraxPlayField_FG, name: nameof(MadTraxPlayField_FG)));
                        MadTraxPalette = s.DoAt(UiOffsetTable.GetPointer(2), () => s.SerializeObject<GBA_Palette>(MadTraxPalette, name: nameof(MadTraxPalette)));
                    }
                    else
                    {
                        Shanghai_Scene = s.DoAt(UiOffsetTable.GetPointer(s.Context.Settings.Level), () => s.SerializeObject<GBA_Shanghai_Scene>(Shanghai_Scene, name: nameof(Shanghai_Scene)));
                    }

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