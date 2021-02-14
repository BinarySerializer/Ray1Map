
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Pointer tables for rom games
    /// </summary>
    public static class PointerTables
    {
        /// <summary>
        /// Gets the pointer table for the specified GBA version
        /// </summary>
        /// <param name="gameMode">The GBA game mode</param>
        /// <param name="romFile">The ROM file</param>
        /// <returns>The pointer table</returns>
        public static Dictionary<R1_GBA_ROMPointer, Pointer> R1_GBA_PointerTable(GameModeSelection gameMode, BinaryFile romFile)
        {
            if (gameMode == GameModeSelection.RaymanAdvanceGBAUS)
            {
                return new Dictionary<R1_GBA_ROMPointer, uint>()
                {
                    [R1_GBA_ROMPointer.LevelMaps] = 0x08548688,
                    [R1_GBA_ROMPointer.BackgroundVignette] = 0x086D4E34,
                    [R1_GBA_ROMPointer.IntroVignette] = 0x080F7A04,
                    [R1_GBA_ROMPointer.SpritePalettes] = 0x0854902A,

                    [R1_GBA_ROMPointer.EventGraphicsPointers] = 0x081A63B8,
                    [R1_GBA_ROMPointer.EventDataPointers] = 0x081A6518,
                    [R1_GBA_ROMPointer.EventGraphicsGroupCountTablePointers] = 0x081A6678,
                    [R1_GBA_ROMPointer.LevelEventGraphicsGroupCounts] = 0x081A67D8,
                    
                    [R1_GBA_ROMPointer.WorldLevelOffsetTable] = 0x08153A40,

                    [R1_GBA_ROMPointer.WorldInfo] = 0x0854A530,
                    [R1_GBA_ROMPointer.WorldMapVignetteImageData] = 0x081452A4,
                    [R1_GBA_ROMPointer.WorldMapVignetteBlockIndices] = 0x08151504,
                    [R1_GBA_ROMPointer.WorldMapVignettePaletteIndices] = 0x08152284,
                    [R1_GBA_ROMPointer.WorldMapVignettePalettes] = 0x08152944,

                    [R1_GBA_ROMPointer.StringPointers] = 0x0854ADB4,

                    [R1_GBA_ROMPointer.TypeZDC] = 0x0854A304,
                    [R1_GBA_ROMPointer.ZdcData] = 0x08549CC4,
                    [R1_GBA_ROMPointer.EventFlags] = 0x08549330,

                    [R1_GBA_ROMPointer.WorldVignetteIndices] = 0x08153A1C,

                    [R1_GBA_ROMPointer.DES_Ray] = 0x0835F9B4,
                    [R1_GBA_ROMPointer.DES_RayLittle] = 0x0835F9D8,
                    [R1_GBA_ROMPointer.DES_Clock] = 0x082C90C8,
                    [R1_GBA_ROMPointer.DES_Div] = 0x082D1D98,
                    [R1_GBA_ROMPointer.DES_Map] = 0x0832A2F0,

                    [R1_GBA_ROMPointer.ETA_Ray] = 0x0832D234,
                    //[R1_GBA_ROMPointer.ETA_Clock] = ,
                    [R1_GBA_ROMPointer.ETA_Div] = 0x082CC884,
                    [R1_GBA_ROMPointer.ETA_Map] = 0x082ED3B8,

                    [R1_GBA_ROMPointer.DES_Alpha] = 0x082E74F4,
                    [R1_GBA_ROMPointer.DES_Alpha2] = 0x082E7514,

                    [R1_GBA_ROMPointer.DES_DrumWalkerGraphics] = 0x082C6C5C,
                    [R1_GBA_ROMPointer.DES_InkGraphics] = 0x082D33D0,
                    [R1_GBA_ROMPointer.DES_PinsGraphics] = 0x0832CBF4,

                    [R1_GBA_ROMPointer.ExtFontImgBuffers] = 0x086DCEE8,
                    [R1_GBA_ROMPointer.MultiplayerImgBuffers] = 0x086DCF98,

                    [R1_GBA_ROMPointer.MusyxFile] = 0x086EFADC,
                }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));
            }
            else if (gameMode == GameModeSelection.RaymanAdvanceGBAEU || gameMode == GameModeSelection.RaymanAdvanceGBAEUBeta)
            {
                var offset = gameMode == GameModeSelection.RaymanAdvanceGBAEU ? 0 : 0xC;

                return new Dictionary<R1_GBA_ROMPointer, uint>()
                {
                    [R1_GBA_ROMPointer.LevelMaps] = 0x085485B4,
                    [R1_GBA_ROMPointer.BackgroundVignette] = 0x086D4D60,
                    [R1_GBA_ROMPointer.IntroVignette] = 0x080F7968,
                    [R1_GBA_ROMPointer.SpritePalettes] = 0x08548F56,
                    
                    [R1_GBA_ROMPointer.EventGraphicsPointers] = 0x081A62E4,
                    [R1_GBA_ROMPointer.EventDataPointers] = 0x081A6444,
                    [R1_GBA_ROMPointer.EventGraphicsGroupCountTablePointers] = 0x081A65A4,
                    [R1_GBA_ROMPointer.LevelEventGraphicsGroupCounts] = 0x081A6704,
                    
                    [R1_GBA_ROMPointer.WorldLevelOffsetTable] = 0x081539A4,

                    [R1_GBA_ROMPointer.WorldInfo] = 0x0854A45C,
                    [R1_GBA_ROMPointer.WorldMapVignetteImageData] = 0x08145208,
                    [R1_GBA_ROMPointer.WorldMapVignetteBlockIndices] = 0x08151468,
                    [R1_GBA_ROMPointer.WorldMapVignettePaletteIndices] = 0x081521E8,
                    [R1_GBA_ROMPointer.WorldMapVignettePalettes] = 0x081528A8,

                    [R1_GBA_ROMPointer.StringPointers] = 0x0854ACE0,

                    [R1_GBA_ROMPointer.TypeZDC] = 0x0854A230,
                    [R1_GBA_ROMPointer.ZdcData] = 0x08549BF0,
                    [R1_GBA_ROMPointer.EventFlags] = 0x0854925C,

                    [R1_GBA_ROMPointer.WorldVignetteIndices] = 0x08153980,

                    [R1_GBA_ROMPointer.DES_Ray] = 0x0835F8E0,
                    [R1_GBA_ROMPointer.DES_RayLittle] = 0x0835F904,
                    [R1_GBA_ROMPointer.DES_Clock] = 0x082C8FF4,
                    [R1_GBA_ROMPointer.DES_Div] = 0x082D1CC4,
                    [R1_GBA_ROMPointer.DES_Map] = 0x0832A21C,

                    [R1_GBA_ROMPointer.ETA_Ray] = 0x0832D160,
                    //[R1_GBA_ROMPointer.ETA_Clock] = ,
                    [R1_GBA_ROMPointer.ETA_Div] = 0x082CC7B0,
                    [R1_GBA_ROMPointer.ETA_Map] = 0x082ED2E4,

                    [R1_GBA_ROMPointer.DES_Alpha] = 0x082E7420,
                    [R1_GBA_ROMPointer.DES_Alpha2] = 0x082E7440,
                    
                    [R1_GBA_ROMPointer.DES_DrumWalkerGraphics] = 0x082C6B88,
                    [R1_GBA_ROMPointer.DES_InkGraphics] = 0x082D32FC,
                    [R1_GBA_ROMPointer.DES_PinsGraphics] = 0x0832CB20,

                    [R1_GBA_ROMPointer.ExtFontImgBuffers] = 0x086dce14,
                    [R1_GBA_ROMPointer.MultiplayerImgBuffers] = 0x086dcec4,

                    [R1_GBA_ROMPointer.MusyxFile] = 0x086EF6D4,
                }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile) - offset);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
            }
        }

        /// <summary>
        /// Gets the pointer table for the specified DSi version
        /// </summary>
        /// <param name="gameMode">The DSi game mode</param>
        /// <param name="dataFile">The data file</param>
        /// <returns>The pointer table</returns>
        public static Dictionary<R1_DSi_Pointer, Pointer> R1_DSi_PointerTable(GameModeSelection gameMode, BinaryFile dataFile)
        {
            return new Dictionary<R1_DSi_Pointer, uint>()
            {
                [R1_DSi_Pointer.JungleMaps] = 0x0226C6B4,
                [R1_DSi_Pointer.LevelMaps] = 0x02361968,
                [R1_DSi_Pointer.BackgroundVignette] = 0x025A1478,
                [R1_DSi_Pointer.WorldMapVignette] = 0x021E17FC,
                [R1_DSi_Pointer.SpecialPalettes] = 0x02268FEC,

                [R1_DSi_Pointer.StringPointers] = 0x022604D0,

                [R1_DSi_Pointer.TypeZDC] = 0x0225F73C,
                [R1_DSi_Pointer.ZdcData] = 0x02262398,
                [R1_DSi_Pointer.EventFlags] = 0x022600B8,

                [R1_DSi_Pointer.WorldInfo] = 0x0225F144,
                [R1_DSi_Pointer.WorldVignetteIndices] = 0x02236ED8,
                [R1_DSi_Pointer.LevelMapsBGIndices] = 0x02913F94,

                [R1_DSi_Pointer.WorldLevelOffsetTable] = 0x02236BF8,

                [R1_DSi_Pointer.EventGraphicsPointers] = 0x0284B5B0,
                [R1_DSi_Pointer.EventDataPointers] = 0x0284B6F8,
                [R1_DSi_Pointer.EventGraphicsGroupCountTablePointers] = 0x0284B988,
                [R1_DSi_Pointer.LevelEventGraphicsGroupCounts] = 0x0284B840,

                [R1_DSi_Pointer.DES_Ray] = 0x02815BF4,
                [R1_DSi_Pointer.DES_RayLittle] = 0x02815DA4,
                [R1_DSi_Pointer.DES_Clock] = 0x0281BA8C,
                [R1_DSi_Pointer.DES_Div] = 0x02816224,
                [R1_DSi_Pointer.DES_Map] = 0x02815A20,

                [R1_DSi_Pointer.ETA_Ray] = 0x02814264,
                //[R1_DSi_Pointer.ETA_Clock] = ,
                [R1_DSi_Pointer.ETA_Div] = 0x02816C74,
                [R1_DSi_Pointer.ETA_Map] = 0x02813048,
            }.ToDictionary(x => x.Key, x => new Pointer(x.Value, dataFile));
        }

        /// <summary>
        /// Gets the pointer table for the Jaguar version
        /// </summary>
        /// <param name="engine">The Jaguar engine version</param>
        /// <param name="romFile">The rom file</param>
        /// <returns>The pointer table</returns>
        public static Dictionary<JaguarR1_Pointer, Pointer> JaguarR1_PointerTable(EngineVersion engine, BinaryFile romFile)
        {
            if (engine == EngineVersion.R1Jaguar)
            {
                return new Dictionary<JaguarR1_Pointer, uint>()
                {
                    [JaguarR1_Pointer.EventDefinitions] = 0x00906130,
                    [JaguarR1_Pointer.FixSprites] = 0x009496C8,
                    [JaguarR1_Pointer.WorldSprites] = 0x00949034,
                    [JaguarR1_Pointer.MapData] = 0x00949054,
                    [JaguarR1_Pointer.Music] = 0x009210F0,
                }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));
            }
            else if (engine == EngineVersion.R1Jaguar_Demo)
            {
                return new Dictionary<JaguarR1_Pointer, uint>()
                {
                    [JaguarR1_Pointer.EventDefinitions] = 0x00918B40,
                    [JaguarR1_Pointer.FixSprites] = 0x008028BA,
                    [JaguarR1_Pointer.WorldSprites] = 0x00874F14,
                    [JaguarR1_Pointer.MapData] = 0x00874F34,
                    [JaguarR1_Pointer.Music] = 0x00846C80,
                }.ToDictionary(x => x.Key, x => x.Value == 0 ? null : new Pointer(x.Value, romFile));
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(engine), engine, null);
            }
        }

        /// <summary>
        /// Gets the pointer table for the specified GBA version
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="romFile">The rom file</param>
        /// <returns>The pointer table</returns>
        public static Dictionary<GBA_Pointer, Pointer> GBA_PointerTable(Context context, BinaryFile romFile)
        {
            switch (context.Settings.GameModeSelection)
            {
                case GameModeSelection.Rayman3GBAEU:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x0829BEEC,
                        [GBA_Pointer.LevelInfo] = 0x080D4080,
                        [GBA_Pointer.Localization] = 0x080d4058,
                        [GBA_Pointer.Vignette] = 0x0820ED94,
                        [GBA_Pointer.VignettePalettes] = 0x080B37C0,

                        [GBA_Pointer.R3SinglePak_OffsetTable] = 0x087fbea0,
                        [GBA_Pointer.R3SinglePak_Palette] = 0x080d95b4,
                        [GBA_Pointer.R3SinglePak_TileSet] = 0x080d97b4,
                        [GBA_Pointer.R3SinglePak_TileMap] = 0x080dd5b4,

                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.Rayman3GBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x0829BE54
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.Rayman3GBAEUBeta:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x0829BC1C,
                        [GBA_Pointer.LevelInfo] = 0x080D3DB0,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.Rayman3GBAUSPrototype:
                    return new Dictionary<GBA_Pointer, uint>()
                    {
                        [GBA_Pointer.UiOffsetTable] = 0x084C1478,
                        [GBA_Pointer.Localization] = 0x080F20C0,
                        [GBA_Pointer.Vignette] = 0x0845FE3C,
                        [GBA_Pointer.VignettePalettes] = 0x080DC46A,

                        [GBA_Pointer.R3SinglePak_OffsetTable] = 0x089f7e90,
                        [GBA_Pointer.R3SinglePak_Palette] = 0x082b2d20,
                        [GBA_Pointer.R3SinglePak_TileSet] = 0x082b2f20,
                        [GBA_Pointer.R3SinglePak_TileMap] = 0x082b7520,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.Rayman3GBAMadTraxEU:
                case GameModeSelection.Rayman3GBAMadTraxUS:
                    switch ((GBA_R3MadTrax_Manager.Files)context.Settings.World)
                    {
                        case GBA_R3MadTrax_Manager.Files.client_pad_english:
                        case GBA_R3MadTrax_Manager.Files.client_pad_french:
                        case GBA_R3MadTrax_Manager.Files.client_pad_german:
                        case GBA_R3MadTrax_Manager.Files.client_pad_italian:
                        case GBA_R3MadTrax_Manager.Files.client_pad_spanish:
                            return new Dictionary<GBA_Pointer, uint>()
                            {
                                [GBA_Pointer.UiOffsetTable] = 0x0802F5EC,
                                [GBA_Pointer.MadTrax_Sprites] = 0x0800E000,
                            }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                        case GBA_R3MadTrax_Manager.Files.client_pad145:
                            return new Dictionary<GBA_Pointer, uint>()
                            {
                                [GBA_Pointer.UiOffsetTable] = 0x0802e34c,
                                [GBA_Pointer.MadTrax_Sprites] = 0x0800DD80,
                            }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                        case GBA_R3MadTrax_Manager.Files.client_pad2:
                        case GBA_R3MadTrax_Manager.Files.client_pad3:
                            return new Dictionary<GBA_Pointer, uint>()
                            {
                                [GBA_Pointer.UiOffsetTable] = 0x08025970,
                                [GBA_Pointer.MadTrax_Sprites] = 0x0800CBC0,
                            }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                        default:
                            throw new ArgumentOutOfRangeException(nameof(context.Settings.World), context.Settings.World, null);
                    }

                case GameModeSelection.Rayman3NGage:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x0
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.Rayman3Digiblast:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x1F9928,
                        [GBA_Pointer.Vignette] = 0x1666A0,
                        [GBA_Pointer.VignettePalettes] = 0x1451E8,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.PrinceOfPersiaGBAEU:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x08165920,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.PrinceOfPersiaGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x08165890,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SabrinaTheTeenageWitchPotionCommotionGBAEU:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x081A0D54,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SabrinaTheTeenageWitchPotionCommotionGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x081A0468,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SplinterCellGBAEU:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x082260FC,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SplinterCellGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x082260A4,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SplinterCellGBAEUBeta:
                    return new Dictionary<GBA_Pointer, uint>()
                    {
                        [GBA_Pointer.UiOffsetTable] = 0x083EB458,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SplinterCellNGage:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x0
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SplinterCellPandoraTomorrowGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x081D7E98,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SplinterCellPandoraTomorrowGBAEU:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x081D7EF0,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.StarWarsTrilogyApprenticeOfTheForceGBAEU:
                    return new Dictionary<GBA_Pointer, uint>()
                    {
                        [GBA_Pointer.UiOffsetTable] = 0x08286208,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.StarWarsTrilogyApprenticeOfTheForceGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x08286274,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.StarWarsEpisodeIIIGBAEU:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x082236DC,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.StarWarsEpisodeIIIGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x082236C0,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.KingKongGBAEU:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x081EA228,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.KingKongGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x081EA1B0,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.BatmanVengeanceGBAEU:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x084FCB9C,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.BatmanVengeanceGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x084FD44C,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.DonaldDuckAdvanceGBAEU:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x081DEDC8,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.DonaldDuckAdvanceGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x081DEFD4,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.CrouchingTigerHiddenDragonGBAEU:
                    return new Dictionary<GBA_Pointer, uint>()
                    {
                        [GBA_Pointer.UiOffsetTable] = 0x082A3DA0,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.CrouchingTigerHiddenDragonGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x0829e810,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.CrouchingTigerHiddenDragonGBAUSBeta:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x08262518,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.BatmanRiseOfSinTzuGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x08207930,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.OpenSeasonGBAEU1:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x082410FC,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.OpenSeasonGBAEU2:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x0824B4B0,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.OpenSeasonGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x0823F064,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.TMNTGBAEU:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x0821A1B0,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.TMNTGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x08213F28,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SurfsUpEU1:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x08182504,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SurfsUpEU2:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x0818AD6C,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SurfsUpUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x081824D8,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.TheMummyEU:
                case GameModeSelection.TheMummyUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x080601b8,
                        [GBA_Pointer.ActorTypeTable] = 0x0805f220,
                        [GBA_Pointer.Localization] = 0x0805fadc,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.TombRaiderTheProphecyEU:
                case GameModeSelection.TombRaiderTheProphecyUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x08061d48,
                        [GBA_Pointer.Localization] = 0x08061250,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.TomClancysRainbowSixRogueSpearEU:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x08045E20,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.TomClancysRainbowSixRogueSpearUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x08045e54,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the pointer table for the specified GBA RRR version
        /// </summary>
        /// <param name="gameMode">The GBA game mode</param>
        /// <param name="romFile">The rom file</param>
        /// <returns>The pointer table</returns>
        public static Dictionary<GBARRR_Pointer, Pointer> GBARRR_PointerTable(GameModeSelection gameMode, BinaryFile romFile)
        {
            switch (gameMode)
            {
                case GameModeSelection.RaymanRavingRabbidsGBAUS:
                    return new Dictionary<GBARRR_Pointer, uint>() {
                        [GBARRR_Pointer.VillageLevelInfo] = 0x08055C60,
                        [GBARRR_Pointer.LevelInfo] = 0x08055CE4,
                        [GBARRR_Pointer.LevelProperties] = 0x0866EA20,
                        [GBARRR_Pointer.OffsetTable] = 0x08708C04,
                        [GBARRR_Pointer.GraphicsTables] = 0x08056264,

                        [GBARRR_Pointer.Mode7_MapTiles] = 0x08708188,
                        [GBARRR_Pointer.Mode7_BG1Tiles] = 0x087081f4,
                        [GBARRR_Pointer.Mode7_Bg1Map] = 0x08708200,
                        [GBARRR_Pointer.Mode7_BG0Tiles] = 0x087081d0,
                        [GBARRR_Pointer.Mode7_BG0Map] = 0x087081dc,
                        [GBARRR_Pointer.Mode7_MapData] = 0x08708194,
                        [GBARRR_Pointer.Mode7_CollisionMapData] = 0x087081a0,
                        [GBARRR_Pointer.Mode7_TilePalette] = 0x087081b8,
                        [GBARRR_Pointer.Mode7_BG1Palette] = 0x0870820c,
                        [GBARRR_Pointer.Mode7_BG0Palette] = 0x087081e8,

                        [GBARRR_Pointer.Mode7_Waypoints] = 0x08708174, // Appears to be very different than on EU
                        [GBARRR_Pointer.Mode7_WaypointsCount] = 0x08708180,
                        [GBARRR_Pointer.Palette_Mode7Sprites_2] = 0x08708150,
                        [GBARRR_Pointer.Palette_Mode7Sprites_1] = 0x08708168,
                        [GBARRR_Pointer.Palette_Mode7Sprites_0] = 0x0868C9B8,

                        [GBARRR_Pointer.Sprites_Compressed_Unk] = 0x086C20EC,
                        [GBARRR_Pointer.Sprites_Compressed_GameOver] = 0x08701318,
                        [GBARRR_Pointer.RNC_2] = 0x086E1164,
                        [GBARRR_Pointer.RNC_3] = 0x086E2044,
                        [GBARRR_Pointer.Sprites_PauseMenu_Carrot] = 0x086c2378,
                        [GBARRR_Pointer.Sprites_Compressed_MainMenu] = 0x086bd1d1,

                        [GBARRR_Pointer.Mode7_Sprites_World] = 0x08708144,
                        [GBARRR_Pointer.Mode7_CollisionTypesArray] = 0x087081C4,
                        [GBARRR_Pointer.Mode7_Objects] = 0x087081ac,
                        [GBARRR_Pointer.Mode7_Sprites_HUD] = 0x0870815c,

                        [GBARRR_Pointer.MenuArray] = 0x087087D4,

                        [GBARRR_Pointer.MusicTable] = 0x0866E710,
                        [GBARRR_Pointer.MusicSampleTable] = 0x083C37BC,
                        [GBARRR_Pointer.SoundEffectSampleTable] = 0x0866D2E8,

                        [GBARRR_Pointer.Sprites_PauseMenu] = 0x086afb00,
                        [GBARRR_Pointer.Sprites_GameOver] = 0x086bc9ec,
                        [GBARRR_Pointer.Sprites_Mode7Rayman] = 0x0869d2f8,
                        [GBARRR_Pointer.Sprites_Mode7UI_LumCount] = 0x086d1c58,
                        [GBARRR_Pointer.Sprites_Mode7UI_TotalLumCount] = 0x086c977c,

                        [GBARRR_Pointer.Palette_MenuFont] = 0x087012d8,
                        [GBARRR_Pointer.Palette_GameOver1] = 0x086b0ef4,
                        [GBARRR_Pointer.Palette_GameOver2] = 0x087018fe,
                        [GBARRR_Pointer.Palette_PauseMenuSprites] = 0x0869e0a0,
                        [GBARRR_Pointer.Palette_UnkSprites] = 0x086c210c,
                        [GBARRR_Pointer.Palette_MainMenuSprites] = 0x086c0224,

                        [GBARRR_Pointer.Mode7_AnimationFrameIndices] = 0x087082c8,
                        [GBARRR_Pointer.Mode7_Animations] = 0x08708080,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.RaymanRavingRabbidsGBAEU:
                    return new Dictionary<GBARRR_Pointer, uint>() {
                        [GBARRR_Pointer.VillageLevelInfo] = 0x08055F40,
                        [GBARRR_Pointer.LevelInfo] = 0x08055FC4,
                        [GBARRR_Pointer.LevelProperties] = 0x08608220,
                        [GBARRR_Pointer.OffsetTable] = 0x08722374,
                        [GBARRR_Pointer.GraphicsTables] = 0x08056544,

                        [GBARRR_Pointer.Mode7_MapTiles] = 0x087218f8,
                        [GBARRR_Pointer.Mode7_BG1Tiles] = 0x08721964,
                        [GBARRR_Pointer.Mode7_Bg1Map] = 0x08721970,
                        [GBARRR_Pointer.Mode7_BG0Tiles] = 0x08721940,
                        [GBARRR_Pointer.Mode7_BG0Map] = 0x0872194c,
                        [GBARRR_Pointer.Mode7_MapData] = 0x08721904,
                        [GBARRR_Pointer.Mode7_CollisionMapData] = 0x08721910,
                        [GBARRR_Pointer.Mode7_TilePalette] = 0x08721928,
                        [GBARRR_Pointer.Mode7_BG1Palette] = 0x0872197c,
                        [GBARRR_Pointer.Mode7_BG0Palette] = 0x08721958,

                        [GBARRR_Pointer.Mode7_Waypoints] = 0x087218e4,
                        [GBARRR_Pointer.Mode7_WaypointsCount] = 0x087218f0,
                        [GBARRR_Pointer.Palette_Mode7Sprites_2] = 0x087218c0,
                        [GBARRR_Pointer.Palette_Mode7Sprites_1] = 0x087218d8,
                        [GBARRR_Pointer.Palette_Mode7Sprites_0] = 0x086a6128,

                        [GBARRR_Pointer.Sprites_Compressed_Unk] = 0x086db85c,
                        [GBARRR_Pointer.Sprites_Compressed_GameOver] = 0x0871aa88,
                        [GBARRR_Pointer.RNC_2] = 0x086fa8d4,
                        [GBARRR_Pointer.RNC_3] = 0x086fb7b4,
                        [GBARRR_Pointer.Sprites_PauseMenu_Carrot] = 0x086dbae8,
                        [GBARRR_Pointer.Sprites_Compressed_MainMenu] = 0x086d6941,

                        [GBARRR_Pointer.Mode7_Sprites_World] = 0x087218b4,
                        [GBARRR_Pointer.Mode7_CollisionTypesArray] = 0x08721934,
                        [GBARRR_Pointer.Mode7_Objects] = 0x0872191c,
                        [GBARRR_Pointer.Mode7_Sprites_HUD] = 0x087218cc,

                        [GBARRR_Pointer.MenuArray] = 0x08721f44,

                        [GBARRR_Pointer.MusicTable] = 0x08607f10,
                        [GBARRR_Pointer.MusicSampleTable] = 0x083c3a9c,
                        [GBARRR_Pointer.SoundEffectSampleTable] = 0x08606ae8,

                        [GBARRR_Pointer.Sprites_PauseMenu] = 0x086c9270,
                        [GBARRR_Pointer.Sprites_GameOver] = 0x086d615c,
                        [GBARRR_Pointer.Sprites_Mode7Rayman] = 0x086b6a68,
                        [GBARRR_Pointer.Sprites_Mode7UI_LumCount] = 0x086eb3c8,
                        [GBARRR_Pointer.Sprites_Mode7UI_TotalLumCount] = 0x086e2eec,

                        [GBARRR_Pointer.Palette_MenuFont] = 0x0871aa48,
                        [GBARRR_Pointer.Palette_GameOver1] = 0x086ca664,
                        [GBARRR_Pointer.Palette_GameOver2] = 0x0871b06e,
                        [GBARRR_Pointer.Palette_PauseMenuSprites] = 0x086b7810,
                        [GBARRR_Pointer.Palette_UnkSprites] = 0x086db87c,
                        [GBARRR_Pointer.Palette_MainMenuSprites] = 0x086d9994,

                        [GBARRR_Pointer.Mode7_AnimationFrameIndices] = 0x08721a38,
                        [GBARRR_Pointer.Mode7_Animations] = 0x087217f0,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the pointer table for the specified GBA Isometric RHR version
        /// </summary>
        /// <param name="gameMode">The GBA game mode</param>
        /// <param name="romFile">The rom file</param>
        /// <returns>The pointer table</returns>
        public static Dictionary<GBAIsometric_RHR_Pointer, Pointer> GBAIsometric_RHR_PointerTable(GameModeSelection gameMode, BinaryFile romFile) {
            switch (gameMode) 
            {
                case GameModeSelection.RaymanHoodlumsRevengeEU:
                    return new Dictionary<GBAIsometric_RHR_Pointer, uint>() {
                        [GBAIsometric_RHR_Pointer.MusyxFile] = 0x080447AC,
                        [GBAIsometric_RHR_Pointer.Levels] = 0x080E938C,
                        [GBAIsometric_RHR_Pointer.Localization] = 0x087F5AFC,
                        [GBAIsometric_RHR_Pointer.ObjTypes] = 0x080F9814,
                        [GBAIsometric_RHR_Pointer.CrabObjType] = 0x087f57c4,
                        [GBAIsometric_RHR_Pointer.Portraits] = 0x087f5948,
                        [GBAIsometric_RHR_Pointer.SpriteIcons] = 0x080e9728,
                        [GBAIsometric_RHR_Pointer.SpriteFlagsUS] = 0x080e8974,
                        [GBAIsometric_RHR_Pointer.SpriteFlagsEU] = 0x080e89d4,

                        [GBAIsometric_RHR_Pointer.Font0] = 0x080ea200,
                        [GBAIsometric_RHR_Pointer.Font1] = 0x080eaab4,
                        [GBAIsometric_RHR_Pointer.Font2] = 0x080ea39c,

                        [GBAIsometric_RHR_Pointer.PaletteAnimations0] = 0x080e99dc,
                        [GBAIsometric_RHR_Pointer.PaletteAnimations1] = 0x087f5a48,
                        [GBAIsometric_RHR_Pointer.PaletteAnimations2] = 0x087f5a78,

                        [GBAIsometric_RHR_Pointer.Map_PauseFrame1] = 0x084819d4,
                        [GBAIsometric_RHR_Pointer.Map_Menu0] = 0x08481898,
                        [GBAIsometric_RHR_Pointer.Map_Menu1] = 0x08481994,
                        [GBAIsometric_RHR_Pointer.Map_Menu2] = 0x084818dc,
                        [GBAIsometric_RHR_Pointer.Map_Menu3] = 0x0848192c,
                        [GBAIsometric_RHR_Pointer.Map_WorldMap] = 0x08481c94,
                        [GBAIsometric_RHR_Pointer.Map_ScoreScreen] = 0x08481a14,
                        [GBAIsometric_RHR_Pointer.Map_Blank] = 0x08481864,
                        [GBAIsometric_RHR_Pointer.Map_LicenseScreen1] = 0x084811a8,
                        [GBAIsometric_RHR_Pointer.Map_UbisoftScreen] = 0x08480ae8,
                        [GBAIsometric_RHR_Pointer.Map_DigitalEclipseLogo1] = 0x08480f68,
                        [GBAIsometric_RHR_Pointer.Map_DigitalEclipseLogo2] = 0x08480d28,
                        [GBAIsometric_RHR_Pointer.Map_LicenseScreen2] = 0x0848162c,
                        [GBAIsometric_RHR_Pointer.Map_GameLogo] = 0x084e4354,
                        [GBAIsometric_RHR_Pointer.Map_PauseFrame2] = 0x08481cd8,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.RaymanHoodlumsRevengeUS:
                    return new Dictionary<GBAIsometric_RHR_Pointer, uint>() {
                        [GBAIsometric_RHR_Pointer.MusyxFile] = 0x08044708,
                        [GBAIsometric_RHR_Pointer.Levels] = 0x080E92E8,
                        [GBAIsometric_RHR_Pointer.Localization] = 0x087F45C4,
                        [GBAIsometric_RHR_Pointer.ObjTypes] = 0x080f9770,
                        [GBAIsometric_RHR_Pointer.CrabObjType] = 0x087f428c,
                        [GBAIsometric_RHR_Pointer.Portraits] = 0x087F4410,
                        [GBAIsometric_RHR_Pointer.SpriteIcons] = 0x080e9684,
                        [GBAIsometric_RHR_Pointer.SpriteFlagsUS] = 0x080e88d0,
                        [GBAIsometric_RHR_Pointer.SpriteFlagsEU] = 0x080e8930,

                        [GBAIsometric_RHR_Pointer.Font0] = 0x080ea15c,
                        [GBAIsometric_RHR_Pointer.Font1] = 0x080eaa10,
                        [GBAIsometric_RHR_Pointer.Font2] = 0x080ea2f8,

                        [GBAIsometric_RHR_Pointer.PaletteAnimations0] = 0x080E9938,
                        [GBAIsometric_RHR_Pointer.PaletteAnimations1] = 0x087F4510,
                        [GBAIsometric_RHR_Pointer.PaletteAnimations2] = 0x087F4540,

                        [GBAIsometric_RHR_Pointer.Map_PauseFrame1] = 0x08481930,
                        [GBAIsometric_RHR_Pointer.Map_Menu0] = 0x084817f4,
                        [GBAIsometric_RHR_Pointer.Map_Menu1] = 0x084818f0,
                        [GBAIsometric_RHR_Pointer.Map_Menu2] = 0x08481838,
                        [GBAIsometric_RHR_Pointer.Map_Menu3] = 0x08481888,
                        [GBAIsometric_RHR_Pointer.Map_WorldMap] = 0x08481bf0,
                        [GBAIsometric_RHR_Pointer.Map_ScoreScreen] = 0x08481970,
                        [GBAIsometric_RHR_Pointer.Map_Blank] = 0x084817c0,
                        [GBAIsometric_RHR_Pointer.Map_LicenseScreen1] = 0x08481348,
                        [GBAIsometric_RHR_Pointer.Map_UbisoftScreen] = 0x08480a44,
                        [GBAIsometric_RHR_Pointer.Map_DigitalEclipseLogo1] = 0x08480ec4,
                        [GBAIsometric_RHR_Pointer.Map_DigitalEclipseLogo2] = 0x08480c84,
                        [GBAIsometric_RHR_Pointer.Map_LicenseScreen2] = 0x08481588,
                        [GBAIsometric_RHR_Pointer.Map_GameLogo] = 0x084e42b0,
                        [GBAIsometric_RHR_Pointer.Map_PauseFrame2] = 0x08481c34,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the pointer table for the specified GBA Isometric Spyro version
        /// </summary>
        /// <param name="gameMode">The GBA game mode</param>
        /// <param name="romFile">The rom file</param>
        /// <returns>The pointer table</returns>
        public static Dictionary<GBAIsometric_Spyro_Pointer, Pointer> GBAIsometric_Spyro_PointerTable(GameModeSelection gameMode, BinaryFile romFile) {
            switch (gameMode) 
            {
                case GameModeSelection.SpyroSeasonIceUS:
                    return new Dictionary<GBAIsometric_Spyro_Pointer, uint>() {
                        [GBAIsometric_Spyro_Pointer.DataTable] = 0x081ADCE0,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SpyroSeasonFlameEU:
                    return new Dictionary<GBAIsometric_Spyro_Pointer, uint>() {
                        [GBAIsometric_Spyro_Pointer.DataTable] = 0x0817ba38,

                        [GBAIsometric_Spyro_Pointer.LocalizationPointers] = 0x08002470,

                        [GBAIsometric_Spyro_Pointer.ObjectTypes] = 0x08175a18,
                        [GBAIsometric_Spyro_Pointer.AnimSets] = 0x081733e4,

                        [GBAIsometric_Spyro_Pointer.PortraitSprites] = 0x08175744,
                        [GBAIsometric_Spyro_Pointer.DialogEntries] = 0x08174c4c,
                        [GBAIsometric_Spyro_Pointer.CutsceneMaps] = 0x0817b0e4,

                        [GBAIsometric_Spyro_Pointer.LevelNames] = 0x08178d48,

                        [GBAIsometric_Spyro_Pointer.LevelMaps] = 0x0817af68,
                        [GBAIsometric_Spyro_Pointer.LevelObjects] = 0x081795bc,

                        [GBAIsometric_Spyro_Pointer.LevelData] = 0x0817ab88,
                        [GBAIsometric_Spyro_Pointer.LevelData_Spyro2_Agent9] = 0x08179208,

                        [GBAIsometric_Spyro_Pointer.States_Spyro2_LevelObjectives] = 0x08176bf8,
                        [GBAIsometric_Spyro_Pointer.States_Spyro2_Portals] = 0x08176e60,
                        [GBAIsometric_Spyro_Pointer.States_Spyro2_ChallengePortals] = 0x0817788c,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SpyroSeasonFlameUS:
                    return new Dictionary<GBAIsometric_Spyro_Pointer, uint>() {
                        [GBAIsometric_Spyro_Pointer.DataTable] = 0x0817B728,

                        [GBAIsometric_Spyro_Pointer.LocalizationPointers] = 0x08002450,

                        [GBAIsometric_Spyro_Pointer.ObjectTypes] = 0x08175708,
                        [GBAIsometric_Spyro_Pointer.AnimSets] = 0x081730f8,

                        [GBAIsometric_Spyro_Pointer.PortraitSprites] = 0x08175434,
                        [GBAIsometric_Spyro_Pointer.DialogEntries] = 0x0817493c,
                        [GBAIsometric_Spyro_Pointer.CutsceneMaps] = 0x0817add4,

                        //[GBAIsometric_Spyro_Pointer.GemCounts] = ,
                        //[GBAIsometric_Spyro_Pointer.LevelIndices] = ,
                        [GBAIsometric_Spyro_Pointer.LevelNames] = 0x08178a38,
                        //[GBAIsometric_Spyro_Pointer.MenuPages] = ,

                        [GBAIsometric_Spyro_Pointer.LevelMaps] = 0x0817ac58,
                        [GBAIsometric_Spyro_Pointer.LevelObjects] = 0x081792ac,

                        [GBAIsometric_Spyro_Pointer.LevelData] = 0x0817a878,
                        [GBAIsometric_Spyro_Pointer.LevelData_Spyro2_Agent9] = 0x08178ef8,

                        [GBAIsometric_Spyro_Pointer.States_Spyro2_LevelObjectives] = 0x081768e8,
                        [GBAIsometric_Spyro_Pointer.States_Spyro2_Portals] = 0x08176b50,
                        [GBAIsometric_Spyro_Pointer.States_Spyro2_ChallengePortals] = 0x0817757c,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SpyroAdventureEU:
                    return new Dictionary<GBAIsometric_Spyro_Pointer, uint>() {
                        [GBAIsometric_Spyro_Pointer.DataTable] = 0x081c1470,

                        [GBAIsometric_Spyro_Pointer.LocalizationBlockIndices] = 0x081c0d9c,
                        [GBAIsometric_Spyro_Pointer.LocalizationDecompressionBlockIndices] = 0x081c0db4,
                        [GBAIsometric_Spyro_Pointer.LocTables] = 0x081C0C6C,

                        [GBAIsometric_Spyro_Pointer.ObjectTypes] = 0x081c9670,
                        [GBAIsometric_Spyro_Pointer.AnimSets] = 0x081c8d58,

                        [GBAIsometric_Spyro_Pointer.PortraitSprites] = 0x081bfe28,
                        [GBAIsometric_Spyro_Pointer.DialogEntries] = 0x081bf238,

                        [GBAIsometric_Spyro_Pointer.GemCounts] = 0x081C07EA,
                        [GBAIsometric_Spyro_Pointer.LevelIndices] = 0x081C0814,
                        [GBAIsometric_Spyro_Pointer.LevelNameInfos] = 0x081D2C5C,
                        [GBAIsometric_Spyro_Pointer.MenuPages] = 0x081c1234,

                        [GBAIsometric_Spyro_Pointer.LevelMaps] = 0x081d0d64,
                        [GBAIsometric_Spyro_Pointer.LevelObjects] = 0x081d13f0,
                        [GBAIsometric_Spyro_Pointer.LevelObjects_Spyro3_Agent9] = 0x081d1ff4,
                        [GBAIsometric_Spyro_Pointer.LevelObjects_Spyro3_SgtByrd] = 0x081d1550,
                        [GBAIsometric_Spyro_Pointer.LevelObjects_Spyro3_ByrdRescue] = 0x081d1580,

                        [GBAIsometric_Spyro_Pointer.LevelData] = 0x081d0b44,
                        [GBAIsometric_Spyro_Pointer.LevelData_Spyro3_Agent9] = 0x081d22bc,
                        [GBAIsometric_Spyro_Pointer.LevelData_Spyro3_SgtByrd] = 0x081d1d34,

                        [GBAIsometric_Spyro_Pointer.States_Spyro3_NPC] = 0x081cb924,
                        [GBAIsometric_Spyro_Pointer.States_Spyro3_DoorTypes] = 0x081cc020,
                        [GBAIsometric_Spyro_Pointer.States_Spyro3_DoorGraphics] = 0x081cbec0,

                        [GBAIsometric_Spyro_Pointer.QuestItems] = 0x081C0880,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SpyroAdventureUS:
                    return new Dictionary<GBAIsometric_Spyro_Pointer, uint>() {
                        [GBAIsometric_Spyro_Pointer.DataTable] = 0x081C0B60,

                        [GBAIsometric_Spyro_Pointer.LocalizationBlockIndices] = 0x081c05b8,
                        [GBAIsometric_Spyro_Pointer.LocalizationDecompressionBlockIndices] = 0x081c05bc,
                        [GBAIsometric_Spyro_Pointer.LocTables] = 0x081c0488,

                        [GBAIsometric_Spyro_Pointer.ObjectTypes] = 0x081c8954,
                        [GBAIsometric_Spyro_Pointer.AnimSets] = 0x081c8024,

                        [GBAIsometric_Spyro_Pointer.PortraitSprites] = 0x081bf644,
                        [GBAIsometric_Spyro_Pointer.DialogEntries] = 0x081bea54,

                        [GBAIsometric_Spyro_Pointer.GemCounts] = 0x081c0006,
                        [GBAIsometric_Spyro_Pointer.LevelIndices] = 0x081c0030,
                        [GBAIsometric_Spyro_Pointer.LevelNameInfos] = 0x081d1f44,
                        [GBAIsometric_Spyro_Pointer.MenuPages] = 0x081c09b0,

                        [GBAIsometric_Spyro_Pointer.LevelMaps] = 0x081d0058,
                        [GBAIsometric_Spyro_Pointer.LevelObjects] = 0x081d06e4,
                        [GBAIsometric_Spyro_Pointer.LevelObjects_Spyro3_Agent9] = 0x081d12e8,
                        [GBAIsometric_Spyro_Pointer.LevelObjects_Spyro3_SgtByrd] = 0x081d0844,
                        [GBAIsometric_Spyro_Pointer.LevelObjects_Spyro3_ByrdRescue] = 0x081d0874,

                        [GBAIsometric_Spyro_Pointer.LevelData] = 0x081CFE38,
                        [GBAIsometric_Spyro_Pointer.LevelData_Spyro3_Agent9] = 0x081D15B0,
                        [GBAIsometric_Spyro_Pointer.LevelData_Spyro3_SgtByrd] = 0x081D1028,

                        [GBAIsometric_Spyro_Pointer.States_Spyro3_NPC] = 0x081cac00,
                        [GBAIsometric_Spyro_Pointer.States_Spyro3_DoorTypes] = 0x081cb2fc,
                        [GBAIsometric_Spyro_Pointer.States_Spyro3_DoorGraphics] = 0x081cb19c,

                        [GBAIsometric_Spyro_Pointer.QuestItems] = 0x081c009c,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.Tron2KillerAppEU:
                case GameModeSelection.Tron2KillerAppUS:
                    return new Dictionary<GBAIsometric_Spyro_Pointer, uint>() {
                        [GBAIsometric_Spyro_Pointer.DataTable] = 0x0812534c,

                        //[GBAIsometric_Spyro_Pointer.LocalizationBlockIndices] = ,
                        //[GBAIsometric_Spyro_Pointer.LocalizationDecompressionBlockIndices] = ,
                        //[GBAIsometric_Spyro_Pointer.LocTables] = ,

                        [GBAIsometric_Spyro_Pointer.ObjectTypes] = 0x0812c5f8,
                        [GBAIsometric_Spyro_Pointer.AnimSets] = 0x0812bff8,

                        [GBAIsometric_Spyro_Pointer.PortraitSprites] = 0x08124410 + 16, // First one is null
                        //[GBAIsometric_Spyro_Pointer.DialogEntries] = ,

                        //[GBAIsometric_Spyro_Pointer.LevelNameInfos] = ,

                        [GBAIsometric_Spyro_Pointer.LevelObjects] = 0x08131ffc + 4, // First one is null

                        [GBAIsometric_Spyro_Pointer.LevelData] = 0x08131d34,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                default:
                    return null;
            }
        }

        public static Dictionary<GBAVV_Pointer, Pointer> GBAVV_PointerTable(GameModeSelection gameMode, BinaryFile romFile) {
            switch (gameMode) 
            {
                // Crash 1

                case GameModeSelection.Crash1GBAEU:
                    return new Dictionary<GBAVV_Pointer, uint>() {
                        [GBAVV_Pointer.LevelInfo] = 0x0816c86c,
                        [GBAVV_Pointer.Localization] = 0x087e5e34,

                        [GBAVV_Pointer.Map2D_Graphics] = 0x084a5600,

                        [GBAVV_Pointer.Mode7_LevelInfo] = 0x08175558,

                        [GBAVV_Pointer.Mode7_Type0_ChaseObjAnimations] = 0x0817a850,
                        [GBAVV_Pointer.Mode7_Type0_ChaseObjFrames] = 0x0817a880,
                        [GBAVV_Pointer.Mode7_Crash1_Type0_TilePalette_0F] = 0x0817aa6c,

                        [GBAVV_Pointer.Mode7_Crash1_Type1_SpecialFrame] = 0x08167ad4,
                        [GBAVV_Pointer.Mode7_Crash1_Type2_SpecialFrame] = 0x08169ae8,

                        [GBAVV_Pointer.Mode7_Crash1_PolarDeathPalette] = 0x0817a728,

                        [GBAVV_Pointer.WorldMap_Crash1_LevelIcons] = 0x0816c5a0,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.Crash1GBAUS:
                    return new Dictionary<GBAVV_Pointer, uint>() {
                        [GBAVV_Pointer.LevelInfo] = 0x0816c3ac,
                        [GBAVV_Pointer.Localization] = 0x08172c18,

                        [GBAVV_Pointer.Map2D_Graphics] = 0x084a3624,

                        [GBAVV_Pointer.Mode7_LevelInfo] = 0x081736a8,

                        [GBAVV_Pointer.Mode7_Type0_ChaseObjAnimations] = 0x081789a0,
                        [GBAVV_Pointer.Mode7_Type0_ChaseObjFrames] = 0x081789d0,
                        [GBAVV_Pointer.Mode7_Crash1_Type0_TilePalette_0F] = 0x08178bbc,

                        [GBAVV_Pointer.Mode7_Crash1_Type1_SpecialFrame] = 0x08167614,
                        [GBAVV_Pointer.Mode7_Crash1_Type2_SpecialFrame] = 0x08169628,

                        [GBAVV_Pointer.Mode7_Crash1_PolarDeathPalette] = 0x08178878,

                        [GBAVV_Pointer.WorldMap_Crash1_LevelIcons] = 0x0816c0e0,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.Crash1GBAJP:
                    return new Dictionary<GBAVV_Pointer, uint>() {
                        [GBAVV_Pointer.LevelInfo] = 0x0816e5f0,
                        //[GBAVV_Pointer.Localization] = ,

                        [GBAVV_Pointer.Map2D_Graphics] = 0x084a63f8,

                        [GBAVV_Pointer.Mode7_LevelInfo] = 0x0817601c,

                        [GBAVV_Pointer.Mode7_Type0_ChaseObjAnimations] = 0x0817b314,
                        [GBAVV_Pointer.Mode7_Type0_ChaseObjFrames] = 0x0817b344,
                        [GBAVV_Pointer.Mode7_Crash1_Type0_TilePalette_0F] = 0x0817b530,

                        [GBAVV_Pointer.Mode7_Crash1_Type1_SpecialFrame] = 0x0816980c,
                        [GBAVV_Pointer.Mode7_Crash1_Type2_SpecialFrame] = 0x0816b820,

                        [GBAVV_Pointer.Mode7_Crash1_PolarDeathPalette] = 0x0817b1ec,

                        [GBAVV_Pointer.WorldMap_Crash1_LevelIcons] = 0x0816e324,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                // Crash 2

                case GameModeSelection.Crash2GBAEU:
                    return new Dictionary<GBAVV_Pointer, uint>() {
                        [GBAVV_Pointer.LevelInfo] = 0x081d0430,
                        [GBAVV_Pointer.Localization] = 0x087ff064,

                        [GBAVV_Pointer.Map2D_Graphics] = 0x084768b4,

                        [GBAVV_Pointer.Mode7_LevelInfo] = 0x081df698,
                        [GBAVV_Pointer.Mode7_TilePalette_Type0] = 0x081df804,
                        [GBAVV_Pointer.Mode7_TilePalette_Type1_Flames] = 0x081e0574,

                        [GBAVV_Pointer.Mode7_Crash2_Type0_BG1] = 0x080cfd1c,
                        [GBAVV_Pointer.Mode7_Type0_ChaseObjAnimations] = 0x081dff1c,
                        [GBAVV_Pointer.Mode7_Type0_ChaseObjFrames] = 0x081dff70,

                        [GBAVV_Pointer.Mode7_Crash2_Type1_FlamesTileMaps] = 0x081e0634,
                        [GBAVV_Pointer.Mode7_Crash2_Type1_FlamesTileSetLengths] = 0x081e05e4,
                        [GBAVV_Pointer.Mode7_Crash2_Type1_FlamesTileSets] = 0x081e0594,

                        [GBAVV_Pointer.Isometric_MapDatas] = 0x086c214c,
                        [GBAVV_Pointer.Isometric_ObjectDatas] = 0x086c23b4,
                        [GBAVV_Pointer.Isometric_ObjAnimations] = 0x086d786c,

                        [GBAVV_Pointer.Isometric_Characters] = 0x086d7d94,
                        [GBAVV_Pointer.Isometric_CharacterIcons] = 0x086cdee8,

                        [GBAVV_Pointer.Isometric_ObjPalette_0] = 0x087d946c,
                        [GBAVV_Pointer.Isometric_ObjPalette_1] = 0x087de7a8,
                        [GBAVV_Pointer.Isometric_ObjPalette_2] = 0x087de7c8,
                        [GBAVV_Pointer.Isometric_ObjPalette_4] = 0x087de7e8,
                        [GBAVV_Pointer.Isometric_ObjPalette_11] = 0x087de808,
                        [GBAVV_Pointer.Isometric_ObjPalette_12] = 0x087de828,
                        [GBAVV_Pointer.Isometric_ObjPalette_13] = 0x087de848,

                        [GBAVV_Pointer.Isometric_AdditionalAnim0_Frames] = 0x086cf750,
                        [GBAVV_Pointer.Isometric_AdditionalAnim1_Frames] = 0x086cf75c,
                        [GBAVV_Pointer.Isometric_AdditionalAnim2_Frames] = 0x086d7c20,
                        [GBAVV_Pointer.Isometric_AdditionalAnim3_Frames] = 0x086d7c78,
                        [GBAVV_Pointer.Isometric_AdditionalAnim4_Frames] = 0x086d7cbc,
                        [GBAVV_Pointer.Isometric_AdditionalAnim5_Frames] = 0x086d7c38,
                        [GBAVV_Pointer.Isometric_AdditionalAnim6_Frames] = 0x086d7c58,
                        [GBAVV_Pointer.Isometric_AdditionalAnim7_Frames] = 0x086d77c0,
                        [GBAVV_Pointer.Isometric_AdditionalAnim8_Frames] = 0x086d7c94,
                        [GBAVV_Pointer.Isometric_AdditionalAnim9_Frames] = 0x086d7830,
                        [GBAVV_Pointer.Isometric_AdditionalAnim10_Frames] = 0x086d80c4,

                        [GBAVV_Pointer.Isometric_AdditionalAnim8_Palette] = 0x087eafa8,
                        [GBAVV_Pointer.Isometric_AdditionalAnim9_Palette] = 0x087de7c8,
                        [GBAVV_Pointer.Isometric_AdditionalAnim10_Palette] = 0x087f79e8,

                        [GBAVV_Pointer.WorldMap] = 0x081e3924,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.Crash2GBAUS:
                    return new Dictionary<GBAVV_Pointer, uint>() {
                        [GBAVV_Pointer.LevelInfo] = 0x081d2714,
                        [GBAVV_Pointer.Localization] = 0x081d5c04,

                        [GBAVV_Pointer.Map2D_Graphics] = 0x0846dd74,

                        [GBAVV_Pointer.Mode7_LevelInfo] = 0x081d6b58,
                        [GBAVV_Pointer.Mode7_TilePalette_Type0] = 0x081d6cc4,
                        [GBAVV_Pointer.Mode7_TilePalette_Type1_Flames] = 0x081d7a34, // Full palette at 0x081d6ec4

                        [GBAVV_Pointer.Mode7_Crash2_Type0_BG1] = 0x080cfc7c,
                        [GBAVV_Pointer.Mode7_Type0_ChaseObjAnimations] = 0x081d73dc,
                        [GBAVV_Pointer.Mode7_Type0_ChaseObjFrames] = 0x081d7430,

                        [GBAVV_Pointer.Mode7_Crash2_Type1_FlamesTileMaps] = 0x081d7af4,
                        [GBAVV_Pointer.Mode7_Crash2_Type1_FlamesTileSetLengths] = 0x081d7aa4,
                        [GBAVV_Pointer.Mode7_Crash2_Type1_FlamesTileSets] = 0x081d7a54,

                        [GBAVV_Pointer.Isometric_MapDatas] = 0x086bf00c,
                        [GBAVV_Pointer.Isometric_ObjectDatas] = 0x086bf274,
                        [GBAVV_Pointer.Isometric_ObjAnimations] = 0x086d4744,

                        [GBAVV_Pointer.Isometric_Characters] = 0x086d4c6c,
                        [GBAVV_Pointer.Isometric_CharacterIcons] = 0x086cada8,

                        [GBAVV_Pointer.Isometric_ObjPalette_0] = 0x087d639c,
                        [GBAVV_Pointer.Isometric_ObjPalette_1] = 0x087db6d8,
                        [GBAVV_Pointer.Isometric_ObjPalette_2] = 0x087db6f8,
                        [GBAVV_Pointer.Isometric_ObjPalette_4] = 0x087db718,
                        [GBAVV_Pointer.Isometric_ObjPalette_11] = 0x087db738,
                        [GBAVV_Pointer.Isometric_ObjPalette_12] = 0x087db758,
                        [GBAVV_Pointer.Isometric_ObjPalette_13] = 0x087db778,

                        [GBAVV_Pointer.Isometric_AdditionalAnim0_Frames] = 0x086cc610,
                        [GBAVV_Pointer.Isometric_AdditionalAnim1_Frames] = 0x086cc61c,
                        [GBAVV_Pointer.Isometric_AdditionalAnim2_Frames] = 0x086d4af8,
                        [GBAVV_Pointer.Isometric_AdditionalAnim3_Frames] = 0x086d4b50,
                        [GBAVV_Pointer.Isometric_AdditionalAnim4_Frames] = 0x086d4b94,
                        [GBAVV_Pointer.Isometric_AdditionalAnim5_Frames] = 0x086d4b10,
                        [GBAVV_Pointer.Isometric_AdditionalAnim6_Frames] = 0x086d4b30,
                        [GBAVV_Pointer.Isometric_AdditionalAnim7_Frames] = 0x086d4680,
                        [GBAVV_Pointer.Isometric_AdditionalAnim8_Frames] = 0x086d4b6c,
                        [GBAVV_Pointer.Isometric_AdditionalAnim9_Frames] = 0x086d46f0,
                        [GBAVV_Pointer.Isometric_AdditionalAnim10_Frames] = 0x086d4ff4,

                        [GBAVV_Pointer.Isometric_AdditionalAnim8_Palette] = 0x087e7ed8,
                        [GBAVV_Pointer.Isometric_AdditionalAnim9_Palette] = 0x087db040,
                        [GBAVV_Pointer.Isometric_AdditionalAnim10_Palette] = 0x087f4918,

                        [GBAVV_Pointer.WorldMap] = 0x081dade4,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.Crash2GBAJP:
                    return new Dictionary<GBAVV_Pointer, uint>() {
                        [GBAVV_Pointer.LevelInfo] = 0x081c46a8,
                        //[GBAVV_Pointer.Localization] = ,

                        [GBAVV_Pointer.Map2D_Graphics] = 0x08460b70,

                        [GBAVV_Pointer.Mode7_LevelInfo] = 0x081c9434,
                        [GBAVV_Pointer.Mode7_TilePalette_Type0] = 0x081c95a0,
                        [GBAVV_Pointer.Mode7_TilePalette_Type1_Flames] = 0x081ca310,

                        [GBAVV_Pointer.Mode7_Crash2_Type0_BG1] = 0x080d2928,
                        [GBAVV_Pointer.Mode7_Type0_ChaseObjAnimations] = 0x081c9cb8,
                        [GBAVV_Pointer.Mode7_Type0_ChaseObjFrames] = 0x081c9d0c,

                        [GBAVV_Pointer.Mode7_Crash2_Type1_FlamesTileMaps] = 0x081ca3d0,
                        [GBAVV_Pointer.Mode7_Crash2_Type1_FlamesTileSetLengths] = 0x081ca380,
                        [GBAVV_Pointer.Mode7_Crash2_Type1_FlamesTileSets] = 0x081ca330,

                        [GBAVV_Pointer.Isometric_MapDatas] = 0x086b1ee4,
                        [GBAVV_Pointer.Isometric_ObjectDatas] = 0x086b214c,
                        [GBAVV_Pointer.Isometric_ObjAnimations] = 0x086c760c,

                        [GBAVV_Pointer.Isometric_Characters] = 0x086c7b34,
                        [GBAVV_Pointer.Isometric_CharacterIcons] = 0x086bdc70,

                        [GBAVV_Pointer.Isometric_ObjPalette_0] = 0x087c91e4,
                        [GBAVV_Pointer.Isometric_ObjPalette_1] = 0x087ce880,
                        [GBAVV_Pointer.Isometric_ObjPalette_2] = 0x087ce8a0,
                        [GBAVV_Pointer.Isometric_ObjPalette_4] = 0x087ce8c0,
                        [GBAVV_Pointer.Isometric_ObjPalette_11] = 0x087ce8e0,
                        [GBAVV_Pointer.Isometric_ObjPalette_12] = 0x087ce900,
                        [GBAVV_Pointer.Isometric_ObjPalette_13] = 0x087ce920,

                        [GBAVV_Pointer.Isometric_AdditionalAnim0_Frames] = 0x086bf4d8,
                        [GBAVV_Pointer.Isometric_AdditionalAnim1_Frames] = 0x086bf4e4,
                        [GBAVV_Pointer.Isometric_AdditionalAnim2_Frames] = 0x086c79c0,
                        [GBAVV_Pointer.Isometric_AdditionalAnim3_Frames] = 0x086c7a18,
                        [GBAVV_Pointer.Isometric_AdditionalAnim4_Frames] = 0x086c7a5c,
                        [GBAVV_Pointer.Isometric_AdditionalAnim5_Frames] = 0x086c79d8,
                        [GBAVV_Pointer.Isometric_AdditionalAnim6_Frames] = 0x086c79f8,
                        [GBAVV_Pointer.Isometric_AdditionalAnim7_Frames] = 0x086c7548,
                        [GBAVV_Pointer.Isometric_AdditionalAnim8_Frames] = 0x086c7a34,
                        [GBAVV_Pointer.Isometric_AdditionalAnim9_Frames] = 0x086c75b8,
                        [GBAVV_Pointer.Isometric_AdditionalAnim10_Frames] = 0x086c7e64,

                        [GBAVV_Pointer.Isometric_AdditionalAnim8_Palette] = 0x087db080,
                        [GBAVV_Pointer.Isometric_AdditionalAnim9_Palette] = 0x087ce8a0,
                        [GBAVV_Pointer.Isometric_AdditionalAnim10_Palette] = 0x087e7ac0,

                        [GBAVV_Pointer.WorldMap] = 0x081cd6bc,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                default:
                    return null;
            }
        }
    }
}