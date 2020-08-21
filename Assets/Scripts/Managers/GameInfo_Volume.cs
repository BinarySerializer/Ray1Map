namespace R1Engine
{
    public class GameInfo_Volume
    {
        public GameInfo_Volume(string name, GameInfo_World[] worlds)
        {
            Name = name;
            Worlds = worlds;
        }

        public string Name { get; }

        public GameInfo_World[] Worlds { get; }

        public static GameInfo_Volume[] SingleVolume(GameInfo_World[] worlds) => new GameInfo_Volume[]
        {
            new GameInfo_Volume(null, worlds), 
        };
    }
}