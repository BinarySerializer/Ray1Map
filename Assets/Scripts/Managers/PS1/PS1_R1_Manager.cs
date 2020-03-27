using R1Engine.Serialize;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_Manager : PS1_Manager
    {
        /// <summary>
        /// Reads the tile set for the specified world
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The tile set</returns>
        public override Common_Tileset ReadTileSet(Context context) {
            // Get the file name
            var filename = GetWorldFilePath(context.Settings);

            // Read the file
            var worldFile = FileFactory.Read<PS1_R1_WorldFile>(filename, context);

            int tile = 0;
            int tileCount = worldFile.TilePaletteIndexTable.Length;
            const int width = 256;
            int height = (worldFile.TilesIndexTable.Length) / width;
            Color[] pixels = new Color[width * height];

            for (int yB = 0; yB < height; yB += 16)
                for (int xB = 0; xB < width; xB += 16, tile++)
                    for (int y = 0; y < CellSize; y++)
                        for (int x = 0; x < CellSize; x++)
                        {
                            if (tile >= tileCount)
                                goto End;

                            int pixel = x + xB + (y + yB) * width;

                            byte tileIndex1 = worldFile.TilePaletteIndexTable[tile];
                            byte tileIndex2 = worldFile.TilesIndexTable[pixel];
                            pixels[pixel] = worldFile.TileColorPalettes[tileIndex1][tileIndex2].GetColor();
                        }
            End:
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            tex.SetPixels(pixels);
            tex.Apply();

            var tiles = new Tile[tex.width * tex.height];

            // Loop through all the 16x16 cells in the tileset Texture2D and generate tiles out of it
            int tileIndex = 0;
            for (int yy = 0; yy < (tex.height / CellSize); yy++)
            {
                for (int xx = 0; xx < (tex.width / CellSize); xx++)
                {
                    // Create a tile
                    Tile t = ScriptableObject.CreateInstance<Tile>();
                    t.sprite = Sprite.Create(tex, new Rect(xx * CellSize, yy * CellSize, CellSize, CellSize),
                        new Vector2(0.5f, 0.5f), CellSize, 20);
                    tiles[tileIndex] = t;
                    tileIndex++;
                }
            }

            return new Common_Tileset(tiles);
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context)
        {
            // Read the allfix file
            var allfix = FileFactory.Read<PS1_R1_AllfixFile>(GetAllfixFilePath(context.Settings), context);

            Controller.status = $"Loading world file";

            await Controller.WaitIfNecessary();

            // Read the world file
            await LoadExtraFile(context, GetWorldFilePath(context.Settings));

            var world = FileFactory.Read<PS1_R1_WorldFile>(GetWorldFilePath(context.Settings), context);

            Controller.status = $"Loading map data";

            // Read the level data
            await LoadExtraFile(context, GetLevelFilePath(context.Settings));

            var levelData = FileFactory.Read<PS1_R1_LevFile>(GetLevelFilePath(context.Settings), context);

            // Load the level
            return await LoadAsync(context, allfix, world, levelData.MapData, levelData.EventData, levelData.TextureBlock);
        }
    }
}