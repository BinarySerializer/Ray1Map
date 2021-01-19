using System;
using UnityEngine;

namespace R1Engine
{
    public static class GBACrash_ObjInit
    {
        public static void InitObj(EngineVersion engineVersion, Unity_Object_GBACrash obj)
        {
            if (engineVersion == EngineVersion.GBACrash_Crash2)
            {
                switch ((GBACrash_Crash2_ObjType)obj.Object.ObjType)
                {
                    case GBACrash_Crash2_ObjType.Crash:
                        switch (obj.ObjManager.MapInfo.MapType)
                        {
                            default:
                            case GBACrash_MapInfo.GBACrash_MapType.Normal:
                                obj.AnimSetIndex = 0;
                                obj.AnimIndex = 18;
                                break;
                            case GBACrash_MapInfo.GBACrash_MapType.Normal_Vehicle_0:
                                obj.AnimSetIndex = 1;
                                obj.AnimIndex = 0;
                                break;
                            case GBACrash_MapInfo.GBACrash_MapType.Normal_Vehicle_1:
                                obj.AnimSetIndex = 2;
                                obj.AnimIndex = 0;
                                break;
                        }
                        break;
                    case GBACrash_Crash2_ObjType.Type_1:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Wumpa:
                        obj.AnimSetIndex = 0x1B;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash2_ObjType.Crystal:
                        obj.AnimSetIndex = 29;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash2_ObjType.Type_4:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_5:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_6:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_7:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_8:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.TimeTrialClock:
                        obj.AnimSetIndex = 28;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash2_ObjType.Type_10:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_11:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_12:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Crate_Normal:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 31;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_Checkpoint:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 26;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_AkuAku:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 23;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_Switch:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 3;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_Up:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 24;
                        obj.FreezeFrame = true;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_Outline:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 21;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_NitroSwitch:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 4;
                        obj.FreezeFrame = true;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_Iron:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 32;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_IronUp:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 2;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_Life:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 28;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_Nitro:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 5;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_QuestionMark:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_Bounce:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 25;
                        obj.FreezeFrame = true;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_Locked:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 6;
                        obj.FreezeFrame = true;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_TNT:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 17;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_Slot:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 7;
                        break;
                    case GBACrash_Crash2_ObjType.Type_29:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_30:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_31:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_32:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Platform_Horizontal_33:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 7;
                        break;
                    case GBACrash_Crash2_ObjType.Type_34:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_35:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_36:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_37:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Exit_Level:
                        obj.AnimSetIndex = 39;
                        obj.AnimIndex = 2;
                        break;
                    case GBACrash_Crash2_ObjType.Exit_Bonus:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash2_ObjType.Type_40:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_41:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_42:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_43:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_44:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Platform_Vertical_45:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 7;
                        break;
                    case GBACrash_Crash2_ObjType.Platform_Bouncy_46:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 4;
                        obj.FreezeFrame = true;
                        break;
                    case GBACrash_Crash2_ObjType.Platform_Bouncy_47:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 5;
                        obj.FreezeFrame = true;
                        break;
                    case GBACrash_Crash2_ObjType.Platform_Bouncy_48:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 6;
                        obj.FreezeFrame = true;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_49:
                        obj.AnimSetIndex = 37;
                        obj.AnimIndex = 7;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_FlyingCarpet:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 38;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_51:
                        obj.AnimSetIndex = 3;
                        obj.AnimIndex = 2;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_52:
                        obj.AnimSetIndex = 4;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_53:
                        obj.AnimSetIndex = 5;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash2_ObjType.Type_54:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_55:
                        obj.AnimSetIndex = 7;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_56:
                        obj.AnimSetIndex = 6;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_57:
                        obj.AnimSetIndex = 8;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_58:
                        obj.AnimSetIndex = 8;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_59:
                        obj.AnimSetIndex = 10;
                        obj.AnimIndex = 1;
                        obj.IsLinked = true;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_60:
                        obj.AnimSetIndex = 11;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_61:
                        obj.AnimSetIndex = 12;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_62:
                        obj.AnimSetIndex = 13;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_63:
                        obj.AnimSetIndex = 15;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_64:
                        obj.AnimSetIndex = 16;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_65:
                        obj.AnimSetIndex = 17;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_66:
                        obj.AnimSetIndex = 14;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash2_ObjType.Door_67:
                        obj.AnimSetIndex = 37;
                        obj.AnimIndex = 1;
                        obj.IsLinked = true;
                        break;
                    case GBACrash_Crash2_ObjType.Switch_68:
                        obj.AnimSetIndex = 37;
                        obj.AnimIndex = 0;
                        obj.FreezeFrame = true;
                        obj.IsLinked = true;
                        break;
                    case GBACrash_Crash2_ObjType.Platform_69:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 8;
                        break;
                    case GBACrash_Crash2_ObjType.Type_70:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Trigger_Water:
                        obj.AnimSetIndex = -1;
                        break;
                    case GBACrash_Crash2_ObjType.Trigger_RisingWater:
                        obj.AnimSetIndex = -1;
                        break;
                    case GBACrash_Crash2_ObjType.Block:
                        obj.AnimSetIndex = 37;
                        obj.AnimIndex = 4;
                        obj.IsLinked = true;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_74:
                        obj.AnimSetIndex = 37;
                        obj.AnimIndex = 3;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_CopterPack:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 39;
                        break;
                    case GBACrash_Crash2_ObjType.Trigger_ExitVehicle:
                        obj.AnimSetIndex = -1;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_77:
                        obj.AnimSetIndex = 18;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_78:
                        obj.AnimSetIndex = 21;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash2_ObjType.Type_79:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_80:
                        obj.AnimSetIndex = 20;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_81:
                        obj.AnimSetIndex = 19;
                        obj.AnimIndex = 2;
                        break;
                    case GBACrash_Crash2_ObjType.Type_82:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Trigger_Direction:
                        obj.AnimSetIndex = -1;
                        break;
                    case GBACrash_Crash2_ObjType.Type_84:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.BouncyPlatform_85:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 10;
                        obj.FreezeFrame = true;
                        break;
                    case GBACrash_Crash2_ObjType.Type_86:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Type_87:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_Crash2_ObjType.Platform_HorizontalTriggered_88:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 9;
                        break;
                    case GBACrash_Crash2_ObjType.Boss_FakeCrash: // TODO: Should use a different palette
                        obj.AnimSetIndex = 0;
                        obj.AnimIndex = 18;
                        break;
                    case GBACrash_Crash2_ObjType.Platform_Horizontal_90:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 9;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_91:
                        obj.AnimSetIndex = 22;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash2_ObjType.Rocket:
                        obj.AnimSetIndex = 39;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash2_ObjType.Door_93:
                        obj.AnimSetIndex = 37;
                        obj.AnimIndex = 6;
                        obj.IsLinked = true;
                        break;
                    case GBACrash_Crash2_ObjType.Trigger_Door:
                        obj.AnimSetIndex = -1;
                        obj.IsLinked = true;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_IronCheckpoint:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 40;
                        break;
                    case GBACrash_Crash2_ObjType.Boss_EvilCrunch:
                        obj.AnimSetIndex = 42;
                        obj.AnimIndex = 4;
                        break;
                    case GBACrash_Crash2_ObjType.Boss_EvilCoco:
                        obj.AnimSetIndex = 43;
                        obj.AnimIndex = 7;
                        break;
                    case GBACrash_Crash2_ObjType.Boss_NTrance:
                        obj.AnimSetIndex = 44;
                        obj.AnimIndex = 12;
                        break;
                    case GBACrash_Crash2_ObjType.GemShard_Red:
                        obj.AnimSetIndex = 24;
                        obj.AnimIndex = 3;
                        break;
                    case GBACrash_Crash2_ObjType.GemShard_Green:
                        obj.AnimSetIndex = 24;
                        obj.AnimIndex = 2;
                        break;
                    case GBACrash_Crash2_ObjType.GemShard_Blue:
                        obj.AnimSetIndex = 24;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash2_ObjType.Crate_Freeze:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 42;
                        obj.FreezeFrame = true;
                        break;
                    case GBACrash_Crash2_ObjType.Boss_NTropy:
                        obj.AnimSetIndex = 45;
                        obj.AnimIndex = 8;
                        break;
                    case GBACrash_Crash2_ObjType.Platform_104:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 3;
                        break;
                    case GBACrash_Crash2_ObjType.TutorialMessage:
                        obj.AnimSetIndex = 30;
                        obj.AnimIndex = 2;
                        break;
                    case GBACrash_Crash2_ObjType.Trigger_Cutscene:
                        obj.AnimSetIndex = -1;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_107:
                        obj.AnimSetIndex = 22;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash2_ObjType.Enemy_108:
                        obj.AnimSetIndex = 9;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash2_ObjType.FakeCrash:
                        obj.AnimSetIndex = 0;
                        obj.AnimIndex = 58;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (engineVersion == EngineVersion.GBACrash_Crash1)
            {
                switch ((GBACrash_Crash1_ObjType)obj.Object.ObjType)
                {
                    case GBACrash_Crash1_ObjType.Crash:
                        obj.AnimSetIndex = 0;
                        obj.AnimIndex = 18;
                        break;
                    case GBACrash_Crash1_ObjType.Type_1:
                        break;
                    case GBACrash_Crash1_ObjType.Crash_Underwater:
                        obj.AnimSetIndex = 1;
                        obj.AnimIndex = 31;
                        break;
                    case GBACrash_Crash1_ObjType.Type_3:
                        break;
                    case GBACrash_Crash1_ObjType.Type_4:
                        break;
                    case GBACrash_Crash1_ObjType.Type_5:
                        break;
                    case GBACrash_Crash1_ObjType.Wumpa:
                        obj.AnimSetIndex = 35;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash1_ObjType.Crystal:
                        obj.AnimSetIndex = 37;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash1_ObjType.Gem_Clear:
                        obj.AnimSetIndex = 32;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash1_ObjType.Type_9:
                        break;
                    case GBACrash_Crash1_ObjType.Type_10:
                        break;
                    case GBACrash_Crash1_ObjType.Type_11:
                        break;
                    case GBACrash_Crash1_ObjType.Type_12:
                        break;
                    case GBACrash_Crash1_ObjType.Type_13:
                        break;
                    case GBACrash_Crash1_ObjType.Type_14:
                        break;
                    case GBACrash_Crash1_ObjType.Type_15:
                        break;
                    case GBACrash_Crash1_ObjType.TimeTrialClock:
                        obj.AnimSetIndex = 36;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash1_ObjType.Type_17:
                        break;
                    case GBACrash_Crash1_ObjType.Type_18:
                        break;
                    case GBACrash_Crash1_ObjType.Type_19:
                        break;
                    case GBACrash_Crash1_ObjType.Type_20:
                        break;
                    case GBACrash_Crash1_ObjType.Crate_Normal:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 22;
                        break;
                    case GBACrash_Crash1_ObjType.Crate_Checkpoint:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 26;
                        break;
                    case GBACrash_Crash1_ObjType.Crate_AkuAku:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 23;
                        break;
                    case GBACrash_Crash1_ObjType.Crate_Switch:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 3;
                        break;
                    case GBACrash_Crash1_ObjType.Crate_Up:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 24;
                        obj.FreezeFrame = true;
                        break;
                    case GBACrash_Crash1_ObjType.Crate_Outline:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 21;
                        break;
                    case GBACrash_Crash1_ObjType.Crate_NitroSwitch:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 4;
                        obj.FreezeFrame = true;
                        break;
                    case GBACrash_Crash1_ObjType.Crate_Iron:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 32;
                        break;
                    case GBACrash_Crash1_ObjType.Crate_IronUp:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 2;
                        break;
                    case GBACrash_Crash1_ObjType.Crate_Life:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 28;
                        break;
                    case GBACrash_Crash1_ObjType.Crate_Nitro:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 5;
                        break;
                    case GBACrash_Crash1_ObjType.Crate_QuestionMark:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash1_ObjType.Crate_Bounce:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 25;
                        obj.FreezeFrame = true;
                        break;
                    case GBACrash_Crash1_ObjType.Type_34:
                        break;
                    case GBACrash_Crash1_ObjType.Crate_TNT:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 17;
                        break;
                    case GBACrash_Crash1_ObjType.Crate_Slot:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 7;
                        break;
                    case GBACrash_Crash1_ObjType.Type_37:
                        break;
                    case GBACrash_Crash1_ObjType.Type_38:
                        break;
                    case GBACrash_Crash1_ObjType.Type_39:
                        break;
                    case GBACrash_Crash1_ObjType.Enemy_40:
                        obj.AnimSetIndex = 13;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash1_ObjType.Enemy_41:
                        obj.AnimSetIndex = 11;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash1_ObjType.Enemy_42:
                        obj.AnimSetIndex = 10;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash1_ObjType.Enemy_43:
                        obj.AnimSetIndex = 14;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash1_ObjType.Enemy_44:
                        obj.AnimSetIndex = 12;
                        obj.AnimIndex = 3;
                        break;
                    case GBACrash_Crash1_ObjType.Enemy_45:
                        obj.AnimSetIndex = 15;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash1_ObjType.Enemy_46:
                        obj.AnimSetIndex = 17;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash1_ObjType.Type_47:
                        break;
                    case GBACrash_Crash1_ObjType.Enemy_48:
                        obj.AnimSetIndex = 5;
                        obj.AnimIndex = 2;
                        break;
                    case GBACrash_Crash1_ObjType.Enemy_49:
                        obj.AnimSetIndex = 4;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash1_ObjType.Enemy_50:
                        obj.AnimSetIndex = 3;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash1_ObjType.Type_51:
                        break;
                    case GBACrash_Crash1_ObjType.Type_52:
                        break;
                    case GBACrash_Crash1_ObjType.Type_53:
                        break;
                    case GBACrash_Crash1_ObjType.Type_54:
                        break;
                    case GBACrash_Crash1_ObjType.Type_55:
                        break;
                    case GBACrash_Crash1_ObjType.Type_56:
                        break;
                    case GBACrash_Crash1_ObjType.Type_57:
                        break;
                    case GBACrash_Crash1_ObjType.Type_58:
                        break;
                    case GBACrash_Crash1_ObjType.Type_59:
                        break;
                    case GBACrash_Crash1_ObjType.Type_60:
                        break;
                    case GBACrash_Crash1_ObjType.Type_61:
                        break;
                    case GBACrash_Crash1_ObjType.Type_62:
                        break;
                    case GBACrash_Crash1_ObjType.Type_63:
                        break;
                    case GBACrash_Crash1_ObjType.Type_64:
                        break;
                    case GBACrash_Crash1_ObjType.Type_65:
                        break;
                    case GBACrash_Crash1_ObjType.Type_66:
                        break;
                    case GBACrash_Crash1_ObjType.Type_67:
                        break;
                    case GBACrash_Crash1_ObjType.Type_68:
                        break;
                    case GBACrash_Crash1_ObjType.Type_69:
                        break;
                    case GBACrash_Crash1_ObjType.Type_70:
                        break;
                    case GBACrash_Crash1_ObjType.Type_71:
                        break;
                    case GBACrash_Crash1_ObjType.Type_72:
                        break;
                    case GBACrash_Crash1_ObjType.Type_73:
                        break;
                    case GBACrash_Crash1_ObjType.Type_74:
                        break;
                    case GBACrash_Crash1_ObjType.Enemy_75:
                        obj.AnimSetIndex = 6;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash1_ObjType.Type_76:
                        break;
                    case GBACrash_Crash1_ObjType.Type_77:
                        break;
                    case GBACrash_Crash1_ObjType.Type_78:
                        break;
                    case GBACrash_Crash1_ObjType.Platform_Moving_79:
                        obj.AnimSetIndex = 39;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash1_ObjType.Type_80:
                        break;
                    case GBACrash_Crash1_ObjType.Exit_RedGem:
                        obj.AnimSetIndex = 39;
                        obj.AnimIndex = 11;
                        break;
                    case GBACrash_Crash1_ObjType.Type_82:
                        break;
                    case GBACrash_Crash1_ObjType.Type_83:
                        break;
                    case GBACrash_Crash1_ObjType.Type_84:
                        break;
                    case GBACrash_Crash1_ObjType.Exit_Level:
                        obj.AnimSetIndex = 39;
                        obj.AnimIndex = 4;
                        break;
                    case GBACrash_Crash1_ObjType.Exit_Bonus:
                        obj.AnimSetIndex = 39;
                        obj.AnimIndex = 5;
                        break;
                    case GBACrash_Crash1_ObjType.Type_87:
                        break;
                    case GBACrash_Crash1_ObjType.Type_88:
                        break;
                    case GBACrash_Crash1_ObjType.Gem:
                        obj.AnimSetIndex = 32;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_Crash1_ObjType.Flame:
                        obj.AnimSetIndex = 44;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_Crash1_ObjType.Seaweed:
                        obj.AnimSetIndex = 45;
                        obj.AnimIndex = 0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (obj.AnimSetIndex == 0 && obj.AnimIndex == 0)
                    Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
            }
        }
    }
}