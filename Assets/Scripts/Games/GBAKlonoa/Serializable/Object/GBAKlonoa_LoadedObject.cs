﻿using System.Collections.Generic;

namespace Ray1Map.GBAKlonoa
{
    public class GBAKlonoa_LoadedObject
    {
        public GBAKlonoa_LoadedObject(short index, byte oamIndex, short xPos, short yPos, short param_1, byte value6, byte param_2, byte value8, byte objType, GBAKlonoa_LevelObject levelObj = null, GBAKlonoa_WorldMapObject worldMapObj = null, byte? dct_GraphicsIndex = null)
        {
            Index = index;
            OAMIndex = oamIndex;
            XPos = xPos;
            YPos = yPos;
            Param_1 = param_1;
            Value_6 = value6;
            Param_2 = param_2;
            Value_8 = value8;
            ObjType = objType;
            LevelObj = levelObj;
            WorldMapObj = worldMapObj;
            DCT_GraphicsIndex = dct_GraphicsIndex;
        }

        public short Index { get; }
        public byte OAMIndex { get; }
        public short XPos { get; set; }
        public short YPos { get; set; }
        public short ZPos
        {
            get => Param_1;
            set => Param_1 = value;
        }
        public short Param_1 { get; set; } // ZPos for maps
        public byte Value_6 { get; set; }
        public byte Param_2 { get; set; }
        public byte Value_8 { get; set; }
        public byte ObjType { get; }
        public GBAKlonoa_LevelObject LevelObj { get; }
        public GBAKlonoa_WorldMapObject WorldMapObj { get; }
        public byte? DCT_GraphicsIndex { get; }

        public static IEnumerable<GBAKlonoa_LoadedObject> GetFixedObjects(EngineVersion engineVersion, int world, int level)
        {
            if (engineVersion == EngineVersion.KlonoaGBA_EOD)
            {
                // Klonoa
                yield return new GBAKlonoa_LoadedObject(0, 0, 0, 0, 0, 0, 0, 0, 0x6e);

                // Effects (item collected etc.)
                yield return new GBAKlonoa_LoadedObject(1, 1, 0, 0, 0, 0, 0, 0x1c, 0x34);
                yield return new GBAKlonoa_LoadedObject(2, 2, 0, 0, 0, 0, 0, 0x1c, 0x34);
                yield return new GBAKlonoa_LoadedObject(3, 3, 0, 0, 0, 0, 0, 0x1c, 0x34);
                yield return new GBAKlonoa_LoadedObject(4, 4, 0, 0, 0, 0, 0, 0x1c, 0x34);
                yield return new GBAKlonoa_LoadedObject(5, 5, 0, 0, 0, 0, 0, 0x1c, 0x34);
                yield return new GBAKlonoa_LoadedObject(6, 6, 0, 0, 0, 0, 0, 0x1c, 0x34);
                yield return new GBAKlonoa_LoadedObject(7, 7, 0, 0, 0, 0, 0, 0x1c, 0x34);
                yield return new GBAKlonoa_LoadedObject(8, 8, 0, 0, 0, 0, 0, 0x1c, 0x34);

                // Klonoa's attack
                yield return new GBAKlonoa_LoadedObject(9, 9, 0, 0, 0, 0, 0, 0, 0);
                yield return new GBAKlonoa_LoadedObject(10, 10, 0, 0, 0, 0, 0, 0, 0);

                // Level text (for example: VISION 1-1)
                yield return new GBAKlonoa_LoadedObject(0xb, 0xb, -32, 60, 0, 0, 0, 0, 0);
                yield return new GBAKlonoa_LoadedObject(0xc, 0xc, -32, 116, 0, 0, 0, 0, 0);
            }
            else
            {
                if (level == 4 || ((world == 4 && ((level == 5 || level == 1) || level == 7))))
                {
                    // Klonoa
                    // Third to last param gets set to ((uint)DAT_03002f10 << 0x1c) >> 0x1e
                    yield return new GBAKlonoa_LoadedObject(0, 0xd, 0, 0, 0, 1, 0, 0, 0x6e);
                }
                else
                {
                    // Klonoa
                    // Third to last param gets set to ((uint)DAT_03002f10 << 0x1c) >> 0x1e
                    yield return new GBAKlonoa_LoadedObject(0, 0, 0, 0, 0, 1, 0, 0, 0x6e);
                }

                // Effects (item collected etc.)
                yield return new GBAKlonoa_LoadedObject(1, 1, 0, 0, 0, 1, 0, 0x1f, 0x3c);
                yield return new GBAKlonoa_LoadedObject(2, 2, 0, 0, 0, 1, 0, 0x1f, 0x3c);
                yield return new GBAKlonoa_LoadedObject(3, 3, 0, 0, 0, 1, 0, 0x1f, 0x3c);
                yield return new GBAKlonoa_LoadedObject(4, 4, 0, 0, 0, 1, 0, 0x1f, 0x3c);
                yield return new GBAKlonoa_LoadedObject(5, 5, 0, 0, 0, 1, 0, 0x1f, 0x3c);
                yield return new GBAKlonoa_LoadedObject(6, 6, 0, 0, 0, 1, 0, 0x1f, 0x3c);
                yield return new GBAKlonoa_LoadedObject(7, 7, 0, 0, 0, 1, 0, 0x1f, 0x3c);
                yield return new GBAKlonoa_LoadedObject(8, 8, 0, 0, 0, 1, 0, 0x1f, 0x3c);

                // Klonoa's attack
                yield return new GBAKlonoa_LoadedObject(9, 9, 0, 0, 0, 1, 0, 0, 0);
                yield return new GBAKlonoa_LoadedObject(10, 10, 0, 0, 0, 1, 0, 0, 0);

                // Level text (for example: VISION 1-1)
                if (level == 4)
                {
                    yield return new GBAKlonoa_LoadedObject(0xb, 0xb, -32, 60, 0, 1, 0, 0, 0);
                    yield return new GBAKlonoa_LoadedObject(0xc, 0xc, -32, 116, 0, 1, 0, 0, 0);
                }
                else
                {
                    yield return new GBAKlonoa_LoadedObject(0xb, 0xb, -32, 60, 0, 0, 0, 0, 0);
                    yield return new GBAKlonoa_LoadedObject(0xc, 0xc, -32, 116, 0, 0, 0, 0, 0);
                }
            }
        }
    }
}