using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    public class LevelTilemapController : MonoBehaviour {
        // References to specific tilemap gameObjects in inspector
        public Tilemap[] tilemaps;
        
        // Reference to type sprites
        public Tile[] typeCollisionTiles;

        // TODO: Add a boolean switch for hd type sprites

        // Level currently in use
        public Common_Lev level;

        public void InitializeTilemaps(Common_Lev lvl) {

            level = lvl;

            // Loop through the level tiledata and populate the tilemaps with it
            foreach(Common_Tile t in level.Tiles) {

                // Index 0 is collision types tilemap
                tilemaps[0].SetTile(new Vector3Int(t.XPosition, t.YPosition, 0), typeCollisionTiles[(int)t.CollisionType]);

                // Assign tiles to their correct tilemaps based on the palette
                if (level.TileSet[t.PaletteIndex] != null && t.TileSetGraphicIndex != -1)
                    tilemaps[t.PaletteIndex].SetTile(new Vector3Int(t.XPosition, t.YPosition, 0), level.TileSet[t.PaletteIndex].Tiles[t.TileSetGraphicIndex]);
            }
        }
    }
}
