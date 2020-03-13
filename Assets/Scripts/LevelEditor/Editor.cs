using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static UnityEngine.Input;
using UnityEngine.Tilemaps;

namespace R1Engine {
    public class Editor : MonoBehaviour {
        //Settings
        public int autoScrollMargin = 60;
        public float autoScrollSpeed = 5;
        //Colours for selections
        public Color colorSelect, colorNew, colorDelete;
        //References
        public LevelMainController lvlController;
        public SelectSquare tileSelectSquare;

        public Common_Tile mouseTile;


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
            //Special cases
            if (currentMode == EditMode.Collisions) {
                layerTypes.SetActive(true);
                layerEvents.SetActive(false);
            }
            else if (currentMode == EditMode.Events || currentMode == EditMode.Links) {
                layerTypes.SetActive(false);
                layerEvents.SetActive(true);
            }
            else if (currentMode == EditMode.Tiles) {
                layerTypes.SetActive(false);
                layerEvents.SetActive(false);
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

            // Get the tile under the mouse
            mouseTile = lvlController.controllerTilemap.GetMouseTile();



            // =============== SELECTION SQUARE ===============

            // Escape clears selection info
            if (GetKeyDown(KeyCode.Escape))
                tileSelectSquare.Clear();


            // Left click begins drag and assigns the starting corner of the selection square
            if (GetMouseButtonDown(0) && mousePosition.y < Screen.height - 100 && mousePosition.x > 250) {
                tileSelectSquare.SetStartCorner(mouseTile.XPosition, mouseTile.YPosition);
                dragging = true;
            }

            if (dragging) {
                // During drag, set the end corner
                if (GetMouseButton(0)) {
                    tileSelectSquare.SetEndCorner(mouseTile.XPosition, mouseTile.YPosition);
                }

                tileSelectSquare.color = colorSelect;

                // Auto scroll if dragging near the screen margin and not manually moving the camera
                if (!GetMouseButton(1)) {
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
            else
                scrolling = false;

            // Reset camera friction from the drag's override if moved manually
            if (GetMouseButtonDown(1))
                cam.friction = cam.fricStart;

            // On mouse up, if dragging, stop drag and finalise the end corner
            if (dragging && !GetMouseButton(0)) {
                tileSelectSquare.SetEndCorner(mouseTile.XPosition, mouseTile.YPosition);
                dragging = false;
            }



            /*
            switch (mode) {

                // =============== Tiles EDITING ===============
                case EditMode.Tiles:

                    // Press Delete to clear selected tiles
                    if (GetKeyDown(KeyCode.Delete))
                        for (int y = (int)tileSelectSquare.YStart; y <= tileSelectSquare.YEnd; y++)
                            for (int x = (int)tileSelectSquare.XStart; x <= tileSelectSquare.XEnd; x++) {
                                lvlController.controllerTilemap.SetTileAtPos(x, y, 0, 0);
                                lvlController.controllerTilemap.SetTypeAtPos(x, y, 0);
                            }
                    break;

            }*/
        }
    }
}