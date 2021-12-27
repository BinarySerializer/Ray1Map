using System;
using System.Collections.Generic;
using System.Linq;

namespace Ray1Map.GBAIsometric
{
    public abstract class GBAIsometric_Spyro3_Manager : GBAIsometric_Spyro_Manager
    {
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, ValueRange.EnumerateRanges(new ValueRange(2, 22), new ValueRange(24, 82)).ToArray()), // 3D
            new GameInfo_World(1, ValueRange.EnumerateRanges(new ValueRange(83, 86)).ToArray()), // Agent 9
            new GameInfo_World(2, ValueRange.EnumerateRanges(new ValueRange(87, 90)).ToArray()), // Sgt Byrd
            new GameInfo_World(3, ValueRange.EnumerateRanges(new ValueRange(0, 8)).ToArray()), // Byrd Rescue
        });

        public override int PortraitsCount => 38;
        public override int DialogCount => 344;
        public override int CutsceneMapsCount => throw new NotImplementedException();
        public override int PrimaryLevelCount => 14;
        public override int LevelMapsCount => 21;
        public override int TotalLevelsCount => 91;
        public override int ObjectTypesCount => 772;
        public override int LevelDataCount => 80;
        public override int MenuPageCount => 18;
    }
    public class GBAIsometric_Spyro3US_Manager : GBAIsometric_Spyro3_Manager
    {
        public override int DataTableCount => 2180;
        public override int AnimSetsCount => 196;

        public override IEnumerable<string> GetLanguages
        {
            get
            {
                yield return "English";
            }
        }
    }
    public class GBAIsometric_Spyro3EU_Manager : GBAIsometric_Spyro3_Manager
    {
        public override int DataTableCount => 2269;
        public override int AnimSetsCount => 194;

        public override IEnumerable<string> GetLanguages
        {
            get
            {
                yield return "English";
                yield return "Dutch";
                yield return "Spanish";
                yield return "Italian";
                yield return "German";
                yield return "French";
            }
        }
    }
}