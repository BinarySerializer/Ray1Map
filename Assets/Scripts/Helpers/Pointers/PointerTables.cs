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
                                [GBA_Pointer.UiOffsetTable] = 0x0802F5EC
                            }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                        case GBA_R3MadTrax_Manager.Files.client_pad145:
                            return new Dictionary<GBA_Pointer, uint>()
                            {
                                [GBA_Pointer.UiOffsetTable] = 0x0802e34c
                            }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                        case GBA_R3MadTrax_Manager.Files.client_pad2:
                        case GBA_R3MadTrax_Manager.Files.client_pad3:
                            return new Dictionary<GBA_Pointer, uint>()
                            {
                                [GBA_Pointer.UiOffsetTable] = 0x08025970
                            }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                        default:
                            throw new ArgumentOutOfRangeException();
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

                case GameModeSelection.DonaldDuckAdvanceGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x081DEFD4,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.CrouchingTigerHiddenDragonGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x0829e810,
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
                        [GBARRR_Pointer.Mode7_MapData] = 0x08708194, // It's compressed! // TODO: Add to export
                        [GBARRR_Pointer.Mode7_UnkMapData] = 0x087081a0, // It's compressed! // TODO: Add to export
                        [GBARRR_Pointer.Mode7_TilePalette] = 0x087081b8,
                        [GBARRR_Pointer.Mode7_BG1Palette] = 0x0870820c,
                        [GBARRR_Pointer.Mode7_BG0Palette] = 0x087081e8,

                        [GBARRR_Pointer.Mode7_Array1] = 0x08708174, // Appears to be very different than on EU
                        [GBARRR_Pointer.Mode7_Array2] = 0x08708150,
                        [GBARRR_Pointer.Mode7_Array3] = 0x08708168,

                        [GBARRR_Pointer.Mode7UnknownPal] = 0x0868C9B8,

                        [GBARRR_Pointer.RNC_0] = 0x086C20EC,
                        [GBARRR_Pointer.RNC_1] = 0x08701318,
                        [GBARRR_Pointer.RNC_2] = 0x086E1164,
                        [GBARRR_Pointer.RNC_3] = 0x086E2044,
                        [GBARRR_Pointer.RNC_4] = 0x086c2378,
                        [GBARRR_Pointer.RNC_5] = 0x086bd1d1,

                        [GBARRR_Pointer.Mode7_ComprArray1] = 0x08708144,
                        [GBARRR_Pointer.Mode7_ComprArray2] = 0x087081C4,
                        [GBARRR_Pointer.Mode7_Objects] = 0x087081ac,
                        [GBARRR_Pointer.Mode7_ComprArray4] = 0x0870815c,

                        [GBARRR_Pointer.MenuArray] = 0x087087D4,

                        [GBARRR_Pointer.MusicTable] = 0x0866E710,
                        [GBARRR_Pointer.MusicSampleTable] = 0x083C37BC,
                        [GBARRR_Pointer.SoundEffectSampleTable] = 0x0866D2E8,
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
                        [GBARRR_Pointer.Mode7_UnkMapData] = 0x08721910,
                        [GBARRR_Pointer.Mode7_TilePalette] = 0x08721928,
                        [GBARRR_Pointer.Mode7_BG1Palette] = 0x0872197c,
                        [GBARRR_Pointer.Mode7_BG0Palette] = 0x08721958,

                        [GBARRR_Pointer.Mode7_Array1] = 0x087218e4,
                        [GBARRR_Pointer.Mode7_Array2] = 0x087218c0,
                        [GBARRR_Pointer.Mode7_Array3] = 0x087218d8,

                        [GBARRR_Pointer.Mode7UnknownPal] = 0x086a6128,

                        [GBARRR_Pointer.RNC_0] = 0x086db85c,
                        [GBARRR_Pointer.RNC_1] = 0x0871aa88,
                        [GBARRR_Pointer.RNC_2] = 0x086fa8d4,
                        [GBARRR_Pointer.RNC_3] = 0x086fb7b4,
                        [GBARRR_Pointer.RNC_4] = 0x086dbae8,
                        [GBARRR_Pointer.RNC_5] = 0x086d6941,

                        [GBARRR_Pointer.Mode7_ComprArray1] = 0x087218b4,
                        [GBARRR_Pointer.Mode7_ComprArray2] = 0x08721934,
                        [GBARRR_Pointer.Mode7_Objects] = 0x0872191c,
                        [GBARRR_Pointer.Mode7_ComprArray4] = 0x087218cc,

                        [GBARRR_Pointer.MenuArray] = 0x08721f44,

                        [GBARRR_Pointer.MusicTable] = 0x08607f10,
                        [GBARRR_Pointer.MusicSampleTable] = 0x083c3a9c,
                        [GBARRR_Pointer.SoundEffectSampleTable] = 0x08606ae8,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the pointer table for the specified GBA Isometric version
        /// </summary>
        /// <param name="gameMode">The GBA game mode</param>
        /// <param name="romFile">The rom file</param>
        /// <returns>The pointer table</returns>
        public static Dictionary<GBAIsometric_RHR_Pointer, Pointer> GBAIsometric_PointerTable(GameModeSelection gameMode, BinaryFile romFile) {
            switch (gameMode) 
            {
                case GameModeSelection.RaymanHoodlumsRevengeEU:
                    return new Dictionary<GBAIsometric_RHR_Pointer, uint>() {
                        [GBAIsometric_RHR_Pointer.MusyxFile] = 0x080447AC,
                        [GBAIsometric_RHR_Pointer.Levels] = 0x080E938C,
                        [GBAIsometric_RHR_Pointer.Localization] = 0x087F5E04,
                        [GBAIsometric_RHR_Pointer.ObjTypes] = 0x080F9814,

                        // TODO: Add menu pointers
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.RaymanHoodlumsRevengeUS:
                    return new Dictionary<GBAIsometric_RHR_Pointer, uint>() {
                        [GBAIsometric_RHR_Pointer.MusyxFile] = 0x08044708,
                        [GBAIsometric_RHR_Pointer.Levels] = 0x080E92E8,
                        [GBAIsometric_RHR_Pointer.Localization] = 0x087F48CC,
                        [GBAIsometric_RHR_Pointer.ObjTypes] = 0x080f9770,

                        [GBAIsometric_RHR_Pointer.Font0] = 0x080ea15c,
                        [GBAIsometric_RHR_Pointer.Font1] = 0x080eaa10,
                        [GBAIsometric_RHR_Pointer.Font2] = 0x080ea2f8,

                        [GBAIsometric_RHR_Pointer.Map_PauseFrame] = 0x08481930,
                        [GBAIsometric_RHR_Pointer.Map_Menu0] = 0x084817f4,
                        [GBAIsometric_RHR_Pointer.Map_Menu1] = 0x084818f0,
                        [GBAIsometric_RHR_Pointer.Map_Menu2] = 0x08481838,
                        [GBAIsometric_RHR_Pointer.Map_Menu3] = 0x08481888,
                        [GBAIsometric_RHR_Pointer.Map_WorldMap] = 0x08481bf0,
                        [GBAIsometric_RHR_Pointer.Map_ScoreScreen] = 0x08481970,
                        [GBAIsometric_RHR_Pointer.Map_Unk0] = 0x084817c0,
                        [GBAIsometric_RHR_Pointer.Map_Unk1] = 0x08481348,
                        [GBAIsometric_RHR_Pointer.Map_Unk2] = 0x08480ec4,
                        [GBAIsometric_RHR_Pointer.Map_Unk3] = 0x08480c84,
                        [GBAIsometric_RHR_Pointer.Map_Unk4] = 0x084e42b0,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                default:
                    return null;
            }
        }
    }
}