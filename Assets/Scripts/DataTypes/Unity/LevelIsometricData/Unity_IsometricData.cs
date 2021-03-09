using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_IsometricData
    {
        #region Public Properties

        /// <summary>
        /// The level width
        /// </summary>
        public int CollisionWidth { get; set; }

        /// <summary>
        /// The level height
        /// </summary>
        public int CollisionHeight { get; set; }

        public Unity_IsometricCollisionTile[] Collision { get; set; }

        public Vector3 Scale { get; set; } = Vector3.one;

        public int TilesWidth { get; set; }
        public int TilesHeight { get; set; }

        public Quaternion ViewAngle { get; set; } = Quaternion.Euler(30f, -45, 0);
        public Func<float> CalculateYDisplacement { get; set; } = () => LevelEditorData.Level.IsometricData.CollisionWidth + LevelEditorData.Level.IsometricData.CollisionHeight;
        public Func<float> CalculateXDisplacement { get; set; } = () => 0;
        public Vector3 ObjectScale { get; set; } = Vector3.one * 16;
        public Vector3 AbsoluteObjectScale => new Vector3(Scale.x / ObjectScale.x, Scale.y / ObjectScale.y, Scale.z / ObjectScale.z);

        #endregion

        #region Helper Methods
        public GameObject GetCollisionVisualGameObject(Material mat) {
            GameObject parent = new GameObject("3D Collision - Visual");
            parent.layer = LayerMask.NameToLayer("3D Collision");
            List<MeshFilter> mfs = new List<MeshFilter>();
            List<MeshFilter> addMfs = new List<MeshFilter>();
            for (int y = 0; y < CollisionHeight; y++) {
                for (int x = 0; x < CollisionWidth; x++) {
                    int ind = y * CollisionWidth + x;
                    var block = Collision[ind];
                    mfs.Add(block.GetGameObject(parent,x,y,mat,Collision,CollisionWidth, CollisionHeight, addMfs));
                }
            }
            CombineVisualMeshes(parent, mfs.ToArray(), addMfs.ToArray(), mat, Collision?.Select(c => c.GetMeshTriangleCount()).ToArray());
            parent.transform.localScale = Scale;
            return parent;
        }
        public void CombineVisualMeshes(GameObject parent, MeshFilter[] mfs, MeshFilter[] addMfs, Material mat, int[] triangleCount) {
            int currentMesh = 0;
            int currentMF = 0;
            while (currentMF < mfs.Length) {
                int currentTriCount = 0;
                int startMF = currentMF;
                while (currentMF < mfs.Length && currentTriCount < 24000) {
                    currentTriCount += triangleCount[currentMF];
                    currentMF++;
                }
                int currentMFCount = currentMF - startMF;
                GameObject gao = new GameObject($"3D Collision - Visual - Mesh {currentMesh}");
                gao.layer = LayerMask.NameToLayer("3D Collision");
                gao.transform.SetParent(parent.transform);
                gao.transform.localPosition = Vector3.zero;
                MeshFilter mf = gao.AddComponent<MeshFilter>();
                CombineInstance[] combine = new CombineInstance[currentMFCount];
                for (int i = 0; i < combine.Length; i++) {
                    var ind = i + startMF;
                    combine[i].mesh = mfs[ind].sharedMesh;
                    combine[i].transform = Matrix4x4.Translate(mfs[ind].transform.localPosition);
                }
                mf.mesh = new Mesh();
                mf.mesh.CombineMeshes(combine);
                MeshRenderer mr = gao.AddComponent<MeshRenderer>();
                mr.sharedMaterial = mat;
                currentMesh++;
            }
            int numCombinedAddMeshes = Mathf.CeilToInt(addMfs.Length / (float)2000);
            for (int m = 0; m < numCombinedAddMeshes; m++) {
                int mfInd = 2000 * m;
                GameObject gao = new GameObject($"3D Collision - Visual - Add Mesh {m}");
                gao.layer = LayerMask.NameToLayer("3D Collision");
                gao.transform.SetParent(parent.transform);
                gao.transform.localPosition = Vector3.zero;
                MeshFilter mf = gao.AddComponent<MeshFilter>();
                CombineInstance[] combine = new CombineInstance[Mathf.Min(2000, addMfs.Length - mfInd)];
                for (int i = 0; i < combine.Length; i++) {
                    var ind = i + mfInd;
                    combine[i].mesh = addMfs[ind].sharedMesh;
                    combine[i].transform = Matrix4x4.Translate(addMfs[ind].transform.localPosition + addMfs[ind].transform.parent.localPosition);
                }
                mf.mesh = new Mesh();
                mf.mesh.CombineMeshes(combine);
                MeshRenderer mr = gao.AddComponent<MeshRenderer>();
                mr.sharedMaterial = mat;
            }
        }
        public GameObject GetCollisionCollidersGameObject() {
            GameObject parent = new GameObject("3D Collision - Colliders");
            parent.transform.localScale = Scale;

            for (int y = 0; y < CollisionHeight; y++) {
                for (int x = 0; x < CollisionWidth; x++) {
                    int ind = y * CollisionWidth + x;
                    var block = Collision[ind];
                    block.GetGameObjectCollider(parent, x, y);
                }
            }
            return parent;
        }

        public Unity_IsometricCollisionTile GetCollisionTile(int x, int y) {
            int ind = y * CollisionWidth + x;
            if(Collision == null) return null;
            if(ind >= Collision.Length) return null;
            return Collision[ind];
        }
        #endregion

        #region Public Static Properties

        public static Unity_IsometricData Mode7(int cellSize) => new Unity_IsometricData()
        {
            CollisionWidth = 0,
            CollisionHeight = 0,
            TilesWidth = 0,
            TilesHeight = 0,
            Collision = null,
            Scale = Vector3.one / 2,
            ViewAngle = Quaternion.Euler(90, 0, 0),
            CalculateYDisplacement = () => 0,
            CalculateXDisplacement = () => 0,
            ObjectScale = Vector3.one * cellSize
        };

        #endregion
    }
}