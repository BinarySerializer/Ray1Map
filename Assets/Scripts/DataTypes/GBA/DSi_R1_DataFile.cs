using System;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Data for Rayman 1 (DSi)
    /// </summary>
    public class DSi_R1_DataFile : R1Serializable
    {
        /// <summary>
        /// The data for the level
        /// </summary>
        public GBA_R1_LevelMapData LevelMapData { get; set; }

        /// <summary>
        /// The event data for the current level
        /// </summary>
        public GBA_R1_LevelEventData LevelEventData { get; set; }

        public DSi_R1_PaletteReference[] Palettes { get; set; }

        public ARGB1555Color[] SpritePalette { get; set; }

        /// <summary>
        /// The background vignette data
        /// </summary>
        public GBA_R1_BackgroundVignette[] BackgroundVignettes { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Get the global level index
            var levelIndex = new DSi_R1_Manager().GetGlobalLevelIndex(s.GameSettings.World, s.GameSettings.Level);

            // Get the pointer table
            var pointerTable = PointerTables.GetDSiPointerTable(s.GameSettings.GameModeSelection, this.Offset.file);

            // Serialize data from the ROM
            s.DoAt((s.GameSettings.World == World.Jungle ? pointerTable[DSi_R1_Pointer.JungleMaps] : pointerTable[DSi_R1_Pointer.LevelMaps]) + (levelIndex * 32), 
                () => LevelMapData = s.SerializeObject<GBA_R1_LevelMapData>(LevelMapData, name: nameof(LevelMapData)));
            s.DoAt(pointerTable[DSi_R1_Pointer.BackgroundVignette],
                () => BackgroundVignettes = s.SerializeObjectArray<GBA_R1_BackgroundVignette>(BackgroundVignettes, 48, name: nameof(BackgroundVignettes)));

            // Serialize the level event data
            LevelEventData = new GBA_R1_LevelEventData();
            LevelEventData.SerializeData(s, pointerTable[DSi_R1_Pointer.EventGraphicsPointers], pointerTable[DSi_R1_Pointer.EventDataPointers], pointerTable[DSi_R1_Pointer.EventGraphicsGroupCountTablePointers], pointerTable[DSi_R1_Pointer.LevelEventGraphicsGroupCounts]);

            s.DoAt(pointerTable[DSi_R1_Pointer.SpecialPalettes], () => {
                Palettes = s.SerializeObjectArray<DSi_R1_PaletteReference>(Palettes, 10, name: nameof(Palettes));
            });
            // TODO: Check this & correct where necessary
            DSi_R1_PaletteReference palRef = null;
            switch (s.Context.Settings.World) {
                case World.Jungle:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_ray");
                    break;
                case World.Music:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_mus");
                    break;
                case World.Mountain:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_mnt");
                    // TODO: there's a mnt2!
                    break;
                case World.Image:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_img");
                    break;
                case World.Cave:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_cav");
                    break;
                case World.Cake:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_ray");
                    break;
            }
            SpritePalette = palRef?.Palette;
            /*s.DoAt(pointerTable[DSi_R1_Pointer.JungleMaps], () =>
            {
                var jun1 = s.SerializeObject<GBA_R1_LevelMapData>(LevelMapData, name: nameof(LevelMapData));
                jun1.SerializeLevelData(s);
                SpritePalettes = jun1.TilePalettes;
            });*/
        }

        /// <summary>
        /// Creates a relocated 0.bin file, that is searchable with file offsets in big endian, prefixed with "DD".
        /// e.g.: the bytes 010F1E02 (a pointer to 0x01) become DD000001.
        /// </summary>
        /// <param name="s"></param>
        public void CreateRelocatedFile(SerializerObject s) {
            byte[] data = s.SerializeArray<byte>(null, s.CurrentLength, name: "fullfile");
            uint addr = 0x021E0F00;
            for (int j = 0; j < data.Length; j++) {
                if (data[j] == 0x02) {
                    int off = j - 3;
                    uint ptr = BitConverter.ToUInt32(data, off);
                    if (ptr >= addr && ptr < addr + data.Length) {
                        ptr = (ptr - addr) + 0xDD000000;
                        byte[] newData = BitConverter.GetBytes(ptr);
                        for (int y = 0; y < 4; y++) {
                            data[off + 3 - y] = newData[y];
                        }
                        j += 3;
                    }
                }
            }
            Util.ByteArrayToFile(s.Context.BasePath + "relocated.bin", data);
        }
    }
}