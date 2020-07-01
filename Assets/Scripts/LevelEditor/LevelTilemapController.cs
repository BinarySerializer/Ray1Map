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

        public Tilemap tilemapFull;
        public bool focusedOnTemplate=false;
        public Editor editor;

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

        // Reference to the background ting
        public SpriteRenderer backgroundTint;

        /// <summary>
        /// The type collision tiles
        /// </summary>
        public Tile[] TypeCollisionTiles;
        public Tile[] TypeCollisionTilesHD;

        // Infro tracked for when switching between template and normal level
        private Vector3 previousCameraPosNormal;
        private Vector3 previousCameraPosTemplate = new Vector3(0,0,-10f);
        private int templateMaxY=0;

        public int camMaxX=1;
        public int camMaxY=1;


        public void InitializeTilemaps() {
            // Disable palette buttons based on if there are 3 palettes or not
            if (!Controller.obj.levelController.EditorManager.Has3Palettes)
            {
                paletteText.SetActive(false);
                paletteButtons[0].gameObject.SetActive(false);
                paletteButtons[1].gameObject.SetActive(false);
                paletteButtons[2].gameObject.SetActive(false);
                paletteButtons[3].gameObject.SetActive(false);
            }

            var map = Controller.obj.levelController.currentLevel.Maps[editor.currentMap];
            var collisionTileSet = Settings.UseHDCollisionSheet ? TypeCollisionTilesHD : TypeCollisionTiles;

            // Fill out types first
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    // Get the collision index
                    var collisionTypeIndex = (int)map.MapTiles[y * map.Width + x].Data.CollisionType;

                    // Make sure it's not out of bounds
                    if (collisionTypeIndex >= collisionTileSet.Length)
                    {
                        Debug.LogWarning($"Collision type {collisionTypeIndex} is not supported");
                        collisionTypeIndex = 0;
                    }

                    // Set the collision tile
                    Tilemaps[0].SetTile(new Vector3Int(x, y, 0), collisionTileSet[collisionTypeIndex]);
                }
            }

            // Fill out tiles
            RefreshTiles(Controller.obj.levelController.EditorManager.Has3Palettes ? 0 : 1);

            //Set max cam sizes
            camMaxX = Controller.obj.levelController.currentLevel.Maps[editor.currentMap].Width;
            camMaxY = Controller.obj.levelController.currentLevel.Maps[editor.currentMap].Height;
        }

        // Used to redraw all tiles with different palette (0 = auto, 1-3 = palette)
        public void RefreshTiles(int palette)
        {
            // Change button visibility
            currentPalette = palette;
            for (int i = 0; i < paletteButtons.Length; i++) {
                ColorBlock b = paletteButtons[i].colors;
                b.normalColor = currentPalette == i ? new Color(1, 1, 1) : new Color(0.5f, 0.5f, 0.5f);
                paletteButtons[i].colors = b;
            }

            // Get the current level and map
            var lvl = Controller.obj.levelController.currentLevel;
            var map = lvl.Maps[editor.currentMap];

            // If auto, refresh indexes
            if (palette == 0)
                lvl.AutoApplyPalette();

            // Refresh tiles
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    var t = map.MapTiles[y * map.Width + x];

                    if (palette != 0)
                        t.PaletteIndex = palette;

                    Tilemaps[1].SetTile(new Vector3Int(x, y, 0), map.GetTile(t, Controller.obj.levelController.EditorManager.Settings));
                }
            }

            // Refresh the full tilemap template
            int xx = 0;
            int yy = 0;
            foreach(Tile t in lvl.Maps[editor.currentMap].TileSet[0].Tiles) 
            {
                tilemapFull.SetTile(new Vector3Int(xx, yy, 0), t);
                xx++;
                if (xx == 16) {
                    xx = 0;
                    yy++;
                }
            }

            templateMaxY = yy + 1;
        }

        // Resize the background tint
        public void ResizeBackgroundTint(int x, int y) => backgroundTint.transform.localScale = new Vector2(x, y);

        // Used for switching the view between template and normal tiles
        public void ShowHideTemplate() {
            focusedOnTemplate = !focusedOnTemplate;

            Tilemaps[1].gameObject.SetActive(!focusedOnTemplate);
            tilemapFull.gameObject.SetActive(focusedOnTemplate);

            //Clear the selection square so it doesn't remain and look bad
            editor.tileSelectSquare.Clear();

            if (focusedOnTemplate) {
                editor.ClearSelection();
                //Save camera and set new
                previousCameraPosNormal = Camera.main.transform.position;
                Camera.main.GetComponent<EditorCam>().pos = previousCameraPosTemplate;
                //Resize the background tint for a better effect
                ResizeBackgroundTint(16, templateMaxY);
                //Set max cam sizes
                camMaxX = 16;
                camMaxY = templateMaxY;
            }
            else {
                //Set camera back
                previousCameraPosTemplate = Camera.main.transform.position;
                Camera.main.GetComponent<EditorCam>().pos = previousCameraPosNormal;
                //Resize background tint
                var lvl = Controller.obj.levelController.currentLevel.Maps[editor.currentMap];
                ResizeBackgroundTint(lvl.Width, lvl.Height);
                //Set max cam sizes
                camMaxX = lvl.Width;
                camMaxY = lvl.Height;
            }
        }

        // Converts mouse position to worldspace and then tile positions (1 = 16)
        public Vector3 MouseToTileCoords(Vector3 mousePos) {
            var worldMouse = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
            return new Vector3(Mathf.Floor(worldMouse.x), Mathf.Floor(worldMouse.y + 1), 10);
        }

        // Get one common tile at given position
        public Editor_MapTile GetTileAtPos(int x, int y) 
        {
            if (Controller.obj.levelController.currentLevel == null)
                return null;

            var map = Controller.obj.levelController.currentLevel.Maps[editor.currentMap];

            if (focusedOnTemplate)
            {
                // Get the 1-dimensional graphic tile index
                var graphicIndex1D = (y * 16) + x;

                if (graphicIndex1D > map.TileSet[0].Tiles.Length - 1)
                    graphicIndex1D = 0;

                Editor_MapTile t = new Editor_MapTile(new MapTile());

                t.Data.TileMapY = (ushort)Mathf.FloorToInt(graphicIndex1D / (float)map.TileSetWidth);
                t.Data.TileMapX = (ushort)(graphicIndex1D - (map.TileSetWidth * t.Data.TileMapY));

                return t;
            }

            return map.GetMapTile(x, y);
        }

        public void SetTileAtPos(int x, int y, Editor_MapTile newTile) 
        {
            var map = Controller.obj.levelController.currentLevel.Maps[editor.currentMap];

            // Update tile graphics
            Tilemaps[0].SetTile(new Vector3Int(x, y, 0), null);
            Tilemaps[1].SetTile(new Vector3Int(x, y, 0), null);
            Tilemaps[0].SetTile(new Vector3Int(x, y, 0), TypeCollisionTiles[(int)newTile.Data.CollisionType]);
            Tilemaps[1].SetTile(new Vector3Int(x, y, 0), map.GetTile(newTile, Controller.obj.levelController.EditorManager.Settings));

            // Get the tile to set
            var destTile = map.MapTiles[y * map.Width + x];

            // Update destination tile values
            destTile.Data.CollisionType = newTile.Data.CollisionType;
            destTile.PaletteIndex = newTile.PaletteIndex;
            destTile.Data.TileMapX = newTile.Data.TileMapX;
            destTile.Data.TileMapY = newTile.Data.TileMapY;
            destTile.Data.PC_Unk1 = newTile.Data.PC_Unk1;
            destTile.Data.PC_Unk2 = newTile.Data.PC_Unk2;

            // Get the correct transparency mode to set if available
            if (map.TileSetTransparencyModes != null)
                destTile.Data.PC_TransparencyMode = map.TileSetTransparencyModes[(map.TileSetWidth * newTile.Data.TileMapY) + newTile.Data.TileMapX];
        }

        public Editor_MapTile SetTypeAtPos(int x, int y, TileCollisionType collisionType) 
        {
            var map = Controller.obj.levelController.currentLevel.Maps[editor.currentMap];

            // Update tile graphics
            Tilemaps[0].SetTile(new Vector3Int(x, y, 0), null);
            Tilemaps[0].SetTile(new Vector3Int(x, y, 0), TypeCollisionTiles[(int)collisionType]);

            // Get the tile to set
            var destTile = map.MapTiles[y * map.Width + x];

            destTile.Data.CollisionType = collisionType;

            return destTile;
        }
    }
}
