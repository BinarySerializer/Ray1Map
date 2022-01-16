﻿using System;
using BinarySerializer.Ray1;
using Ray1Map.GBAIsometric;

namespace Ray1Map
{
    /// <summary>
    /// Common game settings
    /// </summary>
    public class GameSettings
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="gameModeSelection">The game mode selection</param>
        /// <param name="gameDirectory">The game directory</param>
        /// <param name="world">The game world</param>
        /// <param name="level">The game level, starting at 1</param>
        public GameSettings(GameModeSelection gameModeSelection, string gameDirectory, int world, int level)
        {
            // Get the attribute data
            var atr = gameModeSelection.GetAttribute<GameModeAttribute>();

            GameModeSelection = gameModeSelection;
            Game = atr.Game;
            EngineVersion = atr.EngineVersion;
            MajorEngineVersion = atr.MajorEngineVersion;
            EngineFlags = atr.EngineFlags;
            Platform = atr.Platform;
            GameDirectory = Util.NormalizePath(gameDirectory, isFolder: true);
            World = world;
            Level = level;

            EngineVersionTree = EngineVersionTree.Create(this);
        }

        // Global settings

        /// <summary>
        /// The game mode selection
        /// </summary>
        public GameModeSelection GameModeSelection { get; }

        /// <summary>
        /// The major engine version
        /// </summary>
        public MajorEngineVersion MajorEngineVersion { get; }

        /// <summary>
        /// The engine version
        /// </summary>
        public EngineVersion EngineVersion { get; }

        /// <summary>
        /// Engine parameters that should not be counted as a separate version
        /// </summary>
        public EngineFlags EngineFlags { get; }

        /// <summary>
        /// Engine version tree for engines with many branches
        /// </summary>
        public EngineVersionTree EngineVersionTree { get; }

        /// <summary>
        /// The game
        /// </summary>
        public Game Game { get; }

        /// <summary>
        /// The platform this game is on
        /// </summary>
        public Platform Platform { get; }

        /// <summary>
        /// The game directory
        /// </summary>
        public string GameDirectory { get; set; }

        /// <summary>
        /// The game world
        /// </summary>
        public int World { get; set; }

        /// <summary>
        /// The game level, starting at 1
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// The educational game volume name
        /// </summary>
        public string EduVolume { get; set; }

        // Specific game helpers

        public World R1_World
        {
            get => (World)World;
            set => World = (int)value;
        }

        public bool GBA_IsShanghai => EngineVersion <= EngineVersion.GBA_R3_MadTrax;
        public bool GBA_IsMilan => EngineVersion <= EngineVersion.GBA_TombRaiderTheProphecy && EngineVersion >= EngineVersion.GBA_TomClancysRainbowSixRogueSpear;
        public bool GBA_IsCommon => EngineVersion >= EngineVersion.GBA_BatmanVengeance;

        public bool GBAVV_IsFusion => EngineVersion == EngineVersion.GBAVV_CrashFusion || EngineVersion == EngineVersion.GBAVV_SpyroFusion;

        // Helpers

        public BaseGameManager GetGameManager => GameModeSelection.GetManager();
        public T GetGameManagerOfType<T>() where T : BaseGameManager => (T)GetGameManager;
        public Ray1Settings GetRay1Settings()
        {
            Ray1EngineVersion engineVersion = EngineVersion switch
            {
                EngineVersion.R1_PS1 => Ray1EngineVersion.PS1,
                EngineVersion.R2_PS1 => Ray1EngineVersion.R2_PS1,
                EngineVersion.R1_PS1_JP => Ray1EngineVersion.PS1_JP,
                EngineVersion.R1_PS1_JPDemoVol3 => Ray1EngineVersion.PS1_JPDemoVol3,
                EngineVersion.R1_PS1_JPDemoVol6 => Ray1EngineVersion.PS1_JPDemoVol6,
                EngineVersion.R1_Saturn => Ray1EngineVersion.Saturn,
                EngineVersion.R1_PC => Ray1EngineVersion.PC,
                EngineVersion.R1_PocketPC => Ray1EngineVersion.PocketPC,
                EngineVersion.R1_PC_Kit => Ray1EngineVersion.PC_Kit,
                EngineVersion.R1_PC_Edu => Ray1EngineVersion.PC_Edu,
                EngineVersion.R1_PS1_Edu => Ray1EngineVersion.PS1_Edu,
                EngineVersion.R1_GBA => Ray1EngineVersion.GBA,
                EngineVersion.R1_DSi => Ray1EngineVersion.DSi,
                EngineVersion.R1Jaguar => Ray1EngineVersion.Jaguar,
                EngineVersion.R1Jaguar_Proto => Ray1EngineVersion.Jaguar_Proto,
                EngineVersion.R1Jaguar_Demo => Ray1EngineVersion.Jaguar_Demo,
                EngineVersion.SNES => Ray1EngineVersion.SNES,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (GameModeSelection == GameModeSelection.RaymanByHisFansPC || GameModeSelection == GameModeSelection.Rayman60LevelsPC)
                engineVersion = Ray1EngineVersion.PC_Fan;

            if (GameModeSelection == GameModeSelection.RaymanPS1EUDemo)
                engineVersion = Ray1EngineVersion.PS1_EUDemo;

            Ray1PCVersion pcVersion = GameModeSelection switch
            {
                GameModeSelection.RaymanPC_1_00 => Ray1PCVersion.PC_1_00,
                GameModeSelection.RaymanPC_1_10 => Ray1PCVersion.PC_1_10,
                GameModeSelection.RaymanPC_1_12 => Ray1PCVersion.PC_1_12,
                GameModeSelection.RaymanPC_1_20 => Ray1PCVersion.PC_1_20,
                GameModeSelection.RaymanPC_1_21_JP => Ray1PCVersion.PC_1_21_JP,
                GameModeSelection.RaymanPC_1_21 => Ray1PCVersion.PC_1_21,
                GameModeSelection.RaymanPC_Demo_1 => Ray1PCVersion.PC_Demo_1,
                GameModeSelection.RaymanPC_Demo_2 => Ray1PCVersion.PC_Demo_2,
                GameModeSelection.RaymanPocketPC => Ray1PCVersion.PocketPC,
                GameModeSelection.RaymanClassicMobile => Ray1PCVersion.Android,
                _ => Ray1PCVersion.None
            };

            return new Ray1Settings(
                engineVersion: engineVersion, 
                world: R1_World, 
                level: Level, 
                pcVersion: pcVersion,
                volume: EduVolume);
        }
        public GBAIsometricSettings GetGBAIsometricSettings()
        {
            return GameModeSelection switch
            {
                GameModeSelection.SpyroSeasonIceEU => new GBAIsometricSettings(GBAIsometricEngineVersion.Spyro1, GBAIsometricRegion.EU),
                GameModeSelection.SpyroSeasonIceUS => new GBAIsometricSettings(GBAIsometricEngineVersion.Spyro1, GBAIsometricRegion.US),
                GameModeSelection.SpyroSeasonIceJP => new GBAIsometricSettings(GBAIsometricEngineVersion.Spyro1, GBAIsometricRegion.JP),
                GameModeSelection.SpyroSeasonFlameEU => new GBAIsometricSettings(GBAIsometricEngineVersion.Spyro2, GBAIsometricRegion.EU),
                GameModeSelection.SpyroSeasonFlameUS => new GBAIsometricSettings(GBAIsometricEngineVersion.Spyro2, GBAIsometricRegion.US),
                GameModeSelection.SpyroAdventureEU => new GBAIsometricSettings(GBAIsometricEngineVersion.Spyro3, GBAIsometricRegion.EU),
                GameModeSelection.SpyroAdventureUS => new GBAIsometricSettings(GBAIsometricEngineVersion.Spyro3, GBAIsometricRegion.US),
                GameModeSelection.Tron2KillerAppEU => new GBAIsometricSettings(GBAIsometricEngineVersion.Tron, GBAIsometricRegion.EU),
                GameModeSelection.Tron2KillerAppUS => new GBAIsometricSettings(GBAIsometricEngineVersion.Tron, GBAIsometricRegion.US),
                GameModeSelection.RaymanHoodlumsRevengeEU => new GBAIsometricSettings(GBAIsometricEngineVersion.Rayman, GBAIsometricRegion.EU),
                GameModeSelection.RaymanHoodlumsRevengeUS => new GBAIsometricSettings(GBAIsometricEngineVersion.Rayman, GBAIsometricRegion.US),
                _ => throw new ArgumentOutOfRangeException(nameof(GameModeSelection), GameModeSelection, null)
            };
        }
    }
}