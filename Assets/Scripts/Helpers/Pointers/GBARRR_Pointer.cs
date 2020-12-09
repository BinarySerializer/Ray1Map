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

        // Unknown compressed Mode7 pointer arrays
        Mode7_Sprites_World, // 0x06015000 world-specific sprites
        Mode7_CollisionTypesArray, // 0x020150D0 (1024 bytes). Graphics tile -> Collision tile map for each level
        Mode7_Sprites_HUD, // 0x06010A00 hud sprites

        // Unknown compressed data 
        Sprites_Compressed_Unk, // 0x06010000
        Sprites_Compressed_GameOver, // 0x06010800
        RNC_2, // 0x06002000
        RNC_3, // 0x02015500
        Sprites_PauseMenu_Carrot, // 0x06016900
        Sprites_Compressed_MainMenu, // 0x06010000

        MenuArray,

        MusicTable,
        MusicSampleTable,
        SoundEffectSampleTable,

        Sprites_PauseMenu,
        Sprites_GameOver,
        Sprites_Mode7Rayman, // 06010000 (0x400 * 2) (current frame only)
        Sprites_Mode7UI_LumCount, // 06010900 (0x80 * 2) (current frame only)
        Sprites_Mode7UI_TotalLumCount, // 06010800 (0x80 * 2) (current frame only)
        Sprites_MenuFont, // 0600c000 (0x710 * 2)

        Palette_Mode7Sprites_2, // 0x05000200 (length 0x100) - sprite palette?
        Palette_Mode7Sprites_1, // 0x05000200 (length 0x80)
        Palette_Mode7Sprites_0, // 05000200 (0x10 * 2)

        Palette_MenuFont, // 050001c0 (0x20 * 2)
        Palette_GameOver1, // 05000200 (0x10 * 2)
        Palette_GameOver2, // 05000220 ((0x10 * 2)
        Palette_PauseMenuSprites, // 05000200 (0x20 * 2)
        Palette_UnkSprites, // 0500200 (0x100 * 2)
        Palette_MainMenuSprites, // 0500200 (0x100 * 2)


        Mode7_AnimationFrameIndices,
        Mode7_Animations,

        Mode
    }
}