using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PsychoPortal;
using PsychoPortal.Unity;

namespace Ray1Map.Psychonauts
{
    public class PsychonautsObjExporter
    {
        public void Export(string exportDir, string name, Mesh mesh, PsychonautsTexture[] textures)
        {
            Directory.CreateDirectory(exportDir);

            using StreamWriter objWriter = new StreamWriter(Path.Combine(exportDir, $"{name}.obj"));
            objWriter.WriteLine($"mtllib {name}.mtl");

            int[] fragVertsIndex = new int[mesh.MeshFrags.Length];
            int[] fragUVsIndex = new int[mesh.MeshFrags.Length];

            // Vertices
            for (int fragIndex = 0; fragIndex < mesh.MeshFrags.Length; fragIndex++)
            {
                MeshFrag frag = mesh.MeshFrags[fragIndex];

                foreach (VertexNotexNorm v in frag.Vertices)
                {
                    objWriter.WriteLine($"v {v.Vertex.X} {v.Vertex.Y} {v.Vertex.Z}");
                }

                fragVertsIndex[fragIndex] = fragVertsIndex.ElementAtOrDefault(fragIndex - 1) + frag.Vertices.Length;
            }

            // UVs
            for (int fragIndex = 0; fragIndex < mesh.MeshFrags.Length; fragIndex++)
            {
                MeshFrag frag = mesh.MeshFrags[fragIndex];

                foreach (Vec2 uv in frag.UVSets.Select(x => x.UVs[0]).Select(x => x.ToVec2(frag.UVScale)))
                {
                    objWriter.WriteLine($"vt {uv.X} {uv.Y}");
                }

                fragUVsIndex[fragIndex] = fragUVsIndex.ElementAtOrDefault(fragIndex - 1) + frag.UVSets.Length;
            }

            for (int fragIndex = 0; fragIndex < mesh.MeshFrags.Length; fragIndex++)
            {
                MeshFrag frag = mesh.MeshFrags[fragIndex];

                objWriter.WriteLine($"g group{fragIndex}");
                objWriter.WriteLine($"usemtl mtl{fragIndex}");

                // Get the material flags
                MaterialFlags matFlags = frag.MaterialFlags;

                // Set polygon indices. Unity doesn't support triangle strips anymore, so we need to convert it.
                int[] indices = matFlags.HasFlag(MaterialFlags.Tristrip)
                    ? MeshHelpers.TriStripToTriangles(frag.PolygonIndexBuffer, flip: true)
                    : frag.PolygonIndexBuffer.Select(x => (int)x).ToArray();

                // Duplicate triangles if double sided
                if (matFlags.HasFlag(MaterialFlags.DoubleSided))
                {
                    int origLength = indices.Length;
                    Array.Resize(ref indices, indices.Length * 2);

                    MeshHelpers.FlipTriIndices(indices, 0, origLength, origLength);
                }
                else if (!frag.MaterialFlags.HasFlag(MaterialFlags.Tristrip))
                {
                    MeshHelpers.FlipTriIndices(indices, 0, 0, indices.Length);
                }

                for (int i = 0; i < indices.Length; i += 3)
                {
                    objWriter.WriteLine("f {2}/{5}/ {1}/{4}/ {0}/{3}/",
                        fragVertsIndex.ElementAtOrDefault(fragIndex - 1) + indices[i + 0] + 1,
                        fragVertsIndex.ElementAtOrDefault(fragIndex - 1) + indices[i + 1] + 1,
                        fragVertsIndex.ElementAtOrDefault(fragIndex - 1) + indices[i + 2] + 1,
                        fragUVsIndex.ElementAtOrDefault(fragIndex - 1) + indices[i + 0] + 1,
                        fragUVsIndex.ElementAtOrDefault(fragIndex - 1) + indices[i + 1] + 1,
                        fragUVsIndex.ElementAtOrDefault(fragIndex - 1) + indices[i + 2] + 1);
                }
            }

            using StreamWriter mtlWriter = new StreamWriter(Path.Combine(exportDir, $"{name}.mtl"));

            HashSet<long> exportedTextures = new HashSet<long>();

            for (int fragIndex = 0; fragIndex < mesh.MeshFrags.Length; fragIndex++)
            {
                MeshFrag frag = mesh.MeshFrags[fragIndex];
                long texIndex = frag.TextureIndices.Length == 0 ? -1 : (long)frag.TextureIndices[0];

                mtlWriter.WriteLine($"newmtl mtl{fragIndex}");
                mtlWriter.WriteLine("Ka 0.00000 0.00000 0.00000");
                mtlWriter.WriteLine("Kd 0.50000 0.50000 0.50000");
                mtlWriter.WriteLine("Ks 0.00000 0.00000 0.00000");
                mtlWriter.WriteLine("d 1.00000");
                mtlWriter.WriteLine("illum 0");

                if (texIndex == -1)
                    continue;

                PsychonautsTexture tex = textures[texIndex];

                mtlWriter.WriteLine($"map_Kd {tex.GameTexture.FileName}.png");

                if (exportedTextures.Contains(texIndex))
                    continue;

                exportedTextures.Add(texIndex);

                tex.GetTexture(false).ExportPNG(Path.Combine(exportDir, $"{tex.GameTexture.FileName}.png"));
            }
        }
    }
}