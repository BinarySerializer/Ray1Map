using System;
using System.Text;
using BinarySerializer;

namespace R1Engine
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
            Platform = atr.Platform;
            GameDirectory = Util.NormalizePath(gameDirectory, isFolder: true);
            World = world;
            Level = level;
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

        public R1_World R1_World
        {
            get => (R1_World)World;
            set => World = (int)value;
        }

        public bool GBA_IsShanghai => EngineVersion <= EngineVersion.GBA_R3_MadTrax;
        public bool GBA_IsMilan => EngineVersion <= EngineVersion.GBA_TombRaiderTheProphecy && EngineVersion >= EngineVersion.GBA_TomClancysRainbowSixRogueSpear;
        public bool GBA_IsCommon => EngineVersion >= EngineVersion.GBA_BatmanVengeance;

        public bool GBAVV_IsFusion => EngineVersion == EngineVersion.GBAVV_CrashFusion || EngineVersion == EngineVersion.GBAVV_SpyroFusion;

        public Jade_Version Jade_Version {
            get {
                if (Game == Game.Jade_BGE) {
                    return Jade_Version.Default;
                } else if (EngineVersion < EngineVersion.Jade_PoP_SandsOfTime) {
                    if (EngineVersion >= EngineVersion.Jade_Horsez2 && EngineVersion <= EngineVersion.Jade_MovieGames) {
                        return Jade_Version.Phoenix;
                    } else if (Platform == Platform.Xbox360 || GameModeSelection == GameModeSelection.KingKongPCGamersEdition) {
                        return Jade_Version.Xenon;
                    } else {
                        return Jade_Version.Default;
                    }
                } else {
                    return Jade_Version.Montreal;
                }
            }
        }

        // Helpers

        public IGameManager GetGameManager => (IGameManager)Activator.CreateInstance(GameModeSelection.GetAttribute<GameModeAttribute>().ManagerType);
        public T GetGameManagerOfType<T>() where T : IGameManager => (T)Activator.CreateInstance(GameModeSelection.GetAttribute<GameModeAttribute>().ManagerType);
    }
}