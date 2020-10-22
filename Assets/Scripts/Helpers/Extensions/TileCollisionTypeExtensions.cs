namespace R1Engine
{
    public static class TileCollisionTypeExtensions
    {
        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this R1_TileCollisionType collisionType)
        {
            switch (collisionType)
            {
                case R1_TileCollisionType.None:
                    return Unity_MapCollisionTypeGraphic.None;

                case R1_TileCollisionType.Reactionary:
                    return Unity_MapCollisionTypeGraphic.Reactionary;

                case R1_TileCollisionType.Hill_Steep_Left:
                    return Unity_MapCollisionTypeGraphic.Hill_Steep_Left;

                case R1_TileCollisionType.Hill_Steep_Right:
                    return Unity_MapCollisionTypeGraphic.Hill_Steep_Right;

                case R1_TileCollisionType.Hill_Slight_Left_1:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Left_1;

                case R1_TileCollisionType.Hill_Slight_Left_2:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Left_2;

                case R1_TileCollisionType.Hill_Slight_Right_2:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Right_2;

                case R1_TileCollisionType.Hill_Slight_Right_1:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Right_1;

                case R1_TileCollisionType.Damage:
                    return Unity_MapCollisionTypeGraphic.Damage;

                case R1_TileCollisionType.Bounce:
                    return Unity_MapCollisionTypeGraphic.Bounce;

                case R1_TileCollisionType.Water:
                    return Unity_MapCollisionTypeGraphic.Water;

                case R1_TileCollisionType.Exit:
                    return Unity_MapCollisionTypeGraphic.Exit;

                case R1_TileCollisionType.Climb:
                    return Unity_MapCollisionTypeGraphic.Climb;

                case R1_TileCollisionType.WaterNoSplash:
                    return Unity_MapCollisionTypeGraphic.Water_NoSplash;

                case R1_TileCollisionType.Passthrough:
                    return Unity_MapCollisionTypeGraphic.Passthrough;

                case R1_TileCollisionType.Solid:
                    return Unity_MapCollisionTypeGraphic.Solid;

                case R1_TileCollisionType.Seed:
                    return Unity_MapCollisionTypeGraphic.Seed;

                case R1_TileCollisionType.Slippery_Steep_Left:
                    return Unity_MapCollisionTypeGraphic.Slippery_Steep_Left;

                case R1_TileCollisionType.Slippery_Steep_Right:
                    return Unity_MapCollisionTypeGraphic.Slippery_Steep_Right;

                case R1_TileCollisionType.Slippery_Slight_Left_1:
                    return Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_1;

                case R1_TileCollisionType.Slippery_Slight_Left_2:
                    return Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_2;

                case R1_TileCollisionType.Slippery_Slight_Right_2:
                    return Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_2;

                case R1_TileCollisionType.Slippery_Slight_Right_1:
                    return Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_1;

                case R1_TileCollisionType.Spikes:
                    return Unity_MapCollisionTypeGraphic.Spikes;

                case R1_TileCollisionType.Cliff:
                    return Unity_MapCollisionTypeGraphic.Cliff;

                case R1_TileCollisionType.Slippery:
                    return Unity_MapCollisionTypeGraphic.Slippery;

                default:
                    return Unity_MapCollisionTypeGraphic.Unknown0;
            }
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this R1Jaguar_TileCollisionType collisionType)
        {
            switch (collisionType)
            {
                case R1Jaguar_TileCollisionType.None:
                    return Unity_MapCollisionTypeGraphic.None;

                case R1Jaguar_TileCollisionType.Reactionary:
                    return Unity_MapCollisionTypeGraphic.Reactionary;

                case R1Jaguar_TileCollisionType.Hill_Steep_Left:
                    return Unity_MapCollisionTypeGraphic.Hill_Steep_Left;

                case R1Jaguar_TileCollisionType.Hill_Steep_Right:
                    return Unity_MapCollisionTypeGraphic.Hill_Steep_Right;

                case R1Jaguar_TileCollisionType.Hill_Slight_Left_1:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Left_1;

                case R1Jaguar_TileCollisionType.Hill_Slight_Left_2:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Left_2;

                case R1Jaguar_TileCollisionType.Hill_Slight_Right_2:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Right_2;

                case R1Jaguar_TileCollisionType.Hill_Slight_Right_1:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Right_1;

                case R1Jaguar_TileCollisionType.Damage:
                    return Unity_MapCollisionTypeGraphic.Damage;

                case R1Jaguar_TileCollisionType.Bounce:
                    return Unity_MapCollisionTypeGraphic.Bounce;

                case R1Jaguar_TileCollisionType.Water:
                    return Unity_MapCollisionTypeGraphic.Water;

                case R1Jaguar_TileCollisionType.Climb:
                    return Unity_MapCollisionTypeGraphic.Climb;

                case R1Jaguar_TileCollisionType.PassthroughProto:
                    return Unity_MapCollisionTypeGraphic.Passthrough;

                case R1Jaguar_TileCollisionType.Passthrough:
                    return Unity_MapCollisionTypeGraphic.Passthrough;

                case R1Jaguar_TileCollisionType.Solid:
                    return Unity_MapCollisionTypeGraphic.Solid;

                case R1Jaguar_TileCollisionType.Spikes:
                    return Unity_MapCollisionTypeGraphic.Spikes;

                default:
                    return Unity_MapCollisionTypeGraphic.Unknown0;
            }
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this R2_TileCollsionType collisionType)
        {
            switch (collisionType)
            {
                case R2_TileCollsionType.None:
                    return Unity_MapCollisionTypeGraphic.None;

                case R2_TileCollsionType.Reactionary0:
                    return Unity_MapCollisionTypeGraphic.Reactionary;

                case R2_TileCollsionType.Reactionary1:
                    return Unity_MapCollisionTypeGraphic.Reactionary;

                case R2_TileCollsionType.Reactionary2:
                    return Unity_MapCollisionTypeGraphic.Reactionary;

                case R2_TileCollsionType.Reactionary3:
                    return Unity_MapCollisionTypeGraphic.Reactionary;

                case R2_TileCollsionType.Reactionary4:
                    return Unity_MapCollisionTypeGraphic.Reactionary;

                case R2_TileCollsionType.Cliff:
                    return Unity_MapCollisionTypeGraphic.Cliff;

                case R2_TileCollsionType.Water:
                    return Unity_MapCollisionTypeGraphic.Water;

                case R2_TileCollsionType.Solid:
                    return Unity_MapCollisionTypeGraphic.Solid;

                case R2_TileCollsionType.Passthrough:
                    return Unity_MapCollisionTypeGraphic.Passthrough;

                case R2_TileCollsionType.Hill_Slight_Left_1:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Left_1;

                case R2_TileCollsionType.Hill_Slight_Left_2:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Left_2;

                case R2_TileCollsionType.Hill_Steep_Left:
                    return Unity_MapCollisionTypeGraphic.Hill_Steep_Left;

                case R2_TileCollsionType.Hill_Slight_Right_1:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Right_1;

                case R2_TileCollsionType.Hill_Slight_Right_2:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Right_2;

                case R2_TileCollsionType.Hill_Steep_Right:
                    return Unity_MapCollisionTypeGraphic.Hill_Steep_Right;

                case R2_TileCollsionType.ReactionaryEnemy:
                    return Unity_MapCollisionTypeGraphic.Reactionary;

                case R2_TileCollsionType.ReactionaryUnk:
                    return Unity_MapCollisionTypeGraphic.Reactionary;

                default:
                    return Unity_MapCollisionTypeGraphic.Unknown0;
            }
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBA_TileCollisionType collisionType, EngineVersion engineVersion)
        {
            switch (collisionType)
            {
                case GBA_TileCollisionType.Empty:
                    return Unity_MapCollisionTypeGraphic.None;

                case GBA_TileCollisionType.Slippery:
                    return engineVersion != EngineVersion.GBA_BatmanVengeance ? Unity_MapCollisionTypeGraphic.Slippery : Unity_MapCollisionTypeGraphic.Solid;

                case GBA_TileCollisionType.Damage:
                    return Unity_MapCollisionTypeGraphic.Damage;
                case GBA_TileCollisionType.Ledge:
                    return Unity_MapCollisionTypeGraphic.LedgeGrab;
                case GBA_TileCollisionType.Passthrough:
                    return Unity_MapCollisionTypeGraphic.Passthrough;

                case GBA_TileCollisionType.Solid:
                case GBA_TileCollisionType.EndSlippery:
                    return engineVersion != EngineVersion.GBA_BatmanVengeance ? Unity_MapCollisionTypeGraphic.Solid : Unity_MapCollisionTypeGraphic.None;

                case GBA_TileCollisionType.Climb:
                    return Unity_MapCollisionTypeGraphic.Climb_Full;
                case GBA_TileCollisionType.Hang:
                    return Unity_MapCollisionTypeGraphic.Climb_Hang;

                case GBA_TileCollisionType.ClimbableWalls:
                case GBA_TileCollisionType.Climb_Spider_51:
                case GBA_TileCollisionType.Climb_Spider_52:
                case GBA_TileCollisionType.Climb_Spider_53:
                case GBA_TileCollisionType.Climb_Spider_54:
                    return Unity_MapCollisionTypeGraphic.Climb;

                case GBA_TileCollisionType.Solid_Right_2:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Right_2;

                case GBA_TileCollisionType.Solid_Right_1:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Right_1;

                case GBA_TileCollisionType.Solid_Left_2:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Left_2;

                case GBA_TileCollisionType.Solid_Left_1:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Left_1;

                case GBA_TileCollisionType.Slippery_Right_2:
                    return Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_2;

                case GBA_TileCollisionType.Slippery_Right_1:
                    return Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_1;

                case GBA_TileCollisionType.Slippery_Left_2:
                    return Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_2;

                case GBA_TileCollisionType.Slippery_Left_1:
                    return Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_1;

                case GBA_TileCollisionType.Direction_Down:
                    return Unity_MapCollisionTypeGraphic.Direction_Down;
                case GBA_TileCollisionType.Direction_DownLeft:
                    return Unity_MapCollisionTypeGraphic.Direction_DownLeft;
                case GBA_TileCollisionType.Direction_DownRight:
                    return Unity_MapCollisionTypeGraphic.Direction_DownRight;
                case GBA_TileCollisionType.Direction_Left:
                    return Unity_MapCollisionTypeGraphic.Direction_Left;
                case GBA_TileCollisionType.Direction_Right:
                    return Unity_MapCollisionTypeGraphic.Direction_Right;
                case GBA_TileCollisionType.Direction_Up:
                    return Unity_MapCollisionTypeGraphic.Direction_Up;
                case GBA_TileCollisionType.Direction_UpLeft:
                    return Unity_MapCollisionTypeGraphic.Direction_UpLeft;
                case GBA_TileCollisionType.Direction_UpRight:
                    return Unity_MapCollisionTypeGraphic.Direction_UpRight;

                case GBA_TileCollisionType.EnemyTrigger_Left:
                case GBA_TileCollisionType.EnemyTrigger_Right:
                case GBA_TileCollisionType.EnemyTrigger_Up:
                case GBA_TileCollisionType.EnemyTrigger_Down:
                case GBA_TileCollisionType.Reactionary_Turn_45CounterClockwise:
                case GBA_TileCollisionType.Reactionary_Turn_90CounterClockwise:
                case GBA_TileCollisionType.Reactionary_Turn_90Clockwise:
                case GBA_TileCollisionType.Reactionary_Turn_45Clockwise:
                    return Unity_MapCollisionTypeGraphic.Reactionary;
                
                case GBA_TileCollisionType.AutoJump:
                    return Unity_MapCollisionTypeGraphic.Bounce;

                case GBA_TileCollisionType.Water:
                case GBA_TileCollisionType.Lava:
                    return Unity_MapCollisionTypeGraphic.Water;

                case GBA_TileCollisionType.InstaKill:
                    return Unity_MapCollisionTypeGraphic.Spikes;

                case GBA_TileCollisionType.Cliff:
                    return Unity_MapCollisionTypeGraphic.Cliff;

                default:
                    return Unity_MapCollisionTypeGraphic.Unknown0;
            }
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBARRR_TileCollisionType collisionType)
        {
            switch (collisionType)
            {
                case GBARRR_TileCollisionType.Empty:
                    return Unity_MapCollisionTypeGraphic.None;

                case GBARRR_TileCollisionType.Solid:
                    return Unity_MapCollisionTypeGraphic.Solid;

                case GBARRR_TileCollisionType.Climb:
                    return Unity_MapCollisionTypeGraphic.Climb_Full;

                case GBARRR_TileCollisionType.Hang:
                    return Unity_MapCollisionTypeGraphic.Climb_Hang;

                case GBARRR_TileCollisionType.ClimbableWalls:
                    return Unity_MapCollisionTypeGraphic.Climb;

                case GBARRR_TileCollisionType.SolidNoHang:
                    return Unity_MapCollisionTypeGraphic.Solid; // TODO: New graphic

                case GBARRR_TileCollisionType.Damage:
                    return Unity_MapCollisionTypeGraphic.Damage;

                case GBARRR_TileCollisionType.PinObj:
                    return Unity_MapCollisionTypeGraphic.LedgeGrab;

                case GBARRR_TileCollisionType.Trigger_Right1:
                case GBARRR_TileCollisionType.Trigger_Right2:
                case GBARRR_TileCollisionType.Trigger_Right3:
                    return Unity_MapCollisionTypeGraphic.Direction_Right;

                case GBARRR_TileCollisionType.Trigger_Left1:
                case GBARRR_TileCollisionType.Trigger_Left2:
                case GBARRR_TileCollisionType.Trigger_Left3:
                    return Unity_MapCollisionTypeGraphic.Direction_Left;

                case GBARRR_TileCollisionType.Trigger_Up1:
                case GBARRR_TileCollisionType.Trigger_Up2:
                case GBARRR_TileCollisionType.Trigger_Up3:
                    return Unity_MapCollisionTypeGraphic.Direction_Up;

                case GBARRR_TileCollisionType.Trigger_Down1:
                case GBARRR_TileCollisionType.Trigger_Down2:
                case GBARRR_TileCollisionType.Trigger_Down3:
                    return Unity_MapCollisionTypeGraphic.Direction_Down;

                case GBARRR_TileCollisionType.Trigger_Stop:
                    return Unity_MapCollisionTypeGraphic.Seed; // TODO: New graphic

                case GBARRR_TileCollisionType.DetectionZone:
                    return Unity_MapCollisionTypeGraphic.Exit; // TODO: New graphic

                case GBARRR_TileCollisionType.Solid_Left_1:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Left_1;

                case GBARRR_TileCollisionType.Solid_Left_2:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Left_2;

                case GBARRR_TileCollisionType.Solid_Right_2:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Right_2;

                case GBARRR_TileCollisionType.Solid_Right_1:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Right_1;

                case GBARRR_TileCollisionType.Solid_Left:
                    return Unity_MapCollisionTypeGraphic.Hill_Steep_Left;

                case GBARRR_TileCollisionType.Solid_Right:
                    return Unity_MapCollisionTypeGraphic.Hill_Steep_Right;

                case GBARRR_TileCollisionType.InstaKill:
                    return Unity_MapCollisionTypeGraphic.Spikes;

                case GBARRR_TileCollisionType.Slippery_Left1:
                    return Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_1;

                case GBARRR_TileCollisionType.Slippery_Left2:
                    return Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_2;

                case GBARRR_TileCollisionType.Slippery_Right1:
                    return Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_1;

                case GBARRR_TileCollisionType.Slippery_Right2:
                    return Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_2;

                case GBARRR_TileCollisionType.Slippery_Left:
                    return Unity_MapCollisionTypeGraphic.Slippery_Steep_Left;

                case GBARRR_TileCollisionType.Slippery_Right:
                    return Unity_MapCollisionTypeGraphic.Slippery_Steep_Right;

                case GBARRR_TileCollisionType.Slippery:
                    return Unity_MapCollisionTypeGraphic.Slippery;

                default:
                    return Unity_MapCollisionTypeGraphic.Unknown0;
            }
        }
    }
}