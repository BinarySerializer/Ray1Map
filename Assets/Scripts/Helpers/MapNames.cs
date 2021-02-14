using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// The map names
    /// </summary>
    public static class MapNames
    {
        /// <summary>
        /// Gets the map names for the specified game
        /// </summary>
        /// <param name="game">The game</param>
        /// <returns>The map names</returns>
        public static Dictionary<int, Dictionary<int, string>> GetMapNames(Game game)
        {
            switch (game)
            {
                case Game.R1_Rayman1:
                    return Rayman1MapNames;

                case Game.R1_Designer:
                    return RaymanDesignerMapNames;

                case Game.GBA_Rayman3:
                    return Rayman3GBAMapNames;

                case Game.GBARRR_RavingRabbids:
                    return RavingRabbidsNames;

                case Game.GBAIsometric_RHR:
                    return HoodlumsRevengeNames;

                case Game.GBAIsometric_Spyro2:
                    return Spyro2Names;

                case Game.GBAIsometric_Spyro3:
                    return Spyro3Names;

                case Game.GBC_R1:
                    return Rayman1GBCNames;

                case Game.GBC_R2:
                    return Rayman2GBCNames;

                case Game.GBAVV_Crash1:
                    return Crash1GBANames;

                case Game.GBAVV_Crash2:
                    return Crash2GBANames;

                case Game.Gameloft_RK:
                    return RaymanKartNames;

                case Game.R1_Mapper:
                case Game.R1_ByHisFans:
                case Game.R1_60Levels:
                case Game.R1_Educational:
                case Game.R1_Quiz:
                default:
                    return null;
            }
        }

        public static Dictionary<int, string> GetWorldNames(Game game)
        {
            switch (game)
            {
                case Game.R1_Rayman1:
                case Game.R1_Designer:
                case Game.R1_Mapper:
                case Game.R1_ByHisFans:
                case Game.R1_60Levels:
                case Game.R1_Educational:
                case Game.R1_Quiz:
                case Game.R1_Rayman2:
                    return new Dictionary<int, string>()
                    {
                        [1] = "Jungle",
                        [2] = "Music",
                        [3] = "Mountain",
                        [4] = "Image",
                        [5] = "Cave",
                        [6] = "Cake",
                        [7] = "Menu",
                        [8] = "Multiplayer",
                    };

                case Game.GBA_Rayman3:
                    return new Dictionary<int, string>()
                    {
                        [0] = "Forgotten Forests",
                        [1] = "Haunted Dreams",
                        [2] = "Magmacosm",
                        [3] = "Pirate Stronghold",
                        [4] = "Bonus",
                        [5] = "Ly",
                        [6] = "World",
                        [7] = "Multiplayer",
                        
                        [8] = "Menu",
                        [9] = "GameCube",
                        [10] = "Single Pak",
                    };
                case Game.GBARRR_RavingRabbids:
                    return new Dictionary<int, string>() {
                        [0] = "Child",
                        [1] = "Forest",
                        [2] = "Organic Cave",
                        [3] = "Sweets",
                        [4] = "Dark",
                        [5] = "Title Screen",
                        [10] = "Village",
                        [11] = "Mode7",
                        [12] = "Unused Mode7",
                        [13] = "Menu",
                    };

                case Game.GBA_Rayman3_MadTrax:
                    return new Dictionary<int, string>()
                    {
                        [0] = "client_pad_english",
                        [1] = "client_pad_french",
                        [2] = "client_pad_german",
                        [3] = "client_pad_italian",
                        [4] = "client_pad_spanish",
                        [5] = "client_pad145",
                        [6] = "client_pad2",
                        [7] = "client_pad3",
                    };

                case Game.GBA_PrinceOfPersiaTheSandsOfTime:
                case Game.GBA_SabrinaTheTeenageWitchPotionCommotion:
                case Game.GBA_SplinterCell:
                case Game.GBA_SplinterCellPandoraTomorrow:
                case Game.GBA_StarWarsTrilogyApprenticeOfTheForce:
                case Game.GBA_StarWarsEpisodeIII:
                case Game.GBA_KingKong:
                case Game.GBA_BatmanVengeance:
                case Game.GBA_TMNT:
                case Game.GBA_BatmanRiseOfSinTzu:
                case Game.GBA_OpenSeason:
                case Game.GBA_SurfsUp:
                case Game.GBAIsometric_RHR:
                    return new Dictionary<int, string>()
                    {
                        [0] = "Game",
                        [1] = "Menu",
                    };

                case Game.GBAIsometric_Spyro1:
                case Game.GBAIsometric_Spyro2:
                case Game.GBAIsometric_Spyro3:
                    return new Dictionary<int, string>()
                    {
                        [0] = "3D",
                        [1] = "Agent 9",
                        [2] = "Sgt. Byrd",
                        [3] = "Byrd Rescue",
                        [4] = "Cutscenes",
                    };

                case Game.SNES_Prototype:
                default:
                    return null;
            }
        }

        /// <summary>
        /// The Rayman 1 map names
        /// </summary>
        private static Dictionary<int, Dictionary<int, string>> Rayman1MapNames { get; } = new Dictionary<int, Dictionary<int, string>>()
        {
            [1] = new Dictionary<int, string>()
            {
                [1] = "Pink Plant Woods 1",
                [2] = "Pink Plant Woods 2",
                [3] = "Pink Plant Woods 3 - Betilla",
                [4] = "Pink Plant Woods 4",

                [5] = "Anguish Lagoon 1",
                [6] = "Anguish Lagoon 2 - Boss",
                [7] = "Anguish Lagoon 3 - Bzzit Flight",
                [8] = "Anguish Lagoon 4 - Betilla",

                [9] = "The Swamps of Forgetfulness 1",
                [10] = "The Swamps of Forgetfulness 2",
                [11] = "The Swamps of Forgetfulness 3",

                [12] = "Moskito's Nest 1",
                [13] = "Moskito's Nest 2",
                [14] = "Moskito's Nest 3 - Boss Chase",
                [15] = "Moskito's Nest 4",
                [16] = "Moskito's Nest 5 - Boss",
                [17] = "Moskito's Nest 6 - Betilla",

                [18] = "Magician Bonus 1",
                [19] = "Magician Bonus 2",
                [20] = "Magician Bonus 3",
                [21] = "Magician Bonus 4",

                [22] = "Ray Breakout",
            },
            [2] = new Dictionary<int, string>()
            {
                [1] = "Bongo Hills 1",
                [2] = "Bongo Hills 2",
                [3] = "Bongo Hills 3",
                [4] = "Bongo Hills 4",
                [5] = "Bongo Hills 5",
                [6] = "Bongo Hills 6",

                [7] = "Allegro Presto 1",
                [8] = "Allegro Presto 2",
                [9] = "Allegro Presto 3",
                [10] = "Allegro Presto 4",
                [11] = "Allegro Presto 5 - Betilla",

                [12] = "Gong Heights 1",
                [13] = "Gong Heights 2",

                [14] = "Mr Sax's Hullaballoo 1",
                [15] = "Mr Sax's Hullaballoo 2 - Boss Chase",
                [16] = "Mr Sax's Hullaballoo 3 - Boss",

                [17] = "Magician Bonus 1",
                [18] = "Magician Bonus 2",
            },
            [3] = new Dictionary<int, string>()
            {
                [1] = "Twilight Gulch 1",
                [2] = "Twilight Gulch 2",

                [3] = "The Hard Rocks 1",
                [4] = "The Hard Rocks 2",
                [5] = "The Hard Rocks 3",

                [6] = "Mr Stone's Peaks 1",
                [7] = "Mr Stone's Peaks 2",
                [8] = "Mr Stone's Peaks 3",
                [9] = "Mr Stone's Peaks 4",
                [10] = "Mr Stone's Peaks 5 - Boss",
                [11] = "Mr Stone's Peaks 6 - Betilla",

                [12] = "Magician Bonus 1",
                [13] = "Magician Bonus 2",
            },
            [4] = new Dictionary<int, string>()
            {
                [1] = "Eraser Plains 1",
                [2] = "Eraser Plains 2",
                [3] = "Eraser Plains 3",
                [4] = "Eraser Plains 4 - Boss",

                [5] = "Pencil Pentathlon 1",
                [6] = "Pencil Pentathlon 2",
                [7] = "Pencil Pentathlon 3",

                [8] = "Space Mama's Crater 1",
                [9] = "Space Mama's Crater 2",
                [10] = "Space Mama's Crater 3",
                [11] = "Space Mama's Crater 4 - Boss",

                [12] = "Magician Bonus 1",
                [13] = "Magician Bonus 2",
            },
            [5] = new Dictionary<int, string>()
            {
                [1] = "Crystal Palace 1",
                [2] = "Crystal Palace 2",

                [3] = "Eat at Joe's 1",
                [4] = "Eat at Joe's 2",
                [5] = "Eat at Joe's 3",
                [6] = "Eat at Joe's 4",
                [7] = "Eat at Joe's 5",
                [8] = "Eat at Joe's 6",

                [9] = "Mr Skops' Stalactites 1",
                [10] = "Mr Skops' Stalactites 2 - Boss",
                [11] = "Mr Skops' Stalactites 3 - Boss",

                [12] = "Magician Bonus 1",
            },
            [6] = new Dictionary<int, string>()
            {
                [1] = "Mr Dark's Dare 1",
                [2] = "Mr Dark's Dare 2",
                [3] = "Mr Dark's Dare 3",
                [4] = "Mr Dark's Dare 4 - Boss",
            },
            [7] = new Dictionary<int, string>()
            {
                [0] = "World Map",
            },
            [8] = new Dictionary<int, string>()
            {
                [1] = "Multiplayer (Jungle)",
                [2] = "Multiplayer (Music)",
                [3] = "Multiplayer (Mountain)",
                [4] = "Multiplayer (Image)",
                [5] = "Multiplayer (Cave)",
                [6] = "Multiplayer (Cake)",
            },
        };

        /// <summary>
        /// The Rayman Designer map names
        /// </summary>
        private static Dictionary<int, Dictionary<int, string>> RaymanDesignerMapNames { get; } = new Dictionary<int, Dictionary<int, string>>()
        {
            [1] = new Dictionary<int, string>()
            {
                [1] = "The Sky's the Limit",
                [2] = "Fruity Fun",
                [3] = "Junglemania",
                [4] = "Ring a Ling",
            },
            [2] = new Dictionary<int, string>()
            {
                [1] = "Gone with the Wind",
                [2] = "Scale the Scales",
                [3] = "Music Lessons",
                [4] = "Melodic Maracas",
            },
            [3] = new Dictionary<int, string>()
            {
                [1] = "Treetop Adventure",
                [2] = "Tough Climb",
                [3] = "Tip-Top Tempest",
                [4] = "The Diabolical Pursuit",
            },
            [4] = new Dictionary<int, string>()
            {
                [1] = "The Five Doors",
                [2] = "Pencil Pentathalon",
                [3] = "Eraser Mania",
                [4] = "Tic Tack Toe",
            },
            [5] = new Dictionary<int, string>()
            {
                [1] = "Peaks and Rocks",
                [2] = "Dark Journey",
                [3] = "Dreaded Caves",
                [4] = "Dire Darkness",
            },
            [6] = new Dictionary<int, string>()
            {
                [1] = "Chocolate Trap",
                [2] = "Crazy Candy",
                [3] = "Bonbon-a-rama",
                [4] = "Whipped Cream Challenge",
            },
        };

        private static Dictionary<int, Dictionary<int, string>> Rayman3GBAMapNames { get; } = new Dictionary<int, Dictionary<int, string>>()
        {
            [0] = new Dictionary<int, string>()
            {
                [0] = "Wanderwood Forest 1",
                [1] = "Wanderwood Forest 2",
                [2] = "Shining Glade 1",
                [3] = "Shining Glade 2",
                [4] = "Swamp of Bégoniax",
                [5] = "Garish Gears",
                [6] = "Hoodlum Hideout 1",
                [7] = "Hoodlum Hideout 2",
            },
            [1] = new Dictionary<int, string>()
            {
                [8] = "Magma Mayhem",
                [9] = "Vertigo Wastes 1",
                [10] = "Vertigo Wastes 2",
                [11] = "Void of Bones 1",
                [12] = "Void of Bones 2",
                [13] = "Jano's Nest",
                [14] = "Prickly Passage 1",
                [15] = "Prickly Passage 2",
                [16] = "Swamp of Bégoniax 2",
            },
            [2] = new Dictionary<int, string>()
            {
                [17] = "River of Fire 1",
                [18] = "River of Fire 2",
                [19] = "River of Fire 3",
                [20] = "The Underlands 1",
                [21] = "The Underlands 2",
                [22] = "Boulder Brink 1",
                [23] = "Boulder Brink 2",
                [24] = "Den of Rocky",
                [25] = "Wretched Ruins 1",
                [26] = "Wretched Ruins 2 ",
                [27] = "Wicked Flow 1",
                [28] = "Wicked Flow 2",
                [29] = "Wicked Flow 3",
            },
            [3] = new Dictionary<int, string>()
            {
                [30] = "Creeping Chaos 1",
                [31] = "Creeping Chaos 2",
                [32] = "Scaleman's Keep",
                [33] = "The Mettleworks 1",
                [34] = "The Mettleworks 2",
                [35] = "Magma Mayhem 2",
                [36] = "Razor Slide 1",
                [37] = "Razor Slide 2",
                [38] = "Heart of the Ancients 1",
                [39] = "Heart of the Ancients 2",
            },
            [4] = new Dictionary<int, string>()
            {
                [40] = "Mega Havoc 1",
                [41] = "Mega Havoc 2",
                [42] = "Mega Havoc 3",
                [43] = "Mega Havoc 4",
                [44] = "Lum Challenge",
                [45] = "Ly's Punch Challenge 1",
                [46] = "Ly's Punch Challenge 2",
                [47] = "Ly's Punch Challenge 3",
            },
            [5] = new Dictionary<int, string>()
            {
                [48] = "Ly Power 1 (Wanderwood Forest)",
                [49] = "Ly Power 2 (Garish Gears)",
                [50] = "Ly Power 3 (Vertigo Wastes)",
                [51] = "Ly Power 4 (River of Fire)",
                [52] = "Ly Power 5 (Den of Rocky)",
                [53] = "Ly Power 6 (Scaleman's Keep)",
            },
            [6] = new Dictionary<int, string>()
            {
                [54] = "Forgotten Forests",
                [55] = "Haunted Dreams",
                [56] = "Magmacosm",
                [57] = "Pirate Stronghold",
                [58] = "World Map",
            },
            [7] = new Dictionary<int, string>()
            {
                [59] = "Multiplayer 1",
                [60] = "Multiplayer 2",
                [61] = "Multiplayer 3",
                [62] = "Multiplayer 4",
                [63] = "Multiplayer 5",
                [64] = "Multiplayer 6",
                [65] = "Multiplayer 7",
                [66] = "Multiplayer 8",
                [67] = "Multiplayer 9",
                [68] = "Multiplayer 10",
            },
            [8] = new Dictionary<int, string>()
            {
                [91] = "Menu",
                [117] = "Ubisoft Logo",
            },
        };

        private static Dictionary<int, Dictionary<int, string>> RavingRabbidsNames { get; } = new Dictionary<int, Dictionary<int, string>>() {
            [0] = new Dictionary<int, string>() {
                [2] = "Child's play",
                [3] = "The kids' hamlet",
                [4] = "Unused (Child)",
                [5] = "Toy box",
                [6] = "Celestial castle",
                [29] = "Shooting Range",
            },
            [1] = new Dictionary<int, string>() {
                [7] = "Dream forest",
                [8] = "The leafy valley",
                [9] = "Colonial jungle",
                [10] = "The lush mountaintops",
                [11] = "Hidden burrow",
            },
            [2] = new Dictionary<int, string>() {
                [12] = "Unused (Organic Cave)",
                [13] = "Gastric rivers",
                [14] = "Living cavern",
                [15] = "Swallowed treasures",
                [16] = "The sticky lair",
            },
            [3] = new Dictionary<int, string>() {
                [17] = "The desert of desserts",
                [18] = "Unused (Sweets)",
                [19] = "The sweet islands",
                [20] = "Not a piece of cake!",
                [21] = "Tart tunnels",
                [22] = "Ginger-bunny-bread",
                [31] = "Shooting Range",
            },
            [4] = new Dictionary<int, string>() {
                [0] = "Wailing jail",
                [1] = "Boss Prison",
                [23] = "Filthy corridors",
                [24] = "Agony jails",
                [25] = "Infernal escape",
                [26] = "Spikes and yikes!",
                [27] = "The rabbids' lair",
            },
            [5] = new Dictionary<int, string>() {
                [30] = "Title Screen",
            },
            [10] = new Dictionary<int, string>() {
                [0] = "Village 1",
                [1] = "Village 2",
                [2] = "Village 3",
            },
            [11] = new Dictionary<int, string>() {
                [0] = "The toy chase",
                [1] = "Stomach circuit",
                [2] = "Cake race",
            },
            [12] = new Dictionary<int, string>() {
                [0] = "Unused"
            },
            [13] = new Dictionary<int, string>() {
                [0] = "Main Menu",
                [1] = "Credits",
                [2] = "Options",
                [3] = "Game Over",
                [4] = "Continue?",
                [5] = "Health and Safety (English)",
                [6] = "Health and Safety (French)",
                [7] = "Health and Safety (German)",
                [8] = "Health and Safety (Italian)",
                [9] = "Health and Safety (Dutch)",
                [10] = "Health and Safety (Spanish)",
                [11] = "Language Selection",
                [12] = "Health and Safety",
            },
        };

        private static Dictionary<int, Dictionary<int, string>> HoodlumsRevengeNames { get; } = new Dictionary<int, Dictionary<int, string>>() {
            [0] = new Dictionary<int, string>() {
                [0] = "Fairy Council",
                [1] = "Clearleaf Forest",
                [2] = "Clearleaf Falls",
                [3] = "Infernal Machine",
                [4] = "Dungeon of Murk",
                [5] = "Bog of Murk",
                [6] = "Begoniax Bayou",
                [7] = "Rivers of Murk",
                [8] = "Hoodlum Moor",
                [9] = "Land of the Livid Dead",
                [10] = "Menhirs of Power",
                [11] = "Pit of Endless Fire",
                [12] = "Clouds of Peril",
                [13] = "Heart of the World",
                [14] = "Reflux's Lair",
                [15] = "Vertiginous Riddle",
                [16] = "Cloudy Cache",
                [17] = "Mélée Mayhem",
                [18] = "Scalding Cascade",
                [19] = "Sulphurous Sea",
            },
            [1] = new Dictionary<int, string>() {
                [0] = "World Map",
                [1] = "Main Menu",
                [2] = "Pause Screen",
                [3] = "Score Screen",
                [4] = "Splash Screens",
            },
        };

        private static Dictionary<int, Dictionary<int, string>> Spyro2Names { get; } = new Dictionary<int, Dictionary<int, string>>() {
            [0] = new Dictionary<int, string>() {
                [0] = "Sunny Plains - Hub",
                [1] = "Country Farms",
                [2] = "Country Farms - Side Area",
                [3] = "Shamrock Isle",
                [4] = "Shamrock Isle - Side Area 1",
                [5] = "Shamrock Isle - Side Area 2",
                [6] = "Temple of Dune",
                [7] = "Temple of Dune - Side Area 1",
                [8] = "Temple of Dune - Side Area 2",
                [9] = "Tiki Tropics",
                [10] = "Crush - Boss",
                
                [11] = "Celestial Plains - Hub",
                [12] = "Candy Lane",
                [13] = "Candy Lane - Side Area",
                [14] = "Haunted Hills",
                [15] = "Haunted Hills - Side Area 1",
                [16] = "Haunted Hills - Side Area 2",
                [17] = "Winter Mesa",
                [18] = "Winter Mesa - Side Area 1",
                [19] = "Winter Mesa - Side Area 2",
                [20] = "Dreamy Castle",
                [21] = "Dreamy Castle - Side Area 1",
                [22] = "Dreamy Castle - Side Area 2",
                [23] = "Gulp - Boss",

                [24] = "Starry Plains - Hub",
                [25] = "Moon Fondue",
                [26] = "Moon Fondue - Side Area 1",
                [27] = "Moon Fondue - Side Area 2",
                [28] = "Gypsy Road",
                [29] = "Gypsy Road - Side Area 1",
                [30] = "Gypsy Road - Side Area 2",
                [31] = "Watertopia",
                [32] = "Watertopia - Side Area 1",
                [33] = "Watertopia - Side Area 2",
                [34] = "Ripto's Volcano - Boss",

                [39] = "Canyon Hop - Sheila",
                [40] = "Arctic Outback - Sheila",
                [41] = "Lunar Ledges - Sheila",
                [42] = "Ripto's Mondo Volcano - Sheila",
            },
            [1] = new Dictionary<int, string>()
            {
                [0] = "Rumble Jungle - Agent 9",
                [1] = "Minty Mines - Agent 9",
                [2] = "Alpine Adobe - Agent 9",
                [3] = "Volcano Vaults - Agent 9",
            },
            [2] = new Dictionary<int, string>()
            {
                [0] = "Cutscene 0",
                [1] = "Cutscene 1",
                [2] = "Cutscene 2",
                [3] = "Cutscene 3",
                [4] = "Cutscene 4",
                [5] = "Cutscene 5",
                [6] = "Cutscene 6",
                [7] = "Cutscene 7",
                [8] = "Cutscene 8",
                [9] = "Cutscene 9",
                [10] = "Cutscene 10",
            }
        };

        private static Dictionary<int, Dictionary<int, string>> Spyro3Names { get; } = new Dictionary<int, Dictionary<int, string>>() {
            [0] = new Dictionary<int, string>() {
                [2] = "Virtual Playground - Virtual Training Area",
                [3] = "Professor's Secret Lab - Professor's Zoo",

                [4] = "Dragon Shores - Dragon Nests Region",
                [5] = "Dragon Shores - Moneybags' Vault",
                [6] = "Dragon Shores - Dragon Coast Region",
                [7] = "Dragon Shores - Dragon Coast Region",
                [8] = "Dragon Shores - Dragonfly Oasis Region",
                [9] = "Dragon Shores - Dragonfly Oasis Region",
                [10] = "Dragon Shores - Dragonfly Oasis Region",
                [11] = "Dragon Shores - Stormy Passage",
                [12] = "Dragon Shores - Underground Pass",
                [13] = "Dragon Shores - Virtual Hideaway",

                [14] = "Fairy Library - Foyer",
                [15] = "Fairy Library - Main Hall",
                [16] = "Fairy Library - XYZ Section",
                [17] = "Fairy Library - Virtual Hideaway",
                [18] = "Fairy Library - Moneybags' Vault",

                [19] = "Yeti Serengeti - Frozen Hills Region",
                [20] = "Yeti Serengeti - Frontier Region",
                [21] = "Yeti Serengeti - Frontier Region",
                [22] = "Yeti Serengeti - Frozen Hills Region",
                [23] = "Yeti Serengeti - Frontier Region",
                [24] = "Yeti Serengeti - Whistling Caves",
                [25] = "Yeti Serengeti - Whistling Caves",
                [26] = "Yeti Serengeti - Whistling Caves",
                [27] = "Yeti Serengeti - Whistling Caves",
                [28] = "Yeti Serengeti - Virtual Hideaway",
                [29] = "Yeti Serengeti - Moneybags' Vault",

                [30] = "Byrd Barracks - H.Q. Perimeter",
                [31] = "Byrd Barracks - Camp Headquarters",
                [32] = "Byrd Barracks - Virtual Hideaway",
                [33] = "Byrd Barracks - Moneybags' Vault",
                [34] = "Byrd Barracks - Camp Headquarters",

                [35] = "Thieves' Guild - Secret Cave",
                [36] = "Thieves' Guild - Main Guild Hall",
                [37] = "Thieves' Guild - Fast Eddie's Turf",
                [38] = "Thieves' Guild - Virtual Hideaway",
                [39] = "Thieves' Guild - Moneybags' Vault",
                [40] = "Thieves' Guild - Main Guild Hall",

                [41] = "Rabbit Habitat - Rabbit Hole",
                [42] = "Rabbit Habitat - Land of Wonder and Amazement",
                [43] = "Rabbit Habitat - Virtual Hideaway",
                [44] = "Rabbit Habitat - Moneybags' Vault",
                [45] = "Rabbit Habitat - Rhynoc Infestation",

                [46] = "Banana Savannah - Jungle's Edge",
                [47] = "Banana Savannah - Primate Plantation",
                [48] = "Banana Savannah - Primate Plantation",
                [49] = "Banana Savannah - Virtual Hideaway",
                [50] = "Banana Savannah - Moneybags' Vault",

                [51] = "Kangaroo Hoodoos - Outlook Region",
                [52] = "Kangaroo Hoodoos - The Outback",
                [53] = "Kangaroo Hoodoos - The Outback",
                [54] = "Kangaroo Hoodoos - Virtual Hideaway",
                [55] = "Kangaroo Hoodoos - The Outback",
                [56] = "Kangaroo Hoodoos - Moneybags' Vault",

                [57] = "Moneybags' Mansion - Foyer",
                [58] = "Moneybags' Mansion - Foyer",
                [59] = "Moneybags' Mansion - Hall of Majesty",
                [60] = "Moneybags' Mansion - Generator Room",
                [61] = "Moneybags' Mansion - Moneybags' Vault",
                [62] = "Moneybags' Mansion - Virtual Hideaway",

                [63] = "Cheetah Spot Spa - Lobby",
                [64] = "Cheetah Spot Spa - Fitness Center",
                [65] = "Cheetah Spot Spa - Moneybags' Vault",
                [66] = "Cheetah Spot Spa - Virtual Hideaway",
                [67] = "Cheetah Spot Spa - Fitness Center",

                [68] = "Professor's Secret Lab - Research Section",
                [69] = "Professor's Secret Lab - Moneybags' Vault",
                [70] = "Professor's Secret Lab - Prototyping Section",
                [71] = "Professor's Secret Lab - Virtual Hideaway",

                [72] = "Rhynocs n' Clocks - Rallying Point",
                [73] = "Rhynocs n' Clocks - Rhynoc Retreat",
                [74] = "Rhynocs n' Clocks - Rhynoc Retreat",
                [75] = "Rhynocs n' Clocks - Rhynoc Retreat",
                [76] = "Rhynocs n' Clocks - Moneybags' Vault",

                [77] = "Chateau Ripto - Ripto's Grand Expanse",
                [78] = "Chateau Ripto - Ripto's Main Gate",
                [79] = "Chateau Ripto - Ripto's Grand Expanse",
                [80] = "Chateau Ripto - Virtual Hideaway",
                [81] = "Chateau Ripto - Moneybags' Vault",
                [82] = "Chateau Ripto - Ripto's Throne Room",
            },
            [1] = new Dictionary<int, string>()
            {
                [83] = "Thieves' Guild - Stealth Mission",
                [84] = "Banana Savannah - Stealth Mission",
                [85] = "Moneybags' Mansion - Stealth Mission",
                [86] = "Cheetah Spot Spa - Stealth Mission",
            },
            [2] = new Dictionary<int, string>()
            {
                [87] = "Dragon Shores - Rescue Mission",
                [88] = "Byrd Barracks - Rescue Mission",
                [89] = "Rhynocs n' Clocks - Rescue Mission",
                [90] = "Chateau Ripto - Rescue Mission",
            },
            [3] = new Dictionary<int, string>()
            {
                [0] = "Byrd Rescue 1",
                [1] = "Byrd Rescue 2",
                [2] = "Byrd Rescue 3",
                [3] = "Byrd Rescue 4",
                [4] = "Byrd Rescue 5",
                [5] = "Byrd Rescue 6",
                [6] = "Byrd Rescue 7",
                [7] = "Byrd Rescue 8",
                [8] = "Byrd Rescue 9",

                // These are duplicates of the rescue missions, so we ignore them
                //[9] = "Glacial Grotto",
                //[10] = "Hazardous Hollow",
                //[11] = "Cuckoo Caverns",
                //[12] = "Ripto Razzle",
            }
        };

        private static Dictionary<int, Dictionary<int, string>> Rayman1GBCNames { get; } = new Dictionary<int, Dictionary<int, string>>() {
            [0] = new Dictionary<int, string>() {
                [0] = "Fiery Depths 2",
                [1] = "Fiery Depths 1",
                [2] = "Fiery Depths 3",
                [3] = "Fiery Depths 4",

                [4] = "Dark Legacy 2",
                [5] = "Dark Legacy 4",
                [6] = "Dark Legacy 1",
                [7] = "Dark Legacy 3",

                [8] = "Arcane Forest 5 - Final Boss",

                [9] = "Spellbound Forest 1",
                [10] = "Spellbound Forest 2",
                [11] = "Spellbound Forest 3",
                [12] = "Spellbound Forest 4",

                [13] = "Rainy Forest 1",
                [14] = "Rainy Forest 2",
                [15] = "Rainy Forest 3",

                [16] = "Ancient Forest 1",
                [17] = "Ancient Forest 2",
                [18] = "Ancient Forest 3",

                [19] = "Arcane Forest 1",
                [20] = "Arcane Forest 2",
                [21] = "Arcane Forest 3",
                [22] = "Arcane Forest 4",

                [23] = "Rocky Peaks 2",
                [24] = "Rocky Peaks 3",
                [25] = "Rocky Peaks 4",
                [26] = "Rocky Peaks 1",

                [27] = "Airy Tunes 1",
                [28] = "Airy Tunes 2",
                [29] = "Airy Tunes 3",
                [30] = "Airy Tunes 4",

                [31] = "World map",

                [32] = "Arcane Forest - Bonus",
                [33] = "Arcane Forest - Bonus",
                [34] = "Spellbound Forest - Bonus",
                [35] = "Ubi Cliff",
                [36] = "Ancient Forest - Bonus",
                [37] = "Airy Tunes - Bonus",
                [38] = "Airy Tunes - Bonus",
                [39] = "Fiery Depths - Bonus",
                [40] = "Fiery Depths - Bonus",
                [41] = "Spellbound Forest - Bonus",
                [42] = "Rainy Forest - Bonus",
                [43] = "Ancient Forest - Bonus",
                [44] = "Rocky Peaks - Bonus",
                [45] = "Rocky Peaks - Bonus",
                [46] = "Time attack map",
            },
        };

        private static Dictionary<int, Dictionary<int, string>> Rayman2GBCNames { get; } = new Dictionary<int, Dictionary<int, string>>() {
            [0] = new Dictionary<int, string>() {
                [0] = "Sanctuary of Rock & Lava - Bonus", // -1
                [1] = "Sanctuary of Rock & Lava 1", // 25
                [2] = "Sanctuary of Rock & Lava 2", // 26
                [3] = "Sanctuary of Rock & Lava 3", // 27

                [4] = "Tomb of the Ancients 4", // 31
                [5] = "Tomb of the Ancients 1", // 28
                [6] = "Tomb of the Ancients 2", // 29
                [7] = "Tomb of the Ancients 3", // 30
                [8] = "Tomb of the Ancients - Bonus", // -1

                [9] = "Fairy Glade 1", // 0
                [10] = "Fairy Glade 2", // 1
                [11] = "Fairy Glade 3", // 2
                [12] = "Fairy Glade - Bonus", // -1

                [13] = "Marshes of Awakening 1", // 3
                [14] = "Marshes of Awakening 2", // 4
                [15] = "Marshes of Awakening 3", // 5
                [16] = "Marshes of Awakening - Bonus", // -1

                [17] = "Whale Bay 1", // 6
                [18] = "Whale Bay 2", // 7
                [19] = "Whale Bay 3", // 8
                [20] = "Whale Bay - Bonus", // -1

                [21] = "Cave of Bad Dreams - Bonus", // -1
                [22] = "Cave of Bad Dreams 1", // 9
                [23] = "Cave of Bad Dreams 2", // 10
                [24] = "Cave of Bad Dreams 3", // 11

                [25] = "The Canopy 1", // 12
                [26] = "The Canopy 3", // 14
                [27] = "The Canopy - Bonus", // -1
                [28] = "The Canopy 2", // 13

                [29] = "Sanctuary of Stone & Fire 1", // 15
                [30] = "Sanctuary of Stone & Fire 2", // 16
                [31] = "Sanctuary of Stone & Fire 3", // 17
                [32] = "Sanctuary of Stone & Fire - Bonus", // -1

                [33] = "Echoing Caves - Bonus", // -1
                [34] = "Echoing Caves 1", // 18
                [35] = "Echoing Caves 2", // 19
                [36] = "Echoing Caves 3", // 20

                [37] = "The Precipice 1", // 21
                [38] = "The Precipice - Bonus", // -1
                [39] = "The Precipice 2", // 22
                [40] = "The Precipice 3", // 23
                [41] = "The Precipice 4", // 24

                [42] = "Time attack map", // 32
                [43] = "World map", // -1
            },
        };



        private static Dictionary<int, Dictionary<int, string>> RaymanKartNames { get; } = new Dictionary<int, Dictionary<int, string>>() {
            [0] = new Dictionary<int, string>() {
                [0] = "Dreamy Forest - Easy",
                [1] = "Dreamy Forest - Hard",
                [2] = "Rockslide Race - Easy",
                [3] = "Rockslide Race - Hard",
                [4] = "Sanctuary of Fire - Easy",
                [5] = "Sanctuary of Fire - Hard",
                [6] = "Frozen Highway - Easy",
                [7] = "Frozen Highway - Hard",
                [8] = "Shipwreck Track - Easy",
                [9] = "Shipwreck Track - Hard",
                [10] = "Murky Swamp - Easy",
                [11] = "Murky Swamp - Hard",
                [12] = "Moonlight Pass - Easy",
                [13] = "Moonlight Pass - Hard",
                [14] = "Rabbid Battlefield - Easy",
                [15] = "Rabbid Battlefield - Hard",
            },
        };

        private static Dictionary<int, Dictionary<int, string>> Crash1GBANames { get; } = new Dictionary<int, Dictionary<int, string>>() {
            [0] = GBAVV_Crash1_Manager.Levels.Select((x, i) => new {x, i}).ToDictionary(x => x.i, x => x.x.DisplayName)
        };
        private static Dictionary<int, Dictionary<int, string>> Crash2GBANames { get; } = new Dictionary<int, Dictionary<int, string>>() {
            [0] = GBAVV_Crash2_Manager.Levels.Select((x, i) => new {x, i}).ToDictionary(x => x.i, x => x.x.DisplayName)
        };
    }
}