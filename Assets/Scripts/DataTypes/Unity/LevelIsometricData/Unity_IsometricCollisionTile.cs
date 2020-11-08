using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_IsometricCollisionTile
    {
        #region Public Properties
        public CollisionType Type { get; set; }
        public AdditionalTypeFlags AddType { get; set; }
        public ShapeType Shape { get; set; }
        public int Height { get; set; }
        public string DebugText { get; set; }
        #endregion

        #region Methods
        void AddFence(GameObject gao, AdditionalTypeFlags type, Material mat, Color color, float height) {
            int numBars = 3;
            float fenceHeight = 4f;
            for (int i = 0; i < numBars; i++) {
                GameObject sgao = new GameObject($"Fence {i}");
                sgao.layer = LayerMask.NameToLayer("3D Collision");
                sgao.transform.SetParent(gao.transform);
                sgao.transform.localScale = Vector3.one;
                switch (type) {
                    case AdditionalTypeFlags.FenceUpLeft:
                        sgao.transform.localPosition = new Vector3(-(0.5f - 0.05f), height, ((float)(i + 0.5f)) / (numBars) - 0.5f);
                        break;
                    case AdditionalTypeFlags.FenceUpRight:
                        sgao.transform.localPosition = new Vector3(((float)(i + 0.5f)) / (numBars) - 0.5f, height, 0.5f - 0.05f);
                        break;
                    case AdditionalTypeFlags.FenceDownRight:
                        sgao.transform.localPosition = new Vector3((0.5f - 0.05f), height, ((float)(i + 0.5f)) / (numBars) - 0.5f);
                        break;
                    case AdditionalTypeFlags.FenceDownLeft:
                        sgao.transform.localPosition = new Vector3(((float)(i + 0.5f)) / (numBars) - 0.5f, height, -(0.5f - 0.05f));
                        break;
                }
                MeshFilter smf = sgao.AddComponent<MeshFilter>();
                smf.mesh = GeometryHelpers.CreateBoxDifferentHeights(0.1f, fenceHeight, fenceHeight, fenceHeight, fenceHeight);
                MeshRenderer smr = sgao.AddComponent<MeshRenderer>();
                smr.material = mat;
                //smr.material.color = color;
            }
        }

        void AddClimb(GameObject gao, AdditionalTypeFlags type, Material mat, Color color, float height, float baseHeight) {
            int numBars = Mathf.RoundToInt(height - baseHeight);
            for (int i = 0; i < numBars; i++) {
                GameObject sgao = new GameObject($"Fence {i}");
                sgao.layer = LayerMask.NameToLayer("3D Collision");
                sgao.transform.SetParent(gao.transform);
                sgao.transform.localScale = Vector3.one;
                MeshFilter smf = sgao.AddComponent<MeshFilter>();
                switch (type) {
                    case AdditionalTypeFlags.ClimbUpRight:
                        sgao.transform.localPosition = new Vector3(0, baseHeight + i + 0.5f, -0.55f);
                        smf.mesh = GeometryHelpers.CreateBox(1f, 0.2f, 0.1f);
                        break;
                    case AdditionalTypeFlags.ClimbUpLeft:
                        sgao.transform.localPosition = new Vector3(0.55f, baseHeight + i + 0.5f, 0);
                        smf.mesh = GeometryHelpers.CreateBox(0.1f, 0.2f, 1f);
                        break;
                }
                MeshRenderer smr = sgao.AddComponent<MeshRenderer>();
                smr.material = mat;
                //smr.material.color = color;
            }
        }

        public GameObject GetGameObject(GameObject parent, int x, int y, Material mat, Unity_IsometricCollisionTile[] collisionData, int levelWidth, int levelHeight) {
            GameObject gao = new GameObject();
            gao.name = DebugText;

            gao.layer = LayerMask.NameToLayer("3D Collision");
            gao.transform.SetParent(parent.transform);
            gao.transform.localScale = Vector3.one;
            gao.transform.localPosition = new Vector3(x + 0.5f, 0, -y - 0.5f);
            MeshFilter mf = gao.AddComponent<MeshFilter>();
            switch (Shape) {
                case ShapeType.SlopeUpRight:
                    mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(1, Height + 1, Height + 1, Height, Height);
                    break;
                case ShapeType.SlopeUpLeft:
                    mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(1, Height + 1, Height, Height, Height + 1);
                    break;
                default:
                    mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(1, Height, Height, Height, Height);
                    break;
            }
            MeshRenderer mr = gao.AddComponent<MeshRenderer>();
            mr.material = mat;

            var color = CollisionColors[Type];
            mr.material.color = color;

            if (AddType.HasFlag(AdditionalTypeFlags.FenceUpLeft_RHR)) {
                var neighborBlock = x > 0 ? collisionData[y * levelWidth + (x - 1)] : null;
                var maxHeight = Math.Max(Height, neighborBlock?.Height ?? 0);
                AddFence(gao, AdditionalTypeFlags.FenceUpLeft, mat, color, maxHeight);
            }
            if (AddType.HasFlag(AdditionalTypeFlags.FenceUpRight_RHR)) {
                var neighborBlock = y > 0 ? collisionData[(y - 1) * levelWidth + x] : null;
                var maxHeight = Math.Max(Height, neighborBlock?.Height ?? 0);
                AddFence(gao, AdditionalTypeFlags.FenceUpRight, mat, color, maxHeight);
            }
            if (AddType.HasFlag(AdditionalTypeFlags.FenceUpLeft)) {
                AddFence(gao, AdditionalTypeFlags.FenceUpLeft, mat, color, Height);
            }
            if (AddType.HasFlag(AdditionalTypeFlags.FenceUpRight)) {
                AddFence(gao, AdditionalTypeFlags.FenceUpRight, mat, color, Height);
            }
            if (AddType.HasFlag(AdditionalTypeFlags.FenceDownRight)) {
                AddFence(gao, AdditionalTypeFlags.FenceDownRight, mat, color, Height);
            }
            if (AddType.HasFlag(AdditionalTypeFlags.FenceDownLeft)) {
                AddFence(gao, AdditionalTypeFlags.FenceDownLeft, mat, color, Height);
            }

            if (AddType.HasFlag(AdditionalTypeFlags.ClimbUpLeft)) {
                var neighborBlock = x + 1 < levelWidth ? collisionData[y * levelWidth + (x + 1)] : null;
                var baseHeight = neighborBlock?.Height ?? 0;
                AddClimb(gao, AdditionalTypeFlags.ClimbUpLeft, mat, color, Height, baseHeight);
            }
            if (AddType.HasFlag(AdditionalTypeFlags.ClimbUpRight)) {
                var neighborBlock = y + 1 < levelHeight ? collisionData[(y + 1) * levelWidth + x] : null;
                var baseHeight = neighborBlock?.Height ?? 0;
                AddClimb(gao, AdditionalTypeFlags.ClimbUpRight, mat, color, Height, baseHeight);
            }
            return gao;
        }
        #endregion


        [Flags]
        public enum AdditionalTypeFlags : int {
            None = 0,
            FenceUpLeft_RHR = 1 << 0,
            FenceUpRight_RHR = 1 << 1,
            ClimbUpLeft = 1 << 2,
            ClimbUpRight = 1 << 3,
            FenceUpLeft = 1 << 4,
            FenceDownRight = 1 << 5,
            FenceUpRight = 1 << 6,
            FenceDownLeft = 1 << 7
        }

        public enum CollisionType : int {
            Unknown,
            Solid,
            Water,
            Lava,
            Wall,
            ObstacleHurt,
            Pit,
            WaterFlowBottomLeft,
            WaterFlowBottomRight,
            ExitTrigger,
            NearExitTrigger,
            DialogueTrigger1,
            DialogueTrigger2,
            DialogueTrigger3,
            DialogueTrigger4,
            FreezableWater,
        }

        public enum ShapeType : int {
            None,
            Normal,
            SlopeUpRight,
            SlopeUpLeft,
            LevelEdgeTop,
            LevelEdgeBottom,
            LevelEdgeLeft,
            LevelEdgeRight,
            Pit,
            OutOfBounds,
            Unknown
        }

        public Dictionary<CollisionType, Color> CollisionColors { get; } = new Dictionary<CollisionType, Color>()
        {
            [CollisionType.Unknown] = Color.white,

            [CollisionType.Solid] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.Wall] = new Color(68 / 255f, 75 / 255f, 87 / 255f),

            [CollisionType.Water] = new Color(51 / 255f, 126 / 255f, 255 / 255f),
            [CollisionType.WaterFlowBottomLeft] = new Color(49 / 255f, 100 / 255f, 189 / 255f),
            [CollisionType.WaterFlowBottomRight] = new Color(55 / 255f, 93 / 255f, 161) / 255f,
            [CollisionType.FreezableWater] = new Color(103 / 255f, 99 / 255f, 235 / 255f),

            [CollisionType.Lava] = new Color(222 / 255f, 67 / 255f, 24 / 255f),
            [CollisionType.ObstacleHurt] = new Color(212 / 255f, 32 / 255f, 32 / 255f),
            [CollisionType.Pit] = new Color(158 / 255f, 121 / 255f, 0 / 255f),

            [CollisionType.ExitTrigger] = new Color(194 / 255f, 191 / 255f, 25 / 255f),
            [CollisionType.NearExitTrigger] = new Color(155 / 255f, 194 / 255f, 25 / 255f),
            [CollisionType.DialogueTrigger1] = new Color(23 / 255f, 145 / 255f, 38 / 255f),
            [CollisionType.DialogueTrigger2] = new Color(23 / 255f, 145 / 255f, 38 / 255f),
            [CollisionType.DialogueTrigger3] = new Color(23 / 255f, 145 / 255f, 38 / 255f),
            [CollisionType.DialogueTrigger4] = new Color(23 / 255f, 145 / 255f, 38 / 255f),
        };
    }
}