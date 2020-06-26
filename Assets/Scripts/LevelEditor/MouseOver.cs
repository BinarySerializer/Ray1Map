using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Input;

namespace R1Engine
{
    public class MouseOver : MonoBehaviour {
        public Text textGraphic, textCollision;

        // Reference to level's tilemap controller
        public LevelTilemapController tilemapController;

        void Update() {
            transform.position = mousePosition;
            Vector3 mousePositionTile = tilemapController.MouseToTileCoords(mousePosition);

            //Physics2D.Raycast(Camera.main.ScreenPointToRay(mousePosition), out var hit, 30);

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero);

            var e = hit.collider?.GetComponentInParent<Common_Event>();
            // Mouse over event
            if (e != null) {
                textCollision.text = $"{e.DisplayName}";
                textGraphic.text = $"Type: {e.Data.TypeValue}{Environment.NewLine}" +
                                   $"Pos: {e.Data.EventData.XPosition}, {e.Data.EventData.YPosition}{Environment.NewLine}" +
                                   $"Offsets: {e.Data.EventData.OffsetBX} x {e.Data.EventData.OffsetBY} x {e.Data.EventData.OffsetHY}";

                // Set debug text
                Controller.obj.tempDebugText.text = Settings.ShowDebugInfo 
                    ? $"{e.Data.DebugText}{Environment.NewLine}" +
                      $"CurrentFrame: {(int)e.Data.EventData.RuntimeCurrentAnimFrame}{Environment.NewLine}" +
                      $"Frames: {e.CurrentAnimation?.Frames?.GetLength(0)}{Environment.NewLine}" +
                      $"AnimationIndex: {e.Data.EventData.RuntimeCurrentAnimIndex}{Environment.NewLine}" +
                      $"AnimationSpeed: {e.AnimSpeed}{Environment.NewLine}" +
                      $"Sound: {e.State?.SoundIndex}{Environment.NewLine}" +
                      $"Flag: {e.Data.TypeInfo?.Flag}{Environment.NewLine}" +
                      $"Etat: {e.Data.EventData.RuntimeEtat}{Environment.NewLine}" +
                      $"SubEtat: {e.Data.EventData.RuntimeSubEtat}{Environment.NewLine}"
                    : String.Empty;
            }
            // Else Mouse over type
            else {
                Controller.obj.tempDebugText.text = String.Empty;
                Common_Tile t = tilemapController.GetTileAtPos((int)mousePositionTile.x, -(int)mousePositionTile.y);

                if (t != null) {
                    //Debug.Log("Tile here x:" + t.XPosition + " y:" + t.YPosition + " col:" + t.CollisionType);
                    textCollision.text = $"Collision: {t.CollisionType}";
                    textGraphic.text = $"Graphic tile: {t.TileSetGraphicIndex}";

                    // Set debug text
                    Controller.obj.tempDebugText.text = Settings.ShowDebugInfo
                        ? $"{t.DebugText}{Environment.NewLine}" +
                          $"Collision: {t.CollisionType}{Environment.NewLine}" +
                          $"TileIndex: {t.TileSetGraphicIndex}{Environment.NewLine}" +
                          $"PaletteIndex: {t.PaletteIndex}{Environment.NewLine}"
                        : String.Empty;
                }
            }
        }
    }
}