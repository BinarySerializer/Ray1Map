namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Tron2_Manager : GBAIsometric_Dragon_BaseManager
    {
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, new int[]
            {
                1,3,7,9,10,14,20,22,23,24,26,28,33,36,37,39,40,46,47,49
            })
        });
    }
}