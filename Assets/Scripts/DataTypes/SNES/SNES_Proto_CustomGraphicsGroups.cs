using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine {
	public static class SNES_Proto_CustomGraphicsGroups {
        public static SNES_Proto_ImageDescriptor[] Enemy_ImageDescriptors => new SNES_Proto_ImageDescriptor[] {
            new SNES_Proto_ImageDescriptor() { IsEmpty = true, TileIndex = 0x0 }, // NULL
            // Enemy (start index 1)
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE0 }, // Body 1
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE1 },
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE2 }, // Body 2
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE3 },
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF0 }, // Body 3
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF1 },
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF2 }, // Body 4
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF3 },
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE4 }, // Body 5 (vertical)
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF4 },
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE5 }, // Body 6 (vertical)
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF5 },
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE6 }, // Body surprised
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE7 },
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF6 }, // Stinger
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF7 }, // Back

            // Start index 17
        };

        public static R1Jaguar_AnimationDescriptor[] Enemy_Animations => new R1Jaguar_AnimationDescriptor[] {
            // Enemy normal
            new R1Jaguar_AnimationDescriptor() {
                LayersPerFrame = 3,
                FrameCount = 8, // 4 frames, pingponged
                Layers = new R1_AnimationLayer[][] {
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 11, YPosition = 4, ImageIndex = 2 }, // Back
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 1 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 2 },
                    },
                    new R1_AnimationLayer[] { // Frame 1
                        new R1_AnimationLayer() { XPosition = 10, YPosition = 3, ImageIndex = 2 },
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 3 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 4 },
                    },
                    new R1_AnimationLayer[] { // Frame 2
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 3, ImageIndex = 2 },
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 5 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 6 },
                    },
                    new R1_AnimationLayer[] { // Frame 3
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 3, ImageIndex = 2 },
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 7 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 8 },
                    },
                    new R1_AnimationLayer[] { // Frame 3
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 2 },
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 7 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 8 },
                    },
                    new R1_AnimationLayer[] { // Frame 2
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 4, ImageIndex = 2 },
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 3, ImageIndex = 5 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 3, ImageIndex = 6 },
                    },
                    new R1_AnimationLayer[] { // Frame 1
                        new R1_AnimationLayer() { XPosition = 10, YPosition = 4, ImageIndex = 2 },
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 3, ImageIndex = 3 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 3, ImageIndex = 4 },
                    },
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 11, YPosition = 4, ImageIndex = 2 },
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 3, ImageIndex = 1 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 3, ImageIndex = 2 },
                    },
                }.SelectMany(ls => ls).ToArray()
            },
            // Enemy surprised / stinger
            new R1Jaguar_AnimationDescriptor() {
                LayersPerFrame = 4,
                FrameCount = 9,
                Layers = new R1_AnimationLayer[][] {
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 1 }, // Body
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 2 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 0 }, // Back
                        new R1_AnimationLayer() { XPosition = 12, YPosition = 4, ImageIndex = 0 }, // Stinger
                    },
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 1 }, // Body
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 2 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 16 }, // Back
                        new R1_AnimationLayer() { XPosition = 12, YPosition = 4, ImageIndex = 0 }, // Stinger
                    },
                    new R1_AnimationLayer[] { // Frame 5
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 4, ImageIndex = 16 }, // Back
                        new R1_AnimationLayer() { XPosition = 14, YPosition = 4, ImageIndex = 0 }, // Stinger
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 5 }, // Body
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 6 },
                    },
                    new R1_AnimationLayer[] { // Frame 1
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 2, ImageIndex = 16 }, // Back
                        new R1_AnimationLayer() { XPosition = 14, YPosition = 2, ImageIndex = 15 }, // Stinger
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 1, ImageIndex = 13 }, // Body
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 1, ImageIndex = 14 },
                    },
                    new R1_AnimationLayer[] { // Frame 2
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 2, ImageIndex = 16 }, // Back
                        new R1_AnimationLayer() { XPosition = 15, YPosition = 2, ImageIndex = 15 }, // Stinger
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 2, ImageIndex = 13 }, // Body
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 2, ImageIndex = 14 },
                    },
                    new R1_AnimationLayer[] { // Frame 3
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 3, ImageIndex = 16 }, // Back
                        new R1_AnimationLayer() { XPosition = 15, YPosition = 3, ImageIndex = 15 }, // Stinger
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 13 }, // Body
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 14 },
                    },
                    new R1_AnimationLayer[] { // Frame 4
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 4, ImageIndex = 16 }, // Back
                        new R1_AnimationLayer() { XPosition = 14, YPosition = 4, ImageIndex = 15 }, // Stinger
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 13 }, // Body
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 14 },
                    },
                    new R1_AnimationLayer[] { // Frame 5
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 4, ImageIndex = 16 }, // Back
                        new R1_AnimationLayer() { XPosition = 14, YPosition = 3, ImageIndex = 0 }, // Stinger
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 5 }, // Body
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 6 },
                    },
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 1 }, // Body
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 2 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 0 }, // Back
                        new R1_AnimationLayer() { XPosition = 12, YPosition = 4, ImageIndex = 0 }, // Stinger
                    },
                }.SelectMany(ls => ls).ToArray()
            },
            // Enemy vertical
            new R1Jaguar_AnimationDescriptor() {
                LayersPerFrame = 3,
                FrameCount = 6,
                Layers = new R1_AnimationLayer[][] {
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 11, ImageIndex = 10 }, // Back
                        new R1_AnimationLayer() { XPosition = 1, YPosition = 0, ImageIndex = 9 }, // Face
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 8, ImageIndex = 10 }, // Body
                    },
                    new R1_AnimationLayer[] { // Frame 1
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 10, ImageIndex = 10 }, // Back
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 0, ImageIndex = 9 }, // Face
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 8, ImageIndex = 10 }, // Body
                    },
                    new R1_AnimationLayer[] { // Frame 2
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 9, ImageIndex = 10 }, // Back
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 0, ImageIndex = 9 }, // Face
                        new R1_AnimationLayer() { XPosition = 1, YPosition = 8, ImageIndex = 10 }, // Body
                    },
                    new R1_AnimationLayer[] { // Frame 3
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 8, ImageIndex = 10 }, // Back
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 0, ImageIndex = 9 }, // Face
                        new R1_AnimationLayer() { XPosition = 1, YPosition = 7, ImageIndex = 10 }, // Body
                    },
                    new R1_AnimationLayer[] { // Frame 4
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 9, ImageIndex = 10 }, // Back
                        new R1_AnimationLayer() { XPosition = 1, YPosition = 0, ImageIndex = 9 }, // Face
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 7, ImageIndex = 10 }, // Body
                    },
                    new R1_AnimationLayer[] { // Frame 5
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 10, ImageIndex = 10 }, // Back
                        new R1_AnimationLayer() { XPosition = 1, YPosition = 0, ImageIndex = 9 }, // Face
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 8, ImageIndex = 10 }, // Body
                    },
                }.SelectMany(ls => ls).ToArray()
            },
            // Enemy vertical slope
            new R1Jaguar_AnimationDescriptor() {
                LayersPerFrame = 2,
                FrameCount = 6,
                Layers = new R1_AnimationLayer[][] {
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 4, YPosition = 8, ImageIndex = 12 }, // Back
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 11 }, // Face
                    },
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 5, YPosition = 7, ImageIndex = 12 }, // Back
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 11 }, // Face
                    },
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 6, YPosition = 6, ImageIndex = 12 }, // Back
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 3, ImageIndex = 11 }, // Face
                    },
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 6, YPosition = 6, ImageIndex = 12 }, // Back
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 3, ImageIndex = 11 }, // Face
                    },
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 5, YPosition = 7, ImageIndex = 12 }, // Back
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 3, ImageIndex = 11 }, // Face
                    },
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 4, YPosition = 8, ImageIndex = 12 }, // Back
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 11 }, // Face
                    },
                }.SelectMany(ls => ls).ToArray()
            },

        };



        public static SNES_Proto_ImageDescriptor[] Orb_ImageDescriptors => new SNES_Proto_ImageDescriptor[] {
            new SNES_Proto_ImageDescriptor() { IsEmpty = true, TileIndex = 0x0 }, // NULL
            // Orb (start index 1)
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xA2, IsLarge = true }, // Orb 1
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xA4, IsLarge = true }, // Orb 2
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xA6, IsLarge = true }, // Orb 2

            // Sparkles (start index 4)
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xD6, }, // Small spark
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xC4, }, // Big spark
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xC4, FlipX = true }, // Big spark
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xC4, FlipY = true }, // Big spark
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xC4, FlipX = true, FlipY = true }, // Big spark

            // Orb collected animation (start index 9)
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAA, IsLarge = true }, // Disappearing 1
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xA8, IsLarge = true }, // Disappearing 2
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xD7, FlipX = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xC7, FlipX = true },
            
            // Orb collected effects (start index 13)
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xC5, },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xC5, FlipX = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xC5, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xC5, FlipX = true, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xC6, },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xC6, FlipX = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xC6, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xC6, FlipX = true, FlipY = true },
        };

        public static R1Jaguar_AnimationDescriptor[] Orb_Animations => new R1Jaguar_AnimationDescriptor[] {
            // Orb normal
            new R1Jaguar_AnimationDescriptor() {
                LayersPerFrame = 5,
                FrameCount = 24,
                Layers = new R1_AnimationLayer[][] {
                    new R1_AnimationLayer[] { // Spark hidden
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark rolling
                        new R1_AnimationLayer() { XPosition = 24-4, YPosition = 16-4, ImageIndex = 4 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark rolling
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer() { XPosition = 24-4, YPosition = 17-4, ImageIndex = 4 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark rolling
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer() { XPosition = 23-4, YPosition = 20-4, ImageIndex = 4 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark rolling
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer() { XPosition = 21-4, YPosition = 22-4, ImageIndex = 4 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark rolling
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer() { XPosition = 19-4, YPosition = 23-4, ImageIndex = 4 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark rolling
                        new R1_AnimationLayer() { XPosition = 17-4, YPosition = 23-4, ImageIndex = 4 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark rolling
                        new R1_AnimationLayer() { XPosition = 16-4, YPosition = 22-4, ImageIndex = 4 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark hidden
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark hidden
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark hidden
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark hidden
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark hidden
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Small spark
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 4 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Big spark
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 2 },
                        new R1_AnimationLayer() { XPosition = 4, YPosition = 4, ImageIndex = 5 },
                        new R1_AnimationLayer() { XPosition = 11, YPosition = 4, ImageIndex = 6 },
                        new R1_AnimationLayer() { XPosition = 4, YPosition = 11, ImageIndex = 7 },
                        new R1_AnimationLayer() { XPosition = 11, YPosition = 11, ImageIndex = 8 },
                    },
                    new R1_AnimationLayer[] { // Big spark
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 2 },
                        new R1_AnimationLayer() { XPosition = 4, YPosition = 4, ImageIndex = 5 },
                        new R1_AnimationLayer() { XPosition = 11, YPosition = 4, ImageIndex = 6 },
                        new R1_AnimationLayer() { XPosition = 4, YPosition = 11, ImageIndex = 7 },
                        new R1_AnimationLayer() { XPosition = 11, YPosition = 11, ImageIndex = 8 },
                    },
                    new R1_AnimationLayer[] { // Small spark
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 3 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 4 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark hidden
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark hidden
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark hidden
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark hidden
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark hidden
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark hidden
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Spark hidden
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 1 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                }.SelectMany(ls => ls).ToArray()
            },
            // Orb collected
            new R1Jaguar_AnimationDescriptor() {
                LayersPerFrame = 5,
                FrameCount = 10,
                Layers = new R1_AnimationLayer[][] {
                    new R1_AnimationLayer[] { // Frame 1: Disappearing
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 9 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 14 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 8, ImageIndex = 13 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 16, ImageIndex = 16 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 16, ImageIndex = 15 },
                    },
                    new R1_AnimationLayer[] { // Frame 1: Disappearing
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 7, ImageIndex = 9 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 8, ImageIndex = 18 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 8, ImageIndex = 17 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 16, ImageIndex = 20 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 16, ImageIndex = 19 },
                    },
                    new R1_AnimationLayer[] { // Frame 2: Disappearing
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 6, ImageIndex = 10 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Frame 2: Disappearing
                        new R1_AnimationLayer() { XPosition = 10, YPosition = 7, ImageIndex = 10 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Frame 2: Disappearing
                        new R1_AnimationLayer() { XPosition = 15, YPosition = 13, ImageIndex = 11 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Frame 2: Disappearing
                        new R1_AnimationLayer() { XPosition = 17, YPosition = 15, ImageIndex = 11 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Frame 2: Disappearing
                        new R1_AnimationLayer() { XPosition = 19, YPosition = 18, ImageIndex = 11 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Frame 2: Disappearing
                        new R1_AnimationLayer() { XPosition = 20, YPosition = 22, ImageIndex = 12 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Frame 2: Disappearing
                        new R1_AnimationLayer() { XPosition = 21, YPosition = 27, ImageIndex = 12 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                    new R1_AnimationLayer[] { // Frame 2: Disappearing
                        new R1_AnimationLayer() { XPosition = 21, YPosition = 31, ImageIndex = 12 },
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                        new R1_AnimationLayer(),
                    },
                }.SelectMany(ls => ls).ToArray()
            },
        };


        public static SNES_Proto_ImageDescriptor[] Effect_ImageDescriptors => new SNES_Proto_ImageDescriptor[] {
            new SNES_Proto_ImageDescriptor() { IsEmpty = true, TileIndex = 0x0 }, // NULL
            // PAF (start index 1)
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xC0, IsLarge = true }, // PAF Top 1
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xC2, IsLarge = true }, // PAF Top 2
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xD4 }, // PAF Bottom 1
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xD5 }, // PAF Bottom 2

            // Flash (start index 5)
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xA0, IsLarge = true }, // Huge spark
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xA0, IsLarge = true, FlipX = true }, // Huge spark
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xA0, IsLarge = true, FlipY = true }, // Huge spark
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xA0, IsLarge = true, FlipX = true, FlipY = true }, // Huge spark

            // Growing flash (start index 9)
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAF },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAF, FlipX = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAF, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAF, FlipX = true, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBF },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBF, FlipX = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBF, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBF, FlipX = true, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBE },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBE, FlipX = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBE, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBE, FlipX = true, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAE },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAE, FlipX = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAE, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAE, FlipX = true, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBC },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBC, FlipX = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBC, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBC, FlipX = true, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBD },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBD, FlipX = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBD, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xBD, FlipX = true, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAC },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAC, FlipX = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAC, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAC, FlipX = true, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAD },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAD, FlipX = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAD, FlipY = true },
            new SNES_Proto_ImageDescriptor() { Palette = 2, Priority = 2, TileIndex = 0xAD, FlipX = true, FlipY = true },
        };

        public static R1Jaguar_AnimationDescriptor[] Effect_Animations => new R1Jaguar_AnimationDescriptor[] {
            // Effect PAF
            new R1Jaguar_AnimationDescriptor() {
                LayersPerFrame = 4,
                FrameCount = 1,
                Layers = new R1_AnimationLayer[][] {
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 0, ImageIndex = 1 }, // Back
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 0, ImageIndex = 2 },
                        new R1_AnimationLayer() { XPosition = 10, YPosition = 16, ImageIndex = 3 },
                        new R1_AnimationLayer() { XPosition = 18, YPosition = 16, ImageIndex = 4 },
                    },
                }.SelectMany(ls => ls).ToArray()
            },

            // Effect large
            new R1Jaguar_AnimationDescriptor() {
                LayersPerFrame = 4,
                FrameCount = 1,
                Layers = new R1_AnimationLayer[][] {
                    new R1_AnimationLayer[] { // Frame 0: Huge explosion
                        new R1_AnimationLayer() { XPosition = 1, YPosition = 1, ImageIndex = 5 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 1, ImageIndex = 6 },
                        new R1_AnimationLayer() { XPosition = 1, YPosition = 16, ImageIndex = 7 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 16, ImageIndex = 8 },
                    },
                }.SelectMany(ls => ls).ToArray()
            },
            // Effect growing
            new R1Jaguar_AnimationDescriptor() {
                LayersPerFrame = 4,
                FrameCount = 8,
                Layers = new R1_AnimationLayer[][] {
                    new R1_AnimationLayer[] {
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 9, ImageIndex = 9 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 9, ImageIndex = 10 },
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 16, ImageIndex = 11 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 16, ImageIndex = 12 },
                    },
                    new R1_AnimationLayer[] {
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 9, ImageIndex = 13 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 9, ImageIndex = 14 },
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 16, ImageIndex = 15 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 16, ImageIndex = 16 },
                    },
                    new R1_AnimationLayer[] {
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 9, ImageIndex = 17 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 9, ImageIndex = 18 },
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 16, ImageIndex = 19 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 16, ImageIndex = 20 },
                    },
                    new R1_AnimationLayer[] {
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 9, ImageIndex = 21 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 9, ImageIndex = 22 },
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 16, ImageIndex = 23 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 16, ImageIndex = 24 },
                    },
                    new R1_AnimationLayer[] {
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 9, ImageIndex = 25 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 9, ImageIndex = 26 },
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 16, ImageIndex = 27 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 16, ImageIndex = 28 },
                    },
                    new R1_AnimationLayer[] {
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 9, ImageIndex = 29 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 9, ImageIndex = 30 },
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 16, ImageIndex = 31 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 16, ImageIndex = 32 },
                    },
                    new R1_AnimationLayer[] {
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 9, ImageIndex = 33 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 9, ImageIndex = 34 },
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 16, ImageIndex = 35 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 16, ImageIndex = 36 },
                    },
                    new R1_AnimationLayer[] {
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 9, ImageIndex = 37 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 9, ImageIndex = 38 },
                        new R1_AnimationLayer() { XPosition = 9, YPosition = 16, ImageIndex = 39 },
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 16, ImageIndex = 40 },
                    },
                }.SelectMany(ls => ls).ToArray()
            },

        };
    }
}
