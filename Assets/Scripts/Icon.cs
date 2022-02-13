using UnityEngine;

namespace Ray1Map {
    public class Icon : MonoBehaviour {
        void Start() {
        }
        void Update() {
            transform.localScale = Vector3.one * Camera.main.orthographicSize / 25;
        }
    }
}
