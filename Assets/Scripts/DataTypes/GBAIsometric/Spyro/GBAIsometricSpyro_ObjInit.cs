using System;
using System.Linq;

namespace R1Engine
{
    public static class GBAIsometricSpyro_ObjInit
    {
        public static Action<Unity_Object_GBAIsometricSpyro, Unity_Object_GBAIsometricSpyro[]> GetInitFunc(GameSettings settings, uint address)
        {
            if (settings.GameModeSelection == GameModeSelection.SpyroAdventureUS)
            {
                switch (address)
                {
                    case 0x0801580D:
                    case 0x0801BB75:
                    case 0x0801C269:
                    case 0x08021509:
                    case 0x08023B7D:
                    case 0x08024F21:
                    case 0x080250C9:
                    case 0x08025C9D:
                    case 0x080292BD:
                    case 0x0802A805:
                    case 0x0802B271:
                    case 0x0802B779:
                    case 0x08031979:
                    case 0x08033DE1:
                    case 0x08037441:
                    case 0x080383BD:
                    case 0x08038ABD:
                    case 0x08038E11:
                    case 0x08039D51:
                    case 0x0803A02D:
                    case 0x0803BF91:
                    case 0x080404AD:
                    case 0x08041DA5:
                    case 0x08042725:
                    case 0x080430C5:
                    case 0x080448D1:
                    case 0x0801D1A5:
                    case 0x08045D5D:
                    case 0x08046945:
                    case 0x08046A15:
                    case 0x08046FC5:
                    case 0x080470A9:
                    case 0x08047101:
                    case 0x0804739D:
                    case 0x08047651:
                        return Spyro_NotImplemented;

                    case 0x080159BD: // Vertical collision
                    case 0x080236B1: // Screen change trigger
                    case 0x080159E1: // Obj spawner (spawns Spyro, baby dragons etc.)
                    case 0x08024BE5: // Some ice block trigger
                    case 0x080258D5:
                    case 0x08027F55: // Ice quest trigger
                    case 0x08032049: // Race controller (this starts the race and spawns the opponents)
                    case 0x0801B9BD: // ?
                    case 0x0801B499: // ?
                    case 0x080401C1: // Agent 9 pipe controller
                        return Spyro_EditorObj;

                    case 0x08015BBD: return Spyro3_0;
                    case 0x0801A20D: return Spyro3_1;
                    case 0x0801AC2D: return Spyro3_2;
                    case 0x0801B0B1: return Spyro3_3;
                    case 0x0801B5BD: return Spyro3_4;
                    case 0x0801C9A1: return Spyro3_5;
                    case 0x0801CFD5: return Spyro3_6;
                    case 0x0801D4B5: return Spyro3_7;
                    case 0x0801DD85: return Spyro3_8;
                    case 0x0801F48D: return Spyro3_9;
                    case 0x08020D39: return Spyro3_10;
                    case 0x08020EB9: return Spyro3_11;
                    case 0x0802102D: return Spyro3_12;
                    case 0x08021B21: return Spyro3_13;
                    case 0x08021E25: return Spyro3_14;
                    case 0x08022115: return Spyro3_15;
                    case 0x08022229: return Spyro3_16;
                    case 0x08023009: return Spyro3_17;
                    case 0x08023409: return Spyro3_18;
                    case 0x080237E1: return Spyro3_19;
                    case 0x080239E1: return Spyro3_20;
                    case 0x08024411: return Spyro3_21;
                    case 0x08024A35: return Spyro3_22;
                    case 0x0802549D: return Spyro3_23;
                    case 0x080255DD: return Spyro3_24;
                    case 0x08025A0D: return Spyro3_25;
                    case 0x080267C5: return Spyro3_26;
                    case 0x08026E8D: return Spyro3_27;
                    case 0x08027DC1: return Spyro3_28;
                    case 0x0801A565: return Spyro3_29;
                    case 0x080283B5: return Spyro3_30;
                    case 0x08028A31: return Spyro3_31;
                    case 0x0802B9CD: return Spyro3_32;
                    case 0x0802C081: return Spyro3_33;
                    case 0x0802C1C9: return Spyro3_34;
                    case 0x0802C3A1: return Spyro3_35;
                    case 0x0802CC65: return Spyro3_36;
                    case 0x0802F089: return Spyro3_37;
                    case 0x0802F8ED: return Spyro3_38;
                    case 0x0802FA39: return Spyro3_39;
                    case 0x08030225: return Spyro3_40;
                    case 0x080303CD: return Spyro3_41;
                    case 0x0803050D: return Spyro3_42;
                    case 0x08030BD1: return Spyro3_43;
                    case 0x08034765: return Spyro3_45;
                    case 0x0803497D: return Spyro3_46;
                    case 0x08035219: return Spyro3_47;
                    case 0x08035BA1: return Spyro3_48;
                    case 0x0803602D: return Spyro3_49;
                    case 0x08043A79: return Spyro3_50;
                    case 0x0801B259: return Spyro3_52;
                    case 0x08022785: return Spyro3_53;
                    case 0x0803F3FD: return Spyro3_54;
                    case 0x0801DA1D: return Spyro3_55;
                    case 0x0803CAE9: return Spyro3_56;
                    case 0x08034CD5: return Spyro3_57;
                    case 0x08043235: return Spyro3_58;
                    case 0x08020859: return Spyro3_59;
                    case 0x08028DCD: return Spyro3_60;
                    case 0x0803C43D: return Spyro3_61;
                    case 0x08027875: return Spyro3_62;
                    case 0x080370D1: return Spyro3_63;
                    case 0x08036471: return Spyro3_64;
                    case 0x08036319: return Spyro3_65;
                    case 0x0801F9FD: return Spyro3_66;
                    case 0x0803AED9: return Spyro3_67;
                    case 0x0801DC7D: return Spyro3_68;
                    case 0x0801E855: return Spyro3_69;
                    case 0x0801E1C1: return Spyro3_70;
                    case 0x08043E35: return Spyro3_71;
                    case 0x0802641D: return Spyro3_72;
                    case 0x0802C809: return Spyro3_73;
                    case 0x08026C91: return Spyro3_74;
                    case 0x0802BF4D: return Spyro3_75;
                    case 0x08032949: return Spyro3_76;
                    case 0x0802FDD5: return Spyro3_77;
                    case 0x0802825D: return Spyro3_78;
                    case 0x08030FE1: return Spyro3_79;
                    case 0x080307C1: return Spyro3_80;
                    case 0x0803A421: return Spyro3_81;
                    case 0x0802F65D: return Spyro3_82;
                    case 0x080384ED: return Spyro3_83;
                    case 0x080386E5: return Spyro3_84;
                    case 0x08028815: return Spyro3_85;
                    case 0x0803509D: return Spyro3_86;
                    case 0x0803EA1D: return Spyro3_87;
                    case 0x080345D5: return Spyro3_88;
                    case 0x08032DC5: return Spyro3_89;
                    case 0x0802B505: return Spyro3_90;
                    case 0x0803574D: return Spyro3_91;
                    case 0x0803F255: return Spyro3_92;
                    case 0x0803ED41: return Spyro3_93;
                    case 0x08040A61: return Spyro3_94;
                    case 0x0803AB49: return Spyro3_95;
                    case 0x0802756D: return Spyro3_96;
                    case 0x08031DDD: return Spyro3_97;
                    case 0x0803E925: return Spyro3_98;
                    case 0x080315B5: return Spyro3_99;
                    case 0x0803B981: return Spyro3_100;
                    case 0x08033339: return Spyro3_101;
                    case 0x0803B781: return Spyro3_102;
                    case 0x080465B1: return Spyro3_103;
                    case 0x08046475: return Spyro3_104;
                    case 0x0803FC61: return Spyro3_105;
                    case 0x0803FFB1: return Spyro3_106;
                    case 0x0803B3A5: return Spyro3_107;
                    case 0x0803BE0D: return Spyro3_108;
                    case 0x08043C59: return Spyro3_109;
                    case 0x0803B681: return Spyro3_110;
                    case 0x080438F1: return Spyro3_111;
                    case 0x0803C7FD: return Spyro3_112;
                    case 0x0803A785: return Spyro3_113;
                    case 0x0803A62D: return Spyro3_114;
                    case 0x0803A8A9: return Spyro3_115;
                    case 0x08042D99: return Spyro3_116;
                    case 0x080428BD: return Spyro3_117;
                    case 0x080424AD: return Spyro3_118;
                    case 0x08042585: return Spyro3_119;
                    case 0x0803966D: return Spyro3_120;

                    // Agent 9
                    case 0x08044AD5: return Spyro3_121;
                    case 0x080449B9: return Spyro3_122;
                    case 0x0804063D: return Spyro3_123;
                    case 0x0803D829: return Spyro3_124;
                    case 0x0803E6DD: return Spyro3_125;
                    case 0x0803DC59: return Spyro3_126;
                    case 0x08043EAD: return Spyro3_127;
                    case 0x08040DD9: return Spyro3_128;
                    case 0x08040925: return Spyro3_129;
                    case 0x0803E255: return Spyro3_130;
                    case 0x0803CB39: return Spyro3_131;
                    case 0x08040CB5: return Spyro3_132;
                    case 0x0803E559: return Spyro3_133;
                    case 0x0803F58D: return Spyro3_134;
                    case 0x08040571: return Spyro3_135;
                    case 0x080429F1: return Spyro3_136;
                    case 0x0803D0F1: return Spyro3_137;

                    // Sgt. Byrd
                    case 0x08045ED1: return Spyro3_138;
                    case 0x080454F5: return Spyro3_139;
                    case 0x080376A9: return Spyro3_140;
                    case 0x08044D81: return Spyro3_141;
                    case 0x08033541: return Spyro3_44;
                    case 0x08045659: return Spyro3_142;
                    case 0x080459B1: return Spyro3_143;
                    case 0x080338A5: return Spyro3_144;
                    case 0x08046161: return Spyro3_145;
                    case 0x08046271: return Spyro3_146;
                    case 0x080363F5: return Spyro3_147;
                    case 0x080450BD: return Spyro3_148;
                }
            }
            else if (settings.GameModeSelection == GameModeSelection.SpyroSeasonFlameUS)
            {
                switch (address)
                {
                    case 0x08010761:
                    case 0x08010EB5:
                    case 0x08012EA9:
                    case 0x08013EC5:
                    case 0x080149F1:
                    case 0x08014EA1:
                    case 0x080151DD:
                    case 0x08015429:
                    case 0x08015D51:
                    case 0x08016C41:
                    case 0x0801841D:
                    case 0x08018639:
                    case 0x0801ED71:
                    case 0x0801F40D:
                    case 0x0801F829:
                    case 0x08020145:
                    case 0x08020BF5:
                    case 0x080211BD:
                    case 0x0801DCB1:
                    case 0x08022E89:
                    case 0x08023AA1:
                    case 0x08023E89:
                    case 0x080251BD:
                    case 0x08025AFD:
                    case 0x080267A5:
                    case 0x080275B1:
                    case 0x08028025:
                    case 0x08026B01:
                    case 0x08028535:
                    case 0x08028929:
                    case 0x08028AA5:
                    case 0x080290B9:
                    case 0x08029395:
                    case 0x08029FE1:
                    case 0x0802B431:
                    case 0x0802BB9D:
                    case 0x0802BFD5:
                    case 0x0802C6A5:
                    case 0x0802CC6D:
                    case 0x0802D2CD:
                    case 0x0802D901:
                    case 0x0802E2B5:
                    case 0x0802E55D:
                    case 0x0802EA61:
                    case 0x0802ED69:
                    case 0x0802EFBD:
                    case 0x0802F335:
                    case 0x0802F575:
                    case 0x0802FAF5:
                    case 0x080303C5:
                    case 0x08030B21:
                    case 0x08031A0D:
                    case 0x0803240D:
                    case 0x08032799:
                    case 0x08032D41:
                    case 0x080332D1:
                    case 0x08034069:
                    case 0x0803464D:
                    case 0x08034C7D:
                    case 0x08034F39:
                    case 0x0803539D:
                    case 0x08035E61:
                    case 0x080368FD:
                    case 0x08036CB1:
                    case 0x08037005:
                    case 0x08037725:
                    case 0x08037FB5:
                    case 0x08038679:
                    case 0x080387E1:
                    case 0x08038929:
                    case 0x0803A58D:
                    case 0x0803AFD9:
                    case 0x0803B36D:
                    case 0x0803BBCD:
                    case 0x0803BE29:
                    case 0x0803C9B5:
                    case 0x08026E75:
                    case 0x0803D5C1:
                    case 0x0803D1F5:
                    case 0x0803DA05:
                    case 0x0803DE11:
                    case 0x0803C431:
                    case 0x0803C259:
                    case 0x0803C735:
                    case 0x0803DFB9:
                    case 0x0803EDB9:
                    case 0x08040811:
                    case 0x08040ACD:
                    case 0x08041145:
                        return Spyro_NotImplemented;

                    case 0x08036C8D: // Vertical collision
                    case 0x0801074D: // Spawner
                    case 0x0803F941: // Tutorial trigger
                    case 0x0803197D: // Side map area trigger
                    case 0x08041705: // Challenge object spawner
                        return Spyro_EditorObj;

                    case 0x0803E93D: return Spyro2_0;
                    case 0x0803E539: return Spyro2_1;
                    case 0x0803E30D: return Spyro2_2;
                    case 0x08013579: return Spyro2_3;
                    case 0x080132DD: return Spyro2_4;
                    case 0x0801551D: return Spyro2_5;
                    case 0x0803A9A1: return Spyro2_6;
                    case 0x08011EA5: return Spyro2_7;
                    case 0x0803F9F5: return Spyro2_8;
                    case 0x0803EF05: return Spyro2_9;
                    case 0x08012321: return Spyro2_10;
                    case 0x0803AC3D: return Spyro2_11;
                    case 0x0803FCB1: return Spyro2_12;
                    case 0x08034AC9: return Spyro2_13;
                    case 0x08042595: return Spyro2_14;
                    case 0x080102A9: return Spyro2_15;
                    case 0x080423A1: return Spyro2_16;
                    case 0x0803B62D: return Spyro2_17;
                    case 0x0803CB39: return Spyro2_18;
                    case 0x0803CE45: return Spyro2_19;
                    case 0x08040229: return Spyro2_20;
                    case 0x08013AA1: return Spyro2_21;
                    case 0x08016641: return Spyro2_22;
                    case 0x08017F31: return Spyro2_23;
                    case 0x080177DD: return Spyro2_24;
                    case 0x080383B5: return Spyro2_25;
                    case 0x0801811D: return Spyro2_26;
                    case 0x08010B59: return Spyro2_27;
                    case 0x0802099D: return Spyro2_28;
                    case 0x08014549: return Spyro2_29;
                    case 0x0802152D: return Spyro2_30;
                    case 0x0802A8BD: return Spyro2_31;
                    case 0x0802471D: return Spyro2_32;
                    case 0x0801E5A9: return Spyro2_33;
                    case 0x08041465: return Spyro2_34;
                    case 0x08021769: return Spyro2_35;
                    case 0x08021969: return Spyro2_36;
                    case 0x08029A5D: return Spyro2_37;
                    case 0x08021D39: return Spyro2_38;
                    case 0x08041D99: return Spyro2_39;
                    case 0x08012A51: return Spyro2_40;
                    case 0x08039BB5: return Spyro2_41;
                    case 0x08040E4D: return Spyro2_42;
                    case 0x080221E9: return Spyro2_43;
                    case 0x08021F65: return Spyro2_44;
                    case 0x08025689: return Spyro2_45;
                    case 0x0802C53D: return Spyro2_46;
                    case 0x0802C1B5: return Spyro2_47;
                    case 0x080224B5: return Spyro2_48;
                    case 0x080391C5: return Spyro2_49;
                    case 0x080404FD: return Spyro2_50;
                    case 0x0803FF6D: return Spyro2_51;
                    case 0x0802B039: return Spyro2_52;
                    case 0x08024F19: return Spyro2_53;
                    case 0x0802B9DD: return Spyro2_54;
                    case 0x0802B67D: return Spyro2_55;
                    case 0x080312B9: return Spyro2_56;
                    case 0x080316D5: return Spyro2_57;
                    case 0x08033A79: return Spyro2_58;
                }
            }

            return (x, y) => x.AnimSetIndex = -1;
        }


        private static GBAIsometric_Spyro_ROM GetROM(Unity_Object_GBAIsometricSpyro obj) => obj.ObjManager.Context.GetMainFileObject<GBAIsometric_Spyro_ROM>(((GBAIsometric_Spyro_Manager)obj.ObjManager.Context.Settings.GetGameManager).GetROMFilePath);

        private static void Spyro_NotImplemented(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = -1;
        }
        private static void Spyro_EditorObj(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = -1;
            obj.IsEditorObj = true;
        }
        private static void FaceObj(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects, int objType)
        {
            // TODO: Change the animation index as well for animations which support that
            var objToFace = allObjects.FirstOrDefault(x => x.Object.ObjectType == objType);

            if (objToFace == null)
                return;

            var diffX = objToFace.XPosition - obj.XPosition;
            var diffY = objToFace.YPosition - obj.YPosition;

            obj.ForceHorizontalFlip = diffX > diffY;
        }


        private static void Spyro2_0(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Dragon elder (intro)
        {
            obj.AnimSetIndex = 0x1A;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_1(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bianca (intro)
        {
            obj.AnimSetIndex = 0x0C;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_2(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Hunter (intro)
        {
            obj.AnimSetIndex = 0x32;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_3(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Gem
        {
            obj.AnimSetIndex = 0x3B;

            switch (obj.Object.ObjectType - 0x87)
            {
                case 0:
                    obj.AnimationGroupIndex = 0x02;
                    break;

                case 1:
                    obj.AnimationGroupIndex = 0x00;
                    break;

                case 2:
                    obj.AnimationGroupIndex = 0x01;
                    break;

                case 3:
                    obj.AnimationGroupIndex = 0x05;
                    break;

                case 4:
                    obj.AnimationGroupIndex = 0x04;
                    break;
            }
        }
        private static void Spyro2_4(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Gem containers
        {
            var relType = obj.Object.ObjectType - 0x7D;

            obj.AnimSetIndex = relType < 5 ? 0x0B : 0x77;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_5(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Level portal
        {
            obj.AnimSetIndex = 0x56;
            obj.AnimationGroupIndex = 0x02;

            var state = GetROM(obj).States_Spyro2_Portals?.FirstOrDefault(x => x.ObjectType == obj.Object.ObjectType);
            var lvl = obj.ObjManager.Context.Settings.Level;

            var lookAtObj = state?.LevelID == lvl ? 0xDF : state?.SpawnerObjectType;

            FaceObj(obj, allObjects, lookAtObj ?? -1);
        }
        private static void Spyro2_6(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bianca
        {
            obj.AnimSetIndex = 0x0C;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_7(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Level objective
        {
            var lvlObjective = GetROM(obj).States_Spyro2_LevelObjectives;
            var state = lvlObjective.FirstOrDefault(x => x.ObjectType == obj.Object.ObjectType);

            if (state != null)
            {
                obj.AnimSetIndex = state.AnimSetIndex_0;
                obj.AnimationGroupIndex = (byte)state.AnimationGroupIndex_0;
            }
            else
            {
                obj.AnimSetIndex = -1;
            }
        }
        private static void Spyro2_8(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Hunter
        {
            obj.AnimSetIndex = 0x32;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_9(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Hunter (jumping tutorial)
        {
            obj.AnimSetIndex = 0x32;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_10(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Fodder
        {
            if (obj.Object.ObjectType == 0x78)
            {
                obj.AnimSetIndex = 0x67;
                obj.AnimationGroupIndex = 0x02;
            }
            else if (obj.Object.ObjectType == 0x77)
            {
                obj.AnimSetIndex = 0x53;
                obj.AnimationGroupIndex = 0x01;
            }
            else if (obj.Object.ObjectType == 0x79)
            {
                obj.AnimSetIndex = 0x3A;
                obj.AnimationGroupIndex = 0x00;
            }
            else
            {
                obj.AnimSetIndex = -1;
            }
        }
        private static void Spyro2_11(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 NPC
        {
            obj.AnimSetIndex = 0x37;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_12(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bianca
        {
            obj.AnimSetIndex = 0x0C;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_13(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Power-up gate
        {
            // Note: Game creates a second obj with same anim at x + 0xC

            obj.AnimSetIndex = 0x13;

            if (obj.Object.ObjectType == 0x17C)
                obj.AnimationGroupIndex = 0x00;
            else if (obj.Object.ObjectType == 0x17D)
                obj.AnimationGroupIndex = 0x01;
            else if (obj.Object.ObjectType == 0x17E)
                obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro2_14(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Life
        {
            obj.AnimSetIndex = 0x36;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_15(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Dragonfly
        {
            obj.AnimSetIndex = 0x21;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_16(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sparx Panic portal
        {
            obj.AnimSetIndex = 0x56;
            obj.AnimationGroupIndex = 0x02;

            FaceObj(obj, allObjects, 494);
        }
        private static void Spyro2_17(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sheila NPC
        {
            obj.AnimSetIndex = 0x68;
            obj.AnimationGroupIndex = 0x05;
        }
        private static void Spyro2_18(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Moneybags
        {
            obj.AnimSetIndex = 0x51;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_19(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bianca
        {
            obj.AnimSetIndex = 0x0C;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_20(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Moneybags
        {
            obj.AnimSetIndex = 0x51;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_21(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Henrietta
        {
            obj.AnimSetIndex = 0x2F;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_22(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x63;
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro2_23(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Locked chest
        {
            obj.AnimSetIndex = 0x18;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_24(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x6E;
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro2_25(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Checkpoint fairy
        {
            obj.AnimSetIndex = 0x39;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_26(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Key
        {
            obj.AnimSetIndex = 0x3E;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_27(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Daisy
        {
            obj.AnimSetIndex = 0x2F;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_28(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Challenge portal
        {
            obj.AnimSetIndex = 0x57;
            obj.AnimationGroupIndex = 0x01;

            var state = GetROM(obj).States_Spyro2_ChallengePortals?.FirstOrDefault(x => x.ObjectType == obj.Object.ObjectType);
            var lvl = obj.ObjManager.Context.Settings.Level;

            var lookAtObj = state?.LevelID_1 == lvl ? 0xDF : state?.SpawnerObjectType;

            FaceObj(obj, allObjects, lookAtObj ?? -1);
        }
        private static void Spyro2_29(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Mabel
        {
            obj.AnimSetIndex = 0x2F;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_30(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Brian
        {
            obj.AnimSetIndex = 0x45;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_31(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x44;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_32(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x60;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_33(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Boulder
        {
            obj.AnimSetIndex = 0x10;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_34(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Darby
        {
            obj.AnimSetIndex = 0x45;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_35(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Dancing horseshoe
        {
            obj.AnimSetIndex = 0x30;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_36(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Christopher
        {
            obj.AnimSetIndex = 0x07;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_37(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x1F;
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro2_38(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bomb
        {
            obj.AnimSetIndex = 0x0D;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_39(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x52;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_40(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Lizard fodder
        {
            obj.AnimSetIndex = 0x46;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_41(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Thief
        {
            obj.AnimSetIndex = 0x73;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_42(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // NPC
        {
            obj.AnimSetIndex = 0x07;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_43(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ben
        {
            obj.AnimSetIndex = 0x1D;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_44(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Vine
        {
            obj.AnimSetIndex = 0x74;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_45(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x75;
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro2_46(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = 0x76;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_47(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Tommy
        {
            obj.AnimSetIndex = 0x1D;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_48(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Linus
        {
            obj.AnimSetIndex = 0x1D;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_49(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Crush
        {
            obj.AnimSetIndex = 0x19;
            obj.AnimationGroupIndex = 0x05;
        }
        private static void Spyro2_50(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Hunter
        {
            obj.AnimSetIndex = 0x32;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_51(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bianca
        {
            obj.AnimSetIndex = 0x0C;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_52(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x3F;
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro2_53(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x64;
            obj.AnimationGroupIndex = 0x07;
        }
        private static void Spyro2_54(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Caged NPC
        {
            obj.AnimSetIndex = 0x1E;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_55(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bert
        {
            obj.AnimSetIndex = 0x08;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_56(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Robby
        {
            obj.AnimSetIndex = 0x08;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_57(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Cake
        {
            obj.AnimSetIndex = 0x16;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_58(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Spawning enemies
        {
            if (obj.Object.ObjectType == 0x170)
            {
                obj.AnimSetIndex = 0x3F;
                obj.AnimationGroupIndex = 0x02;
            }
            else if (obj.Object.ObjectType == 0x175)
            {
                obj.AnimSetIndex = 0x5D;
                obj.AnimationGroupIndex = 0x02;
            }
            else
            {
                obj.AnimSetIndex = -1;
            }
        }


        private static void Spyro3_0(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Spyro
        {
            obj.AnimSetIndex = 0x7E;
            obj.AnimationGroupIndex = 0x13;
        }
        private static void Spyro3_1(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sparx
        {
            obj.AnimSetIndex = 0x40;
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_2(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Gem containers
        {
            var relType = obj.Object.ObjectType - 0x0D;

            obj.AnimSetIndex = relType < 5 ? 0x09 : 0x84;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_3(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = 0x68;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_4(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Virtual professor
        {
            obj.AnimSetIndex = 0x60; // 0x61 when inactive
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_5(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = 0x10;
            obj.AnimationGroupIndex = 0x0B;
        }
        private static void Spyro3_6(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = 0x5A;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_7(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy in gem container
        {
            obj.AnimSetIndex = 0x66;
            obj.AnimationGroupIndex = 0x02; // Actually defaulted to 0x00, but we do 0x02 so you can see the eyes
        }
        private static void Spyro3_8(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = 0x07;
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_9(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bugs which carry you
        {
            obj.AnimSetIndex = 0x2E;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_10(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Locked chest
        {
            if (obj.Object.ObjectType == 0xa8) // Green
                obj.AnimSetIndex = 0x15;
            else if(obj.Object.ObjectType == 0xa9) // Pink
                obj.AnimSetIndex = 0x16;
            else if(obj.Object.ObjectType == 0xa7) // Red
                obj.AnimSetIndex = 0x17;
            else if (obj.Object.ObjectType == 0xaa) // Yellow
                obj.AnimSetIndex = 0x18;
            else
                obj.AnimSetIndex = -1;

            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_11(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = 0x37;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_12(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // NPC
        {
            var npcStates = GetROM(obj).States_Spyro3_NPC;
            var state = npcStates.FirstOrDefault(x => x.ObjectType == obj.Object.ObjectType);

            if (state != null)
            {
                obj.AnimSetIndex = state.AnimSetIndex;
                obj.AnimationGroupIndex = (byte)state.AnimationGroupIndex;
            }
            else
            {
                obj.AnimSetIndex = -1;
            }
        }
        private static void Spyro3_13(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Mining enemy
        {
            obj.AnimSetIndex = 0x58;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_14(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Flat switch
        {
            if (obj.Object.ObjectType == 0xba)
            {
                obj.AnimSetIndex = 0x5E;
                obj.AnimationGroupIndex = 0x01;
            }
            else if (obj.Object.ObjectType == 0xbb)
            {

                obj.AnimSetIndex = 0x5D;
                obj.AnimationGroupIndex = 0x02;
            }
            else
            {
                obj.AnimSetIndex = -1;
            }
        }
        private static void Spyro3_15(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Red timed switch
        {
            obj.AnimSetIndex = 0x82;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_16(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Timed door
        {
            obj.AnimSetIndex = 0x20;
            obj.AnimationGroupIndex = 0x00; // TODO: This needs to be changed based on some flags!

            // Change the type of the waypoints (the game creates new objects at the waypoint positions)
            for (int i = 0; i < obj.Object.WaypointCount; i++)
                allObjects[i + obj.Object.WaypointIndex].Object.ObjectType = 0xbc;
        }
        private static void Spyro3_17(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Walrus
        {
            obj.AnimSetIndex = 0x87;
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_18(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ripto
        {
            obj.AnimSetIndex = 0x6C;
            obj.AnimationGroupIndex = 0x06;
        }
        private static void Spyro3_19(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Library fairy
        {
            obj.AnimSetIndex = 0x2a;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_20(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Burning book
        {
            obj.AnimSetIndex = 0x48;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_21(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bentley
        {
            obj.AnimSetIndex = 0x0A;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_22(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Small yeti
        {
            obj.AnimSetIndex = 0x08;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_23(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Door
        {
            obj.AnimSetIndex = 0x8E;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_24(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Whistling statue
        {
            obj.AnimSetIndex = 0x8B;

            if (obj.Object.ObjectType == 0xda)
                obj.AnimationGroupIndex = 0x04;
            else if (obj.Object.ObjectType == 0xd9)
                obj.AnimationGroupIndex = 0x02;
            else if (obj.Object.ObjectType == 0xdb)
                obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_25(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = 0x10;
            obj.AnimationGroupIndex = 0x07;
        }
        private static void Spyro3_26(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Penguin
        {
            obj.AnimSetIndex = 0x57;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_27(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Fly
        {
            obj.AnimSetIndex = 0x14;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_28(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Penguin
        {
            obj.AnimSetIndex = 0x57;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_29(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Gem
        {
            obj.AnimSetIndex = 0x3e;

            if (obj.Object.ObjectType == 0x08)
                obj.AnimationGroupIndex = 0x02;
            else if (obj.Object.ObjectType == 0x09)
                obj.AnimationGroupIndex = 0x00;
            else if (obj.Object.ObjectType == 0x0A)
                obj.AnimationGroupIndex = 0x01;
            else if (obj.Object.ObjectType == 0x0B)
                obj.AnimationGroupIndex = 0x05;
            else if (obj.Object.ObjectType == 0x0C)
                obj.AnimationGroupIndex = 0x04;
        }
        private static void Spyro3_30(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = 0x77;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_31(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x7F;
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro3_32(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Moneybags
        {
            obj.AnimSetIndex = 0x55;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_33(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Safe
        {
            obj.AnimSetIndex = 0x71;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_34(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Thief
        {
            obj.AnimSetIndex = 0x53;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_35(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Baby dragon
        {
            obj.AnimSetIndex = 0x23;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_36(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd
        {
            obj.AnimSetIndex = 0x9D;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_37(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Banana pod controller
        {
            obj.AnimSetIndex = 0x05;
            obj.AnimationGroupIndex = 0x03;

            if (obj.Object.ObjectType == 0x195)
            {
                allObjects[obj.Object.WaypointIndex + 0].Object.ObjectType = 0x196;
                allObjects[obj.Object.WaypointIndex + 1].Object.ObjectType = 0x197;
                allObjects[obj.Object.WaypointIndex + 2].Object.ObjectType = 0x198;
            }
            else
            {
                allObjects[obj.Object.WaypointIndex + 0].Object.ObjectType = 0x128;
                allObjects[obj.Object.WaypointIndex + 1].Object.ObjectType = 0x128;
                allObjects[obj.Object.WaypointIndex + 2].Object.ObjectType = 0x128;
            }
        }
        private static void Spyro3_38(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Baby dragon
        {
            obj.AnimSetIndex = 0x23;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_39(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Baby dragon
        {
            obj.AnimSetIndex = 0x23;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_40(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Cube
        {
            obj.AnimSetIndex = 0x63;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_41(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Defeated stealth enemy
        {
            obj.AnimSetIndex = 0xB4;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_42(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Clown from ground
        {
            obj.AnimSetIndex = 0x86;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_43(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Numbers
        {
            obj.AnimSetIndex = 0x02;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_45(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = 0x4D;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_46(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ripto TV
        {
            obj.AnimSetIndex = 0x83;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_47(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Statue
        {
            obj.AnimSetIndex = 0x32;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_48(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Potions
        {
            if (obj.Object.ObjectType == 0x17C)
                obj.AnimSetIndex = 0x42;
            else if (obj.Object.ObjectType == 0x17D)
                obj.AnimSetIndex = 0x43;
            else if (obj.Object.ObjectType == 0x17E)
                obj.AnimSetIndex = 0x44;
            else if (obj.Object.ObjectType == 0x17f)
                obj.AnimSetIndex = 0x45;
            else
                obj.AnimSetIndex = -1;

            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_49(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Rainy cloud
        {
            obj.AnimSetIndex = 0x80;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_50(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Professor's contraption
        {
            obj.AnimSetIndex = 0x85;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_52(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Tutorial objects
        {
            if (obj.Object.ObjectType == 0x1B)
            {
                obj.AnimSetIndex = 0x09;
                obj.AnimationGroupIndex = 0x01;
            }
            else if (obj.Object.ObjectType == 0x1C)
            {
                obj.AnimSetIndex = 0x0C;
                obj.AnimationGroupIndex = 0x00;
            }
            else if (obj.Object.ObjectType == 0x1D)
            {
                obj.AnimSetIndex = 0x84;
                obj.AnimationGroupIndex = 0x01;
            }
            else if (obj.Object.ObjectType == 0x1E)
            {
                obj.AnimSetIndex = 0x0D;
                obj.AnimationGroupIndex = 0x00;
            }
            else if (obj.Object.ObjectType == 0x1F)
            {
                obj.AnimSetIndex = 0x04;
                obj.AnimationGroupIndex = 0x00;
            }
            else
            {
                obj.AnimSetIndex = -1;
            }
        }
        private static void Spyro3_53(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Door
        {
            var rom = GetROM(obj);
            var levID = obj.ObjManager.Context.Settings.Level;
            var typeState = rom.States_Spyro3_DoorTypes.FirstOrDefault(x => x.ObjectType == obj.Object.ObjectType);

            long graphicsID;
            if (typeState?.LevelID == levID)
                graphicsID = typeState?.GraphicsStateID1 ?? -1;
            else
                graphicsID = typeState?.GraphicsStateID2 ?? -1;

            var graphicsState = rom.States_Spyro3_DoorGraphics.FirstOrDefault(x => x.ID == graphicsID);

            if (graphicsState != null)
            {
                obj.AnimSetIndex = graphicsState.AnimSetIndex;
                obj.AnimationGroupIndex = (byte)graphicsState.AnimationGroupIndex;
            }
            else
            {
                obj.AnimSetIndex = -1;
            }
        }
        private static void Spyro3_54(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Electric switches
        {
            if (obj.Object.ObjectType == 562 || obj.Object.ObjectType == 624 || obj.Object.ObjectType == 633)
            {
                obj.AnimSetIndex = 0x46;
                obj.AnimationGroupIndex = 0x02;
            }
            else if (obj.Object.ObjectType == 616)
            {
                obj.AnimSetIndex = 0x31;
                obj.AnimationGroupIndex = 0x01;
            }
            else
            {
                obj.AnimSetIndex = -1;
            }
        }
        private static void Spyro3_55(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sheep
        {
            if (obj.Object.ObjectType == 0x34)
                obj.AnimSetIndex = 0x75;
            else if (obj.Object.ObjectType == 0x33)
                obj.AnimSetIndex = 0x73;
            else if (obj.Object.ObjectType == 0x35)
                obj.AnimSetIndex = 0x74;
            else
                obj.AnimSetIndex = -1;

            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_56(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Spyro gets teleported
        {
            obj.AnimSetIndex = 0x88;
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_57(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Raft
        {
            obj.AnimSetIndex = 0x11;
            obj.AnimationGroupIndex = 0x01;

            allObjects[obj.Object.WaypointIndex].AnimSetIndex = 0x12;
            allObjects[obj.Object.WaypointIndex].AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_58(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Boat
        {
            if (obj.Object.ObjectType == 0x26E)
            {
                obj.AnimSetIndex = 0x2C;
                obj.AnimationGroupIndex = 0x00;
            }
            else if (obj.Object.ObjectType == 0x26D)
            {
                obj.AnimSetIndex = 0x34;
                obj.AnimationGroupIndex = 0x01;
            }
            else
            {
                obj.AnimSetIndex = -1;
            }
        }
        private static void Spyro3_59(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Quest item
        {
            var questItem = GetROM(obj).QuestItems.FirstOrDefault(x => x.ObjectType == obj.Object.ObjectType);

            // NOTE: The game creates an object for the chest and sets it to the chest type if the quest item is in a chest. But to avoid us actually changing the type of the object we hard-code the graphics for the specific chest here.
            switch (questItem?.ItemType)
            {
                case GBAIsometric_Spyro3_QuestItem.QuestItemType.RedChest:
                    obj.AnimSetIndex = 0x17;
                    obj.AnimationGroupIndex = 0x00;
                    break;

                case GBAIsometric_Spyro3_QuestItem.QuestItemType.GreenChest:
                    obj.AnimSetIndex = 0x15;
                    obj.AnimationGroupIndex = 0x00;
                    break;

                case GBAIsometric_Spyro3_QuestItem.QuestItemType.PinkChest:
                    obj.AnimSetIndex = 0x16;
                    obj.AnimationGroupIndex = 0x00;
                    break;

                case GBAIsometric_Spyro3_QuestItem.QuestItemType.YellowChest:
                    obj.AnimSetIndex = 0x18;
                    obj.AnimationGroupIndex = 0x00;
                    break;

                default:
                    obj.AnimSetIndex = 0x00;
                    obj.AnimationGroupIndex = 0x01;
                    obj.ForceFrame = questItem?.AnimFrameIndex ?? 0;
                    // Note: The game also creates an objects for animSet 0, group 0 to be behind it as the shining effect
                    break;
            }
        }
        private static void Spyro3_60(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Hat
        {
            obj.AnimSetIndex = 0x54;
            obj.AnimationGroupIndex = 0x02;
            // Note: The game also creates an objects for the creature inside the hat
        }
        private static void Spyro3_61(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Air vent
        {
            obj.AnimSetIndex = 0x1F;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_62(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ripto poster
        {
            obj.AnimSetIndex = 0x06;
            obj.AnimationGroupIndex = 0x01;

            // TODO: Find better solution to this
            obj.ForceHorizontalFlip = obj.Object.Value2;
        }
        private static void Spyro3_63(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Butler
        {
            obj.AnimSetIndex = 0x13;
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro3_64(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            Spyro_EditorObj(obj, allObjects);

            for (int i = 0; i < obj.Object.WaypointCount; i++)
                allObjects[i + obj.Object.WaypointIndex].Object.ObjectType = 0x181;
        }
        private static void Spyro3_65(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Boulder
        {
            obj.AnimSetIndex = 0x0F;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_66(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Rhynopede
        {
            Spyro_EditorObj(obj, allObjects);

            for (int i = 0; i < obj.Object.WaypointCount; i++)
            {
                var wp = allObjects[i + obj.Object.WaypointIndex];

                if (i == 0)
                {
                    wp.AnimSetIndex = 0x69;
                    wp.AnimationGroupIndex = (byte)(obj.Object.ObjectType == 0x42 ? 0x01 : 0x03);
                }
                else
                {
                    wp.AnimSetIndex = obj.Object.ObjectType != 0x42 && obj.Object.ObjectType != 0x27A ? 0x7B : 0x69;
                }
            }
        }
        private static void Spyro3_67(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Locked chest
        {
            if (obj.Object.ObjectType == 0x1A8) // Green
                obj.AnimSetIndex = 0x15;
            else if (obj.Object.ObjectType == 0x1A9) // Pink
                obj.AnimSetIndex = 0x16;
            else if (obj.Object.ObjectType == 0x1A7) // Red
                obj.AnimSetIndex = 0x17;
            else if (obj.Object.ObjectType == 0x1AA) // Yellow
                obj.AnimSetIndex = 0x18;
            else
                obj.AnimSetIndex = -1;

            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_68(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sheep with clothes
        {
            obj.AnimSetIndex = 0x73;

            if (obj.Object.ObjectType == 0x36)
                obj.AnimationGroupIndex = 0x04;
            else if (obj.Object.ObjectType == 0x37)
                obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro3_69(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ice block
        {
            obj.AnimSetIndex = 0x7D;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_70(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x67;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_71(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ice obstacle
        {
            obj.AnimSetIndex = 0x87;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_72(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x7B;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_73(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Green thief
        {
            obj.AnimSetIndex = 0x53;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_74(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x03;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_75(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Thief NPC
        {
            obj.AnimSetIndex = 0x56;
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro3_76(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Green race thief
        {
            // Note: The game spawns them from the race controller type (253)
            obj.AnimSetIndex = 0x53;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_77(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bianca
        {
            obj.AnimSetIndex = 0x0B;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_78(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x1E;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_79(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // O'Hare
        {
            obj.AnimSetIndex = 0x4F;
            obj.AnimationGroupIndex = obj.Object.ObjectType == 0x156 ? (byte)0x02 : (byte)0x01;
        }
        private static void Spyro3_80(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Clown from ground controller
        {
            Spyro_EditorObj(obj, allObjects);

            for (int i = 0; i < obj.Object.WaypointCount; i++)
            {
                var wp = allObjects[i + obj.Object.WaypointIndex];
                wp.Object.ObjectType = 0x153;
            }
        }
        private static void Spyro3_81(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // George
        {
            obj.AnimSetIndex = 0x8C;
            obj.AnimationGroupIndex = 0x03;

            var wp = allObjects[obj.Object.WaypointCount - 1 + obj.Object.WaypointIndex];
            wp.Object.ObjectType = 0x75;
        }
        private static void Spyro3_82(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Banana pod
        {
            obj.AnimSetIndex = 0x05;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_83(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Albert
        {
            obj.AnimSetIndex = 0x8C;
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro3_84(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Target
        {
            obj.AnimSetIndex = 0x6B;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_85(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x7F;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_86(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Janice
        {
            obj.AnimSetIndex = 0x50;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_87(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ripto statue
        {
            obj.AnimSetIndex = 0x70;
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro3_88(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Hedgehog
        {
            obj.AnimSetIndex = 0x38;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_89(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x2F;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_90(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // R statue
        {
            obj.AnimSetIndex = 0x70;
            obj.AnimationGroupIndex = obj.ObjType.ObjFlags == 0 ? (byte) 0x01 : (byte) 0x02;
        }
        private static void Spyro3_91(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Puzzle
        {
            // Note: The game creates 9 objects here for each piece of the puzzle, with each using a specific frame
            obj.AnimSetIndex = 0x64;
            obj.AnimationGroupIndex = obj.Object.ObjectType == 0x177 ? (byte) 0x00 : (byte) 0x02;
        }
        private static void Spyro3_92(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Moneybags
        {
            obj.AnimSetIndex = 0x55;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_93(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Water fountain
        {
            obj.AnimSetIndex = 0x1D;
            obj.AnimationGroupIndex = 0x01;

            var wp = allObjects[obj.Object.WaypointIndex];
            wp.Object.ObjectType = 0xBB;
        }
        private static void Spyro3_94(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Boulder controller
        {
            Spyro_EditorObj(obj, allObjects);

            for (int i = 0; i < obj.Object.WaypointCount; i++)
            {
                var wp = allObjects[i + obj.Object.WaypointIndex];
                wp.Object.ObjectType = 0x181;
            }
        }
        private static void Spyro3_95(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Blocking drill
        {
            obj.AnimSetIndex = 0x6A;
            obj.AnimationGroupIndex = 0x00;

            for (int i = 0; i < obj.Object.WaypointCount; i++)
            {
                var wp = allObjects[i + obj.Object.WaypointIndex];
                wp.Object.ObjectType = 0xBB;
            }
        }
        private static void Spyro3_96(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Cat
        {
            obj.AnimSetIndex = 0x1C;
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_97(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x8A;
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_98(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Food machine
        {
            obj.AnimSetIndex = 0x36;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_99(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bomb bird
        {
            // Note: Game spawns a bomb object too
            obj.AnimSetIndex = 0x0E;
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_100(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bruiser
        {
            obj.AnimSetIndex = 0x1B;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_101(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x81;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_102(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Treadmill
        {
            obj.AnimSetIndex = 0x33;
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_103(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x2F;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_104(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = 0x3D;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_105(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Baby dragon
        {
            obj.AnimSetIndex = 0x23;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_106(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Blocking drill
        {
            obj.AnimSetIndex = 0x6A;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_107(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Curtain
        {
            obj.AnimSetIndex = 0x25;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_108(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ripto spying machine
        {
            obj.AnimSetIndex = 0x26;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_109(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ripto portal
        {
            obj.AnimSetIndex = 0x3B;
            obj.AnimationGroupIndex = 0x12;
        }
        private static void Spyro3_110(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Electric switch
        {
            obj.AnimSetIndex = 0x46;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_111(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Professor
        {
            obj.AnimSetIndex = 0x5F;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_112(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Professor
        {
            obj.AnimSetIndex = 0x5F;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_113(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Statue top
        {
            obj.AnimSetIndex = 0x6D;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_114(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Baby dragon
        {
            obj.AnimSetIndex = 0x23;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_115(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Statue bottom
        {
            obj.AnimSetIndex = 0x6F;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_116(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Final boss statue
        {
            obj.AnimSetIndex = 0x6C;
            obj.AnimationGroupIndex = 0x10;
        }
        private static void Spyro3_117(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Final boss throne
        {
            obj.AnimSetIndex = 0x6C;
            obj.AnimationGroupIndex = 0x15;
        }
        private static void Spyro3_118(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Poisoned water
        {
            obj.AnimSetIndex = 0x47;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_119(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Boulder controller
        {
            Spyro_EditorObj(obj, allObjects);

            for (int i = 0; i < obj.Object.WaypointCount; i++)
            {
                var wp = allObjects[i + obj.Object.WaypointIndex];
                wp.Object.ObjectType = 0x181;
            }
        }
        private static void Spyro3_120(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ripto
        {
            obj.AnimSetIndex = 0x6E;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_121(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 air vent 
        {
            // Note: Game also spawns Sparx here
            obj.AnimSetIndex = 0xB1;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_122(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 sign post 
        {
            obj.AnimSetIndex = 0xB2;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_123(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 locked doors
        {
            obj.AnimSetIndex = 0xA4;

            var wp = allObjects[obj.Object.WaypointIndex];

            if (obj.Object.ObjectType == 0x24A)
            {
                obj.AnimationGroupIndex = 0x03;
                wp.Object.ObjectType = 0x24B;
            }
            else if (obj.Object.ObjectType == 0x24C)
            {
                obj.AnimationGroupIndex = 0x01;
                wp.Object.ObjectType = 0x24D;
            }
            else if (obj.Object.ObjectType == 0x24E)
            {
                obj.AnimationGroupIndex = 0x00;
                wp.Object.ObjectType = 0x24F;
            }
        }
        private static void Spyro3_124(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 sleeping guard 
        {
            obj.AnimSetIndex = 0xB7;
            obj.AnimationGroupIndex = 0x05;
        }
        private static void Spyro3_125(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 gem
        {
            obj.AnimSetIndex = 0xA6;

            if (obj.Object.ObjectType == 0x20A)
                obj.AnimationGroupIndex = 0x02;
            else if (obj.Object.ObjectType == 0x20B)
                obj.AnimationGroupIndex = 0x00;
            else if (obj.Object.ObjectType == 0x20C)
                obj.AnimationGroupIndex = 0x01;
            else if (obj.Object.ObjectType == 0x20D)
                obj.AnimationGroupIndex = 0x04;
            else if (obj.Object.ObjectType == 0x20E)
                obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro3_126(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 patrolling guard 
        {
            obj.AnimSetIndex = 0xB7;
            obj.AnimationGroupIndex = 0x0A;
        }
        private static void Spyro3_127(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 guard popping out from pipe 
        {
            obj.AnimSetIndex = 0xBA;
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro3_128(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 prisoner 
        {
            obj.AnimSetIndex = 0xB3;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_129(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 locked door key 
        {
            obj.AnimSetIndex = 0xA9;

            if (obj.Object.ObjectType == 0x24B)
                obj.AnimationGroupIndex = 0x03;
            else if (obj.Object.ObjectType == 0x24D)
                obj.AnimationGroupIndex = 0x01;
            else if (obj.Object.ObjectType == 0x24F)
                obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_130(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 camera 
        {
            obj.AnimSetIndex = 0xB9;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_131(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 flying enemy 
        {
            obj.AnimSetIndex = 0xB5;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_132(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 checkpoint fairy 
        {
            obj.AnimSetIndex = 0x3C;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_133(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 gem container 
        {
            if (obj.Object.ObjectType == 0x1F9 || obj.Object.ObjectType == 0x1FA || obj.Object.ObjectType == 0x1FB || obj.Object.ObjectType == 0x1FC || obj.Object.ObjectType == 0x1FD)
                obj.AnimSetIndex = 0xA3;
            else if (obj.Object.ObjectType == 0x1FE || obj.Object.ObjectType == 0x1FF || obj.Object.ObjectType == 0x200 || obj.Object.ObjectType == 0x201 || obj.Object.ObjectType == 0x202)
                obj.AnimSetIndex = 0xAF;
            else
                obj.AnimSetIndex = -1;

            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_134(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 moving platform 
        {
            obj.AnimSetIndex = 0xAB;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_135(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 mines 
        {
            obj.AnimSetIndex = 0xAA;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_136(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = 0xB8;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_137(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 spying enemy
        {
            obj.AnimSetIndex = 0xB7;
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_138(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd air vent 
        {
            // Note: Game also spawns Sparx here
            obj.AnimSetIndex = 0x9E;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_139(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd gem 
        {
            obj.AnimSetIndex = 0xA6;

            if (obj.Object.ObjectType == 0x2C8)
                obj.AnimationGroupIndex = 0x02;
            else if (obj.Object.ObjectType == 0x2C9)
                obj.AnimationGroupIndex = 0x00;
            else if (obj.Object.ObjectType == 0x2CA)
                obj.AnimationGroupIndex = 0x01;
            else if (obj.Object.ObjectType == 0x2CB)
                obj.AnimationGroupIndex = 0x04;
            else if (obj.Object.ObjectType == 0x2CC)
                obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro3_140(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd imprisoned penguin 
        {
            // Note: Game also spawns penguin inside of cage
            obj.AnimSetIndex = 0x90;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_141(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd flying enemy 
        {
            obj.AnimSetIndex = 0x91;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_44(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd shooting enemy
        {
            obj.AnimSetIndex = 0xA1;

            if (obj.Object.ObjectType == 0x167)
                obj.AnimationGroupIndex = 0x05;
            else if (obj.Object.ObjectType == 0x168)
                obj.AnimationGroupIndex = 0x04;
            else if (obj.Object.ObjectType == 0x169)
                obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_142(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd electric gate 
        {
            var states = new[]
            {
                new { ObjType = 0x2CD, SwitchType = 0x2D3, AnimGroup = 0x05 },
                new { ObjType = 0x2CE, SwitchType = 0x2D3, AnimGroup = 0x0B },
                new { ObjType = 0x2CF, SwitchType = 0x2D4, AnimGroup = 0x03 },
                new { ObjType = 0x2D0, SwitchType = 0x2D4, AnimGroup = 0x09 },
                new { ObjType = 0x2D1, SwitchType = 0x2D5, AnimGroup = 0x01 },
                new { ObjType = 0x2D2, SwitchType = 0x2D5, AnimGroup = 0x07 },
            };

            obj.AnimSetIndex = 0x96;
            
            var state = states.FirstOrDefault(x => x.ObjType == obj.Object.ObjectType);

            if (state != null)
            {
                obj.AnimationGroupIndex = (byte)state.AnimGroup;

                for (int i = 0; i < obj.Object.WaypointCount; i++)
                {
                    var wp = allObjects[i + obj.Object.WaypointIndex];
                    wp.Object.ObjectType = (ushort)state.SwitchType;
                }
            }
        }
        private static void Spyro3_143(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd electric gate switch
        {
            obj.AnimSetIndex = 0x9A;

            if (obj.Object.ObjectType == 0x2D3)
                obj.AnimationGroupIndex = 0x05;
            else if (obj.Object.ObjectType == 0x2D4)
                obj.AnimationGroupIndex = 0x03;
            else if (obj.Object.ObjectType == 0x2D5)
                obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_144(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd enemy
        {
            obj.AnimSetIndex = 0xA1;

            if (obj.Object.ObjectType == 0x168)
                obj.AnimationGroupIndex = 0x04;
            else if (obj.Object.ObjectType == 0x169)
                obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_145(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd teleporter controller
        {
            Spyro_EditorObj(obj, allObjects);

            for (int i = 0; i < obj.Object.WaypointCount; i++)
            {
                var wp = allObjects[i + obj.Object.WaypointIndex];

                if (obj.Object.ObjectType == 0x2D8)
                    wp.Object.ObjectType = 0x2DB;
                else if (obj.Object.ObjectType == 0x2D9)
                    wp.Object.ObjectType = 0x2DC;
                else if (obj.Object.ObjectType == 0x2DA)
                    wp.Object.ObjectType = 0x2DD;
            }
        }
        private static void Spyro3_146(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd teleporter
        {
            obj.AnimSetIndex = 0x9B;

            if (obj.Object.ObjectType == 0x2DC)
                obj.AnimationGroupIndex = 0x01;
            else if (obj.Object.ObjectType == 0x2DD)
                obj.AnimationGroupIndex = 0x00;
            else if (obj.Object.ObjectType == 0x2DB)
                obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_147(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd tent
        {
            Spyro_EditorObj(obj, allObjects);

            var wp = allObjects[obj.Object.WaypointIndex];
            wp.AnimSetIndex = 0x9C;
            wp.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_148(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd enemy 
        {
            obj.AnimSetIndex = 0x91;
            obj.AnimationGroupIndex = 0x00;
        }
    }
}