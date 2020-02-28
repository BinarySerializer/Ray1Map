using UnityEngine;

namespace R1Engine {
    [System.Serializable]
    public struct PxlVec {
        public ushort x, y;

        public PxlVec(Vector3 vec) {
            this.vec = vec;
            x = (ushort)(vec.x * 16);
            y = (ushort)(-vec.y * 16);
        }
        public PxlVec(ushort x, ushort y) {
            vec = new Vector3((float)x / 16, -(float)y / 16);
            this.x = x;
            this.y = y;
        }
        Vector3 vec;

        public static Vector3 SnapVec(Vector3 vec) {
            return new Vector3(
                (float)(int)(vec.x * 16) / 16,
                (float)(int)(vec.y * 16) / 16);
        }

        public static implicit operator PxlVec(Vector3 v) => new PxlVec(v);
        public static implicit operator Vector3(PxlVec v) => v.vec;
    }
}
