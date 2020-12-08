namespace R1Engine
{
    public enum GBARRR_Pointer
    {
        VillageLevelInfo,
        LevelInfo,
        LevelProperties,
        OffsetTable,
        GraphicsTables,

        // Mode7
        Mode7_MapTiles, // 0x0x6000000
        Mode7_BG1Tiles, // 0x06008000
        Mode7_Bg1Map, // 0x0600B800
        Mode7_BG0Tiles, // 0x06004000
        Mode7_BG0Map, // 0x06007800
        Mode7_CollisionMapData, // 0x02001D64
        Mode7_MapData, // 0x02001D80
        Mode7_TilePalette, // 0x5000000
        Mode7_BG1Palette, // 0x050001C0
        Mode7_BG0Palette, // 0x05000180
        Mode7_Objects, // 0x020127B0 (32-byte structs)

        // Unknown non-compressed Mode7 pointer arrays
        Mode7_Waypoints, // 0x02000240 (length 0x7D0) - length seems to be 304 for first level and 220 for second? Object related data, two shorts.
        Mode7_WaypointsCount,
        Mode7_LevelSpritePalette, // 0x05000200 (length 0x100) - sprite palette?
        Mode7_Array3, // 0x05000200 (length 0x80)

        Mode7UnknownPal, // 05000200 (length 0x10) - this is not in an array and not compressed

        // Unknown compressed Mode7 pointer arrays
        Mode7_ComprArray1, // 0x06015000 world-specific sprites
        Mode7_CollisionTypesArray, // 0x020150D0 (1024 bytes). Graphics tile -> Collision tile map for each level
        Mode7_ComprArray4, // 0x06010A00 hud sprites

        // Unknown compressed data 
        RNC_0, // 0x06010000
        RNC_1, // 0x06010800
        RNC_2, // 0x06002000
        RNC_3, // 0x02015500
        RNC_4, // 0x06016900
        RNC_5, // 0x06010000

        MenuArray,

        MusicTable,
        MusicSampleTable,
        SoundEffectSampleTable,

        SpritesPauseMenu,
        SpritesGameOver,
        SpritesMode7Rayman,
        SpritesMode7UI,
        Mode7_SpritePalette,

        Mode7_AnimationFrameIndices,
        Mode7_ObjectTypes,
    }
}