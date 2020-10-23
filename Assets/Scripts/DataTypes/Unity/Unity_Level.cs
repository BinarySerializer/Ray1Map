using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Data for a loaded level
    /// </summary>
    public class Unity_Level
    {
        #region Constructor

        public Unity_Level(Unity_Map[] maps,
            Unity_ObjectManager objManager, 
            List<Unity_Object> eventData = null, 
            Unity_Object rayman = null, 
            IReadOnlyDictionary<string, string[]> localization = null, 
            int defaultMap = 0, int defaultCollisionMap = 0, 
            int pixelsPerUnit = 16, 
            int cellSize = 16, 
            Func<byte, Unity_MapCollisionTypeGraphic> getCollisionTypeGraphicFunc = null, 
            Texture2D background = null, 
            Texture2D parallaxBackground = null,
            Unity_Sector[] sectors = null)
        {
            Maps = maps;
            ObjManager = objManager;
            EventData = eventData ?? new List<Unity_Object>();
            Rayman = rayman;
            Localization = localization;
            DefaultMap = defaultMap;
            DefaultCollisionMap = defaultCollisionMap;
            PixelsPerUnit = pixelsPerUnit;
            CellSize = cellSize;
            GetCollisionTypeGraphicFunc = getCollisionTypeGraphicFunc ?? (x => ((R1_TileCollisionType)x).GetCollisionTypeGraphic());
            Background = background;
            ParallaxBackground = parallaxBackground;
            Sectors = sectors;
        }

        #endregion

        #region Public Properties

        public int PixelsPerUnit { get; }
        public int CellSize { get; }

        // TODO: Replace this with toggle in editor
        public int DefaultMap { get; }
        public int DefaultCollisionMap { get; }

        public Unity_Map[] Maps { get; }

        public List<Unity_Object> EventData { get; }
        public Unity_Object Rayman { get; }

        public IReadOnlyDictionary<string, string[]> Localization { get; }

        public Unity_ObjectManager ObjManager { get; }

        public Func<byte, Unity_MapCollisionTypeGraphic> GetCollisionTypeGraphicFunc { get; }

        public Texture2D Background { get; }
        public Texture2D ParallaxBackground { get; }

        public Unity_Sector[] Sectors { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Auto applies the palette to the tiles in the level
        /// </summary>
        public void AutoApplyPalette()
        {
            var r1Events = EventData.OfType<Unity_Object_R1>().ToArray();

            if (!r1Events.Any())
                return;

            // Get the palette changers
            var paletteXChangers = r1Events.Where(x => x.EventData.Type == R1_EventType.TYPE_PALETTE_SWAPPER && x.EventData.SubEtat < 6).ToDictionary(x => x.XPosition, x => (R1_PaletteChangerMode)x.EventData.SubEtat);
            var paletteYChangers = r1Events.Where(x => x.EventData.Type == R1_EventType.TYPE_PALETTE_SWAPPER && x.EventData.SubEtat >= 6).ToDictionary(x => x.YPosition, x => (R1_PaletteChangerMode)x.EventData.SubEtat);

            // NOTE: The auto system won't always work since it just checks one type of palette swapper and doesn't take into account that the palette swappers only trigger when on-screen, rather than based on the axis. Because of this some levels, like Music 5, won't work. More are messed up in the EDU games. There is sadly no solution to this since it depends on the players movement.
            // Check which type of palette changer we have
            bool isPaletteHorizontal = paletteXChangers.Any();

            // Keep track of the default palette
            int defaultPalette = 1;

            // Get the default palette
            if (isPaletteHorizontal && paletteXChangers.Any())
            {
                switch (paletteXChangers.OrderBy(x => x.Key).First().Value)
                {
                    case R1_PaletteChangerMode.Left1toRight2:
                    case R1_PaletteChangerMode.Left1toRight3:
                        defaultPalette = 1;
                        break;
                    case R1_PaletteChangerMode.Left2toRight1:
                    case R1_PaletteChangerMode.Left2toRight3:
                        defaultPalette = 2;
                        break;
                    case R1_PaletteChangerMode.Left3toRight1:
                    case R1_PaletteChangerMode.Left3toRight2:
                        defaultPalette = 3;
                        break;
                }
            }
            else if (!isPaletteHorizontal && paletteYChangers.Any())
            {
                switch (paletteYChangers.OrderByDescending(x => x.Key).First().Value)
                {
                    case R1_PaletteChangerMode.Top1tobottom2:
                    case R1_PaletteChangerMode.Top1tobottom3:
                        defaultPalette = 1;
                        break;
                    case R1_PaletteChangerMode.Top2tobottom1:
                    case R1_PaletteChangerMode.Top2tobottom3:
                        defaultPalette = 2;
                        break;
                    case R1_PaletteChangerMode.Top3tobottom1:
                    case R1_PaletteChangerMode.Top3tobottom2:
                        defaultPalette = 3;
                        break;
                }
            }

            // Keep track of the current palette
            int currentPalette = defaultPalette;

            // Enumerate each cell (PC only has 1 map per level)
            for (int cellY = 0; cellY < Maps[0].Height; cellY++)
            {
                // Reset the palette on each row if we have a horizontal changer
                if (isPaletteHorizontal)
                    currentPalette = defaultPalette;
                // Otherwise check the y position
                else
                {
                    // Check every pixel 16 steps forward
                    for (int y = 0; y < Settings.CellSize; y++)
                    {
                        // Attempt to find a matching palette changer on this pixel
                        var py = paletteYChangers.TryGetValue((short)(Settings.CellSize * cellY + y), out R1_PaletteChangerMode pm) ? (R1_PaletteChangerMode?)pm : null;

                        // If one was found, change the palette based on type
                        if (py != null)
                        {
                            switch (py)
                            {
                                case R1_PaletteChangerMode.Top2tobottom1:
                                case R1_PaletteChangerMode.Top3tobottom1:
                                    currentPalette = 1;
                                    break;
                                case R1_PaletteChangerMode.Top1tobottom2:
                                case R1_PaletteChangerMode.Top3tobottom2:
                                    currentPalette = 2;
                                    break;
                                case R1_PaletteChangerMode.Top1tobottom3:
                                case R1_PaletteChangerMode.Top2tobottom3:
                                    currentPalette = 3;
                                    break;
                            }
                        }
                    }
                }

                for (int cellX = 0; cellX < Maps[0].Width; cellX++)
                {
                    // Check the x position for palette changing
                    if (isPaletteHorizontal)
                    {
                        // Check every pixel 16 steps forward
                        for (int x = 0; x < Settings.CellSize; x++)
                        {
                            // Attempt to find a matching palette changer on this pixel
                            var px = paletteXChangers.TryGetValue((short)(Settings.CellSize * cellX + x), out R1_PaletteChangerMode pm) ? (R1_PaletteChangerMode?)pm : null;

                            // If one was found, change the palette based on type
                            if (px != null)
                            {
                                switch (px)
                                {
                                    case R1_PaletteChangerMode.Left3toRight1:
                                    case R1_PaletteChangerMode.Left2toRight1:
                                        currentPalette = 1;
                                        break;
                                    case R1_PaletteChangerMode.Left1toRight2:
                                    case R1_PaletteChangerMode.Left3toRight2:
                                        currentPalette = 2;
                                        break;
                                    case R1_PaletteChangerMode.Left1toRight3:
                                    case R1_PaletteChangerMode.Left2toRight3:
                                        currentPalette = 3;
                                        break;
                                }
                            }
                        }
                    }

                    // Set the common tile
                    Maps[0].MapTiles[cellY * Maps[0].Width + cellX].PaletteIndex = currentPalette;
                }
            }
        }

        #endregion
    }
}