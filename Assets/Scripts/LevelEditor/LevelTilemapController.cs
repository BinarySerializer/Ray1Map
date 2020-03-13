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
        /// Palette buttons
        /// </summary>
        public Button[] paletteButtons;
        //0 is auto
        private int currentPalette = 1;

        /// <summary>
        /// The type collision tiles
        /// </summary>
        public Tile[] TypeCollisionTiles;
        public Tile[] TypeCollisionTilesHD;

        public void InitializeTilemaps() {
            // Fill out types first
            foreach(Common_Tile t in Controller.obj.levelController.currentLevel.Tiles) {
                // Fill out types first
                if (Settings.UseHDCollisionSheet) {
                    Tilemaps[0].SetTile(new Vector3Int(t.XPosition, t.YPosition, 0), TypeCollisionTilesHD[(int)t.CollisionType]);
                }
                else {
                    Tilemaps[0].SetTile(new Vector3Int(t.XPosition, t.YPosition, 0), TypeCollisionTiles[(int)t.CollisionType]);
                }
            }
            // Fill out tiles
            RefreshTiles(Settings.GetGameManager.Has3Palettes ? 0 : 1);
        }

        // Used to redraw all tiles with different palette (0 = auto, 1-3 = palette)
        public void RefreshTiles(int palette) {
            //Change button visibility
            currentPalette = palette;
            for (int i = 0; i < paletteButtons.Length; i++) {
                ColorBlock b = paletteButtons[i].colors;
                if (currentPalette == i) {
                    b.normalColor = new Color(1, 1, 1);
                }
                else {
                    b.normalColor = new Color(0.5f, 0.5f, 0.5f);
                }
                paletteButtons[i].colors = b;
            }

            // Get the current level
            var lvl = Controller.obj.levelController.currentLevel;

            // If auto, refresh indexes in manager
            if (palette == 0)
                Settings.GetGameManager.AutoApplyPalette(lvl);

            //Refresh tiles
            foreach (Common_Tile t in lvl.Tiles)
            {
                int p = (palette == 0 ? t.PaletteIndex : palette) - 1;
                Tilemaps[1].SetTile(new Vector3Int(t.XPosition, t.YPosition, 0), lvl.TileSet[p].Tiles[t.TileSetGraphicIndex]);
            }
        }

        /// <summary>
        /// Returns the tile under the mouse.
        /// </summary>
        /// <returns></returns>
        public Common_Tile GetMouseTile() {
            var worldMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return GetTileAtPos(Mathf.FloorToInt(worldMouse.x), Mathf.FloorToInt(worldMouse.y + 1));
        }
        // Converts mouse position to worldspace and then tile positions (1 = 16)
        public Vector3 MouseToTileCoords(Vector3 mousePos) {
            var worldMouse = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
            return new Vector3(Mathf.Floor(worldMouse.x), Mathf.Floor(worldMouse.y + 1), 10);
        }

        // Get one common tile at given position
        public Common_Tile GetTileAtPos(int x, int y) {
            if (Controller.obj.levelController.currentLevel == null) {
                return null;
            }
            else {
                foreach (var t in Controller.obj.levelController.currentLevel.Tiles) {
                    if (t.XPosition == x && t.YPosition == y) {
                        return t;
                    }
                }
                return null;
            }
        }

        // Set common tile to given position
        public void SetTileAtPos(int x, int y, Common_Tile t) {
            Common_Tile found = GetTileAtPos(x, y);
            if (found != null) {
                found.PaletteIndex = t.PaletteIndex;
                found.TileSetGraphicIndex = t.TileSetGraphicIndex;
                // Assign correct unity tile to the tilemap
                Tilemaps[1].SetTile(new Vector3Int(x, y, 0), Controller.obj.levelController.currentLevel.TileSet[1].Tiles[t.TileSetGraphicIndex]);
            }
        }

        // Set type to position
        public void SetTypeAtPos(int x, int y, int tIndex) {
            Common_Tile found = GetTileAtPos(x, y);
            if (found != null) {
                found.CollisionType = (TileCollisionType)tIndex;
                // Assign correct type
                if (Settings.UseHDCollisionSheet) {
                    Tilemaps[0].SetTile(new Vector3Int(x, y, 0), TypeCollisionTilesHD[tIndex]);
                }
                else {
                    Tilemaps[0].SetTile(new Vector3Int(x, y, 0), TypeCollisionTiles[tIndex]);
                }
            }
        }
    }
}
