using BinarySerializer;

namespace R1Engine
{
    public class GBA_Data : BinarySerializable
    {
        // Common
        public GBA_OffsetTable UiOffsetTable { get; set; }
        public GBA_Scene Scene { get; set; }
        public GBA_PlayField MenuLevelPlayfield { get; set; }

        // Mad Trax
        public GBA_PlayField MadTraxPlayField_BG { get; set; }
        public GBA_PlayField MadTraxPlayField_FG { get; set; }
        public GBA_Palette MadTraxPalette { get; set; }

        // Shanghai
        public GBA_Shanghai_Scene Shanghai_Scene { get; set; }

        // Milan
        public GBA_Milan_SceneList Milan_SceneList { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Get the pointer table
            var pointerTable = PointerTables.GBA_PointerTable(s.Context, Offset.File);

            // Serialize the offset table
            s.DoAt(pointerTable[GBA_Pointer.UiOffsetTable], () => UiOffsetTable = s.SerializeObject<GBA_OffsetTable>(UiOffsetTable, name: nameof(UiOffsetTable)));

            var manager = (GBA_Manager)s.Context.GetR1Settings().GetGameManager;

            switch (manager.GetLevelType(s.Context))
            {
                case GBA_Manager.LevelType.Game:
                    // Serialize the level block for the current level

                    // Common
                    if (s.GetR1Settings().GBA_IsCommon)
                    {
                        Scene = s.DoAt(UiOffsetTable.GetPointer(s.Context.GetR1Settings().Level), () => s.SerializeObject<GBA_Scene>(Scene, name: nameof(Scene)));
                    }
                    // Mad Trax
                    else if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_R3_MadTrax)
                    {
                        MadTraxPlayField_BG = s.DoAt(UiOffsetTable.GetPointer(0), () => s.SerializeObject<GBA_PlayField>(MadTraxPlayField_BG, name: nameof(MadTraxPlayField_BG)));
                        MadTraxPlayField_FG = s.DoAt(UiOffsetTable.GetPointer(1), () => s.SerializeObject<GBA_PlayField>(MadTraxPlayField_FG, name: nameof(MadTraxPlayField_FG)));
                        MadTraxPalette = s.DoAt(UiOffsetTable.GetPointer(2), () => s.SerializeObject<GBA_Palette>(MadTraxPalette, name: nameof(MadTraxPalette)));
                    }
                    // Milan
                    else if (s.GetR1Settings().GBA_IsMilan)
                    {
                        Milan_SceneList = s.DoAt(UiOffsetTable.GetPointer(0), () => s.SerializeObject<GBA_Milan_SceneList>(Milan_SceneList, name: nameof(Milan_SceneList)));
                        // TODO: Parse block 1 and 2 (menus?)
                    }
                    // Shanghai
                    else
                    {
                        Shanghai_Scene = s.DoAt(UiOffsetTable.GetPointer(s.Context.GetR1Settings().Level), () => s.SerializeObject<GBA_Shanghai_Scene>(Shanghai_Scene, name: nameof(Shanghai_Scene)));
                    }

                    break;
                
                case GBA_Manager.LevelType.Menu:
                    // Serialize the playfield for the current menu
                    MenuLevelPlayfield = s.DoAt(UiOffsetTable.GetPointer(s.Context.GetR1Settings().Level), () => s.SerializeObject<GBA_PlayField>(MenuLevelPlayfield, name: nameof(MenuLevelPlayfield)));
                    break;

                case GBA_Manager.LevelType.DLC:
                    // Nothing more to do here if it's a DLC map...
                    break;
            }
        }
    }
}