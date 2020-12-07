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

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this R2_TileCollisionType collisionType)
        {
            switch (collisionType)
            {
                case R2_TileCollisionType.None:
                    return Unity_MapCollisionTypeGraphic.None;

                case R2_TileCollisionType.Direction_Left:
                    return Unity_MapCollisionTypeGraphic.Direction_Left;

                case R2_TileCollisionType.Direction_Right:
                    return Unity_MapCollisionTypeGraphic.Direction_Right;

                case R2_TileCollisionType.Direction_Up:
                    return Unity_MapCollisionTypeGraphic.Direction_Up;

                case R2_TileCollisionType.Direction_Down:
                    return Unity_MapCollisionTypeGraphic.Direction_Down;

                case R2_TileCollisionType.Direction_UpLeft:
                    return Unity_MapCollisionTypeGraphic.Direction_UpLeft;

                case R2_TileCollisionType.Direction_UpRight:
                    return Unity_MapCollisionTypeGraphic.Direction_UpRight;

                case R2_TileCollisionType.Direction_DownLeft:
                    return Unity_MapCollisionTypeGraphic.Direction_DownLeft;

                case R2_TileCollisionType.Direction_DownRight:
                    return Unity_MapCollisionTypeGraphic.Direction_DownRight;

                case R2_TileCollisionType.Unknown_11:
                case R2_TileCollisionType.Unknown_14:
                    return Unity_MapCollisionTypeGraphic.Reactionary;

                case R2_TileCollisionType.Cliff:
                    return Unity_MapCollisionTypeGraphic.Cliff;

                case R2_TileCollisionType.Water:
                    return Unity_MapCollisionTypeGraphic.Water;

                case R2_TileCollisionType.Solid:
                    return Unity_MapCollisionTypeGraphic.Solid;

                case R2_TileCollisionType.Passthrough:
                    return Unity_MapCollisionTypeGraphic.Passthrough;

                case R2_TileCollisionType.Hill_Slight_Left_1:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Left_1;

                case R2_TileCollisionType.Hill_Slight_Left_2:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Left_2;

                case R2_TileCollisionType.Hill_Steep_Left:
                    return Unity_MapCollisionTypeGraphic.Hill_Steep_Left;

                case R2_TileCollisionType.Hill_Slight_Right_1:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Right_1;

                case R2_TileCollisionType.Hill_Slight_Right_2:
                    return Unity_MapCollisionTypeGraphic.Hill_Slight_Right_2;

                case R2_TileCollisionType.Hill_Steep_Right:
                    return Unity_MapCollisionTypeGraphic.Hill_Steep_Right;

                case R2_TileCollisionType.ReactionaryEnemy:
                    return Unity_MapCollisionTypeGraphic.Reactionary;

                case R2_TileCollisionType.ReactionaryUnk:
                    return Unity_MapCollisionTypeGraphic.Reactionary;

                case R2_TileCollisionType.ValidTarget:
                    return Unity_MapCollisionTypeGraphic.Reactionary; // TODO: New graphic

                case R2_TileCollisionType.InvalidTarget:
                    return Unity_MapCollisionTypeGraphic.Reactionary; // TODO: New graphic

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
                    return Unity_MapCollisionTypeGraphic.Solid_Hangable;
                case GBA_TileCollisionType.Passthrough:
                    return Unity_MapCollisionTypeGraphic.Passthrough;

                case GBA_TileCollisionType.Solid:
                    return engineVersion != EngineVersion.GBA_BatmanVengeance ? Unity_MapCollisionTypeGraphic.Solid : Unity_MapCollisionTypeGraphic.None;

                case GBA_TileCollisionType.EndSlippery:
                    return Unity_MapCollisionTypeGraphic.Slippery_Hangable;

                case GBA_TileCollisionType.Climb:
                    return Unity_MapCollisionTypeGraphic.Climb_Full;
                case GBA_TileCollisionType.Hang:
                    return Unity_MapCollisionTypeGraphic.Climb_Hang;

                case GBA_TileCollisionType.ClimbableWalls:
                    return Unity_MapCollisionTypeGraphic.Climb_Walls;

                case GBA_TileCollisionType.Climb_Spider_51:
                case GBA_TileCollisionType.Climb_Spider_52:
                case GBA_TileCollisionType.Climb_Spider_53:
                case GBA_TileCollisionType.Climb_Spider_54:
                    return Unity_MapCollisionTypeGraphic.Climb_Spider;

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
                    return Unity_MapCollisionTypeGraphic.EnemyDirection_Left;
                case GBA_TileCollisionType.EnemyTrigger_Right:
                    return Unity_MapCollisionTypeGraphic.EnemyDirection_Right;
                case GBA_TileCollisionType.EnemyTrigger_Up:
                    return Unity_MapCollisionTypeGraphic.EnemyDirection_Up;
                case GBA_TileCollisionType.EnemyTrigger_Down:
                    return Unity_MapCollisionTypeGraphic.EnemyDirection_Down;

                case GBA_TileCollisionType.Reactionary_Turn_45CounterClockwise:
                    return Unity_MapCollisionTypeGraphic.Rotate_CounterClockwise45; 
                case GBA_TileCollisionType.Reactionary_Turn_90CounterClockwise:
                    return Unity_MapCollisionTypeGraphic.Rotate_CounterClockwise90; 
                case GBA_TileCollisionType.Reactionary_Turn_90Clockwise:
                    return Unity_MapCollisionTypeGraphic.Rotate_Clockwise90; 
                case GBA_TileCollisionType.Reactionary_Turn_45Clockwise:
                    return Unity_MapCollisionTypeGraphic.Rotate_Clockwise45; 

                case GBA_TileCollisionType.AutoJump:
                    return Unity_MapCollisionTypeGraphic.Bounce;

                case GBA_TileCollisionType.Water:
                    return Unity_MapCollisionTypeGraphic.Water;

                case GBA_TileCollisionType.Lava:
                    return Unity_MapCollisionTypeGraphic.Lava;

                case GBA_TileCollisionType.InstaKill:
                    return Unity_MapCollisionTypeGraphic.Spikes;

                case GBA_TileCollisionType.Cliff:
                    return Unity_MapCollisionTypeGraphic.Cliff;

                default:
                    return Unity_MapCollisionTypeGraphic.Unknown0;
            }
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBA_Mode7TileCollisionType collisionType)
        {
            switch (collisionType)
            {
                case GBA_Mode7TileCollisionType.Empty:
                    return Unity_MapCollisionTypeGraphic.None;

                case GBA_Mode7TileCollisionType.Damage:
                    return Unity_MapCollisionTypeGraphic.Damage;

                case GBA_Mode7TileCollisionType.Solid:
                    return Unity_MapCollisionTypeGraphic.Solid;

                case GBA_Mode7TileCollisionType.Direction_Down:
                    return Unity_MapCollisionTypeGraphic.Direction_Down;
                case GBA_Mode7TileCollisionType.Direction_DownLeft:
                    return Unity_MapCollisionTypeGraphic.Direction_DownLeft;
                case GBA_Mode7TileCollisionType.Direction_DownRight:
                    return Unity_MapCollisionTypeGraphic.Direction_DownRight;
                case GBA_Mode7TileCollisionType.Direction_Left:
                    return Unity_MapCollisionTypeGraphic.Direction_Left;
                case GBA_Mode7TileCollisionType.Direction_Right:
                    return Unity_MapCollisionTypeGraphic.Direction_Right;
                case GBA_Mode7TileCollisionType.Direction_Up:
                    return Unity_MapCollisionTypeGraphic.Direction_Up;
                case GBA_Mode7TileCollisionType.Direction_UpLeft:
                    return Unity_MapCollisionTypeGraphic.Direction_UpLeft;
                case GBA_Mode7TileCollisionType.Direction_UpRight:
                    return Unity_MapCollisionTypeGraphic.Direction_UpRight;

                case GBA_Mode7TileCollisionType.EnemyTrigger_Left:
                    return Unity_MapCollisionTypeGraphic.EnemyDirection_Left;
                case GBA_Mode7TileCollisionType.EnemyTrigger_Right:
                    return Unity_MapCollisionTypeGraphic.EnemyDirection_Right;
                case GBA_Mode7TileCollisionType.EnemyTrigger_Up:
                    return Unity_MapCollisionTypeGraphic.EnemyDirection_Up;
                case GBA_Mode7TileCollisionType.EnemyTrigger_Down:
                    return Unity_MapCollisionTypeGraphic.EnemyDirection_Down;
                case GBA_Mode7TileCollisionType.EnemyTrigger_UpLeft:
                case GBA_Mode7TileCollisionType.EnemyTrigger_DownLeft:
                case GBA_Mode7TileCollisionType.EnemyTrigger_DownRight:
                case GBA_Mode7TileCollisionType.EnemyTrigger_UpRight:
                    return Unity_MapCollisionTypeGraphic.Reactionary; // TODO: New graphic

                case GBA_Mode7TileCollisionType.Bounce:
                    return Unity_MapCollisionTypeGraphic.Bounce;

                case GBA_Mode7TileCollisionType.Bumper1:
                case GBA_Mode7TileCollisionType.Bumper2:
                    return Unity_MapCollisionTypeGraphic.Bounce; // TODO: New graphic

                case GBA_Mode7TileCollisionType.Damage_FinishLine:
                case GBA_Mode7TileCollisionType.FinishLine:
                    return Unity_MapCollisionTypeGraphic.Exit; // TODO: New graphic

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
                    return Unity_MapCollisionTypeGraphic.Solid_Hangable;

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

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBARRR_Mode7TileCollisionType collisionType)
        {
            switch (collisionType)
            {
                case GBARRR_Mode7TileCollisionType.Empty:
                    return Unity_MapCollisionTypeGraphic.None;

                case GBARRR_Mode7TileCollisionType.Solid:
                    return Unity_MapCollisionTypeGraphic.Solid;

                case GBARRR_Mode7TileCollisionType.Speed:
                    return Unity_MapCollisionTypeGraphic.Reactionary; // TODO: New graphic

                case GBARRR_Mode7TileCollisionType.Bounce:
                    return Unity_MapCollisionTypeGraphic.Bounce;

                case GBARRR_Mode7TileCollisionType.Damage:
                    return Unity_MapCollisionTypeGraphic.Spikes;

                case GBARRR_Mode7TileCollisionType.Slippery:
                    return Unity_MapCollisionTypeGraphic.Slippery;

                case GBARRR_Mode7TileCollisionType.SlowDown:
                    return Unity_MapCollisionTypeGraphic.Damage;

                case GBARRR_Mode7TileCollisionType.FinishLine1:
                case GBARRR_Mode7TileCollisionType.FinishLine2:
                case GBARRR_Mode7TileCollisionType.FinishLine3:
                    return Unity_MapCollisionTypeGraphic.Exit; // TODO: New graphic

                default:
                    return Unity_MapCollisionTypeGraphic.Unknown0;
            }
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBAIsometric_Spyro3_TileCollisionType2D collisionType)
        {
            switch (collisionType)
            {
                case GBAIsometric_Spyro3_TileCollisionType2D.Empty:
                    return Unity_MapCollisionTypeGraphic.None;

                case GBAIsometric_Spyro3_TileCollisionType2D.Solid:
                    return Unity_MapCollisionTypeGraphic.Solid;

                case GBAIsometric_Spyro3_TileCollisionType2D.Hidden: // TODO: New graphic
                    return Unity_MapCollisionTypeGraphic.Exit;

                case GBAIsometric_Spyro3_TileCollisionType2D.Hook:
                    return Unity_MapCollisionTypeGraphic.Climb_Full;

                case GBAIsometric_Spyro3_TileCollisionType2D.Damage:
                    return Unity_MapCollisionTypeGraphic.Damage;

                case GBAIsometric_Spyro3_TileCollisionType2D.SolidAngle1:
                    return Unity_MapCollisionTypeGraphic.Hill_Steep_Right;
                case GBAIsometric_Spyro3_TileCollisionType2D.SolidAngle2:
                    return Unity_MapCollisionTypeGraphic.Hill_Steep_Left;
                case GBAIsometric_Spyro3_TileCollisionType2D.SolidAngle3:
                    return Unity_MapCollisionTypeGraphic.Angle_Top_Right;
                case GBAIsometric_Spyro3_TileCollisionType2D.SolidAngle4:
                    return Unity_MapCollisionTypeGraphic.Angle_Top_Left;

                default:
                    return Unity_MapCollisionTypeGraphic.Unknown0;
            }
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBAIsometric_Spyro2_TileCollisionType2D collisionType) {
            switch (collisionType) {
                case GBAIsometric_Spyro2_TileCollisionType2D.Empty:
                    return Unity_MapCollisionTypeGraphic.None;

                case GBAIsometric_Spyro2_TileCollisionType2D.Damage:
                    return Unity_MapCollisionTypeGraphic.Damage;

                case GBAIsometric_Spyro2_TileCollisionType2D.PassThrough:
                    return Unity_MapCollisionTypeGraphic.Passthrough;

                case GBAIsometric_Spyro2_TileCollisionType2D.Solid:
                    return Unity_MapCollisionTypeGraphic.Solid;

                default:
                    return Unity_MapCollisionTypeGraphic.Unknown0;
            }
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBC_TileCollisionType collisionType) {
            switch (collisionType) 
            {
                case GBC_TileCollisionType.Empty: return Unity_MapCollisionTypeGraphic.None;
                case GBC_TileCollisionType.Solid: return Unity_MapCollisionTypeGraphic.Solid;
                case GBC_TileCollisionType.Passthrough: return Unity_MapCollisionTypeGraphic.Passthrough;
                case GBC_TileCollisionType.Slippery: return Unity_MapCollisionTypeGraphic.Slippery;
                case GBC_TileCollisionType.Steep_Left: return Unity_MapCollisionTypeGraphic.Hill_Steep_Left;
                case GBC_TileCollisionType.Steep_Right: return Unity_MapCollisionTypeGraphic.Hill_Steep_Right;
                case GBC_TileCollisionType.Hill_Left1: return Unity_MapCollisionTypeGraphic.Hill_Slight_Left_1;
                case GBC_TileCollisionType.Hill_Left2: return Unity_MapCollisionTypeGraphic.Hill_Slight_Left_2;
                case GBC_TileCollisionType.Hill_Right1: return Unity_MapCollisionTypeGraphic.Hill_Slight_Right_1;
                case GBC_TileCollisionType.Hill_Right2: return Unity_MapCollisionTypeGraphic.Hill_Slight_Right_2;
                case GBC_TileCollisionType.Slippery_Steep_Left: return Unity_MapCollisionTypeGraphic.Slippery_Steep_Left;
                case GBC_TileCollisionType.Slippery_Steep_Right: return Unity_MapCollisionTypeGraphic.Slippery_Steep_Right;
                case GBC_TileCollisionType.Slippery_Hill_Left1: return Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_1;
                case GBC_TileCollisionType.Slippery_Hill_Left2: return Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_2;
                case GBC_TileCollisionType.Slippery_Hill_Right1: return Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_1;
                case GBC_TileCollisionType.Slippery_Hill_Right2: return Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_2;
                case GBC_TileCollisionType.Climb: return Unity_MapCollisionTypeGraphic.Climb;
                case GBC_TileCollisionType.Damage: return Unity_MapCollisionTypeGraphic.Damage;
                case GBC_TileCollisionType.InstaKill: return Unity_MapCollisionTypeGraphic.Spikes;
                case GBC_TileCollisionType.Pit: return Unity_MapCollisionTypeGraphic.Cliff;
                case GBC_TileCollisionType.Trigger_Right: return Unity_MapCollisionTypeGraphic.Direction_Right;
                case GBC_TileCollisionType.Trigger_Left: return Unity_MapCollisionTypeGraphic.Direction_Left;
                case GBC_TileCollisionType.Trigger_Up: return Unity_MapCollisionTypeGraphic.Direction_Up;
                case GBC_TileCollisionType.Trigger_Down: return Unity_MapCollisionTypeGraphic.Direction_Down;
                case GBC_TileCollisionType.Trigger_UpRight: return Unity_MapCollisionTypeGraphic.Direction_UpRight;
                case GBC_TileCollisionType.Trigger_UpLeft: return Unity_MapCollisionTypeGraphic.Direction_UpLeft;
                case GBC_TileCollisionType.Trigger_DownRight: return Unity_MapCollisionTypeGraphic.Direction_DownRight;
                case GBC_TileCollisionType.Trigger_DownLeft: return Unity_MapCollisionTypeGraphic.Direction_DownLeft;
                case GBC_TileCollisionType.Water: return Unity_MapCollisionTypeGraphic.Water;
                case GBC_TileCollisionType.Climb_Full: return Unity_MapCollisionTypeGraphic.Climb_Full;
                default: return Unity_MapCollisionTypeGraphic.Unknown0;
            }
        }
    }
}