using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Ray1;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static UnityEngine.Input;

namespace R1Engine
{
    public class LevelEditorBehaviour : MonoBehaviour {
        //Settings
        public int autoScrollMargin = 60;
        public float autoScrollSpeed = 5;
        //Colours for selections
        public Color colorSelect, colorNew, colorDelete;
        //References
        public LevelMainController lvlController;
        public LevelEventController lvlEventController;
        public LevelTilemapController lvlTilemapController;
        public SelectSquare tileSelectSquare;
        public ObjectHighlight objectHighlight;
        //Reference to UI buttons
        public Button[] modeButtons;
        public Button[] visibilityButtons;
        //Reference to everything that should be visible in each mode
        public GameObject[] modeContents;
        //Reference to layer buttons
        public GameObject layerTiles;
        public GameObject layerTypes;
        public GameObject layerEvents;

        //Current tile under the mouse
        public Unity_Tile mouseTile;

        bool selecting;
        bool dragging;
        [HideInInspector]
        public bool scrolling;
        [HideInInspector]
        public EditorCam cam;

        //Different edit modes to choose from
        public enum EditMode { Tiles, Collisions, Events, Links }
        //Current edit mode
        [HideInInspector]
        public EditMode currentMode;
        //Current type
        [HideInInspector]
        public TileCollisionType currentType;

        //Selected tiles
        public Unity_Tile[,] selection;

        private readonly List<Ray1MapEditorHistoryTile> TempPrevTileHistory = new List<Ray1MapEditorHistoryTile>();
        private readonly List<Ray1MapEditorHistoryTile> TempTileHistory = new List<Ray1MapEditorHistoryTile>();

        public void SetEditMode(int mode) {
            // Set
            currentMode = (EditMode)mode;
            //Change button visibility
            for(int i=0; i<modeButtons.Length; i++) {
                ColorBlock b = modeButtons[i].colors;
                if ((int)currentMode == i) {
                    b.normalColor = new Color(1,1,1);
                    modeContents[i].SetActive(true);
                }
                else {
                    b.normalColor = new Color(0.5f, 0.5f, 0.5f);
                    modeContents[i].SetActive(false);
                }
                modeButtons[i].colors = b;
            }

            //Show tile selection square and the preview in tiles and types modes
            tileSelectSquare.gameObject.SetActive(currentMode == EditMode.Tiles || currentMode == EditMode.Collisions);
            lvlTilemapController.tilemapPreview.gameObject.SetActive(currentMode == EditMode.Tiles);
            
            if (currentMode != EditMode.Tiles) {
                if (lvlTilemapController.focusedOnTemplate) {
                    lvlTilemapController.ShowHideTemplate();
                }
            }
        }

        public void SetLayerVisibility(int index) {
            switch (index) {
                case 0: Settings.ShowTiles = !Settings.ShowTiles; break;
                case 1: Settings.ShowCollision = !Settings.ShowCollision; break;
                case 2: Settings.ShowObjects = !Settings.ShowObjects; break;
                case 3: Settings.ShowLinks = !Settings.ShowLinks; break;
            }
        }

        private void ChangeVisibButton(int which) {
            bool on = false;
            switch (which) {
                case 0: on = Settings.ShowTiles; break;
                case 1: on = Settings.ShowCollision; break;
                case 2: on = Settings.ShowObjects; break;
                case 3: on = Settings.ShowLinks; break;
            }

            ColorBlock b = visibilityButtons[which].colors;
            if (on) {
                b.normalColor = new Color(1, 1, 1);
            }
            else {
                b.normalColor = new Color(0.5f, 0.5f, 0.5f);
            }
            visibilityButtons[which].colors = b;
        }

        /*
        void SetAlpha(float visAlpha, float colAlpha) {
            foreach (var tm in lvlController.controllerTilemap.Tilemaps)
                if (tm.name == "TilemapTypes")
                    tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, colAlpha);
                else tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, visAlpha);
        }

        public EditMode mode { get => _mode; set {
                if (_mode == value) return;
                _mode = value;

                // Set display transparencies for different edit modes
                switch (value) {
                    case EditMode.Tiles:
                    case EditMode.Events:
                    case EditMode.Links:
                        SetAlpha(1, 0); break;
                    case EditMode.Collisions:
                        SetAlpha(0.35f, 0.9f); break;
                }

                tileSelectSquare.Clear();
            } }
        */

        void Awake() {
            cam = Camera.main.GetComponent<EditorCam>();
        }

        void Start() {
            //Default to events
            SetEditMode(2);
        }

        public void SetCurrentType(int type)
        {
            currentType = (TileCollisionType)type;
        }

        public void ClearSelection() {
            selection = null;
            tileSelectSquare.Clear();
            if (currentMode == EditMode.Tiles)
                lvlTilemapController.ClearPreviewTilemap();
        }

        void Update() 
        {
            //Take care of showing and hiding tiles, types and events -layers
            if (Settings.ShowTiles != layerTiles.activeSelf) {
                layerTiles.SetActive(!layerTiles.activeSelf);
                ChangeVisibButton(0);
            }
            if (Settings.ShowCollision != layerTypes.activeSelf) {
                layerTypes.SetActive(!layerTypes.activeSelf);
                ChangeVisibButton(1);
            }
            if (lvlTilemapController?.IsometricCollision != null) {
                if (Settings.ShowCollision != lvlTilemapController.IsometricCollision.activeSelf) {
                    lvlTilemapController.IsometricCollision.SetActive(Settings.ShowCollision);
                }
            }
            if (lvlTilemapController?.collisionLinesGraphics != null) {
                if (Settings.ShowCollision != lvlTilemapController.collisionLinesGraphics.activeSelf) {
                    lvlTilemapController.collisionLinesGraphics.SetActive(Settings.ShowCollision);
                }
            }
            if (Settings.ShowObjects != layerEvents.activeSelf) {
                layerEvents.SetActive(!layerEvents.activeSelf);
                ChangeVisibButton(2);
            }
            ChangeVisibButton(3);

            if (Controller.LoadState != Controller.State.Finished || LevelEditorData.Level == null) return;

            int layerIndex = LevelEditorData.CurrentLayer;
            var layer = LevelEditorData.Level.Layers?.ElementAtOrDefault(layerIndex) as Unity_Layer_Map;

            //Tile editing
            if (layer != null && (currentMode == EditMode.Tiles || currentMode == EditMode.Collisions)) {
                var map = layer.Map;
                // Get the tile under the mouse
                Vector3 mousePositionTile = lvlController.controllerTilemap.MouseToTileCoords(mousePosition);
                Vector2Int mouseTileInt = lvlController.controllerTilemap.MouseToTileInt(mousePosition);
                mouseTile = lvlController.controllerTilemap.GetTileAtPos(mouseTileInt.x, mouseTileInt.y);

                // =============== SELECTION SQUARE ===============

                // Escape clears selection info
                if (GetKeyDown(KeyCode.Escape))
                    ClearSelection();

                // Left click begins drag and assigns the starting corner of the selection square
                if (!dragging && mouseTile != null) {
                    if (!EventSystem.current.IsPointerOverGameObject()) {
                        if (GetMouseButtonDown(0)) {
                            tileSelectSquare.SetStartCorner(mouseTileInt.x, mouseTileInt.y);
                            dragging = true;
                            selecting = true;
                            //Clear old preview tilemap
                            if (currentMode == EditMode.Tiles)
                                lvlTilemapController.ClearPreviewTilemap();
                        }
                        else if (GetMouseButtonDown(1)) {
                            tileSelectSquare.SetStartCorner(mouseTileInt.x, mouseTileInt.y);
                            dragging = true;
                            selecting = false;
                        }
                    }
                }

                if (dragging) {
                    // During drag, set the end corner
                    var endX = Mathf.Clamp(mouseTileInt.x, 0, ((int)lvlTilemapController.TilemapBounds.xMax - 1));
                    var endY = Mathf.Clamp(mouseTileInt.y, 0, ((int)lvlTilemapController.TilemapBounds.yMax - 1));
                    if (selecting) {
                        tileSelectSquare.color = colorSelect;
                        tileSelectSquare.SetEndCorner(endX, endY);
                    } else {

                        tileSelectSquare.color = colorNew;
                        tileSelectSquare.SetEndCorner(endX, endY);

                        if (currentMode == EditMode.Tiles) 
                        {
                            // Change preview position
                            lvlTilemapController.tilemapPreview.transform.position = new Vector3(tileSelectSquare.XStart * lvlTilemapController.CellSizeInUnits, -(tileSelectSquare.YStart * lvlTilemapController.CellSizeInUnits));

                            // Expand the preview tiles
                            if (selection != null) {
                                lvlTilemapController.SetPreviewTilemap(map, selection);
                            }
                        }
                    }

                    // Auto scroll if dragging near the screen margin and not manually moving the camera
                    if (GetMouseButton(1)) {
                        float scr = Camera.main.orthographicSize * cam.friction * autoScrollSpeed * Time.deltaTime;
                        bool inMarginLeft = mousePosition.x < autoScrollMargin;
                        bool inMarginRight = mousePosition.x > Screen.width - autoScrollMargin;
                        bool inMarginTop = mousePosition.y < autoScrollMargin;
                        bool inMarginBottom = mousePosition.y > Screen.height - autoScrollMargin;
                        bool inMargin = inMarginLeft || inMarginRight || inMarginTop || inMarginBottom;

                        if (inMarginLeft) cam.vel.x -= scr;
                        if (inMarginRight) cam.vel.x += scr;
                        if (inMarginTop) cam.vel.y -= scr;
                        if (inMarginBottom) cam.vel.y += scr;

                        if (inMargin)
                            scrolling = true;
                    }
                }
                else {
                    scrolling = false;
                    //Move preview with mouse if not dragging
                    lvlTilemapController.tilemapPreview.transform.position = mousePositionTile;
                }

                // Reset camera friction from the drag's override if moved manually
                if (GetMouseButtonDown(1))
                    cam.friction = cam.fricStart;

                if (dragging) 
                {
                    if (currentMode == EditMode.Tiles) 
                    {
                        // If dragging and selecting mouse up, record the selection
                        if (selecting && !GetMouseButton(0)) {
                            dragging = false;

                            // Create array for selected area
                            selection = new Unity_Tile[(int)(tileSelectSquare.XEnd - tileSelectSquare.XStart) + 1, (int)(tileSelectSquare.YEnd - tileSelectSquare.YStart) + 1];

                            int xi = 0;
                            int yi = 0;

                            // Save the selected area
                            //print(tileSelectSquare.XStart + " - " + tileSelectSquare.XEnd + " - " + tileSelectSquare.YStart + " - " + tileSelectSquare.YEnd);
                            for (int y = (int)tileSelectSquare.YStart; y <= tileSelectSquare.YEnd; y++) 
                            {
                                for (int x = (int)tileSelectSquare.XStart; x <= tileSelectSquare.XEnd; x++) 
                                {
                                    var t = lvlController.controllerTilemap.GetTileAtPos(x, y);
                                    selection[xi, yi] = t;

                                    xi++;
                                }
                                xi = 0;
                                yi++;
                            }
                            if (currentMode == EditMode.Tiles) {
                                lvlTilemapController.SetPreviewTilemap(map, selection);
                            }
                        }
                        // If dragging and painting mouse up, set the selection
                        if (!selecting && GetMouseButtonUp(1)) {
                            dragging = false;
                            if (selection != null) {
                                if (!lvlTilemapController.focusedOnTemplate) {
                                    int xi = 0;
                                    int yi = 0;
                                    //"Paste" the selection
                                    for (int y = (int)tileSelectSquare.YStart; y <= tileSelectSquare.YEnd; y++) {
                                        for (int x = (int)tileSelectSquare.XStart; x <= tileSelectSquare.XEnd; x++) {
                                            var t = map.GetMapTile(x, y);
                                            TempPrevTileHistory.Add(new Ray1MapEditorHistoryTile(t.CloneObj(), x, y, layerIndex));

                                            var selectionTile = selection[xi, yi];
                                            lvlController.controllerTilemap.SetTileAtPos(layerIndex, x, y, selectionTile);

                                            t.HasPendingEdits = true;

                                            TempTileHistory.Add(new Ray1MapEditorHistoryTile(selectionTile.CloneObj(), x, y, layerIndex));

                                            xi++;
                                            if (xi >= selection.GetLength(0))
                                                xi = 0;
                                        }
                                        xi = 0;
                                        yi++;
                                        if (yi >= selection.GetLength(1))
                                            yi = 0;
                                    }
                                }
                                // Cut the preview back to the original size since it's been expanded
                                if (currentMode == EditMode.Tiles) {
                                    lvlTilemapController.SetPreviewTilemap(map, selection);
                                }
                            }
                        }
                    }
                    else if (currentMode == EditMode.Collisions) 
                    {
                        //Fill with selected type
                        if (selecting && !GetMouseButton(0))
                        {
                            dragging = false;
                            //Paste the current type
                            for (int y = (int)tileSelectSquare.YStart; y <= tileSelectSquare.YEnd; y++) {
                                for (int x = (int)tileSelectSquare.XStart; x <= tileSelectSquare.XEnd; x++) {
                                    var t = map.GetMapTile(x, y);
                                    TempPrevTileHistory.Add(new Ray1MapEditorHistoryTile(t.CloneObj(), x, y, layerIndex));

                                    var tile = lvlController.controllerTilemap.SetTypeAtPos(layerIndex, x, y, (ushort)currentType);

                                    t.HasPendingEdits = true;

                                    TempTileHistory.Add(new Ray1MapEditorHistoryTile(tile.CloneObj(), x, y, layerIndex));
                                }
                            }
                        }
                        //Fill with empty
                        if (!selecting && GetMouseButtonUp(1)) {
                            dragging = false;

                            for (int y = (int)tileSelectSquare.YStart; y <= tileSelectSquare.YEnd; y++) {
                                for (int x = (int)tileSelectSquare.XStart; x <= tileSelectSquare.XEnd; x++) {
                                    var t = map.GetMapTile(x, y);
                                    TempPrevTileHistory.Add(new Ray1MapEditorHistoryTile(t.CloneObj(), x, y, layerIndex));

                                    var tile = lvlController.controllerTilemap.SetTypeAtPos(layerIndex, x, y, 0);
                                    t.HasPendingEdits = true;

                                    TempTileHistory.Add(new Ray1MapEditorHistoryTile(tile.CloneObj(), x, y, layerIndex));
                                }
                            }
                        }
                    }
                }

                // Stamp selection with ctrl+v
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V) && !dragging) {
                    if (selection != null && !lvlTilemapController.focusedOnTemplate) {
                        int xi = 0;
                        int yi = 0;
                        int w = map.Width;
                        int h = map.Height;
                        int my = mouseTileInt.y;
                        int mx = mouseTileInt.x;
                        // "Paste" the selection
                        for (int y = my; y <= my+selection.GetLength(1)-1; y++) {
                            for (int x = mx; x <= mx+selection.GetLength(0)-1; x++) {
                                
                                if (x>=0 && y>=0 && x<w && y<h)
                                {
                                    var t = map.GetMapTile(x, y);
                                    TempPrevTileHistory.Add(new Ray1MapEditorHistoryTile(t.CloneObj(), x, y, layerIndex));

                                    //lvlController.controllerTilemap.SetTileAtPos(x, y, selection[xi, yi]);
                                    
                                    t.HasPendingEdits = true;

                                    TempTileHistory.Add(new Ray1MapEditorHistoryTile(selection[xi, yi].CloneObj(), x, y, layerIndex));
                                }

                                xi++;
                            }
                            xi = 0;
                            yi++;
                        }
                        lvlController.controllerTilemap.SetTileBlockAtPos(layerIndex, mx, my, selection.GetLength(0), selection.GetLength(1), selection);
                    }
                }

                // Check if the editor actions should be undone/redone
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                        // Redo
                        Controller.obj.levelController.History.Redo();
                    else
                        // Undo
                        Controller.obj.levelController.History.Undo();
                }

                // Add to history if any tiles were added
                if (TempTileHistory.Any())
                {
                    // Add to history
                    Controller.obj.levelController.History.AddToHistory(new EditorHistoryEntry<Ray1MapEditorHistoryItem>(new Ray1MapEditorHistoryItem(TempPrevTileHistory.ToArray()), new Ray1MapEditorHistoryItem(TempTileHistory.ToArray())));

                    // Clear temp lists
                    TempTileHistory.Clear();
                    TempPrevTileHistory.Clear();
                }
            }
        }
    }
}