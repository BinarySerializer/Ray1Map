using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.GBA;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_ROM : GBAIsometric_IceDragon_BaseROM
    {
        public bool Pre_SerializePortraits { get; set; }

        // TODO: Title screen map
        // TODO: Isometric levels
        // TODO: Sparx levels
        // TODO: Mode7 levels
        // TODO: Level maps (JP only)

        // Portraits
        public Pointer[] PortraitPalettePointers { get; set; }
        public Palette[] PortraitPalettes { get; set; }
        public Pointer[] PortraitTileMapPointers { get; set; }
        public BinarySerializer.GBA.MapTile[][] PortraitTileMaps { get; set; }
        public Pointer[] PortraitTileSetPointers { get; set; }
        public byte[][] PortraitTileSets { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header and base data
            base.SerializeImpl(s);

            GBAIsometricSettings settings = s.GetSettings<GBAIsometricSettings>();
            Dictionary<Spyro_DefinedPointer, Pointer> pointerTable = PointerTables.GBAIsometric_Spyro_PointerTable(s.GetR1Settings().GameModeSelection, Offset.File);

            if (Pre_SerializePortraits)
                SerializePortraits(s, settings, pointerTable);
        }

        private void SerializePortraits(
            SerializerObject s, 
            GBAIsometricSettings settings, 
            Dictionary<Spyro_DefinedPointer, Pointer> pointerTable)
        {
            int count = settings.PortraitsCount;

            s.DoAt(pointerTable.TryGetItem(Spyro_DefinedPointer.Ice_PortraitPalettes), () =>
                PortraitPalettePointers = s.SerializePointerArray(PortraitPalettePointers, count, name: nameof(PortraitPalettePointers)));

            PortraitPalettes ??= new Palette[count];

            for (int i = 0; i < PortraitPalettes.Length; i++)
                s.DoAt(PortraitPalettePointers[i], () =>
                    PortraitPalettes[i] = s.SerializeObject<Palette>(PortraitPalettes[i], name: $"{nameof(PortraitPalettes)}[{i}]"));

            s.DoAt(pointerTable.TryGetItem(Spyro_DefinedPointer.Ice_PortraitTileMaps), () =>
                PortraitTileMapPointers = s.SerializePointerArray(PortraitTileMapPointers, count, name: nameof(PortraitTileMapPointers)));

            PortraitTileMaps ??= new BinarySerializer.GBA.MapTile[count][];

            for (int i = 0; i < PortraitTileMaps.Length; i++)
                s.DoAt(PortraitTileMapPointers[i], () =>
                    PortraitTileMaps[i] = s.SerializeObjectArray<BinarySerializer.GBA.MapTile>(PortraitTileMaps[i], 4 * 4, name: $"{nameof(PortraitTileMaps)}[{i}]"));

            s.DoAt(pointerTable.TryGetItem(Spyro_DefinedPointer.Ice_PortraitTileSets), () =>
                PortraitTileSetPointers = s.SerializePointerArray(PortraitTileSetPointers, count, name: nameof(PortraitTileSetPointers)));

            PortraitTileSets ??= new byte[count][];

            for (int i = 0; i < PortraitTileSets.Length; i++)
            {
                int length = (PortraitTileMaps[i].Max(x => x.TileIndex) + 1) * 0x20;
                s.DoAt(PortraitTileSetPointers[i], () =>
                    PortraitTileSets[i] = s.SerializeArray<byte>(PortraitTileSets[i], length, name: $"{nameof(PortraitTileSets)}[{i}]"));
            }
        }
    }
}