using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_Spyro2_Manager : GBAIsometric_Spyro_Manager
    {
        public override int DataTableCount => 1509;

        public override LevelInfo[] LevelInfos => new LevelInfo[]
        {
            // 3D maps
            new LevelInfo(ValueRange.EnumerateRanges(new ValueRange(0, 34), new ValueRange(39, 42)).ToArray(), true, false, new Dictionary<GameModeSelection, uint>()
            {
                [GameModeSelection.SpyroSeasonFlameUS] = 0x0817a878
            }),
            // Agent 9
            new LevelInfo(ValueRange.EnumerateRanges(new ValueRange(0, 3)).ToArray(), false, true, new Dictionary<GameModeSelection, uint>()
            {
                [GameModeSelection.SpyroSeasonFlameUS] = 0x08178ef8
            }),
        };
    }
}