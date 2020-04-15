using System;
using System.Linq;
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
        //The ui text
        public GameObject paletteText;
        //0 is auto
        private int currentPalette = 1;

        /// <summary>
        /// The type collision tiles
        /// </summary>
        public Tile[] TypeCollisionTiles;
        public Tile[] TypeCollisionTilesHD;

        public void Start() {
            // Disable palette buttons based on if there are 3 palettes or not
            if (!Settings.GetGameManager.Has3Palettes) {
                paletteText.SetActive(false);
                paletteButtons[0].gameObject.SetActive(false);
                paletteButtons[1].gameObject.SetActive(false);
                paletteButtons[2].gameObject.SetActive(false);
                paletteButtons[3].gameObject.SetActive(false);
            }
        }

        public void InitializeTilemaps() {
            // Fill out types first
            foreach(Common_Tile t in Controller.obj.levelController.currentLevel.Tiles) 
            {
                var collisionTypeIndex = (int)t.CollisionType;

                // Fill out types first
                if (Settings.UseHDCollisionSheet) 
                {
                    if (collisionTypeIndex >= TypeCollisionTilesHD.Length)
                    {
                        Debug.LogWarning($"Collision type {t.CollisionType} is not supported");
                        collisionTypeIndex = 0;
                    }

                    Tilemaps[0].SetTile(new Vector3Int(t.XPosition, t.YPosition, 0), TypeCollisionTilesHD[collisionTypeIndex]);
                }
                else 
                {
                    if (collisionTypeIndex >= TypeCollisionTiles.Length)
                    {
                        Debug.LogWarning($"Collision type {t.CollisionType} is not supported");
                        collisionTypeIndex = 0;
                    }

                    Tilemaps[0].SetTile(new Vector3Int(t.XPosition, t.YPosition, 0), TypeCollisionTiles[collisionTypeIndex]);
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

        public Common_Tile SetTileAtPos(int x, int y, Common_Tile newTileInfo, Common_Tile tile = null) {
            ClearUnityTilemapAt(x, y);
            Tilemaps[0].SetTile(new Vector3Int(x, y, 0), TypeCollisionTiles[(int)newTileInfo.CollisionType]);
            Tilemaps[1].SetTile(new Vector3Int(x, y, 0), Controller.obj.levelController.currentLevel.TileSet[1].Tiles[newTileInfo.TileSetGraphicIndex]);

            // Get the tile if null
            if (tile == null)
                tile = Controller.obj.levelController.currentLevel.Tiles.FindItem(item => item.XPosition == x && item.YPosition == y);

            tile.CollisionType = newTileInfo.CollisionType;
            tile.PaletteIndex = newTileInfo.PaletteIndex;
            tile.TileSetGraphicIndex = newTileInfo.TileSetGraphicIndex;

            return tile;
        }

        public Common_Tile SetTypeAtPos(int x, int y, TileCollisionType typeIndex, Common_Tile tile = null) {
            Tilemaps[0].SetTile(new Vector3Int(x, y, 0), null);
            Tilemaps[0].SetTile(new Vector3Int(x, y, 0), TypeCollisionTiles[(int)typeIndex]);

            // Get the tile if null
            if (tile == null)
                tile = Controller.obj.levelController.currentLevel.Tiles.FindItem(item => item.XPosition == x && item.YPosition == y);
            tile.CollisionType = typeIndex;

            return tile;
        }

        /*
        public void SetTileAtPos(int x, int y, int gIndex, int layer) {
            Common_Tile found = GetTileAtPos(x, y);
            if (found != null) {
                found.TileSetGraphicIndex = gIndex;
                found.PaletteIndex = layer;
                //First clear the cell on all tilemaps
                ClearUnityTilemapAt(x, y);
                // If on PC and trying to change the tile to 0 (black square), assing a null tile instead
                if (gIndex == 0) {
                    Tilemaps[layer].SetTile(new Vector3Int(x, y, 0), null);
                }
                else {
                    Tilemaps[layer].SetTile(new Vector3Int(x, y, 0), Controller.obj.levelController.currentLevel.TileSet[layer].Tiles[gIndex]);
                }
            }
        }*/
        /*
        public void SetTypeAtPos(int x, int y, int tIndex) {
            Common_Tile found = GetTileAtPos(x, y);
            if (found != null) {
                found.CollisionType = (TileCollisionType)tIndex;

                // Index 0 is collision types tilemap
                if (Settings.UseHDCollisionSheet) {
                    Tilemaps[0].SetTile(new Vector3Int(x, y, 0), TypeCollisionTilesHD[tIndex]);
                }
                else {
                    Tilemaps[0].SetTile(new Vector3Int(x, y, 0), TypeCollisionTiles[tIndex]);
                }
            }
        }*/

        public void ClearUnityTilemapAt(int x, int y) {
            Tilemaps[0].SetTile(new Vector3Int(x, y, 0), null);
            Tilemaps[1].SetTile(new Vector3Int(x, y, 0), null);
        }
    }
}
