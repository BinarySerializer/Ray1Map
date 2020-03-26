using System.Collections.Generic;

namespace R1Engine
{
    /// <summary>
    /// The map names
    /// </summary>
    public static class MapNames
    {
        /// <summary>
        /// The Rayman 1 map names
        /// </summary>
        public static Dictionary<World, Dictionary<int, string>> Ray1MapNames { get; } = new Dictionary<World, Dictionary<int, string>>()
        {
            [World.Jungle] = new Dictionary<int, string>()
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
            [World.Music] = new Dictionary<int, string>()
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
            [World.Mountain] = new Dictionary<int, string>()
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
            [World.Image] = new Dictionary<int, string>()
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
            [World.Cave] = new Dictionary<int, string>()
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
            [World.Cake] = new Dictionary<int, string>()
            {
                [1] = "Mr Dark's Dare 1",
                [2] = "Mr Dark's Dare 2",
                [3] = "Mr Dark's Dare 3",
                [4] = "Mr Dark's Dare 4 - Boss",
            },
        };

        /// <summary>
        /// The Rayman Designer map names
        /// </summary>
        public static Dictionary<World, Dictionary<int, string>> RayKitMapNames { get; } = new Dictionary<World, Dictionary<int, string>>()
        {
            [World.Jungle] = new Dictionary<int, string>()
            {
                [1] = "The Sky's the Limit",
                [2] = "Fruity Fun",
                [3] = "Junglemania",
                [4] = "Ring a Ling",
            },
            [World.Music] = new Dictionary<int, string>()
            {
                [1] = "Gone with the Wind",
                [2] = "Scale the Scales",
                [3] = "Music Lessons",
                [4] = "Melodic Maracas",
            },
            [World.Mountain] = new Dictionary<int, string>()
            {
                [1] = "Treetop Adventure",
                [2] = "Tough Climb",
                [3] = "Tip-Top Tempest",
                [4] = "The Diabolical Pursuit",
            },
            [World.Image] = new Dictionary<int, string>()
            {
                [1] = "The Five Doors",
                [2] = "Pencil Pentathalon",
                [3] = "Eraser Mania",
                [4] = "Tic Tack Toe",
            },
            [World.Cave] = new Dictionary<int, string>()
            {
                [1] = "Peaks and Rocks",
                [2] = "Dark Journey",
                [3] = "Dreaded Caves",
                [4] = "Dire Darkness",
            },
            [World.Cake] = new Dictionary<int, string>()
            {
                [1] = "Chocolate Trap",
                [2] = "Crazy Candy",
                [3] = "Bonbon-a-rama",
                [4] = "Whipped Cream Challenge",
            },
        };
    }
}