using System;
using System.Linq;
using UnityEngine;

namespace Ray1Map
{
    public class Unity_IsometricCollisionObject
    {
        public Vector3[] Points { get; set; } // 4 points

        public Color Color { get; set; } = Color.gray;
        public string DebugText { get; set; }

        public MeshFilter GetGameObject(GameObject parent)
        {
            if (Points.Length != 4)
                throw new Exception($"Invalid points count of {Points.Length}, should be 4");

            GameObject gao = new GameObject();
            gao.name = DebugText;

            gao.layer = LayerMask.NameToLayer("3D Collision");
            gao.transform.SetParent(parent.transform);
            gao.transform.localScale = Vector3.one;
            gao.transform.localPosition = Vector3.zero;

            MeshFilter mf = gao.AddComponent<MeshFilter>();

            mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(Points.
                Select(x => new Vector3(x.x, 0, -x.y)).
                Concat(Points.Select(x => new Vector3(x.x, x.z, -x.y))).
                ToArray(), color: Color);

            return mf;
        }
    }
}