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
        public GBA_R1_Level[] Levels { get; set; }

        /// <summary>
        /// The event data for the levels
        /// </summary>
        public GBA_R1_LevelEventData[] LevelEventData { get; set; }


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
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the pointer table
            var pointerTable = GBA_R1_PointerTable.GetPointerTable(s.GameSettings.GameModeSelection);

            // Serialize data from the ROM
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.Levels], this.Offset.file), 
                () => Levels = s.SerializeObjectArray<GBA_R1_Level>(Levels, GBA_R1_Manager.LevelCount, name: nameof(Levels)));

            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.BackgroundVignette], this.Offset.file), 
                () => BackgroundVignettes = s.SerializeObjectArray<GBA_R1_BackgroundVignette>(BackgroundVignettes, 48, name: nameof(BackgroundVignettes)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.IntroVignette], this.Offset.file), 
                () => IntroVignettes = s.SerializeObjectArray<GBA_R1_IntroVignette>(IntroVignettes, 14, name: nameof(IntroVignettes)));
            WorldMapVignette = s.SerializeObject<GBA_R1_WorldMapVignette>(WorldMapVignette, name: nameof(WorldMapVignette));

            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.SpritePalettes], this.Offset.file), 
                () => SpritePalettes = s.SerializeObjectArray<ARGB1555Color>(SpritePalettes, 16 * 16 * 2, name: nameof(SpritePalettes)));

            if (LevelEventData == null)
                LevelEventData = new GBA_R1_LevelEventData[GBA_R1_Manager.LevelCount];

            // TODO: Maybe only parse one level?
            for (int i = 0; i < LevelEventData.Length; i++)
            {
                LevelEventData[i] = s.SerializeObject<GBA_R1_LevelEventData>(LevelEventData[i], x => x.LevelIndex = i, name: $"{nameof(LevelEventData)}[{i}]");

                // TODO: Remove this. Temp fix so the object doesn't get cached.
                s.Serialize<byte>(default);
            }
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
    
    (these are aligned weirdly)
    uint array at 0x0854925C
    uin array at 0x0854925D
    uint array at 0x0854925F
    

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


    ROM LOCATIONS TO PARSE:

    0x081539A4 - index table for level offsets based on world in global level array

     */
}