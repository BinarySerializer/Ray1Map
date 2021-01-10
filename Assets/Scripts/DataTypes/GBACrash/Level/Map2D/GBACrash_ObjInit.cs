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
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Crash:
                        obj.AnimSetIndex = 0;
                        obj.AnimIndex = 18;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_1:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Wumpa:
                        obj.AnimSetIndex = 0x1B;
                        obj.AnimIndex = 1;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_3:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_4:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_5:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_6:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_7:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_8:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.TimeTrialClock:
                        obj.AnimSetIndex = 28;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_10:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_11:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_12:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Crate_Normal:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 31;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Crate_Checkpoint:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 26;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Crate_AkuAku:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 23;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_16:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_17:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_18:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Crate_NitroSwitch:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 4;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Crate_Iron:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 32;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Crate_IronUp:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 2;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Crate_Life:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 28;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Crate_Nitro:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 5;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Crate_QuestionMark:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 0;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Crate_Bounce:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 25;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Crate_Locked:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 6;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Crate_TNT:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 17;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Crate_Slot:
                        obj.AnimSetIndex = 23;
                        obj.AnimIndex = 7;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_29:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_30:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_31:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_32:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_33:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_34:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_35:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_36:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_37:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.LevelExit:
                        obj.AnimSetIndex = 39;
                        obj.AnimIndex = 2;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_39:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_40:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_41:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_42:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_43:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_44:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_45:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_46:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_47:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_48:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_49:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_50:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_51:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_52:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_53:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_54:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_55:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_56:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_57:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_58:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_59:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_60:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_61:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_62:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_63:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_64:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_65:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_66:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_67:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_68:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_69:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_70:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_71:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_72:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_73:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_74:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_75:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_76:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_77:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_78:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_79:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_80:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_81:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_82:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_83:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_84:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_85:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_86:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_87:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_88:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_89:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_90:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_91:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_92:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_93:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_94:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_95:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_96:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_97:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_98:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_99:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_100:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_101:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_102:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_103:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_104:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.TutorialMessage:
                        obj.AnimSetIndex = 30;
                        obj.AnimIndex = 2;
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_106:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_107:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_108:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    case GBACrash_ObjData.ObjGroup.Object.ObjectType.Type_109:
                        Debug.LogWarning($"Not implemented for type: {obj.Object.ObjType}");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}