using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_ROM_Volume : GBAVV_BaseROM
    {
        public GBAVV_Volume_BaseManager.LevInfo CurrentLevInfo { get; set; } // Set before serializing

        // Helpers
        public GBAVV_Map CurrentMap
        {
            get
            {
                var v = Volumes[CurrentLevInfo.Volume];

                if (CurrentLevInfo.Level == -1)
                    return v.PrimaryLevelInfo.MapInfos[CurrentLevInfo.Map].Map;
                else
                    return v.LevelInfos[CurrentLevInfo.Level].MapInfos[CurrentLevInfo.Map].Map;
            }
        }

        // Common
        public GBAVV_Volume[] Volumes { get; set; }
        public int[] VolumeLevelInfoCounts { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the pointer table
            var pointerTable = PointerTables.GBAVV_PointerTable(s.GetR1Settings().GameModeSelection, Offset.File);

            var volumesCount = s.GetR1Settings().GetGameManagerOfType<GBAVV_Volume_BaseManager>().VolumesCount;

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_OverTheHedge || s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_OverTheHedgeHammyGoesNuts)
                VolumeLevelInfoCounts = s.DoAt(pointerTable.TryGetItem(DefinedPointer.LevelInfo) + 4 * volumesCount, () => s.SerializeArray<int>(VolumeLevelInfoCounts, volumesCount, name: nameof(VolumeLevelInfoCounts)));

            // Serialize level infos
            s.DoAt(pointerTable.TryGetItem(DefinedPointer.LevelInfo), () =>
            {
                if (Volumes == null)
                    Volumes = new GBAVV_Volume[volumesCount];

                for (int i = 0; i < Volumes.Length; i++)
                    Volumes[i] = s.SerializeObject<GBAVV_Volume>(Volumes[i], x =>
                    {
                        x.CurrentLevInfo = i == CurrentLevInfo?.Volume ? CurrentLevInfo : null;
                        x.LevelsCount = VolumeLevelInfoCounts?.ElementAtOrDefault(i) ?? 0;
                    }, name: $"{nameof(Volumes)}[{i}]");
            });

            // Serialize graphics
            SerializeGraphics(s);

            // Serialize scripts
            SerializeScripts(s);
        }
    }
}