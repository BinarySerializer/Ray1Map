using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.PS1;
using BinarySerializer.Ray1;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Data for a loaded level
    /// </summary>
    public class Unity_Level
    {
        #region Public Properties

        // General
        public int PixelsPerUnit { get; set; } = 16;
        public int CellSize { get; set; } = 16;
        public int? CellSizeOverrideCollision { get; set; }
        public float FramesPerSecond { get; set; } = 60;

        // TODO: Replace this with toggle in editor
        public int DefaultLayer { get; set; } = 0;
        public int DefaultCollisionLayer { get; set; } = -1;

        // Layers
        public Unity_Layer[] Layers { get; set; }
        public Unity_Map[] Maps { get; set; }
        public Unity_Map GridMap { get; set; }
        public float MinX { get; set; }
        public float MinY { get; set; }
        public float MaxX { get; set; }
        public float MaxY { get; set; }

        // Collision
        public Unity_CollisionLine[] CollisionLines { get; set; }
        public Func<ushort, string> GetCollisionTypeNameFunc { get; set; }
        public Func<ushort, Unity_MapCollisionTypeGraphic> GetCollisionTypeGraphicFunc { get; set; }

        // Backgrounds
        public Texture2D Background { get; set; }
        public Texture2D ParallaxBackground { get; set; }

        // Objects
        public Unity_ObjectManager ObjManager { get; set; }
        public string[] ObjectGroups { get; set; }
        public List<Unity_SpriteObject> EventData { get; set; }
        public Unity_SpriteObject Rayman { get; set; }

        // Additional data
        public Unity_IsometricData IsometricData { get; set; }
        public IList<KeyValuePair<string, string[]>> Localization { get; set; }
        public Unity_CameraClear CameraClear { get; set; }
        public Unity_Sector[] Sectors { get; set; }
        public PS1_VRAM PS1_VRAM { get; set; }

        // Track
        public Unity_TrackManager[] TrackManagers { get; set; }
        public string[] TrackNames { get; protected set; }
        public bool CanMoveAlongTrack { get; set; }
        public int SelectedTrack { get; set; }
        public Unity_TrackManager SelectedTrackManager => TrackManagers?.ElementAtOrDefault(SelectedTrack);

        #endregion

        #region Public Methods

        public Unity_Level Init()
        {
            EventData ??= new List<Unity_SpriteObject>();
            GetCollisionTypeNameFunc ??= x => ((TileCollisionType)x).ToString();
            GetCollisionTypeGraphicFunc ??= x => ((TileCollisionType)x).GetCollisionTypeGraphic();

            CameraClear?.Apply();

            // Set default layers
            if (Layers != null)
            {
                if (DefaultCollisionLayer == -1)
                    DefaultCollisionLayer = DefaultLayer;
            }
            else
            {
                InitializeDefaultLayers();
                DefaultLayer = Layers.FindItemIndex(l => (l as Unity_Layer_Map)?.MapIndex == DefaultLayer);
                DefaultCollisionLayer = DefaultCollisionLayer == -1 ? DefaultLayer : Layers.FindItemIndex(l => (l as Unity_Layer_Map)?.MapIndex == DefaultCollisionLayer);
            }

            if (Layers?.Length > 0)
            {
                Rect[] dimensions = Layers.Select(l => l.GetDimensions(CellSize, CellSizeOverrideCollision)).ToArray();
                MinX = dimensions.Min(d => d.xMin);
                MinY = dimensions.Min(d => d.yMin);
                MaxX = dimensions.Max(d => d.xMax);
                MaxY = dimensions.Max(d => d.yMax);
            }

            var width = (ushort)(MaxX - MinX);
            var height = (ushort)(MaxY - MinY);

            if (Layers.Any(l => l is Unity_Layer_Map)) {
                GridMap = new Unity_Map {
                    Width = width,
                    Height = height,
                    TileSet = new Unity_TileSet[]
                    {
                    new Unity_TileSet(Util.GetGridTex(CellSize), CellSize),
                    },
                    MapTiles = Enumerable.Range(0, width * height).Select(x => new Unity_Tile(new MapTile())).ToArray(),
                    Type = Unity_Map.MapType.Graphics,
                    Layer = Unity_Map.MapLayer.Overlay,
                };
            }

            CanMoveAlongTrack = IsometricData != null && TrackManagers != null && TrackManagers.Any(x => x.IsAvailable(LevelEditorData.MainContext, this));

            if (TrackManagers != null)
                TrackNames = TrackManagers.Select(x => x.Name).ToArray();

            return this;
        }

        public void InitializeDefaultLayers()
        {
            if (Layers != null) 
                return;
            
            List<Unity_Layer> ls = new List<Unity_Layer>();
            
            if (Background != null) 
            {
                ls.Add(new Unity_Layer_Texture() 
                {
                    Name = "Background",
                    ShortName = "BG",
                    Texture = Background,
                    Layer = Unity_Map.MapLayer.Back
                });
            }

            if (ParallaxBackground != null) 
            {
                ls.Add(new Unity_Layer_Texture() 
                {
                    Name = "Parallax Background",
                    ShortName = "PAR",
                    Texture = ParallaxBackground,
                    Layer = Unity_Map.MapLayer.Back
                });
            }

            for (int i = 0; i < Maps?.Length; i++) 
            {
                ls.Add(new Unity_Layer_Map() 
                {
                    Name = $"Map {i} ({Maps[i].Type})",
                    ShortName = i.ToString(),
                    MapIndex = i,
                    Map = Maps[i]
                });
            }
            Layers = ls.ToArray();
        }

        /// <summary>
        /// Auto applies the palette to the tiles in the level
        /// </summary>
        public void AutoApplyPalette()
        {
            var r1Events = EventData.OfType<Unity_Object_R1>().ToArray();

            if (!r1Events.Any())
                return;

            // Get the palette changers
            var paletteXChangers = r1Events.Where(x => x.EventData.Type == ObjType.TYPE_PALETTE_SWAPPER && x.EventData.SubEtat < 6).ToDictionary(x => x.XPosition + x.EventData.OffsetBX, x => (SE_PALETTE_SWAPPER)x.EventData.SubEtat);
            var paletteYChangers = r1Events.Where(x => x.EventData.Type == ObjType.TYPE_PALETTE_SWAPPER && x.EventData.SubEtat >= 6).ToDictionary(x => x.YPosition + x.EventData.OffsetBY, x => (SE_PALETTE_SWAPPER)x.EventData.SubEtat);

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
                    case SE_PALETTE_SWAPPER.Left1toRight2:
                    case SE_PALETTE_SWAPPER.Left1toRight3:
                        defaultPalette = 1;
                        break;
                    case SE_PALETTE_SWAPPER.Left2toRight1:
                    case SE_PALETTE_SWAPPER.Left2toRight3:
                        defaultPalette = 2;
                        break;
                    case SE_PALETTE_SWAPPER.Left3toRight1:
                    case SE_PALETTE_SWAPPER.Left3toRight2:
                        defaultPalette = 3;
                        break;
                }
            }
            else if (!isPaletteHorizontal && paletteYChangers.Any())
            {
                switch (paletteYChangers.OrderByDescending(x => x.Key).First().Value)
                {
                    case SE_PALETTE_SWAPPER.Top1toBottom2:
                    case SE_PALETTE_SWAPPER.Top1toBottom3:
                        defaultPalette = 1;
                        break;
                    case SE_PALETTE_SWAPPER.Top2toBottom1:
                    case SE_PALETTE_SWAPPER.Top2toBottom3:
                        defaultPalette = 2;
                        break;
                    case SE_PALETTE_SWAPPER.Top3toBottom1:
                    case SE_PALETTE_SWAPPER.Top3toBottom2:
                        defaultPalette = 3;
                        break;
                }
            }

            // Keep track of the current palette
            int currentPalette = defaultPalette;

            // Enumerate each cell (PC only has 1 map per level)
            var layer = Layers[DefaultLayer] as Unity_Layer_Map;
            if(layer == null) return;
            var map = layer.Map;
            for (int cellY = 0; cellY < map.Height; cellY++)
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
                        var py = paletteYChangers.TryGetValue((short)(Settings.CellSize * cellY + y), out SE_PALETTE_SWAPPER pm) ? (SE_PALETTE_SWAPPER?)pm : null;

                        // If one was found, change the palette based on type
                        if (py != null)
                        {
                            switch (py)
                            {
                                case SE_PALETTE_SWAPPER.Top2toBottom1:
                                case SE_PALETTE_SWAPPER.Top3toBottom1:
                                    currentPalette = 1;
                                    break;
                                case SE_PALETTE_SWAPPER.Top1toBottom2:
                                case SE_PALETTE_SWAPPER.Top3toBottom2:
                                    currentPalette = 2;
                                    break;
                                case SE_PALETTE_SWAPPER.Top1toBottom3:
                                case SE_PALETTE_SWAPPER.Top2toBottom3:
                                    currentPalette = 3;
                                    break;
                            }
                        }
                    }
                }

                for (int cellX = 0; cellX < map.Width; cellX++)
                {
                    // Check the x position for palette changing
                    if (isPaletteHorizontal)
                    {
                        // Check every pixel 16 steps forward
                        for (int x = 0; x < Settings.CellSize; x++)
                        {
                            // Attempt to find a matching palette changer on this pixel
                            var px = paletteXChangers.TryGetValue((short)(Settings.CellSize * cellX + x), out SE_PALETTE_SWAPPER pm) ? (SE_PALETTE_SWAPPER?)pm : null;

                            // If one was found, change the palette based on type
                            if (px != null)
                            {
                                switch (px)
                                {
                                    case SE_PALETTE_SWAPPER.Left3toRight1:
                                    case SE_PALETTE_SWAPPER.Left2toRight1:
                                        currentPalette = 1;
                                        break;
                                    case SE_PALETTE_SWAPPER.Left1toRight2:
                                    case SE_PALETTE_SWAPPER.Left3toRight2:
                                        currentPalette = 2;
                                        break;
                                    case SE_PALETTE_SWAPPER.Left1toRight3:
                                    case SE_PALETTE_SWAPPER.Left2toRight3:
                                        currentPalette = 3;
                                        break;
                                }
                            }
                        }
                    }

                    // Set the common tile
                    map.MapTiles[cellY * map.Width + cellX].PaletteIndex = currentPalette;
                }
            }
        }

        #endregion
    }
}