namespace R1Engine
{
    /// <summary>
    /// ROM data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_ROM : GBA_ROM
    {
        /// <summary>
        /// The data for the levels
        /// </summary>
        public GBA_R1_LevelMapData LevelMapData { get; set; }

        /// <summary>
        /// The event data for the current level
        /// </summary>
        public GBA_R1_LevelEventData LevelEventData { get; set; }


        /// <summary>
        /// The background vignette data
        /// </summary>
        public GBA_R1_BackgroundVignette[] BackgroundVignettes { get; set; }

        public GBA_R1_IntroVignette[] IntroVignettes { get; set; }

        public GBA_R1_WorldMapVignette WorldMapVignette { get; set; }


        /// <summary>
        /// The sprite palettes. The game uses the same 16 palettes (with 16 colors) for every sprite in the game. During runtime this gets copied to 0x05000200.
        /// </summary>
        public ARGB1555Color[] SpritePalettes { get; set; }

        /// <summary>
        /// World level index offset table for global level array
        /// </summary>
        public byte[] WorldLevelOffsetTable { get; set; }


        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the global level index
            var levelIndex = new GBA_R1_Manager().GetGlobalLevelIndex(s.GameSettings.World, s.GameSettings.Level);

            // Get the pointer table
            var pointerTable = GBA_R1_PointerTable.GetPointerTable(s.GameSettings.GameModeSelection, this.Offset.file);

            // Serialize data from the ROM
            s.DoAt(pointerTable[GBA_R1_ROMPointer.LevelMaps] + (levelIndex * 28), 
                () => LevelMapData = s.SerializeObject<GBA_R1_LevelMapData>(LevelMapData, name: nameof(LevelMapData)));

            s.DoAt(pointerTable[GBA_R1_ROMPointer.BackgroundVignette], 
                () => BackgroundVignettes = s.SerializeObjectArray<GBA_R1_BackgroundVignette>(BackgroundVignettes, 48, name: nameof(BackgroundVignettes)));
            s.DoAt(pointerTable[GBA_R1_ROMPointer.IntroVignette], 
                () => IntroVignettes = s.SerializeObjectArray<GBA_R1_IntroVignette>(IntroVignettes, 14, name: nameof(IntroVignettes)));
            WorldMapVignette = s.SerializeObject<GBA_R1_WorldMapVignette>(WorldMapVignette, name: nameof(WorldMapVignette));

            s.DoAt(pointerTable[GBA_R1_ROMPointer.SpritePalettes], 
                () => SpritePalettes = s.SerializeObjectArray<ARGB1555Color>(SpritePalettes, 16 * 16 * 2, name: nameof(SpritePalettes)));

            s.DoAt(pointerTable[GBA_R1_ROMPointer.WorldLevelOffsetTable],
                () => WorldLevelOffsetTable = s.SerializeArray<byte>(WorldLevelOffsetTable, 12, name: nameof(WorldLevelOffsetTable)));

            // Serialize the level event data
            LevelEventData = new GBA_R1_LevelEventData();
            LevelEventData.SerializeData(s, pointerTable);
        }
    }

    /*

    ARRAYS:
     
    Pointer array at 0x086DCE14 - 60 items
    ushort array at 0x08549774
    
    uint array at 0x08549674
    uint array at 0x0854925E
    ushort array at 0x08549200
    
    Loc strings begin at 0x08F1D4C
    
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
    0x0202E5F0 - current level
    0x0202A2DA - some array where first two bytes are used to compare level and world somehow
    0x020226B0 - events
    0x0202D408 - link table
    0x020226A8 - multiplayer flag

     */
}