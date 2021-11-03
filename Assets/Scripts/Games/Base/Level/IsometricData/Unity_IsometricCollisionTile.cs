using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ray1Map
{
    public class Unity_IsometricCollisionTile
    {
        #region Public Properties
        public CollisionType Type { get; set; }
        public AdditionalTypeFlags AddType { get; set; }
        public ShapeType Shape { get; set; }
        public float Height { get; set; }
        public string DebugText { get; set; }
        public int? GBAVV_Rotation { get; set; }
        public float? GBAVV_AdditionalHeight { get; set; }
        #endregion

        #region Methods
        void AddFence(GameObject gao, AdditionalTypeFlags type, Material mat, Color color, float height, List<MeshFilter> addMeshes, float fenceHeight = 4f) {
            int numBars = 3;
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
                addMeshes.Add(smf);
                /*MeshRenderer smr = sgao.AddComponent<MeshRenderer>();
                smr.sharedMaterial = mat;*/
            }
        }

        void AddClimb(GameObject gao, AdditionalTypeFlags type, Material mat, Color color, float height, float baseHeight, List<MeshFilter> addMeshes) {
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
                addMeshes.Add(smf);
                /*MeshRenderer smr = sgao.AddComponent<MeshRenderer>();
                smr.sharedMaterial = mat;*/
            }
        }

        public int GetMeshTriangleCount() {
            const int numParts = 5;
            switch (Type) {
                case CollisionType.GBAVV_Slope_4:
                    return 12; // Regular slope
                case CollisionType.GBAVV_SlopeCorner_5:
                    return 12; // Regular slope corner
                case CollisionType.GBAVV_Corner_6:
                case CollisionType.GBAVV_Corner_12:
                case CollisionType.GBAVV_Corner_17:
                case CollisionType.GBAVV_Corner_19:
                case CollisionType.GBAVV_Corner_21:
                case CollisionType.GBAVV_Corner_22:
                case CollisionType.GBAVV_Corner_24:
                case CollisionType.GBAVV_Corner_25:
                case CollisionType.GBAVV_Corner_26:
                case CollisionType.GBAVV_Corner_28:
                case CollisionType.GBAVV_Corner_30:
                case CollisionType.GBAVV_Corner_32:
                case CollisionType.GBAVV_Corner_35:
                    return 16; // Box with top split into 2
                case CollisionType.GBAVV_Ramp_CornerInward_2:
                case CollisionType.GBAVV_Ramp_CornerInward_15:
                case CollisionType.GBAVV_Ramp_CornerInward_33: // Corner inward
                case CollisionType.GBAVV_Ramp_CornerInward_29:
                    return 12 + 1 * numParts * numParts * 2 + numParts;
                case CollisionType.GBAVV_Ramp_CornerOutward_20:
                case CollisionType.GBAVV_Ramp_CornerOutward_34: // Corner outward
                case CollisionType.GBAVV_Ramp_CornerOutward_31:
                    return 12 + 1 * numParts * numParts * 2 + numParts * 3;
                case CollisionType.GBAVV_Ramp_1:
                    return 12 + 12 * (1+numParts);
                default:
                    return 12; // Box
            }
        }

        public MeshFilter GetGameObject(GameObject parent, int x, int y, Material mat, Unity_IsometricCollisionTile[] collisionData, int levelWidth, int levelHeight, List<MeshFilter> addMeshes) {
            GameObject gao = new GameObject();
            gao.name = DebugText;

            gao.layer = LayerMask.NameToLayer("3D Collision");
            gao.transform.SetParent(parent.transform);
            gao.transform.localScale = Vector3.one;
            gao.transform.localPosition = new Vector3(x + 0.5f, 0, -y - 0.5f);
            
            Color color;

            if (CollisionColors.ContainsKey(Type)) {
                color = new Color(CollisionColors[Type].r, CollisionColors[Type].g, CollisionColors[Type].b);
            } else {
                UnityEngine.Random.InitState((int)Type);
                color = UnityEngine.Random.ColorHSV(0, 1, 0.2f, 1f, 0.8f, 1.0f);
            }
            if ((x + y) % 2 == 1) {
                float h, s, v;
                Color.RGBToHSV(color, out h, out s, out v);
                v -= 0.025f;
                color = Color.HSVToRGB(h, s, v);
            }

            MeshFilter mf = gao.AddComponent<MeshFilter>();
            switch (Type) {
                case CollisionType.GBAVV_Slope_4:
                    // Regular slope
                    float AddHeight = Height + 0.1875f;
                    switch (GBAVV_Rotation) {
                        case 0:
                            mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(1, Height, Height, AddHeight, AddHeight, color: color);
                            break;
                        case 1:
                            mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(1, AddHeight, Height, Height, AddHeight, color: color);
                            break;
                        case 2:
                            mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(1, AddHeight, AddHeight, Height, Height, color: color);
                            break;
                        case 3:
                            mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(1, Height, AddHeight, AddHeight, Height, color: color);
                            break;
                        default:
                            break;
                    }
                    break;
                case CollisionType.GBAVV_SlopeCorner_5: // Regular slope corner
                    mf.mesh = GeometryHelpers.CreateSlopeCornerOutward(1, Height, Height + 0.1875f, GBAVV_Rotation ?? 0, color: color);
                    break;
                case CollisionType.GBAVV_Corner_6:
                case CollisionType.GBAVV_Corner_12:
                case CollisionType.GBAVV_Corner_17:
                case CollisionType.GBAVV_Corner_19:
                case CollisionType.GBAVV_Corner_21:
                case CollisionType.GBAVV_Corner_22:
                case CollisionType.GBAVV_Corner_24:
                case CollisionType.GBAVV_Corner_25:
                case CollisionType.GBAVV_Corner_26:
                case CollisionType.GBAVV_Corner_28:
                case CollisionType.GBAVV_Corner_30:
                case CollisionType.GBAVV_Corner_32:
                case CollisionType.GBAVV_Corner_35:
                    mf.mesh = GeometryHelpers.CreateDoubleSplitBox(1, Height, (GBAVV_AdditionalHeight ?? 0f) + Height, GBAVV_Rotation ?? 0, color: color);
                    break;
                case CollisionType.GBAVV_Ramp_CornerInward_2:
                case CollisionType.GBAVV_Ramp_CornerInward_15:
                case CollisionType.GBAVV_Ramp_CornerInward_29:
                case CollisionType.GBAVV_Ramp_CornerInward_33: // Corner inward
                    mf.mesh = GeometryHelpers.CreateRampCornerInward(1, Height, Height + 0.1875f, 5, GBAVV_Rotation ?? 0, color: color);
                    break;
                case CollisionType.GBAVV_Ramp_CornerOutward_20:
                case CollisionType.GBAVV_Ramp_CornerOutward_31:
                case CollisionType.GBAVV_Ramp_CornerOutward_34: // Corner outward
                    mf.mesh = GeometryHelpers.CreateRampCornerOutward(1, Height, Height + 0.1875f, 5, GBAVV_Rotation ?? 0, color: color);
                    break;
                case CollisionType.GBAVV_Ramp_1:
                    mf.mesh = GeometryHelpers.CreateRamp(1, Height, Height + 0.1875f, 5, GBAVV_Rotation ?? 0, color: color);
                    break;
                case CollisionType.GBAVV_Fence_13:
                case CollisionType.GBAVV_Fence_16:
                case CollisionType.GBAVV_Fence_18:
                case CollisionType.GBAVV_Fence_23:
                    mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(1, Height, Height, Height, Height, color: color);
                    var addType = AdditionalTypeFlags.FenceDownRight;
                    switch ((GBAVV_Rotation ?? 0) % 4) {
                        case 0: addType = AdditionalTypeFlags.FenceDownRight; break;
                        case 1: addType = AdditionalTypeFlags.FenceDownLeft; break;
                        case 2: addType = AdditionalTypeFlags.FenceUpLeft; break;
                        case 3: addType = AdditionalTypeFlags.FenceUpRight; break;
                    }
                    AddFence(gao, addType, mat, color, Height, addMeshes, fenceHeight: (GBAVV_AdditionalHeight ?? 0));
                    break;
                default:
                    switch (Shape) {
                        case ShapeType.SlopeUpRight:
                            mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(1, Height + 1, Height + 1, Height, Height, color: color);
                            break;
                        case ShapeType.SlopeUpLeft:
                            mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(1, Height + 1, Height, Height, Height + 1, color: color);
                            break;
                        default:
                            mf.mesh = GeometryHelpers.CreateBoxDifferentHeights(1, Height, Height, Height, Height, color: color);
                            break;
                    }
                    break;
            }
            /*MeshRenderer mr = gao.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mat;*/
            if (AddType.HasFlag(AdditionalTypeFlags.FenceUpLeft_RHR)) {
                var neighborBlock = x > 0 ? collisionData[y * levelWidth + (x - 1)] : null;
                var maxHeight = Math.Max(Height, neighborBlock?.Height ?? 0);
                AddFence(gao, AdditionalTypeFlags.FenceUpLeft, mat, color, maxHeight, addMeshes);
            }
            if (AddType.HasFlag(AdditionalTypeFlags.FenceUpRight_RHR)) {
                var neighborBlock = y > 0 ? collisionData[(y - 1) * levelWidth + x] : null;
                var maxHeight = Math.Max(Height, neighborBlock?.Height ?? 0);
                AddFence(gao, AdditionalTypeFlags.FenceUpRight, mat, color, maxHeight, addMeshes);
            }
            if (AddType.HasFlag(AdditionalTypeFlags.FenceUpLeft)) {
                AddFence(gao, AdditionalTypeFlags.FenceUpLeft, mat, color, Height, addMeshes);
            }
            if (AddType.HasFlag(AdditionalTypeFlags.FenceUpRight)) {
                AddFence(gao, AdditionalTypeFlags.FenceUpRight, mat, color, Height, addMeshes);
            }
            if (AddType.HasFlag(AdditionalTypeFlags.FenceDownRight)) {
                AddFence(gao, AdditionalTypeFlags.FenceDownRight, mat, color, Height, addMeshes);
            }
            if (AddType.HasFlag(AdditionalTypeFlags.FenceDownLeft)) {
                AddFence(gao, AdditionalTypeFlags.FenceDownLeft, mat, color, Height, addMeshes);
            }

            if (AddType.HasFlag(AdditionalTypeFlags.ClimbUpLeft)) {
                var neighborBlock = x + 1 < levelWidth ? collisionData[y * levelWidth + (x + 1)] : null;
                var baseHeight = neighborBlock?.Height ?? 0;
                AddClimb(gao, AdditionalTypeFlags.ClimbUpLeft, mat, color, Height, baseHeight, addMeshes);
            }
            if (AddType.HasFlag(AdditionalTypeFlags.ClimbUpRight)) {
                var neighborBlock = y + 1 < levelHeight ? collisionData[(y + 1) * levelWidth + x] : null;
                var baseHeight = neighborBlock?.Height ?? 0;
                AddClimb(gao, AdditionalTypeFlags.ClimbUpRight, mat, color, Height, baseHeight, addMeshes);
            }
            return mf;
        }

        public GameObject GetGameObjectCollider(GameObject parent, int x, int y) {
            GameObject gao = new GameObject();
            gao.name = $"{x},{y}";
            float height = Height + (GBAVV_AdditionalHeight ?? 0);
            gao.layer = LayerMask.NameToLayer("3D Collision");
            var col3D = gao.AddComponent<Unity_Collision3DBehaviour>();
            col3D.IsometricTile = this;
            col3D.IsometricPosition = new Vector2Int(x,y);
            gao.transform.SetParent(parent.transform);
            gao.transform.localScale = Vector3.one;
            gao.transform.localPosition = new Vector3(x + 0.5f, 0, -y - 0.5f);
            BoxCollider bc = gao.AddComponent<BoxCollider>();
            bc.center = Vector3.up * height / 2f;
            bc.size = new Vector3(1f,height,1f);
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

            // GBA Milan

            Type_1,
            Type_2,
            Type_3,
            Type_4,
            Type_5,
            Type_6,
            Type_7,

            // GBAVV

            GBAVV_Solid_0,
            GBAVV_Ramp_1,
            GBAVV_Ramp_CornerInward_2,
            GBAVV_3,
            GBAVV_Slope_4,
            GBAVV_SlopeCorner_5,
            GBAVV_Corner_6,
            GBAVV_7,
            GBAVV_8,
            GBAVV_9,
            GBAVV_10,
            GBAVV_11,
            GBAVV_Corner_12,
            GBAVV_Fence_13,
            GBAVV_14,
            GBAVV_Ramp_CornerInward_15,
            GBAVV_Fence_16,
            GBAVV_Corner_17,
            GBAVV_Fence_18,
            GBAVV_Corner_19,
            GBAVV_Ramp_CornerOutward_20,
            GBAVV_Corner_21,
            GBAVV_Corner_22,
            GBAVV_Fence_23,
            GBAVV_Corner_24,
            GBAVV_Corner_25,
            GBAVV_Corner_26,
            GBAVV_Solid_27,
            GBAVV_Corner_28,
            GBAVV_Ramp_CornerInward_29,
            GBAVV_Corner_30,
            GBAVV_Ramp_CornerOutward_31,
            GBAVV_Corner_32,
            GBAVV_Ramp_CornerInward_33,
            GBAVV_Ramp_CornerOutward_34,
            GBAVV_Corner_35,
            GBAVV_36,
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

            // GBA Milan
            
            [CollisionType.Type_1] = new Color(145 / 255f, 92 / 255f, 78 / 255f),
            [CollisionType.Type_2] = new Color(145 / 255f, 128 / 255f, 78 / 255f),
            [CollisionType.Type_3] = new Color(78 / 255f, 145 / 255f, 87 / 255f),
            [CollisionType.Type_4] = new Color(78 / 255f, 145 / 255f, 120 / 255f),
            [CollisionType.Type_5] = new Color(78 / 255f, 126 / 255f, 145 / 255f),
            [CollisionType.Type_6] = new Color(95 / 255f, 78 / 255f, 145 / 255f),
            [CollisionType.Type_7] = new Color(138 / 255f, 78 / 255f, 145 / 255f),

            // Crash
            [CollisionType.GBAVV_Solid_0] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Solid_27] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Slope_4] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_SlopeCorner_5] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Corner_6] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Corner_12] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Corner_17] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Corner_19] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Corner_21] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Corner_22] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Corner_24] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Corner_25] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Corner_26] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Corner_28] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Corner_30] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Corner_32] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Corner_35] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Ramp_CornerInward_2] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Ramp_CornerInward_15] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Ramp_CornerInward_29] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Ramp_CornerInward_33] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Ramp_CornerOutward_20] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Ramp_CornerOutward_31] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Ramp_CornerOutward_34] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Ramp_1] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Fence_13] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Fence_16] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Fence_18] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
            [CollisionType.GBAVV_Fence_23] = new Color(88 / 255f, 98 / 255f, 115 / 255f),
        };
    }
}