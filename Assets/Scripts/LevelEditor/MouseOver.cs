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
                textCollision.text = $"{e.DisplayName}";
                textGraphic.text = $"Type: {e.Data.TypeValue}{Environment.NewLine}" +
                                   $"Pos: {e.Data.Data.XPosition}, {e.Data.Data.YPosition}{Environment.NewLine}" +
                                   $"Offsets: {e.Data.Data.OffsetBX} x {e.Data.Data.OffsetBY} x {e.Data.Data.OffsetHY}";

                // Set debug text
                Controller.obj.tempDebugText.text = Settings.ShowDebugInfo 
                    ? $"{e.Data.DebugText}{Environment.NewLine}" +
                      $"CurrentFrame: {(int)e.Data.Data.RuntimeCurrentAnimFrame}{Environment.NewLine}" +
                      $"Frames: {e.CurrentAnimation?.Frames?.GetLength(0)}{Environment.NewLine}" +
                      $"AnimationIndex: {e.Data.Data.RuntimeCurrentAnimIndex}{Environment.NewLine}" +
                      $"AnimationSpeed: {e.AnimSpeed}{Environment.NewLine}" +
                      $"{Environment.NewLine}" +
                      $"RightSpeed: {e.State?.RightSpeed}{Environment.NewLine}" +
                      $"LeftSpeed: {e.State?.LeftSpeed}{Environment.NewLine}" +
                      $"InteractionType: {e.State?.InteractionType}{Environment.NewLine}" +
                      $"Sound: {e.State?.SoundIndex}{Environment.NewLine}" +
                      $"{Environment.NewLine}" +
                      $"Etat: {e.Data.Data.RuntimeEtat}{Environment.NewLine}" +
                      $"SubEtat: {e.Data.Data.RuntimeSubEtat}{Environment.NewLine}" +
                      $"{Environment.NewLine}" +
                      $"LinkID: {e.LinkID}{Environment.NewLine}"
                    : String.Empty;
            }
            // Else Mouse over type
            else {
                Controller.obj.tempDebugText.text = String.Empty;
                var editorManager = LevelEditorData.EditorManager;
                var t = editorManager?.Level?.Maps?.ElementAtOrDefault(LevelEditorData.CurrentMap)?.GetMapTile(mouseTile.x, mouseTile.y);
                var c = editorManager?.Level?.Maps?.ElementAtOrDefault(LevelEditorData.CurrentCollisionMap)?.GetMapTile(mouseTile.x, mouseTile.y);

                if (t != null && c != null) {
                    //Debug.Log("Tile here x:" + t.XPosition + " y:" + t.YPosition + " col:" + t.CollisionType);
                    textCollision.text = $"Collision: {editorManager.GetCollisionTypeAsEnum(c.Data.CollisionType)}";
                    textGraphic.text = $"Graphic tile: {t.Data.TileMapX}, {t.Data.TileMapY}";

                    // Set debug text
                    Controller.obj.tempDebugText.text = Settings.ShowDebugInfo
                        ? $"{t.DebugText}{Environment.NewLine}" +
                          $"Collision: {editorManager.GetCollisionTypeAsEnum(c.Data.CollisionType)}{Environment.NewLine}" +
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