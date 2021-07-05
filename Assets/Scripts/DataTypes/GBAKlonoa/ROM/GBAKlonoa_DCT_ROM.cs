using BinarySerializer;
using BinarySerializer.GBA;

namespace R1Engine
{
    public class GBAKlonoa_DCT_ROM : GBA_ROMBase
    {
        // Maps
        public GBAKlonoa_DCT_Map[] Maps { get; set; }
        public RGBA5551Color[] FixTilePalette { get; set; }
        public GBAKlonoa_MapSectors[] MapSectors { get; set; }

        // TODO: Use pointer tables
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            var settings = s.GetR1Settings();
            var globalLevelIndex = GBAKlonoa_DCT_Manager.GetGlobalLevelIndex(settings.World, settings.Level);
            const int normalWorldsCount = GBAKlonoa_DCT_Manager.NormalWorldsCount;
            const int levelsCount = GBAKlonoa_DCT_Manager.LevelsCount;
            const int normalLevelsCount = GBAKlonoa_DCT_Manager.NormalLevelsCount;

            // Serialize maps
            s.DoAt(new Pointer(0x8052AFC, Offset.File), () =>
            {
                Maps ??= new GBAKlonoa_DCT_Map[levelsCount];

                for (int i = 0; i < Maps.Length; i++)
                    Maps[i] = s.SerializeObject<GBAKlonoa_DCT_Map>(Maps[i], x => x.Pre_SerializeData = i == globalLevelIndex, name: $"{nameof(Maps)}[{i}]");
            });

            s.DoAt(new Pointer(0x083514e4, Offset.File), () => FixTilePalette = s.SerializeObjectArray<RGBA5551Color>(FixTilePalette, 0x20, name: nameof(FixTilePalette)));

            // Serialize map sectors
            s.DoAt(new Pointer(0x0810a480, Offset.File), () => MapSectors = s.SerializeObjectArray<GBAKlonoa_MapSectors>(MapSectors, normalLevelsCount, name: nameof(MapSectors)));
        }
    }
}