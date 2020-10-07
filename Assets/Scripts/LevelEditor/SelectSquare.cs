using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace R1Engine {
    public class SelectSquare : MonoBehaviour {
        Unity_Level lvl => LevelEditorData.Level;
        Image main, overlay;
        LevelMainController tileController;
        int xs, ys, xe, ye;

        public Color color;
        public bool HasSelection { get; private set; }
        public int XStart { get; private set; }
        public int YStart { get; private set; }
        public int XEnd { get; private set; }
        public int YEnd { get; private set; }

        public void SetStartCorner(int x, int y) {
            xs = x; ys = y;
            HasSelection = true;
            UpdateVisual();
        }
        public void SetEndCorner(int x, int y) {
            xe = x; ye = y;
            HasSelection = true;
            UpdateVisual();
        }
        public void Clear() {
            //xs = 0; ys = 0; xe = 0; ye = 0;
            HasSelection = false;
        }

        private RectTransform rectTransform;

        /// <summary>
        /// Gets the tiles contained within the selection square.
        /// </summary>
        /// <returns></returns>
        public Unity_Tile[] GetTiles() {
            var r = new List<Unity_Tile>();
            for (int y = (int)YStart; y <= YEnd; y++)
                for (int x = (int)XStart; x <= XEnd; x++)
                    r.Add(lvl.Maps[LevelEditorData.CurrentMap].MapTiles[x + y * lvl.Maps[LevelEditorData.CurrentMap].Width]);
            return r.ToArray();
        }


        void Awake() {
            main = GetComponent<Image>();
            overlay = transform.GetChild(0).GetComponent<Image>();
            tileController = FindObjectOfType<LevelMainController>();
        }

        private void Start() {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update() {
            if (HasSelection)
                transform.localScale = Vector3.one;
            else {
                transform.localScale = Vector3.zero; return;
            }
        }

        public void UpdateVisual() {
            main.pixelsPerUnitMultiplier = 8f / Camera.main.orthographicSize;
            main.color = new Color(color.r, color.g, color.b, 0.8f);
            overlay.color = new Color(color.r, color.g, color.b, 0.075f);

            // Adjust for Unity so that the start corner is always the top-left
            XStart = xs; YStart = ys; XEnd = xe; YEnd = ye;
            if (XEnd < XStart) {
                var temp = XEnd;
                XEnd = XStart;
                XStart = temp;
            }
            if (YEnd < YStart) {
                var temp = YEnd;
                YEnd = YStart;
                YStart = temp;
            }

            transform.position =
                new Vector3(
                    XStart * tileController.controllerTilemap.CellSizeInUnits,
                    -YStart * tileController.controllerTilemap.CellSizeInUnits, 2);
            rectTransform.sizeDelta = Vector2.one * tileController.controllerTilemap.CellSizeInUnits
                + (new Vector2(
                    (XEnd - XStart) * tileController.controllerTilemap.CellSizeInUnits,
                    (YEnd - YStart) * tileController.controllerTilemap.CellSizeInUnits));
        }
    }
}