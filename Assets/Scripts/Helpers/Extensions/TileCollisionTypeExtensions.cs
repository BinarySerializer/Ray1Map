using BinarySerializer.Ray1;
using BinarySerializer.Ray1.Jaguar;
using Ray1Map.Gameloft;
using Ray1Map.GBAIsometric;
using Ray1Map.GBARRR;
using Ray1Map.GBAVV;
using Ray1Map.GBC;

namespace Ray1Map
{
    public static class TileCollisionTypeExtensions
    {
        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this BlockType collisionType)
        {
            return collisionType switch
            {
                BlockType.None => Unity_MapCollisionTypeGraphic.None,
                BlockType.ChangeDirection => Unity_MapCollisionTypeGraphic.Reactionary,
                BlockType.Solid_Right_45 => Unity_MapCollisionTypeGraphic.Hill_Steep_Left,
                BlockType.Solid_Left_45 => Unity_MapCollisionTypeGraphic.Hill_Steep_Right,
                BlockType.Solid_Right1_30 => Unity_MapCollisionTypeGraphic.Hill_Slight_Left_1,
                BlockType.Solid_Right2_30 => Unity_MapCollisionTypeGraphic.Hill_Slight_Left_2,
                BlockType.Solid_Left1_30 => Unity_MapCollisionTypeGraphic.Hill_Slight_Right_2,
                BlockType.Solid_Left2_30 => Unity_MapCollisionTypeGraphic.Hill_Slight_Right_1,
                BlockType.Damage => Unity_MapCollisionTypeGraphic.Damage,
                BlockType.Bounce => Unity_MapCollisionTypeGraphic.Bounce,
                BlockType.Water => Unity_MapCollisionTypeGraphic.Water,
                BlockType.Exit => Unity_MapCollisionTypeGraphic.Exit,
                BlockType.Climb => Unity_MapCollisionTypeGraphic.Climb,
                BlockType.WaterNoSplash => Unity_MapCollisionTypeGraphic.Water_NoSplash,
                BlockType.Passthrough => Unity_MapCollisionTypeGraphic.Passthrough,
                BlockType.Solid => Unity_MapCollisionTypeGraphic.Solid,
                BlockType.Seed => Unity_MapCollisionTypeGraphic.Seed,
                BlockType.Slippery_Right_45 => Unity_MapCollisionTypeGraphic.Slippery_Steep_Left,
                BlockType.Slippery_Left_45 => Unity_MapCollisionTypeGraphic.Slippery_Steep_Right,
                BlockType.Slippery_Right1_30 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_1,
                BlockType.Slippery_Right2_30 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_2,
                BlockType.Slippery_Left1_30 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_2,
                BlockType.Slippery_Left2_30 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_1,
                BlockType.Spikes => Unity_MapCollisionTypeGraphic.Spikes,
                BlockType.Cliff => Unity_MapCollisionTypeGraphic.Cliff,
                BlockType.Slippery => Unity_MapCollisionTypeGraphic.Slippery,
                _ => Unity_MapCollisionTypeGraphic.Unknown0
            };
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this JAG_BlockType collisionType)
        {
            return collisionType switch
            {
                JAG_BlockType.None => Unity_MapCollisionTypeGraphic.None,
                JAG_BlockType.Reactionary => Unity_MapCollisionTypeGraphic.Reactionary,
                JAG_BlockType.Hill_Steep_Left => Unity_MapCollisionTypeGraphic.Hill_Steep_Left,
                JAG_BlockType.Hill_Steep_Right => Unity_MapCollisionTypeGraphic.Hill_Steep_Right,
                JAG_BlockType.Hill_Slight_Left_1 => Unity_MapCollisionTypeGraphic.Hill_Slight_Left_1,
                JAG_BlockType.Hill_Slight_Left_2 => Unity_MapCollisionTypeGraphic.Hill_Slight_Left_2,
                JAG_BlockType.Hill_Slight_Right_2 => Unity_MapCollisionTypeGraphic.Hill_Slight_Right_2,
                JAG_BlockType.Hill_Slight_Right_1 => Unity_MapCollisionTypeGraphic.Hill_Slight_Right_1,
                JAG_BlockType.Damage => Unity_MapCollisionTypeGraphic.Damage,
                JAG_BlockType.Bounce => Unity_MapCollisionTypeGraphic.Bounce,
                JAG_BlockType.Water => Unity_MapCollisionTypeGraphic.Water,
                JAG_BlockType.Climb => Unity_MapCollisionTypeGraphic.Climb,
                JAG_BlockType.PassthroughProto => Unity_MapCollisionTypeGraphic.Passthrough,
                JAG_BlockType.Passthrough => Unity_MapCollisionTypeGraphic.Passthrough,
                JAG_BlockType.Solid => Unity_MapCollisionTypeGraphic.Solid,
                JAG_BlockType.Spikes => Unity_MapCollisionTypeGraphic.Spikes,
                _ => Unity_MapCollisionTypeGraphic.Unknown0
            };
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this R2_TileCollisionType collisionType)
        {
            return collisionType switch
            {
                R2_TileCollisionType.None => Unity_MapCollisionTypeGraphic.None,
                R2_TileCollisionType.Direction_Left => Unity_MapCollisionTypeGraphic.Direction_Left,
                R2_TileCollisionType.Direction_Right => Unity_MapCollisionTypeGraphic.Direction_Right,
                R2_TileCollisionType.Direction_Up => Unity_MapCollisionTypeGraphic.Direction_Up,
                R2_TileCollisionType.Direction_Down => Unity_MapCollisionTypeGraphic.Direction_Down,
                R2_TileCollisionType.Direction_UpLeft => Unity_MapCollisionTypeGraphic.Direction_UpLeft,
                R2_TileCollisionType.Direction_UpRight => Unity_MapCollisionTypeGraphic.Direction_UpRight,
                R2_TileCollisionType.Direction_DownLeft => Unity_MapCollisionTypeGraphic.Direction_DownLeft,
                R2_TileCollisionType.Direction_DownRight => Unity_MapCollisionTypeGraphic.Direction_DownRight,
                R2_TileCollisionType.Unknown_11 => Unity_MapCollisionTypeGraphic.Reactionary,
                R2_TileCollisionType.Unknown_14 => Unity_MapCollisionTypeGraphic.Reactionary,
                R2_TileCollisionType.Cliff => Unity_MapCollisionTypeGraphic.Cliff,
                R2_TileCollisionType.Water => Unity_MapCollisionTypeGraphic.Water,
                R2_TileCollisionType.Solid => Unity_MapCollisionTypeGraphic.Solid,
                R2_TileCollisionType.Passthrough => Unity_MapCollisionTypeGraphic.Passthrough,
                R2_TileCollisionType.Hill_Slight_Left_1 => Unity_MapCollisionTypeGraphic.Hill_Slight_Left_1,
                R2_TileCollisionType.Hill_Slight_Left_2 => Unity_MapCollisionTypeGraphic.Hill_Slight_Left_2,
                R2_TileCollisionType.Hill_Steep_Left => Unity_MapCollisionTypeGraphic.Hill_Steep_Left,
                R2_TileCollisionType.Hill_Slight_Right_1 => Unity_MapCollisionTypeGraphic.Hill_Slight_Right_1,
                R2_TileCollisionType.Hill_Slight_Right_2 => Unity_MapCollisionTypeGraphic.Hill_Slight_Right_2,
                R2_TileCollisionType.Hill_Steep_Right => Unity_MapCollisionTypeGraphic.Hill_Steep_Right,
                R2_TileCollisionType.ReactionaryEnemy => Unity_MapCollisionTypeGraphic.Reactionary,
                R2_TileCollisionType.ReactionaryUnk => Unity_MapCollisionTypeGraphic.Reactionary,
                R2_TileCollisionType.ValidTarget => Unity_MapCollisionTypeGraphic.CannonTarget_Valid,
                R2_TileCollisionType.InvalidTarget => Unity_MapCollisionTypeGraphic.CannonTarget_Invalid,
                _ => Unity_MapCollisionTypeGraphic.Unknown0
            };
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBA_TileCollisionType collisionType, EngineVersion engineVersion)
        {
            return collisionType switch
            {
                GBA_TileCollisionType.Empty => Unity_MapCollisionTypeGraphic.None,
                GBA_TileCollisionType.Slippery => engineVersion > EngineVersion.GBA_BatmanVengeance
                    ? Unity_MapCollisionTypeGraphic.Slippery
                    : Unity_MapCollisionTypeGraphic.Solid,
                GBA_TileCollisionType.Damage => Unity_MapCollisionTypeGraphic.Damage,
                GBA_TileCollisionType.Ledge => Unity_MapCollisionTypeGraphic.Solid_Hangable,
                GBA_TileCollisionType.Passthrough => Unity_MapCollisionTypeGraphic.Passthrough,
                GBA_TileCollisionType.Solid => engineVersion > EngineVersion.GBA_BatmanVengeance
                    ? Unity_MapCollisionTypeGraphic.Solid
                    : Unity_MapCollisionTypeGraphic.None,
                GBA_TileCollisionType.Slippery_Ledge => Unity_MapCollisionTypeGraphic.Slippery_Hangable,
                GBA_TileCollisionType.Climb => Unity_MapCollisionTypeGraphic.Climb_Full,
                GBA_TileCollisionType.Hang => Unity_MapCollisionTypeGraphic.Climb_Hang,
                GBA_TileCollisionType.ClimbableWalls => Unity_MapCollisionTypeGraphic.Climb_Walls,
                GBA_TileCollisionType.Climb_Spider_51 => Unity_MapCollisionTypeGraphic.Climb_Spider,
                GBA_TileCollisionType.Climb_Spider_52 => Unity_MapCollisionTypeGraphic.Climb_Spider,
                GBA_TileCollisionType.Climb_Spider_53 => Unity_MapCollisionTypeGraphic.Climb_Spider,
                GBA_TileCollisionType.Climb_Spider_54 => Unity_MapCollisionTypeGraphic.Climb_Spider,
                GBA_TileCollisionType.Solid_Right_2 => Unity_MapCollisionTypeGraphic.Hill_Slight_Right_2,
                GBA_TileCollisionType.Solid_Right_1 => Unity_MapCollisionTypeGraphic.Hill_Slight_Right_1,
                GBA_TileCollisionType.Solid_Left_2 => Unity_MapCollisionTypeGraphic.Hill_Slight_Left_2,
                GBA_TileCollisionType.Solid_Left_1 => Unity_MapCollisionTypeGraphic.Hill_Slight_Left_1,
                GBA_TileCollisionType.Slippery_Right_2 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_2,
                GBA_TileCollisionType.Slippery_Right_1 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_1,
                GBA_TileCollisionType.Slippery_Left_2 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_2,
                GBA_TileCollisionType.Slippery_Left_1 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_1,
                GBA_TileCollisionType.Direction_Down => Unity_MapCollisionTypeGraphic.Direction_Down,
                GBA_TileCollisionType.Direction_DownLeft => Unity_MapCollisionTypeGraphic.Direction_DownLeft,
                GBA_TileCollisionType.Direction_DownRight => Unity_MapCollisionTypeGraphic.Direction_DownRight,
                GBA_TileCollisionType.Direction_Left => Unity_MapCollisionTypeGraphic.Direction_Left,
                GBA_TileCollisionType.Direction_Right => Unity_MapCollisionTypeGraphic.Direction_Right,
                GBA_TileCollisionType.Direction_Up => Unity_MapCollisionTypeGraphic.Direction_Up,
                GBA_TileCollisionType.Direction_UpLeft => Unity_MapCollisionTypeGraphic.Direction_UpLeft,
                GBA_TileCollisionType.Direction_UpRight => Unity_MapCollisionTypeGraphic.Direction_UpRight,
                GBA_TileCollisionType.EnemyTrigger_Left => Unity_MapCollisionTypeGraphic.EnemyDirection_Left,
                GBA_TileCollisionType.EnemyTrigger_Right => Unity_MapCollisionTypeGraphic.EnemyDirection_Right,
                GBA_TileCollisionType.EnemyTrigger_Up => Unity_MapCollisionTypeGraphic.EnemyDirection_Up,
                GBA_TileCollisionType.EnemyTrigger_Down => Unity_MapCollisionTypeGraphic.EnemyDirection_Down,
                GBA_TileCollisionType.Reactionary_Turn_45CounterClockwise => Unity_MapCollisionTypeGraphic
                    .Rotate_CounterClockwise45,
                GBA_TileCollisionType.Reactionary_Turn_90CounterClockwise => Unity_MapCollisionTypeGraphic
                    .Rotate_CounterClockwise90,
                GBA_TileCollisionType.Reactionary_Turn_90Clockwise => Unity_MapCollisionTypeGraphic.Rotate_Clockwise90,
                GBA_TileCollisionType.Reactionary_Turn_45Clockwise => Unity_MapCollisionTypeGraphic.Rotate_Clockwise45,
                GBA_TileCollisionType.AutoJump => Unity_MapCollisionTypeGraphic.Bounce,
                GBA_TileCollisionType.Water => Unity_MapCollisionTypeGraphic.Water,
                GBA_TileCollisionType.Lava => Unity_MapCollisionTypeGraphic.Lava,
                GBA_TileCollisionType.InstaKill => Unity_MapCollisionTypeGraphic.Spikes,
                GBA_TileCollisionType.Cliff => Unity_MapCollisionTypeGraphic.Cliff,
                _ => Unity_MapCollisionTypeGraphic.Unknown0
            };
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBA_Mode7TileCollisionType collisionType)
        {
            return collisionType switch
            {
                GBA_Mode7TileCollisionType.Empty => Unity_MapCollisionTypeGraphic.None,
                GBA_Mode7TileCollisionType.Damage => Unity_MapCollisionTypeGraphic.Damage,
                GBA_Mode7TileCollisionType.Solid => Unity_MapCollisionTypeGraphic.Solid,
                GBA_Mode7TileCollisionType.Direction_Down => Unity_MapCollisionTypeGraphic.Direction_Down,
                GBA_Mode7TileCollisionType.Direction_DownLeft => Unity_MapCollisionTypeGraphic.Direction_DownLeft,
                GBA_Mode7TileCollisionType.Direction_DownRight => Unity_MapCollisionTypeGraphic.Direction_DownRight,
                GBA_Mode7TileCollisionType.Direction_Left => Unity_MapCollisionTypeGraphic.Direction_Left,
                GBA_Mode7TileCollisionType.Direction_Right => Unity_MapCollisionTypeGraphic.Direction_Right,
                GBA_Mode7TileCollisionType.Direction_Up => Unity_MapCollisionTypeGraphic.Direction_Up,
                GBA_Mode7TileCollisionType.Direction_UpLeft => Unity_MapCollisionTypeGraphic.Direction_UpLeft,
                GBA_Mode7TileCollisionType.Direction_UpRight => Unity_MapCollisionTypeGraphic.Direction_UpRight,
                GBA_Mode7TileCollisionType.EnemyTrigger_Left => Unity_MapCollisionTypeGraphic.EnemyDirection_Left,
                GBA_Mode7TileCollisionType.EnemyTrigger_Right => Unity_MapCollisionTypeGraphic.EnemyDirection_Right,
                GBA_Mode7TileCollisionType.EnemyTrigger_Up => Unity_MapCollisionTypeGraphic.EnemyDirection_Up,
                GBA_Mode7TileCollisionType.EnemyTrigger_Down => Unity_MapCollisionTypeGraphic.EnemyDirection_Down,
                GBA_Mode7TileCollisionType.EnemyTrigger_UpLeft => Unity_MapCollisionTypeGraphic.EnemyDirection_UpLeft,
                GBA_Mode7TileCollisionType.EnemyTrigger_DownLeft => Unity_MapCollisionTypeGraphic
                    .EnemyDirection_DownLeft,
                GBA_Mode7TileCollisionType.EnemyTrigger_DownRight => Unity_MapCollisionTypeGraphic
                    .EnemyDirection_DownRight,
                GBA_Mode7TileCollisionType.EnemyTrigger_UpRight => Unity_MapCollisionTypeGraphic.EnemyDirection_UpRight,
                GBA_Mode7TileCollisionType.Bounce => Unity_MapCollisionTypeGraphic.Bounce,
                GBA_Mode7TileCollisionType.Bumper1 => Unity_MapCollisionTypeGraphic.Race_Bumper,
                GBA_Mode7TileCollisionType.Bumper2 => Unity_MapCollisionTypeGraphic.Race_Bumper,
                GBA_Mode7TileCollisionType.Damage_FinishLine => Unity_MapCollisionTypeGraphic.Race_Finish1,
                GBA_Mode7TileCollisionType.FinishLine => Unity_MapCollisionTypeGraphic.Race_Finish2,
                _ => Unity_MapCollisionTypeGraphic.Unknown0
            };
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBARRR_TileCollisionType collisionType)
        {
            return collisionType switch
            {
                GBARRR_TileCollisionType.Empty => Unity_MapCollisionTypeGraphic.None,
                GBARRR_TileCollisionType.Solid => Unity_MapCollisionTypeGraphic.Solid,
                GBARRR_TileCollisionType.Climb => Unity_MapCollisionTypeGraphic.Climb_Full,
                GBARRR_TileCollisionType.Hang => Unity_MapCollisionTypeGraphic.Climb_Hang,
                GBARRR_TileCollisionType.ClimbableWalls => Unity_MapCollisionTypeGraphic.Climb,
                GBARRR_TileCollisionType.SolidNoHang => Unity_MapCollisionTypeGraphic.Solid_NotHangable,
                GBARRR_TileCollisionType.Damage => Unity_MapCollisionTypeGraphic.Damage,
                GBARRR_TileCollisionType.PinObj => Unity_MapCollisionTypeGraphic.SpikePin,
                GBARRR_TileCollisionType.Trigger_Right1 => Unity_MapCollisionTypeGraphic.Direction_Right,
                GBARRR_TileCollisionType.Trigger_Right2 => Unity_MapCollisionTypeGraphic.Direction_Right,
                GBARRR_TileCollisionType.Trigger_Right3 => Unity_MapCollisionTypeGraphic.Direction_Right,
                GBARRR_TileCollisionType.Trigger_Left1 => Unity_MapCollisionTypeGraphic.Direction_Left,
                GBARRR_TileCollisionType.Trigger_Left2 => Unity_MapCollisionTypeGraphic.Direction_Left,
                GBARRR_TileCollisionType.Trigger_Left3 => Unity_MapCollisionTypeGraphic.Direction_Left,
                GBARRR_TileCollisionType.Trigger_Up1 => Unity_MapCollisionTypeGraphic.Direction_Up,
                GBARRR_TileCollisionType.Trigger_Up2 => Unity_MapCollisionTypeGraphic.Direction_Up,
                GBARRR_TileCollisionType.Trigger_Up3 => Unity_MapCollisionTypeGraphic.Direction_Up,
                GBARRR_TileCollisionType.Trigger_Down1 => Unity_MapCollisionTypeGraphic.Direction_Down,
                GBARRR_TileCollisionType.Trigger_Down2 => Unity_MapCollisionTypeGraphic.Direction_Down,
                GBARRR_TileCollisionType.Trigger_Down3 => Unity_MapCollisionTypeGraphic.Direction_Down,
                GBARRR_TileCollisionType.Trigger_Stop => Unity_MapCollisionTypeGraphic.Trigger_StopMovement,
                GBARRR_TileCollisionType.DetectionZone => Unity_MapCollisionTypeGraphic.DetectionZone,
                GBARRR_TileCollisionType.Solid_Left_1 => Unity_MapCollisionTypeGraphic.Hill_Slight_Left_1,
                GBARRR_TileCollisionType.Solid_Left_2 => Unity_MapCollisionTypeGraphic.Hill_Slight_Left_2,
                GBARRR_TileCollisionType.Solid_Right_2 => Unity_MapCollisionTypeGraphic.Hill_Slight_Right_2,
                GBARRR_TileCollisionType.Solid_Right_1 => Unity_MapCollisionTypeGraphic.Hill_Slight_Right_1,
                GBARRR_TileCollisionType.Solid_Left => Unity_MapCollisionTypeGraphic.Hill_Steep_Left,
                GBARRR_TileCollisionType.Solid_Right => Unity_MapCollisionTypeGraphic.Hill_Steep_Right,
                GBARRR_TileCollisionType.InstaKill => Unity_MapCollisionTypeGraphic.Spikes,
                GBARRR_TileCollisionType.Slippery_Left1 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_1,
                GBARRR_TileCollisionType.Slippery_Left2 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_2,
                GBARRR_TileCollisionType.Slippery_Right1 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_1,
                GBARRR_TileCollisionType.Slippery_Right2 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_2,
                GBARRR_TileCollisionType.Slippery_Left => Unity_MapCollisionTypeGraphic.Slippery_Steep_Left,
                GBARRR_TileCollisionType.Slippery_Right => Unity_MapCollisionTypeGraphic.Slippery_Steep_Right,
                GBARRR_TileCollisionType.Slippery => Unity_MapCollisionTypeGraphic.Slippery,
                _ => Unity_MapCollisionTypeGraphic.Unknown0
            };
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBARRR_Mode7TileCollisionType collisionType)
        {
            return collisionType switch
            {
                GBARRR_Mode7TileCollisionType.Empty => Unity_MapCollisionTypeGraphic.None,
                GBARRR_Mode7TileCollisionType.Solid => Unity_MapCollisionTypeGraphic.Solid,
                GBARRR_Mode7TileCollisionType.Speed => Unity_MapCollisionTypeGraphic.Race_SpeedUp,
                GBARRR_Mode7TileCollisionType.Bounce => Unity_MapCollisionTypeGraphic.Bounce,
                GBARRR_Mode7TileCollisionType.Damage => Unity_MapCollisionTypeGraphic.Spikes,
                GBARRR_Mode7TileCollisionType.Slippery => Unity_MapCollisionTypeGraphic.Slippery,
                GBARRR_Mode7TileCollisionType.SlowDown => Unity_MapCollisionTypeGraphic.Damage,
                GBARRR_Mode7TileCollisionType.FinishLine1 => Unity_MapCollisionTypeGraphic.Race_Finish1,
                GBARRR_Mode7TileCollisionType.FinishLine2 => Unity_MapCollisionTypeGraphic.Race_Finish2,
                GBARRR_Mode7TileCollisionType.FinishLine3 => Unity_MapCollisionTypeGraphic.Race_Finish3,
                _ => Unity_MapCollisionTypeGraphic.Unknown0
            };
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBAIsometric_Spyro3_TileCollisionType2D collisionType)
        {
            return collisionType switch
            {
                GBAIsometric_Spyro3_TileCollisionType2D.Empty => Unity_MapCollisionTypeGraphic.None,
                GBAIsometric_Spyro3_TileCollisionType2D.Solid => Unity_MapCollisionTypeGraphic.Solid,
                GBAIsometric_Spyro3_TileCollisionType2D.Hidden => Unity_MapCollisionTypeGraphic.DetectionZone,
                GBAIsometric_Spyro3_TileCollisionType2D.Hook => Unity_MapCollisionTypeGraphic.Climb_Full,
                GBAIsometric_Spyro3_TileCollisionType2D.Damage => Unity_MapCollisionTypeGraphic.Damage,
                GBAIsometric_Spyro3_TileCollisionType2D.SolidAngle1 => Unity_MapCollisionTypeGraphic.Hill_Steep_Right,
                GBAIsometric_Spyro3_TileCollisionType2D.SolidAngle2 => Unity_MapCollisionTypeGraphic.Hill_Steep_Left,
                GBAIsometric_Spyro3_TileCollisionType2D.SolidAngle3 => Unity_MapCollisionTypeGraphic.Angle_Top_Right,
                GBAIsometric_Spyro3_TileCollisionType2D.SolidAngle4 => Unity_MapCollisionTypeGraphic.Angle_Top_Left,
                _ => Unity_MapCollisionTypeGraphic.Unknown0
            };
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBAIsometric_Spyro2_TileCollisionType2D collisionType)
        {
            return collisionType switch
            {
                GBAIsometric_Spyro2_TileCollisionType2D.Empty => Unity_MapCollisionTypeGraphic.None,
                GBAIsometric_Spyro2_TileCollisionType2D.Damage => Unity_MapCollisionTypeGraphic.Damage,
                GBAIsometric_Spyro2_TileCollisionType2D.PassThrough => Unity_MapCollisionTypeGraphic.Passthrough,
                GBAIsometric_Spyro2_TileCollisionType2D.Solid => Unity_MapCollisionTypeGraphic.Solid,
                _ => Unity_MapCollisionTypeGraphic.Unknown0
            };
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBC_TileCollisionType collisionType)
        {
            return collisionType switch
            {
                GBC_TileCollisionType.Empty => Unity_MapCollisionTypeGraphic.None,
                GBC_TileCollisionType.Solid => Unity_MapCollisionTypeGraphic.Solid,
                GBC_TileCollisionType.Passthrough => Unity_MapCollisionTypeGraphic.Passthrough,
                GBC_TileCollisionType.Slippery => Unity_MapCollisionTypeGraphic.Slippery,
                GBC_TileCollisionType.Steep_Left => Unity_MapCollisionTypeGraphic.Hill_Steep_Left,
                GBC_TileCollisionType.Steep_Right => Unity_MapCollisionTypeGraphic.Hill_Steep_Right,
                GBC_TileCollisionType.Hill_Left1 => Unity_MapCollisionTypeGraphic.Hill_Slight_Left_1,
                GBC_TileCollisionType.Hill_Left2 => Unity_MapCollisionTypeGraphic.Hill_Slight_Left_2,
                GBC_TileCollisionType.Hill_Right1 => Unity_MapCollisionTypeGraphic.Hill_Slight_Right_1,
                GBC_TileCollisionType.Hill_Right2 => Unity_MapCollisionTypeGraphic.Hill_Slight_Right_2,
                GBC_TileCollisionType.Slippery_Steep_Left => Unity_MapCollisionTypeGraphic.Slippery_Steep_Left,
                GBC_TileCollisionType.Slippery_Steep_Right => Unity_MapCollisionTypeGraphic.Slippery_Steep_Right,
                GBC_TileCollisionType.Slippery_Hill_Left1 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_1,
                GBC_TileCollisionType.Slippery_Hill_Left2 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Left_2,
                GBC_TileCollisionType.Slippery_Hill_Right1 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_1,
                GBC_TileCollisionType.Slippery_Hill_Right2 => Unity_MapCollisionTypeGraphic.Slippery_Slight_Right_2,
                GBC_TileCollisionType.Climb => Unity_MapCollisionTypeGraphic.Climb,
                GBC_TileCollisionType.Damage => Unity_MapCollisionTypeGraphic.Damage,
                GBC_TileCollisionType.InstaKill => Unity_MapCollisionTypeGraphic.Spikes,
                GBC_TileCollisionType.Pit => Unity_MapCollisionTypeGraphic.Cliff,
                GBC_TileCollisionType.Trigger_Right => Unity_MapCollisionTypeGraphic.Direction_Right,
                GBC_TileCollisionType.Trigger_Left => Unity_MapCollisionTypeGraphic.Direction_Left,
                GBC_TileCollisionType.Trigger_Up => Unity_MapCollisionTypeGraphic.Direction_Up,
                GBC_TileCollisionType.Trigger_Down => Unity_MapCollisionTypeGraphic.Direction_Down,
                GBC_TileCollisionType.Trigger_UpRight => Unity_MapCollisionTypeGraphic.Direction_UpRight,
                GBC_TileCollisionType.Trigger_UpLeft => Unity_MapCollisionTypeGraphic.Direction_UpLeft,
                GBC_TileCollisionType.Trigger_DownRight => Unity_MapCollisionTypeGraphic.Direction_DownRight,
                GBC_TileCollisionType.Trigger_DownLeft => Unity_MapCollisionTypeGraphic.Direction_DownLeft,
                GBC_TileCollisionType.Water => Unity_MapCollisionTypeGraphic.Water,
                GBC_TileCollisionType.Climb_Full => Unity_MapCollisionTypeGraphic.Climb_Full,
                _ => Unity_MapCollisionTypeGraphic.Unknown0
            };
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBAVV_Map2D_CollisionType collisionType)
        {
            return collisionType switch
            {
                GBAVV_Map2D_CollisionType.Solid => Unity_MapCollisionTypeGraphic.Solid,
                GBAVV_Map2D_CollisionType.Damage => Unity_MapCollisionTypeGraphic.Damage,
                GBAVV_Map2D_CollisionType.Rails_Left => Unity_MapCollisionTypeGraphic.Direction_DownLeft,
                GBAVV_Map2D_CollisionType.Slippery => Unity_MapCollisionTypeGraphic.Slippery,
                GBAVV_Map2D_CollisionType.Hang => Unity_MapCollisionTypeGraphic.Climb_Hang,
                GBAVV_Map2D_CollisionType.Move_Left => Unity_MapCollisionTypeGraphic.Direction_Left,
                GBAVV_Map2D_CollisionType.Rails_Right => Unity_MapCollisionTypeGraphic.Direction_DownRight,
                GBAVV_Map2D_CollisionType.Rope => Unity_MapCollisionTypeGraphic.Climb,
                GBAVV_Map2D_CollisionType.Move_Right => Unity_MapCollisionTypeGraphic.Direction_Right,
                _ => Unity_MapCollisionTypeGraphic.Unknown0
            };
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this Gameloft_RRR_CollisionType collisionType)
        {
            return collisionType switch
            {
                Gameloft_RRR_CollisionType.None => Unity_MapCollisionTypeGraphic.None,
                Gameloft_RRR_CollisionType.Solid => Unity_MapCollisionTypeGraphic.Solid,
                Gameloft_RRR_CollisionType.Hangable => Unity_MapCollisionTypeGraphic.Solid_Hangable,
                _ => Unity_MapCollisionTypeGraphic.Unknown0
            };
        }

        public static Unity_MapCollisionTypeGraphic GetCollisionTypeGraphic(this GBAVV_NitroKart_CollisionType collisionType)
        {
            return collisionType switch
            {
                GBAVV_NitroKart_CollisionType.None => Unity_MapCollisionTypeGraphic.None,
                GBAVV_NitroKart_CollisionType.Solid_Full => Unity_MapCollisionTypeGraphic.Solid,
                GBAVV_NitroKart_CollisionType.Solid_Ground_2 => Unity_MapCollisionTypeGraphic.Passthrough,
                GBAVV_NitroKart_CollisionType.Solid_Ground_3 => Unity_MapCollisionTypeGraphic.Passthrough,
                GBAVV_NitroKart_CollisionType.Slow_7 => Unity_MapCollisionTypeGraphic.Direction_Down,
                GBAVV_NitroKart_CollisionType.Slow_8 => Unity_MapCollisionTypeGraphic.Direction_Down,
                GBAVV_NitroKart_CollisionType.Slow_9 => Unity_MapCollisionTypeGraphic.Direction_Down,
                GBAVV_NitroKart_CollisionType.Slow_10 => Unity_MapCollisionTypeGraphic.Direction_Down,
                GBAVV_NitroKart_CollisionType.Slow_11 => Unity_MapCollisionTypeGraphic.Direction_Down,
                GBAVV_NitroKart_CollisionType.Slow_12 => Unity_MapCollisionTypeGraphic.EnemyDirection_Down,
                GBAVV_NitroKart_CollisionType.Slow_13 => Unity_MapCollisionTypeGraphic.EnemyDirection_Down,
                GBAVV_NitroKart_CollisionType.Slow_14 => Unity_MapCollisionTypeGraphic.EnemyDirection_Down,
                GBAVV_NitroKart_CollisionType.Slow_15 => Unity_MapCollisionTypeGraphic.EnemyDirection_Down,
                GBAVV_NitroKart_CollisionType.Water => Unity_MapCollisionTypeGraphic.Water,
                GBAVV_NitroKart_CollisionType.Pit => Unity_MapCollisionTypeGraphic.Cliff,
                GBAVV_NitroKart_CollisionType.SpeedBoost => Unity_MapCollisionTypeGraphic.Race_SpeedUp,
                GBAVV_NitroKart_CollisionType.Jump => Unity_MapCollisionTypeGraphic.Bounce,
                _ => Unity_MapCollisionTypeGraphic.Unknown0
            };
        }
    }
}