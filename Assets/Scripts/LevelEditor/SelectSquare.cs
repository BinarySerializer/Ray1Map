using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace R1Engine.Unity {
    public class SelectSquare : MonoBehaviour {
        public Color color;
        public int xStart, yStart, xEnd, yEnd;
        Image main, overlay;
        LevelMainController tileController;
        Common_Lev lvl => tileController.currentLevel;

        void Awake() {
            main = GetComponent<Image>();
            overlay = transform.GetChild(0).GetComponent<Image>();
            tileController = FindObjectOfType<LevelMainController>();
        }

        /// <summary>
        /// Gets the tiles contained within the selection square.
        /// </summary>
        /// <returns></returns>
        public Common_Tile[] GetTiles() {
            var r = new List<Common_Tile>();
            for (int y = ys; y <= ye; y++)
                for (int x = xs; x <= xe; x++)
                    r.Add(lvl.Tiles[x + y * lvl.Width]);
            return r.ToArray();
        }

        public int xs, ys, xe, ye;

        void Update() {
            main.pixelsPerUnitMultiplier = 8f / Camera.main.orthographicSize;
            main.color = new Color(color.r, color.g, color.b, 0.8f);
            overlay.color = new Color(color.r, color.g, color.b, 0.075f);

            // Adjust for Unity so that the start corner is always the top-left
            xs = xStart; ys = yStart; xe = xEnd; ye = yEnd;
            if (xe < xs) {
                var temp = xe;
                xe = xs;
                xs = temp;
            }
            if (ye < ys) {
                var temp = ye;
                ye = ys;
                ys = temp;
            }

            transform.position = new Vector3(xs, -ys, 2);
            GetComponent<RectTransform>().sizeDelta = Vector2.one
                + (new Vector2(xe, ye) - new Vector2(transform.position.x, -transform.position.y));
        }
    }
}