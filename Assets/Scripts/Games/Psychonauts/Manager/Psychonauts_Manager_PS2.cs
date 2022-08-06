using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.PS2;
using PsychoPortal;
using PsychoPortal.Unity;
using UnityEngine;
using Mesh = PsychoPortal.Mesh;
using RGBA8888Color = PsychoPortal.RGBA8888Color;

namespace Ray1Map.Psychonauts
{
    public class Psychonauts_Manager_PS2 : Psychonauts_Manager
    {
        #region Game Actions

        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return base.GetGameActions(settings).Concat(new GameAction[]
            {
                new("Export PS2 Files", false, true, (_, output) => ExportPS2Files(settings, output, false)),
                new("Export & Convert PS2 Files", false, true, (_, output) => ExportPS2Files(settings, output, true)),
                new("Generate PS2 File List", true, false, (input, _) => GeneratePS2FileList(settings, input)),
            }).ToArray();
        }

        public void GeneratePS2FileList(GameSettings settings, string sourceDirPath)
        {
            GameModeSelection[] ps2GameModes =
            {
                GameModeSelection.Psychonauts_PS2_EU, GameModeSelection.Psychonauts_PS2_US, GameModeSelection.Psychonauts_PS2_US_Demo
            };
            GameModeSelection[] gameModes =
            {
                GameModeSelection.Psychonauts_PC_Digital, GameModeSelection.Psychonauts_Xbox_Proto_20041217
            };
            Loader[] ps2Loaders = ps2GameModes.Select(x => new Ray1MapLoader(new PsychonautsSettings(GetVersion(x)), Settings.GameDirectories[x])).ToArray();
            Loader[] loaders = gameModes.Select(x => new Ray1MapLoader(new PsychonautsSettings(GetVersion(x)), Settings.GameDirectories[x])).ToArray();

            try
            {
                PS2.GenerateFileTable(ps2Loaders, loaders, sourceDirPath, GetLogger()).CopyToClipboard();
            }
            finally
            {
                foreach (Loader loader in ps2Loaders)
                    loader.Dispose();
                foreach (Loader loader in loaders)
                    loader.Dispose();
            }
        }

        public void ExportPS2Files(GameSettings settings, string outputPath, bool convertFiles)
        {
            using Ray1MapLoader loader = CreateLoader(settings);

            loader.LoadFilePackages(loader.Logger);

            ExportPS2Files(loader, loader.Logger, loader.FileManager.PS2_FileTable, outputPath, PS2_FileNames.FileTable.ToDictionary(PS2_FileEntry.GetFilePathHash), true, loader.FileManager.PS2_ResourcePacks, convertFiles);

            Debug.Log("Finished exporting");
        }

        public void ExportPS2Files(Loader loader, IBinarySerializerLogger logger, PS2_FileTable fileTable, string outputDir, Dictionary<uint, string> fileNames, bool categorizeUnnamed, Stream[] resourcePaks, bool convertFiles)
        {
            foreach (PS2_FileEntry file in fileTable.Files)
            {
                bool foundName = fileNames.TryGetValue(file.FilePathHash, out string n);
                string name = foundName ? n : $"{file.FilePathHash:X8}";

                byte[] fileBuffer;

                try
                {
                    fileBuffer = file.ReadFile(resourcePaks);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error reading file {name}: {ex.Message}");
                    continue;
                }

                if (fileBuffer == null)
                    continue;

                if (!foundName && fileNames.Count > 0)
                {
                    const string unnamedDir = "_unnamed";

                    if (categorizeUnnamed && fileBuffer.Length >= 4)
                    {
                        uint magic = BitConverter.ToUInt32(fileBuffer, 0);
                        string ext = null;
                        string type = null;

                        if (magic == 0x4150414B) // APAK
                        {
                            type = "Animation Packs";
                            ext = ".apf";
                        }
                        else if (magic == 0x45504241) // EPBA
                        {
                            type = "Blend Animations";
                            ext = ".pba";
                        }
                        else if (magic == 0x50455645) // PEVE
                        {
                            type = "Event Animations";
                            ext = ".eve";
                        }
                        else if (magic is 0x504A4158 or 0x504A414E) // PJAX or PJAN
                        {
                            type = "Joint Animations";
                            ext = ".ja2";
                        }
                        else if (magic == 0x50535943) // PSYC
                        {
                            type = "Meshes";
                            ext = ".pl2";
                        }
                        else if (magic == 0x4B415050) // PPAK
                        {
                            type = "Level Packs";
                            ext = ".ppf";
                        }
                        else if (magic == 0x61754C1B) // .LUA
                        {
                            type = "Scripts";
                            ext = ".lub";
                        }
                        else if (magic == 0x69746341) // Acti(onFile)
                        {
                            type = "Animation Action Files";
                            ext = ".asd";
                        }
                        else if (magic == 0x4543414D) // MACE
                        {
                            type = "Camera Paths";
                            ext = ".cam";
                        }
                        else if (magic == 0xBA010000)
                        {
                            type = "Videos";
                            ext = ".pss";
                        }
                        else if ((magic & 0xFFFF) == 0x4257) // WB
                        {
                            type = "Sounds";
                            ext = ".pwb";
                        }
                        else if ((magic & 0xFFFF) == 0x4253) // SB
                        {
                            type = "Sounds";
                            ext = ".psb";
                        }
                        else if ((magic & 0xFFFFFF) == 0x325350) // PS2
                        {
                            type = "Textures";
                            ext = ".ps2";
                        }

                        if (type != null && ext != null)
                            name = Path.Combine(unnamedDir, type, $"{name}{ext}");
                        else
                            name = Path.Combine(unnamedDir, name);
                    }
                    else
                    {
                        name = Path.Combine(unnamedDir, name);
                    }
                }

                string outputFile = Path.Combine(outputDir, name);

                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
                File.WriteAllBytes(outputFile, fileBuffer);

                if (convertFiles)
                {
                    string ext = Path.GetExtension(outputFile);

                    if (ext.Equals(".ps2", StringComparison.InvariantCultureIgnoreCase))
                    {
                        PS2_Texture ps2Tex = Binary.ReadFromBuffer<PS2_Texture>(fileBuffer, loader.Settings, onPreSerializing: (_, x) => x.Pre_IsPacked = false, logger: logger, name: name);

                        Texture2D tex = ps2Tex.ToTexture2D(flipY: true);
                        tex.ExportPNG(Path.ChangeExtension(outputFile, ".png"));
                    }
                }
            }
        }

        public void ExportPL2ToPLB(Ray1MapLoader loader, Scene pl2, PsychonautsSettings plbSettings, string outputPath, IBinarySerializerLogger logger = null)
        {
            convertScene(pl2);

            void convertOctree(Octree octree)
            {
                if (octree == null)
                    return;

                octree.Primitives = octree.PS2_Primitives.Select(x => (uint)x).ToArray();
            }

            void convertScene(Scene scene)
            {
                convertOctree(scene.VisibilityTree?.Octree);
                convertDomain(scene.RootDomain);
                
                foreach (Scene childScene in scene.ReferencedScenes)
                    convertScene(childScene);
            }

            void convertDomain(Domain domain)
            {
                foreach (Mesh mesh in domain.Meshes)
                    convertMesh(mesh);

                foreach (Domain childDomain in domain.Children)
                    convertDomain(childDomain);
            }

            void convertMesh(Mesh mesh)
            {
                convertOctree(mesh.CollisionTree?.Octree);

                foreach (MeshFrag frag in mesh.MeshFrags)
                    convertFrag(frag);

                foreach (Mesh childMesh in mesh.Children)
                    convertMesh(childMesh);

                // TODO: Fix this - related to vertex stream basis?
                mesh.MeshFrags = mesh.MeshFrags.Where(x => !x.MaterialFlags.HasFlag(MaterialFlags.Specular)).ToArray();
            }

            void convertFrag(MeshFrag frag)
            {
                convertOctree(frag.Proto_Octree);
                InitPS2MeshFrag(loader.Context, frag);

                // TODO: Removing the flag is not enough - probably need to remove some texture reference as well?
                // TODO: This is a temporary solution. Specular requires VertexStreamBasis to be set and I don't know how to generate that
                //frag.MaterialFlags &= ~MaterialFlags.Specular;

                // Is this correct? Ideally we want to rely on the lights when not used on a PS2.
                if (!frag.MaterialFlags.HasFlag(MaterialFlags.Flag_6))
                {
                    frag.HasVertexColors = 0;
                    frag.VertexColors = null;
                }
            }

            Binary.WriteToFile(pl2, outputPath, plbSettings, logger: logger);
        }

        #endregion

        #region Load

        public override PsychonautsMeshFrag LoadMeshFrag(Ray1MapLoader loader, MeshFrag meshFrag, Transform parent, int index, PsychonautsTexture[] textures, PsychonautsSkeleton[] skeletons, Matrix4x4[][] bindPoses)
        {
            // Convert PS2 mesh data to common format
            try
            {
                InitPS2MeshFrag(loader.Context, meshFrag);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to initialize PS2 mesh frag: {ex.Message}");
                return null;
            }

            return base.LoadMeshFrag(loader, meshFrag, parent, index, textures, skeletons, bindPoses);
        }

        public void InitPS2MeshFrag(Context context, MeshFrag meshFrag)
        {
            const string key = "geo";

            // Some helpful links:
            // https://psi-rockin.github.io/ps2tek
            // https://github.com/PCSX2/pcsx2/issues/1803 > https://github.com/Fireboyd78/driver-tools/tree/dev/GMC2Snooper/PS2
            // https://github.com/PCSX2/pcsx2/tree/master/pcsx2

            try
            {
                // Serialize using BinarySerializer for now (Psychonauts normally uses PsychoPortal)
                context.AddFile(new StreamFile(context, key, new MemoryStream(meshFrag.PS2_GeometryBuffer)));
                PS2_GeometryCommands cmds = FileFactory.Read<PS2_GeometryCommands>(context, key);

                List<VertexNotexNorm> vertices = new();
                List<RGBA8888Color> vertexColors = new();
                List<UVSet> uvSets = new();
                bool? hasVertexColors = null;
                bool? hasTextureMapping = null;

                // TODO: We might need to tri-strip each primitive separetly. Separate meshes? Or update indices.
                foreach (PS2_GeometryCommands.Primitive prim in cmds.EnumeratePrimitives())
                {
                    // Data verification
                    try
                    {
                        // Verify flags match
                        if (prim.GIFTag.PRIM.PrimitiveType == PRIM_PrimitiveType.TriangleStrip !=
                            meshFrag.MaterialFlags.HasFlag(MaterialFlags.Tristrip))
                            throw new Exception("Triangle strip flags are not set correctly");

                        if (prim.GIFTag.PRIM.IIP == PRIM_IIP.FlatShading)
                            throw new Exception("Flat shading is used");

                        // Verify data
                        if (prim.Vertices == null)
                            throw new Exception("No vertices were read");
                        if (prim.Normals == null)
                            throw new Exception("No normals were read");

                        hasVertexColors ??= prim.VertexColors != null;
                        if ((prim.VertexColors != null) != hasVertexColors)
                            throw new Exception("Shading flag doesn't match previous primitives");

                        hasTextureMapping ??= prim.GIFTag.PRIM.TME;
                        if (prim.GIFTag.PRIM.TME != (prim.UVs != null))
                            throw new Exception("Texture mapping flag doesn't match data");
                        else if (prim.GIFTag.PRIM.TME != hasTextureMapping)
                            throw new Exception("Texture mapping flag doesn't match previous primitives");

                        List<int> lengths = new();

                        lengths.Add(prim.GIFTag.NLOOP);
                        lengths.Add(prim.Vertices.Length);
                        lengths.Add(prim.Normals.Length);

                        if (hasVertexColors == true)
                            lengths.Add(prim.VertexColors.Length);
                        if (hasTextureMapping == true)
                            lengths.AddRange(prim.UVs.Where(x => x != null).Select(x => x.Length));

                        // Make sure all lengths match
                        if (lengths.Distinct().Count() != 1)
                            throw new Exception("Some data lengths don't match");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Primitive error: {ex.Message}{Environment.NewLine}{ex}");
                        break;
                    }

                    int length = prim.GIFTag.NLOOP;

                    // Add vertices and normals
                    for (int i = 0; i < length; i++)
                    {
                        Vec3 vertex = prim.Vertices[i];
                        Vec3 normal = prim.Normals[i];

                        vertices.Add(new VertexNotexNorm()
                        {
                            Vertex = vertex,
                            Normal = new NormPacked3(), // TODO: Set normal. We need to compress it to the packed format.
                        });
                    }

                    // Add vertex colors
                    if (hasVertexColors == true)
                        vertexColors.AddRange(prim.VertexColors);

                    // Add UVs
                    if (hasTextureMapping == true)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            uvSets.Add(new UVSet()
                            {
                                UVs = prim.UVs.Select(x => x[i]).ToArray(),
                            });
                        }
                    }
                }

                // Flags
                meshFrag.MaterialFlags |= MaterialFlags.DoubleSided; // Force double-sided on PS2

                // Vertices
                meshFrag.Vertices = vertices.ToArray();

                // Vertex colors
                if (hasVertexColors == true)
                {
                    meshFrag.HasVertexColors = 1;
                    meshFrag.VertexColors = vertexColors.ToArray();
                }
                else
                {
                    meshFrag.HasVertexColors = 0;
                    meshFrag.VertexColors = null;
                }

                // UVs
                if (hasTextureMapping == true)
                {
                    meshFrag.UVSetUVsCount = (uint)uvSets[0].UVs.Length;
                    meshFrag.UVScale = 1;
                    meshFrag.UVSets = uvSets.ToArray();
                }
                else
                {
                    meshFrag.UVSetUVsCount = 0;
                    meshFrag.UVScale = 1;
                    meshFrag.UVSets = null;
                }

                meshFrag.HasVertexStreamBasis = 0;
                meshFrag.VertexStreamBasis = null;

                // Polygons
                meshFrag.PolygonCount = (uint)(meshFrag.MaterialFlags.HasFlag(MaterialFlags.Tristrip) 
                    ? meshFrag.Vertices.Length - 2 
                    : meshFrag.Vertices.Length / 3);
                meshFrag.DegenPolygonCount = 0;
                meshFrag.PolygonIndexBuffer = Enumerable.Range(0, meshFrag.Vertices.Length).Select(x => (short)x).ToArray();
            }
            finally
            {
                context.RemoveFile(key);
            }
        }

        #endregion
    }
}
