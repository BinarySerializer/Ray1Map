using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public abstract class GBAIsometric_Spyro3_Manager : GBAIsometric_Spyro_Manager
    {
        public override int PortraitsCount => 38;
        public override int DialogCount => 344;
        public override int PrimaryLevelCount => 14;
        public override int LevelMapsCount => 21;
        public override int TotalLevelsCount => 91;
        public override int ObjectTypesCount => 772;

        public override LevelInfo[] LevelInfos => new LevelInfo[]
        {
            // 3D maps
            new LevelInfo(ValueRange.EnumerateRanges(new ValueRange(2, 22), new ValueRange(24, 82)).ToArray(), true, false, new Dictionary<GameModeSelection, uint>()
            {
                [GameModeSelection.SpyroAdventureEU] = 0x081d0b44,
                [GameModeSelection.SpyroAdventureUS] = 0x081CFE38,
            }),
            // Agent 9
            new LevelInfo(ValueRange.EnumerateRanges(new ValueRange(83, 86)).ToArray(), true, true, new Dictionary<GameModeSelection, uint>()
            {
                [GameModeSelection.SpyroAdventureEU] = 0x081d22bc,
                [GameModeSelection.SpyroAdventureUS] = 0x081D15B0
            }),
            // Sgt. Byrd 
            new LevelInfo(ValueRange.EnumerateRanges(new ValueRange(0, 12)).ToArray(), true, true, new Dictionary<GameModeSelection, uint>()
            {
                [GameModeSelection.SpyroAdventureEU] = 0x081d1d34,
                [GameModeSelection.SpyroAdventureUS] = 0x081D1028
            }),
        };
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
                // TODO: Get correct language names
                yield return "English";
                yield return "English1";
                yield return "English2";
                yield return "English3";
                yield return "English4";
                yield return "English5";
            }
        }
    }
}