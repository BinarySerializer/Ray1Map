using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using PsychoPortal;
using PsychoPortal.Unity;
using UnityEngine;
using Endian = BinarySerializer.Endian;
using Mesh = PsychoPortal.Mesh;
using BGRA8888Color = PsychoPortal.BGRA8888Color;

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
            convertOctree(pl2.VisibilityTree?.Octree);
            convertDomain(pl2.RootDomain);

            void convertOctree(Octree octree)
            {
                if (octree == null)
                    return;

                octree.Primitives = octree.PS2_Primitives.Select(x => (uint)x).ToArray();
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
            }

            void convertFrag(MeshFrag frag)
            {
                convertOctree(frag.Proto_Octree);
                InitPS2MeshFrag(loader.Context, frag, out _);

                if (frag.HasVertexStreamBasis != 0)
                {
                    frag.VertexStreamBasis = Enumerable.Range(0, frag.Vertices.Length).Select(x => new VertexStreamBasis
                    {
                        // TODO: Set more proper values
                        NormPacked1 = new NormPacked3(),
                        NormPacked2 = new NormPacked3(),
                        NormPacked3 = new NormPacked3()
                    }).ToArray();
                }
            }

            Binary.WriteToFile(pl2, outputPath, plbSettings, logger: logger);
        }

        #endregion

        #region Load

        public override PsychonautsMeshFrag LoadMeshFrag(Ray1MapLoader loader, MeshFrag meshFrag, Transform parent, int index, LoadedTexture[] textures, PsychonautsSkeleton[] skeletons, Matrix4x4[][] bindPoses, out GameObject fragObj)
        {
            PS2_GIF_Command[] parsedCommands;

            // Convert PS2 mesh data to common format
            try
            {
                InitPS2MeshFrag(loader.Context, meshFrag, out parsedCommands);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize PS2 mesh frag: {ex.Message}");
                fragObj = null;
                return null;
            }

            PsychonautsMeshFrag frag = base.LoadMeshFrag(loader, meshFrag, parent, index, textures, skeletons, bindPoses, out fragObj);

            if (parsedCommands?.Length > 0)
            {
                ObjectArray<PS2_GIF_Command> parsedCommandsArray = new()
                {
                    Pre_Length = parsedCommands.Length,
                    Value = parsedCommands,
                };
                parsedCommandsArray.Init(parsedCommands[0].Offset);
                fragObj.AddComponent<BinarySerializer.Unity.BinarySerializableDataComponent>(x => x.Data = parsedCommandsArray);
            }

            return frag;
        }

        private static int _meshFragGlobalIndex;
        public void InitPS2MeshFrag(Context context, MeshFrag meshFrag, out PS2_GIF_Command[] parsedCommands)
        {
            // Get optional settings for configuring the mesh frag conversion
            PS2MeshFragSettings settings = context.GetStoredObject<PS2MeshFragSettings>(PS2MeshFragSettingsKey);
            PS2MeshFragFlags flags = settings?.Flags ?? PS2MeshFragFlags.None;
            bool invertNormals = false;
            if (settings != null && meshFrag.TextureIndices.Length > 0)
                invertNormals = settings.InvertNormalsForTextures.Contains(meshFrag.TextureIndices[0]);

            string key = $"vif_{_meshFragGlobalIndex}";
            _meshFragGlobalIndex++;

            if (flags.HasFlag(PS2MeshFragFlags.CreateDummyColors))
                meshFrag.MaterialFlags |= MaterialFlags.Flag_6;

            // The PS2 version defines vertex colors even if they're not used (in which case they're set to 0),
            // so we need to determine when they should be used and not
            bool useVertexColors = meshFrag.MaterialFlags.HasFlag(MaterialFlags.Flag_6) ||
                                   meshFrag.MaterialFlags.HasFlag(MaterialFlags.Flag_15);

            int jointInfluencesPerVertex = meshFrag.AnimInfo?.InfluencesPerVertex ?? 0;

            try
            {
                // Serialize using BinarySerializer for now (Psychonauts normally uses PsychoPortal)
                context.AddFile(new StreamFile(context, key, new MemoryStream(meshFrag.PS2_VIFCommands), endianness: Endian.Little));
                PS2_VIFCommands cmds = FileFactory.Read<PS2_VIFCommands>(context, key);

                List<VertexNotexNorm> vertices = new();
                List<BGRA8888Color> vertexColors = useVertexColors ? new List<BGRA8888Color>() : null;
                List<UVSet> uvSets = new();
                List<SkinWeight> skinWeights = jointInfluencesPerVertex > 0 ? new List<SkinWeight>() : null;

                parsedCommands = cmds.ParseCommands(context, key, meshFrag.UVChannelsCount, jointInfluencesPerVertex).ToArray();

                // Enumerate every parsed command
                foreach (PS2_GIF_Command cmd in parsedCommands)
                {
                    // Add vertices and normals
                    vertices.AddRange(cmd.Cycles.Select(c => new VertexNotexNorm() 
                    {
                        Vertex = new Vec3(
                            c.Vertex.X - meshFrag.PS2_VertexOffset,
                            c.Vertex.Y - meshFrag.PS2_VertexOffset,
                            c.Vertex.Z - meshFrag.PS2_VertexOffset
                        ),
                        Normal = NormPacked3.FromVec3(invertNormals 
                            ? new Vec3(
                                c.Normal.XFloat * -1,
                                c.Normal.YFloat * -1,
                                c.Normal.ZFloat * -1)
                            : new Vec3(
                                c.Normal.XFloat,
                                c.Normal.YFloat,
                                c.Normal.ZFloat)),
                    }));

                    // Add vertex colors
                    if (flags.HasFlag(PS2MeshFragFlags.CreateDummyColors))
                        vertexColors?.AddRange(cmd.Cycles.Select(c => new BGRA8888Color()
                        {
                            Red = (byte)(c.Normal.XFloat * Byte.MaxValue),
                            Green = (byte)(c.Normal.YFloat * Byte.MaxValue),
                            Blue = (byte)(c.Normal.ZFloat * Byte.MaxValue),
                            Alpha = Byte.MaxValue
                        }));
                    else if ((flags.HasFlag(PS2MeshFragFlags.IgnoreColorsForFlag19) && meshFrag.MaterialFlags.HasFlag(MaterialFlags.Flag_19)) ||
                             flags.HasFlag(PS2MeshFragFlags.IgnoreColors) ||
                             (flags.HasFlag(PS2MeshFragFlags.IgnoreBlackColors) && cmd.Cycles.All(x => x.Color.RByte == 0 && x.Color.GByte == 0 && x.Color.BByte == 0)))
                        vertexColors?.AddRange(cmd.Cycles.Select(c => new BGRA8888Color()
                        {
                            Red = 127,
                            Green = 127,
                            Blue = 127,
                            Alpha = 255,
                        }));
                    else
                        vertexColors?.AddRange(cmd.Cycles.Select(c => new BGRA8888Color()
                        {
                            Red = c.Color.RByte,
                            Green = c.Color.GByte,
                            Blue = c.Color.BByte,
                            Alpha = c.Color.AByte
                        }));

                    // Add UVs
                    uvSets.AddRange(cmd.Cycles.Select(c => new UVSet()
                    {
                        UVs = c.UVs.Select(u => new UV()
                        {
                            Pre_Version = meshFrag.Pre_Version,
                            U = (short)Mathf.RoundToInt(u.UCorrected * 0x1000),
                            V = (short)Mathf.RoundToInt(u.VCorrected * 0x1000),
                        }).ToArray()
                    }));

                    // Add skin weight
                    if (jointInfluencesPerVertex > 0)
                    {
                        skinWeights?.AddRange(cmd.Cycles.Select(x => new SkinWeight
                        {
                            Joint1 = (int)(x.Vertex.JointOffset1 / 4),
                            Joint2 = (int)(x.Normal.JointOffset2 / 4),
                            Weight = new Vec2(x.UVs[0].SkinWeightCorrected, 1 - x.UVs[0].SkinWeightCorrected),
                        }));
                    }
                }

                // Flags
                meshFrag.MaterialFlags |= MaterialFlags.DoubleSided; // Force double-sided on PS2

                if (flags.HasFlag(PS2MeshFragFlags.RemoveFlag19))
                    meshFrag.MaterialFlags &= ~MaterialFlags.Flag_19;

                // Vertices and normals
                meshFrag.Vertices = vertices.ToArray();

                // Vertex colors
                if (useVertexColors)
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
                meshFrag.UVScale = 0x7FFF / 4096f;
                meshFrag.UVSets = uvSets.ToArray();

                // Skin weights
                if (jointInfluencesPerVertex > 0)
                    meshFrag.AnimInfo!.OriginalSkinWeights = skinWeights!.ToArray();

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

        public const string PS2MeshFragSettingsKey = nameof(PS2MeshFragSettings);

        [Flags]
        public enum PS2MeshFragFlags
        {
            None = 0,
            CreateDummyColors = 1 << 0,
            IgnoreColorsForFlag19 = 1 << 1,
            IgnoreColors = 1 << 2,
            IgnoreBlackColors = 1 << 3,
            RemoveFlag19 = 1 << 4,
        }

        public class PS2MeshFragSettings
        {
            public PS2MeshFragFlags Flags { get; set; }
            public int[] InvertNormalsForTextures { get; set; } // Used to fix hallway in cmbt map
        }

        #endregion
    }
}
