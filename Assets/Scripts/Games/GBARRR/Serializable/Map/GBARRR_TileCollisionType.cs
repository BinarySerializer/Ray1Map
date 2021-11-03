namespace Ray1Map.GBARRR
{
    public enum GBARRR_TileCollisionType : byte
    {
        Empty = 0,

        Solid = 2,
        Climb = 3,
        Hang = 4,
        ClimbableWalls = 5,
        SolidNoHang = 6,
        Damage = 7,
        PinObj = 8,
        Trigger_Right1 = 9,
        Trigger_Right2 = 10,
        Trigger_Right3 = 11,
        Trigger_Left1 = 12,
        Trigger_Left2 = 13,
        Trigger_Left3 = 14,
        Trigger_Up1 = 15,
        Trigger_Up2 = 16,
        Trigger_Up3 = 17,
        Trigger_Down1 = 18,
        Trigger_Down2 = 19,
        Trigger_Down3 = 20,
        Trigger_Stop = 21,
        DetectionZone = 22, // For cameras in front of doors
        Solid_Left_1 = 23,
        Solid_Left_2 = 24,
        Solid_Right_2 = 25,
        Solid_Right_1 = 26,
        Solid_Left = 27,
        Solid_Right = 28,
        InstaKill = 29,
        Slippery_Left1 = 30,
        Slippery_Left2 = 31,
        Slippery_Right1 = 32,
        Slippery_Right2 = 33,
        Slippery_Left = 34,
        Slippery_Right = 35,
        Slippery = 36,
    }
}