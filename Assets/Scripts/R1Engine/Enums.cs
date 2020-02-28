namespace R1Engine {
    public enum GameMode {
        RaymanPS1,
    }
    public enum World {
        JUN, MUS, MON, IMG, CAV, CAK
    }
    public enum TypeCollision {
        None = 0,
        Reactionary = 1,
        Hill_Sleep_Left = 2,
        Hill_Steep_Right = 3,
        Hill_Slight_Left_1 = 4,
        Hill_Slight_Left_2 = 5,
        Hill_Slight_Right_2 = 6,
        Hill_Slight_Right_1 = 7,
        Damage = 8,
        Bounce = 9,
        Water = 10,
        Climb = 12,
        Passthrough = 14,
        Solid = 15,
        Slippery_Steep_Left = 18,
        Slippery_Steep_Right = 19,
        Slippery_Slight_Left_1 = 20,
        Slippery_Slight_Left_2 = 21,
        Slippery_Slight_Right_2 = 22,
        Slippery_Slight_Right_1 = 23,
        Spikes = 24,
        Cliff = 25,
        Slippery = 30
    }

    public enum EventBehaviours {
        //_property = 0x3,

        SpriteAnim = 0x04,

        Ting = 0xA1,
        OneUp = 0x8E,
        SuperPower = 0x52,
        Photographer = 0x15,

        BendingPlant = 0x43,
        Plum = 0x08,
        Magician = 0x05,

        FistUpgrade = 0x5F,

        Cage = 0x3A,

        Antitoon = 0x7B,
        Hunter = 0x0C,
        Livingstone_Tall = 0x00,
        Livingstone_Short = 0x09,

        Platform = 0x01,
        Platform_Falling = 0x10,
        Cloud_Disappearing = 0x1A,
        Cloud_Bouncy = 0x1B,
        Cloud_Flashing = 0x1C,

        PricklyBall = 0x29,
        RedDrummer = 0x78,

        Ring = 0x8C,

        MapSign = 0x7C,
        ExitSign = 0x2A,
        Gendoor = 0xA4,

        YinYang = 0xB1,
        YinYang_Spiked = 0x06,

        Pencil_Up_Moving = 0xF1,
        Pencil_Up_Moving_Seq = 0xF2,
        Pencil_Down_Moving = 0xB2,
        Pencil_Down_Moving_Seq = 0xB3,
        Pen = 0xF3,


        Clown_WaterBalloon = 0x3D,
        Clown_Hammer = 0x3C,

        Spell = 0xD4,
        Spell_RemoveSpell = 0xEC
    }
}