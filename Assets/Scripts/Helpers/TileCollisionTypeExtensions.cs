using UnityEngine;

namespace R1Engine
{
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

        public static TileCollisionTypeGraphic GetCollisionTypeGraphic(this GBA_TileCollisionType collisionType)
        {
            switch (collisionType)
            {
                case GBA_TileCollisionType.Empty:
                    return TileCollisionTypeGraphic.None;

                case GBA_TileCollisionType.Ledge:
                    return TileCollisionTypeGraphic.Passthrough;

                case GBA_TileCollisionType.Solid:
                    return TileCollisionTypeGraphic.Solid;

                case GBA_TileCollisionType.Climb:
                case GBA_TileCollisionType.Hang:
                case GBA_TileCollisionType.ClimbableWalls:
                    return TileCollisionTypeGraphic.Climb;

                case GBA_TileCollisionType.Hill_Slight_Right_2:
                    return TileCollisionTypeGraphic.Hill_Slight_Right_2;

                case GBA_TileCollisionType.Hill_Slight_Right_1:
                    return TileCollisionTypeGraphic.Hill_Slight_Right_1;

                case GBA_TileCollisionType.Hill_Slight_Left_2:
                    return TileCollisionTypeGraphic.Hill_Slight_Left_2;

                case GBA_TileCollisionType.Hill_Slight_Left_1:
                    return TileCollisionTypeGraphic.Hill_Slight_Left_1;

                case GBA_TileCollisionType.Reactionary_Up:
                case GBA_TileCollisionType.Reactionary_Down:
                    return TileCollisionTypeGraphic.Reactionary;

                case GBA_TileCollisionType.Water:
                case GBA_TileCollisionType.Lava:
                    return TileCollisionTypeGraphic.Water;

                case GBA_TileCollisionType.InstaKill:
                    return TileCollisionTypeGraphic.Spikes;

                default:
                    Debug.LogWarning($"Collision type {collisionType} is not supported");
                    return TileCollisionTypeGraphic.Unknown0;
            }
        }
    }
}