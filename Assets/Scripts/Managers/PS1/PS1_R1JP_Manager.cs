using System;
using R1Engine.Serialize;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1 - Japan)
    /// </summary>
    public class PS1_R1JP_Manager : PS1_Manager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 1;

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public virtual PS1_R1JP_TileSet GetTileSet(Context context)
        {
            // Get the file name
            var filename = GetWorldFilePath(context.Settings);

            // Read the file
            var worldJPFile = FileFactory.Read<PS1_R1JP_WorldFile>(filename, context);

            // Return the tile set
            return worldJPFile.TileSet;
        }

        /// <summary>
        /// Reads the tile set for the specified world
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The tile set</returns>
        public override Common_Tileset ReadTileSet(Context context)
        {
            // Get the tile set
            var tileSet = GetTileSet(context).Tiles;

            // Create the tile array
            var tilesJP = new Tile[tileSet.Length];

            // Create each tile
            for (var index = 0; index < tileSet.Length / (CellSize * CellSize); index++)
            {
                // Create the texture
                Texture2D texJP = new Texture2D(CellSize, CellSize, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };

                // Get the tile x and y
                var tileY = (int)Math.Floor(index / (double)TileSetWidth);
                var tileX = (index - (TileSetWidth * tileY));

                var tileOffset = (tileY * TileSetWidth * CellSize * CellSize) + (tileX * CellSize);

                // Set every pixel
                for (int y = 0; y < CellSize; y++)
                {
                    for (int x = 0; x < CellSize; x++)
                    {
                        texJP.SetPixel(x, y, tileSet[(tileOffset + (y * CellSize * TileSetWidth + x))].GetColor());
                    }
                }

                // Apply the pixels
                texJP.Apply();

                // Create a tile
                Tile t = ScriptableObject.CreateInstance<Tile>();
                t.sprite = Sprite.Create(texJP, new Rect(0, 0, CellSize, CellSize), new Vector2(0.5f, 0.5f), CellSize,
                    20);

                tilesJP[index] = t;
            }

            // Return the tileset
            return new Common_Tileset(tilesJP);
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context)
        {
            // Read the allfix file
            await LoadExtraFile(context, GetAllfixFilePath(context.Settings));
            var allfix = FileFactory.Read<PS1_R1_AllfixFile>(GetAllfixFilePath(context.Settings), context);

            Controller.status = $"Loading world file";

            await Controller.WaitIfNecessary();

            // Read the world file
            await LoadExtraFile(context, GetWorldFilePath(context.Settings));
            var world = FileFactory.Read<PS1_R1JP_WorldFile>(GetWorldFilePath(context.Settings), context);

            Controller.status = $"Loading map data";

            // Read the level data
            await LoadExtraFile(context, GetLevelFilePath(context.Settings));
            var level = FileFactory.Read<PS1_R1_LevFile>(GetLevelFilePath(context.Settings), context);

            // Load the level
            return await LoadAsync(context, allfix, null, level.MapData, level.EventData, level.TextureBlock);
        }
    }
}