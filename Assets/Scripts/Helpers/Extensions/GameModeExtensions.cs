using System;

namespace Ray1Map
{
    public static class GameModeExtensions
    {
        public static BaseGameManager GetManager(this GameModeSelection mode) => (BaseGameManager)Activator.CreateInstance(mode.GetAttribute<GameModeAttribute>().ManagerType);
    }
}