using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace R1Engine
{
    public class MouseOverUI : MonoBehaviour {
        public Text textGraphic, textCollision;
        public GameObject panel;

        void Update() {
            if (Controller.LoadState != Controller.State.Finished) return;
            transform.position = Input.mousePosition;

            var selector = Controller.obj.levelController.editor.objectHighlight;
            bool freeLook = Controller.obj.levelController.editor.cam.FreeLookMode;
            if (freeLook) {
                if(panel.activeSelf) panel.SetActive(false);
            } else {
                if (!panel.activeSelf) panel.SetActive(true);
            }
            var e = selector?.highlightedObject;
            var t = selector?.highlightedTile;
            var c = selector?.highlightedCollision;

            // Mouse over event
            if (e != null) {
                
                textCollision.text = $"{e.ObjData.PrimaryName} | {e.ObjData.SecondaryName}";
                textGraphic.text = $"Pos: ({e.ObjData.XPosition}, {e.ObjData.YPosition}){Environment.NewLine}" +
                                   $"Pivot: ({e.ObjData.Pivot.x}, {e.ObjData.Pivot.y})";

                // Set debug text
                Controller.obj.tempDebugText.text = Settings.ShowDebugInfo 
                    ? $"{e.ObjData.DebugText}{Environment.NewLine}" +
                      $"{Environment.NewLine}" +
                      $"{nameof(e.ObjData.AnimationIndex)}: {e.ObjData.AnimationIndex}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.AnimationFrame)}: {e.ObjData.AnimationFrame}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.AnimSpeed)}: {e.ObjData.AnimSpeed}{Environment.NewLine}" +
                      $"Frames: {e.ObjData.CurrentAnimation?.Frames?.GetLength(0)}{Environment.NewLine}" +
                      $"{Environment.NewLine}" +
                      $"{nameof(e.ObjData.IsAlways)}: {e.ObjData.IsAlways}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.IsEditor)}: {e.ObjData.IsEditor}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.IsActive)}: {e.ObjData.IsActive}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.IsDisabled)}: {e.ObjData.IsDisabled}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.IsVisible)}: {e.ObjData.IsVisible}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.MapLayer)}: {e.ObjData.MapLayer}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.Scale)}: {e.ObjData.Scale}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.FlipHorizontally)}: {e.ObjData.FlipHorizontally}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.FlipVertically)}: {e.ObjData.FlipVertically}{Environment.NewLine}" +
                      $"{Environment.NewLine}" +
                      $"{nameof(e.Index)}: {e.Index}{Environment.NewLine}" +
                      $"{nameof(e.Layer)}: {e.Layer}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.EditorLinkGroup)}: {e.ObjData.EditorLinkGroup}{Environment.NewLine}" 
                    : String.Empty;
            }
            // Else Mouse over type
            else {
                Controller.obj.tempDebugText.text = String.Empty;

                if (t != null && c != null) {
                    //Debug.Log("Tile here x:" + t.XPosition + " y:" + t.YPosition + " col:" + t.CollisionType);
                    textCollision.text = $"Collision: {String.Join(", ", c.Select(x => x?.Data?.CollisionType))}";
                    textGraphic.text = $"Graphic tile: {String.Join(", ", t.Select(x => $"({x?.Data?.TileMapX}, {x?.Data?.TileMapY})"))}";

                    // Set debug text
                    Controller.obj.tempDebugText.text = Settings.ShowDebugInfo
                        ? $"{String.Join(Environment.NewLine, t.Select(x => x?.DebugText))}{Environment.NewLine}" +
                          $"PC_TransparencyMode: {String.Join(", ", t.Select(x => x?.Data?.PC_TransparencyMode))}{Environment.NewLine}" +
                          $"PC_Unk1: {String.Join(", ", t.Select(x => x?.Data?.PC_Unk1))}{Environment.NewLine}" +
                          $"PC_Unk2: {String.Join(", ", t.Select(x => x?.Data?.PC_Unk2))}{Environment.NewLine}" +
                          $"HorizontalFlip: {String.Join(", ", t.Select(x => x?.Data?.HorizontalFlip))}{Environment.NewLine}" +
                          $"VerticalFlip: {String.Join(", ", t.Select(x => x?.Data?.VerticalFlip))}{Environment.NewLine}" +
                          $"PaletteIndex: {String.Join(", ", t.Select(x => x?.Data?.PaletteIndex))}{Environment.NewLine}"
                        : String.Empty;
                }
            }
        }
    }
}