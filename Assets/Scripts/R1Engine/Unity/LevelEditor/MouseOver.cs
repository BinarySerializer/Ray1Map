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

        void Update() {
            transform.position = mousePosition;
            Vector3 mousePositionTile = tilemapController.MouseToTileCoords(mousePosition);

            Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out var hit, 30);
            var e = hit.collider?.GetComponent<EventBehaviour>();
            // Mouse over event
            if (e != null) {
                textCollision.text = $"{e.ev.type}";
                textGraphic.text = $"Pos: {e.ev.pos.x}, {e.ev.pos.y}";
            }
            // Else Mouse over type
            else {

                Common_Tile t = tilemapController.GetTileAtPos((int)mousePositionTile.x, -(int)mousePositionTile.y);

                if (t != null) {
                    //Debug.Log("Tile here x:" + t.XPosition + " y:" + t.YPosition + " col:" + t.CollisionType);
                    textCollision.text = $"Collision: {t.CollisionType}";
                    textGraphic.text = $"Graphic tile: {t.TileSetGraphicIndex}";
                }
            }
        }
    }
}