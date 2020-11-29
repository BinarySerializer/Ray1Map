using System;
using System.Linq;
using System.Text;

namespace R1Engine
{
    /// <summary>
    /// Data for Rayman 1 (DSi)
    /// </summary>
    public class R1_DSi_DataFile : R1Serializable, IR1_GBAData
    {
        /// <summary>
        /// The map data for the current level
        /// </summary>
        public R1_GBA_LevelMapData LevelMapData { get; set; }

        /// <summary>
        /// The event data for the current level
        /// </summary>
        public R1_GBA_LevelEventData LevelEventData { get; set; }

        public R1_DSi_PaletteReference[] Palettes { get; set; }

        /// <summary>
        /// The sprite palette for the current level
        /// </summary>
        /// <param name="settings">The game settings</param>
        public RGBA5551Color[] GetSpritePalettes(GameSettings settings)
        {
            R1_DSi_PaletteReference palRef;
            switch (settings.R1_World)
            {
                default:
                case R1_World.Jungle:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_ray");
                    break;
                case R1_World.Music:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_mus");
                    break;
                case R1_World.Mountain:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_mnt");
                    // NOTE: There's a mnt2. It appears to be unused?
                    break;
                case R1_World.Image:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_img");
                    break;
                case R1_World.Cave:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_cav");
                    break;
                case R1_World.Cake:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_ray");
                    break;
            }
            return palRef?.Palette;
        }

        /// <summary>
        /// The background vignette data
        /// </summary>
        public R1_GBA_BackgroundVignette[] BackgroundVignettes { get; set; }

        // TODO: Parse these from data
        public R1_GBA_IntroVignette[] IntroVignettes => null;
        public R1_GBA_WorldMapVignette WorldMapVignette { get; set; }

        public byte[] WorldLevelOffsetTable { get; set; }

        public Pointer[] StringPointerTable { get; set; }
        public string[][] Strings { get; set; }

        public R1_ZDCEntry[] TypeZDC { get; set; }
        public R1_ZDCData[] ZdcData { get; set; }
        public R1_EventFlags[] EventFlags { get; set; }

        public Pointer[] WorldVignetteIndicesPointers { get; set; }
        public byte[] WorldVignetteIndices { get; set; }

        public R1_WorldMapInfo[] WorldInfos { get; set; }

        public R1_GBA_EventGraphicsData DES_Ray { get; set; }
        public R1_GBA_EventGraphicsData DES_RayLittle { get; set; }
        public R1_GBA_EventGraphicsData DES_Clock { get; set; }
        public R1_GBA_EventGraphicsData DES_Div { get; set; }
        public R1_GBA_EventGraphicsData DES_Map { get; set; }
        public R1_GBA_ETA ETA_Ray { get; set; }
        public R1_GBA_ETA ETA_Clock { get; set; }
        public R1_GBA_ETA ETA_Div { get; set; }
        public R1_GBA_ETA ETA_Map { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Get the pointer table
            var pointerTable = PointerTables.R1_DSi_PointerTable(s.GameSettings.GameModeSelection, this.Offset.file);

            s.DoAt(pointerTable[R1_DSi_Pointer.WorldLevelOffsetTable],
                () => WorldLevelOffsetTable = s.SerializeArray<byte>(WorldLevelOffsetTable, 8, name: nameof(WorldLevelOffsetTable)));

            // Get the global level index
            var levelIndex = WorldLevelOffsetTable[s.GameSettings.World] + (s.GameSettings.Level - 1);

            DES_Ray = s.DoAt(pointerTable[R1_DSi_Pointer.DES_Ray], () => s.SerializeObject<R1_GBA_EventGraphicsData>(DES_Ray, name: nameof(DES_Ray)));
            ETA_Ray = s.DoAt(pointerTable.TryGetItem(R1_DSi_Pointer.ETA_Ray), () => s.SerializeObject<R1_GBA_ETA>(ETA_Ray, x => x.Lengths = new byte[] { 66, 12, 34, 53, 14, 14, 1, 2 }, name: nameof(ETA_Ray)));

            DES_RayLittle = s.DoAt(pointerTable[R1_DSi_Pointer.DES_RayLittle], () => s.SerializeObject<R1_GBA_EventGraphicsData>(DES_RayLittle, name: nameof(DES_RayLittle)));

            DES_Clock = s.DoAt(pointerTable[R1_DSi_Pointer.DES_Clock], () => s.SerializeObject<R1_GBA_EventGraphicsData>(DES_Clock, name: nameof(DES_Clock)));
            ETA_Clock = s.DoAt(pointerTable.TryGetItem(R1_DSi_Pointer.ETA_Clock), () => s.SerializeObject<R1_GBA_ETA>(ETA_Clock, x => x.Lengths = new byte[] { 3 }, name: nameof(ETA_Clock)));

            DES_Div = s.DoAt(pointerTable[R1_DSi_Pointer.DES_Div], () => s.SerializeObject<R1_GBA_EventGraphicsData>(DES_Div, name: nameof(DES_Div)));
            ETA_Div = s.DoAt(pointerTable.TryGetItem(R1_DSi_Pointer.ETA_Div), () => s.SerializeObject<R1_GBA_ETA>(ETA_Div, x => x.Lengths = new byte[] { 1, 1, 1, 1, 1, 1, 2, 2, 12, 12, 4 }, name: nameof(ETA_Div)));

            DES_Map = s.DoAt(pointerTable[R1_DSi_Pointer.DES_Map], () => s.SerializeObject<R1_GBA_EventGraphicsData>(DES_Map, name: nameof(DES_Map)));
            ETA_Map = s.DoAt(pointerTable.TryGetItem(R1_DSi_Pointer.ETA_Map), () => s.SerializeObject<R1_GBA_ETA>(ETA_Map, x => x.Lengths = new byte[] { 64, 1, 19, 1, 1, 69, 3 }, name: nameof(ETA_Map)));

            // Serialize data from the ROM
            if (s.GameSettings.R1_World != R1_World.Menu)
                s.DoAt((s.GameSettings.R1_World == R1_World.Jungle ? pointerTable[R1_DSi_Pointer.JungleMaps] : pointerTable[R1_DSi_Pointer.LevelMaps]) + (levelIndex * 32), 
                    () => LevelMapData = s.SerializeObject<R1_GBA_LevelMapData>(LevelMapData, name: nameof(LevelMapData)));

            s.DoAt(pointerTable[R1_DSi_Pointer.BackgroundVignette],
                () => BackgroundVignettes = s.SerializeObjectArray<R1_GBA_BackgroundVignette>(BackgroundVignettes, 48, name: nameof(BackgroundVignettes)));

            WorldMapVignette = s.SerializeObject<R1_GBA_WorldMapVignette>(WorldMapVignette, name: nameof(WorldMapVignette));

            // Serialize the level event data
            if (s.GameSettings.R1_World != R1_World.Menu)
            {
                LevelEventData = new R1_GBA_LevelEventData();
                LevelEventData.SerializeData(s, pointerTable[R1_DSi_Pointer.EventGraphicsPointers], pointerTable[R1_DSi_Pointer.EventDataPointers], pointerTable[R1_DSi_Pointer.EventGraphicsGroupCountTablePointers], pointerTable[R1_DSi_Pointer.LevelEventGraphicsGroupCounts]);
            }

            s.DoAt(pointerTable[R1_DSi_Pointer.SpecialPalettes], () => Palettes = s.SerializeObjectArray<R1_DSi_PaletteReference>(Palettes, 10, name: nameof(Palettes)));

            // Serialize strings
            s.DoAt(pointerTable[R1_DSi_Pointer.StringPointers], () =>
            {
                StringPointerTable = s.SerializePointerArray(StringPointerTable, 5 * 394, name: nameof(StringPointerTable));
                
                if (Strings == null)
                    Strings = new string[5][];

                var enc = new Encoding[]
                {
                    // Spanish
                    Encoding.GetEncoding(1252),
                    // English
                    Encoding.GetEncoding(437),
                    // French
                    Encoding.GetEncoding(1252),
                    // Italian
                    Encoding.GetEncoding(1252),
                    // German
                    Encoding.GetEncoding(437),
                };

                for (int i = 0; i < Strings.Length; i++)
                {
                    if (Strings[i] == null)
                        Strings[i] = new string[394];

                    for (int j = 0; j < Strings[i].Length; j++)
                    {
                        s.DoAt(StringPointerTable[i * 394 + j], () => Strings[i][j] = s.SerializeString(Strings[i][j], encoding: enc[i], name: $"{nameof(Strings)}[{i}][{j}]"));
                    }
                }
            });

            // Serialize tables
            s.DoAt(pointerTable[R1_DSi_Pointer.TypeZDC], () => TypeZDC = s.SerializeObjectArray<R1_ZDCEntry>(TypeZDC, 262, name: nameof(TypeZDC)));
            s.DoAt(pointerTable[R1_DSi_Pointer.ZdcData], () => ZdcData = s.SerializeObjectArray<R1_ZDCData>(ZdcData, 200, name: nameof(ZdcData)));
            s.DoAt(pointerTable[R1_DSi_Pointer.EventFlags], () => EventFlags = s.SerializeArray<R1_EventFlags>(EventFlags, 262, name: nameof(EventFlags)));

            if (s.GameSettings.R1_World != R1_World.Menu)
            {
                WorldVignetteIndicesPointers = s.DoAt(pointerTable[R1_DSi_Pointer.WorldVignetteIndices], () => s.SerializePointerArray(WorldVignetteIndicesPointers, 7, name: nameof(WorldVignetteIndicesPointers)));
                WorldVignetteIndices = s.DoAt(WorldVignetteIndicesPointers[s.GameSettings.World], () => s.SerializeArray<byte>(WorldVignetteIndices, 8, name: nameof(WorldVignetteIndices))); // The max size is 8

                // Get the background indices
                s.DoAt(pointerTable[R1_DSi_Pointer.LevelMapsBGIndices] + (levelIndex * 32), () =>
                {
                    LevelMapData.Unk_10 = s.Serialize<byte>(LevelMapData.Unk_10, name: nameof(LevelMapData.Unk_10));
                    LevelMapData.Unk_11 = s.Serialize<byte>(LevelMapData.Unk_11, name: nameof(LevelMapData.Unk_11));
                    LevelMapData.BackgroundIndex = s.Serialize<byte>(LevelMapData.BackgroundIndex, name: nameof(LevelMapData.BackgroundIndex));
                    LevelMapData.ParallaxBackgroundIndex = s.Serialize<byte>(LevelMapData.ParallaxBackgroundIndex, name: nameof(LevelMapData.ParallaxBackgroundIndex));
                });
            }

            WorldInfos = s.DoAt(pointerTable[R1_DSi_Pointer.WorldInfo], () => s.SerializeObjectArray<R1_WorldMapInfo>(WorldInfos, 24, name: nameof(WorldInfos)));
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

    /*

        SPLASH SCREENS:

        ???


        LOADING + CREDITS SCREENS:

        ???


        INTRO SCREENS:

        ???
 */
}