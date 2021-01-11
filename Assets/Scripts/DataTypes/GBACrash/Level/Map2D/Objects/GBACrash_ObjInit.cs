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
                switch (obj.Object.ObjType)
                {
                    case GBACrash_ObjType.Crash:
                        obj.AnimSetIndex = 0;
                        obj.AnimIndex = 18;
                        break;
                    case GBACrash_ObjType.Type_1:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Wumpa:
                        obj.AnimSetIndex = 0x1B;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_ObjType.Crystal:
                        obj.AnimSetIndex = 29;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_ObjType.Type_4:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_5:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_6:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_7:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_8:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.TimeTrialClock:
                        obj.AnimSetIndex = 28;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_ObjType.Type_10:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_11:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_12:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Crate_Normal:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 31;
                        break;
                    case GBACrash_ObjType.Crate_Checkpoint:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 26;
                        break;
                    case GBACrash_ObjType.Crate_AkuAku:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 23;
                        break;
                    case GBACrash_ObjType.Type_16:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_17:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_18:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Crate_NitroSwitch:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 4;
                        break;
                    case GBACrash_ObjType.Crate_Iron:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 32;
                        break;
                    case GBACrash_ObjType.Crate_IronUp:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 2;
                        break;
                    case GBACrash_ObjType.Crate_Life:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 28;
                        break;
                    case GBACrash_ObjType.Crate_Nitro:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 5;
                        break;
                    case GBACrash_ObjType.Crate_QuestionMark:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_ObjType.Crate_Bounce:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 25;
                        break;
                    case GBACrash_ObjType.Crate_Locked:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 6;
                        break;
                    case GBACrash_ObjType.Crate_TNT:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 17;
                        break;
                    case GBACrash_ObjType.Crate_Slot:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 7;
                        break;
                    case GBACrash_ObjType.Type_29:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_30:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_31:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_32:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Platform_0:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 7;
                        break;
                    case GBACrash_ObjType.Type_34:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_35:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_36:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_37:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Exit_Level:
                        obj.AnimSetIndex = 39;
                        obj.AnimIndex = 2;
                        break;
                    case GBACrash_ObjType.Exit_Bonus:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_ObjType.Type_40:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_41:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_42:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_43:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_44:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_45:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.BouncyPlatform_46:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 4;
                        break;
                    case GBACrash_ObjType.BouncyPlatform_47:
                        obj.AnimSetIndex = 31;
                        obj.AnimIndex = 5;
                        break;
                    case GBACrash_ObjType.Type_48:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_49:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_50:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_51:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_52:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Enemy_53:
                        obj.AnimSetIndex = 5;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_ObjType.Type_54:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_55:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_56:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_57:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Enemy_58:
                        obj.AnimSetIndex = 8;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_ObjType.Type_59:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_60:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_61:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_62:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_63:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_64:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_65:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_66:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_67:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_68:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_69:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_70:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_71:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_72:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_73:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_74:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_75:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_76:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_77:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_78:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_79:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_80:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_81:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_82:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_83:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_84:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_85:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_86:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_87:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_88:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_89:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_90:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_91:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_92:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_93:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_94:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_95:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_96:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_97:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_98:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_99:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.GemShard_Green:
                        obj.AnimSetIndex = 24;
                        obj.AnimIndex = 2;
                        break;
                    case GBACrash_ObjType.Type_101:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_102:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_103:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_104:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.TutorialMessage:
                        obj.AnimSetIndex = 30;
                        obj.AnimIndex = 2;
                        break;
                    case GBACrash_ObjType.Type_106:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}"); // TODO: level 0
                        break;
                    case GBACrash_ObjType.Type_107:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_108:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjType.Type_109:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}