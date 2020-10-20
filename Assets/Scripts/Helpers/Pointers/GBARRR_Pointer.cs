namespace R1Engine
{
    public enum GBARRR_Pointer
    {
        VillageLevelInfo,
        LevelInfo,
        OffsetTable,
        GraphicsTables,

        // Mode7
        Mode7_MapTiles,
        Mode7_BG1Tiles,
        Mode7_Bg1Map,
        Mode7_BG0Tiles,
        Mode7_BG0Map,
        Mode7_UnkMapData, // 0x02001D64
        Mode7_MapData,
        Mode7_TilePalette,
        Mode7_BG1Palette,
        Mode7_BG0Palette,

        // TODO: Add remaining non-compressed data:
        /*
         
        087218e4[3] > 040000D4 or 02000240 (length 0x7D0)
        087218c0[3] > 05000200 (length 0x100)
        087218d8[3] > 05000200 (length 0x80)
        086a6128 > 05000200 (length 0x10)
         
         */

        // Mode7Unk
        Mode7_ComprArray1, // 0x06015000 world-specific sprites
        Mode7_ComprArray2, // 0x020150D0 (1024 bytes)
        Mode7_ComprArray3, // 0x020127B0 (32-byte structs)
        Mode7_ComprArray4, // 0x06010A00 hud sprites

        // Unknown compressed data 
        Mode7_Compr1, // 0x06010000
        Mode7_Compr2, // 0x06010800
        Mode7_Compr3, // 0x06002000
        Mode7_Compr4, // 0x02015500
        Mode7_Compr5, // 0x06016900
        Mode7_Compr6, // 0x06010000
        Mode7_MenuArray,

        MusicTable,
        MusicSampleTable,
        SoundEffectSampleTable,
    }
}