using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace R1Engine
{
    public class MouseOverUI : MonoBehaviour {
        public Text textGraphic, textCollision;

        void Update() {
            if (Controller.LoadState != Controller.State.Finished) return;
            transform.position = Input.mousePosition;

            var selector = Controller.obj.levelController.editor.objectHighlight;
            var e = selector?.highlightedObject;
            var t = selector?.highlightedTile;
            var c = selector?.highlightedCollision;

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