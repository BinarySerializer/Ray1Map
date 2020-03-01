using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace R1Engine
{
    public class LevelTilemapController : MonoBehaviour 
    {
        /// <summary>
        /// References to specific tilemap gameObjects in inspector
        /// </summary>
        public Tilemap[] Tilemaps;

        /// <summary>
        /// The events
        /// </summary>
        public GameObject Events;

        /// <summary>
        /// The tile layer buttons, so they can be hidden with versions that don't have 3 palettes
        /// </summary>
        public Button[] LayerButtons;

        /// <summary>
        /// The type collision tiles
        /// </summary>
        public Tile[] TypeCollisionTiles;
        public Tile[] TypeCollisionTilesHD;

        /// <summary>
        /// The current level
        /// </summary>
        public Common_Lev Level { get; set; }

        public void InitializeTilemaps(Common_Lev lvl) {

            Level = lvl;

            // Hide layer buttons according to the version
            if (Settings.Mode == GameMode.RaymanPS1) {
                LayerButtons[1].interactable = false;
                LayerButtons[1].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

                LayerButtons[2].interactable = false;
                LayerButtons[2].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            }

            // Loop through the level tiledata and populate the tilemaps with it
            foreach(Common_Tile t in Level.Tiles) {

                // Index 0 is collision types tilemap
                if (Settings.UseHDCollisionSheet) {
                    Tilemaps[0].SetTile(new Vector3Int(t.XPosition, t.YPosition, 0), TypeCollisionTilesHD[(int)t.CollisionType]);
                }
                else {
                    Tilemaps[0].SetTile(new Vector3Int(t.XPosition, t.YPosition, 0), TypeCollisionTiles[(int)t.CollisionType]);
                }


                // Assign tiles to their correct tilemaps based on the palette
                if (Level.TileSet[t.PaletteIndex] != null && t.TileSetGraphicIndex != -1)
                    Tilemaps[t.PaletteIndex].SetTile(new Vector3Int(t.XPosition, t.YPosition, 0), Level.TileSet[t.PaletteIndex].Tiles[t.TileSetGraphicIndex]);
            }
        }

        public Common_Tile GetTileAtPos(int tilemap, int x, int y) {
            return Array.Find(Level.Tiles, t => t.XPosition == x && t.YPosition == y);
        }

        public void ShowHideLayer(int index) {
            Tilemaps[index].gameObject.SetActive(!Tilemaps[index].gameObject.activeSelf);
        }
        public void ShowHideEvents() {
            Events.SetActive(!Events.activeSelf);
        }
    }
}
