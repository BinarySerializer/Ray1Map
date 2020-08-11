namespace R1Engine
{
    // Event types have to be ushorts right now even though this is a byte
    public enum GBA_R3_ActorID : ushort
    {
        MovingPlatform_Vertical = 8,
        Switch = 10,
        YellowLum = 12,
        Pirate = 25,
        LevelExit_Back = 30,
        Butterfly = 37,
        Jano = 39,
        BreakableGround = 50,
        MurfyStone = 92,
        LevelExit_Next = 101
    }
}