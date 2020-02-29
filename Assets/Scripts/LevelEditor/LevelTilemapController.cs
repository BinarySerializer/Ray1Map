using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine {
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
                tilemaps[0].SetTile(new Vector3Int(t.x, t.y, 0), typeCollisionTiles[(int)t.cType]);
                // Indexes 1-3 are normal tilemaps
                if (level.TileSet[1] != null) tilemaps[1].SetTile(new Vector3Int(t.x, t.y, 0), level.TileSet[1].individualTiles[t.gIndex]);
                if (level.TileSet[2] != null) tilemaps[2].SetTile(new Vector3Int(t.x, t.y, 0), level.TileSet[2].individualTiles[t.gIndex]);
                if (level.TileSet[3] != null) tilemaps[3].SetTile(new Vector3Int(t.x, t.y, 0), level.TileSet[3].individualTiles[t.gIndex]);
            }
        }
    }
}
