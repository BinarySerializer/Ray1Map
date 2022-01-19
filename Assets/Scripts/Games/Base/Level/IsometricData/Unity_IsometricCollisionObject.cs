using UnityEngine;

namespace Ray1Map
{
    public class Unity_IsometricCollisionObject
    {
        public Vector2 Position { get; set; }
        public Vector2Int Dimensions { get; set; }
        public float Height { get; set; }

        public Color Color { get; set; } = Color.gray;
        public string DebugText { get; set; }

        public MeshFilter GetGameObject(GameObject parent)
        {
            GameObject gao = new GameObject();
            gao.name = DebugText;

            gao.layer = LayerMask.NameToLayer("3D Collision");
            gao.transform.SetParent(parent.transform);
            gao.transform.localScale = Vector3.one;
            gao.transform.localPosition = new Vector3(Position.x, 0, -Position.y);

            MeshFilter mf = gao.AddComponent<MeshFilter>();
            mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(Dimensions.x, Dimensions.y, Height, Height, Height, Height, color: Color);

            return mf;
        }

        public GameObject GetGameObjectCollider(GameObject parent)
        {
            GameObject gao = new GameObject();
            float height = Height;
            gao.layer = LayerMask.NameToLayer("3D Collision");
            gao.transform.SetParent(parent.transform);
            gao.transform.localScale = Vector3.one;
            gao.transform.localPosition = new Vector3(Position.x, 0, -Position.y);
            BoxCollider bc = gao.AddComponent<BoxCollider>();
            bc.center = Vector3.up * height / 2f;
            bc.size = new Vector3(1f, height, 1f);
            return gao;
        }
    }
}