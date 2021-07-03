using UnityEngine;

namespace R1Engine
{
    public class Unity_CollisionLine
    {
        public Unity_CollisionLine() { }

        public Unity_CollisionLine(Vector2 pos_0, Vector2 pos_1, Color? lineColor = null)
        {
            Pos_0 = pos_0;
            Pos_1 = pos_1;
            LineColor = lineColor ?? Color.yellow;
        }

        public Vector2 Pos_0 { get; set; }
        public Vector2 Pos_1 { get; set; }
        public Color LineColor { get; set; }
        public string TypeName { get; set; }
        public string DebugText { get; set; }
    }
}