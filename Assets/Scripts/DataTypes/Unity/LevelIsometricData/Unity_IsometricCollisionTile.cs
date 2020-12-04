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
        private static Mesh fenceMesh = null;
        private static Mesh climbMesh = null;
        private static Mesh boxMesh = null;
        private static Mesh planeMesh = null;
        private static Mesh slopeMesh = null;
        private static MaterialPropertyBlock mpb = null;

        void SetRendererColor(MeshRenderer mr, Color color) {
            if (mpb == null) {
                mpb = new MaterialPropertyBlock();
            }
            mr.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", color);
            mr.SetPropertyBlock(mpb);
        }

        void AddFence(GameObject gao, AdditionalTypeFlags type, Material mat, Color color, float height) {
            int numBars = 3;
            if (fenceMesh == null) {
                float fenceHeight = 4f;
                fenceMesh = GeometryHelpers.CreateBoxDifferentHeights(0.1f, fenceHeight, fenceHeight, fenceHeight, fenceHeight);
            }
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
                smf.mesh = fenceMesh;
                MeshRenderer smr = sgao.AddComponent<MeshRenderer>();
                smr.sharedMaterial = mat;
                //smr.material.color = color;
            }
        }
        void AddClimb(GameObject gao, AdditionalTypeFlags type, Material mat, Color color, float height, float baseHeight) {
            int numBars = Mathf.RoundToInt(height - baseHeight);
            if (climbMesh == null) {
                climbMesh = GeometryHelpers.CreateBox(1f, 0.2f, 0.1f);
            }
            for (int i = 0; i < numBars; i++) {
                GameObject sgao = new GameObject($"Fence {i}");
                sgao.layer = LayerMask.NameToLayer("3D Collision");
                sgao.transform.SetParent(gao.transform);
                sgao.transform.localScale = Vector3.one;
                MeshFilter smf = sgao.AddComponent<MeshFilter>();
                smf.mesh = climbMesh;
                switch (type) {
                    case AdditionalTypeFlags.ClimbUpRight:
                        sgao.transform.localPosition = new Vector3(0, baseHeight + i + 0.5f, -0.55f);
                        break;
                    case AdditionalTypeFlags.ClimbUpLeft:
                        sgao.transform.localPosition = new Vector3(0.55f, baseHeight + i + 0.5f, 0);
                        sgao.transform.localRotation = Quaternion.Euler(0,90,0);
                        break;
                }
                MeshRenderer smr = sgao.AddComponent<MeshRenderer>();
                smr.sharedMaterial = mat;
                //smr.material.color = color;
            }
        }
        void CreateBox(GameObject gao, Material mat, Color color, float height) {
            if (boxMesh == null) {
                boxMesh = GeometryHelpers.CreateBoxDifferentHeights(1, 1, 1, 1, 1);
            }
            GameObject sgao = new GameObject($"Box");
            sgao.layer = LayerMask.NameToLayer("3D Collision");
            sgao.transform.SetParent(gao.transform);
            sgao.transform.localScale = new Vector3(1,height,1);
            sgao.transform.localPosition = Vector3.zero;
            MeshFilter smf = sgao.AddComponent<MeshFilter>();
            smf.mesh = boxMesh;
            MeshRenderer smr = sgao.AddComponent<MeshRenderer>();
            smr.sharedMaterial = mat;
            SetRendererColor(smr, color);
            //smr.material.color = color;
        }
        void CreateSlope(GameObject gao, Material mat, ShapeType shape, Color color, float height) {
            if (slopeMesh == null) {
                slopeMesh = GeometryHelpers.CreateBoxDifferentHeights(1, 1, 1, 0, 0);
            }
            GameObject sgao = new GameObject($"Box");
            sgao.layer = LayerMask.NameToLayer("3D Collision");
            sgao.transform.SetParent(gao.transform);
            sgao.transform.localScale = new Vector3(1, 1, 1);
            sgao.transform.localPosition = new Vector3(0,height,0);
            if (shape == ShapeType.SlopeUpLeft) {
                sgao.transform.localRotation = Quaternion.Euler(0, -90, 0);
            }
            MeshFilter smf = sgao.AddComponent<MeshFilter>();
            smf.mesh = slopeMesh;
            MeshRenderer smr = sgao.AddComponent<MeshRenderer>();
            smr.sharedMaterial = mat;
            SetRendererColor(smr, color);
            //smr.material.color = color;
        }
        void CreatePlane(GameObject gao, Material mat, Color color) {
            if (planeMesh == null) {
                planeMesh = GeometryHelpers.CreateBoxDifferentHeights(1, 0,0,0,0);
            }
            GameObject sgao = new GameObject($"Plane");
            sgao.layer = LayerMask.NameToLayer("3D Collision");
            sgao.transform.SetParent(gao.transform);
            sgao.transform.localScale = Vector3.one;
            sgao.transform.localPosition = Vector3.zero;
            MeshFilter smf = sgao.AddComponent<MeshFilter>();
            smf.mesh = planeMesh;
            MeshRenderer smr = sgao.AddComponent<MeshRenderer>();
            smr.sharedMaterial = mat;
            SetRendererColor(smr, color);
            //smr.material.color = color;
        }

        public GameObject GetGameObject(GameObject parent, int x, int y, Material mat, Unity_IsometricCollisionTile[] collisionData, int levelWidth, int levelHeight) {
            GameObject gao = new GameObject();
            gao.name = DebugText;

            gao.layer = LayerMask.NameToLayer("3D Collision");
            gao.transform.SetParent(parent.transform);
            gao.transform.localScale = Vector3.one;
            gao.transform.localPosition = new Vector3(x + 0.5f, 0, -y - 0.5f);
            //MeshFilter mf = gao.AddComponent<MeshFilter>();


            var color = new Color(CollisionColors[Type].r, CollisionColors[Type].g, CollisionColors[Type].b);
            if ((x + y) % 2 == 1) {
                float h, s, v;
                Color.RGBToHSV(color, out h, out s, out v);
                v -= 0.025f;
                color = Color.HSVToRGB(h, s, v);
            }

            switch (Shape) {
                case ShapeType.SlopeUpRight:
                case ShapeType.SlopeUpLeft:
                    if (Height > 0) CreateBox(gao, mat, color, Height);
                    CreateSlope(gao, mat, Shape, color, Height);
                    //mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(1, Height + 1, Height + 1, Height, Height, color: color);
                    break;
                    //mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(1, Height + 1, Height, Height, Height + 1, color: color);
                    //break;
                default:
                    if (Height > 0) {
                        CreateBox(gao, mat, color, Height);
                    } else {
                        CreatePlane(gao, mat, color);
                    }
                    //mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(1, Height, Height, Height, Height, color: color);
                    break;
            }
            MeshRenderer mr = gao.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mat;

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

        public GameObject GetGameObjectCollider(GameObject parent, int x, int y) {
            GameObject gao = new GameObject();
            gao.name = $"{x},{y}";

            gao.layer = LayerMask.NameToLayer("3D Collision");
            gao.transform.SetParent(parent.transform);
            gao.transform.localScale = Vector3.one;
            gao.transform.localPosition = new Vector3(x + 0.5f, 0, -y - 0.5f);
            BoxCollider bc = gao.AddComponent<BoxCollider>();
            bc.center = Vector3.up * Height / 2f;
            bc.size = new Vector3(1f,Height,1f);
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
            HubworldPit,
            Pit,
            WaterFlowBottomLeft,
            WaterFlowBottomRight,
            Trigger,
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
            [CollisionType.Wall] = new Color(50 / 255f, 55 / 255f, 64 / 255f),

            [CollisionType.Water] = new Color(51 / 255f, 126 / 255f, 255 / 255f),
            [CollisionType.WaterFlowBottomLeft] = new Color(49 / 255f, 100 / 255f, 189 / 255f),
            [CollisionType.WaterFlowBottomRight] = new Color(55 / 255f, 93 / 255f, 161) / 255f,
            [CollisionType.FreezableWater] = new Color(103 / 255f, 99 / 255f, 235 / 255f),

            [CollisionType.Lava] = new Color(255 / 255f, 119 / 255f, 0 / 255f),
            [CollisionType.ObstacleHurt] = new Color(212 / 255f, 32 / 255f, 32 / 255f),
            [CollisionType.Pit] = new Color(158 / 255f, 121 / 255f, 0 / 255f),
            [CollisionType.HubworldPit] = new Color(212 / 255f, 175 / 255f, 57 / 255f),

            [CollisionType.Trigger] = new Color(194 / 255f, 191 / 255f, 25 / 255f),
            [CollisionType.ExitTrigger] = new Color(194 / 255f, 191 / 255f, 25 / 255f),
            [CollisionType.NearExitTrigger] = new Color(155 / 255f, 194 / 255f, 25 / 255f),
            [CollisionType.DialogueTrigger1] = new Color(23 / 255f, 145 / 255f, 38 / 255f),
            [CollisionType.DialogueTrigger2] = new Color(23 / 255f, 145 / 255f, 38 / 255f),
            [CollisionType.DialogueTrigger3] = new Color(23 / 255f, 145 / 255f, 38 / 255f),
            [CollisionType.DialogueTrigger4] = new Color(23 / 255f, 145 / 255f, 38 / 255f),
        };
    }
}