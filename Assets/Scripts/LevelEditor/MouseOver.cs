using System;
using System.Linq;
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
            Vector2Int mouseTile = tilemapController.MouseToTileInt(mousePosition);

            //Physics2D.Raycast(Camera.main.ScreenPointToRay(mousePosition), out var hit, 30);

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero);

            var e = hit.collider?.GetComponentInParent<Unity_ObjBehaviour>();
            // Mouse over event
            if (e != null) {
                
                textCollision.text = $"{e.ObjData.DisplayName}";
                textGraphic.text = $"Pos: {e.ObjData.XPosition}, {e.ObjData.YPosition}{Environment.NewLine}" +
                                   $"Offsets: {e.ObjData.Pivot.x} x {e.ObjData.Pivot.y}";

                // Set debug text
                Controller.obj.tempDebugText.text = Settings.ShowDebugInfo 
                    ? $"{e.ObjData.DebugText}{Environment.NewLine}" +
                      $"CurrentFrame: {e.ObjData.CurrentAnimationFrame}{Environment.NewLine}" +
                      $"Frames: {e.ObjData.CurrentAnimation?.Frames?.GetLength(0)}{Environment.NewLine}" +
                      $"{Environment.NewLine}" +
                      $"IsAlways: {e.ObjData.IsAlways}{Environment.NewLine}" +
                      $"IsEditor: {e.ObjData.IsEditor}{Environment.NewLine}" +
                      $"IsActive: {e.ObjData.IsActive}{Environment.NewLine}" +
                      $"IsDisabled: {e.ObjData.IsDisabled}{Environment.NewLine}" +
                      $"IsVisible: {e.ObjData.IsVisible}{Environment.NewLine}" +
                      $"Layer: {e.Layer}{Environment.NewLine}" +
                      $"{Environment.NewLine}" +
                      $"LinkID: {e.ObjData.EditorLinkGroup}{Environment.NewLine}"
                    : String.Empty;
            }
            // Else Mouse over type
            else {
                Controller.obj.tempDebugText.text = String.Empty;
                var t = LevelEditorData.Level?.Maps?.ElementAtOrDefault(LevelEditorData.CurrentMap)?.GetMapTile(mouseTile.x, mouseTile.y);
                var c = LevelEditorData.Level?.Maps?.ElementAtOrDefault(LevelEditorData.CurrentCollisionMap)?.GetMapTile(mouseTile.x, mouseTile.y);

                if (t != null && c != null) {
                    //Debug.Log("Tile here x:" + t.XPosition + " y:" + t.YPosition + " col:" + t.CollisionType);
                    textCollision.text = $"Collision: {c.Data.CollisionType}";
                    textGraphic.text = $"Graphic tile: {t.Data.TileMapX}, {t.Data.TileMapY}";

                    // Set debug text
                    Controller.obj.tempDebugText.text = Settings.ShowDebugInfo
                        ? $"{t.DebugText}{Environment.NewLine}" +
                          $"Collision: {c.Data.CollisionType}{Environment.NewLine}" +
                          $"PC_TransparencyMode: {t.Data.PC_TransparencyMode}{Environment.NewLine}" +
                          $"PC_Unk1: {t.Data.PC_Unk1}{Environment.NewLine}" +
                          $"PC_Unk2: {t.Data.PC_Unk2}{Environment.NewLine}" +
                          $"HorizontalFlip: {t.Data.HorizontalFlip}{Environment.NewLine}" +
                          $"VerticalFlip: {t.Data.VerticalFlip}{Environment.NewLine}" +
                          $"PaletteIndex: {t.PaletteIndex}{Environment.NewLine}"
                        : String.Empty;
                }
            }
        }
    }
}