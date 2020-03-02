using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static UnityEngine.Input;

namespace R1Engine.Unity {
    public class TileEditor : MonoBehaviour {
        public Color colorSelect, colorNew, colorDelete;
        public LevelMainController lvlController;
        public SelectSquare tileSelectSquare;
        public int autoScrollMargin = 60;
        public float autoScrollSpeed = 5;

        Common_Lev lvl => lvlController.currentLevel;
        public Common_Tile mouseTile;

        bool dragging;
        Vector2Int selStart, selEnd;
        public int xStart, yStart, xEnd, yEnd;
        float fricPrev;

        EditorCam cam;

        void Awake() {
            cam = Camera.main.GetComponent<EditorCam>();
            fricPrev = cam.friction;
        }


        void ApplyToSelection(Action action) {
            for (int y = tileSelectSquare.ys; y <= tileSelectSquare.ye; y++)
                for (int x = tileSelectSquare.xs; x <= tileSelectSquare.xe; x++) {

                }
        }


        void Update() {

            // Get current mouse tile
            mouseTile = lvlController.controllerTilemap.GetMouseTile();


            // Left click begins drag and assigns the starting corner of the selection square
            if (GetMouseButtonDown(0) && mousePosition.y < Screen.height - 60) {
                tileSelectSquare.xStart = mouseTile.XPosition;
                tileSelectSquare.yStart = mouseTile.YPosition;
                dragging = true;
                fricPrev = cam.friction;
            }

            if (dragging) {
                // During drag, set the end corner
                if (GetMouseButton(0)) {
                    tileSelectSquare.xEnd = mouseTile.XPosition;
                    tileSelectSquare.yEnd = mouseTile.YPosition;
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
                        cam.friction = 25;
                }
            }

            // Reset camera friction from the drag's override if moved manually
            if (GetMouseButtonDown(1))
                cam.friction = fricPrev;

            // On mouse up, if dragging, stop drag and finalise the end corner
            if (dragging && !GetMouseButton(0)) {
                tileSelectSquare.xEnd = mouseTile.XPosition;
                tileSelectSquare.yEnd = mouseTile.YPosition;
                dragging = false;
            }



            // Press Delete to clear selected tiles
            if (GetKeyDown(KeyCode.Delete))
                for (int y = tileSelectSquare.ys; y <= tileSelectSquare.ye; y++)
                    for (int x = tileSelectSquare.xs; x <= tileSelectSquare.xe; x++) {
                        lvlController.controllerTilemap.SetTileAtPos(x, y, 0, 0);
                        lvlController.controllerTilemap.SetTypeAtPos(x, y, 0);
                    }
        }
    }
}