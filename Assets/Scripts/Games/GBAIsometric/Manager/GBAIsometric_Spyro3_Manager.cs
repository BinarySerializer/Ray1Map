using System.Linq;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro3_Manager : GBAIsometric_Dragon_BaseManager
    {
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, ValueRange.EnumerateRanges(new ValueRange(2, 22), new ValueRange(24, 82)).ToArray()), // 3D
            new GameInfo_World(1, ValueRange.EnumerateRanges(new ValueRange(83, 86)).ToArray()), // Agent 9
            new GameInfo_World(2, ValueRange.EnumerateRanges(new ValueRange(87, 90)).ToArray()), // Sgt Byrd
            new GameInfo_World(3, ValueRange.EnumerateRanges(new ValueRange(0, 8)).ToArray()), // Byrd Rescue
        });
    }
}