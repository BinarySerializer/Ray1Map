using UnityEngine;

namespace R1Engine {
    public class Level {
        public ushort width;
        public ushort height;
        public Type[] types;
        public Event[] events = new Event[0];

        public PxlVec raymanPos;

        public Type TypeFromCoord(Vector3 pos) {
            int i = (int)(System.Math.Floor(-pos.y) * width + System.Math.Floor(pos.x));
            if (i < types.Length && i >= 0) return types[i];
            else return new Type();
        }
    }
}
