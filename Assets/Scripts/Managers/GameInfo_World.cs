namespace R1Engine
{
    public class GameInfo_World
    {
        public GameInfo_World(int index, int[] maps)
        {
            Index = index;
            Maps = maps;
        }

        public int Index { get; }
        public int[] Maps { get; }
    }
}