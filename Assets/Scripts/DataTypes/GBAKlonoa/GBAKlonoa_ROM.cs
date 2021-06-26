using BinarySerializer;
using BinarySerializer.GBA;

namespace R1Engine
{
    public class GBAKlonoa_ROM : GBA_ROMBase
    {
        // Maps
        public GBAKlonoa_Maps[] Maps { get; set; }
        public GBAKlonoa_MapWidths[] MapWidths { get; set; }
        public GBAKlonoa_MapSectors[] MapSectors { get; set; }
        public GBAKlonoa_TileSets[] TileSets { get; set; }
        public Pointer[] MapPalettePointers { get; set; }
        public RGBA5551Color[][] MapPalettes { get; set; }

        // Objects
        public GBAKlonoa_Objects Objects { get; set; } // For current level only - too slow to read all of them

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            var settings = s.GetR1Settings();
            var globalLevel = (settings.World - 1) * 9 + settings.Level;
            const int levelsCount = 54;
            const int normalLevelsCount = 42;

            // Serialize maps
            s.DoAt(new Pointer(0x081892BC, Offset.File), () =>
            {
                Maps ??= new GBAKlonoa_Maps[levelsCount];

                for (int i = 0; i < Maps.Length; i++)
                    Maps[i] = s.SerializeObject<GBAKlonoa_Maps>(Maps[i], x => x.Pre_SerializeData = i == globalLevel, name: $"{nameof(Maps)}[{i}]");
            });

            // Serialize map widths
            s.DoAt(new Pointer(0x08051c76, Offset.File), () => MapWidths = s.SerializeObjectArray<GBAKlonoa_MapWidths>(MapWidths, levelsCount, name: nameof(MapWidths)));
            
            // Serialize map sectors
            s.DoAt(new Pointer(0x080d2e88, Offset.File), () => MapSectors = s.SerializeObjectArray<GBAKlonoa_MapSectors>(MapSectors, normalLevelsCount, name: nameof(MapSectors)));

            // Serialize tile sets
            s.DoAt(new Pointer(0x08189034, Offset.File), () =>
            {
                TileSets ??= new GBAKlonoa_TileSets[levelsCount];

                for (int i = 0; i < TileSets.Length; i++)
                    TileSets[i] = s.SerializeObject<GBAKlonoa_TileSets>(TileSets[i], x => x.Pre_SerializeData = i == globalLevel, name: $"{nameof(TileSets)}[{i}]");
            });

            // Serialize palettes
            s.DoAt(new Pointer(0x08188f5C, Offset.File), () => MapPalettePointers = s.SerializePointerArray(MapPalettePointers, levelsCount, name: nameof(MapPalettePointers)));

            MapPalettes ??= new RGBA5551Color[MapPalettePointers.Length][];
            s.DoAt(MapPalettePointers[globalLevel], () =>
            {
                s.DoEncoded(new GBAKlonoa_Encoder(), () =>
                {
                    MapPalettes[globalLevel] = s.SerializeObjectArray<RGBA5551Color>(MapPalettes[globalLevel], 256, name: $"{nameof(MapPalettes)}[{globalLevel}]");
                });
            });

            // Serialize objects
            if (settings.Level == 0)
            {
                // TODO: World map objects
            }
            else
            {
                // Each level has 100 object slots, each world has 8 level slots and each object is 44 bytes
                var lvlObjOffset = ((settings.World - 1) * 800 + (settings.Level - 1) * 100) * 44;

                s.DoAt(new Pointer(0x080e2b64 + lvlObjOffset, Offset.File), () => Objects = s.SerializeObject<GBAKlonoa_Objects>(Objects, name: nameof(Objects)));
            }
        }
    }
}