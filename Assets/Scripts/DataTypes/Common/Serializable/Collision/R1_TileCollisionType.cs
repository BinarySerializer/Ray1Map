namespace R1Engine
{
    // TODO: Split up between R1 & EDU/KIT
    /// <summary>
    /// The tile collision types for Rayman 1
    /// </summary>
    public enum R1_TileCollisionType : byte
    {
        None = 0,
        Reactionary = 1,
        Hill_Steep_Left = 2,
        Hill_Steep_Right = 3,
        Hill_Slight_Left_1 = 4,
        Hill_Slight_Left_2 = 5,
        Hill_Slight_Right_2 = 6,
        Hill_Slight_Right_1 = 7,
        Damage = 8,
        Bounce = 9,
        Water = 10,
        Exit = 11,
        Climb = 12,
        WaterNoSplash = 13,
        Passthrough = 14,
        Solid = 15,
        Seed = 16,
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

    /// <summary>
    /// The tile collision types for Rayman 1 on Jaguar
    /// </summary>
    public enum Jaguar_R1_TileCollisionType : byte
    {
        None = 0,
        Reactionary = 1,
        Hill_Steep_Left = 2,
        Hill_Steep_Right = 3,
        Hill_Slight_Left_1 = 4,
        Hill_Slight_Left_2 = 5,
        Hill_Slight_Right_2 = 6,
        Hill_Slight_Right_1 = 7,
        Damage = 8,
        Bounce = 9,
        Water = 10,
        Spikes = 11,
        Climb = 12,
        PassthroughProto = 13,
        Passthrough = 14,
        Solid = 15,
    }

    public enum TileCollisionTypeGraphic
    {
        None = 0,
        Reactionary = 1,
        Hill_Steep_Left = 2,
        Hill_Steep_Right = 3,
        Hill_Slight_Left_1 = 4,
        Hill_Slight_Left_2 = 5,
        Hill_Slight_Right_2 = 6,
        Hill_Slight_Right_1 = 7,
        Damage = 8,
        Bounce = 9,
        Water = 10,
        Exit = 11,
        Climb = 12,
        WaterNoSplash = 13,
        Passthrough = 14,
        Solid = 15,
        Seed = 16,
        Unknown0 = 17,
        Slippery_Steep_Left = 18,
        Slippery_Steep_Right = 19,
        Slippery_Slight_Left_1 = 20,
        Slippery_Slight_Left_2 = 21,
        Slippery_Slight_Right_2 = 22,
        Slippery_Slight_Right_1 = 23,
        Spikes = 24,
        Cliff = 25,
        Unknown1 = 26,
        Unknown2 = 27,
        Unknown3 = 28,
        Unknown4 = 29,
        Slippery = 30
    }

    public enum R2_TileCollsionType : byte
    {
        None = 0,

        // It appears that the reactionary tiles work in a way of changing a moving platform's position in directions like "up-left", like how the UFO works in R1
        Reactionary0 = 1,
        Reactionary1 = 2,
        Reactionary2 = 3,
        Reactionary3 = 4,
        Reactionary4 = 5,

        Cliff = 18,
        Water = 19,

        Solid = 22,
        Passthrough = 23,

        Hill_Slight_Left_1 = 25,
        Hill_Slight_Left_2 = 26,
        Hill_Steep_Left = 27,

        Hill_Slight_Right_2 = 28,
        Hill_Slight_Right_1 = 29,
        Hill_Steep_Right = 30,

        // Used for enemy movements
        ReactionaryEnemy = 47,
        
        ReactionaryUnk = 49,
    }

    public static class TileCollisionTypeExtensions
    {
        public static TileCollisionTypeGraphic GetCollisionTypeGraphic(this R1_TileCollisionType collisionType)
        {
            switch (collisionType)
            {
                case R1_TileCollisionType.None:
                    return TileCollisionTypeGraphic.None;

                case R1_TileCollisionType.Reactionary:
                    return TileCollisionTypeGraphic.Reactionary;

                case R1_TileCollisionType.Hill_Steep_Left:
                    return TileCollisionTypeGraphic.Hill_Steep_Left;

                case R1_TileCollisionType.Hill_Steep_Right:
                    return TileCollisionTypeGraphic.Hill_Steep_Right;

                case R1_TileCollisionType.Hill_Slight_Left_1:
                    return TileCollisionTypeGraphic.Hill_Slight_Left_1;

                case R1_TileCollisionType.Hill_Slight_Left_2:
                    return TileCollisionTypeGraphic.Hill_Slight_Left_2;

                case R1_TileCollisionType.Hill_Slight_Right_2:
                    return TileCollisionTypeGraphic.Hill_Slight_Right_2;

                case R1_TileCollisionType.Hill_Slight_Right_1:
                    return TileCollisionTypeGraphic.Hill_Slight_Right_1;

                case R1_TileCollisionType.Damage:
                    return TileCollisionTypeGraphic.Damage;

                case R1_TileCollisionType.Bounce:
                    return TileCollisionTypeGraphic.Bounce;

                case R1_TileCollisionType.Water:
                    return TileCollisionTypeGraphic.Water;

                case R1_TileCollisionType.Exit:
                    return TileCollisionTypeGraphic.Exit;

                case R1_TileCollisionType.Climb:
                    return TileCollisionTypeGraphic.Climb;

                case R1_TileCollisionType.WaterNoSplash:
                    return TileCollisionTypeGraphic.WaterNoSplash;

                case R1_TileCollisionType.Passthrough:
                    return TileCollisionTypeGraphic.Passthrough;

                case R1_TileCollisionType.Solid:
                    return TileCollisionTypeGraphic.Solid;

                case R1_TileCollisionType.Seed:
                    return TileCollisionTypeGraphic.Seed;

                case R1_TileCollisionType.Slippery_Steep_Left:
                    return TileCollisionTypeGraphic.Slippery_Steep_Left;

                case R1_TileCollisionType.Slippery_Steep_Right:
                    return TileCollisionTypeGraphic.Slippery_Steep_Right;

                case R1_TileCollisionType.Slippery_Slight_Left_1:
                    return TileCollisionTypeGraphic.Slippery_Slight_Left_1;

                case R1_TileCollisionType.Slippery_Slight_Left_2:
                    return TileCollisionTypeGraphic.Slippery_Slight_Left_2;

                case R1_TileCollisionType.Slippery_Slight_Right_2:
                    return TileCollisionTypeGraphic.Slippery_Slight_Right_2;

                case R1_TileCollisionType.Slippery_Slight_Right_1:
                    return TileCollisionTypeGraphic.Slippery_Slight_Right_1;

                case R1_TileCollisionType.Spikes:
                    return TileCollisionTypeGraphic.Spikes;

                case R1_TileCollisionType.Cliff:
                    return TileCollisionTypeGraphic.Cliff;

                case R1_TileCollisionType.Slippery:
                    return TileCollisionTypeGraphic.Slippery;

                default:
                    return TileCollisionTypeGraphic.Unknown0;
            }
        }

        public static TileCollisionTypeGraphic GetCollisionTypeGraphic(this Jaguar_R1_TileCollisionType collisionType)
        {
            switch (collisionType)
            {
                case Jaguar_R1_TileCollisionType.None:
                    return TileCollisionTypeGraphic.None;

                case Jaguar_R1_TileCollisionType.Reactionary:
                    return TileCollisionTypeGraphic.Reactionary;

                case Jaguar_R1_TileCollisionType.Hill_Steep_Left:
                    return TileCollisionTypeGraphic.Hill_Steep_Left;

                case Jaguar_R1_TileCollisionType.Hill_Steep_Right:
                    return TileCollisionTypeGraphic.Hill_Steep_Right;

                case Jaguar_R1_TileCollisionType.Hill_Slight_Left_1:
                    return TileCollisionTypeGraphic.Hill_Slight_Left_1;

                case Jaguar_R1_TileCollisionType.Hill_Slight_Left_2:
                    return TileCollisionTypeGraphic.Hill_Slight_Left_2;

                case Jaguar_R1_TileCollisionType.Hill_Slight_Right_2:
                    return TileCollisionTypeGraphic.Hill_Slight_Right_2;

                case Jaguar_R1_TileCollisionType.Hill_Slight_Right_1:
                    return TileCollisionTypeGraphic.Hill_Slight_Right_1;

                case Jaguar_R1_TileCollisionType.Damage:
                    return TileCollisionTypeGraphic.Damage;

                case Jaguar_R1_TileCollisionType.Bounce:
                    return TileCollisionTypeGraphic.Bounce;

                case Jaguar_R1_TileCollisionType.Water:
                    return TileCollisionTypeGraphic.Water;

                case Jaguar_R1_TileCollisionType.Climb:
                    return TileCollisionTypeGraphic.Climb;

                case Jaguar_R1_TileCollisionType.PassthroughProto:
                    return TileCollisionTypeGraphic.Passthrough;

                case Jaguar_R1_TileCollisionType.Passthrough:
                    return TileCollisionTypeGraphic.Passthrough;

                case Jaguar_R1_TileCollisionType.Solid:
                    return TileCollisionTypeGraphic.Solid;

                case Jaguar_R1_TileCollisionType.Spikes:
                    return TileCollisionTypeGraphic.Spikes;

                default:
                    return TileCollisionTypeGraphic.Unknown0;
            }
        }

        public static TileCollisionTypeGraphic GetCollisionTypeGraphic(this R2_TileCollsionType collisionType)
        {
            switch (collisionType)
            {
                case R2_TileCollsionType.None:
                    return TileCollisionTypeGraphic.None;

                case R2_TileCollsionType.Reactionary0:
                    return TileCollisionTypeGraphic.Reactionary;

                case R2_TileCollsionType.Reactionary1:
                    return TileCollisionTypeGraphic.Reactionary;

                case R2_TileCollsionType.Reactionary2:
                    return TileCollisionTypeGraphic.Reactionary;

                case R2_TileCollsionType.Reactionary3:
                    return TileCollisionTypeGraphic.Reactionary;

                case R2_TileCollsionType.Reactionary4:
                    return TileCollisionTypeGraphic.Reactionary;

                case R2_TileCollsionType.Cliff:
                    return TileCollisionTypeGraphic.Cliff;

                case R2_TileCollsionType.Water:
                    return TileCollisionTypeGraphic.Water;

                case R2_TileCollsionType.Solid:
                    return TileCollisionTypeGraphic.Solid;

                case R2_TileCollsionType.Passthrough:
                    return TileCollisionTypeGraphic.Passthrough;

                case R2_TileCollsionType.Hill_Slight_Left_1:
                    return TileCollisionTypeGraphic.Hill_Slight_Left_1;

                case R2_TileCollsionType.Hill_Slight_Left_2:
                    return TileCollisionTypeGraphic.Hill_Slight_Left_2;

                case R2_TileCollsionType.Hill_Steep_Left:
                    return TileCollisionTypeGraphic.Hill_Steep_Left;

                case R2_TileCollsionType.Hill_Slight_Right_1:
                    return TileCollisionTypeGraphic.Hill_Slight_Right_1;

                case R2_TileCollsionType.Hill_Slight_Right_2:
                    return TileCollisionTypeGraphic.Hill_Slight_Right_2;

                case R2_TileCollsionType.Hill_Steep_Right:
                    return TileCollisionTypeGraphic.Hill_Steep_Right;

                case R2_TileCollsionType.ReactionaryEnemy:
                    return TileCollisionTypeGraphic.Reactionary;

                case R2_TileCollsionType.ReactionaryUnk:
                    return TileCollisionTypeGraphic.Reactionary;

                default:
                    return TileCollisionTypeGraphic.Unknown0;
            }
        }
    }
}