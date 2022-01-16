using System;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometricSettings
    {
        public GBAIsometricSettings() { }
        public GBAIsometricSettings(GBAIsometricEngineVersion engineVersion, GBAIsometricRegion region)
        {
            MajorEngineVersion = engineVersion switch
            {
                GBAIsometricEngineVersion.Spyro1 => GBAIsometricMajorEngineVersion.Ice,
                GBAIsometricEngineVersion.Spyro2 => GBAIsometricMajorEngineVersion.Dragon,
                GBAIsometricEngineVersion.Spyro3 => GBAIsometricMajorEngineVersion.Dragon,
                GBAIsometricEngineVersion.Tron => GBAIsometricMajorEngineVersion.Dragon,
                GBAIsometricEngineVersion.Rayman => GBAIsometricMajorEngineVersion.Rayman,
                _ => throw new ArgumentOutOfRangeException(nameof(engineVersion), engineVersion, null)
            };
            EngineVersion = engineVersion;
            Region = region;

            switch (engineVersion)
            {
                case GBAIsometricEngineVersion.Spyro1:
                    ResourcesCount = region switch
                    {
                        GBAIsometricRegion.EU => 83,
                        GBAIsometricRegion.US => 83,
                        GBAIsometricRegion.JP => 84,
                        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                    };
                    PortraitsCount = 24;
                    CutsceneMapsCount = 21;
                    Languages = region switch
                    {
                        GBAIsometricRegion.EU => new[] { "English", "French", "Spanish", "German", "Italian" },
                        GBAIsometricRegion.US => new[] { "English" },
                        GBAIsometricRegion.JP => new[] { "Japanese" },
                        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                    };
                    break;
                
                case GBAIsometricEngineVersion.Spyro2:
                    ResourcesCount = region switch
                    {
                        GBAIsometricRegion.EU => 1514,
                        GBAIsometricRegion.US => 1509,
                        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                    };
                    PortraitsCount = 31;
                    DialogCount = 300;
                    CutsceneMapsCount = 11;
                    LevelMapsCount = 14;
                    TotalLevelsCount = 25;
                    ObjectTypesCount = 509;
                    AnimSetsCount = 162;
                    LevelDataCount = 39;
                    Languages = region switch
                    {
                        GBAIsometricRegion.EU => new [] { "English", "French", "Spanish", "German", "Italian" },
                        GBAIsometricRegion.US => new [] { "English" },
                        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                    };
                    break;
                
                case GBAIsometricEngineVersion.Spyro3:
                    ResourcesCount = region switch
                    {
                        GBAIsometricRegion.EU => 2269,
                        GBAIsometricRegion.US => 2180,
                        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                    };
                    PortraitsCount = 38;
                    DialogCount = 344;
                    PrimaryLevelCount = 14;
                    LevelMapsCount = 21;
                    TotalLevelsCount = 91;
                    ObjectTypesCount = 772;
                    AnimSetsCount = region switch
                    {
                        GBAIsometricRegion.EU => 194,
                        GBAIsometricRegion.US => 196,
                        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                    };
                    LevelDataCount = 80;
                    MenuPageCount = 18;
                    Languages = region switch
                    {
                        GBAIsometricRegion.EU => new[] { "English", "Dutch", "Spanish", "Italian", "German", "French" },
                        GBAIsometricRegion.US => new[] { "English" },
                        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
                    };
                    break;
                
                case GBAIsometricEngineVersion.Tron:
                    ResourcesCount = 2148;
                    PortraitsCount = 57 - 1;
                    ObjectTypesCount = 352;
                    AnimSetsCount = 128;
                    LevelDataCount = 20;
                    break;
                
                case GBAIsometricEngineVersion.Rayman:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(engineVersion), engineVersion, null);
            }
        }

        public GBAIsometricMajorEngineVersion MajorEngineVersion { get; set; }
        public GBAIsometricEngineVersion EngineVersion { get; set; }
        public GBAIsometricRegion Region { get; set; }

        public int ResourcesCount { get; set; }
        public int PortraitsCount { get; set; }
        public int DialogCount { get; set; }
        public int CutsceneMapsCount { get; set; }
        public int PrimaryLevelCount { get; set; }
        public int LevelMapsCount { get; set; }
        public int TotalLevelsCount { get; set; }
        public int ObjectTypesCount { get; set; }
        public int AnimSetsCount { get; set; }
        public int LevelDataCount { get; set; }
        public int MenuPageCount { get; set; }

        public string[] Languages { get; set; }
    }
}