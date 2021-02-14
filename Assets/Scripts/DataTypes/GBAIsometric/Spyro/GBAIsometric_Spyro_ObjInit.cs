using System;
using System.Linq;

namespace R1Engine
{
    public static class GBAIsometric_Spyro_ObjInit
    {
        public static Action<Unity_Object_GBAIsometricSpyro, Unity_Object_GBAIsometricSpyro[]> GetInitFunc(GameSettings settings, uint address)
        {
            address = ConvertAddress(settings.GameModeSelection, address);

            if (settings.EngineVersion == EngineVersion.GBAIsometric_Spyro3)
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
            else if (settings.EngineVersion == EngineVersion.GBAIsometric_Spyro2)
            {
                switch (address)
                {
                    case 0x08010EB5:
                    case 0x08013EC5:
                    case 0x080149F1:
                    case 0x08014EA1:
                    case 0x080151DD:
                    case 0x08015429:
                    case 0x08015D51:
                    case 0x08018639:
                    case 0x0801F829:
                    case 0x08020145:
                    case 0x080211BD:
                    case 0x0801DCB1:
                    case 0x08022E89:
                    case 0x08023E89:
                    case 0x080251BD:
                    case 0x08026B01:
                    case 0x08028535:
                    case 0x08029395:
                    case 0x08029FE1:
                    case 0x0802B431:
                    case 0x0802D2CD:
                    case 0x08032799:
                    case 0x08034C7D:
                    case 0x08034F39:
                    case 0x0803539D:
                    case 0x080368FD:
                    case 0x08036CB1:
                    case 0x08037005:
                    case 0x08037FB5:
                    case 0x08038679:
                    case 0x080387E1:
                    case 0x08038929:
                    case 0x0803A58D:
                    case 0x0803AFD9:
                    case 0x0803B36D:
                    case 0x08026E75:
                    case 0x0803D1F5:
                    case 0x0803DE11:
                    case 0x0803C431:
                    case 0x0803C735:
                    case 0x0803EDB9:
                        return Spyro_NotImplemented;

                    case 0x08036C8D: // Vertical collision
                    case 0x0801074D: // Spawner
                    case 0x0803F941: // Tutorial trigger
                    case 0x0803197D: // Side map area trigger
                    case 0x08041705: // Challenge object spawner
                    case 0x08012EA9: // Freeze tag controller
                    case 0x08028929: // Sheila firefly spawner
                    case 0x0803D5C1: // Sheila firefly spawner
                    case 0x0803DFB9: // Sheila tutorial trigger
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
                    case 0x08010761: return Spyro2_59;
                    case 0x0801ED71: return Spyro2_60;
                    case 0x0801F40D: return Spyro2_61;
                    case 0x08020BF5: return Spyro2_62;
                    case 0x0801841D: return Spyro2_63;
                    case 0x080290B9: return Spyro2_64;
                    case 0x0802BFD5: return Spyro2_65;
                    case 0x0803DA05: return Spyro2_66;
                    case 0x08023AA1: return Spyro2_67;
                    case 0x0802BB9D: return Spyro2_68;
                    case 0x08028AA5: return Spyro2_69;
                    case 0x0802D901: return Spyro2_70;
                    case 0x080332D1: return Spyro2_71;
                    case 0x0802E2B5: return Spyro2_72;
                    case 0x0802CC6D: return Spyro2_73;
                    case 0x08031A0D: return Spyro2_74;
                    case 0x0802EA61: return Spyro2_75;
                    case 0x0802E55D: return Spyro2_76;
                    case 0x08034069: return Spyro2_77;
                    case 0x08037725: return Spyro2_78;
                    case 0x08040811: return Spyro2_79;
                    case 0x08040ACD: return Spyro2_80;
                    case 0x0802EFBD: return Spyro2_81;
                    case 0x08030B21: return Spyro2_82;
                    case 0x0802ED69: return Spyro2_83;
                    case 0x0802FAF5: return Spyro2_84;
                    case 0x0803240D: return Spyro2_85;
                    case 0x08032D41: return Spyro2_86;
                    case 0x0802F335: return Spyro2_87;
                    case 0x08016C41: return Spyro2_88;
                    case 0x0802F575: return Spyro2_89;
                    case 0x0802C6A5: return Spyro2_90;
                    case 0x080303C5: return Spyro2_91;
                    case 0x08041145: return Spyro2_92;
                    case 0x08035E61: return Spyro2_93;
                    case 0x08028025: return Spyro2_94;
                    case 0x0803C9B5: return Spyro2_95;
                    case 0x080275B1: return Spyro2_96;
                    case 0x0803464D: return Spyro2_97;
                    case 0x0803BBCD: return Spyro2_98;
                    case 0x0803C259: return Spyro2_99;
                    case 0x08025AFD: return Spyro2_100;
                    case 0x080267A5: return Spyro2_101;
                    case 0x0803BE29: return Spyro2_102;
                }
            }
            else if (settings.EngineVersion == EngineVersion.GBAIsometric_Tron2)
            {
                return (x, y) => x.AnimSetIndex = 0;
            }

            return (x, y) => x.AnimSetIndex = -1;
        }

        private static uint ConvertAddress(GameModeSelection gameMode, uint address)
        {
            if (gameMode == GameModeSelection.SpyroAdventureEU)
            {
                switch (address)
                {
                    case 0x08015A05: return 0x0801580D;
                    case 0x08015BB5: return 0x080159BD;
                    case 0x08015BD9: return 0x080159E1;
                    case 0x08015DB5: return 0x08015BBD;
                    case 0x0801A3E9: return 0x0801A20D;
                    case 0x0801A74D: return 0x0801A565;
                    case 0x0801AE15: return 0x0801AC2D;
                    case 0x0801B299: return 0x0801B0B1;
                    case 0x0801B441: return 0x0801B259;
                    case 0x0801B681: return 0x0801B499;
                    case 0x0801B7A5: return 0x0801B5BD;
                    case 0x0801BBA5: return 0x0801B9BD;
                    case 0x0801BD5D: return 0x0801BB75;
                    case 0x0801C451: return 0x0801C269;
                    case 0x0801CB89: return 0x0801C9A1;
                    case 0x0801D1BD: return 0x0801CFD5;
                    case 0x0801D69D: return 0x0801D4B5;
                    case 0x0801DC09: return 0x0801DA1D;
                    case 0x0801DE69: return 0x0801DC7D;
                    case 0x0801DF71: return 0x0801DD85;
                    case 0x0801E3BD: return 0x0801E1C1;
                    case 0x0801EA51: return 0x0801E855;
                    case 0x0801F695: return 0x0801F48D;
                    case 0x0801FC05: return 0x0801F9FD;
                    case 0x08020A61: return 0x08020859;
                    case 0x08020F41: return 0x08020D39;
                    case 0x080210C1: return 0x08020EB9;
                    case 0x08021235: return 0x0802102D;
                    case 0x08021711: return 0x08021509;
                    case 0x08021D29: return 0x08021B21;
                    case 0x0802202D: return 0x08021E25;
                    case 0x0802231D: return 0x08022115;
                    case 0x08022431: return 0x08022229;
                    case 0x0802298D: return 0x08022785;
                    case 0x08023211: return 0x08023009;
                    case 0x08023611: return 0x08023409;
                    case 0x080238B9: return 0x080236B1;
                    case 0x080239E9: return 0x080237E1;
                    case 0x08023BE9: return 0x080239E1;
                    case 0x08023D85: return 0x08023B7D;
                    case 0x08024619: return 0x08024411;
                    case 0x08024C3D: return 0x08024A35;
                    case 0x08024DED: return 0x08024BE5;
                    case 0x08025149: return 0x08024F21;
                    case 0x080252F1: return 0x080250C9;
                    case 0x080256C5: return 0x0802549D;
                    case 0x08025805: return 0x080255DD;
                    case 0x08025AFD: return 0x080258D5;
                    case 0x08025C35: return 0x08025A0D;
                    case 0x08025EC5: return 0x08025C9D;
                    case 0x08026645: return 0x0802641D;
                    case 0x080269ED: return 0x080267C5;
                    case 0x08026EC1: return 0x08026C91;
                    case 0x080270BD: return 0x08026E8D;
                    case 0x080277A5: return 0x0802756D;
                    case 0x08027AAD: return 0x08027875;
                    case 0x08027FF9: return 0x08027DC1;
                    case 0x0802818D: return 0x08027F55;
                    case 0x08028495: return 0x0802825D;
                    case 0x080285ED: return 0x080283B5;
                    case 0x08028A4D: return 0x08028815;
                    case 0x08028C69: return 0x08028A31;
                    case 0x08029005: return 0x08028DCD;
                    case 0x080294F5: return 0x080292BD;
                    case 0x0802AA41: return 0x0802A805;
                    case 0x0802B4AD: return 0x0802B271;
                    case 0x0802B741: return 0x0802B505;
                    case 0x0802B9B5: return 0x0802B779;
                    case 0x0802BC09: return 0x0802B9CD;
                    case 0x0802C189: return 0x0802BF4D;
                    case 0x0802C2BD: return 0x0802C081;
                    case 0x0802C405: return 0x0802C1C9;
                    case 0x0802C5DD: return 0x0802C3A1;
                    case 0x0802CA45: return 0x0802C809;
                    case 0x0802CEA1: return 0x0802CC65;
                    case 0x0802F2D1: return 0x0802F089;
                    case 0x0802F8A5: return 0x0802F65D;
                    case 0x0802FB35: return 0x0802F8ED;
                    case 0x0802FC81: return 0x0802FA39;
                    case 0x0803001D: return 0x0802FDD5;
                    case 0x0803046D: return 0x08030225;
                    case 0x08030615: return 0x080303CD;
                    case 0x08030755: return 0x0803050D;
                    case 0x08030A09: return 0x080307C1;
                    case 0x08030E19: return 0x08030BD1;
                    case 0x08031229: return 0x08030FE1;
                    case 0x080317FD: return 0x080315B5;
                    case 0x08031D15: return 0x08031979;
                    case 0x08032119: return 0x08031DDD;
                    case 0x08032385: return 0x08032049;
                    case 0x08032C85: return 0x08032949;
                    case 0x08033101: return 0x08032DC5;
                    case 0x08033675: return 0x08033339;
                    case 0x0803387D: return 0x08033541;
                    case 0x08033CC1: return 0x080338A5;
                    case 0x080341FD: return 0x08033DE1;
                    case 0x08034A79: return 0x080345D5;
                    case 0x08034C05: return 0x08034765;
                    case 0x08034E1D: return 0x0803497D;
                    case 0x08035175: return 0x08034CD5;
                    case 0x0803553D: return 0x0803509D;
                    case 0x080356B9: return 0x08035219;
                    case 0x08035BED: return 0x0803574D;
                    case 0x08036041: return 0x08035BA1;
                    case 0x080364CD: return 0x0803602D;
                    case 0x080367B9: return 0x08036319;
                    case 0x08036895: return 0x080363F5;
                    case 0x08036911: return 0x08036471;
                    case 0x0803757D: return 0x080370D1;
                    case 0x080378ED: return 0x08037441;
                    case 0x08037B55: return 0x080376A9;
                    case 0x08038869: return 0x080383BD;
                    case 0x08038999: return 0x080384ED;
                    case 0x08038B91: return 0x080386E5;
                    case 0x08038F69: return 0x08038ABD;
                    case 0x080392BD: return 0x08038E11;
                    case 0x08039B19: return 0x0803966D;
                    case 0x0803A1FD: return 0x08039D51;
                    case 0x0803A4D9: return 0x0803A02D;
                    case 0x0803A8CD: return 0x0803A421;
                    case 0x0803AAD9: return 0x0803A62D;
                    case 0x0803AC31: return 0x0803A785;
                    case 0x0803AD55: return 0x0803A8A9;
                    case 0x0803AFF5: return 0x0803AB49;
                    case 0x0803B385: return 0x0803AED9;
                    case 0x0803B851: return 0x0803B3A5;
                    case 0x0803BB2D: return 0x0803B681;
                    case 0x0803BC2D: return 0x0803B781;
                    case 0x0803BE2D: return 0x0803B981;
                    case 0x0803C2B9: return 0x0803BE0D;
                    case 0x0803C43D: return 0x0803BF91;
                    case 0x0803C8E9: return 0x0803C43D;
                    case 0x0803CCA9: return 0x0803C7FD;
                    case 0x0803CF95: return 0x0803CAE9;
                    case 0x0803CFE5: return 0x0803CB39;
                    case 0x0803D59D: return 0x0803D0F1;
                    case 0x0803DCD5: return 0x0803D829;
                    case 0x0803E105: return 0x0803DC59;
                    case 0x0803E701: return 0x0803E255;
                    case 0x0803EA05: return 0x0803E559;
                    case 0x0803EB89: return 0x0803E6DD;
                    case 0x0803EDD1: return 0x0803E925;
                    case 0x0803EEC9: return 0x0803EA1D;
                    case 0x0803F1ED: return 0x0803ED41;
                    case 0x0803F749: return 0x0803F255;
                    case 0x0803F8F1: return 0x0803F3FD;
                    case 0x0803FA81: return 0x0803F58D;
                    case 0x08040155: return 0x0803FC61;
                    case 0x080404A5: return 0x0803FFB1;
                    case 0x080406B5: return 0x080401C1;
                    case 0x080409A1: return 0x080404AD;
                    case 0x08040A65: return 0x08040571;
                    case 0x08040B31: return 0x0804063D;
                    case 0x08040E19: return 0x08040925;
                    case 0x08040F55: return 0x08040A61;
                    case 0x080411B5: return 0x08040CB5;
                    case 0x080412D9: return 0x08040DD9;
                    case 0x080422A5: return 0x08041DA5;
                    case 0x080429AD: return 0x080424AD;
                    case 0x08042A85: return 0x08042585;
                    case 0x08042C31: return 0x08042725;
                    case 0x08042DC9: return 0x080428BD;
                    case 0x08042EFD: return 0x080429F1;
                    case 0x080432A5: return 0x08042D99;
                    case 0x080435D1: return 0x080430C5;
                    case 0x08043741: return 0x08043235;
                    case 0x08043DFD: return 0x080438F1;
                    case 0x08043F85: return 0x08043A79;
                    case 0x08044165: return 0x08043C59;
                    case 0x08044341: return 0x08043E35;
                    case 0x080443B9: return 0x08043EAD;
                    case 0x08044DDD: return 0x080448D1;
                    case 0x08044EC5: return 0x080449B9;
                    case 0x08044FE1: return 0x08044AD5;
                    case 0x0801D38D: return 0x0801D1A5;
                    case 0x0804528D: return 0x08044D81;
                    case 0x080455C9: return 0x080450BD;
                    case 0x08045A01: return 0x080454F5;
                    case 0x08045B65: return 0x08045659;
                    case 0x08045EBD: return 0x080459B1;
                    case 0x08046269: return 0x08045D5D;
                    case 0x080463DD: return 0x08045ED1;
                    case 0x0804666D: return 0x08046161;
                    case 0x0804677D: return 0x08046271;
                    case 0x08046981: return 0x08046475;
                    case 0x08046ABD: return 0x080465B1;
                    case 0x08046E51: return 0x08046945;
                    case 0x08046F21: return 0x08046A15;
                    case 0x080474D1: return 0x08046FC5;
                    case 0x080475B5: return 0x080470A9;
                    case 0x0804760D: return 0x08047101;
                    case 0x080478A9: return 0x0804739D;
                    case 0x08047B5D: return 0x08047651;
                }
            }
            else if (gameMode == GameModeSelection.SpyroSeasonFlameEU)
            {
                switch (address)
                {
                    case 0x080102C9: return 0x080102A9;
                    case 0x0801076D: return 0x0801074D;
                    case 0x08010781: return 0x08010761;
                    case 0x08010B79: return 0x08010B59;
                    case 0x08010ED5: return 0x08010EB5;
                    case 0x08011EC5: return 0x08011EA5;
                    case 0x08012341: return 0x08012321;
                    case 0x08012A71: return 0x08012A51;
                    case 0x08012EC9: return 0x08012EA9;
                    case 0x080132FD: return 0x080132DD;
                    case 0x08013595: return 0x08013579;
                    case 0x08013ABD: return 0x08013AA1;
                    case 0x08013EE1: return 0x08013EC5;
                    case 0x08014565: return 0x08014549;
                    case 0x08014A0D: return 0x080149F1;
                    case 0x08014EBD: return 0x08014EA1;
                    case 0x080151F9: return 0x080151DD;
                    case 0x08015445: return 0x08015429;
                    case 0x08015539: return 0x0801551D;
                    case 0x08015D6D: return 0x08015D51;
                    case 0x0801665D: return 0x08016641;
                    case 0x08016C5D: return 0x08016C41;
                    case 0x080177F9: return 0x080177DD;
                    case 0x08017F4D: return 0x08017F31;
                    case 0x08018139: return 0x0801811D;
                    case 0x08018439: return 0x0801841D;
                    case 0x08018655: return 0x08018639;
                    case 0x0801E5CD: return 0x0801E5A9;
                    case 0x0801ED95: return 0x0801ED71;
                    case 0x0801F431: return 0x0801F40D;
                    case 0x0801F845: return 0x0801F829;
                    case 0x08020161: return 0x08020145;
                    case 0x080209B9: return 0x0802099D;
                    case 0x08020C11: return 0x08020BF5;
                    case 0x080211D9: return 0x080211BD;
                    case 0x0801DCD1: return 0x0801DCB1;
                    case 0x08021549: return 0x0802152D;
                    case 0x08021785: return 0x08021769;
                    case 0x08021985: return 0x08021969;
                    case 0x08021D55: return 0x08021D39;
                    case 0x08021F81: return 0x08021F65;
                    case 0x08022205: return 0x080221E9;
                    case 0x080224D1: return 0x080224B5;
                    case 0x08022EB1: return 0x08022E89;
                    case 0x08023AC9: return 0x08023AA1;
                    case 0x08023EB1: return 0x08023E89;
                    case 0x08024745: return 0x0802471D;
                    case 0x08024F41: return 0x08024F19;
                    case 0x080251E5: return 0x080251BD;
                    case 0x080256B1: return 0x08025689;
                    case 0x08025B85: return 0x08025AFD;
                    case 0x0802680D: return 0x080267A5;
                    case 0x08027615: return 0x080275B1;
                    case 0x08028089: return 0x08028025;
                    case 0x08026B65: return 0x08026B01;
                    case 0x08028599: return 0x08028535;
                    case 0x0802898D: return 0x08028929;
                    case 0x08028B09: return 0x08028AA5;
                    case 0x0802911D: return 0x080290B9;
                    case 0x080293F9: return 0x08029395;
                    case 0x08029AD9: return 0x08029A5D;
                    case 0x0802A085: return 0x08029FE1;
                    case 0x0802A961: return 0x0802A8BD;
                    case 0x0802B0DD: return 0x0802B039;
                    case 0x0802B4D5: return 0x0802B431;
                    case 0x0802B721: return 0x0802B67D;
                    case 0x0802BA81: return 0x0802B9DD;
                    case 0x0802BC41: return 0x0802BB9D;
                    case 0x0802C079: return 0x0802BFD5;
                    case 0x0802C259: return 0x0802C1B5;
                    case 0x0802C5E1: return 0x0802C53D;
                    case 0x0802C749: return 0x0802C6A5;
                    case 0x0802CD11: return 0x0802CC6D;
                    case 0x0802D371: return 0x0802D2CD;
                    case 0x0802D9A5: return 0x0802D901;
                    case 0x0802E359: return 0x0802E2B5;
                    case 0x0802E601: return 0x0802E55D;
                    case 0x0802EB05: return 0x0802EA61;
                    case 0x0802EE0D: return 0x0802ED69;
                    case 0x0802F061: return 0x0802EFBD;
                    case 0x0802F3D9: return 0x0802F335;
                    case 0x0802F619: return 0x0802F575;
                    case 0x0802FB99: return 0x0802FAF5;
                    case 0x08030469: return 0x080303C5;
                    case 0x08030BC5: return 0x08030B21;
                    case 0x0803135D: return 0x080312B9;
                    case 0x08031779: return 0x080316D5;
                    case 0x08031A21: return 0x0803197D;
                    case 0x08031AB1: return 0x08031A0D;
                    case 0x080324B1: return 0x0803240D;
                    case 0x0803283D: return 0x08032799;
                    case 0x08032DE5: return 0x08032D41;
                    case 0x08033375: return 0x080332D1;
                    case 0x08033B1D: return 0x08033A79;
                    case 0x0803410D: return 0x08034069;
                    case 0x080346F1: return 0x0803464D;
                    case 0x08034B6D: return 0x08034AC9;
                    case 0x08034D21: return 0x08034C7D;
                    case 0x08034FDD: return 0x08034F39;
                    case 0x0803544D: return 0x0803539D;
                    case 0x08035F0D: return 0x08035E61;
                    case 0x080369A9: return 0x080368FD;
                    case 0x08036D39: return 0x08036C8D;
                    case 0x08036D5D: return 0x08036CB1;
                    case 0x080370B1: return 0x08037005;
                    case 0x080377D1: return 0x08037725;
                    case 0x08038061: return 0x08037FB5;
                    case 0x08038461: return 0x080383B5;
                    case 0x08038725: return 0x08038679;
                    case 0x0803888D: return 0x080387E1;
                    case 0x080389D5: return 0x08038929;
                    case 0x08039271: return 0x080391C5;
                    case 0x08039C61: return 0x08039BB5;
                    case 0x0803A639: return 0x0803A58D;
                    case 0x0803AA4D: return 0x0803A9A1;
                    case 0x0803ACE9: return 0x0803AC3D;
                    case 0x0803B085: return 0x0803AFD9;
                    case 0x0803B419: return 0x0803B36D;
                    case 0x0803B6D9: return 0x0803B62D;
                    case 0x0803BC91: return 0x0803BBCD;
                    case 0x0803BEED: return 0x0803BE29;
                    case 0x0803CAED: return 0x0803C9B5;
                    case 0x0803CC71: return 0x0803CB39;
                    case 0x0803CF7D: return 0x0803CE45;
                    case 0x08026ED9: return 0x08026E75;
                    case 0x0803D6F9: return 0x0803D5C1;
                    case 0x0803D32D: return 0x0803D1F5;
                    case 0x0803DB3D: return 0x0803DA05;
                    case 0x0803DF49: return 0x0803DE11;
                    case 0x0803C559: return 0x0803C431;
                    case 0x0803C381: return 0x0803C259;
                    case 0x0803C86D: return 0x0803C735;
                    case 0x0803E0F1: return 0x0803DFB9;
                    case 0x0803E445: return 0x0803E30D;
                    case 0x0803E671: return 0x0803E539;
                    case 0x0803EA75: return 0x0803E93D;
                    case 0x0803EEF1: return 0x0803EDB9;
                    case 0x0803F03D: return 0x0803EF05;
                    case 0x0803FA79: return 0x0803F941;
                    case 0x0803FB2D: return 0x0803F9F5;
                    case 0x0803FDE9: return 0x0803FCB1;
                    case 0x080400A5: return 0x0803FF6D;
                    case 0x08040361: return 0x08040229;
                    case 0x08040635: return 0x080404FD;
                    case 0x08040949: return 0x08040811;
                    case 0x08040C05: return 0x08040ACD;
                    case 0x08040F85: return 0x08040E4D;
                    case 0x0804127D: return 0x08041145;
                    case 0x0804159D: return 0x08041465;
                    case 0x0804183D: return 0x08041705;
                    case 0x08041ED1: return 0x08041D99;
                    case 0x080424D9: return 0x080423A1;
                    case 0x080426CD: return 0x08042595;
                }
            }

            return address;
        }
        public static int ConvertAnimSetIndex(GameModeSelection gameMode, int index)
        {
            if (gameMode == GameModeSelection.SpyroAdventureEU)
            {
                switch (index)
                {
                    case 0: return 0;
                    case 2: return 1;
                    case 1: return 2;
                    case 126: return 3;
                    case 95: return 4;
                    case 114: return 5;
                    case 9: return 6;
                    case 62: return 7;
                    case 102: return 8;
                    case 132: return 9;
                    case 8: return 10;
                    case 10: return 11;
                    case 16: return 12;
                    case 109: return 13;
                    case 103: return 14;
                    case 104: return 15;
                    case 115: return 16;
                    case 125: return 17;
                    case 90: return 18;
                    case 116: return 19;
                    case 117: return 20;
                    case 4: return 21;
                    case 12: return 22;
                    case 13: return 23;
                    case 40: return 24;
                    case 78: return 25;
                    case 7: return 26;
                    case 46: return 27;
                    case 96: return 28;
                    case 97: return 29;
                    case 82: return 30;
                    case 21: return 31;
                    case 22: return 32;
                    case 23: return 33;
                    case 24: return 34;
                    case 32: return 35;
                    case 130: return 36;
                    case 128: return 37;
                    case 55: return 38;
                    case 42: return 39;
                    case 73: return 40;
                    case 74: return 41;
                    case 75: return 42;
                    case 93: return 43;
                    case 94: return 44;
                    case 105: return 45;
                    case 123: return 46;
                    case 108: return 47;
                    case 72: return 48;
                    case 88: return 49;
                    case 135: return 50;
                    case 139: return 51;
                    case 142: return 52;
                    case 87: return 53;
                    case 20: return 54;
                    case 6: return 55;
                    case 112: return 56;
                    case 3: return 57;
                    case 30: return 58;
                    case 83: return 59;
                    case 92: return 60;
                    case 41: return 61;
                    case 119: return 62;
                    case 127: return 63;
                    case 58: return 64;
                    case 124: return 65;
                    case 84: return 66;
                    case 98: return 67;
                    case 48: return 68;
                    case 35: return 69;
                    case 85: return 70;
                    case 86: return 71;
                    case 33: return 72;
                    case 5: return 73;
                    case 11: return 74;
                    case 28: return 75;
                    case 113: return 76;
                    case 99: return 77;
                    case 14: return 78;
                    case 79: return 79;
                    case 134: return 80;
                    case 36: return 81;
                    case 47: return 82;
                    case 138: return 83;
                    case 129: return 84;
                    case 56: return 85;
                    case 137: return 86;
                    case 131: return 87;
                    case 77: return 88;
                    case 118: return 89;
                    case 80: return 90;
                    case 17: return 91;
                    case 100: return 92;
                    case 107: return 93;
                    case 140: return 94;
                    case 15: return 95;
                    case 19: return 96;
                    case 66: return 97;
                    case 67: return 98;
                    case 68: return 99;
                    case 69: return 100;
                    case 111: return 101;
                    case 25: return 102;
                    case 26: return 103;
                    case 45: return 104;
                    case 110: return 105;
                    case 106: return 106;
                    case 59: return 107;
                    case 60: return 108;
                    case 63: return 109;
                    case 64: return 110;
                    case 57: return 111;
                    case 27: return 112;
                    case 31: return 113;
                    case 37: return 114;
                    case 38: return 115;
                    case 51: return 116;
                    case 70: return 117;
                    case 81: return 118;
                    case 120: return 119;
                    case 29: return 120;
                    case 49: return 121;
                    case 54: return 122;
                    case 122: return 123;
                    case 71: return 124;
                    case 44: return 125;
                    case 52: return 126;
                    case 133: return 127;
                    case 141: return 128;
                    case 61: return 129;
                    case 89: return 130;
                    case 50: return 131;
                    case 76: return 132;
                    case 43: return 133;
                    case 39: return 134;
                    case 101: return 135;
                    case 136: return 136;
                    case 34: return 137;
                    case 18: return 138;
                    case 91: return 139;
                    case 53: return 140;
                    case 121: return 141;
                    case 157: return 142;
                    case 161: return 143;
                    case 149: return 144;
                    case 144: return 145;
                    case 153: return 146;
                    case 143: return 147;
                    case 145: return 148;
                    case 151: return 149;
                    case 152: return 150;
                    case 150: return 151;
                    case 154: return 152;
                    case 155: return 153;
                    case 158: return 154;
                    case 160: return 155;
                    case 147: return 156;
                    case 156: return 157;
                    case 159: return 158;
                    case 162: return 159;
                    case 146: return 160;
                    case 176: return 161;
                    case 180: return 162;
                    case 163: return 163;
                    case 164: return 164;
                    case 166: return 165;
                    case 167: return 166;
                    case 169: return 167;
                    case 170: return 168;
                    case 171: return 169;
                    case 172: return 170;
                    case 173: return 171;
                    case 174: return 172;
                    case 175: return 173;
                    case 181: return 174;
                    case 183: return 175;
                    case 184: return 176;
                    case 185: return 177;
                    case 186: return 178;
                    case 165: return 179;
                    case 187: return 180;
                    case 179: return 181;
                    case 182: return 182;
                    case 168: return 183;
                    case 177: return 184;
                    case 178: return 185;
                    case 189: return 186;
                    case 190: return 187;
                    case 191: return 188;
                    case 192: return 189;
                    case 193: return 190;
                    case 194: return 191;
                    case 188: return 192;
                    case 195: return 193;
                }
            }
            else if (gameMode == GameModeSelection.SpyroSeasonFlameEU)
            {
                switch (index)
                {
                    case 0: return 0;
                    case 1: return 1;
                    case 2: return 2;
                    //case null: return 3;
                    case 3: return 4;
                    //case null: return 5;
                    //case null: return 6;
                    //case null: return 7;
                    //case null: return 8;
                    case 4: return 9;
                    case 5: return 10;
                    case 6: return 11;
                    case 7: return 12;
                    case 8: return 13;
                    case 9: return 14;
                    case 10: return 15;
                    case 11: return 16;
                    case 12: return 17;
                    case 13: return 18;
                    case 14: return 19;
                    case 15: return 20;
                    case 16: return 21;
                    case 17: return 22;
                    case 18: return 23;
                    case 19: return 24;
                    case 20: return 25;
                    case 22: return 26;
                    case 23: return 27;
                    case 24: return 28;
                    case 25: return 29;
                    case 26: return 30;
                    case 27: return 31;
                    case 28: return 32;
                    case 29: return 33;
                    case 30: return 34;
                    case 31: return 35;
                    case 32: return 36;
                    case 33: return 37;
                    case 34: return 38;
                    case 35: return 39;
                    case 36: return 40;
                    case 37: return 41;
                    case 38: return 42;
                    case 39: return 43;
                    case 40: return 44;
                    case 41: return 45;
                    case 42: return 46;
                    case 43: return 47;
                    case 44: return 48;
                    case 45: return 49;
                    case 46: return 50;
                    case 47: return 51;
                    case 48: return 52;
                    case 49: return 53; // Note: Anim indices are different
                    case 50: return 54;
                    case 51: return 55;
                    case 52: return 56;
                    case 53: return 57;
                    case 54: return 58;
                    case 55: return 59;
                    case 56: return 60;
                    case 57: return 61;
                    case 58: return 62;
                    case 59: return 63;
                    case 60: return 64;
                    case 61: return 65;
                    case 62: return 66;
                    case 63: return 67;
                    case 64: return 68;
                    case 65: return 69;
                    case 66: return 70;
                    case 67: return 71;
                    case 68: return 72;
                    case 69: return 73;
                    case 70: return 74;
                    case 71: return 75;
                    case 72: return 76;
                    case 73: return 77;
                    case 74: return 78;
                    case 75: return 79;
                    case 76: return 80;
                    case 78: return 81;
                    case 79: return 82;
                    case 80: return 83;
                    case 81: return 84;
                    case 82: return 85;
                    case 83: return 86;
                    case 84: return 87;
                    case 85: return 88;
                    case 86: return 89;
                    case 87: return 90;
                    case 88: return 91;
                    case 89: return 92;
                    case 90: return 93;
                    case 91: return 94;
                    case 92: return 95;
                    case 93: return 96;
                    case 94: return 97;
                    case 95: return 98;
                    case 96: return 99;
                    case 97: return 100;
                    case 98: return 101;
                    case 99: return 102;
                    case 100: return 103;
                    case 101: return 104;
                    case 102: return 105;
                    case 103: return 106;
                    case 104: return 107;
                    case 105: return 108;
                    case 106: return 109;
                    case 107: return 110;
                    case 108: return 111;
                    case 109: return 112;
                    case 110: return 113;
                    case 111: return 114;
                    case 112: return 115;
                    case 113: return 116;
                    case 114: return 117;
                    case 115: return 118;
                    case 116: return 119;
                    case 117: return 120;
                    case 118: return 121;
                    case 119: return 122;
                    case 120: return 123;
                    case 121: return 124;
                    case 122: return 125;
                    case 123: return 126;
                    case 124: return 127;
                    case 125: return 128;
                    case 126: return 129;
                    case 127: return 130;
                    case 128: return 131;
                    case 129: return 132;
                    case 130: return 133;
                    case 131: return 134;
                    case 132: return 135;
                    case 133: return 136;
                    case 134: return 137;
                    case 135: return 138;
                    case 136: return 139;
                    case 137: return 140;
                    case 138: return 141;
                    case 139: return 142;
                    case 140: return 143;
                    case 141: return 144;
                    case 142: return 145;
                    case 143: return 146;
                    case 144: return 147;
                    case 145: return 148;
                    case 146: return 149;
                    case 147: return 150;
                    case 148: return 151;
                    case 149: return 152;
                    case 150: return 153;
                    case 151: return 154;
                    case 152: return 155;
                    case 153: return 156;
                    case 154: return 157;
                    case 155: return 158;
                    case 156: return 159;
                    case 157: return 160;
                    case 158: return 161;
                }
            }

            return index;
        }

        private static GBAIsometric_Spyro_ROM GetROM(Unity_Object_GBAIsometricSpyro obj) => obj.ObjManager.Context.GetMainFileObject<GBAIsometric_Spyro_ROM>(((GBAIsometric_Spyro_Manager)obj.ObjManager.Context.Settings.GetGameManager).GetROMFilePath);

        private static void Spyro_NotImplemented(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, -1);
        }
        private static void Spyro_EditorObj(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, -1);
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
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x1A);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_1(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bianca (intro)
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x0C);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_2(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Hunter (intro)
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x32);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_3(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Gem
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x3B);

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

            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, relType < 5 ? 0x0B : 0x77);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_5(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Level portal
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x56);
            obj.AnimationGroupIndex = 0x02;

            var state = GetROM(obj).States_Spyro2_Portals?.FirstOrDefault(x => x.ObjectType == obj.Object.ObjectType);
            var lvl = obj.ObjManager.Context.Settings.Level;

            var lookAtObj = state?.LevelID == lvl ? 0xDF : state?.SpawnerObjectType;

            FaceObj(obj, allObjects, lookAtObj ?? -1);
        }
        private static void Spyro2_6(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bianca
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x0C);
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
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x32);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_9(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Hunter (jumping tutorial)
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x32);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_10(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Fodder
        {
            if (obj.Object.ObjectType == 0x78)
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x67);
                obj.AnimationGroupIndex = 0x02;
            }
            else if (obj.Object.ObjectType == 0x77)
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x53);
                obj.AnimationGroupIndex = 0x01;
            }
            else if (obj.Object.ObjectType == 0x79)
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x3A);
                obj.AnimationGroupIndex = 0x00;
            }
            else
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, -1);
            }
        }
        private static void Spyro2_11(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 NPC
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x37);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_12(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bianca
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x0C);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_13(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Power-up gate
        {
            // Note: Game creates a second obj with same anim at x + 0xC

            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x13);

            if (obj.Object.ObjectType == 0x17C)
                obj.AnimationGroupIndex = 0x00;
            else if (obj.Object.ObjectType == 0x17D)
                obj.AnimationGroupIndex = 0x01;
            else if (obj.Object.ObjectType == 0x17E)
                obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro2_14(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Life
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x36);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_15(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Dragonfly
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x21);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_16(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sparx Panic portal
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x56);
            obj.AnimationGroupIndex = 0x02;

            FaceObj(obj, allObjects, 494);
        }
        private static void Spyro2_17(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sheila NPC
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x68);
            obj.AnimationGroupIndex = 0x05;
        }
        private static void Spyro2_18(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Moneybags
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x51);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_19(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bianca
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x0C);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_20(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Moneybags
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x51);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_21(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Henrietta
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x2F);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_22(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x63);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro2_23(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Locked chest
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x18);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_24(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x6E);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro2_25(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Checkpoint fairy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x39);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_26(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Key
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x3E);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_27(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Daisy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x2F);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_28(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Challenge portal
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x57);
            obj.AnimationGroupIndex = 0x01;

            var state = GetROM(obj).States_Spyro2_ChallengePortals?.FirstOrDefault(x => x.ObjectType == obj.Object.ObjectType);
            var lvl = obj.ObjManager.Context.Settings.Level;

            var lookAtObj = state?.LevelID_1 == lvl ? 0xDF : state?.SpawnerObjectType;

            FaceObj(obj, allObjects, lookAtObj ?? -1);
        }
        private static void Spyro2_29(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Mabel
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x2F);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_30(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Brian
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x45);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_31(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x44);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_32(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x60);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_33(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Boulder
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x10);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_34(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Darby
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x45);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_35(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Dancing horseshoe
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x30);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_36(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Christopher
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x07);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_37(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x1F);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro2_38(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bomb
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x0D);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_39(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x52);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_40(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Lizard fodder
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x46);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_41(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Thief
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x73);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_42(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // NPC
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x07);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_43(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ben
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x1D);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_44(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Vine
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x74);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_45(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x75);
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro2_46(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x76);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_47(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Tommy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x1D);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_48(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Linus
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x1D);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_49(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Crush
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x19);
            obj.AnimationGroupIndex = 0x05;
        }
        private static void Spyro2_50(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Hunter
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x32);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_51(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bianca
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x0C);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_52(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x3F);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro2_53(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x64);
            obj.AnimationGroupIndex = 0x07;
        }
        private static void Spyro2_54(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Caged NPC
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x1E);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_55(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bert
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x08);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_56(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Robby
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x08);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_57(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Cake
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x16);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_58(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Spawning enemies
        {
            if (obj.Object.ObjectType == 0x170)
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x3F);
                obj.AnimationGroupIndex = 0x02;
            }
            else if (obj.Object.ObjectType == 0x175)
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x5D);
                obj.AnimationGroupIndex = 0x02;
            }
            else
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, -1);
            }
        }
        private static void Spyro2_59(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Chatter
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x28);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_60(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x4A);
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro2_61(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x17);
            obj.AnimationGroupIndex = (byte)(obj.ObjManager.Context.Settings.GameModeSelection == GameModeSelection.SpyroSeasonFlameEU ? 0x00 : 0x02);
        }
        private static void Spyro2_62(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Chills
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x28);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_63(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Skull
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x79);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_64(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Andy 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x47);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_65(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Candle 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x4E);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_66(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x6D);
            obj.AnimationGroupIndex = 0x05;
        }
        private static void Spyro2_67(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x61);
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro2_68(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Pierre 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x47);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_69(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Luc 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x47);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_70(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ice hockey player 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x29);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro2_71(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Darren 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x47);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_72(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Minda 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x59);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_73(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x40);
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro2_74(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Moving platform 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x3C);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_75(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Balloon 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x0A);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_76(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Mayor Mooney 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x6B);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_77(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Shirley
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x59);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_78(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Gulp
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x2C);
            obj.AnimationGroupIndex = 0x08;
        }
        private static void Spyro2_79(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Hunter
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x32);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_80(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bianca
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x0C);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_81(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Captain Whiskers
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x6F);
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro2_82(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x5D);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro2_83(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Satellite
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x5B);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_84(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x62);
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro2_85(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Mouser
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x6F);
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro2_86(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Rocketship
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x5E);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_87(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Laura
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x25);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_88(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Siam
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x25);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_89(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Madame Meow
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x25);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_90(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Steffi
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x4C);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_91(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x72);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_92(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Stacey
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x4C);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_93(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ripto
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x5C);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro2_94(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x09);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_95(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Rabbit fodder
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x6A);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_96(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x71);
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro2_97(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x11);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro2_98(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Heating device
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x2E);
            obj.AnimationGroupIndex = 0x01;

            for (int i = 0; i < obj.Object.WaypointCount; i++)
                allObjects[i + obj.Object.WaypointIndex].Object.ObjectType = 0x1BB;
        }
        private static void Spyro2_99(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Blocker
        {
            if (obj.Object.ObjectType == 0x1BB)
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x35);
            else if (obj.Object.ObjectType == 0x1BC)
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x74);
            else
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, -1);

            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro2_100(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x55);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_101(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x50);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro2_102(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Fire canon
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x42);
            obj.AnimationGroupIndex = 0x01;

            for (int i = 0; i < obj.Object.WaypointCount; i++)
                allObjects[i + obj.Object.WaypointIndex].Object.ObjectType = 0x1bC;
        }


        private static void Spyro3_0(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Spyro
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x7E);
            obj.AnimationGroupIndex = 0x13;
        }
        private static void Spyro3_1(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sparx
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x40);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_2(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Gem containers
        {
            var relType = obj.Object.ObjectType - 0x0D;

            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, relType < 5 ? 0x09 : 0x84);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_3(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x68);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_4(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Virtual professor
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x60); // 0x61 when inactive
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_5(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x10);
            obj.AnimationGroupIndex = 0x0B;
        }
        private static void Spyro3_6(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x5A);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_7(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy in gem container
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x66);
            obj.AnimationGroupIndex = 0x02; // Actually defaulted to 0x00, but we do 0x02 so you can see the eyes
        }
        private static void Spyro3_8(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x07);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_9(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bugs which carry you
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x2E);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_10(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Locked chest
        {
            if (obj.Object.ObjectType == 0xa8) // Green
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x15);
            else if(obj.Object.ObjectType == 0xa9) // Pink
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x16);
            else if(obj.Object.ObjectType == 0xa7) // Red
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x17);
            else if (obj.Object.ObjectType == 0xaa) // Yellow
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x18);
            else
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, -1);

            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_11(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x37);
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
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x58);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_14(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Flat switch
        {
            if (obj.Object.ObjectType == 0xba)
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x5E);
                obj.AnimationGroupIndex = 0x01;
            }
            else if (obj.Object.ObjectType == 0xbb)
            {

                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x5D);
                obj.AnimationGroupIndex = 0x02;
            }
            else
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, -1);
            }
        }
        private static void Spyro3_15(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Red timed switch
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x82);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_16(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Timed door
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x20);
            obj.AnimationGroupIndex = 0x00; // TODO: This needs to be changed based on some flags!

            // Change the type of the waypoints (the game creates new objects at the waypoint positions)
            for (int i = 0; i < obj.Object.WaypointCount; i++)
                allObjects[i + obj.Object.WaypointIndex].Object.ObjectType = 0xbc;
        }
        private static void Spyro3_17(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Walrus
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x87);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_18(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ripto
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x6C);
            obj.AnimationGroupIndex = 0x06;
        }
        private static void Spyro3_19(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Library fairy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x2a);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_20(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Burning book
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x48);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_21(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bentley
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x0A);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_22(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Small yeti
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x08);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_23(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Door
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x8E);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_24(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Whistling statue
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x8B);

            if (obj.Object.ObjectType == 0xda)
                obj.AnimationGroupIndex = 0x04;
            else if (obj.Object.ObjectType == 0xd9)
                obj.AnimationGroupIndex = 0x02;
            else if (obj.Object.ObjectType == 0xdb)
                obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_25(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x10);
            obj.AnimationGroupIndex = 0x07;
        }
        private static void Spyro3_26(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Penguin
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x57);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_27(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Fly
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x14);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_28(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Penguin
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x57);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_29(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Gem
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x3e);

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
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x77);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_31(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x7F);
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro3_32(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Moneybags
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x55);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_33(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Safe
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x71);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_34(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Thief
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x53);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_35(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Baby dragon
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x23);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_36(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x9D);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_37(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Banana pod controller
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x05);
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
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x23);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_39(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Baby dragon
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x23);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_40(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Cube
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x63);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_41(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Defeated stealth enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xB4);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_42(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Clown from ground
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x86);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_43(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Numbers
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x02);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_45(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x4D);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_46(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ripto TV
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x83);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_47(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Statue
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x32);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_48(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Potions
        {
            if (obj.Object.ObjectType == 0x17C)
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x42);
            else if (obj.Object.ObjectType == 0x17D)
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x43);
            else if (obj.Object.ObjectType == 0x17E)
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x44);
            else if (obj.Object.ObjectType == 0x17f)
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x45);
            else
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, -1);

            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_49(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Rainy cloud
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x80);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_50(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Professor's contraption
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x85);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_52(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Tutorial objects
        {
            if (obj.Object.ObjectType == 0x1B)
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x09);
                obj.AnimationGroupIndex = 0x01;
            }
            else if (obj.Object.ObjectType == 0x1C)
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x0C);
                obj.AnimationGroupIndex = 0x00;
            }
            else if (obj.Object.ObjectType == 0x1D)
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x84);
                obj.AnimationGroupIndex = 0x01;
            }
            else if (obj.Object.ObjectType == 0x1E)
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x0D);
                obj.AnimationGroupIndex = 0x00;
            }
            else if (obj.Object.ObjectType == 0x1F)
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x04);
                obj.AnimationGroupIndex = 0x00;
            }
            else
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, -1);
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
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x46);
                obj.AnimationGroupIndex = 0x02;
            }
            else if (obj.Object.ObjectType == 616)
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x31);
                obj.AnimationGroupIndex = 0x01;
            }
            else
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, -1);
            }
        }
        private static void Spyro3_55(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sheep
        {
            if (obj.Object.ObjectType == 0x34)
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x75);
            else if (obj.Object.ObjectType == 0x33)
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x73);
            else if (obj.Object.ObjectType == 0x35)
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x74);
            else
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, -1);

            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_56(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Spyro gets teleported
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x88);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_57(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Raft
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x11);
            obj.AnimationGroupIndex = 0x01;

            allObjects[obj.Object.WaypointIndex].AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x12);
            allObjects[obj.Object.WaypointIndex].AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_58(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Boat
        {
            if (obj.Object.ObjectType == 0x26E)
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x2C);
                obj.AnimationGroupIndex = 0x00;
            }
            else if (obj.Object.ObjectType == 0x26D)
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x34);
                obj.AnimationGroupIndex = 0x01;
            }
            else
            {
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, -1);
            }
        }
        private static void Spyro3_59(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Quest item
        {
            var questItem = GetROM(obj).QuestItems.FirstOrDefault(x => x.ObjectType == obj.Object.ObjectType);

            // NOTE: The game creates an object for the chest and sets it to the chest type if the quest item is in a chest. But to avoid us actually changing the type of the object we hard-code the graphics for the specific chest here.
            switch (questItem?.ItemType)
            {
                case GBAIsometric_Spyro3_QuestItem.QuestItemType.RedChest:
                    obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x17);
                    obj.AnimationGroupIndex = 0x00;
                    break;

                case GBAIsometric_Spyro3_QuestItem.QuestItemType.GreenChest:
                    obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x15);
                    obj.AnimationGroupIndex = 0x00;
                    break;

                case GBAIsometric_Spyro3_QuestItem.QuestItemType.PinkChest:
                    obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x16);
                    obj.AnimationGroupIndex = 0x00;
                    break;

                case GBAIsometric_Spyro3_QuestItem.QuestItemType.YellowChest:
                    obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x18);
                    obj.AnimationGroupIndex = 0x00;
                    break;

                default:
                    obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x00);
                    obj.AnimationGroupIndex = 0x01;
                    obj.ForceFrame = questItem?.AnimFrameIndex ?? 0;
                    // Note: The game also creates an objects for animSet 0, group 0 to be behind it as the shining effect
                    break;
            }
        }
        private static void Spyro3_60(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Hat
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x54);
            obj.AnimationGroupIndex = 0x02;
            // Note: The game also creates an objects for the creature inside the hat
        }
        private static void Spyro3_61(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Air vent
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x1F);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_62(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ripto poster
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x06);
            obj.AnimationGroupIndex = 0x01;

            // TODO: Find better solution to this
            obj.ForceHorizontalFlip = obj.Object.Value2;
        }
        private static void Spyro3_63(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Butler
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x13);
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
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x0F);
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
                    wp.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x69);
                    wp.AnimationGroupIndex = (byte)(obj.Object.ObjectType == 0x42 ? 0x01 : 0x03);
                }
                else
                {
                    wp.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, obj.Object.ObjectType != 0x42 && obj.Object.ObjectType != 0x27A ? 0x7B : 0x69);
                }
            }
        }
        private static void Spyro3_67(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Locked chest
        {
            if (obj.Object.ObjectType == 0x1A8) // Green
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x15);
            else if (obj.Object.ObjectType == 0x1A9) // Pink
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x16);
            else if (obj.Object.ObjectType == 0x1A7) // Red
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x17);
            else if (obj.Object.ObjectType == 0x1AA) // Yellow
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x18);
            else
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, -1);

            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_68(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sheep with clothes
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x73);

            if (obj.Object.ObjectType == 0x36)
                obj.AnimationGroupIndex = 0x04;
            else if (obj.Object.ObjectType == 0x37)
                obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro3_69(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ice block
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x7D);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_70(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x67);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_71(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ice obstacle
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x87);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_72(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x7B);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_73(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Green thief
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x53);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_74(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x03);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_75(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Thief NPC
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x56);
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro3_76(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Green race thief
        {
            // Note: The game spawns them from the race controller type (253)
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x53);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_77(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bianca
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x0B);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_78(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x1E);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_79(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // O'Hare
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x4F);
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
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x8C);
            obj.AnimationGroupIndex = 0x03;

            var wp = allObjects[obj.Object.WaypointCount - 1 + obj.Object.WaypointIndex];
            wp.Object.ObjectType = 0x75;
        }
        private static void Spyro3_82(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Banana pod
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x05);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_83(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Albert
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x8C);
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro3_84(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Target
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x6B);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_85(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x7F);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_86(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Janice
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x50);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_87(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ripto statue
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x70);
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro3_88(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Hedgehog
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x38);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_89(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x2F);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_90(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // R statue
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x70);
            obj.AnimationGroupIndex = obj.ObjType.ObjFlags == 0 ? (byte) 0x01 : (byte) 0x02;
        }
        private static void Spyro3_91(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Puzzle
        {
            // Note: The game creates 9 objects here for each piece of the puzzle, with each using a specific frame
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x64);
            obj.AnimationGroupIndex = obj.Object.ObjectType == 0x177 ? (byte) 0x00 : (byte) 0x02;
        }
        private static void Spyro3_92(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Moneybags
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x55);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_93(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Water fountain
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x1D);
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
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x6A);
            obj.AnimationGroupIndex = 0x00;

            for (int i = 0; i < obj.Object.WaypointCount; i++)
            {
                var wp = allObjects[i + obj.Object.WaypointIndex];
                wp.Object.ObjectType = 0xBB;
            }
        }
        private static void Spyro3_96(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Cat
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x1C);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_97(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x8A);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_98(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Food machine
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x36);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_99(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bomb bird
        {
            // Note: Game spawns a bomb object too
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x0E);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_100(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Bruiser
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x1B);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_101(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x81);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_102(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Treadmill
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x33);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_103(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x2F);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_104(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x3D);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_105(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Baby dragon
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x23);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_106(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Blocking drill
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x6A);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_107(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Curtain
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x25);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_108(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ripto spying machine
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x26);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_109(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Ripto portal
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x3B);
            obj.AnimationGroupIndex = 0x12;
        }
        private static void Spyro3_110(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Electric switch
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x46);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_111(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Professor
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x5F);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_112(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Professor
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x5F);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_113(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Statue top
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x6D);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_114(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Baby dragon
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x23);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_115(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Statue bottom
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x6F);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_116(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Final boss statue
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x6C);
            obj.AnimationGroupIndex = 0x10;
        }
        private static void Spyro3_117(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Final boss throne
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x6C);
            obj.AnimationGroupIndex = 0x15;
        }
        private static void Spyro3_118(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Poisoned water
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x47);
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
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x6E);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_121(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 air vent 
        {
            // Note: Game also spawns Sparx here
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xB1);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_122(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 sign post 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xB2);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_123(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 locked doors
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xA4);

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
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xB7);
            obj.AnimationGroupIndex = 0x05;
        }
        private static void Spyro3_125(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 gem
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xA6);

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
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xB7);
            obj.AnimationGroupIndex = 0x0A;
        }
        private static void Spyro3_127(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 guard popping out from pipe 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xBA);
            obj.AnimationGroupIndex = 0x03;
        }
        private static void Spyro3_128(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 prisoner 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xB3);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_129(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 locked door key 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xA9);

            if (obj.Object.ObjectType == 0x24B)
                obj.AnimationGroupIndex = 0x03;
            else if (obj.Object.ObjectType == 0x24D)
                obj.AnimationGroupIndex = 0x01;
            else if (obj.Object.ObjectType == 0x24F)
                obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_130(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 camera 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xB9);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_131(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 flying enemy 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xB5);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_132(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 checkpoint fairy 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x3C);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_133(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 gem container 
        {
            if (obj.Object.ObjectType == 0x1F9 || obj.Object.ObjectType == 0x1FA || obj.Object.ObjectType == 0x1FB || obj.Object.ObjectType == 0x1FC || obj.Object.ObjectType == 0x1FD)
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xA3);
            else if (obj.Object.ObjectType == 0x1FE || obj.Object.ObjectType == 0x1FF || obj.Object.ObjectType == 0x200 || obj.Object.ObjectType == 0x201 || obj.Object.ObjectType == 0x202)
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xAF);
            else
                obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, -1);

            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_134(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 moving platform 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xAB);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_135(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 mines 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xAA);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_136(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xB8);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_137(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Agent 9 spying enemy
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xB7);
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_138(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd air vent 
        {
            // Note: Game also spawns Sparx here
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x9E);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_139(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd gem 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0xA6);

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
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x90);
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_141(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd flying enemy 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x91);
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_44(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd shooting enemy
        {
            if (obj.ObjManager.Context.Settings.GameModeSelection == GameModeSelection.SpyroAdventureUS)
            {
                obj.AnimSetIndex = 0xA1;

                if (obj.Object.ObjectType == 0x167)
                    obj.AnimationGroupIndex = 0x05;
                else if (obj.Object.ObjectType == 0x168)
                    obj.AnimationGroupIndex = 0x04;
                else if (obj.Object.ObjectType == 0x169)
                    obj.AnimationGroupIndex = 0x02;
            }
            else
            {
                obj.AnimSetIndex = 0x8F;

                if (obj.Object.ObjectType == 0x167)
                    obj.AnimationGroupIndex = 0x04;
                else if (obj.Object.ObjectType == 0x304)
                    obj.AnimationGroupIndex = 0x05;
            }
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

            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x96);
            
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
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x9A);

            if (obj.Object.ObjectType == 0x2D3)
                obj.AnimationGroupIndex = 0x05;
            else if (obj.Object.ObjectType == 0x2D4)
                obj.AnimationGroupIndex = 0x03;
            else if (obj.Object.ObjectType == 0x2D5)
                obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_144(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd enemy
        {
            if (obj.ObjManager.Context.Settings.GameModeSelection == GameModeSelection.SpyroAdventureUS)
            {
                obj.AnimSetIndex = 0xA1;

                if (obj.Object.ObjectType == 0x168)
                    obj.AnimationGroupIndex = 0x04;
                else if (obj.Object.ObjectType == 0x169)
                    obj.AnimationGroupIndex = 0x02;
            }
            else
            {
                obj.AnimSetIndex = 0x8F;

                if (obj.Object.ObjectType == 0x168)
                    obj.AnimationGroupIndex = 0x03;
                else if (obj.Object.ObjectType == 0x169)
                    obj.AnimationGroupIndex = 0x02;
            }
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
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x9B);

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
            wp.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x9C);
            wp.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_148(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Sgt. Byrd enemy 
        {
            obj.AnimSetIndex = ConvertAnimSetIndex(obj.ObjManager.Context.Settings.GameModeSelection, 0x91);
            obj.AnimationGroupIndex = 0x00;
        }
    }
}