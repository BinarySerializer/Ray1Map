using System;
using System.Linq;

namespace R1Engine
{
    public static class GBAIsometricSpyro_ObjInit
    {
        public static Action<Unity_Object_GBAIsometricSpyro, Unity_Object_GBAIsometricSpyro[]> GetInitFunc(GameSettings settings, uint address)
        {
            if (settings.EngineVersion == EngineVersion.GBAIsometric_Spyro3 && settings.GameModeSelection == GameModeSelection.SpyroAdventureUS)
            {
                switch (address)
                {
                    // TODO: Implement types
                    case 0x0801580D:
                    case 0x080159BD:
                    case 0x0801B499:
                    case 0x0801B9BD: // Has graphics
                    case 0x0801BB75: // Has graphics
                    case 0x0801C269: // Has graphics
                    case 0x0801DA1D: // Has graphics (sheep)
                    case 0x0801DC7D: // Has graphics
                    case 0x0801E1C1: // Has graphics
                    case 0x0801E855: // Has graphics
                    case 0x0801F9FD: // Has graphics (boss which consists of multiple parts?)
                    case 0x08020859: // Has graphics (ui?)
                    case 0x08021509: // Has graphics (0x3A?)
                    case 0x080236B1:
                    case 0x08023B7D: // Has graphics (special effects?)
                    case 0x08024BE5:
                    case 0x08024F21: // Has graphics
                    case 0x080250C9: // Has graphics?
                    case 0x080258D5:
                    case 0x08025C9D:
                    case 0x0802641D: // Has graphics (007B0001?)
                    case 0x08026C91: // Has graphics
                    case 0x0802756D: // Has graphics
                    case 0x08027875: // Has graphics?
                    case 0x08027F55:
                    case 0x0802825D: // Has graphics
                    case 0x08028815: // Has graphics
                    case 0x08028DCD: // Has graphics
                    case 0x080292BD: // Has graphics (Sgt. Byrd UI?)
                    case 0x0802A805:
                    case 0x0802B271: // Has graphics
                    case 0x0802B505: // Has graphics (Ripto statue)
                    case 0x0802B779:
                    case 0x0802BF4D: // Has graphics (Thief? 00560003?)
                    case 0x0802C809:
                    case 0x0802F65D: // Has graphics
                    case 0x0802FDD5: // Has graphics
                    case 0x080307C1: // Has graphics (waypoints init with type 0x153)
                    case 0x08030FE1: // Has graphics (0x4F with group 0, 1 or 2)
                    case 0x080315B5: // Has graphics?
                    case 0x08031979:
                    case 0x08031DDD: // Has graphics?
                    case 0x08032049: // Has graphics
                    case 0x08032949:
                    case 0x08032DC5: // Has graphics
                    case 0x08033339: // Has graphics
                    case 0x080338A5: // Has graphics
                    case 0x08033DE1: // Has graphics
                    case 0x080345D5: // Has graphics
                    case 0x08034CD5: // Has graphics
                    case 0x0803509D:
                    case 0x0803574D: // Has graphics
                    case 0x08036319:
                    case 0x080363F5:
                    case 0x08036471:
                    case 0x080370D1:
                    case 0x08037441: // Has graphics
                    case 0x080376A9:
                        // Check this:
                    case 0x080383BD:
                    case 0x080384ED:
                    case 0x080386E5:
                    case 0x08038ABD:
                    case 0x08038E11:
                    case 0x0803966D:
                    case 0x08039D51:
                    case 0x0803A02D:
                    case 0x0803A421:
                    case 0x0803A62D:
                    case 0x0803A785:
                    case 0x0803A8A9:
                    case 0x0803AB49:
                    case 0x0803AED9:
                    case 0x0803B3A5:
                    case 0x0803B681:
                    case 0x0803B781:
                    case 0x0803B981:
                    case 0x0803BE0D:
                    case 0x0803BF91:
                    case 0x0803C43D:
                    case 0x0803C7FD:
                    case 0x0803CAE9:
                    case 0x0803CB39:
                    case 0x0803D0F1:
                    case 0x0803D829:
                    case 0x0803DC59:
                    case 0x0803E255:
                    case 0x0803E559:
                    case 0x0803E6DD:
                    case 0x0803E925:
                    case 0x0803EA1D:
                    case 0x0803ED41:
                    case 0x0803F255:
                    case 0x0803F3FD:
                    case 0x0803F58D:
                    case 0x0803FC61:
                    case 0x0803FFB1:
                    case 0x080401C1:
                    case 0x080404AD:
                    case 0x08040571:
                    case 0x0804063D:
                    case 0x08040925:
                    case 0x08040A61:
                    case 0x08040CB5:
                    case 0x08040DD9:
                    case 0x08041DA5:
                    case 0x080424AD:
                    case 0x08042585:
                    case 0x08042725:
                    case 0x080428BD:
                    case 0x080429F1:
                    case 0x08042D99:
                    case 0x080430C5:
                    case 0x08043235:
                    case 0x080438F1:
                    case 0x08043C59:
                    case 0x08043E35:
                    case 0x08043EAD:
                    case 0x080448D1:
                    case 0x080449B9:
                    case 0x08044AD5:
                    case 0x0801D1A5:
                    case 0x08044D81:
                    case 0x080450BD:
                    case 0x080454F5:
                    case 0x08045659:
                    case 0x080459B1:
                    case 0x08045D5D:
                    case 0x08045ED1:
                    case 0x08046161:
                    case 0x08046271:
                    case 0x08046475:
                    case 0x080465B1:
                    case 0x08046945:
                    case 0x08046A15:
                    case 0x08046FC5:
                    case 0x080470A9:
                    case 0x08047101:
                    case 0x0804739D:
                    case 0x08047651:
                        return Spyro3_NotImplemented;

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
                    case 0x08033541: return Spyro3_44;
                    case 0x08034765: return Spyro3_45;
                    case 0x0803497D: return Spyro3_46;
                    case 0x08035219: return Spyro3_47;
                    case 0x08035BA1: return Spyro3_48;
                    case 0x0803602D: return Spyro3_49;
                    case 0x08043A79: return Spyro3_50;
                    case 0x080159E1: return Spyro3_51;
                    case 0x0801B259: return Spyro3_52;
                    case 0x08022785: return Spyro3_53;
                }
            }

            return (x, y) => x.AnimSetIndex = -1;
        }

        private static void Spyro3_NotImplemented(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = -1;
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
            if (obj.Object.ObjectType == 13)
            {
                obj.AnimSetIndex = 0x09;
                obj.AnimationGroupIndex = 0x01;
            }
            else
            {
                obj.AnimSetIndex = 0x84;
                obj.AnimationGroupIndex = 0x01;
            }
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

            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_11(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = 0x37;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_12(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // NPC
        {
            var npcStates = obj.ObjManager.Context.GetMainFileObject<GBAIsometric_Spyro_ROM>(((GBAIsometric_Spyro_Manager)obj.ObjManager.Context.Settings.GetGameManager).GetROMFilePath).States_NPC;
            var state = npcStates.FirstOrDefault(x => x.ObjectType == obj.Object.ObjectType);

            if (state != null)
            {
                obj.AnimSetIndex = state.AnimSetIndex;
                obj.AnimationGroupIndex = (byte)state.AnimationGroupIndex;
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
        private static void Spyro3_37(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Banana pod
        {
            obj.AnimSetIndex = 0x05;
            obj.AnimationGroupIndex = 0x03;

            if (obj.Object.ObjectType == 0x195)
            {
                allObjects[obj.Object.WaypointIndex + 0].Object.ObjectType = 0x196;
                allObjects[obj.Object.WaypointIndex + 1].Object.ObjectType = 0x197;
            }
            else
            {
                allObjects[obj.Object.WaypointIndex + 0].Object.ObjectType = 0x128;
                allObjects[obj.Object.WaypointIndex + 1].Object.ObjectType = 0x128;
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
        private static void Spyro3_40(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Flash effect
        {
            obj.AnimSetIndex = 0x4E;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_41(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Defeated stealth enemy
        {
            obj.AnimSetIndex = 0xB4;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_42(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Clown
        {
            obj.AnimSetIndex = 0x86;
            obj.AnimationGroupIndex = 0x01;
        }
        private static void Spyro3_43(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Numbers
        {
            obj.AnimSetIndex = 0x02;
            obj.AnimationGroupIndex = 0x00;
        }
        private static void Spyro3_44(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects)
        {
            obj.AnimSetIndex = 0xA1;

            if (obj.Object.ObjectType == 0x167)
            {
                obj.AnimationGroupIndex = 0x05;
            }
            else if (obj.Object.ObjectType == 0x168)
            {
                obj.AnimationGroupIndex = 0x04;
            }
            else if (obj.Object.ObjectType == 0x169)
            {
                obj.AnimationGroupIndex = 0x02;
            }
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
        private static void Spyro3_51(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Spanwer
        {
            // TODO: Implement
            // Can spawn Spyro, baby dragon etc.
            obj.AnimSetIndex = -1;
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
        }
        private static void Spyro3_53(Unity_Object_GBAIsometricSpyro obj, Unity_Object_GBAIsometricSpyro[] allObjects) // Door
        {
            var rom = obj.ObjManager.Context.GetMainFileObject<GBAIsometric_Spyro_ROM>(((GBAIsometric_Spyro_Manager)obj.ObjManager.Context.Settings.GetGameManager).GetROMFilePath);
            var levID = obj.ObjManager.Context.Settings.Level;
            var typeState = rom.States_DoorTypes.FirstOrDefault(x => x.ObjectType == obj.Object.ObjectType);

            long graphicsID;
            if (typeState?.LevelID == levID)
                graphicsID = typeState?.GraphicsStateID1 ?? -1;
            else
                graphicsID = typeState?.GraphicsStateID2 ?? -1;

            var graphicsState = rom.States_DoorGraphics.FirstOrDefault(x => x.ID == graphicsID);

            if (graphicsState != null)
            {
                obj.AnimSetIndex = graphicsState.AnimSetIndex;
                obj.AnimationGroupIndex = (byte)graphicsState.AnimationGroupIndex;
            }
        }
    }
}