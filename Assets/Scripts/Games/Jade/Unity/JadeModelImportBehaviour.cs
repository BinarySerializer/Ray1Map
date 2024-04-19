using BinarySerializer;
using BinarySerializer.Image;
using Ray1Map;
using Ray1Map.Jade;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JadeModelImportBehaviour : MonoBehaviour {
    public List<GEO_Object> GetJadeModel(LOA_Loader loader, Func<Jade_Key> newKey) {
        List<GEO_Object> mainObjects = new List<GEO_Object>();

        var mrs = GetComponentsInChildren<MeshRenderer>();

        // Add GEO
        GEO_GeometricObject geo = new GEO_GeometricObject();
        GRO_Struct gro = new GRO_Struct() {
            Type = GRO_Type.GEO,
            Value = geo
        };
        geo.GRO = gro;
        GEO_Object geoMain = new GEO_Object() {
            RenderObject = gro,
            Key = newKey(),
            FileSize = 1
        };
        mainObjects.Add(geoMain);

        // Configure GEO
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Material> materials = new List<Material>();

        List<GEO_GeometricObjectElement> els = new List<GEO_GeometricObjectElement>();
        uint materialIndex = 0;

        for(int i = 0; i < mrs.Length; i++) {
            var mr = mrs[i];
            materials.AddRange(mr.sharedMaterials);
            var mf = mr.GetComponent<MeshFilter>();
            var vertsStartCount = verts.Count;
            var curVertsCount = mf.sharedMesh.vertices.Length;
            verts.AddRange(mf.sharedMesh.vertices.Select(v => mr.transform.TransformPoint(v)));
            verts.AddRange(mf.sharedMesh.vertices.Select(v => mr.transform.TransformPoint(v))); // for backfaces
            uvs.AddRange(mf.sharedMesh.uv);
            var tris = mf.sharedMesh.triangles;

            var subMeshCount = mf.sharedMesh.subMeshCount;
            for (int j = 0; j < subMeshCount; j++) {
                var submesh = mf.sharedMesh.GetSubMesh(j);
                var indStart = submesh.indexStart;
                var indCount = submesh.indexCount;


                var el = new GEO_GeometricObjectElement() {
                    GeometricObject = geo,
                    MaterialID = materialIndex
                };
                els.Add(el);
                materialIndex++;


                el.TrianglesCount = (uint)(indCount / 3) * 2;
                el.Triangles = new GEO_GeometricObjectElement.Triangle[el.TrianglesCount];
                for (int t = 0; t < el.Triangles.Length; t++) {
                    var t2 = t/2;
                    if (t % 2 == 0) {
                        var tri = new GEO_GeometricObjectElement.Triangle() {
                            Vertex0 = (ushort)(vertsStartCount + tris[indStart + t2 * 3 + 1]),
                            Vertex1 = (ushort)(vertsStartCount + tris[indStart + t2 * 3 + 0]),
                            Vertex2 = (ushort)(vertsStartCount + tris[indStart + t2 * 3 + 2]),
                            UV0 = (ushort)(vertsStartCount + tris[indStart + t2 * 3 + 1]),
                            UV1 = (ushort)(vertsStartCount + tris[indStart + t2 * 3 + 0]),
                            UV2 = (ushort)(vertsStartCount + tris[indStart + t2 * 3 + 2]),
                            SmoothingGroup = (uint)i,
                        };
                        el.Triangles[t] = tri;
                    } else {
                        var tri = new GEO_GeometricObjectElement.Triangle() {
                            Vertex0 = (ushort)(vertsStartCount + curVertsCount + tris[indStart + t2 * 3 + 0]),
                            Vertex1 = (ushort)(vertsStartCount + curVertsCount + tris[indStart + t2 * 3 + 1]),
                            Vertex2 = (ushort)(vertsStartCount + curVertsCount + tris[indStart + t2 * 3 + 2]),
                            UV0 = (ushort)(vertsStartCount + tris[indStart + t2 * 3 + 0]),
                            UV1 = (ushort)(vertsStartCount + tris[indStart + t2 * 3 + 1]),
                            UV2 = (ushort)(vertsStartCount + tris[indStart + t2 * 3 + 2]),
                            SmoothingGroup = (uint)i,
                        };
                        el.Triangles[t] = tri;
                    }
                }

            }
        }

        geo.Elements = els.ToArray();
        geo.ElementsCount = (uint)geo.Elements.Length;

        geo.Vertices = verts.Select(v => new Jade_Vector() {
            X = v.x,
            Y = v.z,
            Z = v.y
        }).ToArray();
        geo.VerticesCount = (uint)geo.Vertices.Length;
        geo.Code_00 = geo.VerticesCount;
        geo.UVs = uvs.Select(uv => new GEO_GeometricObject.UV() {
            U = uv.x,
            V = uv.y
        }).ToArray();
        geo.UVsCount = (uint)geo.UVs.Length;

        // Create MSM
        MAT_MSM_MultiSingleMaterial msm = new MAT_MSM_MultiSingleMaterial();
        GRO_Struct groM_main = new GRO_Struct() {
            Type = GRO_Type.MAT_MSM,
            Value = msm,
        };
        GEO_Object geoM_main = new GEO_Object() {
            RenderObject = groM_main,
            Key = newKey(),
            FileSize = 1
        };
        mainObjects.Add(geoM_main);

        // Create MTTs
        List<GEO_Object> materialObjects = GetMaterialObjects(loader, newKey, materials);
        mainObjects.AddRange(materialObjects);

        // Configure MSM
        msm.Materials = materialObjects.Select(m => new Jade_Reference<GEO_Object>(loader.Context, m.Key) {
            Value = m
        }).ToArray();
        msm.Count = (uint)msm.Materials.Length;



        return mainObjects;
    }

    public List<GEO_Object> GetMaterialObjects(LOA_Loader loader, Func<Jade_Key> newKey, List<Material> unityMats) {
        List<GEO_Object> materialObjects = new List<GEO_Object>();

        foreach (var mat in unityMats) {
            var matTex = (Texture2D)mat.GetTexture("_MainTex");
            // Create MTT
            MAT_MTT_MultiTextureMaterial mtt = new MAT_MTT_MultiTextureMaterial();
            GRO_Struct gro = new GRO_Struct() {
                Type = GRO_Type.MAT_MTT,
                Value = mtt
            };
            mtt.GRO = gro;
            GEO_Object geo = new GEO_Object() {
                RenderObject = gro,
                Key = newKey(),
                FileSize = 1
            };
            materialObjects.Add(geo);

            // Configure MTT
            var mttl = matTex != null ? new MAT_MTT_MultiTextureMaterial.MAT_MTT_Level() : null;
            if (mttl != null) {
                mtt.FirstLevelPointer = 1; // Any nonzero value will do.
                mtt.Levels = new MAT_MTT_MultiTextureMaterial.MAT_MTT_Level[1];
                mtt.Levels[0] = mttl;
            }
            mtt.Ambiant = new SerializableColor(0.5f, 0.5f, 0.5f, 1f);
            mtt.Diffuse = new SerializableColor(mat.color.r, mat.color.g, mat.color.b, mat.color.a);
            mtt.Specular = new SerializableColor(0,0,0,0);
            mtt.Opacity = 1f;

            if(mttl == null) continue;

            // Configure MTT Level
            if(matTex.wrapModeU == TextureWrapMode.Repeat)
                mttl.Flags |= MAT_MTT_MultiTextureMaterial.MAT_MTT_Level.MaterialFlags.TilingU;

            if(matTex.wrapModeV == TextureWrapMode.Repeat)
                mttl.Flags |= MAT_MTT_MultiTextureMaterial.MAT_MTT_Level.MaterialFlags.TilingV;

            if(matTex.filterMode != FilterMode.Point)
                mttl.Flags |= MAT_MTT_MultiTextureMaterial.MAT_MTT_Level.MaterialFlags.BilinearFiltering;

            mttl.Source = MAT_MTT_MultiTextureMaterial.MAT_MTT_Level.UVSource.Object1;

            //mttl.ScaleSpeedPosV = 1f; // ???
            mttl.TextureID = 0; // Last texture


            TEX_File texture = CreateTexture(loader, newKey, matTex);
            mttl.Texture = new Jade_TextureReference(loader.Context, texture.Key) {
                Info = texture,
                Content = texture
            };

        }

        return materialObjects;
    }

    public TEX_File CreateTexture(LOA_Loader loader, Func<Jade_Key> newKey, Texture2D tex) {
        TEX_File texFile = new TEX_File() {
            Key = newKey(),
            Mark = -1,
            FileSize = 1
        };
        texFile.Init(loader.BigFiles[0].Offset); // Init with any pointer that has a context with the same R1Settings
        if (tex.width > 512 || tex.height > 512) {
            tex.ResizeImageData(tex.width / 4, tex.height / 4, mipmap: false, filter: FilterMode.Bilinear);
        }
        var pixels = tex.GetPixels();
        texFile.Width = (ushort)tex.width;
        texFile.Height = (ushort)tex.height;
        /*if (Content_DDS != null) {
            // invert y
            Color[] newPixels = new Color[pixels.Length];
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    newPixels[y * Width + x] = pixels[(Height - 1 - y) * Width + x];
                }
            }
            pixels = newPixels;
        }*/
        SerializableColor[] pixelsBaseColor = null;
        uint size = (uint)pixels.Length;

        texFile.Format = TEX_File.TexColorFormat.BPP_32;
        pixelsBaseColor = pixels.Select(c => new SerializableColor(c.r, c.g, c.b, c.a)).ToArray();
        size *= 4;

        texFile.Content_TGA = new TGA() {
            Pre_ColorOrder = TGA.RGBColorOrder.BGR,
            Pre_SkipHeader = true,
            Header = new TGA_Header {
                HasColorMap = false,
                ImageType = TGA_ImageType.UnmappedRGB,
                Width = texFile.Width,
                Height = texFile.Height,
                BitsPerPixel = (byte)(texFile.Format == TEX_File.TexColorFormat.BPP_24 ? 24 : 32)
            },
            RGBImageData = pixelsBaseColor,
        };
        texFile.Content_TGA_Header = texFile.Content_TGA.Header;

        texFile.Type = TEX_File.TexFileType.Tga;
        if (texFile.Info != null) texFile.Info.Type = TEX_File.TexFileType.Tga;
        texFile.FileSize = size
            + 18 // Tga header size
            + 32; // Jade header size

        texFile.Color = new SerializableColor(1f,1f,1f,1f);
        return texFile;
    }
}