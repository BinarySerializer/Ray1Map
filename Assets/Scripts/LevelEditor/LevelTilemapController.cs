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

        // Reference to tile cursor
        public Transform tileCursor;

        // References to temp things on the UI
        public Image tileImage;
        public Image typeImage;

        public int currentTile = 0;
        public int currentType = 0;
        public int targetLayer = 1;

        private void Update() {/*
            Vector3 mousePositionTile = MouseToTileCoords(Input.mousePosition);
            // Update square tile cursor
            tileCursor.transform.position = new Vector3(mousePositionTile.x, mousePositionTile.y, 10);

            //Change current tile with mouse wheel
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                if (currentTile > 0) currentTile--;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                if (currentTile < Level.TileSet[1].Tiles.Length) currentTile++;
            }
            //Update current tile and type image
            if (currentTile == 0) {
                tileImage.sprite = null;
            }
            else {
                tileImage.sprite = Level.TileSet[1].Tiles[currentTile].sprite;
            }
            
            if (currentType == 0) {
                typeImage.sprite = null;
            }
            else {
                typeImage.sprite = TypeCollisionTiles[currentType].sprite;
            }
            

            //Change tile at position when clicked
            if (Input.GetMouseButton(0)) {
                if (targetLayer == 0) {
                    SetTypeAtPos((int)mousePositionTile.x, -(int)mousePositionTile.y, currentType);
                }
                else {
                    SetTileAtPos((int)mousePositionTile.x, -(int)mousePositionTile.y, currentTile, targetLayer);
                }
            }*/
        }

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

        // Converts mouse position to worldspace and then tile positions (1 = 16)
        public Vector3 MouseToTileCoords(Vector3 mousePos) {
            var worldMouse = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
            return new Vector3(Mathf.Floor(worldMouse.x), Mathf.Floor(worldMouse.y + 1), 10);
        }

        // Get one common tile at given position
        public Common_Tile GetTileAtPos(int x, int y) {
            return Level.Tiles[
                Mathf.Clamp(Mathf.FloorToInt(x), 0, Level.Width - 1)
                + Mathf.Clamp(Mathf.FloorToInt(-y), 0, Level.Height - 1) * Level.Width];
        }

        /// <summary>
        /// Get the tile under the mouse.
        /// </summary>
        /// <returns></returns>
        public Common_Tile GetMouseTile() {
            var worldMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return GetTileAtPos(Mathf.FloorToInt(worldMouse.x), Mathf.FloorToInt(worldMouse.y + 1));
        }

        public void SetTileAtPos(int x, int y, int gIndex, int layer) {
            Common_Tile found = GetTileAtPos(x, y);
            if (found != null) {
                // If on PC and trying to assign 0 (empty), change to -1 instead
                if (Settings.Mode == GameMode.RaymanPC && gIndex==0) {
                    found.TileSetGraphicIndex = -1;
                }
                else {
                    found.TileSetGraphicIndex = gIndex;
                }
                
                found.PaletteIndex = layer;
                //First clear the cell on all tilemaps
                ClearUnityTilemapAt(x, y);
                // If on PC and trying to change the tile to 0 (black square), assing a null tile instead
                if (gIndex == 0) {
                    Tilemaps[layer].SetTile(new Vector3Int(x, y, 0), null);
                }
                else {
                    Tilemaps[layer].SetTile(new Vector3Int(x, y, 0), Level.TileSet[layer].Tiles[gIndex]);
                }               
            }
        }

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
        }

        public void ClearUnityTilemapAt(int x, int y) {
            Tilemaps[1].SetTile(new Vector3Int(x, y, 0), null);
            Tilemaps[2].SetTile(new Vector3Int(x, y, 0), null);
            Tilemaps[3].SetTile(new Vector3Int(x, y, 0), null);
        }

        // Used by hud buttons to show/hide tilemap layers
        public void ShowHideLayer(int index) {
            Tilemaps[index].gameObject.SetActive(!Tilemaps[index].gameObject.activeSelf);
        }
        public void ShowHideEvents() {
            Events.SetActive(!Events.activeSelf);
        }
    }
}
