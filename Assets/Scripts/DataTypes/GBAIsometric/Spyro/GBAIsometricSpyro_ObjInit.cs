using System;

namespace R1Engine
{
    public static class GBAIsometricSpyro_ObjInit
    {
        public static Action<Unity_Object_GBAIsometricSpyro> GetInitFunc(GameSettings settings, uint address)
        {
            if (settings.EngineVersion == EngineVersion.GBAIsometric_Spyro3 && settings.GameModeSelection == GameModeSelection.SpyroAdventureUS)
            {
                switch (address)
                {
                    case 0x0801580D:
                        return Spyro3_NotImplemented;
                    case 0x080159BD:
                        return Spyro3_NotImplemented;
                    case 0x080159E1:
                        return Spyro3_NotImplemented;
                    case 0x08015BBD:
                        return Spyro3_0;
                    case 0x0801A20D:
                        return Spyro3_1;
                    case 0x0801A565:
                        return Spyro3_NotImplemented;
                    case 0x0801AC2D:
                        return Spyro3_2;
                }
            }

            return x => x.AnimSetIndex = -1;
        }

        private static void Spyro3_NotImplemented(Unity_Object_GBAIsometricSpyro obj)
        {
            obj.AnimSetIndex = -1;
        }
        private static void Spyro3_0(Unity_Object_GBAIsometricSpyro obj) // Spyro
        {
            obj.AnimSetIndex = 0x7E;
            obj.AnimationGroupIndex = 0x13;
        }
        private static void Spyro3_1(Unity_Object_GBAIsometricSpyro obj) // Sparx
        {
            obj.AnimSetIndex = 0x40;
            obj.AnimationGroupIndex = 0x02;
        }
        private static void Spyro3_2(Unity_Object_GBAIsometricSpyro obj) // Gem containers
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
    }
}