using System.Collections.Generic;
using System.Linq;
using BinarySerializer.PlayStation.PS1;
using UnityEngine;

namespace Ray1Map
{
    public class TMDGameObject
    {
        public TMDGameObject(TMD tmd, VRAM vram, float scale)
        {
            TMD = tmd;
            VRAM = vram;
            Scale = scale;
        }

        public TMD TMD { get; }
        public VRAM VRAM { get; }
        public float Scale { get; }

        public bool HasAnimations { get; protected set; }

        protected Vector3 ToVertex(TMD_Vertex v) => new Vector3(v.X / Scale, -v.Y / Scale, v.Z / Scale);
        protected Vector3 ToNormal(TMD_Normal n) => new Vector3(n.X, -n.Y, n.Z);

        protected virtual void OnGetTextureBounds(TMD_Packet packet, PS1VRAMTexture tex) { }
        protected virtual void OnCreateTexture(PS1VRAMTexture tex) { }
        protected virtual void OnCreatedBones(GameObject gameObject, TMD_Object obj, Transform[] bones) { }
        protected virtual void OnCreateObject(GameObject gameObject, GameObject primitivesGameObject, TMD_Object obj, int objIndex) { }
        protected virtual void OnCreatedPrimitives(GameObject gameObject, TMD_Object obj, int objIndex, Mesh[] primitiveMeshes) { }
        protected virtual void OnAppliedTexture(GameObject packetGameObject, TMD_Object obj, TMD_Packet packet, Material mat, PS1VRAMTexture tex) { }
        protected virtual void OnCreatedObjects(GameObject parentGameObject, GameObject[] objects, Transform[][] allBones) { }

        protected virtual Mesh AddPrimitive(GameObject parent, TMD_Object obj, int objIndex, int packetIndex, Dictionary<TMD_Packet, PS1VRAMTexture> vramTexturesLookup, Transform[] bones, Matrix4x4[] bindPoses, bool includeDebugInfo, bool loadTextures)
        {
            // Helper method
            int getBoneForVertex(int vertexIndex)
            {
                if (obj.Bones == null)
                    return -1;

                return obj.Bones.FindItemIndex(b => b.VerticesIndex <= vertexIndex && b.VerticesIndex + b.VerticesCount > vertexIndex) + 1;
            }

            TMD_Packet packet = obj.Primitives[packetIndex];

            //if (!packet.Flags.HasFlag(PS1_TMD_Packet.PacketFlags.LGT))
            //    Debug.LogWarning($"Packet has light source");

            if (packet.Mode.Code != TMD_PacketMode.PacketModeCODE.Polygon)
            {
                if (packet.Mode.Code != 0)
                    Debug.LogWarning($"Skipped packet with code {packet.Mode.Code}");

                return null;
            }

            Mesh unityMesh = new Mesh();

            var vertices = packet.Vertices.Select(x => ToVertex(obj.Vertices[x])).ToArray();

            Vector3[] normals = null;

            if (packet.Normals != null)
            {
                normals = packet.Normals.Select(x => ToNormal(obj.Normals[x])).ToArray();
                if (normals.Length == 1)
                    normals = Enumerable.Repeat(normals[0], vertices.Length).ToArray();
            }

            // Set vertices
            unityMesh.SetVertices(vertices);

            // Set normals
            if (normals != null)
                unityMesh.SetNormals(normals);

            int[] triangles = GetTriangles(packet);

            unityMesh.SetTriangles(triangles, 0);

            Color[] colors = TMD.Pre_HasColorTable 
                ? packet.RGBIndices.Select(x => obj.Colors[x].GetColor()).ToArray() 
                : packet.RGB.Select(x => x.GetColor()).ToArray();

            if (colors.Length == 1)
                colors = Enumerable.Repeat(colors[0], vertices.Length).ToArray();

            unityMesh.SetColors(colors);

            if (normals == null)
                unityMesh.RecalculateNormals();

            if (bones != null)
            {
                BoneWeight[] weights = packet.Vertices.Select(x => new BoneWeight()
                {
                    boneIndex0 = getBoneForVertex(x),
                    weight0 = 1f
                }).ToArray();
                unityMesh.boneWeights = weights;
                unityMesh.bindposes = bindPoses;
            }

            GameObject gao = new GameObject($"Packet_{packetIndex} Offset:{packet.Offset} Flags:{packet.Flags}");

            MeshFilter mf = gao.AddComponent<MeshFilter>();
            gao.layer = LayerMask.NameToLayer("3D Collision");
            gao.transform.SetParent(parent.transform, false);
            gao.transform.localScale = Vector3.one;
            gao.transform.localPosition = Vector3.zero;
            mf.sharedMesh = unityMesh;

            Material mat;

            if (packet.Mode.ABE)
                mat = new Material(Controller.obj.levelController.controllerTilemap.unlitAdditiveMaterial);
            else
                mat = new Material(Controller.obj.levelController.controllerTilemap.unlitTransparentCutoutMaterial);

            if (bones != null)
            {
                SkinnedMeshRenderer smr = gao.AddComponent<SkinnedMeshRenderer>();
                smr.sharedMaterial = mat;
                smr.sharedMesh = unityMesh;
                smr.bones = bones;
                smr.rootBone = bones[0];
            }
            else
            {
                MeshRenderer mr = gao.AddComponent<MeshRenderer>();
                mr.sharedMaterial = mat;
            }

            // Apply texture
            if (packet.Mode.TME && loadTextures)
                ApplyTexture(vramTexturesLookup[packet], mat, unityMesh, obj, packet, gao, objIndex, packetIndex, includeDebugInfo);

            return unityMesh;
        }

        protected virtual int[] GetTriangles(TMD_Packet packet)
        {
            if (packet.Mode.IsQuad)
            {
                if (packet.Flags.HasFlag(TMD_Packet.PacketFlags.FCE))
                {
                    return new int[]
                    {
                        // Lower left triangle
                        0, 1, 2, 0, 2, 1,
                        // Upper right triangle
                        3, 2, 1, 3, 1, 2,
                    };
                }
                else
                {
                    return new int[]
                    {
                        // Lower left triangle
                        0, 1, 2,
                        // Upper right triangle
                        3, 2, 1,
                    };
                }
            }
            else
            {
                if (packet.Flags.HasFlag(TMD_Packet.PacketFlags.FCE))
                {
                    return new int[]
                    {
                        0, 1, 2, 0, 2, 1,
                    };
                }
                else
                {
                    return new int[]
                    {
                        0, 1, 2,
                    };
                }
            }
        }

        protected virtual void ApplyTexture(PS1VRAMTexture tex, Material mat, Mesh unityMesh, TMD_Object obj, TMD_Packet packet, GameObject packetGameObject, int objIndex, int packetIndex, bool includeDebugInfo)
        {
            mat.SetTexture("_MainTex", tex.Texture);

            unityMesh.SetUVs(0, packet.UV.Select((uv, i) =>
            {
                var u = uv.U - tex.Bounds.x;
                var v = uv.V - tex.Bounds.y;
                if (i % 2 == 1) u += 1;
                if (i >= 2) v += 1;

                return new Vector2(u / (float)(tex.Bounds.width), v / (float)(tex.Bounds.height));
            }).ToArray());

            if (tex.AnimatedTexture != null)
            {
                HasAnimations = true;
                var animTex = packetGameObject.AddComponent<AnimatedTextureComponent>();
                animTex.material = mat;
                animTex.animatedTextures = tex.AnimatedTexture.Textures;
                animTex.speed = new AnimSpeed_FrameDelay(tex.AnimatedTexture.Speed);
            }

            if (includeDebugInfo)
                packetGameObject.name = $"{packet.Offset}: {objIndex}-{packetIndex} TX: {packet.TSB.TX}, TY:{packet.TSB.TY}, F:{packet.Flags}, ABE:{packet.Mode.ABE}, TGE:{packet.Mode.TGE}, ABR: {packet.TSB.ABR}, IsAnimated: {tex.AnimatedTexture != null}";

            OnAppliedTexture(packetGameObject, obj, packet, mat, tex);
        }

        public GameObject CreateGameObject(string name, bool includeDebugInfo, bool loadTextures = true)
        {
            HasAnimations = false;

            GameObject gaoParent = new GameObject(name);
            gaoParent.transform.position = Vector3.zero;

            var vramTextures = new HashSet<PS1VRAMTexture>();
            var vramTexturesLookup = new Dictionary<TMD_Packet, PS1VRAMTexture>();

            if (loadTextures)
            {
                // Get texture bounds
                foreach (TMD_Packet packet in TMD.Objects.SelectMany(x => x.Primitives).Where(x => x.Mode.TME))
                {
                    var tex = new PS1VRAMTexture(packet.TSB, packet.CBA, packet.UV);

                    PS1VRAMTexture overlappingTex = vramTextures.FirstOrDefault(x => x.HasOverlap(tex));

                    OnGetTextureBounds(packet, tex);

                    if (overlappingTex != null)
                    {
                        overlappingTex.ExpandWithBounds(tex);
                        vramTexturesLookup.Add(packet, overlappingTex);
                    }
                    else
                    {
                        vramTextures.Add(tex);
                        vramTexturesLookup.Add(packet, tex);
                    }
                }

                // Create textures
                foreach (PS1VRAMTexture vramTex in vramTextures)
                {
                    // Create the default texture
                    vramTex.SetTexture(vramTex.GetTexture(VRAM));

                    OnCreateTexture(vramTex);
                }
            }

            var allBones = new Transform[TMD.Objects.Length][];
            var objects = new GameObject[TMD.Objects.Length];

            // Create each object
            for (var objIndex = 0; objIndex < TMD.Objects.Length; objIndex++)
            {
                var obj = TMD.Objects[objIndex];

                // Create two objects so that if there are no bones then the transform we use is not the same as the main object
                GameObject gameObject = new GameObject($"Object_{objIndex} Offset:{obj.Offset}");
                gameObject.transform.SetParent(gaoParent.transform, false);
                gameObject.transform.localScale = Vector3.one;

                objects[objIndex] = gameObject;

                // Init bones
                bool hasBones = obj.BonesCount > 0;
                Transform[] bones = null;
                Matrix4x4[] bindPoses = null;

                if (hasBones)
                {
                    bones = new Transform[obj.Bones.Length + 1];

                    for (int i = 0; i < bones.Length; i++)
                    {
                        var b = new GameObject($"Bone {i}");
                        b.transform.SetParent(gameObject.transform);
                        bones[i] = b.transform;
                    }

                    // Init Root bone
                    {
                        var b = bones[0];
                        b.transform.localPosition = Vector3.zero;
                        b.transform.localRotation = Quaternion.identity;
                        b.transform.localScale = Vector3.one;
                    }
                    // Init other bones
                    for (int i = 0; i < obj.Bones.Length; i++)
                    {
                        var b = bones[i + 1];
                        b.transform.SetParent(bones[obj.Bones[i].ParentIndex]);
                        b.transform.localPosition = Vector3.zero;
                        b.transform.localRotation = Quaternion.identity;
                        b.transform.localScale = Vector3.one;
                    }

                    bindPoses = new Matrix4x4[bones.Length];

                    for (int i = 0; i < bindPoses.Length; i++)
                        bindPoses[i] = bones[i].worldToLocalMatrix * gameObject.transform.localToWorldMatrix;

                    // Gets set from animations later, ignore for now since it's the same position data
                    //for (int i = 0; i < obj.Bones.Length; i++)
                    //{
                    //    var b = bones[i + 1];
                    //    b.transform.localPosition = new Vector3(obj.Bones[i].XPos / Scale, -obj.Bones[i].YPos / Scale, obj.Bones[i].ZPos / Scale);
                    //}

                    OnCreatedBones(gameObject, obj, bones);
                }

                allBones[objIndex] = bones ?? new Transform[] { gameObject.transform };

                GameObject primitivesGameObj = new GameObject($"Primitives");
                primitivesGameObj.transform.SetParent(gameObject.transform, false);

                OnCreateObject(gameObject, primitivesGameObj, obj, objIndex);

                Mesh[] meshes = new Mesh[obj.Primitives.Length];

                // Add each primitive
                for (var packetIndex = 0; packetIndex < obj.Primitives.Length; packetIndex++)
                    meshes[packetIndex] = AddPrimitive(primitivesGameObj, obj, objIndex, packetIndex, vramTexturesLookup, bones, bindPoses, includeDebugInfo, loadTextures);

                OnCreatedPrimitives(gameObject, obj, objIndex, meshes);
            }

            OnCreatedObjects(gaoParent, objects, allBones);

            return gaoParent;
        }
    }
}