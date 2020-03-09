using System.IO;
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
        /// <param name="settings">The game settings</param>
        /// <returns>The tile set</returns>
        public override Common_Tileset ReadTileSet(GameSettings settings)
        {
            // Get the file name
            var fileName = Path.Combine(GetWorldFolderPath(settings), $"{GetWorldName(settings.World)}.XXX");

            // Read the file
            var worldFile = FileFactory.Read<PS1_R1_WorldFile>(fileName, settings);

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

                            pixels[pixel] = worldFile.TileColorPalettes[worldFile.TilePaletteIndexTable[tile]][worldFile.TilesIndexTable[pixel]].GetColor();
                        }
            End:
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
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
    }
}