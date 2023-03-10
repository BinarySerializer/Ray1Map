using System;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Ray1;
using UnityEngine;

namespace Ray1Map
{
    /// <summary>
    /// Common level map data
    /// </summary>
    public class Unity_Map {
        #region Public Properties

        /// <summary>
        /// The level width
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The level height
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// The width of the tileset in tiles
        /// </summary>
        public int TileSetWidth { get; set; } = 1;

        /// <summary>
        /// The tile-sets, one for each palette
        /// </summary>
        public Unity_TileSet[] TileSet { get; set; }

        /// <summary>
        /// The transparency mode for the tiles in the tileset on PC
        /// </summary>
        public Block.PC_TransparencyMode[] TileSetTransparencyModes { get; set; }

        /// <summary>
        /// Tile texture offset table for PC
        /// </summary>
        public Pointer[] PCTileOffsetTable { get; set; }

        /// <summary>
        /// The map tiles
        /// </summary>
        public Unity_Tile[] MapTiles { get; set; }

        public float? Alpha { get; set; }
        public bool IsAdditive { get; set; }
        public MapType Type { get; set; }
        public MapLayer Layer { get; set; } = MapLayer.Middle;

        public FreeCameraSettings Settings3D { get; set; }

        [Flags]
        public enum MapType {
            None = 0,
            Graphics = 1 << 0,
            Collision = 1 << 1
        }
        public enum MapLayer {
            Middle,
            Back,
            Front,
            Overlay
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the tile for the specific map tile
        /// </summary>
        /// <param name="mapTile">The map tile</param>
        /// <param name="settings">The game settings</param>
        /// <returns>The tile</returns>
        public Unity_TileTexture GetTile(Unity_Tile mapTile, GameSettings settings, int? tileIndexOverride = null)
        {
            // Get the tile index
            int tileIndex;
            if (tileIndexOverride.HasValue) {
                tileIndex = (TileSetWidth * tileIndexOverride.Value) + mapTile.Data.TileMapX;
            } else {
                tileIndex = (TileSetWidth * mapTile.Data.TileMapY) + mapTile.Data.TileMapX;
            }
            // Get the tile array
            var tiles = TileSet[mapTile.PaletteIndex - 1].Tiles;

            // Check if it's out of bounds
            if (tileIndex >= tiles.Length)
            {
                // If it's out of bounds and the level is Jungle 27 in PS1 EDU, hard-code to 509, which is what the game uses there
                if (settings.EngineVersion == EngineVersion.R1_PS1_Edu && settings.R1_World == World.Jungle && settings.Level == 27)
                {
                    tileIndex = 509;
                }
                // Hacky fix for RRR
                else if (settings.EngineVersion == EngineVersion.GBARRR && settings.Level == 25)
                {
                    tileIndex = 4082;
                }
                else
                {
                    Debug.LogWarning($"Out of bounds tile with index {tileIndex} in {settings.GameModeSelection} - {settings.World}-{settings.Level} (tiles count is {tiles.Length})");

                    tileIndex = 0;
                }
            }

            // Return the tile
            return tiles[tileIndex];
        }

        public Unity_AnimatedTile.Instance GetAnimatedTile(Unity_Tile mapTile, GameSettings settings) {
            // Get the tile index
            var tileIndex = (TileSetWidth * mapTile.Data.TileMapY) + mapTile.Data.TileMapX;
            var tileset = TileSet[mapTile.PaletteIndex - 1];

            if (tileset.AnimatedTiles != null) {
                foreach (var at in tileset.AnimatedTiles) {
                    if (at.TileIndices?.Length > 0 && at.TileIndices[0] == tileIndex){
                        //int index = Array.IndexOf(at.TileIndices, tileIndex);
                        int index = 0;
                        if (index >= 0) {
                            return new Unity_AnimatedTile.Instance(at, index);
                        }
                    }
                }
                return null;
            }
            return null;
        }

        public Unity_Tile GetMapTile(int x, int y) => MapTiles.ElementAtOrDefault((Width * y) + x);

        #endregion

        #region Free Camera Settings
        public class FreeCameraSettings {
            public Mode3D Mode { get; set; }
            public Vector3? Position { get; set; }
            public Vector3? PositionCollision { get; set; }
            public Quaternion? Rotation { get; set; }
            public Vector3? Scale { get; set; }
            public int? SortingOrderGraphics { get; set; }
            public int? SortingOrderCollision { get; set; }

            public enum Mode3D {
                Billboard,
                FixedPosition
            }

            public static FreeCameraSettings Mode7 => new FreeCameraSettings()
            {
                Mode = Mode3D.FixedPosition,
                Position = Vector3.zero,
                //PositionCollision = Vector3.forward * 0.05f,
                Rotation = Quaternion.Euler(-90, 0, 0),
                SortingOrderGraphics = -2,
                SortingOrderCollision = -1
            };
        }
		#endregion
	}
}