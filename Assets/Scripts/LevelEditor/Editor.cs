using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static UnityEngine.Input;
using UnityEngine.Tilemaps;

namespace R1Engine {
    public class Editor : MonoBehaviour {
        public int autoScrollMargin = 60;
        public float autoScrollSpeed = 5;
        public Color colorSelect, colorNew, colorDelete;
        public LevelMainController lvlController;
        public SelectSquare tileSelectSquare;

        public Common_Tile mouseTile;

        bool dragging;
        [HideInInspector] public bool scrolling;

        EditorCam cam;


        public enum EditMode { Graphics, Collision, Events, Linking }
        EditMode _mode;
        public void SetEditMode(int mode) {
            this.mode = (EditMode)mode;
        }

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
                    case EditMode.Graphics:
                    case EditMode.Events:
                    case EditMode.Linking:
                        SetAlpha(1, 0); break;
                    case EditMode.Collision:
                        SetAlpha(0.35f, 0.9f); break;
                }

                tileSelectSquare.Clear();
            } }


        void Awake() {
            cam = Camera.main.GetComponent<EditorCam>();
        }

        void Start() {
            mode = EditMode.Graphics;
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




            switch (mode) {

                // =============== GRAPHICS EDITING ===============
                case EditMode.Graphics:

                    // Press Delete to clear selected tiles
                    if (GetKeyDown(KeyCode.Delete))
                        for (int y = (int)tileSelectSquare.YStart; y <= tileSelectSquare.YEnd; y++)
                            for (int x = (int)tileSelectSquare.XStart; x <= tileSelectSquare.XEnd; x++) {
                                lvlController.controllerTilemap.SetTileAtPos(x, y, 0, 0);
                                lvlController.controllerTilemap.SetTypeAtPos(x, y, 0);
                            }
                    break;

            }
        }
    }
}