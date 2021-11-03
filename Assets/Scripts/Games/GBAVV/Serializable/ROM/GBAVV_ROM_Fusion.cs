using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_ROM_Fusion : GBAVV_BaseROM
    {
        public GBAVV_Fusion_Manager.FusionLevInfo CurrentLevInfo { get; set; } // Set before serializing

        // Helpers
        public GBAVV_Map CurrentMap => Context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_CrashFusion ? Crash_LevelInfos[CurrentLevInfo.LevelIndex].MapData : Spyro_Maps[CurrentLevInfo.LevelIndex];

        // Common
        public GBAVV_CrashFusion_LevelInfo[] Crash_LevelInfos { get; set; }
        public GBAVV_Map[] Spyro_Maps { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the pointer table
            var pointerTable = PointerTables.GBAVV_PointerTable(s.GetR1Settings().GameModeSelection, Offset.File);

            // Get the manager
            var manager = s.GetR1Settings().GetGameManagerOfType<GBAVV_Fusion_Manager>();

            // Serialize level infos
            s.DoAt(pointerTable.TryGetItem(GBAVV_Pointer.LevelInfo), () =>
            {
                var levelsCount = manager.LevInfos.Max(x => x.LevelIndex) + 1;

                if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_CrashFusion)
                {
                    if (Crash_LevelInfos == null)
                        Crash_LevelInfos = new GBAVV_CrashFusion_LevelInfo[levelsCount];

                    for (int i = 0; i < Crash_LevelInfos.Length; i++)
                        Crash_LevelInfos[i] = s.SerializeObject<GBAVV_CrashFusion_LevelInfo>(Crash_LevelInfos[i], x => x.LevInfo = i == CurrentLevInfo.LevelIndex ? CurrentLevInfo : null, name: $"{nameof(Crash_LevelInfos)}[{i}]");
                }
                else
                {
                    if (Spyro_Maps == null)
                        Spyro_Maps = new GBAVV_Map[levelsCount];

                    for (int i = 0; i < Spyro_Maps.Length; i++)
                        Spyro_Maps[i] = s.SerializeObject<GBAVV_Map>(Spyro_Maps[i], x => x.SerializeData = i == CurrentLevInfo.LevelIndex, name: $"{nameof(Spyro_Maps)}[{i}]");
                }
            });

            // Serialize graphics
            SerializeGraphics(s);

            // Serialize scripts
            SerializeScripts(s);
        }
    }
}