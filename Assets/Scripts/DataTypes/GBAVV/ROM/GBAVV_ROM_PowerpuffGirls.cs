using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_ROM_PowerpuffGirls : GBAVV_BaseROM
    {
        // Helpers
        public GBAVV_Map CurrentMap => LevelInfos[Context.GetR1Settings().Level].Map;

        // Common
        public GBAVV_PowerpuffGirls_LevelInfo[] LevelInfos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the pointer table
            var pointerTable = PointerTables.GBAVV_PointerTable(s.GetR1Settings().GameModeSelection, Offset.File);

            // Serialize level infos
            s.DoAt(pointerTable.TryGetItem(GBAVV_Pointer.LevelInfo), () =>
            {
                if (LevelInfos == null)
                    LevelInfos = new GBAVV_PowerpuffGirls_LevelInfo[58];

                for (int i = 0; i < LevelInfos.Length; i++)
                    LevelInfos[i] = s.SerializeObject<GBAVV_PowerpuffGirls_LevelInfo>(LevelInfos[i], x => x.SerializeData = i == s.GetR1Settings().Level, name: $"{nameof(LevelInfos)}[{i}]");
            });

            // Serialize graphics
            SerializeGraphics(s);
        }
    }
}