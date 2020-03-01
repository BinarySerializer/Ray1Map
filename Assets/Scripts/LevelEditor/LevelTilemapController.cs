using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace R1Engine
{
    public class LevelTilemapController : MonoBehaviour {
        // References to specific tilemap gameObjects in inspector
        public Tilemap[] tilemaps;
        public GameObject events;

        // References to tile layer buttons so they can be hidden with versions that don't have 3 palettes
        public Button[] layerButtons;
        
        // Reference to type sprites
        public Tile[] typeCollisionTiles;

        // TODO: Add a boolean switch for hd type sprites

        // Level currently in use
        public Common_Lev level;

        public void InitializeTilemaps(Common_Lev lvl) {

            level = lvl;

            //Hide layer buttons according to the version
            if (Settings.Mode == GameMode.RaymanPS1) {
                layerButtons[1].interactable = false;
                layerButtons[1].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

                layerButtons[2].interactable = false;
                layerButtons[2].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            }

            // Loop through the level tiledata and populate the tilemaps with it
            foreach(Common_Tile t in level.Tiles) {

                // Index 0 is collision types tilemap
                tilemaps[0].SetTile(new Vector3Int(t.XPosition, t.YPosition, 0), typeCollisionTiles[(int)t.CollisionType]);

                // Assign tiles to their correct tilemaps based on the palette
                if (level.TileSet[t.PaletteIndex] != null && t.TileSetGraphicIndex != -1)
                    tilemaps[t.PaletteIndex].SetTile(new Vector3Int(t.XPosition, t.YPosition, 0), level.TileSet[t.PaletteIndex].Tiles[t.TileSetGraphicIndex]);
            }
        }

        public Common_Tile GetTileAtPos(int tilemap, int x, int y) {
            return Array.Find(level.Tiles, t => t.XPosition == x && t.YPosition == y);
        }

        public void ShowHideLayer(int index) {
            tilemaps[index].gameObject.SetActive(!tilemaps[index].gameObject.activeSelf);
        }
        public void ShowHideEvents() {
            events.SetActive(!events.activeSelf);
        }
    }
}
