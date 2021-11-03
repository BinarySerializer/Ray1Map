using System.Collections.Generic;
using System.Linq;

namespace Ray1Map
{
    public class GameInfo_World
    {
        public GameInfo_World(int index, int[] maps)
        {
            Index = index;
            Maps = maps;
        }
        public GameInfo_World(int index, string worldName, int[] maps)
        {
            Index = index;
            WorldName = worldName;
            Maps = maps;
        }
        public GameInfo_World(int index, string worldName, int[] maps, string[] mapNames)
        {
            Index = index;
            WorldName = worldName;
            Maps = maps;
            MapNames = mapNames;
        }

        public int Index { get; }
        public string WorldName { get; }
        public int[] Maps { get; }
        public string[] MapNames { get; }

        public Dictionary<int, string> GetLevelNamesDictionary() => Enumerable.Range(0, Maps.Length).Select(i => new
        {
            Map = Maps[i],
            Name = MapNames?.ElementAtOrDefault(i)
        }).ToDictionary(x => x.Map, x => x.Name);
    }
}