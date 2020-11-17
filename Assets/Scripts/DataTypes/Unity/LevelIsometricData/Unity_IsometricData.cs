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

        #endregion

        #region Helper Methods
        public GameObject GetCollisionVisualGameObject(Material mat) {
            GameObject parent = new GameObject("3D Collision - Visual");
            parent.transform.localScale = Scale;

            for (int y = 0; y < CollisionHeight; y++) {
                for (int x = 0; x < CollisionWidth; x++) {
                    int ind = y * CollisionWidth + x;
                    var block = Collision[ind];
                    block.GetGameObject(parent,x,y,mat,Collision,CollisionWidth, CollisionHeight);
                }
            }
            return parent;
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
            if(ind >= Collision.Length) return null;
            return Collision[ind];
        }
        #endregion
    }
}