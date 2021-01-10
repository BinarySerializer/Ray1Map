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

            // Get the current lev info
            var levInfo = ((GBACrash_Crash2_Manager)s.GameSettings.GetGameManager).LevInfos[s.GameSettings.Level];

            LocTable = s.DoAt(pointerTable[GBACrash_Pointer.Localization], () => s.SerializeObject<GBACrash_LocTable>(LocTable, name: nameof(LocTable)));
            s.Context.StoreObject(GBACrash_Crash2_Manager.LocTableID, LocTable);

            s.DoAt(pointerTable[GBACrash_Pointer.LevelInfo], () =>
            {
                if (LevelInfos == null)
                    LevelInfos = new GBACrash_LevelInfo[29];

                for (int i = 0; i < LevelInfos.Length; i++)
                    LevelInfos[i] = s.SerializeObject<GBACrash_LevelInfo>(LevelInfos[i], x => x.LevInfo = i == levInfo.LevelIndex ? levInfo : null, name: $"{nameof(LevelInfos)}[{i}]");
            });

            AnimSets = s.DoAt(pointerTable[GBACrash_Pointer.Map2D_AnimSets], () => s.SerializeObjectArray<GBACrash_AnimSet>(AnimSets, 49, name: nameof(AnimSets)));
        }
    }
}