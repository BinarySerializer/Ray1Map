namespace R1Engine
{
    public class GBACrash_ROM : GBA_ROMBase
    {
        public GBACrash_LocTable LocTable { get; set; }
        public GBACrash_LevelInfo[] LevelInfos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            LocTable = s.DoAt(Offset + 0x1d5c04, () => s.SerializeObject<GBACrash_LocTable>(LocTable, name: nameof(LocTable)));
            s.Context.StoreObject(GBACrash_Crash2_Manager.LocTableID, LocTable);

            LevelInfos = s.DoAt(Offset + 0x1d2714, () => s.SerializeObjectArray<GBACrash_LevelInfo>(LevelInfos, 29, name: nameof(LevelInfos)));
        }
    }
}