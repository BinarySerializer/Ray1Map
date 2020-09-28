namespace R1Engine
{
    public enum R1_RayMode : short
    {
        PlaceRay = -1, // All other values are PlaceRay, but the game uses -1
        Rayman = 1,
        RayOnMS = 2,
        MortDeRayman1 = 3,
        MortDeRayman2 = 4,
        RayCasseBrique = 5, // PC only
    }
}