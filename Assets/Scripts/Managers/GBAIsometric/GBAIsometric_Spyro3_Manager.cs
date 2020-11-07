using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_Spyro3_Manager : GBAIsometric_Spyro_Manager
    {
        public override int DataTableCount => 2180;

        public override IEnumerable<string> GetLanguages(GameModeSelection gameModeSelection)
        {
            yield return "English";

            //if (gameModeSelection == GameModeSelection.SpyroAdventureEU)
            //{
            //    // TODO: Other languages
            //}
        }

        public override LevelInfo[] LevelInfos => new LevelInfo[]
        {
            // 3D maps
            new LevelInfo(ValueRange.EnumerateRanges(new ValueRange(2, 22), new ValueRange(24, 82)).ToArray(), true, false, new Dictionary<GameModeSelection, uint>()
            {
                [GameModeSelection.SpyroAdventureUS] = 0x081CFE38
            }),
            // Agent 9
            new LevelInfo(ValueRange.EnumerateRanges(new ValueRange(83, 86)).ToArray(), true, true, new Dictionary<GameModeSelection, uint>()
            {
                [GameModeSelection.SpyroAdventureUS] = 0x081D15B0
            }),
            // Sgt. Byrd 
            new LevelInfo(ValueRange.EnumerateRanges(new ValueRange(0, 12)).ToArray(), true, true, new Dictionary<GameModeSelection, uint>()
            {
                [GameModeSelection.SpyroAdventureUS] = 0x081D1028
            }),
        };
    }
}