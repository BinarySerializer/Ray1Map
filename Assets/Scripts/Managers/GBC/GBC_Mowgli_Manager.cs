using System.Linq;

namespace R1Engine
{
    public class GBC_Mowgli_Manager : GBC_R1_Manager
    {
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 65).ToArray()), 
        });
    }
}