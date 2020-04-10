using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static UnityEngine.Input;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

namespace R1Engine {
    public class Editor : MonoBehaviour {
        //Settings
        public int autoScrollMargin = 60;
        public float autoScrollSpeed = 5;
        //Colours for selections
        public Color colorSelect, colorNew, colorDelete;
        //References
        public LevelMainController lvlController;
        public LevelEventController lvlEventController;
        public SelectSquare tileSelectSquare;

        public Common_Tile mouseTile;

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

        //Reference to UI buttons
        public Button[] modeButtons;
        //Reference to everything that should be visible in each mode
        public GameObject[] modeContents;
        //Reference to layer buttons
        public GameObject layerTypes;
        public GameObject layerEvents;

        //Selected tiles
        public Common_Tile[,] selection;

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

            //What to show/hide with each mode
            if (currentMode == EditMode.Collisions) {
                layerTypes.SetActive(true);
                layerEvents.SetActive(false);
                lvlEventController.ToggleLinks(false);
            }
            else if (currentMode == EditMode.Events) {
                layerTypes.SetActive(false);
                layerEvents.SetActive(true);
                lvlEventController.ToggleLinks(false);
            }
            else if (currentMode == EditMode.Links) {
                layerTypes.SetActive(false);
                layerEvents.SetActive(true);
                lvlEventController.ToggleLinks(true);
            }
            else if (currentMode == EditMode.Tiles) {
                layerTypes.SetActive(false);
                layerEvents.SetActive(false);
                lvlEventController.ToggleLinks(false);
            }
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

        void Update() {
            //Tile editing
            if (currentMode == EditMode.Tiles || currentMode == EditMode.Collisions) {
                // Get the tile under the mouse
                Vector3 mousePositionTile = lvlController.controllerTilemap.MouseToTileCoords(mousePosition);
                mouseTile = lvlController.controllerTilemap.GetTileAtPos((int)mousePositionTile.x, -(int)mousePositionTile.y);

                // =============== SELECTION SQUARE ===============

                // Escape clears selection info
                if (GetKeyDown(KeyCode.Escape))
                    tileSelectSquare.Clear();

                // Left click begins drag and assigns the starting corner of the selection square
                if (!dragging && mouseTile != null) {
                    if (GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                        tileSelectSquare.SetStartCorner(mousePositionTile.x, -mousePositionTile.y);
                        dragging = true;
                        selecting = !GetKey(KeyCode.LeftControl);
                    }
                }

                if (dragging) {
                    // During drag, set the end corner
                    var endX = Mathf.Clamp(mousePositionTile.x, 0, lvlController.currentLevel.Width - 1);
                    var endY = Mathf.Clamp(-mousePositionTile.y, 0, lvlController.currentLevel.Height - 1);
                    if (selecting) {
                        tileSelectSquare.color = colorSelect;
                        tileSelectSquare.SetEndCorner(endX, endY);
                    } else {
                        tileSelectSquare.color = colorNew;
                        tileSelectSquare.SetEndCorner(endX, endY);
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
                }

                // Reset camera friction from the drag's override if moved manually
                if (GetMouseButtonDown(1))
                    cam.friction = cam.fricStart;

                if (dragging) {
                    if (currentMode == EditMode.Tiles) {
                        // If dragging and selecting mouse up, record the selection
                        if (selecting && !GetMouseButton(0)) {
                            dragging = false;
                            //Create array for selected area
                            selection = new Common_Tile[(int)(tileSelectSquare.XEnd - tileSelectSquare.XStart) + 1, (int)(tileSelectSquare.YEnd - tileSelectSquare.YStart) + 1];
                            int xi = 0;
                            int yi = 0;
                            //Save the selected area
                            for (int y = (int)tileSelectSquare.YStart; y <= tileSelectSquare.YEnd; y++) {
                                for (int x = (int)tileSelectSquare.XStart; x <= tileSelectSquare.XEnd; x++) {
                                    selection[xi, yi] = lvlController.controllerTilemap.GetTileAtPos(x, y);
                                    xi++;
                                }
                                xi = 0;
                                yi++;
                            }
                        }
                        // If dragging and painting mouse up, set the selection
                        if (!selecting && GetMouseButtonUp(0)) {
                            dragging = false;
                            int xi = 0;
                            int yi = 0;
                            //"Paste" the selection
                            for (int y = (int)tileSelectSquare.YStart; y <= tileSelectSquare.YEnd; y++) {
                                for (int x = (int)tileSelectSquare.XStart; x <= tileSelectSquare.XEnd; x++) {
                                    lvlController.controllerTilemap.SetTileAtPos(x, y, selection[xi, yi]);
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
                    }
                    else if (currentMode == EditMode.Collisions) {

                    }
                }
            }
        }
    }
}