namespace R1Engine
{
    /// <summary>
    /// The type of Rayman Advance GBA ROM pointers
    /// </summary>
    public enum GBA_R1_ROMPointer
    {
        LevelMaps,
        BackgroundVignette,
        IntroVignette,
        SpritePalettes,
        
        EventGraphicsPointers,
        EventDataPointers,
        EventGraphicsGroupCountTablePointers,
        LevelEventGraphicsGroupCounts,
        
        WorldLevelOffsetTable,

        WorldMapVignetteImageData,
        WorldMapVignetteBlockIndices,
        WorldMapVignettePaletteIndices,
        WorldMapVignettePalettes,

        StringPointers,

        // Graphics not referenced from events
        DrumWalkerGraphics,
        ClockGraphics,
        InkGraphics,
        FontSmallGraphics,
        FontLargeGraphics,
        PinsGraphics
    }
}