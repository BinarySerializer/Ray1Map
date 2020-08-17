using UnityEngine;

namespace R1Engine {
    [System.Serializable]
    public struct PxlVec {
        public ushort x, y;

        public PxlVec(Vector3 vec) {
            this.vec = vec;
            x = (ushort)(vec.x * LevelEditorData.EditorManager.PixelsPerUnit);
            y = (ushort)(-vec.y * LevelEditorData.EditorManager.PixelsPerUnit);
        }
        public PxlVec(ushort x, ushort y) {
            vec = new Vector3((float)x / LevelEditorData.EditorManager.PixelsPerUnit, -(float)y / LevelEditorData.EditorManager.PixelsPerUnit);
            this.x = x;
            this.y = y;
        }
        Vector3 vec;

        public static Vector3 SnapVec(Vector3 vec) {
            return new Vector3(
                (float)(int)(vec.x * LevelEditorData.EditorManager.PixelsPerUnit) / LevelEditorData.EditorManager.PixelsPerUnit,
                (float)(int)(vec.y * LevelEditorData.EditorManager.PixelsPerUnit) / LevelEditorData.EditorManager.PixelsPerUnit);
        }

        public static implicit operator PxlVec(Vector3 v) => new PxlVec(v);
        public static implicit operator Vector3(PxlVec v) => v.vec;
    }
}
