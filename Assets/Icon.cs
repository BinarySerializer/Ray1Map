using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine.Unity {
    public class Icon : MonoBehaviour {
        void Start() {
        }
        void Update() {
            transform.localScale = Vector3.one * Camera.main.orthographicSize / 25;
        }
    }
}
