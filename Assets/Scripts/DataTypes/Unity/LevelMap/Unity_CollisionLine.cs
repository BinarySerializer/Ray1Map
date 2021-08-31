using UnityEngine;

namespace R1Engine
{
    public class Unity_CollisionLine
    {
        public Unity_CollisionLine() { }

        public Unity_CollisionLine(Vector3 pos_0, Vector3 pos_1, Color? lineColor = null)
        {
            Pos_0 = pos_0;
            Pos_1 = pos_1;
            LineColor = lineColor ?? Color.yellow;
        }

        public Vector3 Pos_0 { get; set; }
        public Vector3 Pos_1 { get; set; }
        public Color LineColor { get; set; }
        public string TypeName { get; set; }
        public string DebugText { get; set; }

        public bool is3D { get; set; }

        public Vector3 GetUnityPosition(int i) {
            Vector3 pos = default;
            switch (i) {
                case 0: pos = Pos_0; break;
                case 1: pos = Pos_1; break;
                default: throw new System.Exception("Lines only have 2 points");
            }
            if (is3D) {
                Vector3 isometricScale = LevelEditorData.Level.IsometricData.AbsoluteObjectScale;
                return Vector3.Scale(new Vector3(pos.x, pos.z, -pos.y), isometricScale);
                //return new Vector3(pos.x, pos.y, pos.z);
            } else {
                return new Vector3(pos.x, -pos.y, pos.z) / LevelEditorData.Level.PixelsPerUnit;
            }
        }
        public float UnityWidth { get; set; } = 0.095f;
    }
}