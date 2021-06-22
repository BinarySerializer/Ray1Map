using System;
using System.Linq;

namespace R1Engine
{
    public abstract class Jade_Montpellier_BaseManager : Jade_BaseManager {
        // Levels
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(LevelInfos?.GroupBy(x => x.WorldName).Select((x, i) => {
            return new GameInfo_World(
                index: i,
                worldName: x.Key.ReplaceFirst(CommonLevelBasePath, String.Empty),
                maps: x.Select(m => (int)m.Key).ToArray(),
                mapNames: x.Select(m => m.MapName).ToArray());
        }).ToArray() ?? new GameInfo_World[0]);

        public virtual string CommonLevelBasePath => @"ROOT/EngineDatas/06 Levels/";
    }
}
