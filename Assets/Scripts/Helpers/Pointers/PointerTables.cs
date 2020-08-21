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

                    [R1_GBA_ROMPointer.WorldMapVignetteImageData] = 0x081452A4,
                    [R1_GBA_ROMPointer.WorldMapVignetteBlockIndices] = 0x08151504,
                    [R1_GBA_ROMPointer.WorldMapVignettePaletteIndices] = 0x08152284,
                    [R1_GBA_ROMPointer.WorldMapVignettePalettes] = 0x08152944,

                    [R1_GBA_ROMPointer.StringPointers] = 0x0854ADB4,

                    [R1_GBA_ROMPointer.DrumWalkerGraphics] = 0x082C6C5C,
                    [R1_GBA_ROMPointer.ClockGraphics] = 0x082C90C8,
                    [R1_GBA_ROMPointer.InkGraphics] = 0x082D33D0,
                    [R1_GBA_ROMPointer.FontSmallGraphics] = 0x082E74F4,
                    [R1_GBA_ROMPointer.FontLargeGraphics] = 0x082E7514,
                    [R1_GBA_ROMPointer.PinsGraphics] = 0x0832CBF4,
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

                    [R1_GBA_ROMPointer.WorldMapVignetteImageData] = 0x08145208,
                    [R1_GBA_ROMPointer.WorldMapVignetteBlockIndices] = 0x08151468,
                    [R1_GBA_ROMPointer.WorldMapVignettePaletteIndices] = 0x081521E8,
                    [R1_GBA_ROMPointer.WorldMapVignettePalettes] = 0x081528A8,

                    [R1_GBA_ROMPointer.StringPointers] = 0x0854ACE0,

                    [R1_GBA_ROMPointer.DrumWalkerGraphics] = 0x082C6B88,
                    [R1_GBA_ROMPointer.ClockGraphics] = 0x082C8FF4,
                    [R1_GBA_ROMPointer.InkGraphics] = 0x082D32FC,
                    [R1_GBA_ROMPointer.FontSmallGraphics] = 0x082E7420,
                    [R1_GBA_ROMPointer.FontLargeGraphics] = 0x082E7440,
                    [R1_GBA_ROMPointer.PinsGraphics] = 0x0832CB20,
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

                [R1_DSi_Pointer.WorldLevelOffsetTable] = 0x02236BF8,

                [R1_DSi_Pointer.EventGraphicsPointers] = 0x0284B5B0,
                [R1_DSi_Pointer.EventDataPointers] = 0x0284B6F8,
                [R1_DSi_Pointer.EventGraphicsGroupCountTablePointers] = 0x0284B988,
                [R1_DSi_Pointer.LevelEventGraphicsGroupCounts] = 0x0284B840,

                [R1_DSi_Pointer.ClockGraphics] = 0x0281BA8C,
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
        /// <param name="gameMode">The GBA game mode</param>
        /// <param name="romFile">The rom file</param>
        /// <returns>The pointer table</returns>
        public static Dictionary<GBA_Pointer, Pointer> GBA_PointerTable(GameModeSelection gameMode, BinaryFile romFile)
        {
            switch (gameMode)
            {
                case GameModeSelection.Rayman3GBAEU:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x0829BEEC,
                        [GBA_Pointer.LevelInfo] = 0x080D4080,
                        [GBA_Pointer.Localization] = 0x080d4058,
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
                        [GBA_Pointer.UiOffsetTable] = 0x084C1478
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.Rayman3NGage:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x0
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

                case GameModeSelection.SplinterCellGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x082260A4,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.SplinterCellNGage:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x0
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

                case GameModeSelection.StarWarsEpisodeIIIGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x082236C0,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.KingKongGBAEU:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x081EA228,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.BatmanVengeanceGBAEU:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x084FCB9C,
                    }.ToDictionary(x => x.Key, x => new Pointer(x.Value, romFile));

                case GameModeSelection.BatmanVengeanceGBAUS:
                    return new Dictionary<GBA_Pointer, uint>() {
                        [GBA_Pointer.UiOffsetTable] = 0x084FD44C,
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
    }
}