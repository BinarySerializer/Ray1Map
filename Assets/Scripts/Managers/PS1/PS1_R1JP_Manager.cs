using R1Engine.Serialize;
using System.Collections.Generic;
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
        /// Reads the tile set for the specified world
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The tile set</returns>
        public override Common_Tileset ReadTileSet(Context context)
        {
            // Get the file name
            var filename = GetWorldFilePath(context.Settings);

            // Read the file
            var worldJPFile = FileFactory.Read<PS1_R1JP_WorldFile>(filename, context);

            // Create the tile array
            var tilesJP = new Tile[worldJPFile.Tiles.Length];

            int tileIndexJP = 0;

            // Create each tile
            foreach (var tileJP in worldJPFile.Tiles)
            {
                // Create the texture
                Texture2D texJP = new Texture2D(CellSize, CellSize, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point
                };

                // Set every pixel
                for (int y = 0; y < CellSize; y++)
                {
                    for (int x = 0; x < CellSize; x++)
                    {
                        texJP.SetPixel(x, y, tileJP[y * CellSize + x].GetColor());
                    }
                }

                // Apply the pixels
                texJP.Apply();

                // Create a tile
                Tile t = ScriptableObject.CreateInstance<Tile>();
                t.sprite = Sprite.Create(texJP, new Rect(0, 0, CellSize, CellSize), new Vector2(0.5f, 0.5f), CellSize, 20);

                tilesJP[tileIndexJP] = t;
                tileIndexJP++;
            }

            // Return the tileset
            return new Common_Tileset(tilesJP);
        }

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="eventDesigns">The list of event designs to populate</param>
        /// <returns>The level</returns>
        public override async Task<Common_Lev> LoadLevelAsync(Context context, List<Common_Design> eventDesigns)
        {
            // Read the allfix file
            var allfix = FileFactory.Read<PS1_R1_AllfixFile>(GetAllfixFilePath(context.Settings), context);

            Controller.status = $"Loading world file";

            await Controller.WaitIfNecessary();

            // Read the world file
            await LoadExtraFile(context, GetWorldFilePath(context.Settings));

            var world = FileFactory.Read<PS1_R1JP_WorldFile>(GetWorldFilePath(context.Settings), context);

            Controller.status = $"Loading map data";

            // Read the level data
            await LoadExtraFile(context, GetLevelFilePath(context.Settings));

            var levelData = FileFactory.Read<PS1_R1_LevFile>(GetLevelFilePath(context.Settings), context);

            // Load and return the common level
            return await LoadCommonLevelAsync(context, eventDesigns, allfix, null, levelData.MapData, levelData.EventData, levelData.TextureBlock);
        }
    }
}