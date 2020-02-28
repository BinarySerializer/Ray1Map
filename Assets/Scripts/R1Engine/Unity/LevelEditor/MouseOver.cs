using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Input;

namespace R1Engine.Unity {
    public class MouseOver : MonoBehaviour {
        public Text gfxText, colText;
        void Start() {

        }

        void Update() {
            transform.position = mousePosition;


            Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out var hit, 30);
            var e = hit.collider?.GetComponent<EventBehaviour>();
            // Mouse over event
            if (e != null) {
                colText.text = $"{e.ev.behaviour}";
                gfxText.text = $"Pos: {e.ev.pos.x}, {e.ev.pos.y}";
            }
            // Else Mouse over type
            else {
                var type = Controller.obj.lvl.level.TypeFromCoord(Camera.main.ScreenToWorldPoint(mousePosition));
                colText.text = $"Collision: {type.col.ToString().Replace('_', ' ')}";
                gfxText.text = $"Graphic tile: {type.gX}, {type.gY}";
            }
        }
    }
}