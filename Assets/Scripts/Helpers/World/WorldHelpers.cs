using BinarySerializer.Ray1;

namespace R1Engine
{
    public static class WorldHelpers
    {
        public static World[] GetR1Worlds(bool includeSpecial = false)
        {
            if (includeSpecial)
                return new World[]
                {
                    World.Jungle,
                    World.Music,
                    World.Mountain,
                    World.Image,
                    World.Cave,
                    World.Cake,
                    World.Menu,
                    World.Multiplayer,
                };
            else
                return new World[]
                {
                    World.Jungle,
                    World.Music,
                    World.Mountain,
                    World.Image,
                    World.Cave,
                    World.Cake,
                };
        }
    }
}