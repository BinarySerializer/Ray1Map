using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Input;

namespace R1Engine.Unity {
    public class MouseOver : MonoBehaviour {
        public Text textGraphic, textCollision;

        // Reference to level's tilemap controller
        public LevelTilemapController tilemapController;

        // Reference to the tile square cursor
        public Transform tileSquare;

        void Update() {
            transform.position = mousePosition;
            Vector3 mousePositionTile = MouseToTileCoords(mousePosition);

            // Update square tile cursor
            tileSquare.transform.position = new Vector3(mousePositionTile.x,mousePositionTile.y,10);

            Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out var hit, 30);
            var e = hit.collider?.GetComponent<EventBehaviour>();
            // Mouse over event
            if (e != null) {
                textCollision.text = $"{e.ev.type}";
                textGraphic.text = $"Pos: {e.ev.pos.x}, {e.ev.pos.y}";
            }
            // Else Mouse over type
            else {

                Common_Tile t = tilemapController.GetTileAtPos(0, (int)mousePositionTile.x, -(int)mousePositionTile.y);

                if (t != null) {
                    //Debug.Log("Tile here x:" + t.XPosition + " y:" + t.YPosition + " col:" + t.CollisionType);
                    textCollision.text = $"Collision: {t.CollisionType}";
                    textGraphic.text = $"Graphic tile: {t.TileSetGraphicIndex}";
                }
            }
        }

        // Converts mouse position to worldspace and then tile positions (1 = 16)
        private Vector3 MouseToTileCoords(Vector3 mousePos) {
            var worldMouse = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
            return new Vector3(Mathf.Floor(worldMouse.x), Mathf.Floor(worldMouse.y + 1), 10);
        }
    }
}