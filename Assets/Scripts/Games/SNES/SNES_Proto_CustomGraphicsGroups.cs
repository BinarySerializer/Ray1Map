using BinarySerializer.Ray1;
using System.Linq;
using BinarySerializer.Ray1.Jaguar;

namespace Ray1Map.SNES
{
    public static class SNES_Proto_CustomGraphicsGroups {
        public static SNES_Sprite[] Enemy_ImageDescriptors => new SNES_Sprite[] {
            new SNES_Sprite() { IsEmpty = true, TileIndex = 0x0 }, // NULL
            // Enemy (start index 1)
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xE0 }, // Body 1
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xE1 },
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xE2 }, // Body 2
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xE3 },
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xF0 }, // Body 3
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xF1 },
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xF2 }, // Body 4
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xF3 },
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xE4 }, // Body 5 (vertical)
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xF4 },
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xE5 }, // Body 6 (vertical)
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xF5 },
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xE6 }, // Body surprised
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xE7 },
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xF6 }, // Stinger
            new SNES_Sprite() { Palette = 5, Priority = 2, TileIndex = 0xF7 }, // Back

            // Start index 17
        };

        public static Animation[] Enemy_Animations => new Animation[] {
            // Enemy normal
            new Animation() {
                LayersCount = 3,
                FramesCount = 8, // 4 frames, pingponged
                Layers = new AnimationLayer[][] {
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 11, YPosition = 4, SpriteIndex = 2 }, // Back
                        new AnimationLayer() { XPosition = 0, YPosition = 4, SpriteIndex = 1 },
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 2 },
                    },
                    new AnimationLayer[] { // Frame 1
                        new AnimationLayer() { XPosition = 10, YPosition = 3, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 0, YPosition = 4, SpriteIndex = 3 },
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 4 },
                    },
                    new AnimationLayer[] { // Frame 2
                        new AnimationLayer() { XPosition = 9, YPosition = 3, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 0, YPosition = 4, SpriteIndex = 5 },
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 6 },
                    },
                    new AnimationLayer[] { // Frame 3
                        new AnimationLayer() { XPosition = 8, YPosition = 3, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 0, YPosition = 4, SpriteIndex = 7 },
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 8 },
                    },
                    new AnimationLayer[] { // Frame 3
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 0, YPosition = 4, SpriteIndex = 7 },
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 8 },
                    },
                    new AnimationLayer[] { // Frame 2
                        new AnimationLayer() { XPosition = 9, YPosition = 4, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 0, YPosition = 3, SpriteIndex = 5 },
                        new AnimationLayer() { XPosition = 8, YPosition = 3, SpriteIndex = 6 },
                    },
                    new AnimationLayer[] { // Frame 1
                        new AnimationLayer() { XPosition = 10, YPosition = 4, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 0, YPosition = 3, SpriteIndex = 3 },
                        new AnimationLayer() { XPosition = 8, YPosition = 3, SpriteIndex = 4 },
                    },
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 11, YPosition = 4, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 0, YPosition = 3, SpriteIndex = 1 },
                        new AnimationLayer() { XPosition = 8, YPosition = 3, SpriteIndex = 2 },
                    },
                }.SelectMany(ls => ls).ToArray()
            },
            // Enemy surprised / stinger
            new Animation() {
                LayersCount = 4,
                FramesCount = 9,
                Layers = new AnimationLayer[][] {
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 0, YPosition = 4, SpriteIndex = 1 }, // Body
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 0 }, // Back
                        new AnimationLayer() { XPosition = 12, YPosition = 4, SpriteIndex = 0 }, // Stinger
                    },
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 0, YPosition = 4, SpriteIndex = 1 }, // Body
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 16 }, // Back
                        new AnimationLayer() { XPosition = 12, YPosition = 4, SpriteIndex = 0 }, // Stinger
                    },
                    new AnimationLayer[] { // Frame 5
                        new AnimationLayer() { XPosition = 9, YPosition = 4, SpriteIndex = 16 }, // Back
                        new AnimationLayer() { XPosition = 14, YPosition = 4, SpriteIndex = 0 }, // Stinger
                        new AnimationLayer() { XPosition = 0, YPosition = 4, SpriteIndex = 5 }, // Body
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 6 },
                    },
                    new AnimationLayer[] { // Frame 1
                        new AnimationLayer() { XPosition = 9, YPosition = 2, SpriteIndex = 16 }, // Back
                        new AnimationLayer() { XPosition = 14, YPosition = 2, SpriteIndex = 15 }, // Stinger
                        new AnimationLayer() { XPosition = 0, YPosition = 1, SpriteIndex = 13 }, // Body
                        new AnimationLayer() { XPosition = 8, YPosition = 1, SpriteIndex = 14 },
                    },
                    new AnimationLayer[] { // Frame 2
                        new AnimationLayer() { XPosition = 9, YPosition = 2, SpriteIndex = 16 }, // Back
                        new AnimationLayer() { XPosition = 15, YPosition = 2, SpriteIndex = 15 }, // Stinger
                        new AnimationLayer() { XPosition = 0, YPosition = 2, SpriteIndex = 13 }, // Body
                        new AnimationLayer() { XPosition = 8, YPosition = 2, SpriteIndex = 14 },
                    },
                    new AnimationLayer[] { // Frame 3
                        new AnimationLayer() { XPosition = 9, YPosition = 3, SpriteIndex = 16 }, // Back
                        new AnimationLayer() { XPosition = 15, YPosition = 3, SpriteIndex = 15 }, // Stinger
                        new AnimationLayer() { XPosition = 0, YPosition = 4, SpriteIndex = 13 }, // Body
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 14 },
                    },
                    new AnimationLayer[] { // Frame 4
                        new AnimationLayer() { XPosition = 9, YPosition = 4, SpriteIndex = 16 }, // Back
                        new AnimationLayer() { XPosition = 14, YPosition = 4, SpriteIndex = 15 }, // Stinger
                        new AnimationLayer() { XPosition = 0, YPosition = 4, SpriteIndex = 13 }, // Body
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 14 },
                    },
                    new AnimationLayer[] { // Frame 5
                        new AnimationLayer() { XPosition = 9, YPosition = 4, SpriteIndex = 16 }, // Back
                        new AnimationLayer() { XPosition = 14, YPosition = 3, SpriteIndex = 0 }, // Stinger
                        new AnimationLayer() { XPosition = 0, YPosition = 4, SpriteIndex = 5 }, // Body
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 6 },
                    },
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 0, YPosition = 4, SpriteIndex = 1 }, // Body
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 0 }, // Back
                        new AnimationLayer() { XPosition = 12, YPosition = 4, SpriteIndex = 0 }, // Stinger
                    },
                }.SelectMany(ls => ls).ToArray()
            },
            // Enemy vertical
            new Animation() {
                LayersCount = 3,
                FramesCount = 6,
                Layers = new AnimationLayer[][] {
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 0, YPosition = 11, SpriteIndex = 10 }, // Back
                        new AnimationLayer() { XPosition = 1, YPosition = 0, SpriteIndex = 9 }, // Face
                        new AnimationLayer() { XPosition = 0, YPosition = 8, SpriteIndex = 10 }, // Body
                    },
                    new AnimationLayer[] { // Frame 1
                        new AnimationLayer() { XPosition = 0, YPosition = 10, SpriteIndex = 10 }, // Back
                        new AnimationLayer() { XPosition = 0, YPosition = 0, SpriteIndex = 9 }, // Face
                        new AnimationLayer() { XPosition = 0, YPosition = 8, SpriteIndex = 10 }, // Body
                    },
                    new AnimationLayer[] { // Frame 2
                        new AnimationLayer() { XPosition = 0, YPosition = 9, SpriteIndex = 10 }, // Back
                        new AnimationLayer() { XPosition = 0, YPosition = 0, SpriteIndex = 9 }, // Face
                        new AnimationLayer() { XPosition = 1, YPosition = 8, SpriteIndex = 10 }, // Body
                    },
                    new AnimationLayer[] { // Frame 3
                        new AnimationLayer() { XPosition = 0, YPosition = 8, SpriteIndex = 10 }, // Back
                        new AnimationLayer() { XPosition = 0, YPosition = 0, SpriteIndex = 9 }, // Face
                        new AnimationLayer() { XPosition = 1, YPosition = 7, SpriteIndex = 10 }, // Body
                    },
                    new AnimationLayer[] { // Frame 4
                        new AnimationLayer() { XPosition = 0, YPosition = 9, SpriteIndex = 10 }, // Back
                        new AnimationLayer() { XPosition = 1, YPosition = 0, SpriteIndex = 9 }, // Face
                        new AnimationLayer() { XPosition = 0, YPosition = 7, SpriteIndex = 10 }, // Body
                    },
                    new AnimationLayer[] { // Frame 5
                        new AnimationLayer() { XPosition = 0, YPosition = 10, SpriteIndex = 10 }, // Back
                        new AnimationLayer() { XPosition = 1, YPosition = 0, SpriteIndex = 9 }, // Face
                        new AnimationLayer() { XPosition = 0, YPosition = 8, SpriteIndex = 10 }, // Body
                    },
                }.SelectMany(ls => ls).ToArray()
            },
            // Enemy vertical slope
            new Animation() {
                LayersCount = 2,
                FramesCount = 6,
                Layers = new AnimationLayer[][] {
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 4, YPosition = 8, SpriteIndex = 12 }, // Back
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 11 }, // Face
                    },
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 5, YPosition = 7, SpriteIndex = 12 }, // Back
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 11 }, // Face
                    },
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 6, YPosition = 6, SpriteIndex = 12 }, // Back
                        new AnimationLayer() { XPosition = 9, YPosition = 3, SpriteIndex = 11 }, // Face
                    },
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 6, YPosition = 6, SpriteIndex = 12 }, // Back
                        new AnimationLayer() { XPosition = 9, YPosition = 3, SpriteIndex = 11 }, // Face
                    },
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 5, YPosition = 7, SpriteIndex = 12 }, // Back
                        new AnimationLayer() { XPosition = 9, YPosition = 3, SpriteIndex = 11 }, // Face
                    },
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 4, YPosition = 8, SpriteIndex = 12 }, // Back
                        new AnimationLayer() { XPosition = 8, YPosition = 4, SpriteIndex = 11 }, // Face
                    },
                }.SelectMany(ls => ls).ToArray()
            },

        };



        public static SNES_Sprite[] Orb_ImageDescriptors => new SNES_Sprite[] {
            new SNES_Sprite() { IsEmpty = true, TileIndex = 0x0 }, // NULL
            // Orb (start index 1)
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xA2, IsLarge = true }, // Orb 1
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xA4, IsLarge = true }, // Orb 2
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xA6, IsLarge = true }, // Orb 2

            // Sparkles (start index 4)
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xD6, }, // Small spark
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xC4, }, // Big spark
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xC4, FlipX = true }, // Big spark
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xC4, FlipY = true }, // Big spark
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xC4, FlipX = true, FlipY = true }, // Big spark

            // Orb collected animation (start index 9)
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAA, IsLarge = true }, // Disappearing 1
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xA8, IsLarge = true }, // Disappearing 2
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xD7, FlipX = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xC7, FlipX = true },
            
            // Orb collected effects (start index 13)
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xC5, },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xC5, FlipX = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xC5, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xC5, FlipX = true, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xC6, },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xC6, FlipX = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xC6, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xC6, FlipX = true, FlipY = true },
        };

        public static Animation[] Orb_Animations => new Animation[] {
            // Orb normal
            new Animation() {
                LayersCount = 5,
                FramesCount = 24,
                Layers = new AnimationLayer[][] {
                    new AnimationLayer[] { // Spark hidden
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark rolling
                        new AnimationLayer() { XPosition = 24-4, YPosition = 16-4, SpriteIndex = 4 },
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark rolling
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer() { XPosition = 24-4, YPosition = 17-4, SpriteIndex = 4 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark rolling
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer() { XPosition = 23-4, YPosition = 20-4, SpriteIndex = 4 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark rolling
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer() { XPosition = 21-4, YPosition = 22-4, SpriteIndex = 4 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark rolling
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer() { XPosition = 19-4, YPosition = 23-4, SpriteIndex = 4 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark rolling
                        new AnimationLayer() { XPosition = 17-4, YPosition = 23-4, SpriteIndex = 4 },
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark rolling
                        new AnimationLayer() { XPosition = 16-4, YPosition = 22-4, SpriteIndex = 4 },
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark hidden
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark hidden
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark hidden
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark hidden
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark hidden
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Small spark
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 4 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Big spark
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 4, YPosition = 4, SpriteIndex = 5 },
                        new AnimationLayer() { XPosition = 11, YPosition = 4, SpriteIndex = 6 },
                        new AnimationLayer() { XPosition = 4, YPosition = 11, SpriteIndex = 7 },
                        new AnimationLayer() { XPosition = 11, YPosition = 11, SpriteIndex = 8 },
                    },
                    new AnimationLayer[] { // Big spark
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 4, YPosition = 4, SpriteIndex = 5 },
                        new AnimationLayer() { XPosition = 11, YPosition = 4, SpriteIndex = 6 },
                        new AnimationLayer() { XPosition = 4, YPosition = 11, SpriteIndex = 7 },
                        new AnimationLayer() { XPosition = 11, YPosition = 11, SpriteIndex = 8 },
                    },
                    new AnimationLayer[] { // Small spark
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 3 },
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 4 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark hidden
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark hidden
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark hidden
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark hidden
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark hidden
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark hidden
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Spark hidden
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 1 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                }.SelectMany(ls => ls).ToArray()
            },
            // Orb collected
            new Animation() {
                LayersCount = 5,
                FramesCount = 10,
                Layers = new AnimationLayer[][] {
                    new AnimationLayer[] { // Frame 1: Disappearing
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 9 },
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 14 },
                        new AnimationLayer() { XPosition = 16, YPosition = 8, SpriteIndex = 13 },
                        new AnimationLayer() { XPosition = 8, YPosition = 16, SpriteIndex = 16 },
                        new AnimationLayer() { XPosition = 16, YPosition = 16, SpriteIndex = 15 },
                    },
                    new AnimationLayer[] { // Frame 1: Disappearing
                        new AnimationLayer() { XPosition = 8, YPosition = 7, SpriteIndex = 9 },
                        new AnimationLayer() { XPosition = 8, YPosition = 8, SpriteIndex = 18 },
                        new AnimationLayer() { XPosition = 16, YPosition = 8, SpriteIndex = 17 },
                        new AnimationLayer() { XPosition = 8, YPosition = 16, SpriteIndex = 20 },
                        new AnimationLayer() { XPosition = 16, YPosition = 16, SpriteIndex = 19 },
                    },
                    new AnimationLayer[] { // Frame 2: Disappearing
                        new AnimationLayer() { XPosition = 9, YPosition = 6, SpriteIndex = 10 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Frame 2: Disappearing
                        new AnimationLayer() { XPosition = 10, YPosition = 7, SpriteIndex = 10 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Frame 2: Disappearing
                        new AnimationLayer() { XPosition = 15, YPosition = 13, SpriteIndex = 11 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Frame 2: Disappearing
                        new AnimationLayer() { XPosition = 17, YPosition = 15, SpriteIndex = 11 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Frame 2: Disappearing
                        new AnimationLayer() { XPosition = 19, YPosition = 18, SpriteIndex = 11 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Frame 2: Disappearing
                        new AnimationLayer() { XPosition = 20, YPosition = 22, SpriteIndex = 12 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Frame 2: Disappearing
                        new AnimationLayer() { XPosition = 21, YPosition = 27, SpriteIndex = 12 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Frame 2: Disappearing
                        new AnimationLayer() { XPosition = 21, YPosition = 31, SpriteIndex = 12 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                }.SelectMany(ls => ls).ToArray()
            },
        };


        public static SNES_Sprite[] Effect_ImageDescriptors => new SNES_Sprite[] {
            new SNES_Sprite() { IsEmpty = true, TileIndex = 0x0 }, // NULL
            // PAF (start index 1)
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xC0, IsLarge = true }, // PAF Top 1
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xC2, IsLarge = true }, // PAF Top 2
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xD4 }, // PAF Bottom 1
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xD5 }, // PAF Bottom 2

            // Flash (start index 5)
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xA0, IsLarge = true }, // Huge spark
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xA0, IsLarge = true, FlipX = true }, // Huge spark
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xA0, IsLarge = true, FlipY = true }, // Huge spark
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xA0, IsLarge = true, FlipX = true, FlipY = true }, // Huge spark

            // Growing flash (start index 9)
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAF },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAF, FlipX = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAF, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAF, FlipX = true, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBF },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBF, FlipX = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBF, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBF, FlipX = true, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBE },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBE, FlipX = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBE, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBE, FlipX = true, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAE },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAE, FlipX = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAE, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAE, FlipX = true, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBC },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBC, FlipX = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBC, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBC, FlipX = true, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBD },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBD, FlipX = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBD, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xBD, FlipX = true, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAC },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAC, FlipX = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAC, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAC, FlipX = true, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAD },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAD, FlipX = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAD, FlipY = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0xAD, FlipX = true, FlipY = true },

            // Stars (Start index 41)
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0x8A, IsLarge = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0x8C, IsLarge = true },
            new SNES_Sprite() { Palette = 2, Priority = 2, TileIndex = 0x8E, IsLarge = true },

        };

        public static Animation[] Effect_Animations => new Animation[] {
            // Effect PAF
            new Animation() {
                LayersCount = 5,
                FramesCount = 10,
                Layers = new AnimationLayer[][] {
                    new AnimationLayer[] {
                        new AnimationLayer() { XPosition = 14, YPosition = 8, SpriteIndex = 43 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] {
                        new AnimationLayer() { XPosition = 14, YPosition = 8, SpriteIndex = 42 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 14, YPosition = 4, SpriteIndex = 41 },
                        new AnimationLayer() { XPosition = 4, YPosition = 8, SpriteIndex = 1 }, // PAF
                        new AnimationLayer() { XPosition = 20, YPosition = 8, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 14, YPosition = 24, SpriteIndex = 3 },
                        new AnimationLayer() { XPosition = 22, YPosition = 24, SpriteIndex = 4 },
                    },
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 18, YPosition = 1, SpriteIndex = 41 },
                        new AnimationLayer() { XPosition = 1, YPosition = 11, SpriteIndex = 1 }, // PAF
                        new AnimationLayer() { XPosition = 17, YPosition = 11, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 11, YPosition = 27, SpriteIndex = 3 },
                        new AnimationLayer() { XPosition = 19, YPosition = 27, SpriteIndex = 4 },
                    },
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 19, YPosition = 0, SpriteIndex = 41 },
                        new AnimationLayer() { XPosition = 0, YPosition = 12, SpriteIndex = 1 }, // PAF
                        new AnimationLayer() { XPosition = 16, YPosition = 12, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 10, YPosition = 28, SpriteIndex = 3 },
                        new AnimationLayer() { XPosition = 18, YPosition = 28, SpriteIndex = 4 },
                    },
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 19, YPosition = 0, SpriteIndex = 41 },
                        new AnimationLayer() { XPosition = 0, YPosition = 12, SpriteIndex = 1 }, // PAF
                        new AnimationLayer() { XPosition = 16, YPosition = 12, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 10, YPosition = 28, SpriteIndex = 3 },
                        new AnimationLayer() { XPosition = 18, YPosition = 28, SpriteIndex = 4 },
                    },
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 18, YPosition = 1, SpriteIndex = 41 },
                        new AnimationLayer() { XPosition = 1, YPosition = 11, SpriteIndex = 1 }, // PAF
                        new AnimationLayer() { XPosition = 17, YPosition = 11, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 11, YPosition = 27, SpriteIndex = 3 },
                        new AnimationLayer() { XPosition = 19, YPosition = 27, SpriteIndex = 4 },
                    },
                    new AnimationLayer[] { // Frame 0
                        new AnimationLayer() { XPosition = 14, YPosition = 4, SpriteIndex = 41 },
                        new AnimationLayer() { XPosition = 4, YPosition = 8, SpriteIndex = 1 }, // PAF
                        new AnimationLayer() { XPosition = 20, YPosition = 8, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 14, YPosition = 24, SpriteIndex = 3 },
                        new AnimationLayer() { XPosition = 22, YPosition = 24, SpriteIndex = 4 },
                    },
                    new AnimationLayer[] {
                        new AnimationLayer() { XPosition = 14, YPosition = 8, SpriteIndex = 42 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                    new AnimationLayer[] {
                        new AnimationLayer() { XPosition = 14, YPosition = 8, SpriteIndex = 43 },
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                        new AnimationLayer(),
                    },
                }.SelectMany(ls => ls).ToArray()
            },

            // Effect large
            new Animation() {
                LayersCount = 4,
                FramesCount = 1,
                Layers = new AnimationLayer[][] {
                    new AnimationLayer[] { // Frame 0: Huge explosion
                        new AnimationLayer() { XPosition = 1, YPosition = 1, SpriteIndex = 5 },
                        new AnimationLayer() { XPosition = 16, YPosition = 1, SpriteIndex = 6 },
                        new AnimationLayer() { XPosition = 1, YPosition = 16, SpriteIndex = 7 },
                        new AnimationLayer() { XPosition = 16, YPosition = 16, SpriteIndex = 8 },
                    },
                }.SelectMany(ls => ls).ToArray()
            },
            // Effect growing
            new Animation() {
                LayersCount = 4,
                FramesCount = 8,
                Layers = new AnimationLayer[][] {
                    new AnimationLayer[] {
                        new AnimationLayer() { XPosition = 9, YPosition = 9, SpriteIndex = 9 },
                        new AnimationLayer() { XPosition = 16, YPosition = 9, SpriteIndex = 10 },
                        new AnimationLayer() { XPosition = 9, YPosition = 16, SpriteIndex = 11 },
                        new AnimationLayer() { XPosition = 16, YPosition = 16, SpriteIndex = 12 },
                    },
                    new AnimationLayer[] {
                        new AnimationLayer() { XPosition = 9, YPosition = 9, SpriteIndex = 13 },
                        new AnimationLayer() { XPosition = 16, YPosition = 9, SpriteIndex = 14 },
                        new AnimationLayer() { XPosition = 9, YPosition = 16, SpriteIndex = 15 },
                        new AnimationLayer() { XPosition = 16, YPosition = 16, SpriteIndex = 16 },
                    },
                    new AnimationLayer[] {
                        new AnimationLayer() { XPosition = 9, YPosition = 9, SpriteIndex = 17 },
                        new AnimationLayer() { XPosition = 16, YPosition = 9, SpriteIndex = 18 },
                        new AnimationLayer() { XPosition = 9, YPosition = 16, SpriteIndex = 19 },
                        new AnimationLayer() { XPosition = 16, YPosition = 16, SpriteIndex = 20 },
                    },
                    new AnimationLayer[] {
                        new AnimationLayer() { XPosition = 9, YPosition = 9, SpriteIndex = 21 },
                        new AnimationLayer() { XPosition = 16, YPosition = 9, SpriteIndex = 22 },
                        new AnimationLayer() { XPosition = 9, YPosition = 16, SpriteIndex = 23 },
                        new AnimationLayer() { XPosition = 16, YPosition = 16, SpriteIndex = 24 },
                    },
                    new AnimationLayer[] {
                        new AnimationLayer() { XPosition = 9, YPosition = 9, SpriteIndex = 25 },
                        new AnimationLayer() { XPosition = 16, YPosition = 9, SpriteIndex = 26 },
                        new AnimationLayer() { XPosition = 9, YPosition = 16, SpriteIndex = 27 },
                        new AnimationLayer() { XPosition = 16, YPosition = 16, SpriteIndex = 28 },
                    },
                    new AnimationLayer[] {
                        new AnimationLayer() { XPosition = 9, YPosition = 9, SpriteIndex = 29 },
                        new AnimationLayer() { XPosition = 16, YPosition = 9, SpriteIndex = 30 },
                        new AnimationLayer() { XPosition = 9, YPosition = 16, SpriteIndex = 31 },
                        new AnimationLayer() { XPosition = 16, YPosition = 16, SpriteIndex = 32 },
                    },
                    new AnimationLayer[] {
                        new AnimationLayer() { XPosition = 9, YPosition = 9, SpriteIndex = 33 },
                        new AnimationLayer() { XPosition = 16, YPosition = 9, SpriteIndex = 34 },
                        new AnimationLayer() { XPosition = 9, YPosition = 16, SpriteIndex = 35 },
                        new AnimationLayer() { XPosition = 16, YPosition = 16, SpriteIndex = 36 },
                    },
                    new AnimationLayer[] {
                        new AnimationLayer() { XPosition = 9, YPosition = 9, SpriteIndex = 37 },
                        new AnimationLayer() { XPosition = 16, YPosition = 9, SpriteIndex = 38 },
                        new AnimationLayer() { XPosition = 9, YPosition = 16, SpriteIndex = 39 },
                        new AnimationLayer() { XPosition = 16, YPosition = 16, SpriteIndex = 40 },
                    },
                }.SelectMany(ls => ls).ToArray()
            },

        };



        public static SNES_Sprite[] Fist_ImageDescriptors => new SNES_Sprite[] {
            new SNES_Sprite() { IsEmpty = true, TileIndex = 0x0 }, // NULL
            // Fist Normal (start index 1)
            new SNES_Sprite() { Palette = 0, Priority = 2, TileIndex = 0xC8, IsLarge = true },
            new SNES_Sprite() { Palette = 0, Priority = 2, TileIndex = 0xCA, IsLarge = true },
            new SNES_Sprite() { Palette = 0, Priority = 2, TileIndex = 0xCC, IsLarge = true },
            new SNES_Sprite() { Palette = 0, Priority = 2, TileIndex = 0xCE, IsLarge = true },
            // Fist Gold (start index 5)
            new SNES_Sprite() { Palette = 1, Priority = 2, TileIndex = 0xC8, IsLarge = true },
            new SNES_Sprite() { Palette = 1, Priority = 2, TileIndex = 0xCA, IsLarge = true },
            new SNES_Sprite() { Palette = 1, Priority = 2, TileIndex = 0xCC, IsLarge = true },
            new SNES_Sprite() { Palette = 1, Priority = 2, TileIndex = 0xCE, IsLarge = true },

        };

        public static Animation[] Fist_Animations => new Animation[] {
            // Fist normal
            new Animation() {
                LayersCount = 4,
                FramesCount = 1,
                Layers = new AnimationLayer[][] {
                    new AnimationLayer[] {
                        new AnimationLayer() { XPosition = 0, YPosition = 0, SpriteIndex = 1 },
                        new AnimationLayer() { XPosition = 16, YPosition = 0, SpriteIndex = 2 },
                        new AnimationLayer() { XPosition = 0, YPosition = 16, SpriteIndex = 3 },
                        new AnimationLayer() { XPosition = 16, YPosition = 16, SpriteIndex = 4 },
                    },
                }.SelectMany(ls => ls).ToArray()
            },
            // Fist gold
            new Animation() {
                LayersCount = 4,
                FramesCount = 1,
                Layers = new AnimationLayer[][] {
                    new AnimationLayer[] {
                        new AnimationLayer() { XPosition = 0, YPosition = 0, SpriteIndex = 5 },
                        new AnimationLayer() { XPosition = 16, YPosition = 0, SpriteIndex = 6 },
                        new AnimationLayer() { XPosition = 0, YPosition = 16, SpriteIndex = 7 },
                        new AnimationLayer() { XPosition = 16, YPosition = 16, SpriteIndex = 8 },
                    },
                }.SelectMany(ls => ls).ToArray()
            },

        };
    }
}
