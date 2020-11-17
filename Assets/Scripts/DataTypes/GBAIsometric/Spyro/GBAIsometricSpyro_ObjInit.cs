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
                    case 0x080159E1:
                    case 0x0801A565:
                    case 0x0801B259: // Has graphics
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
                    case 0x08022785: // Has graphics
                    case 0x080236B1:
                    case 0x08023B7D: // Has graphics (special effects?)
                        return Spyro3_NotImplemented;

                    case 0x08015BBD:
                        return Spyro3_0;
                    case 0x0801A20D:
                        return Spyro3_1;
                    case 0x0801AC2D:
                        return Spyro3_2;
                    case 0x0801B0B1:
                        return Spyro3_3;
                    case 0x0801B5BD:
                        return Spyro3_4;
                    case 0x0801C9A1:
                        return Spyro3_5;
                    case 0x0801CFD5:
                        return Spyro3_6;
                    case 0x0801D4B5:
                        return Spyro3_7;
                    case 0x0801DD85:
                        return Spyro3_8;
                    case 0x0801F48D:
                        return Spyro3_9;
                    case 0x08020D39:
                        return Spyro3_10;
                    case 0x08020EB9:
                        return Spyro3_11;
                    case 0x0802102D:
                        return Spyro3_12;
                    case 0x08021B21:
                        return Spyro3_13;
                    case 0x08021E25:
                        return Spyro3_14;
                    case 0x08022115:
                        return Spyro3_15;
                    case 0x08022229:
                        return Spyro3_16;
                    case 0x08023009:
                        return Spyro3_17;
                    case 0x08023409:
                        return Spyro3_18;
                    case 0x080237E1:
                        return Spyro3_19;
                    case 0x080239E1:
                        return Spyro3_20;
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
    }
}