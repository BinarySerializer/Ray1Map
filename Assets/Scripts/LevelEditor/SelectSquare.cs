using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace R1Engine.Unity {
    public class SelectSquare : MonoBehaviour {
        Common_Lev lvl => tileController.currentLevel;
        Image main, overlay;
        LevelMainController tileController;
        float xs, ys, xe, ye;

        public Color color;
        public bool HasSelection { get; private set; }
        public float XStart { get; private set; }
        public float YStart { get; private set; }
        public float XEnd { get; private set; }
        public float YEnd { get; private set; }

        public void SetStartCorner(float x, float y) {
            xs = x; ys = y;
            HasSelection = true;
        }
        public void SetEndCorner(float x, float y) {
            xe = x; ye = y;
            HasSelection = true;
        }
        public void Clear() {
            //xs = 0; ys = 0; xe = 0; ye = 0;
            HasSelection = false;
        }

        /// <summary>
        /// Gets the tiles contained within the selection square.
        /// </summary>
        /// <returns></returns>
        public Common_Tile[] GetTiles() {
            var r = new List<Common_Tile>();
            for (int y = (int)YStart; y <= YEnd; y++)
                for (int x = (int)XStart; x <= XEnd; x++)
                    r.Add(lvl.Tiles[x + y * lvl.Width]);
            return r.ToArray();
        }


        void Awake() {
            main = GetComponent<Image>();
            overlay = transform.GetChild(0).GetComponent<Image>();
            tileController = FindObjectOfType<LevelMainController>();
        }

        void Update() {
            if (HasSelection)
                transform.localScale = Vector3.one;
            else {
                transform.localScale = Vector3.zero; return;
            }

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

            transform.position = new Vector3(XStart, -YStart, 2);
            GetComponent<RectTransform>().sizeDelta = Vector2.one
                + (new Vector2(XEnd, YEnd) - new Vector2(transform.position.x, -transform.position.y));
        }
    }
}