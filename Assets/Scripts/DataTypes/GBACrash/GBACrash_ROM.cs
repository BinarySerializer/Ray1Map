namespace R1Engine
{
    public class GBACrash_ROM : GBA_ROMBase
    {
        public GBACrash_LocTable LocTable { get; set; }
        public GBACrash_LevelInfo[] LevelInfos { get; set; }

        // 2D
        public GBACrash_AnimSet[] AnimSets { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the pointer table
            var pointerTable = PointerTables.GBACrash_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            LocTable = s.DoAt(pointerTable[GBACrash_Pointer.Localization], () => s.SerializeObject<GBACrash_LocTable>(LocTable, name: nameof(LocTable)));
            s.Context.StoreObject(GBACrash_Crash2_Manager.LocTableID, LocTable);

            LevelInfos = s.DoAt(pointerTable[GBACrash_Pointer.LevelInfo], () => s.SerializeObjectArray<GBACrash_LevelInfo>(LevelInfos, 29, name: nameof(LevelInfos)));

            AnimSets = s.DoAt(pointerTable[GBACrash_Pointer.Map2D_AnimSets], () => s.SerializeObjectArray<GBACrash_AnimSet>(AnimSets, 49, name: nameof(AnimSets)));
        }
    }
}