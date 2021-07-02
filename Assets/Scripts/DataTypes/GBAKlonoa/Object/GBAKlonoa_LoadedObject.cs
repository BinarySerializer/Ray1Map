namespace R1Engine
{
    public class GBAKlonoa_LoadedObject
    {
        public GBAKlonoa_LoadedObject(short index, byte oamIndex, short xPos, short yPos, byte param_1, byte value6, byte param_2, byte value8, byte objType, GBAKlonoa_LevelObject levelObj = null, GBAKlonoa_WorldMapObject worldMapObj = null)
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
        }

        public short Index { get; }
        public byte OAMIndex { get; }
        public short XPos { get; set; }
        public short YPos { get; set; }
        public byte Param_1 { get; set; } // ZPos for maps
        public byte Value_6 { get; }
        public byte Param_2 { get; }
        public byte Value_8 { get; }
        public byte ObjType { get; }
        public GBAKlonoa_LevelObject LevelObj { get; }
        public GBAKlonoa_WorldMapObject WorldMapObj { get; }

        public static GBAKlonoa_LoadedObject[] FixedObjects => new GBAKlonoa_LoadedObject[]
        {
            // Klonoa
            new GBAKlonoa_LoadedObject(0,0,0,0,0,0,0,0,0x6e),
            
            // Effects (item collected etc.)
            new GBAKlonoa_LoadedObject(1,1,0,0,0,0,0,0x1c,0x34),
            new GBAKlonoa_LoadedObject(2,2,0,0,0,0,0,0x1c,0x34),
            new GBAKlonoa_LoadedObject(3,3,0,0,0,0,0,0x1c,0x34),
            new GBAKlonoa_LoadedObject(4,4,0,0,0,0,0,0x1c,0x34),
            new GBAKlonoa_LoadedObject(5,5,0,0,0,0,0,0x1c,0x34),
            new GBAKlonoa_LoadedObject(6,6,0,0,0,0,0,0x1c,0x34),
            new GBAKlonoa_LoadedObject(7,7,0,0,0,0,0,0x1c,0x34),
            new GBAKlonoa_LoadedObject(8,8,0,0,0,0,0,0x1c,0x34),
            
            // Klonoa's attack
            new GBAKlonoa_LoadedObject(9,9,0,0,0,0,0,0,0),
            new GBAKlonoa_LoadedObject(10,10,0,0,0,0,0,0,0),

            // Level text (for example: VISION 1-1)
            new GBAKlonoa_LoadedObject(0xb,0xb,-32,60,0,0,0,0,0),
            new GBAKlonoa_LoadedObject(0xc,0xc,-32,116,0,0,0,0,0),
        };
    }
}