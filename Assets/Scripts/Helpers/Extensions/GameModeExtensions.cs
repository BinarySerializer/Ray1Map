using System;

namespace R1Engine
{
    public static class GameModeExtensions
    {
        public static BaseGameManager GetManager(this GameModeSelection mode) => (BaseGameManager)Activator.CreateInstance(mode.GetAttribute<GameModeAttribute>().ManagerType);
    }
}