using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Common_Tileset
{
    // Hold the original texture this tileset is based on (not sure if needed in the end)
    public Texture2D tilesetTexture;

    // All the individual tile cells
    public Tile[] individualTiles;

    private int cellSize;

    public Common_Tileset(Texture2D tex, int cell = 16) {
        tilesetTexture = tex;
        cellSize = cell;

        individualTiles = new Tile[tex.width * tex.height];

        // Loop through all the 16x16 cells in the tileset Texture2D and generate tiles out of it
        int tileIndex = 0;
        for (int yy = 0; yy < (tex.height / cellSize); yy++) {
            for (int xx = 0; xx < (tex.width / cellSize); xx++) {
                // Create a tile
                Tile t = new Tile();
                t.sprite = Sprite.Create(tilesetTexture, new Rect(xx * cellSize, yy * cellSize, cellSize, cellSize), new Vector2(0.5f, 0.5f), cellSize, 20);
                individualTiles[tileIndex] = t;
                tileIndex++;
            }
        }
    }
}
