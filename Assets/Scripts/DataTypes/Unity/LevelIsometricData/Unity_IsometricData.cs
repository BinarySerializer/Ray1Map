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
        public GameObject GetCollisionGameObject() {
            GameObject parent = new GameObject("Collision parent");
            parent.transform.localScale = Scale;
            Shader sh = Shader.Find("Standard");
            Material mat = new Material(sh);

            for (int y = 0; y < CollisionHeight; y++) {
                for (int x = 0; x < CollisionWidth; x++) {
                    int ind = y * CollisionWidth + x;
                    var block = Collision[ind];
                    block.GetGameObject(parent,x,y,mat,Collision,CollisionWidth, CollisionHeight);
                }
            }
            return parent;
        }
        #endregion
    }
}