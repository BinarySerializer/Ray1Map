namespace R1Engine
{
    // Event types have to be ushorts right now even though this is a byte
    public enum GBA_R3_ActorID : ushort
    {
        Rayman = 0,
        MovingPlatform_Vertical = 8,
        Switch = 10,
        Lum = 12,
        PirateBlue = 24,
        Pirate = 25,
        LevelExit_Back = 30,
        Butterfly = 37,
        Jano = 39,
        Ring = 47,
        BreakableGround = 50,
        BreakableWall = 81,
        MurfyStone = 92,
        LevelExit_Next = 101
    }
}