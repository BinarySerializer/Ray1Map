namespace R1Engine
{
    /// <summary>
    /// ROM data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_ROM : R1Serializable
    {
        /// <summary>
        /// The data for the levels
        /// </summary>
        public GBA_R1_Level[] Levels { get; set; }

        public GBA_R1_UnkStruct[] UnkStructs { get; set; }

        /// <summary>
        /// The sprite palettes. The game uses the same 16 palettes (with 16 colors) for every sprite in the game. During runtime this gets copied to 0x05000200.
        /// </summary>
        public ARGB1555Color[] SpritePalettes { get; set; }

        // These pointers lead to even more pointers
        public Pointer[] UnkLevelPointerArray1 { get; set; }
        public Pointer[] UnkLevelPointerArray2 { get; set; }
        public Pointer[] UnkLevelPointerArray3 { get; set; }

        // Indexes to something. It almost matches the music tracks.
        public uint[] UnkLevelDwordArray { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Get the pointer table
            var pointerTable = GBA_R1_PointerTable.GetPointerTable(s.GameSettings.GameModeSelection);

            const int levelCount = 22 + 18 + 13 + 13 + 12 + 4 + 6;

            // Serialize data from the ROM
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.Levels], this.Offset.file), 
                () => Levels = s.SerializeObjectArray<GBA_R1_Level>(Levels, levelCount, name: nameof(Levels)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.UnkStructs], this.Offset.file), 
                () => UnkStructs = s.SerializeObjectArray<GBA_R1_UnkStruct>(UnkStructs, 48, name: nameof(UnkStructs)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.SpritePalettes], this.Offset.file), 
                () => SpritePalettes = s.SerializeObjectArray<ARGB1555Color>(SpritePalettes, 16 * 16 * 2, name: nameof(SpritePalettes)));

            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.UnkLevelPointerArray1], this.Offset.file), () => UnkLevelPointerArray1 = s.SerializePointerArray(UnkLevelPointerArray1, levelCount, name: nameof(UnkLevelPointerArray1)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.UnkLevelPointerArray2], this.Offset.file), () => UnkLevelPointerArray2 = s.SerializePointerArray(UnkLevelPointerArray2, levelCount, name: nameof(UnkLevelPointerArray2)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.UnkLevelPointerArray3], this.Offset.file), () => UnkLevelPointerArray3 = s.SerializePointerArray(UnkLevelPointerArray3, levelCount, name: nameof(UnkLevelPointerArray3)));
            s.DoAt(new Pointer(pointerTable[GBA_R1_ROMPointer.UnkLevelDwordArray], this.Offset.file), () => UnkLevelDwordArray = s.SerializeArray<uint>(UnkLevelDwordArray, levelCount, name: nameof(UnkLevelDwordArray)));
        }
    }
}