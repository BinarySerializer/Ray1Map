namespace R1Engine
{
    public static class WorldHelpers
    {
        public static R1_World[] GetR1Worlds(bool includeSpecial = false)
        {
            if (includeSpecial)
                return new R1_World[]
                {
                    R1_World.Jungle,
                    R1_World.Music,
                    R1_World.Mountain,
                    R1_World.Image,
                    R1_World.Cave,
                    R1_World.Cake,
                    R1_World.Menu,
                    R1_World.Multiplayer,
                };
            else
                return new R1_World[]
                {
                    R1_World.Jungle,
                    R1_World.Music,
                    R1_World.Mountain,
                    R1_World.Image,
                    R1_World.Cave,
                    R1_World.Cake,
                };
        }
    }
}