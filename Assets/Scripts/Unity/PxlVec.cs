using UnityEngine;

namespace Ray1Map {
    [System.Serializable]
    public struct PxlVec {
        public ushort x, y;

        public PxlVec(Vector3 vec) {
            this.vec = vec;
            x = (ushort)(vec.x * LevelEditorData.Level.PixelsPerUnit);
            y = (ushort)(-vec.y * LevelEditorData.Level.PixelsPerUnit);
        }
        public PxlVec(ushort x, ushort y) {
            vec = new Vector3((float)x / LevelEditorData.Level.PixelsPerUnit, -(float)y / LevelEditorData.Level.PixelsPerUnit);
            this.x = x;
            this.y = y;
        }
        Vector3 vec;

        public static Vector3 SnapVec(Vector3 vec) {
            return new Vector3(
                (float)(int)(vec.x * LevelEditorData.Level.PixelsPerUnit) / LevelEditorData.Level.PixelsPerUnit,
                (float)(int)(vec.y * LevelEditorData.Level.PixelsPerUnit) / LevelEditorData.Level.PixelsPerUnit);
        }

        public static implicit operator PxlVec(Vector3 v) => new PxlVec(v);
        public static implicit operator Vector3(PxlVec v) => v.vec;
    }
}
