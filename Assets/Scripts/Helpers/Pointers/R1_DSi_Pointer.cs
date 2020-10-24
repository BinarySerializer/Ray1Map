namespace R1Engine
{
    /// <summary>
    /// The type of Rayman 1 DSi pointers
    /// </summary>
    public enum R1_DSi_Pointer
    {
        JungleMaps,
        LevelMaps,
        BackgroundVignette,
        WorldMapVignette,
        SpecialPalettes,

        StringPointers,

        TypeZDC,
        ZdcData,
        EventFlags,

        WorldInfo,
        WorldVignetteIndices,
        LevelMapsBGIndices,

        WorldLevelOffsetTable,

        EventGraphicsPointers,
        EventDataPointers,
        EventGraphicsGroupCountTablePointers,
        LevelEventGraphicsGroupCounts,

        DES_Ray,
        DES_RayLittle,
        DES_Clock,
        DES_Div,
        DES_Map,

        ETA_Ray,
        ETA_Clock,
        ETA_Div,
        ETA_Map
    }
}