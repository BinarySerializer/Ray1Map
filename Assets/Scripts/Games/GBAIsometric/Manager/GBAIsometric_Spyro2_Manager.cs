using System.Linq;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro2_Manager : GBAIsometric_Dragon_BaseManager
    {
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, ValueRange.EnumerateRanges(new ValueRange(0, 34), new ValueRange(39, 42)).ToArray()), // 3D
            new GameInfo_World(1, ValueRange.EnumerateRanges(new ValueRange(0, 3)).ToArray()), // Agent 9
            new GameInfo_World(4, ValueRange.EnumerateRanges(new ValueRange(0, 10)).ToArray()), // Cutscenes
        });
    }
}