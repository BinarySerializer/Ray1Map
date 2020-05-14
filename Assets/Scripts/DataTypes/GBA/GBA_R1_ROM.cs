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
        /// The background vignette data
        /// </summary>
        public GBA_R1_BackgroundVignette[] BackgroundVignettes { get; set; }

        public GBA_R1_IntroVignette[] IntroVignettes { get; set; }

        public GBA_R1_WorldMapVignette WorldMapVignette { get; set; }

        /// <summary>
        /// The sprite palettes. The game uses the same 16 palettes (with 16 colors) for every sprite in the game. During runtime this gets copied to 0x05000200.
        /// </summary>
        public ARGB1555Color[] SpritePalettes { get; set; }

        // Pointers seem to lead to another pointer-array which in term leads to structs like this {ImageBufferPointer, (uint)Length} - maybe not? Only matches some.
        public Pointer[] UnkLevelPointerArray1 { get; set; }

        // Pointers seem to lead to another pointer-array which in term leads to 28-byte long structs which begin with 2 pointers
        public Pointer[] UnkLevelPointerArray2 { get; set; }

        // Pointers lead to 6 initial bytes followed by 28-byte long structs with two pointers in the middle
        public Pointer[] UnkLevelPointerArray3 { get; set; }

        // Indexes to something. It almost matches the music tracks.
        public uint[] UnkLevelDwordArray { get; set; }

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

            const int levelCount = 22 + 18 + 13 + 13 + 12 + 4 + 6;

            // Serialize data from the ROM
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.Levels], this.Offset.file), 
                () => Levels = s.SerializeObjectArray<GBA_R1_Level>(Levels, levelCount, name: nameof(Levels)));

            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.BackgroundVignette], this.Offset.file), 
                () => BackgroundVignettes = s.SerializeObjectArray<GBA_R1_BackgroundVignette>(BackgroundVignettes, 48, name: nameof(BackgroundVignettes)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.IntroVignette], this.Offset.file), 
                () => IntroVignettes = s.SerializeObjectArray<GBA_R1_IntroVignette>(IntroVignettes, 14, name: nameof(IntroVignettes)));
            WorldMapVignette = s.SerializeObject<GBA_R1_WorldMapVignette>(WorldMapVignette, name: nameof(WorldMapVignette));

            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.SpritePalettes], this.Offset.file), 
                () => SpritePalettes = s.SerializeObjectArray<ARGB1555Color>(SpritePalettes, 16 * 16 * 2, name: nameof(SpritePalettes)));

            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.UnkLevelPointerArray1], this.Offset.file), () => UnkLevelPointerArray1 = s.SerializePointerArray(UnkLevelPointerArray1, levelCount, name: nameof(UnkLevelPointerArray1)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.UnkLevelPointerArray2], this.Offset.file), () => UnkLevelPointerArray2 = s.SerializePointerArray(UnkLevelPointerArray2, levelCount, name: nameof(UnkLevelPointerArray2)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.UnkLevelPointerArray3], this.Offset.file), () => UnkLevelPointerArray3 = s.SerializePointerArray(UnkLevelPointerArray3, levelCount, name: nameof(UnkLevelPointerArray3)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.UnkLevelDwordArray], this.Offset.file), () => UnkLevelDwordArray = s.SerializeArray<uint>(UnkLevelDwordArray, levelCount, name: nameof(UnkLevelDwordArray)));
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

     */
}