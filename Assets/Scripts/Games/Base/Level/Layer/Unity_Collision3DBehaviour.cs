using UnityEngine;

namespace Ray1Map {
    public class Unity_Collision3DBehaviour : MonoBehaviour {
        public string Type { get; set; }
        public string Shape { get; set; }
        public string AddType { get; set; }
        public string DebugText { get; set; }
        public Unity_IsometricCollisionTile IsometricTile { get; set; }
        public Vector2Int? IsometricPosition { get; set; }
    }
}
