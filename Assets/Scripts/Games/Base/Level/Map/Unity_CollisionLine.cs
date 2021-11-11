using UnityEngine;

namespace Ray1Map
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

        public bool Is3D { get; set; }

        public Vector3 GetUnityPosition(int i, Unity_IsometricData isometricData)
        {
            Vector3 pos = i switch
            {
                0 => Pos_0,
                1 => Pos_1,
                _ => throw new System.Exception("Lines only have 2 points")
            };

            if (Is3D) 
            {
                Vector3 isometricScale = isometricData.AbsoluteObjectScale;
                return Vector3.Scale(new Vector3(pos.x, pos.z, -pos.y), isometricScale);
            } 
            else 
            {
                return new Vector3(pos.x, -pos.y, pos.z) / LevelEditorData.Level.PixelsPerUnit;
            }
        }

        public float UnityWidth { get; set; } = 0.095f;
    }
}