using System.Text;

namespace R1Engine
{
    /// <summary>
    /// ROM data for Rayman Advance (GBA)
    /// </summary>
    public class R1_GBA_ROM : GBA_ROMBase, IR1_GBAData
    {
        /// <summary>
        /// The map data for the current level
        /// </summary>
        public R1_GBA_LevelMapData LevelMapData { get; set; }

        /// <summary>
        /// The event data for the current level
        /// </summary>
        public R1_GBA_LevelEventData LevelEventData { get; set; }


        /// <summary>
        /// The background vignette data
        /// </summary>
        public R1_GBA_BackgroundVignette[] BackgroundVignettes { get; set; }

        /// <summary>
        /// The intro vignette data
        /// </summary>
        public R1_GBA_IntroVignette[] IntroVignettes { get; set; }

        /// <summary>
        /// The world map vignette
        /// </summary>
        public R1_GBA_WorldMapVignette WorldMapVignette { get; set; }


        /// <summary>
        /// The sprite palettes. The game uses the same 16 palettes (with 16 colors) for every sprite in the game. During runtime this gets copied to 0x05000200.
        /// </summary>
        public ARGB1555Color[] SpritePalettes { get; set; }

        /// <summary>
        /// The sprite palette for the current level
        /// </summary>
        /// <param name="settings">The game settings</param>
        public ARGB1555Color[] GetSpritePalettes(GameSettings settings) => SpritePalettes; 


        /// <summary>
        /// World level index offset table for global level array
        /// </summary>
        public byte[] WorldLevelOffsetTable { get; set; }


        public Pointer[] StringPointerTable { get; set; }

        /// <summary>
        /// The strings
        /// </summary>
        public string[][] Strings { get; set; }

        public R1_ZDCEntry[] TypeZDC { get; set; }
        public R1_ZDCData[] ZdcData { get; set; }
        public R1_EventFlags[] EventFlags { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the pointer table
            var pointerTable = PointerTables.R1_GBA_PointerTable(s.GameSettings.GameModeSelection, this.Offset.file);

            s.DoAt(pointerTable[R1_GBA_ROMPointer.WorldLevelOffsetTable],
                () => WorldLevelOffsetTable = s.SerializeArray<byte>(WorldLevelOffsetTable, 12, name: nameof(WorldLevelOffsetTable)));

            // Get the global level index
            var levelIndex = WorldLevelOffsetTable[s.GameSettings.World] + (s.GameSettings.Level - 1);

            // Hardcode files
            if (s is BinaryDeserializer && s.GameSettings.R1_World == R1_World.Image && s.GameSettings.Level == 4) {
                s.DoAt(Offset + 0x55cc, () => {
                    void CreateFakeFile(int index, int size) {
                        uint memAddress = s.Serialize<uint>(default, name: nameof(memAddress));
                        Pointer off_array = s.SerializePointer(default, name: nameof(off_array));
                        byte[] bytes = null;
                        s.DoAt(off_array, () => {
                            bytes = s.SerializeArray<byte>(default, size, name: nameof(bytes));
                        });
                        if (bytes != null) {
                            s.Context.AddMemoryMappedByteArrayFile("ETA_" + index, bytes, memAddress);
                        }
                    }
                    CreateFakeFile(0, 0x58);
                    CreateFakeFile(1, 0x8);
                    CreateFakeFile(2, 0x78);
                    CreateFakeFile(3, 0x210);
                    CreateFakeFile(4, 0x60);
                    CreateFakeFile(5, 0x110);
                    CreateFakeFile(6, 0x1a8);
                    CreateFakeFile(7, 0x70);
                    CreateFakeFile(8, 0x70);
                });

            }

            // Serialize data from the ROM
            s.DoAt(pointerTable[R1_GBA_ROMPointer.LevelMaps] + (levelIndex * 28), 
                () => LevelMapData = s.SerializeObject<R1_GBA_LevelMapData>(LevelMapData, name: nameof(LevelMapData)));

            s.DoAt(pointerTable[R1_GBA_ROMPointer.BackgroundVignette], 
                () => BackgroundVignettes = s.SerializeObjectArray<R1_GBA_BackgroundVignette>(BackgroundVignettes, 48, name: nameof(BackgroundVignettes)));
            s.DoAt(pointerTable[R1_GBA_ROMPointer.IntroVignette], 
                () => IntroVignettes = s.SerializeObjectArray<R1_GBA_IntroVignette>(IntroVignettes, 14, name: nameof(IntroVignettes)));
            WorldMapVignette = s.SerializeObject<R1_GBA_WorldMapVignette>(WorldMapVignette, name: nameof(WorldMapVignette));

            s.DoAt(pointerTable[R1_GBA_ROMPointer.SpritePalettes], 
                () => SpritePalettes = s.SerializeObjectArray<ARGB1555Color>(SpritePalettes, 16 * 16, name: nameof(SpritePalettes)));

            // Serialize the level event data
            LevelEventData = new R1_GBA_LevelEventData();
            LevelEventData.SerializeData(s, pointerTable[R1_GBA_ROMPointer.EventGraphicsPointers], pointerTable[R1_GBA_ROMPointer.EventDataPointers], pointerTable[R1_GBA_ROMPointer.EventGraphicsGroupCountTablePointers], pointerTable[R1_GBA_ROMPointer.LevelEventGraphicsGroupCounts]);

            // Serialize strings
            s.DoAt(pointerTable[R1_GBA_ROMPointer.StringPointers], () =>
            {
                StringPointerTable = s.SerializePointerArray(StringPointerTable, 5 * 259, name: nameof(StringPointerTable));

                // NOTE: There is data for two leftover languages from the PC version (Japanese and Chinese) but they have incorrect lengths, so we don't parse them
                if (Strings == null)
                    Strings = new string[5][];

                var enc = new Encoding[]
                {
                    // English
                    Encoding.GetEncoding(437),
                    // French
                    Encoding.GetEncoding(1252),
                    // German
                    Encoding.GetEncoding(437),
                    // Spanish
                    Encoding.GetEncoding(1252),
                    // Italian
                    Encoding.GetEncoding(1252),
                };

                for (int i = 0; i < Strings.Length; i++)
                {
                    if (Strings[i] == null)
                        Strings[i] = new string[259];

                    for (int j = 0; j < Strings[i].Length; j++)
                    {
                        s.DoAt(StringPointerTable[i * 259 + j], () => Strings[i][j] = s.SerializeString(Strings[i][j], encoding: enc[i], name: $"{nameof(Strings)}[{i}][{j}]"));
                    }
                }
            });

            // Serialize tables
            s.DoAt(pointerTable[R1_GBA_ROMPointer.TypeZDC], () => TypeZDC = s.SerializeObjectArray<R1_ZDCEntry>(TypeZDC, 262, name: nameof(TypeZDC)));
            s.DoAt(pointerTable[R1_GBA_ROMPointer.ZdcData], () => ZdcData = s.SerializeObjectArray<R1_ZDCData>(ZdcData, 200, name: nameof(ZdcData)));
            s.DoAt(pointerTable[R1_GBA_ROMPointer.EventFlags], () => EventFlags = s.SerializeArray<R1_EventFlags>(EventFlags, 262, name: nameof(EventFlags)));
        }
    }

    /*

    ARRAYS:
     
    Pointer array at 0x086DCE14 - 60 items
    ushort array at 0x08549774
    
    uint array at 0x08549674
    uint array at 0x0854925E
    ushort array at 0x08549200
        
    (these might begin 1 byte earlier)
    byte[4] array at 0x0854925C    


    SPLASH SCREENS:

    16 palettes for Ubi logo are     at 0x086EEDD8
    16 palettes for Eclipse logo are at 0x086EEFD8
    16 palettes for Rayman logo are  at 0x086EF188

    0x086DEC00 has 6 pointers. First 3 to image data and last 3 to the palettes for Ubi, Eclipse and RayLogo - where are the index tables?


    LOADING + CREDITS SCREENS:

    Palettes don't seem to exist in the rom - compressed?


    MEMORY LOCATIONS:

    0x02030394 - current world
    0x0202FB7C - current level
    0x0202A2DA - some array where first two bytes are used to compare level and world somehow
    0x020226B0 - events
    0x0202D408 - link table
    0x020226A8 - multiplayer flag

     */
}