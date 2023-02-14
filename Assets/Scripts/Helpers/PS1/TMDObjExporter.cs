using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer.PS1;
using UnityEngine;

namespace Ray1Map
{
    public class TMDObjExporter
    {
        protected Vector3 ToVertex(TMD_Vertex v, float scale) => new Vector3(-v.X / scale, -v.Y / scale, v.Z / scale);
        protected Vector3 ToNormal(TMD_Normal n) => new Vector3(n.X, -n.Y, n.Z);

        public void Export(string dir, string name, TMD tmd, VRAM vram, float scale)
        {
            string exportDir = Path.Combine(dir, name);
            Directory.CreateDirectory(exportDir);

            using StreamWriter objWriter = new StreamWriter(Path.Combine(exportDir, "obj0.obj"));
            objWriter.WriteLine("mtllib mtl0.mtl");

            var vramTextures = new List<PS1VRAMTexture>();
            var vramTexturesLookup = new Dictionary<TMD_Packet, PS1VRAMTexture>();

            // Get texture bounds
            foreach (TMD_Packet packet in tmd.Objects.SelectMany(x => x.Primitives).Where(x => x.Mode.TME))
            {
                var tex = new PS1VRAMTexture(packet.TSB, packet.CBA, packet.UV);

                PS1VRAMTexture overlappingTex = vramTextures.FirstOrDefault(x => x.HasOverlap(tex));

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
                vramTex.SetTexture(vramTex.GetTexture(vram));
            }

            // Vertices
            foreach (TMD_Object obj in tmd.Objects)
            {
                foreach (TMD_Packet p in obj.Primitives)
                {
                    foreach (Vector3 vertex in p.Vertices.Select(x => ToVertex(obj.Vertices[x], scale)))
                    {
                        objWriter.WriteLine($"v {vertex.x} {vertex.y} {vertex.z}");
                    }
                }
            }
            
            //// Normals
            //foreach (PS1_TMD_Object obj in tmd.Objects)
            //{
            //    foreach (PS1_TMD_Packet p in obj.Primitives)
            //    {
            //        foreach (Vector3 normal in p.Normals.Select(x => ToNormal(obj.Normals[x])))
            //        {
            //            objWriter.WriteLine($"vn {normal.x} {normal.y} {normal.z}");
            //        }
            //    }
            //}

            // UVs
            foreach (TMD_Object obj in tmd.Objects)
            {
                foreach (TMD_Packet p in obj.Primitives)
                {
                    PS1VRAMTexture tex = vramTexturesLookup[p];

                    for (var i = 0; i < p.UV.Length; i++)
                    {
                        TMD_UV uv = p.UV[i];
                        var u = uv.U - tex.Bounds.x;
                        var v = uv.V - tex.Bounds.y;

                        if (i % 2 == 1)
                            u += 1;

                        if (i >= 2)
                            v += 1;

                        objWriter.WriteLine($"vt {u / (float)(tex.Bounds.width)} {v / (float)(tex.Bounds.height)}");
                    }
                }
            }

            int groupIndex = 0;
            int triIndex = 1;
            foreach (TMD_Object obj in tmd.Objects)
            {
                foreach (TMD_Packet p in obj.Primitives)
                {
                    PS1VRAMTexture tex = vramTexturesLookup[p];

                    objWriter.WriteLine($"g group{groupIndex++}");
                    objWriter.WriteLine("usemtl mtl{0}", vramTextures.IndexOf(tex));

                    if (p.Vertices.Length == 3)
                        objWriter.WriteLine("f {2}/{2}/ {1}/{1}/ {0}/{0}/", triIndex++, triIndex++, triIndex++);
                    else
                        objWriter.WriteLine("f {2}/{2}/ {3}/{3}/ {1}/{1}/ {0}/{0}/", triIndex++, triIndex++, triIndex++, triIndex++);
                }
            }

            using StreamWriter mtlWriter = new StreamWriter(Path.Combine(exportDir, "mtl0.mtl"));

            for (var i = 0; i < vramTextures.Count; i++)
            {
                PS1VRAMTexture tex = vramTextures[i];
                mtlWriter.WriteLine($"newmtl mtl{i}");
                mtlWriter.WriteLine("Ka 0.00000 0.00000 0.00000");
                mtlWriter.WriteLine("Kd 0.50000 0.50000 0.50000");
                mtlWriter.WriteLine("Ks 0.00000 0.00000 0.00000");
                mtlWriter.WriteLine("d 1.00000");
                mtlWriter.WriteLine("illum 0");
                mtlWriter.WriteLine($"map_Kd {i}.png");

                File.WriteAllBytes(Path.Combine(exportDir, $"{i}.png"), tex.Texture.EncodeToPNG());
            }
        }
    }
}